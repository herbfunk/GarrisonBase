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
                        Building.WorkOrder.Pending -= Building.WorkOrder.Pickup;
                        Building.WorkOrder.Pickup = 0;
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
                    _movement = new Movement(WorkOrderObject.Location, WorkOrderObject.InteractRange);
                
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

           



            private int InteractionAttempts = 0;
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

                    if (InteractionAttempts > 3)
                    {
                        GarrisonBase.Log("Interaction Attempts for {0} has exceeded 3! Preforming movement..",
                            WorkOrderObject.Name);
                        _movement = new Movement(Building.EntranceMovementPoint, 6.7f);
                        InteractionAttempts = 0;
                        return true;
                    }

                    if (LuaCommands.IsGarrisonCapacitiveDisplayFrame())
                    {
                        //Workorder frame is displayed!
                        return false;
                    }
                    InteractionAttempts++;
                    WorkOrderObject.Interact();
                    await CommonCoroutines.SleepForRandomUiInteractionTime();
                    return true;
                }

                if (_specialMovement != null && _specialMovement.CurrentMovementQueue.Count > 0)
                {

                    var nearestPoint = Herbfunk.GarrisonBase.Movement.FindNearestPoint(WorkOrderObject.Location, _specialMovement.CurrentMovementQueue.ToList());
                    var result = await _specialMovement.ClickToMove_Result(false);

                    if (!nearestPoint.Equals(_specialMovement.CurrentMovementQueue.Peek()))
                    {
                        if (result == MoveResult.ReachedDestination)
                            _specialMovement.ForceDequeue(true);
                        
                        return true;
                    }


                    ////Last position was nearest and we reached our destination.. so lets finish special movement!
                    if (result == MoveResult.ReachedDestination)
                    {
                        _specialMovement.DequeueAll(false);
                    }
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
                        return false;
                    }

                    await CommonCoroutines.SleepForRandomUiInteractionTime();
                    LuaCommands.ClickStartOrderButton();
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
    }
}
