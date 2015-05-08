using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bots.Quest;
using Buddy.Coroutines;
using Herbfunk.GarrisonBase.Cache;
using Herbfunk.GarrisonBase.Cache.Objects;
using Herbfunk.GarrisonBase.Character;
using Herbfunk.GarrisonBase.Garrison.Enums;
using Herbfunk.GarrisonBase.Garrison.Objects;
using Herbfunk.GarrisonBase.Helpers;
using Styx;
using Styx.CommonBot;
using Styx.CommonBot.Coroutines;
using Styx.Pathing;

namespace Herbfunk.GarrisonBase.Coroutines.Behaviors
{
    public class BehaviorWorkOrderStartUp : Behavior
    {
        public BehaviorWorkOrderStartUp(Building building)
            : base(new[] { building.SafeMovementPoint, building.EntranceMovementPoint }, building.WorkOrderNpcEntryId)
        {
            Building = building;
            Criteria += () => BaseSettings.CurrentSettings.BehaviorWorkOrderStartup &&
                             !Building.CanActivate && !Building.IsBuilding &&
                             BaseSettings.CurrentSettings.WorkOrderTypes.HasFlag(Building.WorkOrderType) &&
                             !Building.CheckedWorkOrderStartUp &&
                             Building.WorkOrder != null &&
                             Building.WorkOrder.Type != WorkOrderType.None &&
                             Building.WorkOrder.Pending < Building.WorkOrder.Maximum &&
                             ((Building.Type == BuildingType.Barn) ||
                             (Building.Type != BuildingType.TradingPost && Building.WorkOrder.TotalWorkorderStartups() > 0) ||
                             (Building.Type == BuildingType.TradingPost && !BaseSettings.CurrentSettings.TradePostReagents.Equals(WorkOrder.TradePostReagentTypes.None)));
        }


        public override void Initalize()
        {
            _checkedReagent = false;
            _interactionAttempts = 0;
            _movement = null;
            //if (Building.SpecialMovementPoints != null)
            //    _specialMovement = new Movement(Building.SpecialMovementPoints.ToArray(), 2f, name: "WorkOrderPickupSpecialMovement");

            if (Building.Type == BuildingType.Barn)
            {
                BarnWorkOrderCurrencies = new List<Tuple<CraftingReagents, int>[]>(WorkOrder.BarnWorkOrderItemList);
                //The starting index (Each Level adds a new gossip entry about using the trap)
                BarnWorkOrderGossipStartingIndex = Building.Level;
            }

            base.Initalize();
        }

        public override BehaviorType Type { get { return BehaviorType.WorkOrderStartUp; } }
        public Building Building { get; set; }

        private Movement _movement;
        private int _interactionAttempts = 0;
        private bool _checkedReagent = false;
        private List<Tuple<CraftingReagents, int>[]> BarnWorkOrderCurrencies;
        private Tuple<CraftingReagents, int>[] CurrentBarnCurrceny;
        private int BarnWorkOrderGossipStartingIndex = 1;

        public C_WoWUnit WorkOrderObject
        {
            get
            {
                return ObjectCacheManager.GetWoWUnits(Building.WorkOrderNpcEntryId).FirstOrDefault();
            }
        }

        //This is the initial order of gossip entries if user had all 6 items available
        private List<CraftingReagents> _barnGossipWorkOrderEntries = new List<CraftingReagents>
        {
            CraftingReagents.FurryCagedBeast,
            CraftingReagents.CagedMightyWolf,
            CraftingReagents.LeatheryCagedBeast,
            CraftingReagents.CagedMightyClefthoof,
            CraftingReagents.MeatyCagedBeast,
            CraftingReagents.CagedMightyRiverbeast
        };

        private void _updateBarnGossipWorkOrderEntries()
        {
            var removalList = new List<int>();
            foreach (var entry in _barnGossipWorkOrderEntries)
            {
                if (Player.Inventory.GetCraftingReagentsById((int)entry, !BaseSettings.CurrentSettings.IgnoreBankItems, !BaseSettings.CurrentSettings.IgnoreReagentBankItems).Count == 0)
                    removalList.Add(_barnGossipWorkOrderEntries.IndexOf(entry));
            }

            if (removalList.Count <= 0) return;

            foreach (var i in removalList.OrderByDescending(i => i))
            {
                _barnGossipWorkOrderEntries.RemoveAt(i);
            }
        }

        private int BarnGossipIndex
        {
            get
            {
                return BarnWorkOrderGossipStartingIndex + _barnGossipWorkOrderEntries.IndexOf(CurrentBarnCurrceny[0].Item1);
            }
        }


        public override async Task<bool> BehaviorRoutine()
        {
            if (await base.BehaviorRoutine()) return true;
            if (IsDone) return false;


            if (Building.Type == BuildingType.TradingPost && Building.WorkOrderNpcEntryId != -1)
            {
                if (!_checkedReagent)
                {
                    WorkOrder.TradePostReagentTypes workOrderReagent;
                    workOrderReagent = WorkOrder.GetTradePostNPCReagent(Building.WorkOrderNpcEntryId);
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

            if (Building.Type == BuildingType.Barn && !Building.CheckedWorkOrderStartUp && CurrentBarnCurrceny == null)
            {
                //TODO:: Further Testing!
                //Gossip Dialog
                //0: Gossip 
                //1: Gossip (Barn Level 2+ Only)
                //2: Gossip (Barn Level 3+ Only)
                //3: Fur
                //4: Fur Large
                //5: Leather
                //6: Leather Large
                //7: Meat
                //8: Meat Large

                bool found = false;
                var removalList = new List<int>();
                for (int i = 0; i < BarnWorkOrderCurrencies.Count; i++)
                {
                    Tuple<CraftingReagents, int>[] item = BarnWorkOrderCurrencies[i];
                    switch (item[0].Item1)
                    {
                        case CraftingReagents.CagedMightyWolf:
                        case CraftingReagents.FurryCagedBeast:
                            if (!BaseSettings.CurrentSettings.BarnWorkOrderFur)
                            {
                                removalList.Add(i);
                                continue;
                            }
                            break;

                        case CraftingReagents.CagedMightyClefthoof:
                        case CraftingReagents.LeatheryCagedBeast:
                            if (!BaseSettings.CurrentSettings.BarnWorkOrderLeather)
                            {
                                removalList.Add(i);
                                continue;
                            }
                            break;

                        case CraftingReagents.CagedMightyRiverbeast:
                        case CraftingReagents.MeatyCagedBeast:
                            if (!BaseSettings.CurrentSettings.BarnWorkOrderMeat)
                            {
                                removalList.Add(i);
                                continue;
                            }
                            break;
                    }

                    var totalCount = WorkOrder.GetTotalWorkorderStartups(item);
                    if (totalCount <= 0)
                    {
                        removalList.Add(i);
                        continue;
                    }

                    CurrentBarnCurrceny = item;
                    found = true;
                    break;
                }

                if (!found)
                {
                    Building.CheckedWorkOrderStartUp = true;
                    //if (_specialMovement != null)
                    //{
                    //    _specialMovement.UseDeqeuedPoints(true);
                    //    if (_specialMovement.CurrentMovementQueue.Count == 0)
                    //    {
                    //        return false;
                    //    }

                    //    return true;
                    //}

                    return false;
                }

                
                _updateBarnGossipWorkOrderEntries();
                GarrisonBase.Debug("Staring Work Order for Barn using {0} at gossip index {1}", CurrentBarnCurrceny[0].Item1.ToString(), BarnGossipIndex);

                foreach (var i in removalList.OrderByDescending(i=>i))
                {
                    BarnWorkOrderCurrencies.RemoveAt(i);
                }
            }

            if (await StartMovement.MoveTo())
                return true;

            if (await Movement())
                return true;

            if (await Interaction())
                return true;

            //if (_specialMovement != null && await _specialMovement.ClickToMove())
            //    return true;

            if (await EndMovement.MoveTo())
                return true;

            return false;
        }






        private async Task<bool> Movement()
        {
            if (Building.CheckedWorkOrderStartUp) return false;

            if (WorkOrderObject == null)
            {
                GarrisonBase.Err("Could not find Work Order Npc Id {0}", Building.WorkOrderNpcEntryId);
                //Error Cannot find object!
                Building.CheckedWorkOrderStartUp = true;
                return false;
            }



            if (WorkOrderObject.WithinInteractRange)
            {
                if (StyxWoW.Me.IsMoving) await CommonCoroutines.StopMoving();
                await CommonCoroutines.SleepForLagDuration();

                if (_interactionAttempts > 3)
                {
                    GarrisonBase.Log("Interaction Attempts for {0} has exceeded 3! Preforming movement..",
                        WorkOrderObject.Name);
                    StartMovement.Reset();
                    _interactionAttempts = 0;
                    return true;
                }


                if (LuaUI.WorkOrder.IsVisible())
                {
                    //Workorder frame is displayed!
                    _interactionAttempts = 0;
                    return false;
                }

                if (Building.Type == BuildingType.Barn && GossipHelper.IsOpen)
                {
                    //var entries = GossipHelper.GossipOptions.Where(
                    //                entry => entry.Text.ToLower().Contains(BarnWorkOrderGossipString)).ToList();
                    var entries = GossipHelper.GossipOptions.Where(
                                    entry => entry.Index==BarnGossipIndex).ToList();
                    if (entries.Count > 0)
                    {
                        int index = entries[0].Index;
                        QuestManager.GossipFrame.SelectGossipOption(index);
                        await CommonCoroutines.SleepForRandomUiInteractionTime();
                        return true;
                    }
                    else
                    {
                        GarrisonBase.Err("Could not find gossip index for barn currency {0}",
                            CurrentBarnCurrceny[0].Item1.ToString());
                    }
                }

                _interactionAttempts++;
                WorkOrderObject.Interact();
                await CommonCoroutines.SleepForRandomUiInteractionTime();
                return true;
            }


            //Setup the NPC movement!
            if (_movement == null || _movement.CurrentMovementQueue.Count==0)
                _movement = new Movement(WorkOrderObject, WorkOrderObject.InteractRange-0.50f, WorkOrderObject.Name);

            await _movement.MoveTo();
            return true;
        }



        private async Task<bool> Interaction()
        {
            if (Building.CheckedWorkOrderStartUp)
                return false;

            if ((Building.Type != BuildingType.TradingPost && Building.Type != BuildingType.Barn) &&
                Building.WorkOrder.TotalWorkorderStartups() == 0)
            {
                GarrisonBase.Debug("Total Work Order Startup Count is Zero!");
                Building.CheckedWorkOrderStartUp = true;
                return false;
            }

            TreeRoot.StatusText = String.Format("Behavior {0} [{1}] Interaction", Type.ToString(), Building.Type);
            if (LuaEvents.ShipmentOrderFrameOpen)
            {

                if (BaseSettings.CurrentSettings.DEBUG_FAKESTARTWORKORDER || !LuaUI.WorkOrder.StartWorkOrder.IsEnabled())
                {
                    if (Building.Type != BuildingType.Barn)
                    {
                        Building.CheckedWorkOrderStartUp = true;
                    }
                    else
                    {
                        CurrentBarnCurrceny = null;
                        BarnWorkOrderCurrencies.RemoveAt(0);
                    }

                    //if (_specialMovement != null) _specialMovement.UseDeqeuedPoints(true);
                    GarrisonBase.Log("Order Button Disabled!");
                    Building.WorkOrder.Refresh();
                    LuaUI.WorkOrder.Close.Click();
                    await CommonCoroutines.SleepForRandomUiInteractionTime();
                    await Coroutine.Yield();
                    return Building.Type == BuildingType.Barn;
                }

                await CommonCoroutines.SleepForRandomUiInteractionTime();

                if (Building.Type == BuildingType.WarMillDwarvenBunker)
                    LuaUI.WorkOrder.StartWorkOrder.Click();
                else
                    LuaUI.WorkOrder.CreateAllWorkOrder.Click();

                await Coroutine.Yield();
            }

            return true;
        }

        public override string GetStatusText
        {
            get { return base.GetStatusText + Building.Type.ToString(); }
        }
    }
}