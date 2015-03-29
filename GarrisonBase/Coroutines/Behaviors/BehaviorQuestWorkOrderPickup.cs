using System;
using Herbfunk.GarrisonBase.Garrison.Objects;

namespace Herbfunk.GarrisonBase.Coroutines.Behaviors
{
    public class BehaviorQuestWorkOrderPickup : BehaviorWorkOrderPickUp
    {
        public BehaviorQuestWorkOrderPickup(Building building) : base(building)
        {
            
        }

        public override Func<bool> Criteria
        {
            get { return () => true; }
        }
    }
}
