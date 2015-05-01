using System;
using System.Threading.Tasks;
using Styx.Common.Helpers;

namespace Herbfunk.GarrisonBase.Coroutines.Behaviors
{
    public class BehaviorSleep : Behavior
    {
        public override BehaviorType Type { get { return BehaviorType.Sleep; } }

        private readonly WaitTimer _sleeptimer;
        public BehaviorSleep(int milliseconds)
        {
            _sleeptimer = new WaitTimer(new TimeSpan(0, 0, 0, 0, milliseconds));
        }

        public override void Initalize()
        {
            _sleeptimer.Reset();
            base.Initalize();
        }

        public override async Task<bool> BehaviorRoutine()
        {
            if (await base.BehaviorRoutine()) return true;
            if (IsDone) return false;

            if (!_sleeptimer.IsFinished) return true;
            IsDone = true;
            return false;
        }

        public override string GetStatusText
        {
            get
            {
                return base.GetStatusText + " " + _sleeptimer.TimeLeft.TotalMilliseconds.ToString() + " ms";
            }
        }


    }
}
