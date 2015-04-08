using Herbfunk.GarrisonBase.Garrison.Objects;

namespace Herbfunk.GarrisonBase.Coroutines.Behaviors
{
    public class BehaviorQuestWorkOrderPickup : BehaviorWorkOrderPickUp
    {
        public BehaviorQuestWorkOrderPickup(Building building) : base(building)
        {
            Criteria = () => true;
        }


    }
}
