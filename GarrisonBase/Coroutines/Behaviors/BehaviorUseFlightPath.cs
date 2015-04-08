using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bots.Quest;
using Herbfunk.GarrisonBase.Helpers;
using Styx;
using Styx.CommonBot;
using Styx.CommonBot.Coroutines;
using Styx.CommonBot.Frames;

namespace Herbfunk.GarrisonBase.Coroutines.Behaviors
{
    public class BehaviorUseFlightPath : Behavior
    {
        public override BehaviorType Type { get { return BehaviorType.Taxi; } }

        private readonly List<string> _nodenames = new List<string>();
        //private NpcHelper.TaxiNpc TaxiInfo;
        public BehaviorUseFlightPath(string nodeName, string[] alternatives = null) :
            base()
        {
            Common.ShouldUpdateTaxiNodes = true;
            _nodenames.Add(nodeName.ToLower());
            if (alternatives == null) return;
            foreach (string s in alternatives)
            {
                _nodenames.Add(s.ToLower());
            }
        }

        public override void Initalize()
        {
            //if (!NpcHelper.TaxiNpcs.ContainsKey(Convert.ToInt32(Player.MapId)))
            //{
            //    RunCondition += () => false;
            //}
            //else
            //{
            //    var taxiNpcs = NpcHelper.TaxiNpcs[Convert.ToInt32(Player.MapId)].ToList();
            //    foreach (var npc in taxiNpcs)
            //    {
            //        if (Player.IsAlliance && npc.Faction.HasFlag(NpcHelper.NpcFaction.Alliance))
            //        {
            //            TaxiInfo = npc;
            //            break;
            //        }
            //        if (!Player.IsAlliance && npc.Faction.HasFlag(NpcHelper.NpcFaction.Horde))
            //        {
            //            TaxiInfo = npc;
            //            break;
            //        }
            //    }

            //    if (TaxiInfo == null)
            //    {
            //        GarrisonBase.Debug("Failed to find Taxi Info for map id {0}", Player.MapId);
            //        RunCondition += () => false;
            //        IsDone = true;
            //        return;
            //    }

            //    GarrisonBase.Debug("Using Taxi NPC {0}", TaxiInfo.EntryId);
            //}

            bool foundUsableNode = _nodenames.Any(alt => TaxiFlightHelper.TaxiNodes.Any(n => n.Name.ToLower().Contains(alt)));
            if (!foundUsableNode)
            {
                GarrisonBase.Err("UseFlightPath could not find a valid flight node!");
                IsDone = true;
                return;
            }

            var nearestNpc = FlightPaths.NearestFlightMerchant;
            GarrisonBase.Debug("UseFlightPath nearest npc {0}", nearestNpc.Name);
            MovementPoints.Add(nearestNpc.Location);
            InteractionEntryId = (int)nearestNpc.Entry;

            base.Initalize();
        }


        private Movement _npcMovement;
        public override async Task<bool> BehaviorRoutine()
        {
            if (await base.BehaviorRoutine()) return true;
            if (IsDone) return false;

            if (await StartMovement.MoveTo()) return true;


            if (LuaEvents.GossipFrameOpen)
            {
                if (QuestManager.GossipFrame.GossipOptionEntries.All(o => o.Type != GossipEntry.GossipEntryType.Taxi))
                {
                    //Could not find Taxi Option!
                    return false;
                }
                var gossipOptionTaxi = QuestManager.GossipFrame.GossipOptionEntries.FirstOrDefault(o => o.Type == GossipEntry.GossipEntryType.Taxi);

                QuestManager.GossipFrame.SelectGossipOption(gossipOptionTaxi.Index);
                await CommonCoroutines.SleepForRandomUiInteractionTime();
                return true;
            }

            if (LuaEvents.TaxiMapOpen)
            {
                if (!TaxiFrame.Instance.IsVisible)
                {
                    //Error.. LuaEvents says open but frame instance does not!
                    return false;
                }

                var taxiFrameNodes = new List<TaxiFrame.TaxiFrameNode>();
                foreach (var alt in _nodenames)
                {
                    taxiFrameNodes = TaxiFrame.Instance.Nodes.Where(n => n.Name.ToLower().Contains(alt) && n.Reachable).ToList();
                    if (taxiFrameNodes.Count > 0)
                        break;
                }

                if (taxiFrameNodes.Count == 0)
                {
                    //Error could not find matching node!
                    return false;
                }

                await CommonCoroutines.WaitForLuaEvent("TAXIMAP_CLOSED", 3500, null, taxiFrameNodes[0].TakeNode);
                await CommonCoroutines.SleepForRandomUiInteractionTime();

                IsDone = true;
                return false;
            }

            if (InteractionObject != null)
            {
                if (InteractionObject.WithinInteractRange)
                {
                    if (StyxWoW.Me.IsMoving) await CommonCoroutines.StopMoving();
                    await CommonCoroutines.SleepForLagDuration();
                    InteractionObject.Interact();
                    await CommonCoroutines.SleepForRandomUiInteractionTime();
                    return true;
                }

                if (_npcMovement == null)
                    _npcMovement = new Movement(InteractionObject.Location, InteractionObject.InteractRange - 0.25f);

                await _npcMovement.MoveTo();
                return true;
            }

            return false;
        }
    }
}
