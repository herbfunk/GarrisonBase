using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bots.Quest;
using Buddy.Coroutines;
using Herbfunk.GarrisonBase.Character;
using Herbfunk.GarrisonBase.Helpers;
using Styx;
using Styx.CommonBot;
using Styx.CommonBot.Coroutines;
using Styx.CommonBot.Frames;
using Styx.WoWInternals.WoWObjects;

namespace Herbfunk.GarrisonBase.Coroutines.Behaviors
{
    public class BehaviorUseFlightPath : Behavior
    {
        public override BehaviorType Type { get { return BehaviorType.Taxi; } }

        private readonly List<string> _nodenames = new List<string>();
        /// <summary>
        /// Attempts to take flight path using names to match valid nodes.
        /// </summary>
        /// <param name="nodeName">First Choice, does not need to be exact string</param>
        /// <param name="alternatives">Alternative choices</param>
        public BehaviorUseFlightPath(string nodeName, string[] alternatives = null)
        {
            _nodenames.Add(nodeName.ToLower());
            if (alternatives == null) return;
            foreach (string s in alternatives)
            {
                _nodenames.Add(s.ToLower());
            }
        }

        private readonly WoWPoint _destination = WoWPoint.Empty;
        /// <summary>
        /// Attempts to take flight path nearest to the location given.
        /// </summary>
        /// <param name="destination"></param>
        public BehaviorUseFlightPath(WoWPoint destination)
        {
            _destination = destination;
            Criteria += () =>  TaxiFlightHelper.ShouldTakeFlightPath(destination);
        }

        public override void Initalize()
        {
            Common.CloseOpenFrames();
            Initalized = true;
        }



        public override async Task<bool> BehaviorRoutine()
        {
            if (await base.BehaviorRoutine()) return true;
            if (IsDone) return false;

            if (TaxiFlightHelper.TaxiNodes.Count > 0 && _selectedTaxiNode == null && await CheckFlightNodes()) return true;

            if (await VerifyFlightNpc()) return true;

            
            if (await UseFlightPath()) return true;

            return false;
        }

        private TaxiFlightHelper.TaxiNodeInfo _selectedTaxiNode = null;
        private async Task<bool> CheckFlightNodes()
        {
            //Validate that we have a node we can use..

            if (_destination != WoWPoint.Empty)
            {
                var validInfoNodes = TaxiFlightHelper.TaxiNodes.Where(n => n.Known && n.Info != null).ToList();
                if (validInfoNodes.Count > 0)
                {
                    var orderedNodes = validInfoNodes.OrderBy(n => n.Info.Location.Distance(_destination)).ToList();
                    _selectedTaxiNode = orderedNodes[0];

                    GarrisonBase.Debug("UseFlightPath using closest node {0}!", _selectedTaxiNode.Name);
                    _nodenames.Add(_selectedTaxiNode.Name);
                }
                else
                {
                    GarrisonBase.Debug("UseFlightPath could not find a valid flight point using location!");
                    IsDone = true;
                    return true;
                }
            }
            else
            {
                var taxiHelperNodes = new List<TaxiFlightHelper.TaxiNodeInfo>();
                foreach (var alt in _nodenames)
                {
                    taxiHelperNodes = TaxiFlightHelper.TaxiNodes.Where(n => n.Known && n.Name.Contains(alt)).ToList();
                    if (taxiHelperNodes.Count > 0)
                        break;
                }

                if (taxiHelperNodes.Count == 0)
                {
                    GarrisonBase.Err("UseFlightPath could not find a valid flight node!");
                    IsDone = true;
                    return true;
                }

                _selectedTaxiNode = taxiHelperNodes[0];
            }

            //Garrison FP!
            if (_selectedTaxiNode.Name.Contains("frostwall garrison") ||
                _selectedTaxiNode.Name.Contains("lunarfall"))
            {
                Common.PreChecks.IgnoreHearthing = false;
            }


            return false;
        }

        private Movement _taxiMovement;
        private WoWUnit _taxiNpc;
        private TaxiFlightHelper.FlightPathInfo _flightPathInfo;
        private async Task<bool> VerifyFlightNpc()
        {
            if (TaxiFlightHelper.IsOpen)
            {
                await CommonCoroutines.SleepForLagDuration();
                //TaxiFrame.Instance.Close();
                return TaxiFlightHelper.TaxiNodes.Count == 0;
            }

            if (GossipHelper.IsOpen)
            {
                if (GossipHelper.GossipOptions.All(o => o.Type != GossipEntry.GossipEntryType.Taxi))
                {
                    //Could not find Taxi Option!
                    GarrisonBase.Debug("UseFlightPath gossip frame contains no taxi options!");
                    Common.CloseOpenFrames();
                    return true;
                }
                var gossipOptionTaxi = GossipHelper.GossipOptions.FirstOrDefault(o => o.Type == GossipEntry.GossipEntryType.Taxi);

                QuestManager.GossipFrame.SelectGossipOption(gossipOptionTaxi.Index);
                await CommonCoroutines.SleepForRandomUiInteractionTime();
                return true;
            }

        
            if (_taxiNpc != null)
            {//We got a valid object.. so lets move into interact range!
                if (_taxiNpc.WithinInteractRange)
                {
                    if (StyxWoW.Me.IsMoving) await CommonCoroutines.StopMoving();
                    await CommonCoroutines.SleepForRandomUiInteractionTime();
                    _taxiNpc.Interact();
                    await CommonCoroutines.SleepForRandomUiInteractionTime();
                    await CommonCoroutines.SleepForLagDuration();
                    return true;
                }

                if (_taxiMovement == null || _taxiMovement.CurrentMovementQueue.Count == 0)
                    _taxiMovement = new Movement(_taxiNpc.Location, 5f - 0.25f);

                await _taxiMovement.MoveTo();
                return true;
            }


            if (_taxiMovement != null && _taxiMovement.CurrentMovementQueue.Count > 0)
            {
                await _taxiMovement.MoveTo();
                return true;
            }

            if (FlightPaths.NearestFlightMerchant == null)
            {
                //Attempt to get location of nearest FP..
                if (_flightPathInfo == null)
                {
                    _flightPathInfo = TaxiFlightHelper.NearestFlightPath;
                    if (_flightPathInfo == null)
                    {//Failed!
                        GarrisonBase.Err("UseFlightPath could not find valid flight path to move to!");
                        IsDone = true;
                        return true;
                    }
                }


                GarrisonBase.Debug("Could not find Nearest Flight Merchant, but using nearest flight path location! {0} at {1}",_flightPathInfo.Name, _flightPathInfo.Location);
                _taxiMovement = new Movement(_flightPathInfo.Location, 20f);
                return true;
            }

            //Found valid npc object..
            _taxiNpc = FlightPaths.NearestFlightMerchant;
            _taxiMovement = new Movement(_taxiNpc.Location, 5f - 0.25f);


            return true;
        }


        private int _nonVisiblityCount = 0;
        private async Task<bool> UseFlightPath()
        {
            if (TaxiFlightHelper.IsOpen)
            {
                await CommonCoroutines.SleepForLagDuration();

                if (!TaxiFrame.Instance.IsVisible)
                {
                    //Error.. LuaEvents says open but frame instance does not!
                    GarrisonBase.Debug("UseFlightPath TaxiFrame Is not Visible");
                    _nonVisiblityCount++;

                    if (_nonVisiblityCount > 3)
                    {
                        //Attempted 3 times.. giving up!
                        GarrisonBase.Debug("UseFlightPath TaxiFrame Is not Visible 3x -- failed!");
                        return false;
                    }

                    return true;
                }

                _nonVisiblityCount = 0;

                var taxiFrameNodes = TaxiFrame.Instance.Nodes.Where(n => n.Reachable && n.Name.ToLower().Contains(_selectedTaxiNode.Name)).ToList();

                if (taxiFrameNodes.Count == 0)
                {
                    //Error could not find matching node!
                    GarrisonBase.Debug("UseFlightPath could not find any matching nodes!");
                    return false;
                }

                GarrisonBase.Debug("UseFlightPath taking node {0}", taxiFrameNodes[0].Name);
                await CommonCoroutines.WaitForLuaEvent("TAXIMAP_CLOSED", 3500, null, taxiFrameNodes[0].TakeNode);
                await CommonCoroutines.SleepForRandomUiInteractionTime();

                if (Player.LastErrorMessage == "You are busy and can't use the taxi service now.")
                {
                    await Coroutine.Yield();
                    return true;
                }

                IsDone = true;
                return false;
            }

            if (GossipHelper.IsOpen)
            {
                if (GossipHelper.GossipOptions.All(o => o.Type != GossipEntry.GossipEntryType.Taxi))
                {
                    //Could not find Taxi Option!
                    GarrisonBase.Debug("UseFlightPath gossip frame contains no taxi options!");
                    return false;
                }
                var gossipOptionTaxi = GossipHelper.GossipOptions.FirstOrDefault(o => o.Type == GossipEntry.GossipEntryType.Taxi);

                QuestManager.GossipFrame.SelectGossipOption(gossipOptionTaxi.Index);
                await CommonCoroutines.SleepForRandomUiInteractionTime();
                return true;
            }

            return false;
        }
    }
}
