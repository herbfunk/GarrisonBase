using System.Threading.Tasks;
using Herbfunk.GarrisonBase.Quest;
using Styx.CommonBot.Coroutines;
using Styx.WoWInternals;

namespace Herbfunk.GarrisonBase.Coroutines.Behaviors
{
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
}