using System.Threading.Tasks;
using Bots.Grind;
using Buddy.Coroutines;
using Herbfunk.GarrisonBase.Cache;
using Herbfunk.GarrisonBase.Coroutines.Behaviors;
using Herbfunk.GarrisonBase.Helpers;
using Styx;
using Styx.CommonBot.Coroutines;
using Styx.CommonBot.Frames;
using Styx.TreeSharp;

namespace Herbfunk.GarrisonBase.Coroutines
{
    public class Common
    {
        public static readonly BehaviorPrechecks PreChecks = new BehaviorPrechecks();

        private static Composite _deathBehavior;

        internal static Composite DeathBehavior
        {
            get { return _deathBehavior ?? (_deathBehavior = LevelBot.CreateDeathBehavior()); }
        }
        public static async Task<bool> CheckCommonCoroutines()
        {
            if (ObjectCacheManager.ShouldUpdateObjectCollection)
                ObjectCacheManager.UpdateCache();
            
            if (StyxWoW.Me.IsDead || StyxWoW.Me.IsGhost)
            {
                await DeathBehavior.ExecuteCoroutine();
                return true;
            }
                

            if (await CombatBehavior.ExecuteBehavior()) return true;

            if (await VendorBehavior.ExecuteBehavior()) return true;

            if (await LootBehavior.ExecuteBehavior()) return true;

            


            return false;
        }

        internal static void ResetCommonBehaviors()
        {
            VendorBehavior.Reset();
            LootBehavior.ResetLoot();
            CombatBehavior.ResetCombat();
        }

        internal static async Task<bool> CloseFrames()
        {
            if (GossipHelper.IsOpen)
            {
                GossipFrame.Instance.Close();
                await CommonCoroutines.SleepForRandomUiInteractionTime();
                await Coroutine.Yield();
                return true;
            }

            if (MerchantHelper.IsOpen)
            {
                MerchantFrame.Instance.Close();
                await CommonCoroutines.SleepForRandomUiInteractionTime();
                await Coroutine.Yield();
                return true;
            }

            if (TaxiFlightHelper.IsOpen)
            {
                TaxiFrame.Instance.Close();
                await CommonCoroutines.SleepForRandomUiInteractionTime();
                await Coroutine.Yield();
                return true;
            }

            if (MailHelper.IsOpen)
            {
                MailFrame.Instance.Close();
                await CommonCoroutines.SleepForRandomUiInteractionTime();
                await Coroutine.Yield();
                return true;
            }

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
