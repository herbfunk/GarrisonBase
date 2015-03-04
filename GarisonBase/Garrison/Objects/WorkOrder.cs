using System;
using System.Collections.Generic;
using System.Linq;
using Herbfunk.GarrisonBase.Cache;
using Herbfunk.GarrisonBase.Garrison.Enums;

namespace Herbfunk.GarrisonBase.Garrison.Objects
{
    public class WorkOrder
    {
        public BuildingType BuildingType { get; set; }
        public string BuildingId { get; set; }
        public WorkOrderType Type { get; set; }
        public int Maximum { get; set; }
        public int Pending { get; set; }
        public int Pickup { get; set; }
        public Tuple<CraftingReagents, int>[] Currency { get; set; }

        public WorkOrder(string buildingId, BuildingType buildingType, WorkOrderType workorderType,
            int max, Tuple<CraftingReagents, int>[] currency, int pending = 0, int pickup = 0)
        {
            BuildingId = buildingId;
            BuildingType = buildingType;
            Type = workorderType;
            Maximum = max;
            Currency = currency;
            Pending = pending;
            Pickup = pickup;
        }

        public long GetTotalWorkorderStartups()
        {

            if (Currency == null) return 0;

            long buyableCount = 0;
            foreach (var c in Currency)
            {
                List<C_WoWItem> items = Player.Inventory.GetCraftingReagentsById((int)c.Item1);
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
            return buyableCount / Currency.Length;
        }

        public void Refresh()
        {
            WorkOrder ret = LuaCommands.GetWorkOrder(BuildingId);
            Maximum = ret.Maximum;
            Pending = ret.Pending;
            Pickup = ret.Pickup;
        }

        public override string ToString()
        {
            return String.Format("Type[{0}] Building {1} ({2}) - Max {3} Pending {4} Pickup {5}",
                Type, BuildingType, BuildingId, Maximum, Pending, Pickup);
        }

        #region Static Collections and Methods

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
            {WorkOrderType.WarMill, 000},
            {WorkOrderType.DwarvenBunker, 000},
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
            {WorkOrderType.WarMill, "War Mill Work Order"},
            {WorkOrderType.DwarvenBunker, "Dwarven Bunker Work Order"},
            {WorkOrderType.Barn, "Barn Work Order"}
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
        * 
        * Alliance
        * 87207 - ?
        * 87208 - Fireweed
        * 87209 - Frostweed
        * 87210 - Raw Beast Hide
        * 87211 - True Iron Ore
        * 87212 - ?
        * 87213 - Sumptuous Fur
        * 87214 - Starflower
        * 87215 - Nagrand Arrowbloom
        * 87216 - ?
        * 87217 - Blackrock Ore
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
                case BuildingType.Warmill:
                    return WorkOrderType.WarMill;
                case BuildingType.DwarvenBunker:
                    return WorkOrderType.DwarvenBunker;
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
            }

            return null;
        }

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
            }

            return -1;
        }
        
        #endregion
    }
}