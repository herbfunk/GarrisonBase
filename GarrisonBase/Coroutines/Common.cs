using System.Threading.Tasks;
using Bots.Grind;
using Buddy.Coroutines;
using Herbfunk.GarrisonBase.Cache;
using Herbfunk.GarrisonBase.Character;
using Herbfunk.GarrisonBase.Coroutines.Behaviors;
using Herbfunk.GarrisonBase.Garrison;
using Herbfunk.GarrisonBase.Helpers;
using Herbfunk.GarrisonBase.TargetHandling;
using Styx;
using Styx.CommonBot.Coroutines;
using Styx.CommonBot.Frames;
using Styx.CommonBot.Profiles;
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

            if (await VendoringBehavior()) return true;

            if (await LootBehavior.ExecuteBehavior()) return true;

            


            return false;
        }


        public static async Task<bool> VendoringBehavior()
        {
            if (await DisenchantBehavior.Disenchanting()) return true;
            if (await VendorBehavior.Vendoring()) return true;
            if (await MailBehavior.Mailing()) return true;

            if (Player.Inventory.TotalFreeSlots < BaseSettings.CurrentSettings.MinimumBagSlotsFree || ForceBagCheck)
            {
                ForceBagCheck = false;

                if (VendorBehavior.ShouldVendor)
                {
                    GarrisonBase.Debug("Enabling Vendor behavior!");
                    VendorBehavior.IsVendoring = true;
                }

                if (MailBehavior.ShouldMail)
                {
                    GarrisonBase.Debug("Enabling Mailing behavior!");
                    MailBehavior.IsMailing = true;
                }

                if (DisenchantBehavior.ShouldDisenchant)
                {
                    GarrisonBase.Debug("Enabling Disenchanting behavior!");
                    DisenchantBehavior.IsDisenchanting = true;
                }

                if (VendorBehavior.IsVendoring || MailBehavior.ShouldMail || DisenchantBehavior.ShouldDisenchant)
                {
                    _originalTargetManagerLoot = TargetManager.ShouldLoot;
                    _originalTargetManagerKill = TargetManager.ShouldKill;
                    TargetManager.ShouldKill = false;
                    TargetManager.ShouldLoot = false;
                    _resetTargetManager = true;
                    return true;
                }
            }

            if (_resetTargetManager)
            {
                _resetTargetManager = false;
                TargetManager.ShouldKill = _originalTargetManagerKill;
                TargetManager.ShouldLoot = _originalTargetManagerLoot;
            }

            return false;
        }
        internal static bool ForceBagCheck
        {
            get { return _forceBagCheck; }
            set
            {
                _forceBagCheck = value;
                GarrisonBase.Debug("ForceBagCheck set to {0}", value);
            }
        }
        private static bool _forceBagCheck = false;
        private static bool _originalTargetManagerLoot, _originalTargetManagerKill, _resetTargetManager;

        internal static void ResetVendoring()
        {
            _forceBagCheck = false;
            _resetTargetManager = false;
            VendorBehavior.ResetVendoring();
            MailBehavior.ResetMailing();
            DisenchantBehavior.ResetDisenchanting();
        }

        internal static void ResetCommonBehaviors()
        {
            ResetVendoring();
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
