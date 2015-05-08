using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Herbfunk.GarrisonBase.Cache;
using Herbfunk.GarrisonBase.Cache.Enums;
using Herbfunk.GarrisonBase.Cache.Objects;
using Herbfunk.GarrisonBase.Helpers;
using Styx;
using Styx.CommonBot;
using Styx.CommonBot.Coroutines;
using Styx.Pathing;

namespace Herbfunk.GarrisonBase.Coroutines
{
    public static class MailBehavior
    {
        internal static bool IsMailing { get; set; }
        private static bool _mailingSetRecipient = false;
        private static C_WoWObject _mailobject;
        private static Movement _mailingMovement;
        internal static bool ShouldMail
        {
            get { return BaseSettings.CurrentSettings.MailAutoSend && GetMailItems.Count > 0; }
        }

        internal static async Task<bool> Mailing()
        {
            if (IsMailing)
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
                        _mailingMovement = new Movement(_mailobject, _mailobject.InteractRange - 0.25f, "Mailbox Movement");
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

            return false;
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

        private static async Task<bool> MailboxInteraction(C_WoWObject mailbox, Dictionary<string, List<C_WoWItem>> items)
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
      
        internal static void ResetMailing()
        {
            _mailingMovement = null;
            IsMailing = false;
            _mailobject = null;
            _mailingSetRecipient = false;
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

        private static List<C_WoWObject> GetMailBoxes
        {
            get { return ObjectCacheManager.GetWoWObjects(WoWObjectTypes.Mailbox).OrderBy(m => m.Distance).ToList(); }
        }
    }
}
