using System;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Herbfunk.GarrisonBase.Cache;
using Herbfunk.GarrisonBase.Quest;
using Styx;
using Styx.CommonBot.Coroutines;
using Styx.CommonBot.Frames;

namespace Herbfunk.GarrisonBase.Coroutines.Behaviors
{
    public class BehaviorQuestPickup : Behavior
    {
        public override BehaviorType Type { get { return BehaviorType.QuestPickup; } }

        public readonly uint QuestID;
        private readonly bool CompleteQuest = false;
        public BehaviorQuestPickup(uint questId, WoWPoint loc, int npcEntryId, bool _completeQuest=false)
            : base(loc, npcEntryId)
        {
            QuestID = questId;
            CompleteQuest = _completeQuest;
            if (!_completeQuest)
            {
                RunCondition += () => !QuestManager.QuestLogFull &&
                                  !QuestManager.QuestContainedInQuestLog(QuestID);
            }
            ObjectCacheManager.QuestNpcIds.Add(Convert.ToUInt32(InteractionEntryId));

            Criteria += (() =>  (!QuestManager.QuestLogFull &&
                                  !QuestManager.QuestContainedInQuestLog(QuestID)));
        }

        public override void Dispose()
        {
            base.Dispose();
            ObjectCacheManager.QuestNpcIds.Remove(Convert.ToUInt32(InteractionEntryId));
        }
        private Movement _npcMovement;
        public override async Task<bool> BehaviorRoutine()
        {
            if (await base.BehaviorRoutine()) return true;

            if (IsDone) return false;

            //if (QuestManager.QuestLogFull) return false;

            //if (QuestManager.QuestContainedInQuestLog(QuestID)) return false;

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

                    if (_npcMovement == null)
                        _npcMovement = new Movement(InteractionObject.Location, InteractionObject.InteractRange - 0.25f);

                    await _npcMovement.ClickToMove(false);

                    return true;
                }
            }
            else
            {
                if (QuestFrame.Instance.IsVisible)
                {
                    if (CompleteQuest)
                    {
                        if (BaseSettings.CurrentSettings.DEBUG_FAKEFINISHQUEST)
                        {
                            IsDone = true;
                            return true;
                        }

                        bool success = await CommonCoroutines.WaitForLuaEvent(
                                "QUEST_TURNED_IN",
                                7500,
                                null,
                                QuestFrame.Instance.CompleteQuest);

                        if (success)
                        {
                            IsDone = true;
                        }

                        return true;
                    }
                    
                    QuestFrame.Instance.AcceptQuest();
                    await CommonCoroutines.SleepForRandomUiInteractionTime();
                    await CommonCoroutines.SleepForLagDuration();
                    await Coroutine.Sleep(1250);
                }
                return true;
            }


            IsDone = true;
            return false;
        }
    }
}