using System;
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
           

        }

        public void Use()
        {
            ref_WoWItem.Use();
        }
        public void Interact()
        {
            ref_WoWItem.Interact();
        }

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
                                 "IsOpenable {9} Consumable Class {10} IsSoulbound {12}",
                Name, Entry, Guid,
                BagIndex, BagSlot,
                Quality, StackCount,
                Level, RequiredLevel,
                IsOpenable, ConsumableClass, ItemClass, IsSoulbound);
        }
    }
}