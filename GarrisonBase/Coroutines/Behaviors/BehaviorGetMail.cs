using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Herbfunk.GarrisonBase.Cache;
using Herbfunk.GarrisonBase.Cache.Enums;
using Herbfunk.GarrisonBase.Cache.Objects;
using Styx;
using Styx.CommonBot;
using Styx.CommonBot.Coroutines;
using Styx.CommonBot.Frames;

namespace Herbfunk.GarrisonBase.Coroutines.Behaviors
{
    public class BehaviorGetMail : Behavior
    {
        public override BehaviorType Type { get { return BehaviorType.Mail; } }
        public BehaviorGetMail() : base(MovementCache.GarrisonEntrance)
        {
            Criteria += () => BaseSettings.CurrentSettings.MailAutoGet && LuaCommands.HasNewMail();
        }


        public C_WoWObject Mailbox
        {
            get { return ObjectCacheManager.GetWoWObjects(WoWObjectTypes.Mailbox).FirstOrDefault(); }
        }

        public override async Task<bool> BehaviorRoutine()
        {
            if (await base.BehaviorRoutine()) return true;
            if (IsDone) return false;

            if (await StartMovement.MoveTo()) return true;

            if (await Movement()) return true;

            if (await Interaction()) return true;


            return false;
        }

        private Movement _movement;
        public override async Task<bool> Movement()
        {
            if (LuaEvents.MailOpen)
                return false;

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

            if (Character.Player.Inventory.TotalFreeSlots== 0)
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

                if (LuaUI.InboxPreviousPage.IsEnabled())
                {
                    //We need to reset back to first page!
                    while (LuaUI.InboxPreviousPage.IsEnabled())
                    {
                        LuaUI.InboxPreviousPage.Click();
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
                    if (!LuaUI.InboxPreviousPage.IsEnabled())
                    {
                        for (int i = 1; i < highestInboxPageIndex; i++)
                        {//Click next page until we are at the correct page.
                            LuaUI.InboxNextPage.Click();
                            await CommonCoroutines.SleepForRandomUiInteractionTime();
                        }
                    }
                }

                //Now get all items on the same page.. order by highest index first (last item)
                var itemsToGet =
                    InboxMailItems.Where<InboxMailItem>(i => i.InboxPageIndex == highestInboxPageIndex)
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
                            LuaUI.InboxClose.Click();
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

           
    }
}