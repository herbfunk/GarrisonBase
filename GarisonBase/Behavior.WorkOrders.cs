using System;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Herbfunk.GarrisonBase.Cache;
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
                if (Building.BuildingPolygon != null && !Building.BuildingPolygon.LocationInsidePolygon(StyxWoW.Me.Location))
                    MovementPoints.Insert(0, Building.BuildingPolygon.Entrance);

                base.Initalize();
            }

            public Building Building { get; set; }
            public override C_WoWObject InteractionObject
            {
                get
                {
                    if (_interactionObject == null)
                        _interactionObject = ObjectCacheManager.GetWoWObjects(Building.WorkOrderObjectName).FirstOrDefault();
                    else if (!_interactionObject.IsValid)
                        _interactionObject = null;
                    return _interactionObject;
                }
                set { _interactionObject = value; }
            }
            private C_WoWObject _interactionObject;

            public C_WoWGameObject WorkOrderObject
            {
                get { return ObjectCacheManager.GetWoWGameObjects(Building.WorkOrderObjectName).FirstOrDefault(); }
            }

            public override Func<bool> Criteria
            {
                get
                {
                    return () => BaseSettings.CurrentSettings.BehaviorWorkOrderPickup && !Building.CheckedWorkOrderPickUp && Building.WorkOrder.Pickup > 0;
                }
            }
            public override async Task<bool> Movement()
            {
                if (WorkOrderObject != null)
                {
                    if (WorkOrderObject.GetCursor == WoWCursorType.PointCursor)
                    {
                        //Nothing to pickup!
                        Building.CheckedWorkOrderPickUp = true;
                        return false;
                    }
                }

                TreeRoot.StatusText = String.Format("Behavior {0} [{1}] Movement", Type.ToString(), Building.Type);
                if (await base.Movement()) return true;

                TreeRoot.StatusText = String.Format("Behavior {0} [{1}] Movement2", Type.ToString(), Building.Type);
                if (WorkOrderObject == null)
                {
                    //Error Cannot find object!
                    Building.CheckedWorkOrderPickUp = true;
                    return false;
                }

                //Move to the interaction object (within 6.7f)
                if (_movement == null)
                {
                    _movement = new Movement(WorkOrderObject.Location, 6.7f);
                }

                return await _movement.MoveTo();
            }

            private Movement _movement;

            public override async Task<bool> Interaction()
            {
                TreeRoot.StatusText = String.Format("Behavior {0} [{1}] Interaction", Type.ToString(), Building.Type);
                if (WorkOrderObject == null || !WorkOrderObject.IsStillValid())
                {
                    //Error Cannot find object!
                    Building.CheckedWorkOrderPickUp = true;
                    return false;
                }

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

            public override async Task<bool> BehaviorRoutine()
            {
                if (IsDone) return false;

                if (await base.BehaviorRoutine()) return true;

                if (await Movement())
                    return true;

                if (await Interaction())
                    return true;

                return false;
            }
        }

        public class BehaviorWorkOrderStartUp : Behavior
        {
            public override BehaviorType Type { get { return BehaviorType.WorkOrderStartUp; } }

            public BehaviorWorkOrderStartUp(Building building)
                : base(building.SafeMovementPoint, building.WorkOrderNPCEntryId)
            {
                Building = building;
                _finalmovement = new Movement(building.SafeMovementPoint);
            }

            public override void Initalize()
            {
                if (Building.BuildingPolygon != null && !Building.BuildingPolygon.LocationInsidePolygon(StyxWoW.Me.Location))
                    MovementPoints.Insert(0, Building.BuildingPolygon.Entrance);

                MovementPoints.Add(Building.EntranceMovementPoint);

                base.Initalize();
            }

            public C_WoWUnit WorkOrderObject
            {
                get { return ObjectCacheManager.GetWoWUnits(Building.WorkOrderNPCEntryId).FirstOrDefault(); }
            }

            public override async Task<bool> BehaviorRoutine()
            {
                if (IsDone) return false;

                if (await base.BehaviorRoutine()) return true;

                if (await Movement())
                    return true;
                if (await Interaction())
                    return true;
                if (await _finalmovement.MoveTo())
                    return true;

                return false;
            }

            public Building Building { get; set; }

            public override Func<bool> Criteria
            {
                get
                {
                    return () => BaseSettings.CurrentSettings.BehaviorWorkOrderStartup &&
                           !Building.CheckedWorkOrderStartUp &&
                           Building.WorkOrder != null &&
                           Building.WorkOrder.Type != WorkOrderType.None &&
                           Building.WorkOrder.Pending < Building.WorkOrder.Maximum &&
                           Building.WorkOrder.GetTotalWorkorderStartups() > 0;
                }
            }

            private int InteractionAttempts = 0;
            public override async Task<bool> Movement()
            {
                if (Building.CheckedWorkOrderStartUp)
                    return false;

                TreeRoot.StatusText = String.Format("Behavior {0} [{1}] Movement", Type.ToString(), Building.Type);
                if (await base.Movement())
                    return true;

                TreeRoot.StatusText = String.Format("Behavior {0} [{1}] Movement2", Type.ToString(), Building.Type);
                //Move to the interaction object (within 6.7f)
                if (_movement == null)
                {
                    if (WorkOrderObject == null)
                    {
                        //Error Cannot find object!
                        Building.CheckedWorkOrderPickUp = true;
                        return false;
                    }
                    _movement = new Movement(WorkOrderObject.Location, 6.7f);
                }

                if (await _movement.MoveTo())
                    return true;

                if (WorkOrderObject == null)
                {
                    //Error Cannot find object!
                    Building.CheckedWorkOrderStartUp = true;
                    return false;
                }

                if (WorkOrderObject.CentreDistance < 6.9 && WorkOrderObject.WithinInteractRange)
                {
                    if (InteractionAttempts > 3)
                    {
                        GarrisonBase.Log("Interaction Attempts for {0} has exceeded 3! Preforming movement..",
                            WorkOrderObject.Name);
                        _movement = new Movement(Building.SafeMovementPoint, 6.7f);
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

                if (WorkOrderObject.InLineOfSight && WorkOrderObject.WithinInteractRange)
                {
                    WoWMovement.ClickToMove(WorkOrderObject.Guid, WoWMovement.ClickToMoveType.NpcInteract);
                    return true;
                }

                Navigator.MoveTo(WorkOrderObject.Location);
                return !WorkOrderObject.WithinInteractRange;
            }

            private Movement _movement;
            private readonly Movement _finalmovement;

            public override async Task<bool> Interaction()
            {
                if (Building.CheckedWorkOrderStartUp)
                    return false;

                TreeRoot.StatusText = String.Format("Behavior {0} [{1}] Interaction", Type.ToString(), Building.Type);
                if (LuaEvents.ShipmentOrderFrameOpen)
                {
                    if (!LuaCommands.ClickStartOrderButtonEnabled())
                    {
                        Building.CheckedWorkOrderStartUp = true;
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
        }
    }
}
