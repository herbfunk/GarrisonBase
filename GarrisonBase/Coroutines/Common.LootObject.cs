using System;
using System.Threading.Tasks;
using Herbfunk.GarrisonBase.Cache;
using Herbfunk.GarrisonBase.Cache.Enums;
using Herbfunk.GarrisonBase.TargetHandling;
using Styx;
using Styx.Common.Helpers;
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

            if (!TargetManager.ShouldLoot)
            {
                _lootMovement = null;
                return false;
            }

            if (TargetManager.LootableObject == null || !TargetManager.LootableObject.IsValid)
            {
                TargetManager.UpdateLootableTarget();
                if (TargetManager.LootableObject == null)
                {
                    _lootMovement = null;
                    return false;
                }
            }

            //if (!TargetManager.LootableObject.IsValid)
            //{
            //    GarrisonBase.Debug("LootObject no longer valid!");
            //    _lootMovement = null;
            //    TargetManager.LootableObject = null;
            //    return false;
            //}

            if (!TargetManager.LootableObject.ValidForLooting)
            {
                GarrisonBase.Debug("LootObject no longer valid for looting {0}", TargetManager.LootableObject.Name);
                
                TargetManager.LootableObject.NeedsRemoved = true;
                TargetManager.LootableObject.BlacklistType= BlacklistType.Guid;
                Blacklist.TempBlacklistGuids.Add(TargetManager.LootableObject.Guid);

                TargetManager.LootableObject = null;

                TargetManager.UpdateLootableTarget();

                _lootMovement = null;
                return false;
            }



            if (_lootMovement == null)
            {
                _lootMovement = new Movement(TargetManager.LootableObject.Location,
                    TargetManager.LootableObject.InteractRange,
                    name: String.Format("Loot {0}", TargetManager.LootableObject.Name));
            }
            else if (_lootMovement.CurrentMovementQueue.Count == 0)
            {
                _lootMovement = new Movement(TargetManager.LootableObject.Location,
                    TargetManager.LootableObject.InteractRange -= 0.25f,
                    name: String.Format("Loot {0}", TargetManager.LootableObject.Name));
            }

            //TreeRoot.StatusText = String.Format("Behavior Looting Movement {0}", TargetManager.LootableObject.Name);
            MoveResult result = await _lootMovement.MoveTo_Result();

            if (TargetManager.LootableObject.WithinInteractRange)
            {
                //TreeRoot.StatusText = String.Format("Behavior Looting {0}", TargetManager.LootableObject.Name);
                if (StyxWoW.Me.IsMoving)
                {
                    await CommonCoroutines.StopMoving();
                }

                await CommonCoroutines.SleepForLagDuration();
                //await Coroutine.Sleep(StyxWoW.Random.Next(1001, 1999));

                if (TargetManager.LootableObject.IsValid)
                {
                    bool success = await CommonCoroutines.WaitForLuaEvent(
                    "LOOT_CLOSED",
                    6000,
                    null,
                    TargetManager.LootableObject.Interact);

                    if (TargetManager.LootableObject.InteractionAttempts > 2)
                    {
                        GarrisonBase.Debug("LootObject interaction attempts has excedeed max! Ignoring {0}", TargetManager.LootableObject.Name);
                        TargetManager.LootableObject.NeedsRemoved = true;
                        TargetManager.LootableObject.BlacklistType = BlacklistType.Guid;
                        TargetManager.LootableObject = null;
                        _lootMovement = null;
                        return false;
                        
                    }

                    if (success)
                    {
                        await CheckLootFrame();
                        TargetManager.LootableObject.NeedsRemoved = true;
                        TargetManager.LootableObject.BlacklistType = BlacklistType.Guid;
                        _lootMovement = null;
                    }
                }

                return true;
            }


            if (result == MoveResult.Failed)
            {
                GarrisonBase.Debug("Behavior Looting Movement FAILED for {0}", TargetManager.LootableObject.Name);
                TargetManager.LootableObject.NeedsRemoved = true;
                TargetManager.LootableObject.BlacklistType= BlacklistType.Guid;
                TargetManager.LootableObject = null;
                _lootMovement = null;
                return false;
            }

            return true;
        }
    }
}
