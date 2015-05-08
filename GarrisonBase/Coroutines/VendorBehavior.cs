using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bots.Quest;
using Buddy.Coroutines;
using Herbfunk.GarrisonBase.Cache;
using Herbfunk.GarrisonBase.Cache.Enums;
using Herbfunk.GarrisonBase.Cache.Objects;
using Herbfunk.GarrisonBase.Character;
using Herbfunk.GarrisonBase.Helpers;
using Styx;
using Styx.CommonBot;
using Styx.CommonBot.Coroutines;
using Styx.CommonBot.Frames;
using Styx.Pathing;

namespace Herbfunk.GarrisonBase.Coroutines
{
    public static class VendorBehavior
    {

        internal static bool IsVendoring { get; set; }
        private static C_WoWObject _vendorobject;
        private static Movement _vendorMovement;

        internal static async Task<bool> Vendoring()
        {
            if (IsVendoring)
            {
                var items = GetVendorItems;
                if (items.Count == 0)
                {
                    if (await Common.CloseFrames()) return true;
                    ResetVendoring();
                    return true;
                }

                if (_vendorobject == null)
                {
                    var vendors = GetVendors;
                    if (vendors.Count > 0)
                    {
                        _vendorobject = vendors[0];
                        GarrisonBase.Debug("Vendor Behavior Using {0} as Vendor Npc!", _vendorobject.Name);
                    }
                }


                if (_vendorMovement == null || _vendorMovement.CurrentMovementQueue.Count == 0)
                {
                    //Check for any vendors in cache..
                    //  -If none than goto safety inside garrison.
                    if (_vendorobject == null)
                    {
                        var vendorWoWPoint = MovementCache.SellRepairNpcLocation;
                        if (vendorWoWPoint == WoWPoint.Zero)
                        {
                            vendorWoWPoint = StyxWoW.Me.IsAlliance
                                ? MovementCache.AllianceSellRepairNpc
                                : MovementCache.HordeSellRepairNpc;
                        }

                        _vendorMovement = new Movement(new[] { vendorWoWPoint }, 20f, "Garrison Vendor");
                    }
                    else
                    {
                        _vendorMovement = new Movement(_vendorobject, _vendorobject.InteractRange - 0.25f, "Vendor Movement");
                    }
                }

                if (_vendorobject != null)
                {
                    if (_vendorobject.WithinInteractRange)
                    {
                        //Do Vendoring..
                        bool vendoring = await VendorInteraction(_vendorobject, items);
                        if (!vendoring)
                        {
                            ResetVendoring();
                            return true;
                        }

                        return true;
                    }
                }

                if (_vendorMovement != null)
                {
                    bool movement = await VendorMovement();
                    if (!movement)
                    {
                        //Failed to move?
                        GarrisonBase.Debug("Failed to move to vendor location!");
                        return false;
                    }
                }

                return true;
            }

            return false;
        }


        private static async Task<bool> VendorMovement()
        {
            TreeRoot.StatusText = "Vendor Movement Behavior";
            var result = await _vendorMovement.MoveTo_Result();

            if (result == MoveResult.Failed)
            {
                GarrisonBase.Debug("Behavior Vendor Movement FAILED!");
                _vendorMovement = null;
                return false;
            }

            return true;
        }

        private static async Task<bool> VendorInteraction(C_WoWObject vendor, List<C_WoWItem> items)
        {
            TreeRoot.StatusText = "Vendor Interaction Behavior";

            if (GossipHelper.IsOpen)
            {
                await Coroutine.Yield();

                if (GossipHelper.GossipOptions.All(o => o.Type != GossipEntry.GossipEntryType.Vendor))
                {
                    //Could not find Vendor Option!
                    GarrisonBase.Debug("Vendor Gossip Has No Option for Vendoring!");
                    return false;
                }
                var gossipEntryVendor = GossipHelper.GossipOptions.FirstOrDefault(o => o.Type == GossipEntry.GossipEntryType.Vendor);

                QuestManager.GossipFrame.SelectGossipOption(gossipEntryVendor.Index);
                await CommonCoroutines.SleepForRandomUiInteractionTime();
                return true;
            }

            if (MerchantHelper.IsOpen)
            {
                if (items.Count > 0)
                {
                    foreach (var item in items)
                    {
                        GarrisonBase.Debug("Vendoring Item {0} ({1}) Quality {2}", item.Name, item.Entry, item.Quality);
                        MerchantFrame.Instance.SellItem(item.ref_WoWItem);
                        await CommonCoroutines.SleepForRandomUiInteractionTime();
                        await Coroutine.Yield();
                        if (!MerchantHelper.IsOpen) break;
                    }

                    return true;
                }

                //No vendor items found..
                return false;
            }

            if (StyxWoW.Me.IsMoving) await CommonCoroutines.StopMoving();
            await CommonCoroutines.SleepForLagDuration();
            vendor.Interact();
            await CommonCoroutines.SleepForRandomUiInteractionTime();
            return true;
        }


        internal static bool ShouldVendor
        {
            get { return BaseSettings.CurrentSettings.BehaviorRepairSell && GetVendorItems.Count > 0; }
        }

        internal static void ResetVendoring()
        {
            _vendorobject = null;
            IsVendoring = false;
            _vendorMovement = null;
        }


        private static List<C_WoWItem> GetVendorItems
        {
            get { return Player.Inventory.GetBagVendorItems(); }
        }

        private static List<C_WoWUnit> GetVendors
        {
            get
            {
                return ObjectCacheManager.GetWoWUnits(WoWObjectTypes.Vendor).OrderBy(unit => unit.Distance).ToList();
            }
        }


    }
}
