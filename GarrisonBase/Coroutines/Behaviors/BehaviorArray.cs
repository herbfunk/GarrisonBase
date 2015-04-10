using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Herbfunk.GarrisonBase.Coroutines.Behaviors
{
    /// <summary>
    /// Encapsulates multiple behaviors and runs each one, removing those that failed or did not pass critiera check.
    /// </summary>
    public class BehaviorArray : Behavior
    {
        public override BehaviorType Type { get { return BehaviorType.Array; } }

        public List<Behavior> Behaviors = new List<Behavior>();

        public BehaviorArray(Behavior[] behaviors)
        {
            Behaviors.AddRange(behaviors);
            
        }

        public override string GetStatusText
        {
            get
            {
                return _currentBehavior != null ? _currentBehavior.GetStatusText : base.GetStatusText;
            }
        }

        public override void Initalize()
        {
            //Each behavior should inherit the parent criteria!
            foreach (var behavior in Behaviors)
            {
                behavior.Criteria += Criteria;
            }

            base.Initalize();
        }


        public override void Dispose()
        {
            foreach (var behavior in Behaviors.Where(behavior => !behavior.Disposed))
            {
                behavior.Dispose();
            }

            base.Dispose();
        }

        private Behavior _currentBehavior;
        public override async Task<bool> BehaviorRoutine()
        {
            if (await base.BehaviorRoutine()) return true;
            if (IsDone) return false;

            if (_currentBehavior == null)
            {
                while (Behaviors.Count > 0)
                {
                    if (!Behaviors[0].CheckCriteria())
                        Behaviors.RemoveAt(0);
                    else
                    {
                        _currentBehavior = Behaviors[0];
                        _currentBehavior.Initalize();
                        break;
                    }
                }
            }

            if (_currentBehavior != null)
            {
                if (await _currentBehavior.BehaviorRoutine()) return true;
                if (!_currentBehavior.Disposed) _currentBehavior.Dispose();
                Behaviors.RemoveAt(0);
                _currentBehavior = null;

                return true;
            }

            return false;
        }

        public override string ToString()
        {
            string childrenStrs = Behaviors.Aggregate(string.Empty, (current, b) => current + b.ToString() + "\r\n");
            return String.Format("{0} Criteria Check {1} IsDone {2}\r\n" +
                                 "{3}", Type, CheckCriteria(), IsDone, childrenStrs);
        }


    }
}