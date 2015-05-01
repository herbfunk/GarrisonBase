using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bots.Quest.Actions.Combat;
using Herbfunk.GarrisonBase.Cache;
using Herbfunk.GarrisonBase.Cache.Enums;
using Herbfunk.GarrisonBase.Cache.Objects;
using Herbfunk.GarrisonBase.Character;
using Herbfunk.GarrisonBase.TargetHandling;
using Styx;
using Styx.Common.Helpers;
using Styx.CommonBot;
using Styx.CommonBot.Coroutines;
using Styx.CommonBot.POI;
using Styx.CommonBot.Routines;
using Styx.Helpers;
using Styx.Pathing;

namespace Herbfunk.GarrisonBase.Coroutines
{
    public static class CombatBehavior
    {
        public static async Task<bool> ExecuteBehavior()
        {
            bool inCombat = StyxWoW.Me.Combat;


            //Check for custom combat
            if (TargetManager.CombatType != TargetManager.CombatFlags.Normal)
            {
                if (ValidateCombatObject())
                {
                    if (TargetManager.CombatObject.Distance <= TargetManager.PullDistance && await ValidateCombatPoi())
                        return true;

                    if (await TrappingBehavior(inCombat))
                        return true;

                    if (TargetManager.CombatObject.Distance > TargetManager.PullDistance)
                    {
                        if (!await CombatBehaviorActuallyInCombat())
                        {
                            if (TargetManager.LootableObject != null)
                                return false;

                            if (await CombatMovement())
                                return true;
                        }
                    }
                    else
                    {


                        if (!await CombatBehaviorActuallyInCombat())
                        {
                            await RoutineManager.Current.CombatBehavior.ExecuteCoroutine();
                        }

                        return true;
                    }
                }
            }

            if (!inCombat)
            {
                #region Rest
                //Rest
                if (RoutineManager.Current.RestBehavior != null &&
                    await RoutineManager.Current.RestBehavior.ExecuteCoroutine())
                {
                    TreeRoot.StatusText = "Rest Behavior";
                    return true;
                }

                if (RoutineManager.Current.NeedRest)
                {
                    TreeRoot.StatusText = "Rest Behavior";
                    RoutineManager.Current.Rest();
                    return true;
                }
                #endregion

                #region Precombat Buffs
                if (TargetManager.CheckCombatFlag(TargetManager.CombatFlags.PrecombatBuffs))
                {
                    //Precombat Buffs
                    if (RoutineManager.Current.PreCombatBuffBehavior != null &&
                        await RoutineManager.Current.PreCombatBuffBehavior.ExecuteCoroutine())
                    {
                        TreeRoot.StatusText = "PreCombat Behavior";
                        return true;
                    }

                    if (RoutineManager.Current.NeedPreCombatBuffs)
                    {
                        TreeRoot.StatusText = "PreCombat Behavior";
                        RoutineManager.Current.PreCombatBuff();
                        return true;
                    }
                }
                #endregion

                #region Pull Behavior
                if (TargetManager.CheckCombatFlag(TargetManager.CombatFlags.Pull))
                {
                    if (BotPoi.Current.Type == PoiType.Kill)
                    {
                        if (Targeting.Instance.TargetList.Count != 0)
                        {
                            if (BotPoi.Current.AsObject != Targeting.Instance.FirstUnit &&
                                BotPoi.Current.Type == PoiType.Kill)
                            {
                                BotPoi.Current = new BotPoi(Targeting.Instance.FirstUnit, PoiType.Kill);
                                BotPoi.Current.AsObject.ToUnit().Target();
                                ResetCombat();

                                if (NeedPull())
                                {
                                    if (RoutineManager.Current.PullBuffBehavior != null &&
                                        await RoutineManager.Current.PullBuffBehavior.ExecuteCoroutine())
                                    {
                                        TreeRoot.StatusText = "Pull Buff Behavior";
                                        return true;
                                    }

                                    if (RoutineManager.Current.PullBehavior != null &&
                                        await RoutineManager.Current.PullBehavior.ExecuteCoroutine())
                                    {
                                        TreeRoot.StatusText = "PullBehavior Behavior";
                                        return true;
                                    }

                                    if (await new ActionPull().ExecuteCoroutine())
                                    {
                                        TreeRoot.StatusText = "Action Pull Behavior";
                                        return true;
                                    }
                                }
                                return true;
                            }
                        }
                    }
                }

                #endregion
            }
            else
            {
                #region Heal

                if (TargetManager.CheckCombatFlag(TargetManager.CombatFlags.Heal))
                {
                    if (RoutineManager.Current.HealBehavior != null &&
                        await RoutineManager.Current.HealBehavior.ExecuteCoroutine())
                    {
                        TreeRoot.StatusText = "Heal Behavior";
                        return true;
                    }

                    if (RoutineManager.Current.NeedHeal)
                    {
                        TreeRoot.StatusText = "Heal Behavior";
                        RoutineManager.Current.Heal();
                        return true;
                    }
                }

                #endregion

                #region Combat Buff
                if (TargetManager.CheckCombatFlag(TargetManager.CombatFlags.CombatBuffs))
                {
                    //Combat Buffs
                    if (RoutineManager.Current.CombatBuffBehavior != null &&
                        await RoutineManager.Current.CombatBuffBehavior.ExecuteCoroutine())
                    {
                        TreeRoot.StatusText = "Combat Buff Behavior";
                        return true;
                    }
                }
                #endregion

                #region Combat
                if (TargetManager.CheckCombatFlag(TargetManager.CombatFlags.Combat))
                {
                    if (StyxWoW.Me.IsActuallyInCombat && !StyxWoW.Me.Mounted)
                    {
                        //Combat
                        if (RoutineManager.Current.CombatBehavior != null &&
                            await RoutineManager.Current.CombatBehavior.ExecuteCoroutine())
                        {
                            TreeRoot.StatusText = "Combat Behavior";
                        }

                        return true;
                    }
                }
                #endregion
            }


            return false;
        }

        internal static void ResetCombat()
        {
            _combatMovement = null;
            _trapMovement = null;
        }

        private static bool ValidateCombatObject()
        {
            if (ObjectCacheManager.ShouldUpdateObjectCollection)
                ObjectCacheManager.UpdateCache();

            if (!TargetManager.ShouldKill)
            {
                ResetCombat();
                return false;
            }

            if (TargetManager.CombatObject == null)
            {
                TargetManager.UpdateCombatTarget();
                if (TargetManager.CombatObject == null)
                {
                    ResetCombat();
                    return false;
                }
            }

            if (!TargetManager.CombatObject.ValidForCombat)
            {
                GarrisonBase.Debug("EngageObject no longer valid for combat!");
                TargetManager.UpdateCombatTarget();


                //Clear and update lootable object
                TargetManager.LootableObject = null;
                LootBehavior.ResetLoot();
                TargetManager.UpdateLootableTarget();

                ResetCombat();
                return false;
            }

            if (TargetManager.CombatObject.IsEvadeRunningBack)
            {
                GarrisonBase.Debug("EngageObject is evading!");
                TargetManager.CombatObject.IgnoredTimer = WaitTimer.TenSeconds;
                TargetManager.CombatObject.IgnoredTimer.Reset();
                return false;
            }

            return true;
        }

        private static async Task<bool> ValidateCombatPoi()
        {
            if (StyxWoW.Me.CurrentTarget == null || StyxWoW.Me.CurrentTarget.Guid != TargetManager.CombatObject.Guid)
            {
                GarrisonBase.Debug("Behavior Combat changing POI to unit {0}", TargetManager.CombatObject.Name);
                BotPoi.Current = new BotPoi(TargetManager.CombatObject.RefWoWUnit, PoiType.Kill);
                TargetManager.CombatObject.RefWoWUnit.Target();
                ResetCombat();
                return true;
            }

            return false;
        }

        private static Movement _combatMovement;
        private static async Task<bool> CombatMovement()
        {
            if (_combatMovement == null)
            {
                _combatMovement = new Movement(TargetManager.CombatObject,
                   (float)TargetManager.PullDistance,
                   String.Format("Combat {0}", TargetManager.CombatObject.Name));
            }


            TreeRoot.StatusText = "Combat Movement Behavior";
            var result = await _combatMovement.MoveTo_Result(false);

            if (result == MoveResult.Failed)
            {
                if (_combatMovement.CanNavigateFailures > 2)
                {
                    GarrisonBase.Debug("Behavior Combat Movement FAILED for {0}", TargetManager.CombatObject.Name);
                    TargetManager.CombatObject.IgnoredTimer = WaitTimer.TenSeconds;
                    TargetManager.CombatObject = null;
                    _combatMovement = null;
                    return false;
                }

                return true;
            }

            if (result != MoveResult.ReachedDestination) return true;

            _combatMovement = null;
            return false;
        }

        private static async Task<bool> CombatBehaviorActuallyInCombat()
        {
            if ((StyxWoW.Me.IsActuallyInCombat && !StyxWoW.Me.Mounted))
            {
                TreeRoot.StatusText = "Combat Behavior";
                await RoutineManager.Current.CombatBehavior.ExecuteCoroutine();
                return true;
            }

            return false;
        }

        private static bool NeedPull()
        {
            var target = StyxWoW.Me.CurrentTarget;

            if (target == null)
                return false;

            if (!target.InLineOfSight)
                return false;

            if (target.Distance > Targeting.PullDistance)
                return false;

            return true;
        }


        private static Movement _trapMovement;
        private static async Task<bool> TrappingBehavior(bool inCombat)
        {
            if (TargetManager.CombatType != TargetManager.CombatFlags.Trapping ||
                                        !TargetManager.CombatObject.Trappable)
                return false;


            bool withinTrapDistance = TargetManager.CombatObject.Distance <= TargetManager.PullDistance;
            var traps = nearbyTraps;
            if (traps.Count == 0 && withinTrapDistance)
            {
                if (Player.Inventory.Trap != null)
                {
                    if (Player.Inventory.Trap.ref_WoWItem.Usable &&
                        !Player.Inventory.Trap.OnCooldown)
                    {
                        TreeRoot.StatusText = "Trapping Behavior";
                        Player.Inventory.Trap.Use();
                        await CommonCoroutines.SleepForRandomUiInteractionTime();
                        return true;
                    }
                }
            }

            if (inCombat)
            {
                TreeRoot.StatusText = "Trapping Behavior";

                //We want to move the target to our trap..
                //  -If we don't have aggro than we should move to the target
                //  -Else we should wait near our trap.

                if (!TargetManager.CombatObject.IsTargetingMe)
                {
                    //Wait for movement..
                    if (!withinTrapDistance) return false;
                }
                else
                {
                    //Move to nearest trap..
                    if (traps.Count > 0)
                    {
                        var nearestTrap = traps[0];
                        var distance = nearestTrap.Location.Distance(TargetManager.CombatObject.Location);
                        if (distance > 1.5f || _trapMovement == null)
                        {
                            if (_trapMovement == null || _trapMovement.CurrentMovementQueue.Count == 0)
                            {
                                //Get the current side facing flipped!
                                float requiredFacingDegrees = WoWMathHelper.RadiansToDegrees(
                                                             WoWMathHelper.CalculateNeededFacing(
                                                                            TargetManager.CombatObject.Location, nearestTrap.Location));




                                _trapMovement =
                                    new Movement(
                                        nearestTrap.GetSidePoint(requiredFacingDegrees, 8f),
                                        5f,
                                        "Trap Point");
                            }


                            if (_trapMovement != null) await _trapMovement.MoveTo();
                            return true;
                        }
                    }
                }
            }
            else if (withinTrapDistance) //Not in Combat
            {
                if (RoutineManager.Current.CombatBehavior != null &&
                            await RoutineManager.Current.CombatBehavior.ExecuteCoroutine())
                {
                    TreeRoot.StatusText = "Combat Behavior";
                    return true;
                }
            }

            return false;
        }

        internal static List<C_WoWUnit> nearbyTraps
        {
            get
            {
                return
                    ObjectCacheManager.ObjectCollection.Values.OfType<C_WoWUnit>().
                    Where(obj => obj.SubType == WoWObjectTypes.Trap
                        //&& (obj.Flags & 0x80000)==0 
                        && obj.Distance < 7f).OrderBy(obj => obj.Distance).ToList();
            }
        }
    }
}
