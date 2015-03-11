using System;
using System.Collections.Generic;
using System.Linq;
using Bots.Grind;
using Herbfunk.GarrisonBase.Cache.Objects;
using Styx;
using Styx.CommonBot.Inventory;
using Styx.WoWInternals;

namespace Herbfunk.GarrisonBase.Cache
{
    public class PlayerInventory
    {
        public Dictionary<WoWGuid, C_WoWItem> BagItems = new Dictionary<WoWGuid, C_WoWItem>();
        public Dictionary<WoWGuid, C_WoWItem> BankItems = new Dictionary<WoWGuid, C_WoWItem>();
        public Dictionary<WoWGuid, C_WoWItem> BankReagentItems = new Dictionary<WoWGuid, C_WoWItem>();
        public List<WoWGuid> EquipmentManagerItemGuids = new List<WoWGuid>();
        public int[] BagContainerTotalSlots { get; set; }

        public int TotalBagSlots
        {
            get
            {
                return BagContainerTotalSlots.Sum();
            }
        }

        public int TotalFreeSlots
        {
            get { return TotalBagSlots - BagItems.Count; }
        }
        public double LowestEquippedDurability { get; set; }

        public const int PrimalSpiritEntryId = 120945;

        public PlayerInventory()
        {
            RefreshEquipmentManagerItemGuids();
            RefreshBackpackContainerInfo();
            UpdateBagItems();
            UpdateBankItems();
            UpdateBankReagentItems();
            ItemDisenchantingBlacklistedGuids.AddRange(EquipmentManagerItemGuids);
            RefreshLowestDurabilityPercent();

            GarrisonBase.Log(DebugString);

        }

        internal string DebugString
        {
            get { return String.Format("Total Bag Slots {0} / Free Slots {1}\r\n" +
                                       "Total Bag Items {2}\r\n" +
                                       "Total Bank Items {3}\r\n" +
                                       "Total Reagent Bank Items {4}\r\n" +
                                       "Lowest Durability Item {5}",
                                       TotalBagSlots,TotalFreeSlots,
                                       BagItems.Count,
                                       BankItems.Count,
                                       BankReagentItems.Count,
                                       LowestEquippedDurability);
            }
        }

        public void Update()
        {
            if (ShouldUpdateBagItems) UpdateBagItems();
            if (ShouldUpdateBankItems) UpdateBankItems();
            if (ShouldUpdateBankReagentItems) UpdateBankReagentItems();
        }

        internal void RefreshLowestDurabilityPercent()
        {
            LowestEquippedDurability = 100;
            using (StyxWoW.Memory.AcquireFrame())
            {
                foreach (var woWItem in StyxWoW.Me.Inventory.Equipped.Items)
                {
                    if (woWItem == null) continue;
                    double durability = woWItem.DurabilityPercent;
                    if (durability < LowestEquippedDurability)
                        LowestEquippedDurability = durability;
                }
            }
        }
        
        internal void RefreshEquipmentManagerItemGuids()
        {
            EquipmentManagerItemGuids.Clear();
            foreach (var equipmentSet in EquipmentManager.EquipmentSets)
            {
                foreach (var woWItem in equipmentSet.Items)
                {
                    if (!EquipmentManagerItemGuids.Contains(woWItem.Guid))
                        EquipmentManagerItemGuids.Add(woWItem.Guid);
                }
            }
        }

        internal Dictionary<int, bool> IgnoredBackpackBags = new Dictionary<int, bool>
        {
            //{-1, false}, //Cannot retrieve default backpack flags
            {0, false},
            {1, false},
            {2, false},
            {3, false}
        };
        internal void RefreshBackpackContainerInfo()
        {
            BagContainerTotalSlots = new[] {16, 0, 0, 0, 0};
            for (int i = 1; i < 5; i++)
            {
                bool ignored = LuaCommands.GetBagSlotFlag(i, 1);
                IgnoredBackpackBags[i - 1] = ignored;
                BagContainerTotalSlots[i] = LuaCommands.GetNumberContainerSlots(i);
            }
        }
        //-1 Base Backpack Bag
        //0 - 3 Extra Backpack Bags

        
        public void UpdateBagItems()
        {
            BagItems.Clear();
            using (StyxWoW.Memory.AcquireFrame())
            {
                foreach (var item in StyxWoW.Me.BagItems)
                {
                    BagItems.Add(item.Guid, new C_WoWItem(item));
                }
            }

            
            ShouldUpdateBagItems = false;
        }
        public bool ShouldUpdateBagItems = true;

        
        public void UpdateBankItems()
        {
            BankItems.Clear();
            using (StyxWoW.Memory.AcquireFrame())
            {
                foreach (var item in StyxWoW.Me.Inventory.Bank.Items)
                {
                    if (item == null) continue;
                    BankItems.Add(item.Guid, new C_WoWItem(item));
                }
            }
            ShouldUpdateBankItems = false;
        }
        public bool ShouldUpdateBankItems = true;

        
        public void UpdateBankReagentItems()
        {
            BankReagentItems.Clear();
            using (StyxWoW.Memory.AcquireFrame())
            {
                foreach (var item in StyxWoW.Me.Inventory.ReagentBank.Items)
                {
                    if (item == null) continue;
                    BankReagentItems.Add(item.Guid, new C_WoWItem(item));
                }
            }
            ShouldUpdateBankReagentItems = false;
        }
        public bool ShouldUpdateBankReagentItems = true;


        public bool FindFreeBagSlot(out int bagIndex , out int bagSlot )
        {
            bagIndex = -1;
            bagSlot = -1;

            for (int i = 0; i < 5; i++)
            {
                var bagItemSlots = BagItems.Values.Where(item => item.BagIndex+1 == i).Select(item => item.BagSlot).ToList();
                for (int j = 0; j < BagContainerTotalSlots[i]; j++)
                {
                    if (bagItemSlots.Contains(j)) continue;
                    bagIndex = i;
                    bagSlot = j;
                    return true;
                }
            }

            return false;
        }

        public List<C_WoWItem> GetCraftingReagentsById(int id)
        {
            var bagItems = BagItems.Values.Where(i => i.Entry == id).ToArray();
            var reagentBankItems = BankReagentItems.Values.Where(i => i.Entry == id).ToArray();
            List<C_WoWItem> retList = new List<C_WoWItem>(bagItems);
            retList.AddRange(reagentBankItems);
            return retList;
        }
        public List<C_WoWItem> GetBagItemsById(int id)
        {
            return BagItems.Values.Where(i => i.Entry == id).ToList();
        }
        public List<C_WoWItem> GetBagItemsById(params int[] args)
        {
            var ids = new List<int>(args);
            return BagItems.Values.Where(i => ids.Contains((int)i.Entry)).ToList();
        }
        public List<C_WoWItem> GetBagItemsByQuality(WoWItemQuality quality)
        {
            return BagItems.Values.Where(i => i.Quality == quality).ToList();
        }

        public List<WoWGuid> ItemDisenchantingBlacklistedGuids =new List<WoWGuid>();

        public List<C_WoWItem> GetBagItemsVendor()
        {
            List<C_WoWItem> retList = new List<C_WoWItem>();

            if (BaseSettings.CurrentSettings.VendorJunkItems)
                retList.AddRange(GetBagItemsByQuality(WoWItemQuality.Poor));

            if (BaseSettings.CurrentSettings.VendorCommonItems)
            {
                //Low level green equipment
                retList.AddRange(GetBagItemsByQuality(WoWItemQuality.Common)
                    .Where(i =>
                        !i.IsOpenable &&
                        i.ConsumableClass == WoWItemConsumableClass.None &&
                        i.RequiredLevel > 0 && (i.RequiredLevel < 90 || (i.RequiredLevel == 90 && i.Level == 429)) &&
                        (i.ItemClass == WoWItemClass.Armor || i.ItemClass == WoWItemClass.Weapon) &&
                        !i.IsSoulbound &&
                        !EquipmentManagerItemGuids.Contains(i.Guid))
                    .ToList());
            }

            if (BaseSettings.CurrentSettings.VendorUncommonItems)
            {
                //Low level green equipment
                retList.AddRange(GetBagItemsByQuality(WoWItemQuality.Uncommon)
                    .Where(i =>
                        !i.IsOpenable &&
                        i.ConsumableClass == WoWItemConsumableClass.None &&
                        i.RequiredLevel > 0 && (i.RequiredLevel < 90 || (i.RequiredLevel == 90 && i.Level == 429)) &&
                        (i.ItemClass == WoWItemClass.Armor || i.ItemClass == WoWItemClass.Weapon) &&
                        !i.IsSoulbound &&
                        !EquipmentManagerItemGuids.Contains(i.Guid))
                    .ToList());
            }

            if (BaseSettings.CurrentSettings.VendorRareItems)
            {
                //Low level green equipment
                retList.AddRange(GetBagItemsByQuality(WoWItemQuality.Rare)
                    .Where(i =>
                        !i.IsOpenable &&
                        i.ConsumableClass == WoWItemConsumableClass.None &&
                        i.RequiredLevel > 0 && (i.RequiredLevel < 90 || (i.RequiredLevel == 90 && i.Level == 429)) &&
                        (i.ItemClass == WoWItemClass.Armor || i.ItemClass == WoWItemClass.Weapon) &&
                        !i.IsSoulbound &&
                        !EquipmentManagerItemGuids.Contains(i.Guid))
                    .ToList());
            }
            

            return retList;
        }
        public List<C_WoWItem> GetBagItemsDisenchantable()
        {
            
            List<C_WoWItem> retList = new List<C_WoWItem>();
            if (BaseSettings.CurrentSettings.DisenchantingUncommon)
            {
                retList.AddRange(GetBagItemsByQuality(WoWItemQuality.Uncommon)
                    .Where(i =>
                            !i.IsOpenable &&
                            i.ConsumableClass == WoWItemConsumableClass.None &&
                            i.RequiredLevel > 89 && (i.Level > 429 && i.Level<=BaseSettings.CurrentSettings.DisenchantingUncommonItemLevel) &&
                            (i.ItemClass == WoWItemClass.Armor || i.ItemClass == WoWItemClass.Weapon) &&
                            !ItemDisenchantingBlacklistedGuids.Contains(i.Guid) &&
                            (!BaseSettings.CurrentSettings.DisenchantingUncommonSoulbounded || i.IsSoulbound))
                    .ToList());
            }
            if (BaseSettings.CurrentSettings.DisenchantingRare)
            {
                retList.AddRange(GetBagItemsByQuality(WoWItemQuality.Rare)
                    .Where(i =>
                            !i.IsOpenable &&
                            i.ConsumableClass == WoWItemConsumableClass.None &&
                            i.RequiredLevel > 89 && (i.Level > 429 && i.Level <= BaseSettings.CurrentSettings.DisenchantingRareItemLevel) &&
                            (i.ItemClass == WoWItemClass.Armor || i.ItemClass == WoWItemClass.Weapon) &&
                            !ItemDisenchantingBlacklistedGuids.Contains(i.Guid) &&
                            (!BaseSettings.CurrentSettings.DisenchantingRareSoulbounded || i.IsSoulbound))
                    .ToList());
            }
            if (BaseSettings.CurrentSettings.DisenchantingEpic)
            {
                retList.AddRange(GetBagItemsByQuality(WoWItemQuality.Epic)
                    .Where(i =>
                            !i.IsOpenable &&
                            i.ConsumableClass == WoWItemConsumableClass.None &&
                            i.RequiredLevel > 89 && (i.Level > 429 && i.Level <= BaseSettings.CurrentSettings.DisenchantingEpicItemLevel) &&
                            (i.ItemClass == WoWItemClass.Armor || i.ItemClass == WoWItemClass.Weapon) &&
                            !ItemDisenchantingBlacklistedGuids.Contains(i.Guid) &&
                            (!BaseSettings.CurrentSettings.DisenchantingEpicSoulbounded || i.IsSoulbound))
                    .ToList());
            }
            return retList;
        }
        public List<C_WoWItem> GetBagItemsBOE()
        {
            List<C_WoWItem> retList = new List<C_WoWItem>();
            return BagItems.Values.Where(i => !i.IsSoulbound).ToList();
        }

        public List<C_WoWItem> GetBankItemsBOE()
        {
            return BankItems.Values.Where(i => !i.IsSoulbound).ToList();
        }
        public List<C_WoWItem> GetBankItemsById(int id)
        {
            return BankItems.Values.Where(i => i.Entry == id).ToList();
        }

        public List<C_WoWItem> GetReagentBankItemsById(int id)
        {
            return BankReagentItems.Values.Where(i => i.Entry == id).ToList();
        }
        public List<C_WoWItem> GetReagentBankItemsBOE()
        {
            return BankReagentItems.Values.Where(i => !i.IsSoulbound).ToList();
        }

        public List<C_WoWItem> GetBagItemsBOEByQuality(WoWItemQuality quality)
        {
            List<C_WoWItem> retList = new List<C_WoWItem>(GetBagItemsBOE());
            return retList.Where(i => i.Quality == quality).ToList();
        }

        public List<C_WoWItem> GetBagItemsEnchanting()
        {
            return BagItems.Values.Where(i => 
                (i.Entry==(int)CraftingReagents.DraenicDust||
                i.Entry==(int)CraftingReagents.LuminousShard||
                i.Entry == (int)CraftingReagents.TemporalCrystal||
                i.Entry == (int)CraftingReagents.FracturedTemporalCrystal)).ToList();
        }
        public List<C_WoWItem> GetBagItemsHerbs()
        {
            return BagItems.Values.Where(i =>
                (i.Entry == (int)CraftingReagents.Frostweed ||
                i.Entry == (int)CraftingReagents.Fireweed ||
                i.Entry == (int)CraftingReagents.NagrandArrowbloom ||
                i.Entry == (int)CraftingReagents.TaladorOrchid ||
                i.Entry == (int)CraftingReagents.GorgrondFlytrap ||
                i.Entry == (int)CraftingReagents.Starflower)).ToList();
        }
        public List<C_WoWItem> GetBagItemsOre()
        {
            return BagItems.Values.Where(i =>
                (i.Entry == (int)CraftingReagents.TrueIronOre ||
                i.Entry == (int)CraftingReagents.BlackrockOre)).ToList();
        }

        internal Dictionary<C_WoWItem, string> GetMailItems()
        {
            Dictionary<C_WoWItem, string> mailItems = new Dictionary<C_WoWItem, string>();

            foreach (var mailSendItem in BaseSettings.CurrentSettings.MailSendItems)
            {
                var items = GetBagItemsById(mailSendItem.EntryId).Where(i => i.StackCount > mailSendItem.OnCount).ToList();
                if (items.Count > 0)
                {
                    foreach (var item in items )
                    {
                        mailItems.Add(item, mailSendItem.Recipient);
                    }
                }
            }

            return mailItems;
        }
        public C_WoWItem GarrisonHearthstone
        {
            get
            {
                if (_garrisonhearthstone == null)
                    _garrisonhearthstone = BagItems.Values.First(i => i.ref_WoWItem != null && i.ref_WoWItem.IsValid && i.Entry == 110560);
                return _garrisonhearthstone;
            }
        }
        private C_WoWItem _garrisonhearthstone;
    }
}
