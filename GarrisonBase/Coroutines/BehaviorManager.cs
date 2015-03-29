using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Herbfunk.GarrisonBase.Cache;
using Herbfunk.GarrisonBase.Character;
using Herbfunk.GarrisonBase.Coroutines.Behaviors;
using Herbfunk.GarrisonBase.Garrison;
using Herbfunk.GarrisonBase.Garrison.Enums;
using Herbfunk.GarrisonBase.Garrison.Objects;
using Herbfunk.GarrisonBase.Quest;
using Styx;
using Styx.CommonBot;
using Styx.CommonBot.Coroutines;
using Styx.WoWInternals;

namespace Herbfunk.GarrisonBase.Coroutines
{
    public class BehaviorManager
    {
        public static Behavior CurrentBehavior { get; private set; }
        internal static List<Behavior> Behaviors = new List<Behavior>();

        /// <summary>
        /// Used to switch the current behavior
        /// </summary>
        public static Behavior SwitchBehavior { get; set; }



        public static async Task<bool> RootLogic()
        {

            if (!CacheStaticLookUp.InitalizedCache)
            {
                CacheStaticLookUp.Update();
                ObjectCacheManager.Initalize();
                //Simple Check if garrison can be accessed!
                if (Player.Level < 90 || Player.Inventory.GarrisonHearthstone == null)
                {
                    GarrisonBase.Log("No access to garrison!");
                    TreeRoot.Stop("No Access to Garrison");
                    return false;
                }
            }

            if (await Common.CheckCommonCoroutines())
                return true;


            if (!BaseSettings.CurrentSettings.DEBUG_IGNOREHEARTHSTONE && await Common.PreChecks.BehaviorRoutine())
                return true;



            if (!GarrisonManager.Initalized)
            {
                await CommonCoroutines.WaitForLuaEvent("GARRISON_SHOW_LANDING_PAGE", 2500, null, LuaCommands.ClickGarrisonMinimapButton);
                await CommonCoroutines.SleepForRandomUiInteractionTime();
                await Coroutine.Sleep(StyxWoW.Random.Next(1234, 2331));
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



            if (!InitalizedBehaviorList)
                InitalizeBehaviorsList();



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
                if (SwitchBehavior != null && CurrentBehavior != SwitchBehavior)
                {
                    if (!SwitchBehavior.CheckCriteria())
                    {
                        SwitchBehavior = null;
                    }
                    else
                    {
                        GarrisonBase.Debug("Switching behaviors to {0}", SwitchBehavior.Type);
                        CurrentBehavior = SwitchBehavior;
                        CurrentBehavior.Initalize();
                    }
                }

                bool x = await CurrentBehavior.BehaviorRoutine();

                if (!x || CurrentBehavior.IsDone)
                {
                    if (!x)
                        GarrisonBase.Debug("Finishing Behavior {0} because it returned false!", CurrentBehavior.Type);
                    else
                        GarrisonBase.Debug("Finishing Behavior {0} because IsDone is true", CurrentBehavior.Type);

                    if (!CurrentBehavior.Disposed) CurrentBehavior.Dispose();

                    if (SwitchBehavior != null && CurrentBehavior.Equals(SwitchBehavior))
                    {
                        SwitchBehavior = null;
                        CurrentBehavior = Behaviors[0];
                        CurrentBehavior.Initalize();
                    }
                    else
                    {
                        Behaviors.RemoveAt(0);
                        CurrentBehavior = null;
                    }

                }




                return true;
            }



            TreeRoot.StatusText = "GarrisonBase is finished!";
            TreeRoot.Stop();
            return false;
        }


        internal static bool InitalizedBehaviorList = false;
        internal static void InitalizeBehaviorsList()
        {
            //Insert new missions behavior at beginning!
            Behaviors.Clear();
            CurrentBehavior = null;
            SwitchBehavior = null;

            //Behaviors.Add(Follower.FollowerQuestBehaviorArray(189));
            //Behaviors.Add(Follower.FollowerQuestBehaviorArray(193));
            //Behaviors.Add(Follower.FollowerQuestBehaviorArray(207));

            //Move to entrance!
            //Behaviors.Add(new Behaviors.BehaviorMove(MovementCache.GarrisonEntrance, 7f));
            Behaviors.Add(new BehaviorGetMail());

            Behaviors.Add(new BehaviorMissionComplete()); //Mission Complete
            Behaviors.Add(new BehaviorCache()); //Garrison Cache

            //Finalize Plots
            foreach (var b in GarrisonManager.Buildings.Values.Where(b => b.CanActivate))
                Behaviors.Add(new BehaviorFinalizePlots(b));


            //Buildings that are active but have not completed the quest yet (that are setup already)
            #region Building First Quest Behaviors
            foreach (var b in GarrisonManager.Buildings.Values.Where(b => !b.FirstQuestCompleted && !b.IsBuilding && b.QuestNpcID > -1 && b.FirstQuestID > -1))
            {
                if (b.Type == BuildingType.SalvageYard)
                {
                    //var abandon = new Behaviors.BehaviorQuestAbandon(b.FirstQuestID);
                    var pickup = new BehaviorQuestPickup(b.FirstQuestID, b.SafeMovementPoint, b.WorkOrderNPCEntryId);
                    var itemInteraction = new BehaviorItemInteraction(118473);
                    var turnin = new BehaviorQuestTurnin(b.FirstQuestID, b.SafeMovementPoint, b.WorkOrderNPCEntryId);
                    var behaviorArray = new BehaviorArray(new Behavior[] { pickup, itemInteraction, turnin });
                    behaviorArray.Criteria += () => BaseSettings.CurrentSettings.BehaviorQuests;
                    Behaviors.Add(behaviorArray);
                }
                else if (b.Type == BuildingType.TradingPost)
                {
                    var pickup = new BehaviorQuestPickup(b.FirstQuestID, b.SafeMovementPoint, b.WorkOrderNPCEntryId);
                    var moveLoc = Player.IsAlliance
                        ? new WoWPoint(1764.08, 150.39, 76.02)
                        : new WoWPoint(5745.101, 4570.491, 138.8332);

                    var moveto = new BehaviorMove(moveLoc, 7f);
                    var npcId = Player.IsAlliance ? 87288 : 87260;
                    var target = new BehaviorSelectTarget(moveLoc);
                    var interact = new BehaviorItemInteraction(118418, true);
                    var turnin = new BehaviorQuestTurnin(b.FirstQuestID, b.SafeMovementPoint, b.WorkOrderNPCEntryId);

                    var behaviorArray = new BehaviorArray(new Behavior[] { pickup, moveto, target, interact, turnin });
                    behaviorArray.Criteria += () => BaseSettings.CurrentSettings.BehaviorQuests;
                    Behaviors.Add(behaviorArray);
                }
                else if (b.Type == BuildingType.Storehouse)
                {
                    var pickup = new BehaviorQuestPickup(b.FirstQuestID, b.EntranceMovementPoint, b.QuestNpcID);

                    List<WoWPoint> _hotSpots = new List<WoWPoint>
                    {
                        MovementCache.GarrisonEntrance,

                        MovementCache.GardenPlot63SafePoint,
                        MovementCache.MinePlot59SafePoint,

                        MovementCache.MediumPlot22SafePoint,
                        MovementCache.LargePlot23SafePoint,
                        MovementCache.LargePlot24SafePoint,
                        MovementCache.MediumPlot25SafePoint,

                        MovementCache.SmallPlot18SafePoint,
                        MovementCache.SmallPlot19SafePoint,
                        MovementCache.SmallPlot20SafePoint,
                    };

                    var looting = new BehaviorHotspotRunning(b.FirstQuestID, _hotSpots.ToArray(), BehaviorHotspotRunning.HotSpotType.Looting, () => HasQuestAndNotCompleted(b.FirstQuestID));
                    var turnin = new BehaviorQuestTurnin(b.FirstQuestID, b.EntranceMovementPoint, b.QuestNpcID);

                    var behaviorArray = new BehaviorArray(new Behavior[] { pickup, looting, turnin });
                    behaviorArray.Criteria += () => BaseSettings.CurrentSettings.BehaviorQuests;
                    Behaviors.Add(behaviorArray);
                }
                else if (b.Type == BuildingType.Lumbermill)
                {
                    var pickup = new BehaviorQuestPickup(b.FirstQuestID, b.EntranceMovementPoint, b.QuestNpcID);

                    WoWPoint movementPoint;
                    if (Player.IsAlliance)
                        movementPoint = new WoWPoint(1555.087, 173.8229, 72.59766);
                    else
                        movementPoint = new WoWPoint(6082.979, 4795.821, 149.1655);

                    var looting = new BehaviorHotspotRunning(b.FirstQuestID, new[] { movementPoint }, new uint[] { 234021, 233922 }, BehaviorHotspotRunning.HotSpotType.Looting, () => HasQuestAndNotCompleted(b.FirstQuestID));
                    var turnin = new BehaviorQuestTurnin(b.FirstQuestID, b.EntranceMovementPoint, b.QuestNpcID);
                    var behaviorArray = new BehaviorArray(new Behavior[] { pickup, looting, turnin });
                    behaviorArray.Criteria += () => BaseSettings.CurrentSettings.BehaviorQuests;
                    Behaviors.Add(behaviorArray);
                }
                else if (b.Type == BuildingType.Mines)
                {
                    var pickup = new BehaviorQuestPickup(b.FirstQuestID, b.EntranceMovementPoint, b.QuestNpcID);

                    var looting = new BehaviorHotspotRunning(
                        b.FirstQuestID,
                        Player.IsAlliance ?
                        MovementCache.Alliance_Mine_LevelOne.ToArray() :
                        MovementCache.Horde_Mine_LevelOne.ToArray(),
                        CacheStaticLookUp.MineQuestMobIDs.ToArray(),
                        BehaviorHotspotRunning.HotSpotType.Killing,
                        () => HasQuestAndNotCompleted(b.FirstQuestID));

                    var turnin = new BehaviorQuestTurnin(b.FirstQuestID, b.EntranceMovementPoint, b.QuestNpcID);

                    var behaviorArray = new BehaviorArray(new Behavior[] { pickup, looting, turnin });
                    behaviorArray.Criteria += () => BaseSettings.CurrentSettings.BehaviorQuests;
                    Behaviors.Add(behaviorArray);
                }
                else if (b.Type == BuildingType.HerbGarden)
                {
                    var pickup = new BehaviorQuestPickup(b.FirstQuestID, b.EntranceMovementPoint, b.QuestNpcID);

                    var looting = new BehaviorHotspotRunning(
                        b.FirstQuestID,
                        Player.IsAlliance ?
                        MovementCache.Alliance_Herb_LevelOne.ToArray() :
                        MovementCache.Horde_Herb_LevelOne.ToArray(),
                        CacheStaticLookUp.HerbQuestMobIDs.ToArray(),
                        BehaviorHotspotRunning.HotSpotType.Killing,
                        () => HasQuestAndNotCompleted(b.FirstQuestID));

                    var turnin = new BehaviorQuestTurnin(b.FirstQuestID, b.EntranceMovementPoint, b.QuestNpcID);

                    var behaviorArray = new BehaviorArray(new Behavior[] { pickup, looting, turnin });
                    behaviorArray.Criteria += () => BaseSettings.CurrentSettings.BehaviorQuests;
                    Behaviors.Add(behaviorArray);
                }
                else
                {
                    var pickup = new BehaviorQuestPickup(b.FirstQuestID, b.SafeMovementPoint, b.QuestNpcID);
                    var workorder = new BehaviorQuestWorkOrder(b);
                    var workorderPickup = new BehaviorQuestWorkOrderPickup(b);
                    var turnin = new BehaviorQuestTurnin(b.FirstQuestID, b.SafeMovementPoint, b.WorkOrderNPCEntryId);
                    var behaviorArray = new BehaviorArray(new Behavior[] { pickup, workorder, workorderPickup, turnin });
                    behaviorArray.Criteria += () => BaseSettings.CurrentSettings.BehaviorQuests;
                    Behaviors.Add(behaviorArray);
                }
            }

            #endregion

            #region Herbing and Mining

            if (GarrisonManager.Buildings.Values.Any(b => b.Type == BuildingType.Mines))
            {
                Behaviors.Add(new BehaviorMine());
                Behaviors.Add(new BehaviorWorkOrderPickUp(GarrisonManager.Buildings[BuildingType.Mines]));
                Behaviors.Add(new BehaviorWorkOrderStartUp(GarrisonManager.Buildings[BuildingType.Mines]));
            }

            if (GarrisonManager.Buildings.Values.Any(b => b.Type == BuildingType.HerbGarden))
            {
                Behaviors.Add(new BehaviorHerb());
                Behaviors.Add(new BehaviorWorkOrderPickUp(GarrisonManager.Buildings[BuildingType.HerbGarden]));
                Behaviors.Add(new BehaviorWorkOrderStartUp(GarrisonManager.Buildings[BuildingType.HerbGarden]));
            }

            #endregion

            //Profession Crafting
            foreach (var skill in Player.Professions.ProfessionSkills)
            {
                int[] spellIds = PlayerProfessions.ProfessionDailyCooldownSpellIds[skill];
                Behaviors.Add(new BehaviorCraftingProfession(skill, spellIds[1]));
                Behaviors.Add(new BehaviorCraftingProfession(skill, spellIds[0]));
            }

            //Salvaging Behavior
            if (GarrisonManager.Buildings.Values.Any(b => b.Type == BuildingType.SalvageYard))
            {
                var b = GarrisonManager.Buildings.Values.First(building => building.Type == BuildingType.SalvageYard);
                Behaviors.Add(new BehaviorSalvage());
            }

            //Work Order Pickup and Startup (and Mine/Herb)
            foreach (var b in GarrisonManager.Buildings.Values.Where(b => b.FirstQuestID <= 0 || b.FirstQuestCompleted).OrderBy(b => b.Plot))
            {
                if (b.Type != BuildingType.Mines || b.Type != BuildingType.HerbGarden)
                {
                    if (b.WorkOrder != null)
                    {
                        Behaviors.Add(new BehaviorWorkOrderPickUp(b));
                        if (b.Type == BuildingType.EnchantersStudy) Behaviors.Add(new BehaviorDisenchant()); //Disenchanting!
                        Behaviors.Add(new BehaviorWorkOrderStartUp(b));
                    }
                }
            }

            //Primal Spirit Exchange
            Behaviors.Add(new BehaviorPrimalTrader());

            //Send any mail..
            Behaviors.Add(new BehaviorSendMail());

            //Finally, start some new missions!
            Behaviors.Add(new BehaviorMissionStartup());

            


            InitalizedBehaviorList = true;
        }

        internal static void Reset()
        {
            InitalizedBehaviorList = false;
            Behaviors.ForEach(b => b.IsDone = true);
            foreach (var behavior in Behaviors)
            {
                behavior.Dispose();
            }
        }

        internal static bool HasQuestAndNotCompleted(uint questId)
        {
            return
                QuestManager.QuestContainedInQuestLog(questId) &&
                !QuestManager.GetQuestFromQuestLog(questId).IsCompleted;
        }
        internal static bool ObjectNotValidOrNotFound(uint id)
        {
            var obj = ObjectCacheManager.GetWoWObject(id);
            return obj == null || !obj.IsValid;
        }
        internal static bool CanInteractWithUnit(uint id)
        {
            var unit = ObjectCacheManager.GetWoWUnits(id).FirstOrDefault();
            return unit != null && unit.IsValid && unit.RefWoWUnit.CanInteract;
        }
        internal static bool CanAttackUnit(uint id)
        {
            var unit = ObjectCacheManager.GetWoWUnits(id).FirstOrDefault();
            return unit != null && unit.IsValid && unit.RefWoWUnit.Attackable;
        }
        internal static bool UnitHasQuestGiverStatus(uint id, QuestGiverStatus status)
        {
            var unit = ObjectCacheManager.GetWoWUnits(id).FirstOrDefault();
            return unit != null && unit.IsValid && unit.RefWoWUnit.QuestGiverStatus == status;
        }
    }
}
