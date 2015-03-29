using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Herbfunk.GarrisonBase.Cache;
using Styx;

namespace Herbfunk.GarrisonBase.Coroutines.Behaviors
{
    public class BehaviorHotspotRunning : Behavior
    {
        [Flags]
        public enum HotSpotType
        {
            None = 0,
            Looting = 1,
            Killing = 2,
            Both = Looting | Killing,
        }
        public override BehaviorType Type { get { return BehaviorType.HotspotRunning; } }

        public BehaviorHotspotRunning(uint questId, WoWPoint[] hotspots, HotSpotType type, Func<bool> runCondition)
            : this(questId, hotspots, new uint[0], type, runCondition)
        {
        }

        public BehaviorHotspotRunning(uint questId, WoWPoint[] hotspots, uint[] ids, HotSpotType type, Func<bool> runCondition)
        {
            _objectIds.AddRange(ids);
            _hotSpots.AddRange(hotspots);
            _hotspotType = type;
            RunCondition += runCondition;

        }

        private readonly List<uint> _objectIds = new List<uint>();
        private readonly List<WoWPoint> _hotSpots = new List<WoWPoint>();
        private readonly HotSpotType _hotspotType;

        public override void Initalize()
        {
            _hotSpotMovement = new Movement(_hotSpots.ToArray());

            ObjectCacheManager.ShouldLoot = _hotspotType.HasFlag(HotSpotType.Looting);
            ObjectCacheManager.ShouldKill = _hotspotType.HasFlag(HotSpotType.Killing);

            foreach (var i in _objectIds)
            {
                if (_hotspotType.HasFlag(HotSpotType.Looting)) ObjectCacheManager.LootIds.Add(i);
                if (_hotspotType.HasFlag(HotSpotType.Killing)) ObjectCacheManager.CombatIds.Add(i);
            }

           
            base.Initalize();
        }

        public override void Dispose()
        {
            if (_hotspotType.HasFlag(HotSpotType.Looting)) ObjectCacheManager.ShouldLoot = false;
            if (_hotspotType.HasFlag(HotSpotType.Killing)) ObjectCacheManager.ShouldKill = false;

            foreach (var i in _objectIds)
            {
                if (_hotspotType.HasFlag(HotSpotType.Looting)) ObjectCacheManager.LootIds.Remove(i);
                if (_hotspotType.HasFlag(HotSpotType.Killing)) ObjectCacheManager.CombatIds.Remove(i);
            }

            base.Dispose();
        }

        private Movement _hotSpotMovement;

        public override async Task<bool> BehaviorRoutine()
        {
            if (await base.BehaviorRoutine()) return true;
            if (IsDone) return false;

            //if (_hotspotType.HasFlag(HotSpotType.Looting)) ObjectCacheManager.UpdateLootableTarget();
            //if (_hotspotType.HasFlag(HotSpotType.Killing)) ObjectCacheManager.UpdateCombatTarget();

            if (_hotSpotMovement.CurrentMovementQueue.Count == 0)
                _hotSpotMovement.UseDeqeuedPoints(true);

            if (((_hotspotType.HasFlag(HotSpotType.Looting) && ObjectCacheManager.LootableObject == null) ||
                (_hotspotType.HasFlag(HotSpotType.Killing) && ObjectCacheManager.CombatObject == null)))
                await _hotSpotMovement.MoveTo();

            return true;
        }
    }
}