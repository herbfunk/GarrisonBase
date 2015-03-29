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

namespace Herbfunk.GarrisonBase.Coroutines.Behaviors
{
    public class BehaviorWorkOrderStartUp : Behavior
    {
        public BehaviorWorkOrderStartUp(Building building)
            : base(new[] { building.SafeMovementPoint, building.EntranceMovementPoint }, building.WorkOrderNPCEntryId)
        {
            Building = building;

        }
        public override Func<bool> Criteria
        {
            get
            {
                return () => BaseSettings.CurrentSettings.BehaviorWorkOrderStartup &&
                             !Building.CanActivate && !Building.IsBuilding &&
                             BaseSettings.CurrentSettings.WorkOrderTypes.HasFlag(Building.WorkOrderType) &&
                             !Building.CheckedWorkOrderStartUp &&
                             Building.WorkOrder != null &&
                             Building.WorkOrder.Type != WorkOrderType.None &&
                             Building.WorkOrder.Pending < Building.WorkOrder.Maximum &&
                             ((Building.Type != BuildingType.TradingPost && Building.WorkOrder.TotalWorkorderStartups() > 0) ||
                             (Building.Type == BuildingType.TradingPost && !BaseSettings.CurrentSettings.TradePostReagents.Equals(WorkOrder.TradePostReagentTypes.None)));
            }
        }

        public override void Initalize()
        {
            _checkedReagent = false;
            _interactionAttempts = 0;
            _movement = null;
            if (Building.SpecialMovementPoints != null)
                _specialMovement = new Movement(Building.SpecialMovementPoints.ToArray(), 2f);

            base.Initalize();
        }

        public override BehaviorType Type { get { return BehaviorType.WorkOrderStartUp; } }
        public Building Building { get; set; }

        private Movement _movement, _specialMovement;
        private int _interactionAttempts = 0;
        private bool _checkedReagent = false;

        public C_WoWUnit WorkOrderObject
        {
            get
            {
                return ObjectCacheManager.GetWoWUnits(Building.WorkOrderNPCEntryId).FirstOrDefault();
            }
        }





        
        public override async Task<bool> BehaviorRoutine()
        {
            if (await base.BehaviorRoutine()) return true;
            if (IsDone) return false;


            if (Building.Type == BuildingType.TradingPost && Building.WorkOrderNPCEntryId != -1)
            {
                if (!_checkedReagent)
                {
                    WorkOrder.TradePostReagentTypes workOrderReagent;
                    workOrderReagent = WorkOrder.GetTradePostNPCReagent(Building.WorkOrderNPCEntryId);
                    if (!BaseSettings.CurrentSettings.TradePostReagents.HasFlag(workOrderReagent))
                    {
                        //Does not have the current reagent set!
                        return false;
                    }

                    var currency = WorkOrder.GetTradePostItemAndQuanityRequired(workOrderReagent);
                    var totalCount = WorkOrder.GetTotalWorkorderStartups(currency);
                    if (totalCount <= 0)
                    {
                        //Not enough reagents.
                        return false;
                    }

                    _checkedReagent = true;
                }
            }

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
                    var nearestPoint = Coroutines.Movement.FindNearestPoint(WorkOrderObject.Location, _specialMovement.CurrentMovementQueue.ToList());
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
                    

                if (_movement == null)
                    _movement = new Movement(WorkOrderObject.Location, 4f);

                //since we are navigating inside building.. we must continue to use CTM
                if (await _movement.ClickToMove(false))
                    return true;
            }

            if (_movement == null)
                _movement = new Movement(WorkOrderObject.Location, 4f);

            if (await _movement.ClickToMove(false)) 
                return true;

            return !WorkOrderObject.WithinInteractRange;
        }

        

        public override async Task<bool> Interaction()
        {
            if (Building.CheckedWorkOrderStartUp)
                return false;

            if (Building.Type != BuildingType.TradingPost && Building.WorkOrder.TotalWorkorderStartups() == 0)
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
}