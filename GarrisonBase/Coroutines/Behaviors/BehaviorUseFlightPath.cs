using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bots.Quest;
using Herbfunk.GarrisonBase.Garrison;
using Styx;
using Styx.CommonBot.Coroutines;
using Styx.CommonBot.Frames;

namespace Herbfunk.GarrisonBase.Coroutines.Behaviors
{
    public class BehaviorUseFlightPath : Behavior
    {
        public override BehaviorType Type { get { return BehaviorType.Taxi; } }

        private readonly string _nodename;
        private readonly List<string> _nodenameAlternatives=new List<string>();
        public BehaviorUseFlightPath(string nodeName, string[] alternatives=null) : base(MovementCache.FlightPathNpcLocation, GarrisonManager.FlightPathNpcId)
        {
            _nodename = nodeName.ToLower();
            if (alternatives != null)
            {
                foreach (string s in alternatives)
                {
                    _nodenameAlternatives.Add(s.ToLower());
                }
            }
        }

        public override Func<bool> Criteria
        {
            get
            {
                return () => (StyxWoW.Me.CurrentMap.IsGarrison);
            }
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

                var possibleNodes = TaxiFrame.Instance.Nodes.Where(n => n.Name.ToLower().Contains(_nodename) && n.Reachable).ToList();
                if (possibleNodes.Count == 0)
                {
                    foreach (var alt in _nodenameAlternatives)
                    {
                        possibleNodes = TaxiFrame.Instance.Nodes.Where(n => n.Name.ToLower().Contains(alt) && n.Reachable).ToList();
                        if (possibleNodes.Count > 0)
                            break;
                    }
                    if (possibleNodes.Count == 0)
                    {
                        //Error could not find matching node!
                        return false;
                    }
                }

                await CommonCoroutines.WaitForLuaEvent("TAXIMAP_CLOSED", 3500, null, possibleNodes[0].TakeNode);
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
