using System;
using System.Threading.Tasks;
using Herbfunk.GarrisonBase.Cache;
using Herbfunk.GarrisonBase.Cache.Enums;
using Styx;
using Styx.CommonBot.Coroutines;
using Styx.Pathing;

namespace Herbfunk.GarrisonBase.Coroutines
{
    public partial class Common
    {
        private static Movement _lootMovement;
        public static async Task<bool> LootObject()
        {
            if (ObjectCacheManager.ShouldUpdateObjectCollection)
            {
                ObjectCacheManager.UpdateCache();
            }

            if (!ObjectCacheManager.ShouldLoot)
            {
                _lootMovement = null;
                return false;
            }

            if (ObjectCacheManager.LootableObject == null || !ObjectCacheManager.LootableObject.IsValid)
            {
                ObjectCacheManager.UpdateLootableTarget();
                if (ObjectCacheManager.LootableObject == null)
                {
                    _lootMovement = null;
                    return false;
                }
            }

            //if (!ObjectCacheManager.LootableObject.IsValid)
            //{
            //    GarrisonBase.Debug("LootObject no longer valid!");
            //    _lootMovement = null;
            //    ObjectCacheManager.LootableObject = null;
            //    return false;
            //}

            if (!ObjectCacheManager.LootableObject.ValidForLooting)
            {
                GarrisonBase.Debug("LootObject no longer valid for looting!");
                ObjectCacheManager.UpdateLootableTarget();
                _lootMovement = null;
                return false;
            }



            if (_lootMovement == null)
            {
                _lootMovement = new Movement(ObjectCacheManager.LootableObject.Location,
                    ObjectCacheManager.LootableObject.InteractRange,
                    name: String.Format("Loot {0}", ObjectCacheManager.LootableObject.Name));
            }
            else if (_lootMovement.CurrentMovementQueue.Count == 0)
            {
                _lootMovement = new Movement(ObjectCacheManager.LootableObject.Location,
                    ObjectCacheManager.LootableObject.InteractRange -= 0.25f,
                    name: String.Format("Loot {0}", ObjectCacheManager.LootableObject.Name));
            }

            //TreeRoot.StatusText = String.Format("Behavior Looting Movement {0}", ObjectCacheManager.LootableObject.Name);
            MoveResult result = await _lootMovement.MoveTo_Result();

            if (ObjectCacheManager.LootableObject.WithinInteractRange)
            {
                //TreeRoot.StatusText = String.Format("Behavior Looting {0}", ObjectCacheManager.LootableObject.Name);
                if (StyxWoW.Me.IsMoving)
                {
                    await CommonCoroutines.StopMoving();
                }

                await CommonCoroutines.SleepForLagDuration();
                //await Coroutine.Sleep(StyxWoW.Random.Next(1001, 1999));

                if (ObjectCacheManager.LootableObject.IsValid)
                {
                    bool success = await CommonCoroutines.WaitForLuaEvent(
                    "LOOT_OPENED",
                    7500,
                    () => LuaEvents.LootOpened,
                    ObjectCacheManager.LootableObject.Interact);

                    if (success)
                    {
                        await CheckLootFrame();
                        ObjectCacheManager.LootableObject.NeedsRemoved = true;
                        ObjectCacheManager.LootableObject.BlacklistType = BlacklistType.Guid;
                        _lootMovement = null;
                    }
                }

                return true;
            }


            if (result == MoveResult.Failed)
            {
                GarrisonBase.Debug("Behavior Looting Movement FAILED for {0}", ObjectCacheManager.LootableObject.Name);
                ObjectCacheManager.LootableObject.NeedsRemoved = true;
                ObjectCacheManager.LootableObject.BlacklistType= BlacklistType.Guid;
                ObjectCacheManager.LootableObject = null;
                _lootMovement = null;
                return false;
            }

            return true;
        }
    }
}
