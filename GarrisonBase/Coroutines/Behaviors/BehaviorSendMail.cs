using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Herbfunk.GarrisonBase.Cache;
using Herbfunk.GarrisonBase.Cache.Enums;
using Herbfunk.GarrisonBase.Cache.Objects;
using Styx;
using Styx.CommonBot.Coroutines;

namespace Herbfunk.GarrisonBase.Coroutines.Behaviors
{
    public class BehaviorSendMail : Behavior
    {
        public override BehaviorType Type { get { return BehaviorType.Mail; } }

        public BehaviorSendMail()
            : base(MovementCache.GarrisonEntrance)
        {
            Criteria += () => BaseSettings.CurrentSettings.MailAutoSend && GetMailingDictionary().Count > 0;
        }


        public override void Initalize()
        {
            _movement = null;
            base.Initalize();
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
        private async Task<bool> Movement()
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
                return true;
            }

            if (_movement == null)
                _movement = new Movement(Mailbox.Location, 5f, name: "Mailbox");

            await _movement.MoveTo(false);
            return true;
        }
        private async Task<bool> Interaction()
        {
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
        private async Task<bool> SendMail(string recipient, List<C_WoWItem> items)
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
                            bool foundFreeSpot = Character.Player.Inventory.FindFreeBagSlot(out freeBagIndex, out freeBagSlot);

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
}