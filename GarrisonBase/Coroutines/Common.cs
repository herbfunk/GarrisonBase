using System.Threading.Tasks;
using Bots.Grind;
using Herbfunk.GarrisonBase.Cache;
using Herbfunk.GarrisonBase.Coroutines.Behaviors;
using Styx;
using Styx.CommonBot.Coroutines;
using Styx.CommonBot.Routines;
using Styx.TreeSharp;

namespace Herbfunk.GarrisonBase.Coroutines
{
    public partial class Common
    {
        public static readonly BehaviorPrechecks PreChecks = new BehaviorPrechecks();

        private static Composite _lootBehavior;
        private static Composite _vendorBehavior;
        internal static Composite LootBehavior
        {
            get { return _lootBehavior ?? (_lootBehavior = LevelBot.CreateLootBehavior()); }
        }
        internal static Composite VendorBehavior
        {
            get { return _vendorBehavior ?? (_vendorBehavior = LevelBot.CreateVendorBehavior()); }
        }

        public static async Task<bool> CheckCommonCoroutines()
        {
            if (ObjectCacheManager.ShouldUpdateObjectCollection)
                ObjectCacheManager.UpdateCache();

            if (StyxWoW.Me.IsDead && StyxWoW.Me.IsGhost && await RoutineManager.Current.DeathBehavior.ExecuteCoroutine())
                return true;

            bool inCombat = StyxWoW.Me.Combat;
            
            if (!inCombat && await RoutineManager.Current.PreCombatBuffBehavior.ExecuteCoroutine()) 
                return true;

            //if ((!inCombat || ObjectCacheManager.CombatObject!=null) && await EngageObject()) 
            //    return true;

            if (await EngageObject())
                return true;

            if ((inCombat && StyxWoW.Me.IsActuallyInCombat && !StyxWoW.Me.Mounted) && await RoutineManager.Current.CombatBehavior.ExecuteCoroutine())
                return true;

            //if (await VendorBehavior.ExecuteCoroutine())
            //    return true;
           
            if (await CheckLootFrame())
                return true;

            if (await LootBehavior.ExecuteCoroutine())
                return true;

            if (await LootObject())
                return true;

            return false;
        }

        
    }
}
