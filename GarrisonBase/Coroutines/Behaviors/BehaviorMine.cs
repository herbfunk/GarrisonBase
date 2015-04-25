using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Herbfunk.GarrisonBase.Cache;
using Herbfunk.GarrisonBase.Cache.Enums;
using Herbfunk.GarrisonBase.Garrison;
using Herbfunk.GarrisonBase.Garrison.Enums;
using Herbfunk.GarrisonBase.TargetHandling;
using Styx;

namespace Herbfunk.GarrisonBase.Coroutines.Behaviors
{
    public class BehaviorMine : Behavior
    {
        public override BehaviorType Type { get { return BehaviorType.Mining; } }

        public BehaviorMine():base(MovementCache.MinePlot59SafePoint)
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

            //Coroutines.Movement.MovementCache.ShouldRecord = true;

            foreach (var mobId in CacheStaticLookUp.MineQuestMobIDs)
            {
                ObjectCacheManager.CombatIds.Add(mobId);
                ObjectCacheManager.LootIds.Add(mobId);
            }

            foreach (var deposit in CacheStaticLookUp.MineDeposits)
            {
                ObjectCacheManager.LootIds.Add(deposit);
            }

            TargetManager.ShouldLoot = true;
            TargetManager.ShouldKill = true;
            TargetManager.LootDistance = 25f;
            ObjectCacheManager.IgnoreLineOfSightFailure = true;

            if (_movementQueue != null) return;

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
            //Coroutines.Movement.MovementCache.ResetCache();
            TargetManager.ShouldLoot = false;
            TargetManager.ShouldKill = false;
            ObjectCacheManager.IgnoreLineOfSightFailure = false;

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

           // ObjectCacheManager.UpdateLootableTarget();

            if (_movement == null || _movement.CurrentMovementQueue.Count == 0)
            {
                if (_movementQueue.Count > 0)
                {
                    //while (_movementQueue.Count > 0)
                    //{
                    //    if (ObjectCacheManager.GetGameObjectsNearPoint(_movementQueue.Peek(), 50f, WoWObjectTypes.OreVein).Count==0)
                    //    {
                    //        GarrisonBase.Debug("Dequeueing point from mine movement!");
                    //        _movementQueue.Dequeue();
                    //        continue;
                    //    }

                    //    break;
                    //}

                    if (_movementQueue.Count > 0)
                    {
                        _movement = new Movement(_movementQueue.Dequeue(), 5f, name: "Mining");
                        TargetManager.LootDistance += 10f;
                    }
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
            TargetManager.ShouldLoot = false;

            return false;
        }


           
    }
}