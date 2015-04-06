using System;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Herbfunk.GarrisonBase.Garrison.Enums;
using Herbfunk.GarrisonBase.Garrison.Objects;
using Styx.CommonBot.Coroutines;

namespace Herbfunk.GarrisonBase.Coroutines.Behaviors
{
    public class BehaviorWorkOrderRush : Behavior
    {
        public override BehaviorType Type { get { return BehaviorType.WorkOrderRush; } }

        public BehaviorWorkOrderRush(Building building)
        {
            Building = building;
            Criteria += () => Building.WorkOrder != null &&
                        Building.WorkOrder.Type != WorkOrderType.None &&
                        (Building.WorkOrder.Pending - Building.WorkOrder.Pickup) >= 5 &&
                        Character.Player.Inventory.GetBagItemsById((int)Building.WorkOrder.RushOrderItemType).Count > 0;
        }
        public Building Building { get; set; }



        public override async Task<bool> BehaviorRoutine()
        {
            if (await base.BehaviorRoutine()) return true;
            if (IsDone) return false;
                
            if (Building.WorkOrder.Pending - Building.WorkOrder.Pickup <= 5) return false;

            var rushOrderItems = Character.Player.Inventory.GetBagItemsById((int) Building.WorkOrder.RushOrderItemType);
            if (rushOrderItems.Count > 0)
            {
                var item = rushOrderItems[0];
                await CommonCoroutines.WaitForLuaEvent(
                    "BAG_UPDATE",
                    10000,
                    () => false,
                    item.Interact);
                await CommonCoroutines.SleepForRandomUiInteractionTime();
                await Coroutine.Sleep(3500);
                Building.WorkOrder.Refresh();
                return true;
            }

            return false;
        }
    }
}