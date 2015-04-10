using System;
using System.Collections.Generic;
using System.Linq;
using Styx.Common;
using Styx.CommonBot.Frames;
using Styx.Helpers;
using Styx.WoWInternals;

namespace Herbfunk.GarrisonBase.Helpers
{
    public static class MerchantHelper
    {
        public static List<MItem> MerchantItems { get; set; }
        public static int MerchantItemCount { get; set; }
        public static bool IsOpen { get; set; }
        static MerchantHelper()
        {
            MerchantItemCount = 0;
            MerchantItems = new List<MItem>();
            LuaEvents.OnMerchantShow += MerchantFrameOpened;
            LuaEvents.OnMerchantClosed += MerchantFrameClosed;
            IsOpen = MerchantFrame.Instance.IsVisible;
        }

        /// <summary>
        /// Attempts to buy merchant item using item name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="amount"></param>
        /// <param name="forced">Forced will send lua to buy the item instead of MerchantFrame</param>
        /// <returns>False if item could not be found or Merchant Frame returned false</returns>
        public static bool BuyItem(string name, int amount, bool forced=false)
        {
            name = name.ToLower();
            var items = MerchantItems.Where(i => i.Name.Contains(name)).ToList();
            if (items.Count == 0) return false;
            return buyitem(items[0], amount, forced);
        }
        /// <summary>
        /// Attempts to buy merchant item using item index
        /// </summary>
        /// <param name="name"></param>
        /// <param name="amount"></param>
        /// <param name="forced">Forced will send lua to buy the item instead of MerchantFrame</param>
        /// <returns>False if item could not be found or Merchant Frame returned false</returns>
        public static bool BuyItem(int index, int amount, bool forced = false)
        {
            var items = MerchantItems.Where(i => i.Index==index).ToList();
            if (items.Count == 0) return false;
            return buyitem(items[0], amount, forced);
        }

        private static bool buyitem(MItem item, int amount, bool forced=false)
        {
            GarrisonBase.Debug("Attempting to buy Merchant item ({0}) at index {1} of quantity {2} (forced={3})",
                                                                item.Name,item.Index,amount,forced);
            if (!forced)
            {
                return MerchantFrame.Instance.BuyItem(item.Index, amount);
            }

            LuaBuyMerchantItem(item.Index, amount);
            return true;
        }

        private static void LuaBuyMerchantItem(int index, int amount)
        {
            string lua = String.Format("BuyMerchantItem(\"{0}\"" + ", \"{1}\");", index, amount);
            GarrisonBase.Debug("LuaCommand: BuyMerchantItem {0} / {1}", index, amount);
            Lua.DoString(lua);
        }

        private static void MerchantFrameOpened()
        {
            GarrisonBase.Debug("Updating Merchant Frame Info..");
            MerchantItemCount = GetMerchantItemCount();
            MerchantItems.Clear();
            for (int i = 1; i < MerchantItemCount+1; i++)
            {
                string name = GetMerchantItemName(i);
                MerchantItems.Add(new MItem(i, name));
            }
            //foreach (var item in MerchantFrame.Instance.GetAllMerchantItems())
            //{
            //    MerchantItems.Add(new MItem(item));
            //}
            IsOpen = true;
        }
        private static void MerchantFrameClosed()
        {
            MerchantItems.Clear();
            IsOpen = false;
        }

        private static string GetMerchantItemName(int index)
        {
            //GetMerchantItemInfo(5)
            //[1] = Name
            //[2] = Icon
            GarrisonBase.Debug("LuaCommand: GetMerchantItemInfo {0}", index);
            string lua = String.Format("return GetMerchantItemInfo(\"{0}\")", index);
            List<string> retvalues = Lua.GetReturnValues(lua);
            var name = retvalues[0];
            return name;
        }

        private static int GetMerchantItemCount()
        {
            GarrisonBase.Debug("LuaCommand: GetMerchantNumItems");
            string lua = String.Format("return GetMerchantNumItems()");
            List<string> retvalues = Lua.GetReturnValues(lua);
            Logging.Write("{0}", retvalues[0].ToInt32());
            return retvalues[0].ToInt32();
        }

        public static readonly List<uint> GarrisonVendorEntryIds = new List<uint>
        {
            //Horde
            76872,
            79774,
            87868,
            81981,
            79857,

            //Alliance
            81346,
            88223,
            85708,
            77378,
        };


        public class MItem
        {
            public int Index { get; set; }
            public string Name { get; set; }
            public uint ItemId { get; set; }

            public MItem(MerchantItem item)
            {
                Index = item.Index;
                Name = item.Name.ToLower();
                ItemId = item.ItemId;
            }

            public MItem(int index, string name)
            {
                Index = index;
                Name = name.ToLower();
            }
        }
    }
}
