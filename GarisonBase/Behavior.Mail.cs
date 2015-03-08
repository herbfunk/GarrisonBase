using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Herbfunk.GarrisonBase.Cache;
using Styx;
using Styx.Common;
using Styx.CommonBot;
using Styx.CommonBot.Coroutines;
using Styx.CommonBot.Frames;

namespace Herbfunk.GarrisonBase
{
    public partial class Behaviors
    {
        public class BehaviorGetMail : Behavior
        {
            public override BehaviorType Type { get { return BehaviorType.Mail; } }
            public BehaviorGetMail()
            {

            }
            public override Func<bool> Criteria
            {
                get { return () => BaseSettings.CurrentSettings.MailAutoGet && LuaCommands.HasNewMail(); }
            }
            public override void Initalize()
            {
                MovementPoints.Add(MovementCache.GarrisonEntrance);
                base.Initalize();
            }
            public C_WoWObject Mailbox
            {
                get { return ObjectCacheManager.GetWoWObjects(WoWObjectTypes.Mailbox).FirstOrDefault(); }
            }
            private Movement _movement;
            public override async Task<bool> Movement()
            {
                if (LuaEvents.MailOpen)
                    return false;


                TreeRoot.StatusText = String.Format("Behavior {0} Movement", Type.ToString());
                if (await base.Movement())
                    return true;



                TreeRoot.StatusText = String.Format("Behavior {0} Movement2", Type.ToString());
                if (Mailbox == null)
                {
                    //No object found..
                    IsDone = true;
                    GarrisonBase.Err("Could not find mail box object!");
                    return false;
                }

                if (Mailbox.CentreDistance < 6)
                {
                    Mailbox.Interact();
                    await CommonCoroutines.SleepForRandomUiInteractionTime();
                    await Coroutine.Sleep(StyxWoW.Random.Next(1250, 2555));
                    return true;
                }

                if (_movement == null)
                    _movement = new Movement(Mailbox.Location, 6f);

                await _movement.MoveTo();
                return true;
            }

            public override async Task<bool> Interaction()
            {
                TreeRoot.StatusText = String.Format("Behavior {0} Interaction", Type.ToString());

                if (Player.Inventory.TotalFreeSlots== 0)
                {
                    GarrisonBase.Err("Bags are full!");
                    IsDone = true;
                    return false;
                }

                RefreshInboxMailItemsCollection();

                if (InboxMailItems.Count > 0)
                {
                    if (!LuaEvents.MailOpen)
                        return true;

                    if (LuaCommands.IsButtonEnabled(LuaCommands.ButtonNames.InboxPrevPageButton))
                    {
                        //We need to reset back to first page!
                        while (LuaCommands.IsButtonEnabled(LuaCommands.ButtonNames.InboxPrevPageButton))
                        {
                            LuaCommands.ClickButton(LuaCommands.ButtonNames.InboxPrevPageButton);
                            await CommonCoroutines.SleepForRandomUiInteractionTime();
                        }
                    }

                    int highestInboxPageIndex = 1;
                    foreach (var item in InboxMailItems)
                    {
                        if (item.InboxPageIndex > highestInboxPageIndex)
                            highestInboxPageIndex = item.InboxPageIndex;
                    }

                    if (highestInboxPageIndex>1)
                    {
                        //We want to loot mail that is not on the first page..
                        if (!LuaCommands.IsButtonEnabled(LuaCommands.ButtonNames.InboxPrevPageButton))
                        {
                            for (int i = 1; i < highestInboxPageIndex; i++)
                            {//Click next page until we are at the correct page.
                                LuaCommands.ClickButton(LuaCommands.ButtonNames.InboxNextPageButton);
                                await CommonCoroutines.SleepForRandomUiInteractionTime();
                            }
                        }
                    }

                    //Now get all items on the same page.. order by highest index first (last item)
                    var itemsToGet =
                        InboxMailItems.Where(i => i.InboxPageIndex == highestInboxPageIndex)
                            .OrderByDescending(i => i.Index)
                            .ToList();

                    foreach (var inboxMailItem in itemsToGet)
                    {
                        //Open the mail item..
                        LuaCommands.ClickMailItemButton(inboxMailItem.RealIndex);
                        await CommonCoroutines.SleepForRandomUiInteractionTime();

                        if (LuaCommands.OpenMailFrameIsVisible())
                        {
                            //Click the attachments..
                            for (int i = 0; i < inboxMailItem.ItemCount; i++)
                            {
                                LuaCommands.ClickOpenMailAttachmentButton(i+1);
                                await CommonCoroutines.SleepForRandomUiInteractionTime();
                            }

                            //Close the open mail frame (if visible..)
                            if (LuaCommands.OpenMailFrameIsVisible())
                            {
                                LuaCommands.ClickButton(LuaCommands.ButtonNames.OpenMailFrameCloseButton);
                                await CommonCoroutines.SleepForRandomUiInteractionTime();
                            }
                        }

                        //LuaCommands.AutoLootMailItem(inboxMailItem.Index);
                        //await CommonCoroutines.SleepForRandomUiInteractionTime();
                    }

                    return true;
                }

                return false;
            }



            private class InboxMailItem
            {
                public int Index { get; set; }
                public int ItemCount { get; set; }
                public string Sender { get; set; }
                public string Subject { get; set; }

                public readonly int InboxPageIndex;
                public readonly int RealIndex;

                public InboxMailItem(int index, int itemcount, string sender, string subject)
                {
                    Index = index;
                    ItemCount = itemcount;
                    Sender = sender;
                    Subject = subject;

                    InboxPageIndex = GetMailItemPageIndex(Index);
                    RealIndex = index;
                    if (InboxPageIndex > 1) RealIndex = index - ((InboxPageIndex - 1)*7);
                }

                private int GetMailItemPageIndex(int mailItemIndex)
                {
                    int remainder;
                    int result = Math.DivRem(mailItemIndex, 7, out remainder);
                    if (remainder > 0) result++;
                    return result;
                }

                public override string ToString()
                {
                    return String.Format("Subject {0} Sender {1}\r\n" +
                                         "Index {2} (RealIndex {3}) PageIndex {4}",
                        Subject, Sender, Index, RealIndex, InboxPageIndex);
                }
            }

            private List<InboxMailItem> InboxMailItems = new List<InboxMailItem>();

            private void RefreshInboxMailItemsCollection()
            {
                InboxMailItems.Clear();
                foreach (var inboxMailItem in MailFrame.Instance.GetAllMails())
                {
                    if (inboxMailItem.ItemCount > 0)
                    {
                        InboxMailItems.Add(new InboxMailItem(inboxMailItem.Index, inboxMailItem.ItemCount, inboxMailItem.Sender, inboxMailItem.Subject));
                    }
                }
            }

            public override async Task<bool> BehaviorRoutine()
            {
                if (await base.BehaviorRoutine()) return true;

                if (await Movement()) return true;

                if (await Interaction()) return true;


                return false;
            }
        }
        public class BehaviorSendMail : Behavior
        {
            public override BehaviorType Type { get { return BehaviorType.Mail; } }

            public BehaviorSendMail()
            {

            }

            public override Func<bool> Criteria
            {
                get { return () => BaseSettings.CurrentSettings.MailAutoSend && GetMailingDictionary().Count > 0; }
            }
            public override void Initalize()
            {
                MovementPoints.Add(MovementCache.GarrisonEntrance);
                base.Initalize();
            }

            public C_WoWObject Mailbox
            {
                get { return ObjectCacheManager.GetWoWObjects(WoWObjectTypes.Mailbox).FirstOrDefault(); }
            }
            private Movement _movement;
            public override async Task<bool> Movement()
            {
                if (LuaEvents.MailOpen)
                    return false;


                TreeRoot.StatusText = String.Format("Behavior {0} Movement", Type.ToString());
                if (await base.Movement())
                    return true;



                TreeRoot.StatusText = String.Format("Behavior {0} Movement2", Type.ToString());
                if (Mailbox == null)
                {
                    //No object found..
                    IsDone = true;
                    GarrisonBase.Err("Could not find mail box object!");
                    return false;
                }

                if (Mailbox.WithinInteractRange)
                {
                    Mailbox.Interact();
                    return true;
                }

                if (_movement == null)
                    _movement = new Movement(Mailbox.Location, 5f);

                await _movement.MoveTo();
                return true;
            }
            public override async Task<bool> Interaction()
            {
                TreeRoot.StatusText = String.Format("Behavior {0} Interaction", Type.ToString());

                if (!LuaCommands.IsSendMailFrameVisible())
                {
                    //Click Send Mail Tab
                    LuaCommands.ClickSendMailTab();
                    await CommonCoroutines.SleepForRandomUiInteractionTime();
                    return true;
                }

                _mailingDictionary = GetMailingDictionary();

                foreach (var keypair in _mailingDictionary)
                {
                    GarrisonBase.Log("Found {0} items to mail", keypair.Value.Count);
                    bool success = await SendMail(keypair.Key, keypair.Value);
                    await Coroutine.Yield();
                    return true;
                }

                return false;
            }

            private bool _setRecipient = false;
            public async Task<bool> SendMail(string recipient, List<C_WoWItem> items)
            {

                int count = 0;

                foreach (var cWoWItem in items)
                {
                    if (!LuaEvents.MailOpen) return false;
                    
                    if (count > 11) break;

                    if (BaseSettings.CurrentSettings.MailSendItems.Any(i => i.EntryId == cWoWItem.Entry && i.OnCount > 0))
                    {
                        var mailItemInfo = BaseSettings.CurrentSettings.MailSendItems.FirstOrDefault(i => i.EntryId == cWoWItem.Entry);

                        if (mailItemInfo != null)
                        {
                            int excessCount = (int)cWoWItem.StackCount - mailItemInfo.OnCount;

                            if (excessCount > 0)
                            {
                                GarrisonBase.Log("Send Mail Spliting Item {0} to send count {1}", cWoWItem.Name,excessCount);
                                int freeBagIndex, freeBagSlot;
                                bool foundFreeSpot = Player.Inventory.FindFreeBagSlot(out freeBagIndex, out freeBagSlot);

                                if (foundFreeSpot)
                                {
                                    GarrisonBase.Log("Send Mail Split Item Moving to Bag Index {0} Slot {1}", freeBagIndex, freeBagSlot);

                                    bool success=await SplitItem(cWoWItem, excessCount, freeBagIndex, freeBagSlot);

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
                    LuaCommands.UseContainerItem(cWoWItem.BagIndex+1, cWoWItem.BagSlot+1);
                    await CommonCoroutines.SleepForRandomUiInteractionTime();
                    await Coroutine.Sleep(StyxWoW.Random.Next(1250, 2223));
                    count++;
                }

                if (!_setRecipient)
                {
                    if (!LuaEvents.MailOpen) return false;
                    LuaCommands.SetSendMailRecipient(recipient);
                    _setRecipient = true;
                    await CommonCoroutines.SleepForRandomUiInteractionTime();
                }

                if (!LuaEvents.MailOpen) return false;
                LuaCommands.ClickSendMailButton();
                await CommonCoroutines.SleepForRandomUiInteractionTime();
                await Coroutine.Sleep(StyxWoW.Random.Next(1250, 2223));
                _setRecipient = false;
                return true;
            }

            private async Task<bool> SplitItem(C_WoWItem item, int Count, int BagIndex, int BagSlot)
            {
                if (!LuaCommands.CursorHasItem())
                {
                    //Split Item..
                    bool pickup=
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

            private Dictionary<string, List<C_WoWItem>> _mailingDictionary = new Dictionary<string, List<C_WoWItem>>();

            private Dictionary<string, List<C_WoWItem>> GetMailingDictionary()
            {
                var retDictionary = new Dictionary<string, List<C_WoWItem>>();

                if (BaseSettings.CurrentSettings.MailSendEnchanting)
                {
                    var items = Player.Inventory.GetBagItemsEnchanting();
                    if (items.Count > 0)
                    {
                        retDictionary.Add(BaseSettings.CurrentSettings.MailSendEnchantingRecipient, items);
                    }
                }
                if (BaseSettings.CurrentSettings.MailSendHerbs)
                {
                    var items = Player.Inventory.GetBagItemsHerbs();
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
                    var items = Player.Inventory.GetBagItemsOre();
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
                    var items = Player.Inventory.GetBagItemsBOEByQuality(WoWItemQuality.Uncommon);
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
                    var items = Player.Inventory.GetBagItemsBOEByQuality(WoWItemQuality.Rare);
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
                    var items = Player.Inventory.GetBagItemsBOEByQuality(WoWItemQuality.Epic);
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
                    var items = Player.Inventory.GetMailItems();

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
            public override async Task<bool> BehaviorRoutine()
            {
                if (IsDone) return false;

                if (await base.BehaviorRoutine()) return true;

                if (await Movement()) return true;

                if (await Interaction()) return true;


                return false;
            }
        }
    }
}
