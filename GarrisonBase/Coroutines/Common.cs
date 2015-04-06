using System.Linq;
using System.Threading.Tasks;
using Bots.Grind;
using Bots.Quest;
using Herbfunk.GarrisonBase.Cache;
using Herbfunk.GarrisonBase.Coroutines.Behaviors;
using Herbfunk.GarrisonBase.Helpers;
using Styx;
using Styx.CommonBot;
using Styx.CommonBot.Coroutines;
using Styx.CommonBot.Frames;
using Styx.CommonBot.Routines;
using Styx.TreeSharp;
using Styx.WoWInternals.WoWObjects;

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

            bool inCombat = StyxWoW.Me.Combat;

            if (!inCombat && await RoutineManager.Current.PreCombatBuffBehavior.ExecuteCoroutine())
                return true;

            if (await EngageObject())
                return true;

            if ((inCombat && StyxWoW.Me.IsActuallyInCombat && !StyxWoW.Me.Mounted) && await RoutineManager.Current.CombatBehavior.ExecuteCoroutine())
                return true;

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

            if (await TaxiNodeUpdateCoroutine())
                return true;

            //await Coroutine.Yield();

            return false;
        }

        internal static bool ShouldUpdateTaxiNodes = false;
        private static Movement _taxiMovement;
        private static WoWUnit _taxiNpc;
        public static async Task<bool> TaxiNodeUpdateCoroutine()
        {
            if (!ShouldUpdateTaxiNodes || TaxiFlightHelper.TaxiNodes.Count > 0 || StyxWoW.Me.CurrentMap.ExpansionId != 5) return false;

            TreeRoot.StatusText = "Updating Taxi Node Info";

            if (LuaEvents.GossipFrameOpen)
            {
                if (QuestManager.GossipFrame.GossipOptionEntries.All(o => o.Type != GossipEntry.GossipEntryType.Taxi))
                {
                    //Could not find Taxi Option!
                    return false;
                }
                var gossipOptionTaxi = QuestManager.GossipFrame.GossipOptionEntries.FirstOrDefault(o => o.Type == GossipEntry.GossipEntryType.Taxi);

                QuestManager.GossipFrame.SelectGossipOption(gossipOptionTaxi.Index);
                await CommonCoroutines.SleepForRandomUiInteractionTime();
                return true;
            }

            if (LuaEvents.TaxiMapOpen)
            {
                if (!TaxiFrame.Instance.IsVisible)
                {
                    //Error.. LuaEvents says open but frame instance does not!
                    return false;
                }

                TaxiFlightHelper.PopulateTaxiNodes();
                TaxiFrame.Instance.Close();
                ShouldUpdateTaxiNodes = false;
                return true;
            }

            if (_taxiNpc == null)
                _taxiNpc = FlightPaths.NearestFlightMerchant;

            if (_taxiNpc.WithinInteractRange)
            {
                if (StyxWoW.Me.IsMoving) await CommonCoroutines.StopMoving();
                await CommonCoroutines.SleepForLagDuration();
                _taxiNpc.Interact();
                await CommonCoroutines.SleepForRandomUiInteractionTime();
                return true;
            }

            if (_taxiMovement == null || _taxiMovement.CurrentMovementQueue.Count==0)
                _taxiMovement = new Movement(_taxiNpc.Location, 5f - 0.25f);

            await _taxiMovement.MoveTo();

            return true;
        }

    }
}
