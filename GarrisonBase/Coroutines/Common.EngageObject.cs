using System.Threading.Tasks;
using Herbfunk.GarrisonBase.Cache;
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
        private static Movement _combatMovement;
        public static async Task<bool> EngageObject()
        {
            if (ObjectCacheManager.ShouldUpdateObjectCollection)
            {
                ObjectCacheManager.UpdateCache();
            }

            if (!ObjectCacheManager.ShouldKill)
            {
                _combatMovement = null;
                return false;
            }

            if (ObjectCacheManager.CombatObject == null)
            {
                ObjectCacheManager.UpdateCombatTarget();
                if (ObjectCacheManager.CombatObject == null)
                {
                    _combatMovement = null;
                    return false;
                }
            }

            if (!ObjectCacheManager.CombatObject.ValidForCombat)
            {
                GarrisonBase.Debug("EngageObject no longer valid for combat!");
                ObjectCacheManager.UpdateCombatTarget();
                _combatMovement = null;
                return false;
            }


            if (ObjectCacheManager.CombatObject.Distance <= Targeting.PullDistance)
            {
                if (!ObjectCacheManager.CombatObject.IsValid) return true;
                if (StyxWoW.Me.CurrentTarget == null || StyxWoW.Me.CurrentTarget.Guid != ObjectCacheManager.CombatObject.Guid)
                {
                    GarrisonBase.Debug("Behavior Combat changing POI to unit {0}", ObjectCacheManager.CombatObject.Name);
                    BotPoi.Current = new BotPoi(ObjectCacheManager.CombatObject.RefWoWUnit, PoiType.Kill);

                    if (RoutineManager.Current.PullBuffBehavior != null)
                        await RoutineManager.Current.PullBuffBehavior.ExecuteCoroutine();

                    if (RoutineManager.Current.PullBehavior != null)
                        await RoutineManager.Current.PullBehavior.ExecuteCoroutine();

                    return false;
                }

                await RoutineManager.Current.CombatBehavior.ExecuteCoroutine();
                return true;
            }

            if (_combatMovement == null)
            {
                _combatMovement = new Movement(ObjectCacheManager.CombatObject.Location,
                    RoutineManager.Current.PullDistance.HasValue ? (float)RoutineManager.Current.PullDistance.Value :
                    ObjectCacheManager.CombatObject.InteractRange);
            }
            else if (_combatMovement.CurrentMovementQueue.Count == 0)
            {
                _combatMovement = new Movement(ObjectCacheManager.CombatObject.Location,
                    ObjectCacheManager.CombatObject.InteractRange -= 0.25f);
            }

            var result = await _combatMovement.MoveTo_Result();
            if (result == MoveResult.Failed)
            {
                GarrisonBase.Debug("Behavior Combat Movement FAILED for {0}", ObjectCacheManager.LootableObject.Name);
                ObjectCacheManager.LootableObject.IgnoredTimer = WaitTimer.TenSeconds;
                ObjectCacheManager.LootableObject = null;
                _lootMovement = null;
                return false;
            }

            return true;
        }
    }
}
