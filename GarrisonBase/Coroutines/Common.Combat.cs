using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Documents;
using Buddy.Coroutines;
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
using Styx.Pathing;

namespace Herbfunk.GarrisonBase.Coroutines
{
    public partial class Common
    {
        internal static bool IsTrapping = false;
        public static async Task<bool> Combat()
        {
            bool inCombat = StyxWoW.Me.Combat;
            
            if (!inCombat && await RoutineManager.Current.PreCombatBuffBehavior.ExecuteCoroutine())
            {
                TreeRoot.StatusText = "Pre Combat Buff Behavior";
                return true;
            }

            if (ValidateCombatObject())
            {
                bool actualCombat = false;
                if (TargetManager.CombatObject.Distance <= TargetManager.PullDistance)
                {
                    bool poiCheck=await ValidateCombatPoi();

                    

                    if (poiCheck && TargetManager.CombatType== TargetManager.CombatFlags.Normal)
                    {
                        if (RoutineManager.Current.PullBuffBehavior != null)
                        {
                            await RoutineManager.Current.PullBuffBehavior.ExecuteCoroutine();
                            TreeRoot.StatusText = "Pull Buff Behavior";
                        }

                        if (RoutineManager.Current.PullBehavior != null)
                        {
                            await RoutineManager.Current.PullBehavior.ExecuteCoroutine();
                            TreeRoot.StatusText = "Pull Behavior";
                        }

                        TreeRoot.StatusText = "POI Combat Behavior";
                        await RoutineManager.Current.CombatBehavior.ExecuteCoroutine();

                        return true;
                    }

                    if (await TrappingBehavior(inCombat))
                        return true;

                    actualCombat = await CombatBehaviorActuallyInCombat(inCombat);
                    if (!actualCombat)
                    {
                        await RoutineManager.Current.CombatBehavior.ExecuteCoroutine();
                    }

                    

                    return true;
                }

                actualCombat = await CombatBehaviorActuallyInCombat(inCombat);
                if (!actualCombat)
                {
                    if (TargetManager.LootableObject != null)
                        return false;

                    if (await CombatMovement())
                        return true;
                }

                return true;
            }

            //if (await EngageObject())
            //{
            //    return true;
            //}


            if (await CombatBehaviorActuallyInCombat(inCombat))
                return true;


            return false;
        }

        
        public static async Task<bool> EngageObject()
        {
            if (!ValidateCombatObject())
            {
                return false;
            }


            if (TargetManager.CombatObject.Distance <= Targeting.PullDistance)
            {
                if (!TargetManager.CombatObject.IsValid) return true;

                if (StyxWoW.Me.CurrentTarget == null || StyxWoW.Me.CurrentTarget.Guid != TargetManager.CombatObject.Guid)
                {
                    GarrisonBase.Debug("Behavior Combat changing POI to unit {0}", TargetManager.CombatObject.Name);
                    BotPoi.Current = new BotPoi(TargetManager.CombatObject.RefWoWUnit, PoiType.Kill);

                    if (RoutineManager.Current.PullBuffBehavior != null)
                    {
                        await RoutineManager.Current.PullBuffBehavior.ExecuteCoroutine();
                        TreeRoot.StatusText = "Pull Buff Behavior";
                    }

                    if (RoutineManager.Current.PullBehavior != null)
                    {
                        await RoutineManager.Current.PullBehavior.ExecuteCoroutine();
                        TreeRoot.StatusText = "Pull Behavior";
                    }

                    return false;
                }

                await RoutineManager.Current.CombatBehavior.ExecuteCoroutine();
                return true;
            }

            if (_combatMovement == null)
            {
                _combatMovement = new Movement(TargetManager.CombatObject.Location,
                    RoutineManager.Current.PullDistance.HasValue ? (float)RoutineManager.Current.PullDistance.Value :
                        TargetManager.CombatObject.InteractRange,
                    name: String.Format("Combat {0}", TargetManager.CombatObject.Name));
            }
            else if (_combatMovement.CurrentMovementQueue.Count == 0)
            {
                _combatMovement = new Movement(TargetManager.CombatObject.Location,
                    TargetManager.CombatObject.InteractRange -= 0.25f,
                    name: String.Format("Combat {0}", TargetManager.CombatObject.Name));
            }

            TreeRoot.StatusText = "Combat Movement Behavior";
            var result = await _combatMovement.MoveTo_Result();

            if (result == MoveResult.Failed)
            {
                GarrisonBase.Debug("Behavior Combat Movement FAILED for {0}", TargetManager.CombatObject.Name);
                TargetManager.CombatObject.IgnoredTimer = WaitTimer.TenSeconds;
                TargetManager.CombatObject = null;
                _combatMovement = null;
                return false;
            }

            return true;
        }

        private static void ResetCombat()
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
                _lootMovement = null;
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
                GarrisonBase.Debug("Behavior Combat Movement FAILED for {0}", TargetManager.CombatObject.Name);
                TargetManager.CombatObject.IgnoredTimer = WaitTimer.TenSeconds;
                TargetManager.CombatObject = null;
                _combatMovement = null;
                return false;
            }

            if (result == MoveResult.ReachedDestination)
            {
                _combatMovement = null;
                return false;
            }

            return true;
        }

        private static async Task<bool> CombatBehaviorActuallyInCombat(bool inCombat)
        {
            if ((inCombat && StyxWoW.Me.IsActuallyInCombat && !StyxWoW.Me.Mounted))
            {

                TreeRoot.StatusText = "Combat Behavior";
                await RoutineManager.Current.CombatBehavior.ExecuteCoroutine();
                return true;
            }

            return false;
        }

        private static Movement _trapMovement;
        private static async Task<bool> TrappingBehavior(bool inCombat)
        {
            if (inCombat && TargetManager.CombatType == TargetManager.CombatFlags.Trapping &&
                        TargetManager.CombatObject.Trappable)
            {
                var traps = nearbyTraps;
                if (traps.Count == 0)
                {
                    if (Player.Inventory.Trap != null)
                    {
                        if (Player.Inventory.Trap.ref_WoWItem.Usable &&
                            !Player.Inventory.Trap.OnCooldown)
                        {
                            Player.Inventory.Trap.Use();
                            await Coroutine.Sleep(StyxWoW.Random.Next(1500, 2001));
                        }
                    }
                }
                else
                {
                    if (_trapMovement == null)
                    {
                        var nearestTrap = traps[0];
                        var distance = nearestTrap.Location.Distance(TargetManager.CombatObject.Location);
                        if (distance > 1.5f)
                        {
                            if (!TargetManager.CombatObject.IsBehindObject(nearestTrap))
                            {
                                _trapMovement = new Movement(new[] { nearestTrap.Location, nearestTrap.GetBehindPoint(5f) }, 3f, "Trap Behind");
                            }
                            else
                            {
                                _trapMovement = new Movement(new[] { nearestTrap.Location, nearestTrap.GetFrontPoint(5f) }, 3f, "Trap Front");

                            }
                        }
                        
                    }

                    if (_trapMovement!=null && await _trapMovement.MoveTo())
                        return true;
                }
                

                //Check trappable target is low hp.. and
                if (TargetManager.CombatObject.CurrentHealthPercent < 0.50d)
                {
                    GarrisonBase.Debug("Trappable Target is at {0} health percent",
                        TargetManager.CombatObject.CurrentHealthPercent);
                    _trapMovement = null;
                    return true;
                }
            }

            return false;
        }

        private static List<C_WoWUnit> nearbyTraps
        {
            get
            {
                return
                    ObjectCacheManager.ObjectCollection.Values.OfType<C_WoWUnit>().Where(obj => obj.SubType == WoWObjectTypes.Trap && obj.Distance<6f).OrderBy(obj=>obj.Distance).ToList();
            }
        }
    }
}
