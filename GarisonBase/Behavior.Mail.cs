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
                get { return () => BaseSettings.CurrentSettings.MailAutoGet && LuaCommands.IsMiniMapMailIconVisible(); }
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

                foreach (var inboxMailItem in MailFrame.Instance.GetAllMails())
                {
                    if (inboxMailItem.ItemCount > 0)
                    {
                        //int index = inboxMailItem.Index;
                        //if (index > 7)
                        //{
                        //    index -= 7;
                        //    LuaCommands.ClickMailboxNextPageButton();
                        //    await CommonCoroutines.SleepForRandomReactionTime();
                        //}

                        //LuaCommands.ClickMailItemButton(index);
                        //await CommonCoroutines.SleepForRandomReactionTime();

                        await inboxMailItem.TakeAttachedItemsCoroutine();
                        return true;
                    }
                }

                return false;
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

                if (Mailbox.CentreDistance < 6)
                {
                    Mailbox.Interact();
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
                    GarrisonBase.Log("Found {0} items to mail to {1}", keypair.Value.Count, keypair.Key);
                    await SendMail(keypair.Key, keypair.Value);
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
                    if (count > 11) break;
                    GarrisonBase.Log("Attaching item {0} to mail", cWoWItem.Name);
                    LuaCommands.UseContainerItem(cWoWItem.BagIndex+1, cWoWItem.BagSlot+1);
                    await CommonCoroutines.SleepForRandomUiInteractionTime();
                    await Coroutine.Sleep(StyxWoW.Random.Next(1250, 2223));
                    count++;
                }

                if (!_setRecipient)
                {
                    LuaCommands.SetSendMailRecipient(recipient);
                    _setRecipient = true;
                    await CommonCoroutines.SleepForRandomUiInteractionTime();
                }

                LuaCommands.ClickSendMailButton();
                await CommonCoroutines.SleepForRandomUiInteractionTime();
                await Coroutine.Sleep(StyxWoW.Random.Next(1250, 2223));
                _setRecipient = false;
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
                if (await base.BehaviorRoutine()) return true;

                if (await Movement()) return true;

                if (await Interaction()) return true;


                return false;
            }
        }
    }
}
