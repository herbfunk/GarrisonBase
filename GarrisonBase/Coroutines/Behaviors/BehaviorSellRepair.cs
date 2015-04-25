using System.Linq;
using System.Threading.Tasks;
using Bots.Quest;
using Herbfunk.GarrisonBase.Cache;
using Herbfunk.GarrisonBase.Cache.Enums;
using Herbfunk.GarrisonBase.Cache.Objects;
using Herbfunk.GarrisonBase.Garrison;
using Herbfunk.GarrisonBase.Helpers;
using Herbfunk.GarrisonBase.TargetHandling;
using Styx;
using Styx.CommonBot.Coroutines;
using Styx.CommonBot.Frames;

namespace Herbfunk.GarrisonBase.Coroutines.Behaviors
{
    public class BehaviorSellRepair : Behavior
    {
        public override BehaviorType Type { get { return BehaviorType.SellRepair; } }

        public BehaviorSellRepair()
            : base()
        {
            Criteria += () => 
                BaseSettings.CurrentSettings.BehaviorRepairSell && 
                Character.Player.Inventory.VendorItems.Count > 0;

            RunCondition += () => 
                BaseSettings.CurrentSettings.BehaviorRepairSell &&
                Character.Player.Inventory.VendorItems.Count > 0;
        }


        public override void Initalize()
        {
            TargetManager.ShouldKill = false;
            TargetManager.ShouldLoot = false;

            Common.CloseOpenFrames();

            //MovementCache.SellRepairNpcLocation, GarrisonManager.SellRepairNpcId

            var vendors = ObjectCacheManager.GetWoWUnits(WoWObjectTypes.Vendor).OrderBy(unit => unit.Distance).ToList();
            if (vendors.Count > 0)
            {
                var vendor = vendors[0];
                MovementPoints.Add(vendor.Location);
                InteractionEntryId = (int)vendor.Entry;
                GarrisonBase.Debug("Using nearest vendor {0} ({1})", vendor.Name, vendor.Entry);
            }
            else
            {
                MovementPoints.Add(MovementCache.SellRepairNpcLocation);
                InteractionEntryId = GarrisonManager.SellRepairNpcId;
                GarrisonBase.Debug("Using default vendor");
            }

            _npcMovement = null;
            base.Initalize();
        }

        private Movement _npcMovement;
        public override async Task<bool> BehaviorRoutine()
        {
            if (await base.BehaviorRoutine()) return true;
            if (IsDone) return false;


            if (InteractionObject == null)
                if (await StartMovement.MoveTo()) return true;




            if (GossipHelper.IsOpen)
            {
                if (GossipHelper.GossipOptions.All(o => o.Type != GossipEntry.GossipEntryType.Vendor))
                {
                    //Could not find Vendor Option!
                    return false;
                }
                var gossipEntryVendor = GossipHelper.GossipOptions.FirstOrDefault(o => o.Type == GossipEntry.GossipEntryType.Vendor);

                QuestManager.GossipFrame.SelectGossipOption(gossipEntryVendor.Index);
                await CommonCoroutines.SleepForRandomUiInteractionTime();
                return true;
            }

            if (MerchantHelper.IsOpen)
            {
                foreach (var item in Character.Player.Inventory.VendorItems)
                {
                    GarrisonBase.Debug("Vendoring Item {0} ({1}) Quality {2}", item.Name, item.Entry, item.Quality);
                    MerchantFrame.Instance.SellItem(item.ref_WoWItem);
                    await CommonCoroutines.SleepForRandomUiInteractionTime();
                    // await CommonCoroutines.SleepForLagDuration();
                    // await Coroutine.Sleep(StyxWoW.Random.Next(256, 712));
                }

                return true;
            }

            if (InteractionObject != null)
            {
                if (InteractionObject.WithinInteractRange)
                {
                    if (StyxWoW.Me.IsMoving) await CommonCoroutines.StopMoving();
                    await CommonCoroutines.SleepForLagDuration();
                    InteractionObject.Interact();
                    await CommonCoroutines.SleepForRandomUiInteractionTime();
                    return true;
                }

                if (_npcMovement == null)
                    _npcMovement = new Movement(InteractionObject.Location, InteractionObject.InteractRange - 0.25f, name: "Vendor");

                await _npcMovement.MoveTo(false);
                return true;
            }

            

            return false;
        }
    }
}