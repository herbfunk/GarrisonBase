using System;
using System.Collections.Generic;
using System.Linq;
using Herbfunk.GarrisonBase.Cache.Objects;
using Herbfunk.GarrisonBase.Garrison.Enums;

namespace Herbfunk.GarrisonBase.Garrison.Objects
{
    public class WorkOrder
    {
        public BuildingType BuildingType { get; set; }
        public int BuildingId { get; set; }
        public WorkOrderType Type { get; set; }
        public int Maximum { get; set; }
        public int Pending { get; set; }
        public int Pickup { get; set; }
        public CacheStaticLookUp.RushOrders RushOrderItemType { get; set; }
        public Tuple<CraftingReagents, int>[] Currency { get; set; }

        public WorkOrder(int buildingId, BuildingType buildingType, WorkOrderType workorderType,
            int max, Tuple<CraftingReagents, int>[] currency, int pending = 0, int pickup = 0)
        {
            BuildingId = buildingId;
            BuildingType = buildingType;
            Type = workorderType;
            Maximum = max;
            Currency = currency;
            Pending = pending;
            Pickup = pickup;
            RushOrderItemType = GetRushOrderItem(Type);
        }

        public long TotalWorkorderStartups()
        {
            if (Type == WorkOrderType.WarMillDwarvenBunker)
            {
                int garrisonResources = Character.Player.AvailableGarrisonResource;
                return garrisonResources >= 20 ? garrisonResources / 20 : 0;
            }
            return GetTotalWorkorderStartups(Currency);
        }

        public void Refresh()
        {
            string plotid;
            int shipcap, shipmax, shipready;
            bool isbuilding, canactivate;

            LuaCommands.GetBuildingInfo(BuildingId, out plotid, out canactivate, out shipcap, out shipready, out shipmax, out isbuilding);

            Maximum = shipcap;
            Pending = shipready;
            Pickup = shipmax;
        }

        public override string ToString()
        {
            return String.Format("Type[{0}] Building {1} ({2}) - Max {3} Pending {4} Pickup {5}",
                Type, BuildingType, BuildingId, Maximum, Pending, Pickup);
        }

        #region Static Collections and Methods

        [Flags]
        public enum TradePostReagentTypes
        {
            None = 0,
            BlackrockOre=1,
            TrueIronOre=2,
            DraenicDust=4,
            SumptuousFur=8,
            RawBeastHide=16,
            Starflower=32,
            Frostweed=64,
            Fireweed=128,
            GorgrondFlytrap=256,
            NagrandArrowbloom=512,
            TaladorOrchid=1024,
            CrescentSaberfishFlesh=2048,

            All = BlackrockOre | TrueIronOre | DraenicDust | SumptuousFur | RawBeastHide | Starflower | Frostweed | Fireweed | GorgrondFlytrap | NagrandArrowbloom | TaladorOrchid | CrescentSaberfishFlesh,
        }

        public static CacheStaticLookUp.RushOrders GetRushOrderItem(WorkOrderType type)
        {
            switch (type)
            {
                case WorkOrderType.Enchanting:
                    return CacheStaticLookUp.RushOrders.EnchantersStudy;
                case WorkOrderType.Inscription:
                    return CacheStaticLookUp.RushOrders.Scribe;
                case WorkOrderType.Mining:
                    return CacheStaticLookUp.RushOrders.Mines;
                case WorkOrderType.Herbalism:
                    return CacheStaticLookUp.RushOrders.HerbGarden;
                case WorkOrderType.Blacksmith:
                    return CacheStaticLookUp.RushOrders.Forge;
                //case WorkOrderType.Lumbermill:
                //    return 
                //case WorkOrderType.Tradepost:
                //    break;
                case WorkOrderType.Alchemy:
                    return CacheStaticLookUp.RushOrders.AlchemyLab;
                case WorkOrderType.Engineering:
                    return CacheStaticLookUp.RushOrders.Engineering;
                case WorkOrderType.Jewelcrafting:
                    return CacheStaticLookUp.RushOrders.Gem;
                case WorkOrderType.Tailoring:
                    return CacheStaticLookUp.RushOrders.Tailoring;
                case WorkOrderType.Leatherworking:
                    return CacheStaticLookUp.RushOrders.Tannery;
                case WorkOrderType.WarMillDwarvenBunker:
                    return CacheStaticLookUp.RushOrders.WarMill;
                case WorkOrderType.DwarvenBunker:
                    break;
                case WorkOrderType.Barn:
                    return CacheStaticLookUp.RushOrders.Barn;
            }

            return CacheStaticLookUp.RushOrders.None;
        }

        public static TradePostReagentTypes GetTradePostNPCReagent(int npcID)
        {
            switch (npcID)
            {
                case 86803:
                case 87211:
                    return TradePostReagentTypes.TrueIronOre;
                case 87217:
                case 87121:
                    return TradePostReagentTypes.BlackrockOre;
                case 87216:
                case 87118:
                    return TradePostReagentTypes.GorgrondFlytrap;
                case 87215:
                case 87116:
                    return TradePostReagentTypes.NagrandArrowbloom;
                case 87214:
                case 87114:
                    return TradePostReagentTypes.Starflower;
                case 87213:
                case 87113:
                    return TradePostReagentTypes.SumptuousFur;
                case 87212:
                case 87112:
                    return TradePostReagentTypes.TaladorOrchid;
                case 87210:
                case 87115:
                    return TradePostReagentTypes.RawBeastHide;
                case 87209:
                case 87117:
                    return TradePostReagentTypes.Frostweed;
                case 87208:
                case 87119:
                    return TradePostReagentTypes.Fireweed;
                case 87207:
                case 87120:
                    return TradePostReagentTypes.DraenicDust;
                case 91071:
                    return TradePostReagentTypes.CrescentSaberfishFlesh;
            }

            return TradePostReagentTypes.None;
        }

        public static Tuple<CraftingReagents, int>[] GetTradePostItemAndQuanityRequired(TradePostReagentTypes type)
        {
            switch (type)
            {
                case TradePostReagentTypes.BlackrockOre:
                    return new[] { new Tuple<CraftingReagents, int>(CraftingReagents.BlackrockOre, 5) };
                case TradePostReagentTypes.TrueIronOre:
                    return new[] { new Tuple<CraftingReagents, int>(CraftingReagents.TrueIronOre, 5) };
                case TradePostReagentTypes.DraenicDust:
                    return new[] { new Tuple<CraftingReagents, int>(CraftingReagents.DraenicDust, 5) };
                case TradePostReagentTypes.SumptuousFur:
                    return new[] { new Tuple<CraftingReagents, int>(CraftingReagents.SumptuousFur, 5) };
                case TradePostReagentTypes.RawBeastHide:
                    return new[] { new Tuple<CraftingReagents, int>(CraftingReagents.RawBeastHide, 5) };
                case TradePostReagentTypes.Starflower:
                    return new[] { new Tuple<CraftingReagents, int>(CraftingReagents.Starflower, 5) };
                case TradePostReagentTypes.Frostweed:
                    return new[] { new Tuple<CraftingReagents, int>(CraftingReagents.Frostweed, 5) };
                case TradePostReagentTypes.Fireweed:
                    return new[] { new Tuple<CraftingReagents, int>(CraftingReagents.Fireweed, 5) };
                case TradePostReagentTypes.GorgrondFlytrap:
                    return new[] { new Tuple<CraftingReagents, int>(CraftingReagents.GorgrondFlytrap, 5) };
                case TradePostReagentTypes.NagrandArrowbloom:
                    return new[] { new Tuple<CraftingReagents, int>(CraftingReagents.NagrandArrowbloom, 5) };
                case TradePostReagentTypes.TaladorOrchid:
                    return new[] { new Tuple<CraftingReagents, int>(CraftingReagents.TaladorOrchid, 5) };
                case TradePostReagentTypes.CrescentSaberfishFlesh:
                    return new[] { new Tuple<CraftingReagents, int>(CraftingReagents.CrescentSaberfishFlesh, 20) };
            }

            return null;
        }

        #region Work Order Object Entry Ids

        internal static readonly Dictionary<WorkOrderType, int> WorkOrderPickupEntryIds = new Dictionary<WorkOrderType, int>
        {
            {WorkOrderType.Enchanting, 237133},
            {WorkOrderType.Inscription, 237063}, //237064
            {WorkOrderType.Mining, 239237},
            {WorkOrderType.Herbalism, 239238},
            {WorkOrderType.Blacksmith, 237127},
            {WorkOrderType.Lumbermill, 233832},
            {WorkOrderType.Tradepost, 237355},
            {WorkOrderType.Leatherworking, 237103}, //237104
            {WorkOrderType.Alchemy, 237913},
            {WorkOrderType.Engineering, 237139},
            {WorkOrderType.Jewelcrafting, 237067},

            {WorkOrderType.Tailoring, 000},
            {WorkOrderType.WarMillDwarvenBunker, 000},
            {WorkOrderType.Barn, 000}
        };
        
        #endregion

        #region Work Order Object Names

        internal static readonly Dictionary<WorkOrderType, string> WorkOrderPickupNames = new Dictionary<WorkOrderType, string>
        {
            {WorkOrderType.Enchanting, "Enchanting Work Order" },
            {WorkOrderType.Inscription, "Inscription Work Order" }, 
            {WorkOrderType.Mining, "Mine Work Order"},
            {WorkOrderType.Herbalism, "Herb Garden Work Order"},
            {WorkOrderType.Blacksmith, "Blacksmithing Work Order"},
            {WorkOrderType.Lumbermill, "Lumber Mill Work Order"},
            {WorkOrderType.Tradepost, "Trading Post Work Order"},
            {WorkOrderType.Leatherworking, "Leatherworking Work Order"}, 
            {WorkOrderType.Alchemy, "Alchemy Work Order"},
            {WorkOrderType.Engineering, "Engineering Work Order"},
            {WorkOrderType.Jewelcrafting, "Jewelcrafting Work Order"},
            {WorkOrderType.Tailoring, "Tailoring Work Order"},
            {WorkOrderType.WarMillDwarvenBunker, "War Mill Work Order"},
            {WorkOrderType.DwarvenBunker, "Dwarven Bunker Work Order"},
            {WorkOrderType.Barn, "Barn Work Order"},
            {WorkOrderType.None, "Crate"}
        };
        
        #endregion

        #region Trade Post IDs

        public static readonly List<uint> HordeTradePostNpcIds = new List<uint>
        {
            86803,
            87112,
            87113,
            87114,
            87115,
            87116,
            87117,
            87118,
            87119,
            87120,
            87121,
            91070,
        };
        public static readonly List<uint> AllianceTradePostNpcIds = new List<uint>
        {
            87207,
            87208,
            87209,
            87210,
            87211,
            87212,
            87213,
            87214,
            87215,
            87216,
            87217,
            91071,
        };

        /* Trade Post
        * 
        * Horde
        * EntryID - Currency
        * 86803 - True Iron Ore
        * 87112 - Talador Orchid
        * 87113 - Sumptuous Fur
        * 87114 - Starflower
        * 87115 - Raw Beast Hide
        * 87116 - Nagrand Arrowbloom
        * 87117 - Frostweed
        * 87118 - Gorgrond Flytrap
        * 87119 - Fireweed
        * 87120 - Draenic Dust
        * 87121 - Blackrock Ore
        * 91070 - Crescent Saberfish Flesh
        * 
        * Alliance
        * 87207 - Draenic Dust
        * 87208 - Fireweed
        * 87209 - Frostweed
        * 87210 - Raw Beast Hide
        * 87211 - True Iron Ore
        * 87212 - Talador Orchid
        * 87213 - Sumptuous Fur
        * 87214 - Starflower
        * 87215 - Nagrand Arrowbloom
        * 87216 - Gorgrond Flytrap
        * 87217 - Blackrock Ore
        * 91071 - Crescent Saberfish Flesh
        */

        #endregion

        #region Work Order Npc Ids Collection

        public static readonly List<uint> WorkOrderNpcIds = new List<uint>
        {
            79857,
            77378,
            79820,
            77781,
            79817,
            77792,
            79831,
            77777,
            81688,
            77730,
            85783,
            85514,
            84247,
            84248,
            79814,
            77791,
            86696,
            77831,
            79830,
            77775,
            79863,
            77778,
            79833,
            78207,

            89066, 89065,


            //Alliance Trade Post
            87207,
            87208,
            87209,
            87210,
            87211,
            87212,
            87213,
            87214,
            87215,
            87216,
            87217,

            //Horde Trade Post
            86803,
            87112,
            87113,
            87114,
            87115,
            87116,
            87117,
            87118,
            87119,
            87120,
            87121,
        };
        
        #endregion

        public static WorkOrderType GetWorkorderType(BuildingType type)
        {
            switch (type)
            {
                case BuildingType.EnchantersStudy:
                    return WorkOrderType.Enchanting;
                case BuildingType.ScribesQuarters:
                    return WorkOrderType.Inscription;
                case BuildingType.Mines:
                    return WorkOrderType.Mining;
                case BuildingType.HerbGarden:
                    return WorkOrderType.Herbalism;
                case BuildingType.TheForge:
                    return WorkOrderType.Blacksmith;
                case BuildingType.Lumbermill:
                    return WorkOrderType.Lumbermill;
                case BuildingType.TradingPost:
                    return WorkOrderType.Tradepost;
                case BuildingType.AlchemyLab:
                    return WorkOrderType.Alchemy;
                case BuildingType.EngineeringWorks:
                    return WorkOrderType.Engineering;
                case BuildingType.GemBoutique:
                    return WorkOrderType.Jewelcrafting;
                case BuildingType.TailoringEmporium:
                    return WorkOrderType.Tailoring;
                case BuildingType.TheTannery:
                    return WorkOrderType.Leatherworking;
                case BuildingType.Barn:
                    return WorkOrderType.Barn;
                case BuildingType.WarMillDwarvenBunker:
                    return WorkOrderType.WarMillDwarvenBunker;
            }

            return WorkOrderType.None;
        }

        public static Tuple<CraftingReagents, int>[] GetWorkOrderItemAndQuanityRequired(WorkOrderType type)
        {
            switch (type)
            {
                case WorkOrderType.Enchanting:
                    return new[] { new Tuple<CraftingReagents, int>(CraftingReagents.DraenicDust, 5) };
                case WorkOrderType.Inscription:
                    return new[] { new Tuple<CraftingReagents, int>(CraftingReagents.CeruleanPigment, 2) };
                case WorkOrderType.Mining:
                    return new[] { new Tuple<CraftingReagents, int>(CraftingReagents.DraenicStone, 5) };
                case WorkOrderType.Herbalism:
                    return new[] { new Tuple<CraftingReagents, int>(CraftingReagents.DraenicSeeds, 5) };
                case WorkOrderType.Blacksmith:
                    return new[] { new Tuple<CraftingReagents, int>(CraftingReagents.TrueIronOre, 5) };
                case WorkOrderType.Lumbermill:
                    return new[] { new Tuple<CraftingReagents, int>(CraftingReagents.Timber, 10) };
                case WorkOrderType.Tradepost:
                    break;
                case WorkOrderType.Alchemy:
                    return new[] { new Tuple<CraftingReagents, int>(CraftingReagents.Frostweed, 5) };
                case WorkOrderType.Engineering:
                    return new[] { new Tuple<CraftingReagents, int>(CraftingReagents.TrueIronOre, 2), new Tuple<CraftingReagents, int>(CraftingReagents.BlackrockOre, 2) };
                case WorkOrderType.Jewelcrafting:
                    return new[] { new Tuple<CraftingReagents, int>(CraftingReagents.BlackrockOre, 5) };
                case WorkOrderType.Tailoring:
                    return new[] { new Tuple<CraftingReagents, int>(CraftingReagents.SumptuousFur, 5) };
                case WorkOrderType.Leatherworking:
                    return new[] { new Tuple<CraftingReagents, int>(CraftingReagents.RawBeastHide, 5) };
                case WorkOrderType.Barn:
                    return new[] { new Tuple<CraftingReagents, int>(CraftingReagents.FurryCagedBeast, 1) };
            }

            return null;
        }

        internal static readonly List<Tuple<CraftingReagents, int>[]> BarnWorkOrderItemList = new List
            <Tuple<CraftingReagents, int>[]>
        {
            new[] {new Tuple<CraftingReagents, int>(CraftingReagents.FurryCagedBeast, 1)},
            new[] {new Tuple<CraftingReagents, int>(CraftingReagents.LeatheryCagedBeast, 1)},
            new[] {new Tuple<CraftingReagents, int>(CraftingReagents.MeatyCagedBeast, 1)},
            new[] {new Tuple<CraftingReagents, int>(CraftingReagents.CagedMightyClefthoof, 1)},
            new[] {new Tuple<CraftingReagents, int>(CraftingReagents.CagedMightyRiverbeast, 1)},
            new[] {new Tuple<CraftingReagents, int>(CraftingReagents.CagedMightyWolf, 1)},
        };

        public static int GetWorkOrderNpcEntryId(BuildingType type, bool ally)
        {
            switch (type)
            {
                case BuildingType.SalvageYard:
                    return !ally ? 79857 : 77378;
                case BuildingType.EnchantersStudy:
                    return !ally ? 79820 : 77781;
                case BuildingType.TheForge:
                    return !ally ? 79817 : 77792;
                case BuildingType.TradingPost:
                    break;
                case BuildingType.ScribesQuarters:
                    return !ally ? 79831 : 77777;
                case BuildingType.Mines:
                    return !ally ? 81688 : 77730;
                case BuildingType.HerbGarden:
                    return !ally ? 85783 : 85514;
                case BuildingType.Lumbermill:
                    return !ally ? 84247 : 84248;
                case BuildingType.AlchemyLab:
                    return !ally ? 79814 : 77791;
                case BuildingType.EngineeringWorks:
                    return !ally ? 86696 : 77831;
                case BuildingType.GemBoutique:
                    return !ally ? 79830 : 77775;
                case BuildingType.TailoringEmporium:
                    return !ally ? 79863 : 77778;
                case BuildingType.TheTannery:
                    return !ally ? 79833 : 78207;
                case BuildingType.WarMillDwarvenBunker:
                    return !ally ? 89066 : 89065;
                case BuildingType.Barn:
                    return !ally ? 85048 : 84524;
            }

            return -1;
        }

        public static long GetTotalWorkorderStartups(Tuple<CraftingReagents, int>[] currency)
        {

            if (currency == null) return 0;

            long buyableCount = 0;
            foreach (var c in currency)
            {
                List<C_WoWItem> items = Character.Player.Inventory.GetCraftingReagentsById((int)c.Item1);
                if (items.Count == 0)
                {
                    return 0;
                }
                long totalItems = items.Aggregate<C_WoWItem, long>(0, (current, i) => current + i.StackCount);
                if (totalItems < c.Item2)
                {
                    return 0;
                }

                buyableCount += totalItems / c.Item2;
            }
            return buyableCount / currency.Length;
        }
        
        #endregion
    }
}