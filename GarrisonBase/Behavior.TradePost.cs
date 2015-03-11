using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Herbfunk.GarrisonBase.Cache;
using Herbfunk.GarrisonBase.Cache.Objects;
using Herbfunk.GarrisonBase.Garrison;
using Herbfunk.GarrisonBase.Garrison.Enums;
using Herbfunk.GarrisonBase.Garrison.Objects;
using Styx;
using Styx.CommonBot;
using Styx.CommonBot.Coroutines;

namespace Herbfunk.GarrisonBase
{
    public partial class Behaviors
    {
        public class BehaviorTradePost : Behavior
        {
            public override BehaviorType Type { get { return BehaviorType.WorkOrderStartUp; } }

            public BehaviorTradePost(Building building)
                : base(building.SafeMovementPoint)
            {
                Building = building;
            }
            public Building Building { get; set; }

            public override void Initalize()
            {
                if (Building.Level == 1)
                {
                    if (Building.PlotId == 22)
                        _specialMovement=new Movement(MovementCache.Plot22_TradePost_Level1.ToArray(), 2f);
                    else
                        _specialMovement = new Movement(MovementCache.Plot25_TradePost_Level1.ToArray(), 2f);
                }
                else if (Building.Level == 2)
                {
                    if (Building.PlotId == 22)
                        _specialMovement = new Movement(MovementCache.Plot22_TradePost_Level2.ToArray(), 2f);
                    else
                        _specialMovement = new Movement(MovementCache.Plot25_TradePost_Level2.ToArray(), 2f);
                }
                else
                {
                    if (Building.PlotId == 22)
                        _specialMovement = new Movement(MovementCache.Plot22_TradePost_Level3.ToArray(), 2f);
                    else
                        _specialMovement = new Movement(MovementCache.Plot25_TradePost_Level3.ToArray(), 2f);
                }

                MovementPoints.Add(Building.EntranceMovementPoint);
                base.Initalize();
            }

            public override Func<bool> Criteria
            {
                get
                {
                    return () => BaseSettings.CurrentSettings.BehaviorWorkOrderStartup &&
                                 BaseSettings.CurrentSettings.WorkOrderTypes.HasFlag(Building.WorkOrderType) &&
                                 !BaseSettings.CurrentSettings.TradePostReagents.Equals(
                                     WorkOrder.TradePostReagentTypes.None) &&
                                 !Building.CheckedWorkOrderStartUp &&
                                 Building.WorkOrder.Pending < Building.WorkOrder.Maximum;
                }
            }
            public C_WoWUnit TradePostNPC
            {
                get { return ObjectCacheManager.GetWoWUnits(Building.WorkOrderNPCEntryId).FirstOrDefault(); }
            }
            private Movement _npcMovement;
            private Movement _specialMovement;
            private bool _checkedReagent = false;
            public override async Task<bool> BehaviorRoutine()
            {
                if (IsDone) return false;

                if (await base.BehaviorRoutine()) return true;

                WorkOrder.TradePostReagentTypes workOrderReagent;
                //Now determine the current trade post NPC..
                int workorderID = Building.WorkOrderNPCEntryId;

                if (workorderID == -1)
                {
                    await StartMovement.MoveTo();
                    return true;
                }

                if (!_checkedReagent)
                {
                    workOrderReagent = WorkOrder.GetTradePostNPCReagent(workorderID);
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

                if (await StartMovement.MoveTo()) return true;

                if (_specialMovement!=null && await _specialMovement.ClickToMove()) return true;

                if (await Interaction()) return true;

                if (await EndMovement.MoveTo()) return true;

                return false;
            }

            public override async Task<bool> Interaction()
            {
                if (Building.CheckedWorkOrderStartUp)
                    return false;

                if (TradePostNPC == null)
                {
                    //Could not find NPC!
                    IsDone = true;
                    return true;
                }

                if (!LuaEvents.ShipmentOrderFrameOpen)
                {
                    if (TradePostNPC.WithinInteractRange)
                    {
                        if (StyxWoW.Me.IsMoving) await CommonCoroutines.StopMoving();
                        await CommonCoroutines.SleepForLagDuration();
                        bool success = await CommonCoroutines.WaitForLuaEvent("SHIPMENT_CRAFTER_OPENED", 2500, null, TradePostNPC.Interact);
                    }
                    else
                    {
                        if (_npcMovement == null || _npcMovement.CurrentMovementQueue.Count == 0)
                            _npcMovement = new Movement(TradePostNPC.Location, TradePostNPC.InteractRange);
                        await _npcMovement.MoveTo();
                    }

                    return true;
                }
                
                if (BaseSettings.CurrentSettings.DEBUG_FAKESTARTWORKORDER || !LuaCommands.ClickStartOrderButtonEnabled())
                {
                    Building.CheckedWorkOrderStartUp = true;
                    if (_specialMovement!=null) _specialMovement.UseDeqeuedPoints(true);
                    return true;
                }

                LuaCommands.ClickStartAllOrderButton();
                await CommonCoroutines.SleepForRandomUiInteractionTime();
                await Coroutine.Yield();
                return true;
            }

            public override string GetStatusText
            {
                get { return base.GetStatusText + Building.Type.ToString(); }
            }

        }
    }
}
