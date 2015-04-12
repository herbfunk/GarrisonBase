using System;
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
using Herbfunk.GarrisonBase.Helpers;
using Styx;
using Styx.CommonBot;
using Styx.CommonBot.Coroutines;
using Styx.WoWInternals;
using Styx.WoWInternals.DB;
using Styx.WoWInternals.Garrison;

namespace Herbfunk.GarrisonBase.Coroutines
{
    public class BehaviorManager
    {
        internal static Behavior CurrentBehavior { get; private set; }
        internal static List<Behavior> Behaviors = new List<Behavior>();

        
        /// <summary>
        /// Used to switch the current behavior
        /// </summary>
        internal static List<Behavior> SwitchBehaviors = new List<Behavior>();
        internal static Behavior SwitchBehavior { get; set; }

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

            //Do we need to hearth or fly to our garrison?
            if (await Common.PreChecks.BehaviorRoutine())
                return true;

            //Disable and reload UI if master plan is enabled..
            if (await Common.PreChecks.CheckAddons()) return true;

            //We need "open" the garrison up and initalize it.. (so we don't get errors trying to inject!)
            if (await Common.PreChecks.InitalizeGarrisonManager()) return true;

            //Inject our lua addon code for mission success function
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
                //Check for any switch behaviors.. (override current behavior temporarly)
                if (SwitchBehaviors.Count > 0 && SwitchBehavior == null)
                {
                    while (SwitchBehaviors.Count > 0)
                    {
                        if (!SwitchBehaviors[0].CheckCriteria())
                            SwitchBehaviors.RemoveAt(0);
                        else
                        {
                            SwitchBehavior = SwitchBehaviors[0];
                            break;
                        }
                    }
                }
                
                if (SwitchBehavior != null && CurrentBehavior != SwitchBehavior)
                {
                    GarrisonBase.Debug("Switching behaviors to {0}", SwitchBehavior.Type);
                    CurrentBehavior = SwitchBehavior;
                    CurrentBehavior.Initalize();
                }

                bool x = await CurrentBehavior.BehaviorRoutine();

                if (x && !CurrentBehavior.IsDone) return true;

                GarrisonBase.Debug(
                    !x ? "Finishing Behavior {0} because it returned false!"
                        : "Finishing Behavior {0} because IsDone is true", CurrentBehavior.Type);

                if (!CurrentBehavior.Disposed) CurrentBehavior.Dispose();

                if (SwitchBehavior != null && CurrentBehavior.Equals(SwitchBehavior))
                {
                    SwitchBehaviors.RemoveAt(0);
                    SwitchBehavior = null;

                    CurrentBehavior = Behaviors[0];
                    CurrentBehavior.Initalize();
                }
                else
                {
                    Behaviors.RemoveAt(0);
                    CurrentBehavior = null;
                }


                return true;
            }

            if (Common.PreChecks.DisabledMasterPlanAddon)
            {
                Common.PreChecks.ShouldCheckAddons = false;
                LuaCommands.EnableAddon("MasterPlan");
                LuaCommands.ReloadUI();
                Common.PreChecks.DisabledMasterPlanAddon = false;
                await Coroutine.Wait(6000, () => StyxWoW.IsInGame);
                return true;
            }



            TreeRoot.StatusText = "GarrisonBase is finished!";
            TreeRoot.Stop();
            return false;
        }


        internal static bool InitalizedBehaviorList = false;
        internal static void InitalizeBehaviorsList()
        {
            GarrisonBase.Debug("Initalize Behaviors List..");
            //Insert new missions behavior at beginning!
            Behaviors.Clear();
            SwitchBehaviors.Clear();
            CurrentBehavior = null;
            SwitchBehavior = null;

            Behaviors.Add(Follower.FollowerQuestBehaviorArray(209));
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
            foreach (var b in GarrisonManager.Buildings.Values.Where(b => !b.FirstQuestCompleted && !b.IsBuilding && b.FirstQuestNpcId > -1 && b.FirstQuestId > -1))
            {
                if (b.Type == BuildingType.SalvageYard)
                {
                    //var abandon = new Behaviors.BehaviorQuestAbandon(b.FirstQuestID);
                    var pickup = new BehaviorQuestPickup(b.FirstQuestId, b.SafeMovementPoint, b.WorkOrderNpcEntryId);
                    var itemInteraction = new BehaviorItemInteraction(118473);
                    var turnin = new BehaviorQuestTurnin(b.FirstQuestId, b.SafeMovementPoint, b.WorkOrderNpcEntryId);
                    var behaviorArray = new BehaviorArray(new Behavior[] { pickup, itemInteraction, turnin });
                    behaviorArray.Criteria += () => BaseSettings.CurrentSettings.BehaviorQuests;
                    Behaviors.Add(behaviorArray);
                }
                else if (b.Type == BuildingType.TradingPost)
                {
                    var pickup = new BehaviorQuestPickup(b.FirstQuestId, b.SafeMovementPoint, b.WorkOrderNpcEntryId);
                    var moveLoc = Player.IsAlliance
                        ? new WoWPoint(1764.08, 150.39, 76.02)
                        : new WoWPoint(5745.101, 4570.491, 138.8332);

                    var moveto = new BehaviorMove(moveLoc, 7f);
                    var npcId = Player.IsAlliance ? 87288 : 87260;
                    var target = new BehaviorSelectTarget(moveLoc);
                    var interact = new BehaviorItemInteraction(118418, true);
                    var turnin = new BehaviorQuestTurnin(b.FirstQuestId, b.SafeMovementPoint, b.WorkOrderNpcEntryId);

                    var behaviorArray = new BehaviorArray(new Behavior[] { pickup, moveto, target, interact, turnin });
                    behaviorArray.Criteria += () => BaseSettings.CurrentSettings.BehaviorQuests;
                    Behaviors.Add(behaviorArray);
                }
                else if (b.Type == BuildingType.Storehouse)
                {
                    var pickup = new BehaviorQuestPickup(b.FirstQuestId, b.EntranceMovementPoint, b.FirstQuestNpcId);

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

                    var looting = new BehaviorHotspotRunning(b.FirstQuestId, _hotSpots.ToArray(), BehaviorHotspotRunning.HotSpotType.Looting, () => HasQuestAndNotCompleted(b.FirstQuestId));
                    var turnin = new BehaviorQuestTurnin(b.FirstQuestId, b.EntranceMovementPoint, b.FirstQuestNpcId);

                    var behaviorArray = new BehaviorArray(new Behavior[] { pickup, looting, turnin });
                    behaviorArray.Criteria += () => BaseSettings.CurrentSettings.BehaviorQuests;
                    Behaviors.Add(behaviorArray);
                }
                else if (b.Type == BuildingType.Lumbermill)
                {
                    var pickup = new BehaviorQuestPickup(b.FirstQuestId, b.EntranceMovementPoint, b.FirstQuestNpcId);

                    WoWPoint movementPoint;
                    if (Player.IsAlliance)
                        movementPoint = new WoWPoint(1555.087, 173.8229, 72.59766);
                    else
                        movementPoint = new WoWPoint(6082.979, 4795.821, 149.1655);

                    var looting = new BehaviorHotspotRunning(b.FirstQuestId, new[] { movementPoint }, new uint[] { 234021, 233922 }, BehaviorHotspotRunning.HotSpotType.Looting, () => HasQuestAndNotCompleted(b.FirstQuestId));
                    var turnin = new BehaviorQuestTurnin(b.FirstQuestId, b.EntranceMovementPoint, b.FirstQuestNpcId);
                    var behaviorArray = new BehaviorArray(new Behavior[] { pickup, looting, turnin });
                    behaviorArray.Criteria += () => BaseSettings.CurrentSettings.BehaviorQuests;
                    Behaviors.Add(behaviorArray);
                }
                else if (b.Type == BuildingType.Mines)
                {
                    var pickup = new BehaviorQuestPickup(b.FirstQuestId, b.EntranceMovementPoint, b.FirstQuestNpcId);

                    var looting = new BehaviorHotspotRunning(
                        b.FirstQuestId,
                        Player.IsAlliance ?
                        MovementCache.Alliance_Mine_LevelOne.ToArray() :
                        MovementCache.Horde_Mine_LevelOne.ToArray(),
                        CacheStaticLookUp.MineQuestMobIDs.ToArray(),
                        BehaviorHotspotRunning.HotSpotType.Killing,
                        () => HasQuestAndNotCompleted(b.FirstQuestId));

                    var turnin = new BehaviorQuestTurnin(b.FirstQuestId, b.EntranceMovementPoint, b.FirstQuestNpcId);

                    var behaviorArray = new BehaviorArray(new Behavior[] { pickup, looting, turnin });
                    behaviorArray.Criteria += () => BaseSettings.CurrentSettings.BehaviorQuests;
                    Behaviors.Add(behaviorArray);
                }
                else if (b.Type == BuildingType.HerbGarden)
                {
                    var pickup = new BehaviorQuestPickup(b.FirstQuestId, b.EntranceMovementPoint, b.FirstQuestNpcId);

                    var looting = new BehaviorHotspotRunning(
                        b.FirstQuestId,
                        Player.IsAlliance ?
                        MovementCache.Alliance_Herb_LevelOne.ToArray() :
                        MovementCache.Horde_Herb_LevelOne.ToArray(),
                        CacheStaticLookUp.HerbQuestMobIDs.ToArray(),
                        BehaviorHotspotRunning.HotSpotType.Killing,
                        () => HasQuestAndNotCompleted(b.FirstQuestId));

                    var turnin = new BehaviorQuestTurnin(b.FirstQuestId, b.EntranceMovementPoint, b.FirstQuestNpcId);

                    var behaviorArray = new BehaviorArray(new Behavior[] { pickup, looting, turnin });
                    behaviorArray.Criteria += () => BaseSettings.CurrentSettings.BehaviorQuests;
                    Behaviors.Add(behaviorArray);
                }
                else
                {
                    var pickup = new BehaviorQuestPickup(b.FirstQuestId, b.SafeMovementPoint, b.FirstQuestNpcId);
                    var workorder = new BehaviorQuestWorkOrder(b);
                    var workorderPickup = new BehaviorQuestWorkOrderPickup(b);
                    var turnin = new BehaviorQuestTurnin(b.FirstQuestId, b.SafeMovementPoint, b.WorkOrderNpcEntryId);
                    var behaviorArray = new BehaviorArray(new Behavior[] { pickup, workorder, workorderPickup, turnin });
                    behaviorArray.Criteria += () => BaseSettings.CurrentSettings.BehaviorQuests;
                    Behaviors.Add(behaviorArray);
                }
            }

            #endregion

            #region Herbing and Mining

            if (GarrisonManager.Buildings.Values.Any(b => b.Type == BuildingType.Mines))
            {
                var miningArray = new BehaviorArray(new Behavior[]
                {
                    new BehaviorMove(MovementCache.MinePlot59SafePoint),
                    new BehaviorMine()
                });
                miningArray.Criteria += () => (!GarrisonManager.Buildings[BuildingType.Mines].IsBuilding &&
                                               !GarrisonManager.Buildings[BuildingType.Mines].CanActivate &&
                                               GarrisonManager.Buildings[BuildingType.Mines].FirstQuestCompleted &&
                                               LuaCommands.CheckForDailyReset(BaseSettings.CurrentSettings.LastCheckedMine) &&
                                               BaseSettings.CurrentSettings.BehaviorMineGather);

                Behaviors.Add(miningArray);
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
                if (skill == SkillLine.Inscription)
                    Behaviors.Add(new BehaviorMilling());
                
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
            foreach (var b in GarrisonManager.Buildings.Values.Where(b => b.FirstQuestId <= 0 || b.FirstQuestCompleted).OrderBy(b => b.Plot))
            {
                if (b.Type != BuildingType.Mines || b.Type != BuildingType.HerbGarden)
                {
                    if (b.WorkOrder != null)
                    {
                        Behaviors.Add(new BehaviorWorkOrderPickUp(b));

                        if (b.Type == BuildingType.EnchantersStudy) 
                            Behaviors.Add(new BehaviorDisenchant()); //Disenchanting!
                        else if(b.Type== BuildingType.ScribesQuarters)
                            Behaviors.Add(new BehaviorMilling()); //Milling!

                        Behaviors.Add(new BehaviorWorkOrderStartUp(b));

                        if (b.Type == BuildingType.WarMillDwarvenBunker && b.Level == 3)
                        {
                            var questid = Player.IsAlliance ? 38175 : 38188;
                            Behaviors.Add(QuestHelper.GetDailyQuestArray(Convert.ToUInt32(questid), Player.IsAlliance));
                        }
                        else if (b.Type == BuildingType.AlchemyLab && b.HasFollowerWorking)
                        {
                            Behaviors.Add(QuestHelper.GetDailyQuestArray(Convert.ToUInt32(37270), Player.IsAlliance));
                        }
                    }
                }
            }

            //Primal Spirit Exchange
            Behaviors.Add(new BehaviorPrimalTrader());

            //Send any mail..
            Behaviors.Add(new BehaviorSendMail());

            //Finally, start some new missions!
            Behaviors.Add(new BehaviorMissionStartup());

            //Optional follower behaviors (to unlock)
            Behaviors.Add(new BehaviorArray(new Behavior[]
            {
                //Follower.FollowerQuestBehaviorArray(209),
                Follower.FollowerQuestBehaviorArray(170),
                Follower.FollowerQuestBehaviorArray(467),
                Follower.FollowerQuestBehaviorArray(189),
                Follower.FollowerQuestBehaviorArray(193),
                Follower.FollowerQuestBehaviorArray(207),
                Follower.FollowerQuestBehaviorArray(190),
                new BehaviorCustomAction(() => Common.PreChecks.IgnoreHearthing=false),
                new BehaviorUseFlightPath(MovementCache.GarrisonEntrance)
            }));
           

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

        internal static bool HasQuest(uint questId)
        {
            return QuestHelper.QuestContainedInQuestLog(questId);
        }
        internal static bool HasQuestAndNotCompleted(uint questId)
        {
            return
                HasQuest(questId) &&
                !QuestHelper.GetQuestFromQuestLog(questId).IsCompleted;
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
