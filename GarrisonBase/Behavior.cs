using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Herbfunk.GarrisonBase.Cache;
using Herbfunk.GarrisonBase.Cache.Objects;
using Styx;
using Styx.CommonBot;
using Styx.CommonBot.Coroutines;
using Styx.WoWInternals.WoWObjects;

namespace Herbfunk.GarrisonBase
{
    public partial class Behaviors
    {
        public enum BehaviorType
        {
            None,

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
            ProfessionCrafting,
            SellRepair,
            MoveTo,
            Disenchanting,
            Mail,
            PrimalTrader,
        }

        public abstract class Behavior
        {
            protected Behavior(WoWPoint movepoint)
            {
                MovementPoints.Add(movepoint);
            }
            protected Behavior(WoWPoint movepoint, int interactionEntryId, Func<bool> criteria=null )
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

            /// <summary>
            /// Checks if this behavior meets the defined requirements to run
            /// This check will occur only once!
            /// </summary>
            /// <returns></returns>
            public virtual Func<bool> Criteria
            {
                get { return _criteria; }
                set { _criteria = value; }
            }
            private Func<bool> _criteria;

            /// <summary>
            /// This occurs only once to initalize any variable only after the Criteria check occurs and passes.
            /// </summary>
            public virtual void Initalize()
            {
                StartMovement = new Movement(MovementPoints.ToArray(), 2f);
                EndMovement = new Movement(MovementPoints.ToArray().Reverse().ToArray(), 2f);
            }

            /// <summary>
            /// A convient way to end the behavior!
            /// </summary>
            public virtual bool IsDone { get; set; }
            public virtual BehaviorType Type { get{return BehaviorType.None;} }
            public int InteractionEntryId { get; set; }

            public virtual string GetStatusText
            {
                get
                {
                    return String.Format("Behavior {0} ", Type.ToString());
                }
            }
            public virtual async Task<bool> BehaviorRoutine()
            {
                TreeRoot.StatusText = GetStatusText;

                if (IsDone) return false;
                await Coroutine.Yield();
                return await Coroutines.PreCheckCoroutines();
            }



            public bool CheckCriteria()
            {
                return Criteria.GetInvocationList().Cast<Func<bool>>().All(f => f());
            }

            /// <summary>
            /// Attempts to interact with the object matching the InteractionEntryId
            /// </summary>
            /// <returns></returns>
            public virtual async Task<bool> Interaction()
            {
                return false;
            }

            /// <summary>
            /// Attempts to move to the MovementPoint than to the object matching the InteractionEntryId
            /// </summary>
            /// <returns></returns>
            public virtual async Task<bool> Movement()
            {
                return await StartMovement.MoveTo();
            }
            public Movement StartMovement;
            public Movement EndMovement;
            public List<WoWPoint> MovementPoints=new List<WoWPoint>();

            public virtual C_WoWObject InteractionObject
            {
                get
                {
                    if (_interactionObject == null || !_interactionObject.IsValid)
                        _interactionObject = ObjectCacheManager.GetWoWObject(InteractionEntryId);
                    else if (!_interactionObject.IsValid)
                        _interactionObject = null;

                    return _interactionObject;
                }
                set { _interactionObject = value; }
            }
            private C_WoWObject _interactionObject;
        }

        public class BehaviorArray : Behavior
        {
            public readonly Behavior[] Behaviors;
            public BehaviorArray(Behavior[] behaviors)
            {
                Behaviors = behaviors;
            }
            public override void Initalize()
            {
                foreach (var behavior in Behaviors)
                {
                    behavior.Initalize();
                }
            }
            public override async Task<bool> BehaviorRoutine()
            {
                if (IsDone) return false;

                foreach (var b in Behaviors)
                {
                    if (await b.BehaviorRoutine())
                        return true;
                }

                return false;
            }
        }
    }

}
