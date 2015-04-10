using System.Threading.Tasks;
using Bots.Grind;
using Herbfunk.GarrisonBase.Cache;
using Herbfunk.GarrisonBase.Coroutines.Behaviors;
using Herbfunk.GarrisonBase.Helpers;
using Styx;
using Styx.CommonBot.Coroutines;
using Styx.CommonBot.Frames;
using Styx.CommonBot.Routines;
using Styx.TreeSharp;

namespace Herbfunk.GarrisonBase.Coroutines
{
    public partial class Common
    {
        public static readonly BehaviorPrechecks PreChecks = new BehaviorPrechecks();

        private static Composite _lootBehavior, _deathBehavior, _vendorBehavior;
        internal static Composite LootBehavior
        {
            get { return _lootBehavior ?? (_lootBehavior = LevelBot.CreateLootBehavior()); }
        }
        internal static Composite VendorBehavior
        {
            get { return _vendorBehavior ?? (_vendorBehavior = LevelBot.CreateVendorBehavior()); }
        }
        internal static Composite DeathBehavior
        {
            get { return _deathBehavior ?? (_deathBehavior = LevelBot.CreateDeathBehavior()); }
        }

        public static async Task<bool> CheckCommonCoroutines()
        {
            if (ObjectCacheManager.ShouldUpdateObjectCollection)
                ObjectCacheManager.UpdateCache();

            if (StyxWoW.Me.IsDead && StyxWoW.Me.IsGhost && await DeathBehavior.ExecuteCoroutine())
                return true;

            if (await Combat()) return true;

            //GarrisonBase.Debug("Vendor Behavior..");
            bool vendorBehavior = await VendorBehavior.ExecuteCoroutine();
            if (vendorBehavior)
            {
                GarrisonBase.Debug("Vendor Behavior");
                return true;
            }

            if (await CheckLootFrame())
                return true;

            //GarrisonBase.Debug("Loot Behavior..");
            bool lootBehavior = await LootBehavior.ExecuteCoroutine();
            if (lootBehavior)
            {
                GarrisonBase.Debug("Loot Behavior");
                return true;
            }

            if (await LootObject())
                return true;


            return false;
        }

      

        internal static void CloseOpenFrames()
        {
            if (GossipHelper.IsOpen)
                GossipFrame.Instance.Close();
            else if (MerchantHelper.IsOpen)
                MerchantFrame.Instance.Close();
            else if (TaxiFlightHelper.IsOpen)
                TaxiFrame.Instance.Close();
        }

    }
}
