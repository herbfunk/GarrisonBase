using System.Threading.Tasks;
using Styx.CommonBot.Coroutines;

namespace Herbfunk.GarrisonBase.Coroutines.Behaviors
{
    public class BehaviorClickStaticPopup : Behavior
    {
    
        public BehaviorClickStaticPopup(int buttonIndex)
        {
            _buttonIndex = buttonIndex;
            RunCondition += LuaCommands.IsStaticPopupVisible;
            Criteria += LuaCommands.IsStaticPopupVisible;
        }

        private readonly int _buttonIndex = 1;
        public override async Task<bool> BehaviorRoutine()
        {
            if (await base.BehaviorRoutine()) return true;
            if (IsDone) return false;

            LuaCommands.ClickStaticPopupButton(_buttonIndex);
            await CommonCoroutines.SleepForRandomUiInteractionTime();
            return true;
        }


    }
}
