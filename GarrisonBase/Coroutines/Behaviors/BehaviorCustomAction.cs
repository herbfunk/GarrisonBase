using System;
using System.Linq;
using System.Threading.Tasks;

namespace Herbfunk.GarrisonBase.Coroutines.Behaviors
{
    public class BehaviorCustomAction : Behavior
    {
        public Action CustomAction;
        public Func<bool> CustomCondition = () => true;
        public readonly bool RepeatAction;
        public BehaviorCustomAction(Action action, bool repeat = false)
        {
            CustomAction = action;
            RepeatAction = repeat;
        }

        public BehaviorCustomAction(Action action, Func<bool> condition, bool repeat = false) : this(action, repeat)
        {
            CustomCondition = condition;
        }
        private bool CheckCondition()
        {
            return CustomCondition.GetInvocationList().Cast<Func<bool>>().All(f => f());
        }
        public override async Task<bool> BehaviorRoutine()
        {
            if (await base.BehaviorRoutine()) return true;

            if (IsDone) return false;

            if (!CheckCondition())
            {
                IsDone = !RepeatAction;
                return false;
            }

            CustomAction.Invoke();
            
            if (!RepeatAction)
            {
                IsDone = true;
            }

            return RepeatAction;
        }
    }
}