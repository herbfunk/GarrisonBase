using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Herbfunk.GarrisonBase.Cache;
using Herbfunk.GarrisonBase.Cache.Enums;
using Herbfunk.GarrisonBase.TargetHandling;
using Styx;
using Styx.Common.Helpers;
using Styx.CommonBot;
using Styx.CommonBot.Coroutines;
using Styx.CommonBot.Frames;
using Styx.Pathing;

namespace Herbfunk.GarrisonBase.Coroutines
{
    public static class LootBehavior
    {
        private static Movement _lootMovement;

        public static async Task<bool> ExecuteBehavior()
        {
            if (await CheckLootFrame())
                return true;

            //Loot Units

            //Harvest Objects
            //  -Gathering
            //  -Skinning

            if (ValidateLootObject())
            {
                if (TargetManager.LootableObject.WithinInteractRange)
                {
                    TreeRoot.StatusText = "Looting";
                    if (await Looting()) return true;
                }
                else
                {
                    TreeRoot.StatusText = "Loot Movement";
                    await LootMovement();
                    return true;
                }
            }

            return false;
        }

        private static bool ValidateLootObject()
        {
            if (ObjectCacheManager.ShouldUpdateObjectCollection)
            {
                ObjectCacheManager.UpdateCache();
            }

            if (!TargetManager.ShouldLoot)
            {
                return false;
            }

            if (TargetManager.LootableObject == null || !TargetManager.LootableObject.IsValid)
            {
                if (TargetManager.LootableObject != null)
                    TargetManager.LootableObject.ShouldLoot = false;


                TargetManager.UpdateLootableTarget();
                if (TargetManager.LootableObject == null)
                {
                    ResetLoot();
                    return false;
                }
            }

            if (!TargetManager.LootableObject.ValidForLooting)
            {
                GarrisonBase.Debug("LootObject no longer valid for looting {0}", TargetManager.LootableObject.Name);

                IgnoreLootableObject();

                TargetManager.UpdateLootableTarget();

                ResetLoot();
                return false;
            }

            return true;
        }

        private static async Task<bool> LootMovement()
        {
            if (_lootMovement == null)
            {
                _lootMovement = new Movement(TargetManager.LootableObject.Location,
                    TargetManager.LootableObject.InteractRange - 0.25f,
                    String.Format("Loot {0}", TargetManager.LootableObject.Name));
            }

            MoveResult result = await _lootMovement.MoveTo_Result();

            if (result == MoveResult.Failed)
            {
                if (_lootMovement.CanNavigateFailures > 2)
                {
                    GarrisonBase.Debug("Behavior Looting Movement FAILED 3x Times! for {0}", TargetManager.LootableObject.Name);
                    TargetManager.LootableObject.IgnoredTimer = WaitTimer.TenSeconds;
                    TargetManager.LootableObject.IgnoredTimer.Reset();
                    ResetLoot();
                    return false;
                }

                return true;
            }

            if (result != MoveResult.ReachedDestination) return true;

            ResetLoot();
            return false;
        }

        private static async Task<bool> Looting()
        {
            if (StyxWoW.Me.IsMoving && await CommonCoroutines.StopMoving())

                await CommonCoroutines.SleepForLagDuration();

            if (TargetManager.LootableObject.IsValid)
            {
                bool success = await CommonCoroutines.WaitForLuaEvent(
                "LOOT_CLOSED",
                4200,
                null,
                TargetManager.LootableObject.Interact);

                if (TargetManager.LootableObject.InteractionAttempts > 1)
                {
                    GarrisonBase.Debug("LootObject interaction attempts has excedeed max! Ignoring {0}", TargetManager.LootableObject.Name);
                    IgnoreLootableObject();
                    ResetLoot();
                    return false;

                }

                if (success)
                {
                    await CheckLootFrame();
                    IgnoreLootableObject();
                    ResetLoot();
                }
            }

            return true;
        }

        private static async Task<bool> CheckLootFrame()
        {
            // loot everything.
            if (!LuaEvents.LootOpened) return false;

            var lootSlotInfos = new List<LootSlotInfo>();
            for (int i = 0; i < LootFrame.Instance.LootItems; i++)
            {
                lootSlotInfos.Add(LootFrame.Instance.LootInfo(i));
            }

            if (await Coroutine.Wait(2000, () =>
            {
                LootFrame.Instance.LootAll();
                return !LuaEvents.LootOpened;
            }))
            {
                GarrisonBase.Log("Succesfully looted: ");
                foreach (LootSlotInfo lootinfo in lootSlotInfos)
                {
                    try
                    {
                        string lootQuanity = lootinfo.LootQuantity.ToString();
                        string lootName = lootinfo.LootName;
                        GarrisonBase.Log(lootQuanity + "x " + lootName);
                    }
                    catch
                    {
                        GarrisonBase.Log("exception occured");
                    }

                }
            }
            else
            {
                GarrisonBase.Err("Failed to loot from Frame.");
            }
            await CommonCoroutines.SleepForLagDuration();
            return true;
        }

        internal static void ResetLoot()
        {
            _lootMovement = null;
        }

        internal static void IgnoreLootableObject()
        {
            TargetManager.LootableObject.ShouldLoot = false;
            TargetManager.LootableObject.NeedsRemoved = true;
            TargetManager.LootableObject.BlacklistType = BlacklistType.Guid;
            TargetManager.LootableObject = null;
        }
    }
}
