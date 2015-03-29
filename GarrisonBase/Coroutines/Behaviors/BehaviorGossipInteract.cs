using System;
using System.Linq;
using System.Threading.Tasks;
using Herbfunk.GarrisonBase.Cache;
using Herbfunk.GarrisonBase.Cache.Objects;
using Herbfunk.GarrisonBase.Quest;
using Herbfunk.GarrisonBase.Quest.Objects;
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
        public override async Task<bool> BehaviorRoutine()
        {
            if (await base.BehaviorRoutine()) return true;
            if (IsDone) return false;

            if (LuaEvents.GossipFrameOpen)
            {
                if (QuestManager.GossipFrameInfo.GossipOptionEntries.Count == 0)
                {
                    GarrisonBase.Err("Could not find any valid gossip entries!");
                    GossipFrame.Instance.Close();
                    IsDone = true;
                    return false;
                }

                CGossipEntry gossipEntry=null;
                if (_gossipIndex > -1)
                {
                    gossipEntry = QuestManager.GossipFrameInfo.GossipOptionEntries.FirstOrDefault(g => g.Index == _gossipIndex);
                }
                else if (!string.IsNullOrEmpty(_gossipText))
                {
                    gossipEntry = QuestManager.GossipFrameInfo.GossipOptionEntries.FirstOrDefault(g => g.Text.ToLower().Contains(_gossipText));
                }
                
                if (gossipEntry == null)
                {
                    GarrisonBase.Err("Could not find any valid gossip entries index [{0}] text [{1}]", _gossipIndex, _gossipText);
                    GossipFrame.Instance.Close();
                    IsDone = true;
                    return false;
                }

                GarrisonBase.Debug("Selecting Gossip Option {0}", gossipEntry.Index);
                GossipFrame.Instance.SelectGossipOption(gossipEntry.Index);
                await CommonCoroutines.SleepForRandomUiInteractionTime();

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
                if (LuaEvents.GossipFrameOpen)
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
