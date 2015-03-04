using System;
using System.Threading.Tasks;
using Buddy.Coroutines;
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

                if (QuestManager.QuestLogFull())
                    return false;

                //if (LuaCommands.IsQuestFlaggedCompleted(QuestID.ToString()))
                //    return false;

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
                                TreeRoot.StatusText = String.Format("Behavior {0} Quest Pickup NPC Interact", Type.ToString());
                                InteractionObject.Interact();
                                await CommonCoroutines.SleepForRandomUiInteractionTime();
                                return true;
                            }

                            TreeRoot.StatusText = String.Format("Behavior {0} Quest Pickup NPC Moveto", Type.ToString());
                            await CommonCoroutines.MoveTo(InteractionObject.Location);
                            return true;
                        }

                        if (await base.Movement())
                            return true;
                    }
                    else
                    {
                        if (QuestFrame.Instance.IsVisible)
                        {
                            TreeRoot.StatusText = String.Format("Behavior {0} Quest Pickup NPC Accept Quest", Type.ToString());

                            QuestFrame.Instance.AcceptQuest();
                            await CommonCoroutines.SleepForRandomUiInteractionTime();
                            await CommonCoroutines.SleepForLagDuration();
                            await Coroutine.Sleep(1250);
                        }
                        return true;
                    }
                }


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


            public override async Task<bool> BehaviorRoutine()
            {
                if (IsDone) return false;

                if (await base.BehaviorRoutine()) return true;

                if (QuestManager.QuestContainedInQuestLog(QuestID))
                {
                    if (QuestManager.GetQuestFromQuestLog(QuestID).IsCompleted)
                    {
                        TreeRoot.StatusText = String.Format("Behavior {0} Quest Completion", Type.ToString());

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

                            if (InteractionObject != null && InteractionObject.IsValid)
                            {
                                if (InteractionObject.WithinInteractRange)
                                {
                                    TreeRoot.StatusText = String.Format("Behavior {0} Quest Completion NPC Interact",
                                        Type.ToString());
                                    InteractionObject.Interact();
                                    await CommonCoroutines.SleepForRandomUiInteractionTime();
                                    return true;
                                }

                                TreeRoot.StatusText = String.Format("Behavior {0} Quest Completion NPC MoveTo",
                                    Type.ToString());
                                await CommonCoroutines.MoveTo(InteractionObject.Location);
                                return true;
                            }

                            if (await base.Movement())
                                return true;
                        }
                        else
                        {
                            if (QuestFrame.Instance.IsVisible)
                            {
                                TreeRoot.StatusText = String.Format("Behavior {0} Quest Completion NPC Complete Quest", Type.ToString());

                                if (LuaEvents.QuestFrameType == QuestFrameTypes.Progress)
                                {
                                    QuestFrame.Instance.ClickContinue();
                                    await CommonCoroutines.SleepForRandomUiInteractionTime();
                                    await Coroutine.Sleep(5000);
                                }
                                else if (LuaEvents.QuestFrameType == QuestFrameTypes.Complete)
                                {
                                    GarrisonBase.Log("Completing Quest!");
                                    QuestFrame.Instance.CompleteQuest();
                                    await CommonCoroutines.SleepForRandomUiInteractionTime();
                                    await Coroutine.Sleep(5000);

                                    if (GarrisonManager.Buildings.ContainsKey(buildingType))
                                        GarrisonManager.Buildings[buildingType].FirstQuestCompleted = true;

                                    return false;
                                }
                            }
                            return true;
                        }
                    }
                }


                return false;
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
                if (Building.BuildingPolygon != null && !Building.BuildingPolygon.LocationInsidePolygon(StyxWoW.Me.Location))
                    MovementPoints.Insert(0, Building.BuildingPolygon.Entrance);

                base.Initalize();
            }


            public override async Task<bool> BehaviorRoutine()
            {
                if (IsDone) return false;

                if (!QuestManager.QuestContainedInQuestLog(QuestID) || QuestManager.GetQuestFromQuestLog(QuestID).IsCompleted)
                    return false;

                if (Building.WorkOrder == null || Building.WorkOrder.GetTotalWorkorderStartups() == 0)
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
    }
}
