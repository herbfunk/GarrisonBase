using System;
using Herbfunk.GarrisonBase.Character;
using Styx;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace Herbfunk.GarrisonBase.Cache.Objects
{
    public class C_WoWItem
    {
        public uint Entry { get; set; }
        public string Name { get; set; }
        public WoWGuid Guid { get; set; }
        public int BagIndex { get; set; }
        public int BagSlot { get; set; }
        public WoWItemClass ItemClass { get; set; }
        public uint StackCount { get; set; }
        public WoWItemQuality Quality { get; set; }
        public WoWItemConsumableClass ConsumableClass { get; set; }
        public int RequiredLevel { get; set; }
        public int Level { get; set; }
        public bool IsOpenable { get; set; }
        public bool IsSoulbound { get; set; }
        public bool IsAccountBound { get;set; }

        public int TrapRank
        {
            get
            {
                if (Entry == 113991) return 0; //Iron Trap
                if (Entry == 115009) return 1; //Improved Iron Trap
                if (Entry == 115010) return 2; //Deadly Iron Trap
                return -1;
            }
        }

        public int RequiredDisenchantingLevel
        {
            get
            {
                if (_requiredDisenchantingLevel == -1)
                {
                   _requiredDisenchantingLevel=PlayerProfessions.DisenchantRequiredEnchantingSkill(this);
                }

                return _requiredDisenchantingLevel;
            }
        }
        private int _requiredDisenchantingLevel = -1;

        public readonly WoWItem ref_WoWItem;
        public C_WoWItem(WoWItem item)
        {
            ref_WoWItem = item;
            try
            {
                Entry = item.Entry;
            }
            catch
            {
            }

            try
            {
                Name = item.Name;
            }
            catch
            {
                Name = "Null";
            }


            try
            {
                Guid = item.Guid;
            }
            catch
            {
                
            }


            try
            {
                BagIndex = item.BagIndex;
            }
            catch
            {
                BagIndex = -1;
            }


            try
            {
                BagSlot = item.BagSlot;
            }
            catch
            {
                BagSlot = -1;
            }


            try
            {
                StackCount = item.StackCount;
            }
            catch
            {
                StackCount = Convert.ToUInt32(1);
            }


            try
            {
                Quality = item.Quality;
            }
            catch
            {
                Quality = WoWItemQuality.Artifact;
            }

            try
            {
                Level = item.ItemInfo.Level;
            }
            catch
            {
                Level = 0;
            }

            try
            {
                RequiredLevel = item.ItemInfo.RequiredLevel;
            }
            catch
            {
                RequiredLevel = 0;
            }

            try
            {
                IsOpenable = item.IsOpenable;
            }
            catch
            {
                IsOpenable = false;
            }

            try
            {
                ConsumableClass = item.ItemInfo.ConsumableClass;
            }
            catch
            {
                ConsumableClass = WoWItemConsumableClass.None;
            }

            try
            {
                ItemClass = item.ItemInfo.ItemClass;
            }
            catch
            {
                ItemClass = WoWItemClass.Miscellaneous;
            }

            try
            {
                IsSoulbound = item.IsSoulbound;
            }
            catch
            {
                IsSoulbound = true;
            }

            try
            {
                IsAccountBound = item.IsAccountBound;
            }
            catch
            {
                IsAccountBound = true;
            }
            

        }

        public bool IsValid
        {
            get
            {
                return Guid.IsValid &&
                       ref_WoWItem != null &&
                       ref_WoWItem.BaseAddress != IntPtr.Zero &&
                       ref_WoWItem.IsValid;
            }
        }

        public DateTime LastUsed = DateTime.MinValue;
        public void Use()
        {
            if (ref_WoWItem.Use())
                LastUsed = DateTime.Now;
        }
        public void Interact()
        {
            ref_WoWItem.Interact();
        }

        public bool OnCooldown
        {
            get { return ref_WoWItem.CooldownTimeLeft.TotalMilliseconds > 0f; }
        }

        public bool ShouldVendor
        {
            get
            {
                if (!IsValid) return false;

                if ((Quality == WoWItemQuality.Poor && !BaseSettings.CurrentSettings.VendorJunkItems) ||
                    (Quality == WoWItemQuality.Common && !BaseSettings.CurrentSettings.VendorCommonItems)||
                    (Quality == WoWItemQuality.Uncommon && !BaseSettings.CurrentSettings.VendorUncommonItems)||
                    (Quality == WoWItemQuality.Rare && !BaseSettings.CurrentSettings.VendorRareItems))
                {
                    return false;
                }

                switch (Quality)
                {
                    case WoWItemQuality.Poor:
                        return true;

                    case WoWItemQuality.Common:
                    case WoWItemQuality.Uncommon:
                    case WoWItemQuality.Rare:

                        return (!IsOpenable && !IsAccountBound &&
                                ConsumableClass == WoWItemConsumableClass.None &&
                                RequiredLevel > 0 &&
                                ((RequiredLevel < 90 && Level > 1) || (RequiredLevel == 90 && Level == 429)) &&
                                (ItemClass == WoWItemClass.Armor || ItemClass == WoWItemClass.Weapon) &&
                                !IsSoulbound &&
                                !PlayerInventory.EquipmentManagerItemGuids.Contains(Guid));
                }

                return false;
            }
        }
        public bool ShouldDisenchantWithForge
        {
            get
            {
                if (!IsValid) return false;

                if (Quality != WoWItemQuality.Uncommon && Quality != WoWItemQuality.Rare &&
                    Quality != WoWItemQuality.Epic) return false;

                if ((PlayerInventory.ItemDisenchantingBlacklistedGuids.Contains(Guid) ||
                     IsOpenable ||
                     IsAccountBound ||
                     RequiredLevel < 90 ||
                     Level < 430 ||
                     ConsumableClass != WoWItemConsumableClass.None ||
                     (ItemClass != WoWItemClass.Armor && ItemClass != WoWItemClass.Weapon)) ||

                    (Quality == WoWItemQuality.Uncommon &&
                     (!BaseSettings.CurrentSettings.DisenchantingUncommon ||
                      (Level > BaseSettings.CurrentSettings.DisenchantingUncommonItemLevel) ||
                      (IsSoulbound && !BaseSettings.CurrentSettings.DisenchantingUncommonSoulbounded))) ||

                    (Quality == WoWItemQuality.Rare &&
                     (!BaseSettings.CurrentSettings.DisenchantingRare ||
                      (Level > BaseSettings.CurrentSettings.DisenchantingRareItemLevel) ||
                      (IsSoulbound && !BaseSettings.CurrentSettings.DisenchantingRareSoulbounded))) ||

                    (Quality == WoWItemQuality.Epic &&
                     (!BaseSettings.CurrentSettings.DisenchantingEpic ||
                      (Level > BaseSettings.CurrentSettings.DisenchantingEpicItemLevel) ||
                      (IsSoulbound && !BaseSettings.CurrentSettings.DisenchantingEpicSoulbounded))))
                {
                    return false;
                }


                return true;
            }
        }
        public bool ShouldDisenchant
        {
            get
            {
                if (!IsValid) return false;

                if (Quality != WoWItemQuality.Uncommon && Quality != WoWItemQuality.Rare &&
                    Quality != WoWItemQuality.Epic) return false;


                if ((PlayerInventory.ItemDisenchantingBlacklistedGuids.Contains(Guid) ||
                     IsOpenable ||
                     IsAccountBound ||
                     ConsumableClass != WoWItemConsumableClass.None ||
                     (ItemClass != WoWItemClass.Armor && ItemClass != WoWItemClass.Weapon) ||
                     RequiredDisenchantingLevel > Player.Professions.ProfessionSkills[SkillLine.Enchanting].CurrentValue) ||

                    (Quality == WoWItemQuality.Uncommon &&
                     (!BaseSettings.CurrentSettings.DisenchantingUncommon ||
                      (Level > BaseSettings.CurrentSettings.DisenchantingUncommonItemLevel) ||
                      (IsSoulbound && !BaseSettings.CurrentSettings.DisenchantingUncommonSoulbounded))) ||

                    (Quality == WoWItemQuality.Rare &&
                     (!BaseSettings.CurrentSettings.DisenchantingRare ||
                      (Level > BaseSettings.CurrentSettings.DisenchantingRareItemLevel) ||
                      (IsSoulbound && !BaseSettings.CurrentSettings.DisenchantingRareSoulbounded))) ||

                    (Quality == WoWItemQuality.Epic &&
                     (!BaseSettings.CurrentSettings.DisenchantingEpic ||
                      (Level > BaseSettings.CurrentSettings.DisenchantingEpicItemLevel) ||
                      (IsSoulbound && !BaseSettings.CurrentSettings.DisenchantingEpicSoulbounded))))
                {
                    return false;
                }


                return true;
            }
        }
        public bool ShouldMail
        {
            get
            {
                MailItem mailitem;
                if (BaseSettings.CurrentSettings.DictMailSendItems.TryGetValue((int) Entry, out mailitem))
                {
                    MailItemInfo = mailitem;
                    return true;
                }
                return false;
            }
        }

        public MailItem MailItemInfo = null;

        public override int GetHashCode()
        {
            return Guid.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("{0} [{1}]--[{2}]\r\n" +
                                 "Item Class {11}\r\n" +
                                 "Bag Index {3} / Bag Slot {4}\r\n" +
                                 "Quality {5} -- StackCount {6}\r\n" +
                                 "Level {7} Required Level {8}\r\n" +
                                 "IsOpenable {9} Consumable Class {10} IsSoulbound {12}\r\n" +
                                 "RequiredDisenchantLevel {13}",
                Name, Entry, Guid,
                BagIndex, BagSlot,
                Quality, StackCount,
                Level, RequiredLevel,
                IsOpenable, ConsumableClass, ItemClass, IsSoulbound,
                RequiredDisenchantingLevel);
        }
    }
}