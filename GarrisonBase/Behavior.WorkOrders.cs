using System;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Herbfunk.GarrisonBase.Cache;
using Herbfunk.GarrisonBase.Cache.Objects;
using Herbfunk.GarrisonBase.Garrison.Enums;
using Herbfunk.GarrisonBase.Garrison.Objects;
using Styx;
using Styx.CommonBot;
using Styx.CommonBot.Coroutines;
using Styx.Pathing;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace Herbfunk.GarrisonBase
{
    public partial class Behaviors
    {
        public class BehaviorWorkOrderPickUp : Behavior
        {
            public override BehaviorType Type { get { return BehaviorType.WorkOrderPickUp; } }

            public BehaviorWorkOrderPickUp(Building building)
                : base(building.SafeMovementPoint, building.WorkOrderObjectEntryId)
            {
                Building = building;
            }
            public override void Initalize()
            {
                base.Initalize();
            }

            public Building Building { get; set; }

            public C_WoWGameObject WorkOrderObject
            {
                get
                {
                    C_WoWGameObject obj;
                    obj=ObjectCacheManager.GetWoWGameObjects(Building.WorkOrderObjectName).FirstOrDefault() ??
                        ObjectCacheManager.GetWoWGameObjects("Crate").FirstOrDefault();
                    return obj;
                }
            }

            public override Func<bool> Criteria
            {
                get
                {
                    return () => BaseSettings.CurrentSettings.BehaviorWorkOrderPickup &&
                        BaseSettings.CurrentSettings.WorkOrderTypes.HasFlag(Building.WorkOrderType) &&
                        !Building.CheckedWorkOrderPickUp && Building.WorkOrder.Pickup > 0;
                }
            }


            private Movement _movement;

            public override async Task<bool> Interaction()
            {
                if (Building.CheckedWorkOrderPickUp)
                    return false;

                if (WorkOrderObject == null || !WorkOrderObject.IsValid)
                {
                    //Error Cannot find object!
                    Building.CheckedWorkOrderPickUp = true;
                    Building.WorkOrder.Refresh();
                    return false;
                }

                if (WorkOrderObject.WithinInteractRange)
                {
                    if (StyxWoW.Me.IsMoving) 
                        await CommonCoroutines.StopMoving();

                    if (WorkOrderObject.GetCursor == WoWCursorType.PointCursor)
                    {
                        //Nothing to pickup!
                        Building.CheckedWorkOrderPickUp = true;
                        await Coroutine.Sleep(StyxWoW.Random.Next(755, 1449));
                        await Coroutine.Yield();
                        Building.WorkOrder.Refresh();
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


                //Move to the interaction object
                if (_movement == null || _movement.CurrentMovementQueue.Count==0)
                    _movement = new Movement(WorkOrderObject.Location, WorkOrderObject.InteractRange-0.25f);
                
                await _movement.MoveTo();

                return true;
            }

            public override async Task<bool> BehaviorRoutine()
            {
                if (IsDone) return false;

                if (await base.BehaviorRoutine()) return true;

                if (await StartMovement.MoveTo()) return true;

                if (await Interaction()) return true;

                if (await EndMovement.MoveTo()) return true;

                return false;
            }

            public override string GetStatusText
            {
                get { return base.GetStatusText + Building.Type.ToString(); }
            }
        }

        public class BehaviorWorkOrderStartUp : Behavior
        {
            public BehaviorWorkOrderStartUp(Building building)
                : base(building.SafeMovementPoint, building.WorkOrderNPCEntryId)
            {
                Building = building;

            }

            public override BehaviorType Type { get { return BehaviorType.WorkOrderStartUp; } }
            public Building Building { get; set; }
            public C_WoWUnit WorkOrderObject
            {
                get { return ObjectCacheManager.GetWoWUnits(Building.WorkOrderNPCEntryId).FirstOrDefault(); }
            }
           
            public override Func<bool> Criteria
            {
                get
                {
                    return () => BaseSettings.CurrentSettings.BehaviorWorkOrderStartup &&
                           BaseSettings.CurrentSettings.WorkOrderTypes.HasFlag(Building.WorkOrderType) &&
                           !Building.CheckedWorkOrderStartUp &&
                           Building.WorkOrder != null &&
                           Building.WorkOrder.Type != WorkOrderType.None &&
                           Building.WorkOrder.Pending < Building.WorkOrder.Maximum &&
                           Building.WorkOrder.TotalWorkorderStartups() > 0;
                }
            }

            public override void Initalize()
            {
                if (Building.Type == BuildingType.WarMillDwarvenBunker)
                {
                    if (Building.Level == 2)
                    {
                        if (Building.PlotId == 23)
                            _specialMovement = new Movement(MovementCache.Plot23_Warmill_Level2.ToArray(), 2f);
                        else
                            _specialMovement = new Movement(MovementCache.Plot24_Warmill_Level2.ToArray(), 2f);
                    }
                    else if (Building.Level == 3)
                    {
                        if (Building.PlotId == 23)
                            _specialMovement = new Movement(MovementCache.Plot23_Warmill_Level3.ToArray(), 2f);
                        else
                            _specialMovement = new Movement(MovementCache.Plot24_Warmill_Level3.ToArray(), 2f);
                    }
                }

                MovementPoints.Add(Building.EntranceMovementPoint);
                base.Initalize();
            }




            public override async Task<bool> BehaviorRoutine()
            {
                if (IsDone) return false;

                if (await base.BehaviorRoutine()) return true;

                if (await StartMovement.MoveTo())
                    return true;

                if (await Movement())
                    return true;

                if (await Interaction())
                    return true;

                if (_specialMovement != null && await _specialMovement.ClickToMove()) 
                    return true;

                if (await EndMovement.MoveTo())
                    return true;

                return false;
            }

           



            private int _interactionAttempts = 0;
            public override async Task<bool> Movement()
            {
                if (Building.CheckedWorkOrderStartUp || WorkOrderObject == null)
                {
                    //Error Cannot find object!
                    Building.CheckedWorkOrderStartUp = true;
                    return false;
                }



                if (WorkOrderObject.WithinInteractRange)
                {
                    if (StyxWoW.Me.IsMoving)  await CommonCoroutines.StopMoving();
                    await CommonCoroutines.SleepForLagDuration();

                    if (_interactionAttempts > 3)
                    {
                        GarrisonBase.Log("Interaction Attempts for {0} has exceeded 3! Preforming movement..",
                            WorkOrderObject.Name);
                        _movement = new Movement(Building.EntranceMovementPoint, 6.7f);
                        _interactionAttempts = 0;
                        return true;
                    }

                    if (LuaCommands.IsGarrisonCapacitiveDisplayFrame())
                    {
                        //Workorder frame is displayed!
                        return false;
                    }
                    _interactionAttempts++;
                    WorkOrderObject.Interact();
                    await CommonCoroutines.SleepForRandomUiInteractionTime();
                    return true;
                }

                if (_specialMovement != null)
                {//Special Movement for navigating inside buildings using Click To Move

                    if (_specialMovement.CurrentMovementQueue.Count > 0)
                    {
                        //find the nearest point to the npc in our special movement queue 
                        var nearestPoint = Herbfunk.GarrisonBase.Movement.FindNearestPoint(WorkOrderObject.Location, _specialMovement.CurrentMovementQueue.ToList());
                        //click to move.. but don't dequeue
                        var result = await _specialMovement.ClickToMove_Result(false);

                        if (!nearestPoint.Equals(_specialMovement.CurrentMovementQueue.Peek()))
                        {
                            //force dequeue now since its not nearest point
                            if (result == MoveResult.ReachedDestination)
                                _specialMovement.ForceDequeue(true);

                            return true;
                        }


                        //Last position was nearest and we reached our destination.. so lets finish special movement!
                        if (result == MoveResult.ReachedDestination)
                        {
                            _specialMovement.DequeueAll(false);
                        }
                    }
                    

                    if (_movement == null || _movement.CurrentMovementQueue.Count == 0)
                        _movement = new Movement(WorkOrderObject.Location, 4f);

                    //since we are navigating inside building.. we must continue to use CTM
                    if (await _movement.ClickToMove())
                        return true;
                }

                if (_movement == null || _movement.CurrentMovementQueue.Count == 0)
                    _movement = new Movement(WorkOrderObject.Location, 4f);
                
                if (await _movement.MoveTo()) 
                    return true;

                return !WorkOrderObject.WithinInteractRange;
            }

            private Movement _movement;
            private Movement _specialMovement;

            public override async Task<bool> Interaction()
            {
                if (Building.CheckedWorkOrderStartUp)
                    return false;

                if (Building.WorkOrder.TotalWorkorderStartups() == 0)
                {
                    Building.CheckedWorkOrderStartUp = true;
                    if (_specialMovement != null) _specialMovement.UseDeqeuedPoints(true);
                    return false;
                }

                TreeRoot.StatusText = String.Format("Behavior {0} [{1}] Interaction", Type.ToString(), Building.Type);
                if (LuaEvents.ShipmentOrderFrameOpen)
                {
                    if (BaseSettings.CurrentSettings.DEBUG_FAKESTARTWORKORDER || !LuaCommands.ClickStartOrderButtonEnabled())
                    {
                        Building.CheckedWorkOrderStartUp = true;
                        if (_specialMovement!=null) _specialMovement.UseDeqeuedPoints(true);
                        GarrisonBase.Log("Order Button Disabled!");
                        Building.WorkOrder.Refresh();
                        return false;
                    }

                    await CommonCoroutines.SleepForRandomUiInteractionTime();

                    if (Building.Type == BuildingType.WarMillDwarvenBunker)
                        LuaCommands.ClickStartOrderButton();
                    else
                        LuaCommands.ClickStartAllOrderButton();

                    await Coroutine.Yield();
                    return true;
                }
                return false;
            }

            public override string GetStatusText
            {
                get { return base.GetStatusText + Building.Type.ToString(); }
            }
        }

        public class BehaviorWorkOrderRush : Behavior
        {
            public override BehaviorType Type { get { return BehaviorType.WorkOrderRush; } }

            public BehaviorWorkOrderRush(Building building)
            {
                Building = building;
            }
            public Building Building { get; set; }

            public override Func<bool> Criteria
            {
                get
                {
                    return
                        () =>
                            Building.WorkOrder != null &&
                            Building.WorkOrder.Type != WorkOrderType.None &&
                            (Building.WorkOrder.Pending-Building.WorkOrder.Pickup) >= 5 &&
                            Player.Inventory.GetBagItemsById((int) Building.WorkOrder.RushOrderItemType).Count > 0;
                }
            }

            public override async Task<bool> BehaviorRoutine()
            {
                if (IsDone) return false;

                if (await base.BehaviorRoutine()) return true;
                
                if (Building.WorkOrder.Pending - Building.WorkOrder.Pickup <= 5) return false;

                var rushOrderItems = Player.Inventory.GetBagItemsById((int) Building.WorkOrder.RushOrderItemType);
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
}
