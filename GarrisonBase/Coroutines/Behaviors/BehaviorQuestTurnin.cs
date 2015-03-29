using System;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Herbfunk.GarrisonBase.Cache;
using Herbfunk.GarrisonBase.Garrison;
using Herbfunk.GarrisonBase.Garrison.Enums;
using Herbfunk.GarrisonBase.Quest;
using Styx;
using Styx.CommonBot.Coroutines;
using Styx.CommonBot.Frames;

namespace Herbfunk.GarrisonBase.Coroutines.Behaviors
{
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

            RunCondition += () => QuestManager.QuestContainedInQuestLog(QuestID) &&
                                  QuestManager.GetQuestFromQuestLog(QuestID).IsCompleted;

            ObjectCacheManager.QuestNpcIds.Add(Convert.ToUInt32(InteractionEntryId));
        }

        public override void Initalize()
        {
            _npcMovement = null;
            base.Initalize();
        }

        public override void Dispose()
        {
            ObjectCacheManager.QuestNpcIds.Remove(Convert.ToUInt32(InteractionEntryId));
            base.Dispose();
        }

        private Movement _npcMovement;
        public override async Task<bool> BehaviorRoutine()
        {
            if (await base.BehaviorRoutine()) return true;
            if (IsDone) return false;

            //if (!QuestManager.QuestContainedInQuestLog(QuestID)) return false;
            //if (!QuestManager.GetQuestFromQuestLog(QuestID).IsCompleted) return false;

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

                await _npcMovement.ClickToMove(false);

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
}