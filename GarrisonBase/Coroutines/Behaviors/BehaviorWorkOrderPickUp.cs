using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Herbfunk.GarrisonBase.Cache;
using Herbfunk.GarrisonBase.Cache.Enums;
using Herbfunk.GarrisonBase.Cache.Objects;
using Herbfunk.GarrisonBase.Garrison.Enums;
using Herbfunk.GarrisonBase.Garrison.Objects;
using Styx;
using Styx.CommonBot.Coroutines;
using Styx.Pathing;

namespace Herbfunk.GarrisonBase.Coroutines.Behaviors
{
    public class BehaviorWorkOrderPickUp : Behavior
    {
        public override BehaviorType Type { get { return BehaviorType.WorkOrderPickUp; } }

        public BehaviorWorkOrderPickUp(Building building)
            : base(building.SafeMovementPoint, building.WorkOrderObjectEntryId)
        {
            Building = building;

            Criteria += () => BaseSettings.CurrentSettings.BehaviorWorkOrderPickup &&
                             !Building.CanActivate && !Building.IsBuilding &&
                             BaseSettings.CurrentSettings.WorkOrderTypes.HasFlag(Building.WorkOrderType) &&
                             !Building.CheckedWorkOrderPickUp && Building.WorkOrder.Pickup > 0;
        }
        public override void Initalize()
        {
            _movement = null;

            //if (Building.Type == BuildingType.Barn && Building.SpecialMovementPoints != null)
            //{
            //    _specialMovement = new Movement(Building.SpecialMovementPoints.ToArray(), 2f, name: "BarnSpecialMovement");
            //}

            base.Initalize();
        }

        public Building Building { get; set; }

        public C_WoWGameObject WorkOrderObject
        {
            get
            {
                C_WoWGameObject obj = 
                    //ObjectCacheManager.GetGameObjectsNearPoint(Building.EntranceMovementPoint, 150f, Building.WorkOrderObjectName).FirstOrDefault() 
                    //??
                    ObjectCacheManager.GetGameObjectsNearPoint(Building.EntranceMovementPoint, 150f, WoWObjectTypes.GarrisonShipment).FirstOrDefault();

                return obj;
            }
        }



        private Movement _movement;

        private async Task<bool> Interaction()
        {
            if (Building.CheckedWorkOrderPickUp)
                return false;

            if (WorkOrderObject == null || !WorkOrderObject.IsValid)
            {
                //Error Cannot find object!
                GarrisonBase.Debug("Workorder Pickup object null!");
                Building.CheckedWorkOrderPickUp = true;
                await Coroutine.Sleep(StyxWoW.Random.Next(755, 1449));
                await Coroutine.Yield();
                Building.WorkOrder.Refresh();
                //if (_specialMovement != null) _specialMovement.UseDeqeuedPoints(true);
                return false;
            }

            if (WorkOrderObject.WithinInteractRange)
            {
                if (StyxWoW.Me.IsMoving) 
                    await CommonCoroutines.StopMoving();

                if (BaseSettings.CurrentSettings.DEBUG_FAKEPICKUPWORKORDER)
                {
                    Building.CheckedWorkOrderPickUp = true;
                    //if (_specialMovement != null) _specialMovement.UseDeqeuedPoints(true);
                    return false;
                }

                if (WorkOrderObject.GetCursor == WoWCursorType.PointCursor)
                {
                    //Nothing to pickup!
                    GarrisonBase.Debug("Workorder Pickup Object Non-loot Cursor!");
                    Building.CheckedWorkOrderPickUp = true;
                    await Coroutine.Sleep(StyxWoW.Random.Next(755, 1449));
                    await Coroutine.Yield();
                    //Building.WorkOrder.Refresh();
                    Building.WorkOrder.Pending -= Building.WorkOrder.Pickup;
                    Building.WorkOrder.Pickup = 0;
                    //if (_specialMovement != null) _specialMovement.UseDeqeuedPoints(true);
                    return false;
                }

                WorkOrderObject.Interact();
                await CommonCoroutines.SleepForRandomUiInteractionTime();
                await Coroutine.Yield();

                if (StyxWoW.LastRedErrorMessage.Contains("Inventory is full."))
                {
                    GarrisonBase.Log("Stopping Work Order Pickup due to inventory full!");
                    return false;
                }

                return true;
            }


            //if (_specialMovement != null)
            //{//Special Movement for navigating inside buildings using Click To Move

            //    if (_specialMovement.CurrentMovementQueue.Count > 0)
            //    {
            //        //find the nearest point to the npc in our special movement queue 
            //        var nearestPoint = Coroutines.Movement.FindNearestPoint(WorkOrderObject.Location, _specialMovement.CurrentMovementQueue.ToList());
            //        //click to move.. but don't dequeue
            //        var result = await _specialMovement.ClickToMove_Result(false);

            //        if (!nearestPoint.Equals(_specialMovement.CurrentMovementQueue.Peek()))
            //        {
            //            //force dequeue now since its not nearest point
            //            if (result == MoveResult.ReachedDestination)
            //                _specialMovement.ForceDequeue(true);

            //            return true;
            //        }


            //        //Last position was nearest and we reached our destination.. so lets finish special movement!
            //        if (result == MoveResult.ReachedDestination)
            //        {
            //            _specialMovement.ForceDequeue(true);
            //            _specialMovement.DequeueAll(false);
            //        }
            //    }
            //}


            //Move to the interaction object
            if (_movement == null)
                _movement = new Movement(WorkOrderObject, WorkOrderObject.InteractRange-0.25f, name: "WorkOrderPickup " + WorkOrderObject.Name);


            await _movement.MoveTo(false);
            return true;
        }

        public override async Task<bool> BehaviorRoutine()
        {
            if (await base.BehaviorRoutine()) return true;
            if (IsDone) return false;

            if (await StartMovement.MoveTo()) return true;

            if (await Interaction()) return true;

            //if (_specialMovement != null && await _specialMovement.ClickToMove())
            //    return true;

            if (await EndMovement.MoveTo()) return true;

            return false;
        }

        public override string GetStatusText
        {
            get { return base.GetStatusText + Building.Type.ToString(); }
        }
    }
}