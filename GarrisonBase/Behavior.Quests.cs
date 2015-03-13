using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Herbfunk.GarrisonBase.Cache;
using Herbfunk.GarrisonBase.Garrison;
using Herbfunk.GarrisonBase.Garrison.Enums;
using Herbfunk.GarrisonBase.Garrison.Objects;
using Herbfunk.GarrisonBase.Quest;
using Styx;
using Styx.CommonBot;
using Styx.CommonBot.Coroutines;
using Styx.CommonBot.Frames;
using Styx.WoWInternals;

namespace Herbfunk.GarrisonBase
{
    public partial class Behaviors
    {
        public class BehaviorQuestPickup : Behavior
        {
            public override BehaviorType Type { get { return BehaviorType.QuestPickup; } }

            public readonly uint QuestID;
            public BehaviorQuestPickup(uint questId, WoWPoint loc, int npcEntryId) :base(loc, npcEntryId)
            {
                QuestID = questId;
            }


            public override async Task<bool> BehaviorRoutine()
            {
                if (IsDone) return false;

                if (await base.BehaviorRoutine()) return true;

                if (QuestManager.QuestLogFull()) return false;

                if (!QuestManager.QuestContainedInQuestLog(QuestID))
                {
                    if (await StartMovement.MoveTo())
                        return true;

                    if (LuaEvents.GossipFrameOpen)
                    {
                        int index = QuestManager.GetAvailableQuestIndexFromGossipFrame(QuestID);
                        if (index == -1)
                        {
                            GarrisonBase.Err("Failed to find quest {0} in gossip frame!", QuestID);
                            return false;
                        }
                        GossipFrame.Instance.SelectAvailableQuest(index);
                        await CommonCoroutines.SleepForRandomUiInteractionTime();
                        return true;
                    }

                    if (!LuaEvents.QuestFrameOpen)
                    {
                        if (InteractionObject != null && InteractionObject.IsValid)
                        {
                            if (InteractionObject.WithinInteractRange)
                            {
                                InteractionObject.Interact();
                                await CommonCoroutines.SleepForRandomUiInteractionTime();
                                return true;
                            }

                            await CommonCoroutines.MoveTo(InteractionObject.Location);
                            return true;
                        }
                    }
                    else
                    {
                        if (QuestFrame.Instance.IsVisible)
                        {
                            QuestFrame.Instance.AcceptQuest();
                            await CommonCoroutines.SleepForRandomUiInteractionTime();
                            await CommonCoroutines.SleepForLagDuration();
                            await Coroutine.Sleep(1250);
                        }
                        return true;
                    }
                }

                IsDone = true;
                return false;
            }
        }

        public class BehaviorQuestTurnin : Behavior
        {
            public override BehaviorType Type { get { return BehaviorType.QuestTurnin; } }

            public readonly uint QuestID;
            private readonly BuildingType buildingType;
            public BehaviorQuestTurnin(uint questId, WoWPoint loc, int npcEntryId, BuildingType type = BuildingType.Unknown)
                : base(loc, npcEntryId)
            {
                QuestID = questId;
                buildingType = type;
            }

            private Movement _npcMovement;
            public override async Task<bool> BehaviorRoutine()
            {
                if (IsDone) return false;

                if (await base.BehaviorRoutine()) return true;

                if (!QuestManager.QuestContainedInQuestLog(QuestID)) return false;
                if (!QuestManager.GetQuestFromQuestLog(QuestID).IsCompleted) return false;

                if (await StartMovement.MoveTo()) return true;
                //TreeRoot.StatusText = String.Format("Behavior {0} Quest Completion", Type.ToString());

                if (LuaEvents.GossipFrameOpen)
                {
                    int index = QuestManager.GetActiveQuestIndexFromGossipFrame(QuestID);
                    if (index == -1)
                    {
                        GarrisonBase.Err("Failed to find quest {0} in gossip frame!", QuestID);
                        return false;
                    }
                    GossipFrame.Instance.SelectActiveGossipQuest(index);
                    await CommonCoroutines.SleepForRandomUiInteractionTime();
                    return true;
                }

                if (!LuaEvents.QuestFrameOpen)
                {
                    if (InteractionObject == null || !InteractionObject.IsValid)
                    {
                        //NPC object not found!
                        GarrisonBase.Err("Could not find valid NPC");
                        return false;
                    }

                    if (_npcMovement == null) 
                        _npcMovement = new Movement(InteractionObject.Location, InteractionObject.InteractRange-0.25f);

                    await _npcMovement.MoveTo();


                    if (!InteractionObject.WithinInteractRange) return true;

                    //TreeRoot.StatusText = String.Format("Behavior {0} Quest Completion NPC Interact",Type.ToString());
                    if (StyxWoW.Me.IsMoving) await CommonCoroutines.StopMoving();

                    InteractionObject.Interact();
                    await CommonCoroutines.SleepForRandomUiInteractionTime();

                    return true;
                }

                if (!QuestFrame.Instance.IsVisible) return true;
                //TreeRoot.StatusText = String.Format("Behavior {0} Quest Completion NPC Complete Quest", Type.ToString());

                switch (LuaEvents.QuestFrameType)
                {
                    case QuestFrameTypes.Progress:
                        QuestFrame.Instance.ClickContinue();
                        await CommonCoroutines.SleepForRandomUiInteractionTime();
                        await Coroutine.Sleep(5000);
                        break;
                    case QuestFrameTypes.Complete:
                        GarrisonBase.Log("Completing Quest!");
                        if (!BaseSettings.CurrentSettings.DEBUG_FAKEFINISHQUEST)
                        {
                            QuestFrame.Instance.CompleteQuest();
                            await CommonCoroutines.SleepForRandomUiInteractionTime();
                            await Coroutine.Sleep(5000);

                            if (GarrisonManager.Buildings.ContainsKey(buildingType))
                                GarrisonManager.Buildings[buildingType].FirstQuestCompleted = true;
                        }
                        return false;
                }
                return true;
            }
        }

        public class BehaviorQuestAbandon : Behavior
        {
            public override BehaviorType Type { get { return BehaviorType.QuestAbandon; } }
            public readonly uint QuestID;

            public BehaviorQuestAbandon(uint questId)
            {
                QuestID = questId;
            }



            public override async Task<bool> BehaviorRoutine()
            {
                if (IsDone) return false;

                if (QuestManager.QuestContainedInQuestLog(QuestID))
                {
                    QuestLog.Instance.AbandonQuestById(QuestID);
                    await CommonCoroutines.SleepForRandomUiInteractionTime();
                    return true;
                }

                return false;
            }
        }

        public class BehaviorQuestWorkOrder : Behavior
        {
            
            public readonly uint QuestID;
            public readonly Building Building;
            public BehaviorQuestWorkOrder(Building building)
                : base(building.SafeMovementPoint, building.WorkOrderNPCEntryId)
            {
                Building = building;
                QuestID = building.FirstQuestID;
            }

            public override void Initalize()
            {

                base.Initalize();
            }


            public override async Task<bool> BehaviorRoutine()
            {
                if (IsDone) return false;

                if (!QuestManager.QuestContainedInQuestLog(QuestID) || QuestManager.GetQuestFromQuestLog(QuestID).IsCompleted)
                    return false;

                if (Building.WorkOrder == null || Building.WorkOrder.TotalWorkorderStartups() == 0)
                    return false;

                if (Building.WorkOrderObject != null &&
                    Building.WorkOrderObject.GetCursor() != WoWCursorType.PointCursor)
                {
                    return false;
                }

                if (await base.BehaviorRoutine()) return true;

                if (await StartMovement.MoveTo())
                    return true;
                
                if (LuaEvents.GossipFrameOpen)
                {
                    GossipFrame.Instance.SelectGossipOption(0);
                    await CommonCoroutines.SleepForRandomUiInteractionTime();
                    return true;
                }

                if (LuaCommands.IsGarrisonCapacitiveDisplayFrame())
                {
                    if (!LuaCommands.ClickStartOrderButtonEnabled())
                    {
                        Building.CheckedWorkOrderStartUp = true;
                        GarrisonBase.Log("Order Button Disabled!");
                        return false;
                    }

                   
                    LuaCommands.ClickStartOrderButton();
                    await CommonCoroutines.SleepForRandomUiInteractionTime();
                    await CommonCoroutines.SleepForLagDuration();
                    return true;
                }

                if (InteractionObject != null && InteractionObject.IsValid)
                {
                    if (InteractionObject.WithinInteractRange)
                    {
                        TreeRoot.StatusText = String.Format("Behavior {0} Quest NPC Interact", Type.ToString());
                        InteractionObject.Interact();
                        await CommonCoroutines.SleepForRandomUiInteractionTime();
                        return true;
                    }

                    TreeRoot.StatusText = String.Format("Behavior {0} Quest NPC Moveto", Type.ToString());
                    await CommonCoroutines.MoveTo(InteractionObject.Location);
                    return true;
                }

                return false;
            }

        }

        public class BehaviorQuestLootKill : Behavior
        {
            public override BehaviorType Type { get { return BehaviorType.QuestLootKill; } }

            public BehaviorQuestLootKill(uint questId, WoWPoint[] hotspots, bool looting) : this(questId, hotspots, new uint[0], looting)
            {
            }

            public BehaviorQuestLootKill(uint questId, WoWPoint[] hotspots, uint[] ids, bool looting)
            {
                _questId = questId;
                _objectIds.AddRange(ids);
                _hotSpots.AddRange(hotspots);
                _lootingBehavior = looting;
            }

            private readonly uint _questId;
            private readonly List<uint> _objectIds = new List<uint>();
            private readonly List<WoWPoint> _hotSpots = new List<WoWPoint>();
            private readonly bool _lootingBehavior;

            public override void Initalize()
            {
                _hotSpotMovement = new Movement(_hotSpots.ToArray());

                if (_lootingBehavior) 
                    ObjectCacheManager.LootableEntryIds.AddRange(_objectIds);
                else
                    ObjectCacheManager.KillableEntryIds.AddRange(_objectIds);

                base.Initalize();
            }

            private Movement _hotSpotMovement;

            public override async Task<bool> BehaviorRoutine()
            {
                if (IsDone) return false;

                if (await base.BehaviorRoutine()) return true;

                if (!QuestManager.QuestContainedInQuestLog(_questId))
                {
                    IsDone = true;
                    return false;
                }

                if (QuestManager.GetQuestFromQuestLog(_questId).IsCompleted)
                {
                    if (_lootingBehavior) 
                        ObjectCacheManager.ShouldLoot = false;
                    else 
                        ObjectCacheManager.ShouldKill = false;

                    IsDone = true;
                    return false;
                }

                if (_lootingBehavior)
                {
                    ObjectCacheManager.ShouldLoot = true;
                    ObjectCacheManager.UpdateLootableTarget();
                }
                else
                {
                    ObjectCacheManager.ShouldKill = true;
                    ObjectCacheManager.UpdateCombatTarget();
                }

                if (_hotSpotMovement.CurrentMovementQueue.Count==0)
                    _hotSpotMovement.UseDeqeuedPoints(true);

                await _hotSpotMovement.MoveTo();

                return true;
            }
        }
    }
}
