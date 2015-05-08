using System;
using System.Collections.Generic;
using System.Linq;
using Herbfunk.GarrisonBase.Cache.Objects;
using Herbfunk.GarrisonBase.Coroutines;
using Herbfunk.GarrisonBase.Coroutines.Behaviors;
using Herbfunk.GarrisonBase.Garrison;
using Styx;
using Styx.CommonBot.Inventory;
using Styx.WoWInternals;

namespace Herbfunk.GarrisonBase.Character
{
    public class PlayerInventory
    {
        public Dictionary<WoWGuid, C_WoWItem> BagItems = new Dictionary<WoWGuid, C_WoWItem>();
        public Dictionary<WoWGuid, C_WoWItem> BankItems = new Dictionary<WoWGuid, C_WoWItem>();
        public Dictionary<WoWGuid, C_WoWItem> BankReagentItems = new Dictionary<WoWGuid, C_WoWItem>();
        internal List<C_WoWItem> VendorItems = new List<C_WoWItem>();
        internal List<C_WoWItem> DisenchantItems = new List<C_WoWItem>();
        internal List<C_WoWItem> MailItems = new List<C_WoWItem>(); 

        internal static List<WoWGuid> EquipmentManagerItemGuids = new List<WoWGuid>();
        internal static List<WoWGuid> ItemDisenchantingBlacklistedGuids = new List<WoWGuid>();

        internal static bool ShouldVendor = true;

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
            EquipmentManagerItemGuids.Clear();
            ItemDisenchantingBlacklistedGuids.Clear();
            RefreshEquipmentManagerItemGuids();
            ItemDisenchantingBlacklistedGuids.AddRange(EquipmentManagerItemGuids);

            RefreshBackpackContainerInfo();
            UpdateBagItems();
            UpdateBankItems();
            UpdateBankReagentItems();
            
            RefreshLowestDurabilityPercent();

            LuaEvents.OnBagUpdate += () => ShouldUpdateBagItems = true;
            LuaEvents.OnPlayerBankSlotsChanged += () => ShouldUpdateBankItems = true;
            LuaEvents.OnPlayerReagentsBankSlotsChanged += () => ShouldUpdateBankReagentItems = true;

            GarrisonBase.Log(DebugString);

        }

        internal string DebugString
        {
            get
            {
                return String.Format("Total Bag Slots {0} / Free Slots {1}\r\n" +
                                     "Total Bag Items {2}\r\n" +
                                     "Total Bank Items {3}\r\n" +
                                     "Total Reagent Bank Items {4}\r\n" +
                                     "Lowest Durability Item {5}",
                                     TotalBagSlots, TotalFreeSlots,
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
            BagContainerTotalSlots = new[] { 16, 0, 0, 0, 0 };
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
                    var newItem = new C_WoWItem(item);
                    BagItems.Add(item.Guid, newItem);
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


        public bool FindFreeBagSlot(out int bagIndex, out int bagSlot)
        {
            bagIndex = -1;
            bagSlot = -1;

            for (int i = 0; i < 5; i++)
            {
                var bagItemSlots = BagItems.Values.Where(item => item.BagIndex + 1 == i).Select(item => item.BagSlot).ToList();
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

        public List<C_WoWItem> GetCraftingReagentsById(int id, bool includeBankItems=true, bool includeBankReagentItems=true)
        {
            var bagItems = BagItems.Values.Where(i => i.Entry == id).ToArray();
            List<C_WoWItem> retList = new List<C_WoWItem>(bagItems);

            if (includeBankItems)
            {
                var bankItems = BankItems.Values.Where(i => i.Entry == id).ToArray();
                retList.AddRange(bankItems);
            }
            if (includeBankReagentItems)
            {
                var reagentBankItems = BankReagentItems.Values.Where(i => i.Entry == id).ToArray();
                retList.AddRange(reagentBankItems);
            }
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


        public List<C_WoWItem> GetBagItemsBOE()
        {
            List<C_WoWItem> retList = new List<C_WoWItem>();
            return BagItems.Values.Where(i =>
                 !i.IsOpenable &&
                 i.ConsumableClass == WoWItemConsumableClass.None &&
                 !i.IsSoulbound &&
                 (i.ItemClass == WoWItemClass.Armor || i.ItemClass == WoWItemClass.Weapon) &&
                 !EquipmentManagerItemGuids.Contains(i.Guid)).ToList();
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
            return BagItems.Values.Where(i => EnchantingIds.Contains((int)i.Entry)).ToList();
        }
        public List<C_WoWItem> GetBagItemsHerbs()
        {
            return BagItems.Values.Where(i => HerbIds.Contains((int)i.Entry)).ToList();
        }
        public List<C_WoWItem> GetBagItemsOre()
        {
            return BagItems.Values.Where(i => OreIds.Contains((int)i.Entry)).ToList();
        }

        public List<C_WoWItem> GetBagVendorItems()
        {
            return BagItems.Values.Where(i => i.ShouldVendor).ToList();
        }
        public List<C_WoWItem> GetBagDisenchantingForgeItems()
        {
            return BagItems.Values.Where(i => i.ShouldDisenchantWithForge).ToList();
        }
        public List<C_WoWItem> GetBagDisenchantingItems()
        {
            return BagItems.Values.Where(i => i.ShouldDisenchant).ToList();
        }
        public List<C_WoWItem> GetBagItemsMilling()
        {
            var returnHerbs = new List<C_WoWItem>();
            var herbs = GetBagItemsHerbs();
            foreach (var item in herbs)
            {
                if (item.StackCount < 4) continue;

                var type = (CraftingReagents) Enum.Parse(typeof (CraftingReagents), item.Entry.ToString());
                switch (type)
                {
                    case CraftingReagents.Frostweed:
                        if (!BaseSettings.CurrentSettings.MillingFrostWeed.Ignored && 
                            BaseSettings.CurrentSettings.MillingFrostWeed.Reserved<item.StackCount)
                        {
                            returnHerbs.Add(item);
                        }
                        break;
                    case CraftingReagents.Fireweed:
                        if (!BaseSettings.CurrentSettings.MillingFireWeed.Ignored &&
                            BaseSettings.CurrentSettings.MillingFireWeed.Reserved < item.StackCount)
                        {
                            returnHerbs.Add(item);
                        }
                        break;
                    case CraftingReagents.GorgrondFlytrap:
                        if (!BaseSettings.CurrentSettings.MillingGorgrondFlytrap.Ignored &&
                            BaseSettings.CurrentSettings.MillingGorgrondFlytrap.Reserved < item.StackCount)
                        {
                            returnHerbs.Add(item);
                        }
                        break;
                    case CraftingReagents.NagrandArrowbloom:
                        if (!BaseSettings.CurrentSettings.MillingNagrandArrowbloom.Ignored &&
                            BaseSettings.CurrentSettings.MillingNagrandArrowbloom.Reserved < item.StackCount)
                        {
                            returnHerbs.Add(item);
                        }
                        break;
                    case CraftingReagents.Starflower:
                        if (!BaseSettings.CurrentSettings.MillingStarflower.Ignored &&
                            BaseSettings.CurrentSettings.MillingStarflower.Reserved < item.StackCount)
                        {
                            returnHerbs.Add(item);
                        }
                        break;
                    case CraftingReagents.TaladorOrchid:
                        if (!BaseSettings.CurrentSettings.MillingTaladorOrchid.Ignored &&
                            BaseSettings.CurrentSettings.MillingTaladorOrchid.Reserved < item.StackCount)
                        {
                            returnHerbs.Add(item);
                        }
                        break;
                }
            }

            return returnHerbs.OrderByDescending(h => h.StackCount).ToList();
        }

        internal Dictionary<C_WoWItem, string> GetMailItems()
        {
            Dictionary<C_WoWItem, string> mailItems = new Dictionary<C_WoWItem, string>();

            foreach (var mailSendItem in BaseSettings.CurrentSettings.MailSendItems)
            {
                var items = GetBagItemsById(mailSendItem.EntryId).Where(i => i.StackCount > mailSendItem.OnCount).ToList();
                if (items.Count > 0)
                {
                    foreach (var item in items)
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

        public C_WoWItem Trap
        {
            get
            {
                if (_trap == null)
                {
                    var traps = BagItems.Values.Where(i => i.ref_WoWItem != null && i.ref_WoWItem.IsValid && TrapItemEntryIds.Contains(i.Entry)).OrderByDescending(i=>i.TrapRank);
                    _trap = traps.First();
                }

                return _trap;
            }
           
        }
        private C_WoWItem _trap;

        public bool HasSkinningKnife
        {
            get
            {
                if (!_hasSkinningKnife.HasValue || !_hasSkinningKnife.Value)
                    _hasSkinningKnife = BagItems.Values.Any(i => i.ref_WoWItem != null && i.ref_WoWItem.IsValid && SkinningKnifeEntryIds.Contains(i.Entry));

                return _hasSkinningKnife.Value;
            }
        }
        private bool? _hasSkinningKnife;

        public static uint GetTotalStackCount(C_WoWItem[] items)
        {
            uint count = 0;
            foreach (var item in items)
            {
                count += item.StackCount;
            }
            return count;
        }

        public static readonly uint[] SkinningKnifeEntryIds =
        {
            7005, //Skinning Knife
            40772, //Gnomish Army Knife
            69618, //Zulian Slasher
            118724, //Finkle's Flenser
        };

        public static readonly uint[] TrapItemEntryIds =
        {
            113991, //Iron Trap
            115009, //Improved Iron Trap
            115010, //Deadly Iron Trap
        };

        public static readonly List<int> HerbIds = new List<int>
        {
            109125, //Fireweed
            109124, //Frostweed
            109126, //GorgrondFlytrap
            109128, //NagrandArrowbloom
            109127, //Starflower
            109129, //TaladorOrchid
        };

        public static readonly List<int> OreIds = new List<int>
        {
            109118, //BlackrockOre
            109119, //TrueIronOre
        };

        public static readonly List<int> EnchantingIds = new List<int>
        {
            109693, //DraenicDust
            111245, //LuminousShard
            113588, //TemporalCrystal
            115504, //FracturedTemporalCrystal
        };
    }
}
