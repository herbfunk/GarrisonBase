using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bots.Quest;
using Buddy.Coroutines;
using Herbfunk.GarrisonBase.Cache;
using Herbfunk.GarrisonBase.Cache.Enums;
using Herbfunk.GarrisonBase.Cache.Objects;
using Herbfunk.GarrisonBase.Character;
using Herbfunk.GarrisonBase.Garrison;
using Herbfunk.GarrisonBase.Garrison.Enums;
using Herbfunk.GarrisonBase.Helpers;
using Herbfunk.GarrisonBase.TargetHandling;
using Styx;
using Styx.Common.Helpers;
using Styx.CommonBot;
using Styx.CommonBot.Coroutines;
using Styx.CommonBot.Frames;
using Styx.Pathing;

namespace Herbfunk.GarrisonBase.Coroutines
{
    public static class VendorBehavior
    {
        internal static bool ForceBagCheck
        {
            get { return _forceBagCheck; }
            set
            {
                _forceBagCheck = value;
                GarrisonBase.Debug("VendorBehavior ForceBagCheck set to {0}", value);
            }
        }
        private static bool _forceBagCheck = false;

        private static bool _isvendoring, _ismailing, _isdisenchanting, _mailingSetRecipient = false;
        private static C_WoWObject _vendorobject, _mailobject;
        private static C_WoWGameObject _disenchantobject;
        private static Movement _vendorMovement, _mailingMovement, _disenchantMovement;
        private static bool _originalTargetManagerLoot, _originalTargetManagerKill, _resetTargetManager;

        public static async Task<bool> ExecuteBehavior()
        {
            #region Vendoring

            if (_isvendoring)
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
                        _vendorMovement = new Movement(_vendorobject, _vendorobject.InteractRange, "Vendor Movement");
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
            #endregion

            #region Send Mail
            if (_ismailing)
            {
                var items = GetMailItems;
                if (items.Count == 0)
                {
                    if (await Common.CloseFrames()) return true;
                    ResetMailing();
                    return true;
                }

                if (_mailobject == null)
                {
                    var mailboxes = GetMailBoxes;
                    if (mailboxes.Count > 0)
                    {
                        _mailobject = mailboxes[0];
                        GarrisonBase.Debug("Mail Behavior found mailbox object at {0}", _mailobject.Location);
                    }
                }

                if (_mailingMovement == null || _mailingMovement.CurrentMovementQueue.Count == 0)
                {
                    if (_mailobject == null)
                    {
                        var mailboxWoWPoint = MovementCache.GarrisonEntrance;
                        if (mailboxWoWPoint == WoWPoint.Zero)
                        {
                            mailboxWoWPoint = StyxWoW.Me.IsAlliance
                                ? MovementCache.AllianceGarrisonEntrance
                                : MovementCache.HordeGarrisonEntrance;
                        }

                        _mailingMovement = new Movement(new[] { mailboxWoWPoint }, 20f, "Garrison Entrance");
                    }
                    else
                    {
                        _mailingMovement = new Movement(_mailobject, _mailobject.InteractRange, "Mailbox Movement");
                    }
                }


                if (_mailobject != null)
                {
                    if (_mailobject.WithinInteractRange)
                    {
                        bool mailing = await MailboxInteraction(_mailobject, items);
                        if (!mailing)
                        {
                            ResetMailing();
                            return true;
                        }

                        return true;
                    }
                }

                if (_mailingMovement != null)
                {
                    bool movement = await MailMovement();
                    if (!movement)
                    {
                        //Failed to move?
                        GarrisonBase.Debug("Failed to move to mailbox location!");
                        return false;
                    }
                }

                return true;

            }
            #endregion

            #region Disenchanting

            if (_isdisenchanting)
            {
                var items = GetDisenchantItems;
                if (items.Count == 0)
                {
                    if (await Common.CloseFrames()) return true;
                    ResetDisenchanting();
                    return true;
                }

                if (_disenchantobject == null)
                {
                    var disenchantObjects = DisenchantingObjects;
                    if (disenchantObjects.Count > 0)
                    {
                        _disenchantobject = disenchantObjects[0];
                        GarrisonBase.Debug("Disenchant Behavior found object at {0}", _disenchantobject.Location);
                    }
                }

                if (_disenchantMovement == null || _disenchantMovement.CurrentMovementQueue.Count == 0)
                {
                    if (_disenchantobject == null)
                    {
                        var mailboxWoWPoint = GarrisonManager.Buildings[BuildingType.EnchantersStudy].EntranceMovementPoint;
                        _disenchantMovement = new Movement(new[] { mailboxWoWPoint }, 20f, "Enchanters Study Entrance");
                    }
                    else
                    {
                        _disenchantMovement = new Movement(_disenchantobject, _disenchantobject.InteractRange, "Disenchant Object Movement");
                    }
                }


                if (_disenchantobject != null)
                {
                    if (_disenchantobject.WithinInteractRange)
                    {
                        bool disenchanting = await DisenchantInteraction(_disenchantobject, items);
                        if (!disenchanting)
                        {
                            ResetDisenchanting();
                            return true;
                        }

                        return true;
                    }
                }

                if (_disenchantMovement != null)
                {
                    bool movement = await DisenchantMovement();
                    if (!movement)
                    {
                        //Failed to move?
                        GarrisonBase.Debug("Failed to move to disenchant location!");
                        return false;
                    }
                }

                return true;
            }
            #endregion

            if (Player.Inventory.TotalFreeSlots < BaseSettings.CurrentSettings.MinimumBagSlotsFree || ForceBagCheck)
            {
                ForceBagCheck = false;

                if (BaseSettings.CurrentSettings.BehaviorRepairSell && GetVendorItems.Count > 0)
                {
                    GarrisonBase.Debug("Enabling Vendor behavior!");
                    _isvendoring = true;
                }

                if (BaseSettings.CurrentSettings.MailAutoSend && GetMailItems.Count > 0)
                {
                    GarrisonBase.Debug("Enabling Mailing behavior!");
                    _ismailing = true;
                }

                if (BaseSettings.CurrentSettings.BehaviorDisenchanting && GarrisonManager.Initalized &&
                    GarrisonManager.HasDisenchant && Player.InsideGarrison && GetDisenchantItems.Count > 0)
                {
                    GarrisonBase.Debug("Enabling Disenchanting behavior!");
                    _isdisenchanting = true;
                }

                if (_isvendoring || _ismailing || _isdisenchanting)
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
        private static async Task<bool> MailMovement()
        {
            TreeRoot.StatusText = "Mail Movement Behavior";
            var result = await _mailingMovement.MoveTo_Result();

            if (result == MoveResult.Failed)
            {
                GarrisonBase.Debug("Behavior Mailbox Movement FAILED!");
                _mailingMovement = null;
                return false;
            }

            return true;
        }
        private static async Task<bool> DisenchantMovement()
        {
            TreeRoot.StatusText = "Disenchanting Movement Behavior";
            var result = await _disenchantMovement.MoveTo_Result();

            if (result == MoveResult.Failed)
            {
                GarrisonBase.Debug("Behavior Disenchant Movement FAILED!");
                _disenchantMovement = null;
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
        private static async Task<bool> MailboxInteraction(C_WoWObject mailbox,Dictionary<string, List<C_WoWItem>> items)
        {
            if (MailHelper.IsOpen)
            {
                //Click Send Mail Tab
                if (!LuaCommands.IsSendMailFrameVisible())
                {
                    LuaCommands.ClickSendMailTab();
                    await CommonCoroutines.SleepForRandomUiInteractionTime();
                    return true;
                }

                foreach (var keypair in items)
                {
                    GarrisonBase.Log("Found {0} items to mail", keypair.Value.Count);
                    bool success = await SendMail(keypair.Key, keypair.Value);
                    if (!success)
                    {
                        GarrisonBase.Debug("Sending Mail Failed!");
                        return false;
                    }
                    await Coroutine.Yield();
                    return true;
                }
            }


            if (StyxWoW.Me.IsMoving) await CommonCoroutines.StopMoving();
            await CommonCoroutines.SleepForLagDuration();
            mailbox.Interact();
            await CommonCoroutines.SleepForRandomUiInteractionTime();
            return true;
        }
        private static async Task<bool> DisenchantInteraction(C_WoWGameObject disenchantobject, List<C_WoWItem> items )
        {
            if (disenchantobject.GetCursor == WoWCursorType.InteractCursor)
            {
                GarrisonBase.Debug("Disenchant Interaction failed -- Cursor was not InteractCursor!");
                return false;
            }

            if (Player.CurrentPendingCursorSpellId == 160201)
            {
                //Item Interaction!
                GarrisonBase.Log("Disenchant Cursor!");

                if (items.Count > 0)
                {
                    var item = items[0];
                    GarrisonBase.Debug("Disenchanting Item {0} ({1}) Quality {2}", item.Name, item.Entry, item.Quality);
                    bool bagChanged = await CommonCoroutines.WaitForLuaEvent(
                        "BAG_UPDATE",
                        6200,
                        null,
                        item.Use);

                    PlayerInventory.ItemDisenchantingBlacklistedGuids.Add(item.Guid);

                    //Force update if bag didn't change.. (so we ignore this item now)
                    if (!bagChanged)
                        Player.Inventory.UpdateBagItems();

                    return true;
                }
                return false;
            }

            bool cursorChanged = await CommonCoroutines.WaitForLuaEvent(
                "CURRENT_SPELL_CAST_CHANGED",
                StyxWoW.Random.Next(555, 2002),
                null,
                disenchantobject.Interact);


            await CommonCoroutines.SleepForRandomUiInteractionTime();
            await Coroutine.Yield();
            await Coroutine.Sleep(StyxWoW.Random.Next(1222, 2222));

            if (!cursorChanged)
            {
                if (LuaCommands.IsStaticPopupVisible())
                {
                    LuaCommands.ClickStaticPopupButton(1);
                    await CommonCoroutines.SleepForRandomUiInteractionTime();
                    return true;
                }
            }


            return true;
        }
        
        private static async Task<bool> SendMail(string recipient, List<C_WoWItem> items)
        {

            int count = 0;

            foreach (var cWoWItem in items)
            {
                if (!MailHelper.IsOpen)
                {
                    GarrisonBase.Debug("Send Mail Failed due to mailbox not open!");
                    return false;
                }

                if (count > 11) break;

                if (BaseSettings.CurrentSettings.MailSendItems.Any(i => i.EntryId == cWoWItem.Entry && i.OnCount > 0))
                {
                    var mailItemInfo = BaseSettings.CurrentSettings.MailSendItems.FirstOrDefault(i => i.EntryId == cWoWItem.Entry);

                    if (mailItemInfo != null)
                    {
                        int excessCount = (int)cWoWItem.StackCount - mailItemInfo.OnCount;

                        if (excessCount > 0)
                        {
                            GarrisonBase.Log("Send Mail Spliting Item {0} to send count {1}", cWoWItem.Name, excessCount);
                            int freeBagIndex, freeBagSlot;
                            bool foundFreeSpot = Character.Player.Inventory.FindFreeBagSlot(out freeBagIndex, out freeBagSlot);

                            if (foundFreeSpot)
                            {
                                GarrisonBase.Log("Send Mail Split Item Moving to Bag Index {0} Slot {1}", freeBagIndex, freeBagSlot);

                                bool success = await SplitItem(cWoWItem, excessCount, freeBagIndex, freeBagSlot);

                                if (success)
                                {
                                    GarrisonBase.Log("Attaching item {0} to mail", cWoWItem.Name);
                                    LuaCommands.UseContainerItem(freeBagIndex, freeBagSlot + 1);
                                    await CommonCoroutines.SleepForRandomUiInteractionTime();
                                    await Coroutine.Sleep(StyxWoW.Random.Next(1250, 2223));
                                    count++;
                                }

                                LuaCommands.ClearCursor();
                                continue;
                            }
                        }
                    }
                }

                GarrisonBase.Log("Attaching item {0} to mail", cWoWItem.Name);
                LuaCommands.UseContainerItem(cWoWItem.BagIndex + 1, cWoWItem.BagSlot + 1);
                await CommonCoroutines.SleepForRandomUiInteractionTime();
                await Coroutine.Sleep(StyxWoW.Random.Next(1250, 2223));
                count++;
            }

            if (!_mailingSetRecipient)
            {
                if (!MailHelper.IsOpen)
                {
                    GarrisonBase.Debug("Send Mail Failed due to mailbox not open!");
                    return false;
                }
                LuaCommands.SetSendMailRecipient(recipient);
                _mailingSetRecipient = true;
                await CommonCoroutines.SleepForRandomUiInteractionTime();
            }

            if (!MailHelper.IsOpen)
            {
                GarrisonBase.Debug("Send Mail Failed due to mailbox not open!");
                _mailingSetRecipient = false;
                return false;
            }
            LuaCommands.ClickSendMailButton();
            await CommonCoroutines.SleepForRandomUiInteractionTime();
            await Coroutine.Sleep(StyxWoW.Random.Next(1250, 2223));
            _mailingSetRecipient = false;
            return true;
        }
        private static async Task<bool> SplitItem(C_WoWItem item, int Count, int BagIndex, int BagSlot)
        {
            if (!LuaCommands.CursorHasItem())
            {
                //Split Item..
                bool pickup =
                    await CommonCoroutines.WaitForLuaEvent("CURSOR_UPDATE",
                        2500,
                        null,
                        () => LuaCommands.SplitContainerItem(item.BagIndex + 1, item.BagSlot + 1, Count));

                await CommonCoroutines.SleepForRandomUiInteractionTime();

                if (pickup)
                {
                    //Select Empty Bag Slot
                    await CommonCoroutines.WaitForLuaEvent(
                        "CURSOR_UPDATE",
                        2500,
                        null,
                        () => LuaCommands.PickupContainerItem(BagIndex, BagSlot + 1));

                    await CommonCoroutines.SleepForRandomUiInteractionTime();
                    return true;
                }
            }

            return false;
        }

        internal static void Reset()
        {
            _forceBagCheck = false;
            _resetTargetManager = false;
            ResetVendoring();
            ResetMailing();
            ResetDisenchanting();
        }

        private static void ResetVendoring()
        {
            _vendorobject = null;
            _isvendoring = false;
            _vendorMovement = null;
        }
        private static void ResetMailing()
        {
            _mailingMovement = null;
            _ismailing = false;
            _mailobject = null;
            _mailingSetRecipient = false;
        }
        private static void ResetDisenchanting()
        {
            _disenchantMovement = null;
            _isdisenchanting = false;
            _disenchantobject = null;
        }

        private static List<C_WoWItem> GetVendorItems
        {
            get { return Player.Inventory.GetBagVendorItems(); }
        }
        private static Dictionary<string, List<C_WoWItem>> GetMailItems
        {
            get
            {
                var retDictionary = new Dictionary<string, List<C_WoWItem>>();

                if (BaseSettings.CurrentSettings.MailSendEnchanting)
                {
                    var items = Character.Player.Inventory.GetBagItemsEnchanting();
                    if (items.Count > 0)
                    {
                        retDictionary.Add(BaseSettings.CurrentSettings.MailSendEnchantingRecipient, items);
                    }
                }
                if (BaseSettings.CurrentSettings.MailSendHerbs)
                {
                    var items = Character.Player.Inventory.GetBagItemsHerbs();
                    if (items.Count > 0)
                    {
                        if (retDictionary.ContainsKey(BaseSettings.CurrentSettings.MailSendHerbsRecipient))
                            retDictionary[BaseSettings.CurrentSettings.MailSendHerbsRecipient].AddRange(items);
                        else
                            retDictionary.Add(BaseSettings.CurrentSettings.MailSendHerbsRecipient, items);
                    }
                }
                if (BaseSettings.CurrentSettings.MailSendOre)
                {
                    var items = Character.Player.Inventory.GetBagItemsOre();
                    if (items.Count > 0)
                    {
                        if (retDictionary.ContainsKey(BaseSettings.CurrentSettings.MailSendOreRecipient))
                            retDictionary[BaseSettings.CurrentSettings.MailSendOreRecipient].AddRange(items);
                        else
                            retDictionary.Add(BaseSettings.CurrentSettings.MailSendOreRecipient, items);
                    }
                }
                if (BaseSettings.CurrentSettings.MailSendUncommon)
                {
                    var items = Character.Player.Inventory.GetBagItemsBOEByQuality(WoWItemQuality.Uncommon);
                    if (items.Count > 0)
                    {
                        if (retDictionary.ContainsKey(BaseSettings.CurrentSettings.MailSendUncommonRecipient))
                            retDictionary[BaseSettings.CurrentSettings.MailSendUncommonRecipient].AddRange(items);
                        else
                            retDictionary.Add(BaseSettings.CurrentSettings.MailSendUncommonRecipient, items);
                    }
                }
                if (BaseSettings.CurrentSettings.MailSendRare)
                {
                    var items = Character.Player.Inventory.GetBagItemsBOEByQuality(WoWItemQuality.Rare);
                    if (items.Count > 0)
                    {
                        if (retDictionary.ContainsKey(BaseSettings.CurrentSettings.MailSendRareRecipient))
                            retDictionary[BaseSettings.CurrentSettings.MailSendRareRecipient].AddRange(items);
                        else
                            retDictionary.Add(BaseSettings.CurrentSettings.MailSendRareRecipient, items);
                    }
                }
                if (BaseSettings.CurrentSettings.MailSendEpic)
                {
                    var items = Character.Player.Inventory.GetBagItemsBOEByQuality(WoWItemQuality.Epic);
                    if (items.Count > 0)
                    {
                        if (retDictionary.ContainsKey(BaseSettings.CurrentSettings.MailSendEpicRecipient))
                            retDictionary[BaseSettings.CurrentSettings.MailSendEpicRecipient].AddRange(items);
                        else
                            retDictionary.Add(BaseSettings.CurrentSettings.MailSendEpicRecipient, items);
                    }
                }
                if (BaseSettings.CurrentSettings.MailSendItems.Count > 0)
                {
                    var items = Character.Player.Inventory.GetMailItems();

                    foreach (var value in items)
                    {
                        if (retDictionary.ContainsKey(value.Value))
                            retDictionary[value.Value].Add(value.Key);
                        else
                        {
                            var newlist = new List<C_WoWItem>();
                            newlist.Add(value.Key);
                            retDictionary.Add(value.Value, newlist);
                        }
                    }
                }

                return retDictionary;
            }
        }
        private static List<C_WoWItem> GetDisenchantItems
        {
            get { return Player.Inventory.GetBagDisenchantingItems(); }
        }

        private static List<C_WoWUnit> GetVendors
        {
            get
            {
                return ObjectCacheManager.GetWoWUnits(WoWObjectTypes.Vendor).OrderBy(unit => unit.Distance).ToList();
            }
        }
        private static List<C_WoWObject> GetMailBoxes
        {
            get { return ObjectCacheManager.GetWoWObjects(WoWObjectTypes.Mailbox).OrderBy(m => m.Distance).ToList(); }
        }
        private static List<C_WoWGameObject> DisenchantingObjects
        {
            get { return ObjectCacheManager.GetWoWGameObjects(GarrisonManager.DisenchantingEntryId); }
        }

    }
}
