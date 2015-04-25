using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Styx.CommonBot;

namespace Herbfunk.GarrisonBase.Coroutines.Behaviors
{
    /// <summary>
    /// Encapsulates multiple behaviors and runs each one, removing those that failed or did not pass critiera check.
    /// </summary>
    public class BehaviorArray : Behavior
    {
        public override BehaviorType Type { get { return BehaviorType.Array; } }

        public Queue<Behavior> Behaviors = new Queue<Behavior>();
        private readonly Behavior[] _behaviors;

        public string Name { get; set; }

        public BehaviorArray(Behavior[] behaviors, string arrayname="")
        {
            _behaviors = behaviors;
            Name = arrayname;
        }
        

        public override string GetStatusText
        {
            get
            {
                return _currentBehavior != null ? _currentBehavior.GetStatusText :
                    String.Format("Behavior {0} {1} ", Type.ToString(), Name);
            }
        }

        public override void Initalize()
        {
            _shouldInitalize = true;
            IsDone = false;
            Initalized = true;
        }

        private bool _shouldInitalize = false;
        private void _initalize()
        {
            //Clear behavior queue and current
            _currentBehavior = null;
            Behaviors.Clear();

            //Clone the given behaviors
            var newBehaviors = (Behavior[])_behaviors.Clone();

            //Set the behaviors up and queue 'em!
            foreach (var behavior in newBehaviors)
            {
                behavior.Criteria += Criteria;
                behavior.Parent = this;
                behavior.TopParent = TopParent != null ? TopParent : Parent!=null?Parent:this;
                Behaviors.Enqueue(behavior);
            }

            base.Initalize();
            _shouldInitalize = false;
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

            if (_shouldInitalize) _initalize();
            
            if (_currentBehavior == null)
            {
                while (Behaviors.Count > 0)
                {
                    if (!Behaviors.Peek().CheckCriteria())
                        Behaviors.Dequeue();
                    else
                    {
                        _currentBehavior = Behaviors.Peek();
                        _currentBehavior.Initalize();
                        break;
                    }
                }
            }

            if (_currentBehavior != null)
            {
                if (await _currentBehavior.BehaviorRoutine()) return true;
                if (!_currentBehavior.Disposed) _currentBehavior.Dispose();
                Behaviors.Dequeue();
                _currentBehavior = null;
                if (_shouldInitalize) _initalize();
            
                return true;
            }

            return false;
        }

        public override Behavior Clone()
        {
            var behaviors = (Behavior[])_behaviors.Clone();
            BehaviorArray clone = new BehaviorArray(behaviors, Name);
            return clone;
        }

        public override string ToString()
        {
            string childrenStrs = Behaviors.Aggregate(string.Empty, (current, b) => current + b.ToString() + "\r\n\t");
            return String.Format("{0} ({5}) IsDone {3}\r\n" +
                                 "{1} {2}\r\n" +
                                 "Children:\r\n{4}", 
                Type,
                Parent != null ? "Parent[" + Parent.GetHashCode().ToString() + "]" : "",
                TopParent != null ? "TopParent[" + TopParent.GetHashCode().ToString() + "]" : "",
                IsDone,
                childrenStrs,
                GetHashCode().ToString());
        }


    }
}