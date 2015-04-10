using System.Threading.Tasks;
using Herbfunk.GarrisonBase.Helpers;
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

            if (QuestHelper.QuestContainedInQuestLog(QuestID))
            {
                QuestLog.Instance.AbandonQuestById(QuestID);
                await CommonCoroutines.SleepForRandomUiInteractionTime();
                return true;
            }

            return false;
        }
    }
}