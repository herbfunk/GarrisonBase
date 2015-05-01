using System;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Herbfunk.GarrisonBase.Cache;
using Herbfunk.GarrisonBase.Garrison;
using Herbfunk.GarrisonBase.Garrison.Enums;
using Herbfunk.GarrisonBase.Helpers;
using Styx;
using Styx.CommonBot.Coroutines;
using Styx.CommonBot.Frames;
using Styx.Pathing;

namespace Herbfunk.GarrisonBase.Coroutines.Behaviors
{
    public class BehaviorQuestTurnin : Behavior
    {
        public override BehaviorType Type { get { return BehaviorType.QuestTurnin; } }

        public readonly uint QuestID;
        private readonly BuildingType buildingType;
        public readonly int RewardIndex;
        public BehaviorQuestTurnin(uint questId, WoWPoint loc, int npcEntryId, int rewardChoiceIndex = -1, BuildingType type = BuildingType.Unknown)
            : base(loc, npcEntryId)
        {
            QuestID = questId;
            buildingType = type;
            RewardIndex = rewardChoiceIndex;
            RunCondition += () => _questActionFinished || 
                                    (!_questActionFinished && 
                                        QuestHelper.QuestContainedInQuestLog(QuestID) && 
                                        QuestHelper.GetQuestFromQuestLog(QuestID).IsCompleted);

           
        }

        public BehaviorQuestTurnin(uint questId, WoWPoint loc, WoWPoint[] specialPoints, int npcEntryId,int rewardChoiceIndex = -1) 
            : this(questId,loc,npcEntryId,rewardChoiceIndex)
        {
            _specialMovementPoints = specialPoints;
        }
        public override void Initalize()
        {
            ObjectCacheManager.IsQuesting = true;
            ObjectCacheManager.QuestNpcIds.Add(Convert.ToUInt32(InteractionEntryId));

            _questActionFinished = false;
            _npcMovement = null;
            if (_specialMovementPoints != null)
                _specialMovement = new Movement(_specialMovementPoints, 2f, name: "QuestTurninSpecialMovement");

            base.Initalize();
        }

        public override void Dispose()
        {
            ObjectCacheManager.IsQuesting = false;
            ObjectCacheManager.QuestNpcIds.Remove(Convert.ToUInt32(InteractionEntryId));
            base.Dispose();
        }

        private Movement _npcMovement, _specialMovement;
        private readonly WoWPoint[] _specialMovementPoints;

        public override async Task<bool> BehaviorRoutine()
        {
            if (await base.BehaviorRoutine()) return true;
            if (IsDone) return false;

            if (await StartMovement.MoveTo())
                return true;

            if (!InteractionObjectValid)
            {
                GarrisonBase.Err("Interaction Object Id {0} not valid or found!", InteractionEntryId);
                return false;
            }

            if (GossipHelper.IsOpen)
            {
                int index = QuestHelper.GetActiveQuestIndexFromGossipFrame(QuestID);
                if (index == -1)
                {
                    GarrisonBase.Err("Failed to find quest {0} in gossip frame!", QuestID);
                    return false;
                }
                GossipFrame.Instance.SelectActiveGossipQuest(index);
                await CommonCoroutines.SleepForRandomUiInteractionTime();
                return true;
            }

            if (await Movement())
                return true;

            if (await Interaction())
                return true;

            if (_specialMovement != null && await _specialMovement.ClickToMove())
                return true;

            if (await EndMovement.MoveTo())
                return true;

            IsDone = true;
            return false;
        }

        private async Task<bool> Movement()
        {
            if (!_questActionFinished && !QuestHelper.QuestFrameOpen)
            {
                if (InteractionObject.WithinInteractRange)
                {
                    InteractionObject.Interact();
                    await CommonCoroutines.SleepForRandomUiInteractionTime();
                    return true;
                }

                #region SpecialMovement
                if (_specialMovement != null)
                {
                    //Special Movement for navigating inside buildings using Click To Move

                    if (_specialMovement.CurrentMovementQueue.Count > 0)
                    {
                        //find the nearest point to the npc in our special movement queue 
                        var nearestPoint = Coroutines.Movement.FindNearestPoint(InteractionObject.Location,
                            _specialMovement.CurrentMovementQueue.ToList());
                        //click to move.. but don't dequeue
                        var result = await _specialMovement.ClickToMove_Result(false);

                        if (!nearestPoint.Equals(_specialMovement.CurrentMovementQueue.Peek()))
                        {
                            //force dequeue now since its not nearest point
                            if (result == MoveResult.ReachedDestination)
                                _specialMovement.ForceDequeue(true);

                            return true;
                        }


                        //Last position was nearest and we reached our destination.. so lets finish special movement!
                        if (result == MoveResult.ReachedDestination)
                        {
                            _specialMovement.ForceDequeue(true);
                            _specialMovement.DequeueAll(false);
                        }
                    }
                }
                #endregion

                if (_npcMovement == null)
                    _npcMovement = new Movement(InteractionObject, InteractionObject.InteractRange - 0.25f);

                await _npcMovement.MoveTo(false);

                return true;
            }

            return false;
        }

        private bool _questActionFinished = false;

        private async Task<bool> Interaction()
        {
            if (_questActionFinished) return false;
            if (!QuestFrame.Instance.IsVisible) return true;

            switch (QuestHelper.QuestFrameType)
            {
                case QuestHelper.QuestFrameTypes.Progress:
                    QuestFrame.Instance.ClickContinue();
                    await CommonCoroutines.SleepForRandomUiInteractionTime();
                    await Coroutine.Sleep(5000);
                    break;
                case QuestHelper.QuestFrameTypes.Complete:
                    GarrisonBase.Log("Completing Quest..");

                    if (RewardIndex > -1)
                    {
                        GarrisonBase.Log("Selecting Reward Index {0}", RewardIndex);
                        QuestFrame.Instance.SelectQuestReward(RewardIndex);
                        await CommonCoroutines.SleepForRandomUiInteractionTime();
                    }

                    if (!BaseSettings.CurrentSettings.DEBUG_FAKEFINISHQUEST)
                    {
                        var successTurnedIn = await CommonCoroutines.WaitForLuaEvent(
                            "QUEST_FINISHED",
                            7500,
                            null,
                            QuestFrame.Instance.CompleteQuest);

                        await CommonCoroutines.SleepForRandomUiInteractionTime();
                        await CommonCoroutines.SleepForLagDuration();

                        if (successTurnedIn)
                        {
                            _questActionFinished = true;
                            if (buildingType != BuildingType.Unknown && GarrisonManager.Buildings.ContainsKey(buildingType))
                                GarrisonManager.Buildings[buildingType].FirstQuestCompleted = true;
                            if (_specialMovement != null) _specialMovement.UseDeqeuedPoints(true);
                            return false;
                        }

                        return true;
                    }

                    if (_specialMovement != null) _specialMovement.UseDeqeuedPoints(true);
                    _questActionFinished = true;
                    return false;
            }

            return true;
        }
    }
}