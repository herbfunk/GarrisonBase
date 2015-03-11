using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bots.Grind;
using Bots.Professionbuddy.Dynamic;
using Buddy.Coroutines;
using Herbfunk.GarrisonBase.Cache;
using Herbfunk.GarrisonBase.Cache.Enums;
using Herbfunk.GarrisonBase.Garrison;
using Herbfunk.GarrisonBase.Garrison.Enums;
using Herbfunk.GarrisonBase.Garrison.Objects;
using Styx;
using Styx.Common.Helpers;
using Styx.CommonBot;
using Styx.CommonBot.Coroutines;
using Styx.CommonBot.Frames;
using Styx.CommonBot.Routines;
using Styx.Pathing;
using Styx.TreeSharp;
using Styx.WoWInternals;

namespace Herbfunk.GarrisonBase
{
    public partial class Coroutines
    {



        internal static void Reset()
        {
            CacheStaticLookUp.Reset();
        }

        private static Composite _deathBehavior;
        private static Composite _lootBehavior;
        private static Composite _combatBehavior;
        private static Composite _vendorBehavior;
        internal static Composite LootBehavior
        {
            get { return _lootBehavior ?? (_lootBehavior = LevelBot.CreateLootBehavior()); }
        }
        internal static Composite DeathBehavior
        {
            get { return _deathBehavior ?? (_deathBehavior = LevelBot.CreateDeathBehavior()); }
        }
        internal static Composite CombatBehavior
        {
            get { return _combatBehavior ?? (_combatBehavior = LevelBot.CreateCombatBehavior()); }
        }
        internal static Composite VendorBehavior
        {
            get { return _vendorBehavior ?? (_vendorBehavior = LevelBot.CreateVendorBehavior()); }
        }


        internal static List<Behaviors.Behavior> Behaviors = new List<Behaviors.Behavior>();
        internal static Behaviors.Behavior CurrentBehavior = null;
        private static Behaviors.BehaviorMine _mineQuest;
        private static Behaviors.BehaviorHerb _herbQuest;

        private static bool _checkedFirstQuests = false;
        private static Behaviors.BehaviorPrechecks PreChecks = new Behaviors.BehaviorPrechecks();
        public static async Task<bool> RootLogic()
        {
            if (!CacheStaticLookUp.InitalizedCache)
            {
                CacheStaticLookUp.Update();

                if (Player.IsAlliance)
                {
                    _mineQuest = new Behaviors.BehaviorMine(MovementCache.Alliance_Mine_LevelOne);
                    _herbQuest = new Behaviors.BehaviorHerb(MovementCache.Alliance_Herb_LevelOne);
                }
                else
                {
                    _mineQuest = new Behaviors.BehaviorMine(MovementCache.Horde_Mine_LevelOne);
                    _herbQuest = new Behaviors.BehaviorHerb(MovementCache.Horde_Herb_LevelOne);
                }
                _mineQuest.Initalize();
                _herbQuest.Initalize();
            }


            //if (!StyxWoW.Me.IsAlive || StyxWoW.Me.Combat || RoutineManager.Current.NeedRest)
            //    return false;

            if (await PreCheckCoroutines())
                return true;

            if (await PreChecks.BehaviorRoutine()) 
                return true;

            //Simple Check if garrison can be accessed!
            if (Player.Level < 90 || Player.Inventory.GarrisonHearthstone == null)
            {
                GarrisonBase.Log("No access to garrison!");
                TreeRoot.Stop("No Access to Garrison");
                return false;
            }

            if (!GarrisonManager.Initalized)
            {
                await CommonCoroutines.WaitForLuaEvent("GARRISON_SHOW_LANDING_PAGE", 2500, null, LuaCommands.ClickGarrisonMinimapButton);
                await CommonCoroutines.SleepForRandomUiInteractionTime();
                await Coroutine.Sleep(StyxWoW.Random.Next(1234,2331));
                Lua.DoString("GarrisonLandingPage.CloseButton:Click()");
                await CommonCoroutines.SleepForRandomUiInteractionTime();

                GarrisonManager.Initalize();

                if (!LuaEvents.LuaAddonInjected)
                {
                    if (LuaCommands.TestLuaInjectionCode())
                    {//Prevent multiple injections by checking simple function return!
                        LuaEvents.LuaAddonInjected = true;
                    }
                    else
                    {
                        await LuaEvents.InjectLuaAddon();
                        return true;
                    }
                }

                return true;
            }

            if (ObjectCacheManager.ShouldUpdateObjectCollection) ObjectCacheManager.UpdateCache();

            if (!_checkedFirstQuests)
            {
                if (!GarrisonManager.Buildings[BuildingType.Mines].FirstQuestCompleted && Player.Level >= 92)
                {
                    if (await _mineQuest.BehaviorRoutine())
                        return true;
                }

                if (!GarrisonManager.Buildings[BuildingType.HerbGarden].FirstQuestCompleted && Player.Level >= 96)
                {
                    if (await _herbQuest.BehaviorRoutine())
                        return true;
                }


                _checkedFirstQuests = true;
            }
            

            if (!_initalizedBehaviorList)
            {
                InitalizeBehaviorsList();
            }


            //Check for next behavior
            if (CurrentBehavior == null)
            {
                while (Behaviors.Count > 0)
                {
                    if (!Behaviors[0].CheckCriteria())
                        Behaviors.RemoveAt(0);
                    else
                    {
                        CurrentBehavior = Behaviors[0];
                        CurrentBehavior.Initalize();
                        break;
                    }
                }
            }




            if (CurrentBehavior != null)
            {
                bool x = await CurrentBehavior.BehaviorRoutine();

                if (!x)
                {
                    GarrisonBase.Debug("Behavior {0} returned false!", CurrentBehavior.Type);
                    Behaviors.RemoveAt(0);
                    CurrentBehavior = null;
                }

                return true;
            }



            TreeRoot.StatusText = "GarrisonBase is finished!";
            TreeRoot.Stop();
            return false;
        }


        internal static bool _initalizedBehaviorList = false;
        internal static void InitalizeBehaviorsList()
        {
            //Insert new missions behavior at beginning!
            Behaviors.Clear();
            CurrentBehavior = null;
            
            //Move to entrance!
            //Behaviors.Add(new Behaviors.BehaviorMove(MovementCache.GarrisonEntrance, 7f));
            Behaviors.Add(new Behaviors.BehaviorGetMail());

            Behaviors.Add(new Behaviors.BehaviorMissions()); //Mission Complete
            Behaviors.Add(new Behaviors.BehaviorCache()); //Garrison Cache

            //Finalize Plots
            foreach (var b in GarrisonManager.Buildings.Values.Where(b => b.CanActivate))
                Behaviors.Add(new Behaviors.BehaviorFinalizePlots(b));


            //Buildings that are active but have not completed the quest yet (that are setup already)
            #region Building First Quest Behaviors
            foreach (var b in GarrisonManager.Buildings.Values.Where(b => !b.FirstQuestCompleted && !b.IsBuilding && b.QuestNpcID > -1 && b.FirstQuestID > -1))
            {
                if (b.Type == BuildingType.SalvageYard)
                {
                    //var abandon = new Behaviors.BehaviorQuestAbandon(b.FirstQuestID);
                    var pickup = new Behaviors.BehaviorQuestPickup(b.FirstQuestID, b.SafeMovementPoint, b.WorkOrderNPCEntryId);
                    var itemInteraction = new Behaviors.BehaviorItemInteraction(118473);
                    var turnin = new Behaviors.BehaviorQuestTurnin(b.FirstQuestID, b.SafeMovementPoint, b.WorkOrderNPCEntryId);
                    var behaviorArray = new Behaviors.BehaviorArray(new Behaviors.Behavior[] { pickup, itemInteraction, turnin });
                    behaviorArray.Criteria += () => BaseSettings.CurrentSettings.BehaviorQuests;
                    Behaviors.Add(behaviorArray);
                }
                else if (b.Type == BuildingType.TradingPost)
                {
                    var pickup = new Behaviors.BehaviorQuestPickup(b.FirstQuestID, b.SafeMovementPoint, b.WorkOrderNPCEntryId);
                    var moveLoc = Player.IsAlliance
                        ? new WoWPoint(1766.308, 151.4053, 75.89236)
                        : new WoWPoint(5745.101, 4570.491, 138.8332);

                    var moveto = new Behaviors.BehaviorMove(moveLoc, 7f);
                    var npcId = Player.IsAlliance ? 87288 : 87260;
                    var target = new Behaviors.BehaviorSelectTarget(Convert.ToUInt32(npcId));
                    var interact = new Behaviors.BehaviorItemInteraction(118418, true);
                    var turnin = new Behaviors.BehaviorQuestTurnin(b.FirstQuestID, b.SafeMovementPoint, b.WorkOrderNPCEntryId);

                    var behaviorArray = new Behaviors.BehaviorArray(new Behaviors.Behavior[] { pickup, moveto, target, interact, turnin });
                    behaviorArray.Criteria += () => BaseSettings.CurrentSettings.BehaviorQuests;
                    Behaviors.Add(behaviorArray);
                }
                else if(b.Type != BuildingType.Mines && b.Type != BuildingType.HerbGarden)
                {
                    var pickup = new Behaviors.BehaviorQuestPickup(b.FirstQuestID, b.SafeMovementPoint, b.QuestNpcID);
                    var workorder = new Behaviors.BehaviorQuestWorkOrder(b);
                    var workorderPickup = new Behaviors.BehaviorWorkOrderPickUp(b);
                    var turnin = new Behaviors.BehaviorQuestTurnin(b.FirstQuestID, b.SafeMovementPoint, b.WorkOrderNPCEntryId);
                    var behaviorArray = new Behaviors.BehaviorArray(new Behaviors.Behavior[] { pickup, workorder, workorderPickup, turnin });
                    behaviorArray.Criteria += () => BaseSettings.CurrentSettings.BehaviorQuests;
                    Behaviors.Add(behaviorArray);
                }
            }

            #endregion

            #region Herbing and Mining

            if (GarrisonManager.Buildings.Values.Any(b => b.Type == BuildingType.Mines && b.FirstQuestCompleted && !b.IsBuilding && !b.CanActivate))
            {
                if (Player.IsAlliance)
                {
                    Behaviors.Add(
                        new Behaviors.BehaviorMine(
                            GarrisonManager.Buildings[BuildingType.Mines].Level == 1 ? MovementCache.Alliance_Mine_LevelOne :
                            GarrisonManager.Buildings[BuildingType.Mines].Level == 2 ? MovementCache.Alliance_Mine_LevelTwo :
                            MovementCache.Alliance_Mine_LevelThree));
                }
                else
                {
                    Behaviors.Add(
                        new Behaviors.BehaviorMine(
                            GarrisonManager.Buildings[BuildingType.Mines].Level == 1 ? MovementCache.Horde_Mine_LevelOne :
                            GarrisonManager.Buildings[BuildingType.Mines].Level == 2 ? MovementCache.Horde_Mine_LevelTwo :
                            MovementCache.Horde_Mine_LevelThree));
                }

                Behaviors.Add(new Behaviors.BehaviorWorkOrderPickUp(GarrisonManager.Buildings[BuildingType.Mines]));
                Behaviors.Add(new Behaviors.BehaviorWorkOrderStartUp(GarrisonManager.Buildings[BuildingType.Mines]));
            }

            if (GarrisonManager.Buildings.Values.Any(b => b.Type == BuildingType.HerbGarden && b.FirstQuestCompleted && !b.IsBuilding && !b.CanActivate))
            {
                if (Player.IsAlliance)
                {
                    Behaviors.Add(new Behaviors.BehaviorHerb(MovementCache.Alliance_Herb_LevelOne));
                }
                else
                {
                    Behaviors.Add(new Behaviors.BehaviorHerb(MovementCache.Horde_Herb_LevelOne));
                }

                Behaviors.Add(new Behaviors.BehaviorWorkOrderPickUp(GarrisonManager.Buildings[BuildingType.HerbGarden]));
                Behaviors.Add(new Behaviors.BehaviorWorkOrderStartUp(GarrisonManager.Buildings[BuildingType.HerbGarden]));
            }
            
            #endregion

            //Profession Crafting
            foreach (var skill in Player.Professions.ProfessionSkills)
            {
                int[] spellIds = PlayerProfessions.ProfessionDailyCooldownSpellIds[skill];
                Behaviors.Add(new Behaviors.BehaviorCraftingProfession(skill, spellIds[1]));
                Behaviors.Add(new Behaviors.BehaviorCraftingProfession(skill, spellIds[0]));
            }

            //Salvaging Behavior
            if (GarrisonManager.Buildings.Values.Any(b => b.Type == BuildingType.SalvageYard && !b.IsBuilding))
            {
                var b = GarrisonManager.Buildings.Values.First(building => building.Type == BuildingType.SalvageYard);
                Behaviors.Add(new Behaviors.BehaviorSalvage(b));

                //Add Sell and Repair behavior afterwards..
                Behaviors.Add(new Behaviors.BehaviorSellRepair(GarrisonManager.SellRepairNpcId, MovementCache.SellRepairNpcLocation));
            }

            //Work Order Pickup and Startup (and Mine/Herb)
            foreach (var b in GarrisonManager.Buildings.Values.Where(b => b.FirstQuestID <= 0 || b.FirstQuestCompleted).OrderBy(b => b.Plot))
            {
                if (b.Type != BuildingType.Mines || b.Type != BuildingType.HerbGarden)
                {

                    if (b.WorkOrder != null && !b.IsBuilding)
                    {
                        if (b.Type == BuildingType.TradingPost)
                        {
                            Behaviors.Add(new Behaviors.BehaviorWorkOrderPickUp(b));
                            Behaviors.Add(new Behaviors.BehaviorTradePost(b));
                        }
                        else
                        {
                            Behaviors.Add(new Behaviors.BehaviorWorkOrderPickUp(b));
                            Behaviors.Add(new Behaviors.BehaviorWorkOrderStartUp(b));
                            if (b.Type == BuildingType.EnchantersStudy)
                                Behaviors.Add(new Behaviors.BehaviorDisenchant()); //Disenchanting!
                        }
                    }
                }
            }

            //Sell and Repair
            Behaviors.Add(new Behaviors.BehaviorSellRepair(GarrisonManager.SellRepairNpcId, MovementCache.SellRepairNpcLocation));

            //Primal Spirit Exchange
            Behaviors.Add(new Behaviors.BehaviorPrimalTrader());

            //Send any mail..
            Behaviors.Add(new Behaviors.BehaviorSendMail());

            //Finally, start some new missions!
            Behaviors.Add(new Behaviors.BehaviorNewMissions());

            _initalizedBehaviorList = true;
        }


        internal static async Task<bool> CheckLootFrame()
        {
            // loot everything.
            if (!LuaEvents.LootOpened) return false;

            var lootSlotInfos = new List<LootSlotInfo>();
            for (int i = 0; i < LootFrame.Instance.LootItems; i++)
            {
                lootSlotInfos.Add(LootFrame.Instance.LootInfo(i));
            }

            if (await Coroutine.Wait(2000, () =>
            {
                LootFrame.Instance.LootAll();
                return !LuaEvents.LootOpened;
            }))
            {
                GarrisonBase.Log("Succesfully looted: ");
                foreach (LootSlotInfo lootinfo in lootSlotInfos)
                {
                    try
                    {
                        string lootQuanity = lootinfo.LootQuantity.ToString();
                        string lootName = lootinfo.LootName;
                        GarrisonBase.Log(lootQuanity + "x " + lootName);
                    }
                    catch 
                    {
                        GarrisonBase.Log("exception occured");
                    }
                    
                }
            }
            else
            {
                GarrisonBase.Err("Failed to loot from Frame.");
            }
            await CommonCoroutines.SleepForLagDuration();
            return true;
        }

        internal static async Task<bool> PreCheckCoroutines()
        {
            if (ObjectCacheManager.ShouldUpdateObjectCollection)
                ObjectCacheManager.UpdateCache();

            if (await DeathBehavior.ExecuteCoroutine())
                return true;

            if (StyxWoW.Me.Combat && await CombatBehavior.ExecuteCoroutine())
                return true;

            if (await VendorBehavior.ExecuteCoroutine())
                return true;

            if (await CheckLootFrame())
                return true;

            if (await LootBehavior.ExecuteCoroutine())
                return true;

            if (await LootObject())
                return true;

            return false;
        }

        private static Movement _lootMovement;
        internal static async Task<bool> LootObject()
        {
            if (ObjectCacheManager.ShouldUpdateObjectCollection)
                ObjectCacheManager.UpdateCache();

            if (!ObjectCacheManager.ShouldLoot || ObjectCacheManager.LootableObject == null)
            {
                _lootMovement = null;
                return false;
            }

            if (!ObjectCacheManager.LootableObject.IsValid)
            {
                _lootMovement = null;
                return false;
            }

            if (_lootMovement == null)
            {
                _lootMovement = new Movement(ObjectCacheManager.LootableObject.Location,
                    ObjectCacheManager.LootableObject.InteractRange);
            }
            else if (_lootMovement.CurrentMovementQueue.Count == 0)
            {
                _lootMovement = new Movement(ObjectCacheManager.LootableObject.Location,
                    ObjectCacheManager.LootableObject.InteractRange-=0.25f);
            }

            TreeRoot.StatusText = String.Format("Behavior Looting Movement {0}", ObjectCacheManager.LootableObject.Name);
            MoveResult result = await _lootMovement.MoveTo_Result();

            if (ObjectCacheManager.LootableObject.WithinInteractRange)
            {
                TreeRoot.StatusText = String.Format("Behavior Looting {0}", ObjectCacheManager.LootableObject.Name);
                if (StyxWoW.Me.IsMoving)
                {
                    await CommonCoroutines.StopMoving();
                }

                await CommonCoroutines.SleepForLagDuration();
                //await Coroutine.Sleep(StyxWoW.Random.Next(1001, 1999));

                if (ObjectCacheManager.LootableObject.IsValid)
                {
                    bool success = await CommonCoroutines.WaitForLuaEvent(
                    "LOOT_OPENED",
                    7500,
                    () => LuaEvents.LootOpened,
                    ObjectCacheManager.LootableObject.Interact);

                    if (success)
                    {
                        await CheckLootFrame();
                        ObjectCacheManager.LootableObject.NeedsRemoved = true;
                        ObjectCacheManager.LootableObject.BlacklistType = BlacklistType.Guid;
                        _lootMovement = null;
                    }
                }

                return true;
            }


            if (result == MoveResult.Failed)
            {
                GarrisonBase.Debug("Behavior Looting Movement FAILED for {0}", ObjectCacheManager.LootableObject.Name);
                ObjectCacheManager.LootableObject.IgnoredTimer = WaitTimer.TenSeconds;
                ObjectCacheManager.LootableObject = null;
                _lootMovement = null;
                return false;
            }

            return true;
        }
        

    }
}
