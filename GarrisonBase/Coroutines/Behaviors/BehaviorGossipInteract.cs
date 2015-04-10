using System;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Herbfunk.GarrisonBase.Cache;
using Herbfunk.GarrisonBase.Cache.Objects;
using Herbfunk.GarrisonBase.Helpers;
using Styx;
using Styx.CommonBot.Coroutines;
using Styx.CommonBot.Frames;

namespace Herbfunk.GarrisonBase.Coroutines.Behaviors
{
    public class BehaviorGossipInteract : Behavior
    {
        private readonly int _npcId;
        private readonly int _gossipIndex=-1;
        private readonly string _gossipText = string.Empty;
        private readonly bool _oneTime;

        public BehaviorGossipInteract(int npcId, int gossipIndex, bool oneTime = false, Func<bool> runCondition = null)
            : base(WoWPoint.Zero, npcId)
        {
            _npcId = npcId;
            _gossipIndex = gossipIndex;
            _oneTime = oneTime;
            if (runCondition != null) RunCondition += runCondition;
            ObjectCacheManager.QuestNpcIds.Add(Convert.ToUInt32(_npcId));
        }

        public BehaviorGossipInteract(int npcId, string gossipText, bool oneTime = false, Func<bool> runCondition = null) : base(WoWPoint.Zero, npcId)
        {
            _npcId = npcId;
            _gossipText = gossipText.ToLower();
            _oneTime = oneTime;
            if (runCondition != null) RunCondition += runCondition;
            ObjectCacheManager.QuestNpcIds.Add(Convert.ToUInt32(_npcId));
        }

        public override void Dispose()
        {
            base.Dispose();
            ObjectCacheManager.QuestNpcIds.Remove(Convert.ToUInt32(_npcId));
        }

        private Movement _npcMovement;
        private C_WoWUnit NpcObject
        {
            get { return ObjectCacheManager.GetWoWUnits(_npcId).FirstOrDefault(); }
        }

        private int _failedToFindGossipEntries = 0;
        public override async Task<bool> BehaviorRoutine()
        {
            if (await base.BehaviorRoutine()) return true;
            if (IsDone) return false;

            if (GossipHelper.IsOpen)
            {
                if (GossipHelper.GossipOptions.Count==0)
                {
                    _failedToFindGossipEntries++;
                    if (_failedToFindGossipEntries > 3)
                    {
                        GarrisonBase.Err("Could not find any valid gossip entries!");
                        GossipFrame.Instance.Close();
                        IsDone = true;
                        return false;
                    }

                    await Coroutine.Yield();
                    return true;
                }

                GossipHelper.GossipOptionEntry gossipEntry=null;
                if (_gossipIndex > -1)
                {
                    gossipEntry = GossipHelper.GossipOptions[_gossipIndex];
                }
                else if (!string.IsNullOrEmpty(_gossipText))
                {
                    gossipEntry = GossipHelper.GossipOptions.FirstOrDefault(g => g.Text.ToLower().Contains(_gossipText));
                }
                
                if (gossipEntry == null)
                {
                    GarrisonBase.Err("Could not find any valid gossip entries index [{0}] text [{1}]", _gossipIndex, _gossipText);
                    GossipFrame.Instance.Close();
                    IsDone = true;
                    return false;
                }

                GarrisonBase.Debug("Selecting Gossip Option {0}", gossipEntry.Index);
                var success=await CommonCoroutines.WaitForLuaEvent("GOSSIP_SHOW", 2500, null,
                                    () => GossipFrame.Instance.SelectGossipOption(gossipEntry.Index));


                GarrisonBase.Debug("Gossip Selection success = {0}", success);
                
                await CommonCoroutines.SleepForRandomUiInteractionTime();

                if (!success)
                {
                    return false;
                }

                if (_oneTime) IsDone = true;
                return !_oneTime;
            }

            if (NpcObject == null || !NpcObject.CanInteract)
            {
                GarrisonBase.Err("Could not find npc {0} to interact with!", _npcId);
                IsDone = true;
                return false;
            }
            if (_npcMovement==null)
                _npcMovement = new Movement(NpcObject.Location, NpcObject.InteractRange - 0.25f);

            if (await _npcMovement.MoveTo()) 
                return true;

            if (NpcObject.WithinInteractRange)
            {
                if (StyxWoW.Me.IsMoving) await CommonCoroutines.StopMoving();
                await CommonCoroutines.SleepForLagDuration();
                if (GossipHelper.IsOpen)
                {
                    //frame is displayed!
                    return true;
                }

                NpcObject.Interact();
                await CommonCoroutines.SleepForRandomUiInteractionTime();
                return true;
            }

            return true;
        }
    }
}
