using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Herbfunk.GarrisonBase.Cache;
using Herbfunk.GarrisonBase.Garrison;
using Herbfunk.GarrisonBase.Garrison.Enums;
using Styx;

namespace Herbfunk.GarrisonBase.Coroutines.Behaviors
{
    public class BehaviorMine : Behavior
    {
        public override BehaviorType Type { get { return BehaviorType.Mining; } }

        public BehaviorMine()
            : base(MovementCache.MinePlot59SafePoint)
        {
            Criteria += () =>
                    (!GarrisonManager.Buildings[BuildingType.Mines].IsBuilding &&
                     !GarrisonManager.Buildings[BuildingType.Mines].CanActivate &&
                     GarrisonManager.Buildings[BuildingType.Mines].FirstQuestCompleted &&
                     LuaCommands.CheckForDailyReset(BaseSettings.CurrentSettings.LastCheckedMine) &&
                     BaseSettings.CurrentSettings.BehaviorMineGather);
        }



        public override void Initalize()
        {
            base.Initalize();

            foreach (var mobId in CacheStaticLookUp.MineQuestMobIDs)
            {
                ObjectCacheManager.CombatIds.Add(mobId);
                ObjectCacheManager.LootIds.Add(mobId);
            }

            foreach (var deposit in CacheStaticLookUp.MineDeposits)
            {
                ObjectCacheManager.LootIds.Add(deposit);
            }
            
            ObjectCacheManager.ShouldLoot = true;
            ObjectCacheManager.ShouldKill = true;

            List<WoWPoint> miningPoints;

            if (Character.Player.IsAlliance)
                miningPoints = GarrisonManager.Buildings[BuildingType.Mines].Level == 1
                    ? MovementCache.Alliance_Mine_LevelOne
                    : GarrisonManager.Buildings[BuildingType.Mines].Level == 2
                        ? MovementCache.Alliance_Mine_LevelTwo
                        : MovementCache.Alliance_Mine_LevelThree;
            else
                miningPoints = GarrisonManager.Buildings[BuildingType.Mines].Level == 1
                    ? MovementCache.Horde_Mine_LevelOne
                    : GarrisonManager.Buildings[BuildingType.Mines].Level == 2
                        ? MovementCache.Horde_Mine_LevelTwo
                        : MovementCache.Horde_Mine_LevelThree;

            _movementQueue = new Queue<WoWPoint>();
            foreach (var p in miningPoints)
            {
                _movementQueue.Enqueue(p);
            }
            _movement = null;
        }

        public override void Dispose()
        {
            ObjectCacheManager.ShouldLoot = false;
            ObjectCacheManager.ShouldKill = false;
            foreach (var id in CacheStaticLookUp.MineQuestMobIDs)
            {
                ObjectCacheManager.CombatIds.Remove(id);
                ObjectCacheManager.LootIds.Remove(id);
            }
            foreach (var deposit in CacheStaticLookUp.MineDeposits)
            {
                ObjectCacheManager.LootIds.Remove(deposit);
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

            
           // ObjectCacheManager.UpdateLootableTarget();

            if (_movement == null || _movement.CurrentMovementQueue.Count == 0)
            {
                if (_movementQueue.Count > 0)
                {
                    _movement = new Movement(_movementQueue.Dequeue(), 5f);
                }
            }

            if (ObjectCacheManager.FoundOreObject)
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

            BaseSettings.CurrentSettings.LastCheckedMineString = LuaCommands.GetGameTime().ToString("yyyy-MM-ddTHH:mm:ss");
            BaseSettings.SerializeToXML(BaseSettings.CurrentSettings);
            ObjectCacheManager.ShouldLoot = false;

            return false;
        }


           
    }
}