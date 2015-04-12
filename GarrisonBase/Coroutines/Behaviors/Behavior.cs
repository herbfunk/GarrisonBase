using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Herbfunk.GarrisonBase.Cache;
using Herbfunk.GarrisonBase.Cache.Objects;
using Styx;
using Styx.CommonBot;

namespace Herbfunk.GarrisonBase.Coroutines.Behaviors
{
    public abstract class Behavior
    {
        #region Constructors

        protected Behavior(WoWPoint movepoint)
        {
            MovementPoints.Add(movepoint);
        }
        protected Behavior(WoWPoint movepoint, int interactionEntryId, Func<bool> criteria = null)
        {
            MovementPoints.Add(movepoint);
            InteractionEntryId = interactionEntryId;

            if (criteria != null) _criteria = criteria; else _criteria += () => true;
        }
        protected Behavior(WoWPoint[] movepoints, int interactionEntryId, Func<bool> criteria = null)
        {
            MovementPoints.AddRange(movepoints);
            InteractionEntryId = interactionEntryId;

            if (criteria != null) _criteria = criteria; else _criteria += () => true;
        }
        protected Behavior(Func<bool> criteria = null)
        {
            if (criteria != null) _criteria = criteria; else _criteria += () => true;
        }

        #endregion


        /// <summary>
        /// The universal method used by all behaviors for execution.
        /// </summary>
        /// <returns></returns>
        public virtual async Task<bool> BehaviorRoutine()
        {
            TreeRoot.StatusText = GetStatusText;

            if (!Initalized)
            {
                GarrisonBase.Debug("Initalizing Behavior {0}", Type);
                Initalize();
            }
            if (!IsDone) IsDone = !CheckRunCondition();

            if (IsDone)
            {
                if (!Disposed)
                {
                    GarrisonBase.Debug("Disposing Behavior {0}", Type);
                    Dispose();
                }
                return false;
            }

            //await Coroutine.Yield();
            return await Common.CheckCommonCoroutines();
        }


        /// <summary>
        /// Checks if this behavior meets the defined requirements to run
        /// This check will occur only once!
        /// </summary>
        /// <returns></returns>
        public Func<bool> Criteria
        {
            get { return _criteria; }
            set { _criteria = value; }
        }
        private Func<bool> _criteria;

        /// <summary>
        /// Conditional function that is checked every call. Returning false will end the behavior (IsDone=True)
        /// </summary>
        public Func<bool> RunCondition
        {
            get { return _runcondition; }
            set { _runcondition = value; }
        }
        private Func<bool> _runcondition = () => true;

        public bool CheckCriteria()
        {
            return Criteria.GetInvocationList().Cast<Func<bool>>().All(f => f());
        }
        public bool CheckRunCondition()
        {
            return RunCondition.GetInvocationList().Cast<Func<bool>>().All(f => f());
        }


        public bool Initalized { get; set; }
        /// <summary>
        /// This occurs only once to initalize any variable only after the Criteria check occurs and passes.
        /// </summary>
        public virtual void Initalize()
        {
            StartMovement = new Movement(MovementPoints.ToArray(), 2f, false, String.Format("StartMovement({0})", Type.ToString()));
            EndMovement = new Movement(MovementPoints.ToArray().Reverse().ToArray(), 2f, false, String.Format("EndMovement({0})", Type.ToString()));
            _interactionObject = null;
            IsDone = false;
            Initalized = true;
            GarrisonBase.Debug("Behavior {0} has been initalized", Type);
        }

        public bool Disposed { get; set; }
        /// <summary>
        /// This occurs only once after the behavior is considered done.
        /// </summary>
        public virtual void Dispose()
        {
            IsDone = true;
            Disposed = true;
            GarrisonBase.Debug("Behavior {0} has been disposed", Type);
        }

        /// <summary>
        /// A convient way to end the behavior!
        /// </summary>
        public virtual bool IsDone { get; set; }

        public virtual BehaviorType Type { get { return BehaviorType.None; } }
        /// <summary>
        /// The Entry Id of the WoW object!
        /// </summary>
        public int InteractionEntryId { get; set; }
        /// <summary>
        /// The interaction object
        /// </summary>
        public virtual C_WoWObject InteractionObject
        {
            get
            {
                if (_interactionObject == null || !_interactionObject.IsValid)
                    _interactionObject = GetInteractionObject(InteractionEntryId);
                else if (!_interactionObject.IsValid)
                    _interactionObject = null;

                return _interactionObject;
            }
            set { _interactionObject = value; }
        }
        private C_WoWObject _interactionObject;

        /// <summary>
        /// The default function we use to find and set the value of InteractionObject
        /// </summary>
        public Func<int, C_WoWObject> GetInteractionObject
        {
            get { return _getinteractionobject; }
            set { _getinteractionobject = value; }
        }
        private Func<int, C_WoWObject> _getinteractionobject = (i) => ObjectCacheManager.GetWoWObject(i);

        public bool InteractionObjectValid
        {
            get
            {
                return InteractionObject != null && InteractionObject.IsValid;
            }
        }

        //Movement behaviors that use the WoWPoints given in construction of behavior class
        public Movement StartMovement;
        public Movement EndMovement;
        public List<WoWPoint> MovementPoints = new List<WoWPoint>();

        public BehaviorArray Parent { get; set; }
        public BehaviorArray TopParent { get; set; }

        public virtual string GetStatusText
        {
            get
            {
                return String.Format("Behavior {0} ", Type.ToString());
            }
        }

        public override string ToString()
        {
            return String.Format("{0} IsDone {1} Initalized {2} Disposed {3}",
                Type,  IsDone, Initalized, Disposed);
        }

        public static void ResetBehavior(Behavior behavior, bool allParents=false)
        {
            if (behavior.Parent != null)
            {
                if (behavior.Parent.Parent != null)
                {
                    BehaviorArray _currentParent = behavior.Parent.Parent;
                    _currentParent.Dispose();
                    _currentParent.Initalize();
                }
                
                behavior.Parent.Dispose();
                behavior.Parent.Initalize();
            }
            else
            {
                behavior.Dispose();
                behavior.Initalize();
            }
        }
    }

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

    public enum BehaviorType
    {
        None,
        Array,
        WorkOrderRush,
        WorkOrderPickUp,
        WorkOrderStartUp,
        Salvage,
        MissionsComplete,
        MissionsStart,
        Cache,
        FinalizeBuilding,
        GarrisonReturn,
        Mining,
        Herbing,
        QuestPickup,
        QuestTurnin,
        QuestAbandon,
        HotspotRunning,
        ProfessionCrafting,
        SellRepair,
        MoveTo,
        Disenchanting,
        Mail,
        PrimalTrader,
        Taxi,
        Milling,
        Sleep,
    }

}
