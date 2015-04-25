using System.Collections.Generic;
using System.Threading.Tasks;
using Herbfunk.GarrisonBase.Cache;
using Herbfunk.GarrisonBase.Garrison;
using Herbfunk.GarrisonBase.Garrison.Enums;
using Herbfunk.GarrisonBase.TargetHandling;
using Styx;

namespace Herbfunk.GarrisonBase.Coroutines.Behaviors
{
    public class BehaviorHerb : Behavior
    {
        public override BehaviorType Type { get { return BehaviorType.Herbing; } }

        public BehaviorHerb() : base(MovementCache.GardenPlot63SafePoint)
        {
            Criteria += () => (!GarrisonManager.Buildings[BuildingType.HerbGarden].IsBuilding &&
                     !GarrisonManager.Buildings[BuildingType.HerbGarden].CanActivate &&
                     GarrisonManager.Buildings[BuildingType.HerbGarden].FirstQuestCompleted &&
                     LuaCommands.CheckForDailyReset(BaseSettings.CurrentSettings.LastCheckedHerb) &&
                     BaseSettings.CurrentSettings.BehaviorHerbGather);
        }

        public override void Initalize()
        {
            base.Initalize();

            foreach (var mobId in CacheStaticLookUp.HerbQuestMobIDs)
            {
                ObjectCacheManager.CombatIds.Add(mobId);
                ObjectCacheManager.LootIds.Add(mobId);
            }

            foreach (var herb in CacheStaticLookUp.HerbDeposits)
            {
                ObjectCacheManager.LootIds.Add(herb);
            }

            TargetManager.ShouldLoot = true;
            TargetManager.ShouldKill = true;

            List<WoWPoint> herbpoints = Character.Player.IsAlliance
                ? MovementCache.Alliance_Herb_LevelOne
                : MovementCache.Horde_Herb_LevelOne;

            _movementQueue = new Queue<WoWPoint>();
            foreach (var p in herbpoints)
            {
                _movementQueue.Enqueue(p);
            }
            _movement = null;
        }

        public override void Dispose()
        {
            TargetManager.ShouldLoot = false;
            TargetManager.ShouldKill = false;
            foreach (var id in CacheStaticLookUp.HerbQuestMobIDs)
            {
                ObjectCacheManager.CombatIds.Remove(id);
                ObjectCacheManager.LootIds.Remove(id);
            }
            foreach (var herb in CacheStaticLookUp.HerbDeposits)
            {
                ObjectCacheManager.LootIds.Remove(herb);
            }
            base.Dispose();
        }

        private Movement _movement;
        private Queue<WoWPoint> _movementQueue;

        public override async Task<bool> BehaviorRoutine()
        {
            if (await base.BehaviorRoutine()) return true;
            if (IsDone) return false;

            if (await StartMovement.MoveTo()) return true;



            TargetManager.UpdateLootableTarget();

            if (_movement == null || _movement.CurrentMovementQueue.Count == 0)
            {
                if (_movementQueue.Count > 0)
                {
                    _movement = new Movement(_movementQueue.Dequeue(), 5f, name: "Herbing");
                }
            }

            if (ObjectCacheManager.FoundHerbObject)
            {
                if (_movementQueue.Count > 0)
                {
                    if (_movement != null)
                    {
                        if (await _movement.MoveTo()) return true;
                    }

                    return true;
                }
            }

            if (await EndMovement.MoveTo()) return true;

            BaseSettings.CurrentSettings.LastCheckedHerbString = LuaCommands.GetGameTime().ToString("yyyy-MM-ddTHH:mm:ss");
            BaseSettings.SerializeToXML(BaseSettings.CurrentSettings);
            TargetManager.ShouldLoot = false;

            return false;
        }
    }
}