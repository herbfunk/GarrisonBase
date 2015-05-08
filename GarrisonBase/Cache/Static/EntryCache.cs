using System.Collections.Generic;
using Herbfunk.GarrisonBase.Cache.Enums;
using Styx;

namespace Herbfunk.GarrisonBase.Cache.Static
{
    public static class EntryCache
    {
        public static readonly Dictionary<uint, ObjectEntryInfo> ObjectEntries = new Dictionary<uint, ObjectEntryInfo>
        {
            #region Herbs
		    {235390, new ObjectEntryInfo(WoWObjectType.GameObject, WoWObjectTypes.Herb)},
            {235388, new ObjectEntryInfo(WoWObjectType.GameObject, WoWObjectTypes.Herb)},
            {235391, new ObjectEntryInfo(WoWObjectType.GameObject, WoWObjectTypes.Herb)},
            {235387, new ObjectEntryInfo(WoWObjectType.GameObject, WoWObjectTypes.Herb)},
            {235389, new ObjectEntryInfo(WoWObjectType.GameObject, WoWObjectTypes.Herb)},
            {235376, new ObjectEntryInfo( WoWObjectType.GameObject, WoWObjectTypes.Herb)}, 
	        #endregion

            #region Ore
            {232544, new ObjectEntryInfo( WoWObjectType.GameObject, WoWObjectTypes.OreVein)},
            {232542, new ObjectEntryInfo( WoWObjectType.GameObject, WoWObjectTypes.OreVein)},
            {232541, new ObjectEntryInfo( WoWObjectType.GameObject, WoWObjectTypes.OreVein)},
            {232543, new ObjectEntryInfo( WoWObjectType.GameObject, WoWObjectTypes.OreVein)},
            {232545, new ObjectEntryInfo( WoWObjectType.GameObject, WoWObjectTypes.OreVein)},
	        #endregion

            #region Timber
            {233635, new ObjectEntryInfo( WoWObjectType.GameObject, WoWObjectTypes.Timber)},
            {233634, new ObjectEntryInfo( WoWObjectType.GameObject, WoWObjectTypes.Timber)},
            {233604, new ObjectEntryInfo( WoWObjectType.GameObject, WoWObjectTypes.Timber)},
	        #endregion

            #region Traps
            {83709, new ObjectEntryInfo( WoWObjectType.Unit, WoWObjectTypes.Trap)},
            {83925, new ObjectEntryInfo( WoWObjectType.Unit, WoWObjectTypes.Trap)},
            {84773, new ObjectEntryInfo( WoWObjectType.Unit, WoWObjectTypes.Trap)},
            {84774, new ObjectEntryInfo( WoWObjectType.Unit, WoWObjectTypes.Trap)},
            
	        #endregion

            #region GARRISON Command Table
            {85805, new ObjectEntryInfo(WoWObjectType.Unit, WoWObjectTypes.GarrisonCommandTable)},
            {86031, new ObjectEntryInfo(WoWObjectType.Unit, WoWObjectTypes.GarrisonCommandTable)},
            {84224, new ObjectEntryInfo(WoWObjectType.Unit, WoWObjectTypes.GarrisonCommandTable)},
            {84698, new ObjectEntryInfo(WoWObjectType.Unit, WoWObjectTypes.GarrisonCommandTable)},
            #endregion

            #region GARRISON Resource Cache
            {237191, new ObjectEntryInfo( WoWObjectType.GameObject, WoWObjectTypes.GarrisonCache)},
            {236916, new ObjectEntryInfo( WoWObjectType.GameObject, WoWObjectTypes.GarrisonCache)},
            {237720, new ObjectEntryInfo( WoWObjectType.GameObject, WoWObjectTypes.GarrisonCache)},
            {237722, new ObjectEntryInfo( WoWObjectType.GameObject, WoWObjectTypes.GarrisonCache)},
            {237724, new ObjectEntryInfo( WoWObjectType.GameObject, WoWObjectTypes.GarrisonCache)},
            {237723, new ObjectEntryInfo( WoWObjectType.GameObject, WoWObjectTypes.GarrisonCache)},
            #endregion

            #region GARRISON Primal Traders
            {84967, new ObjectEntryInfo(WoWObjectType.Unit, WoWObjectTypes.PrimalTrader)},
            {84246, new ObjectEntryInfo(WoWObjectType.Unit, WoWObjectTypes.PrimalTrader)},
            #endregion

            #region GARRISON Work Order Npc
            {79857, new ObjectEntryInfo( WoWObjectType.Unit, WoWObjectTypes.GarrisonWorkOrderNpc | WoWObjectTypes.Vendor)},
            {77378, new ObjectEntryInfo( WoWObjectType.Unit, WoWObjectTypes.GarrisonWorkOrderNpc | WoWObjectTypes.Vendor)},
            {79820, new ObjectEntryInfo( WoWObjectType.Unit, WoWObjectTypes.GarrisonWorkOrderNpc)},
            {77781, new ObjectEntryInfo( WoWObjectType.Unit, WoWObjectTypes.GarrisonWorkOrderNpc)},
            {79817, new ObjectEntryInfo( WoWObjectType.Unit, WoWObjectTypes.GarrisonWorkOrderNpc)},
            {77792, new ObjectEntryInfo( WoWObjectType.Unit, WoWObjectTypes.GarrisonWorkOrderNpc)},
            {79831, new ObjectEntryInfo( WoWObjectType.Unit, WoWObjectTypes.GarrisonWorkOrderNpc)},
            {77777, new ObjectEntryInfo( WoWObjectType.Unit, WoWObjectTypes.GarrisonWorkOrderNpc)},
            {81688, new ObjectEntryInfo( WoWObjectType.Unit, WoWObjectTypes.GarrisonWorkOrderNpc)},
            {77730, new ObjectEntryInfo( WoWObjectType.Unit, WoWObjectTypes.GarrisonWorkOrderNpc)},
            {85783, new ObjectEntryInfo( WoWObjectType.Unit, WoWObjectTypes.GarrisonWorkOrderNpc)},
            {85514, new ObjectEntryInfo( WoWObjectType.Unit, WoWObjectTypes.GarrisonWorkOrderNpc)},
            {84247, new ObjectEntryInfo( WoWObjectType.Unit, WoWObjectTypes.GarrisonWorkOrderNpc)},
            {84248, new ObjectEntryInfo( WoWObjectType.Unit, WoWObjectTypes.GarrisonWorkOrderNpc)},
            {79814, new ObjectEntryInfo( WoWObjectType.Unit, WoWObjectTypes.GarrisonWorkOrderNpc)},
            {77791, new ObjectEntryInfo( WoWObjectType.Unit, WoWObjectTypes.GarrisonWorkOrderNpc)},
            {86696, new ObjectEntryInfo( WoWObjectType.Unit, WoWObjectTypes.GarrisonWorkOrderNpc)},
            {77831, new ObjectEntryInfo( WoWObjectType.Unit, WoWObjectTypes.GarrisonWorkOrderNpc)},
            {79830, new ObjectEntryInfo( WoWObjectType.Unit, WoWObjectTypes.GarrisonWorkOrderNpc)},
            {77775, new ObjectEntryInfo( WoWObjectType.Unit, WoWObjectTypes.GarrisonWorkOrderNpc)},
            {79863, new ObjectEntryInfo( WoWObjectType.Unit, WoWObjectTypes.GarrisonWorkOrderNpc)},
            {77778, new ObjectEntryInfo( WoWObjectType.Unit, WoWObjectTypes.GarrisonWorkOrderNpc)},
            {79833, new ObjectEntryInfo( WoWObjectType.Unit, WoWObjectTypes.GarrisonWorkOrderNpc)},
            {78207, new ObjectEntryInfo( WoWObjectType.Unit, WoWObjectTypes.GarrisonWorkOrderNpc)},
            {89066, new ObjectEntryInfo( WoWObjectType.Unit, WoWObjectTypes.GarrisonWorkOrderNpc)}, 
            {89065, new ObjectEntryInfo( WoWObjectType.Unit, WoWObjectTypes.GarrisonWorkOrderNpc)},
            {85048, new ObjectEntryInfo( WoWObjectType.Unit, WoWObjectTypes.GarrisonWorkOrderNpc)},
            {84524, new ObjectEntryInfo( WoWObjectType.Unit, WoWObjectTypes.GarrisonWorkOrderNpc)},


            //Alliance Trade Post
            {87207, new ObjectEntryInfo( WoWObjectType.Unit, WoWObjectTypes.GarrisonWorkOrderNpc)},
            {87208, new ObjectEntryInfo( WoWObjectType.Unit, WoWObjectTypes.GarrisonWorkOrderNpc)},
            {87209, new ObjectEntryInfo( WoWObjectType.Unit, WoWObjectTypes.GarrisonWorkOrderNpc)},
            {87210, new ObjectEntryInfo( WoWObjectType.Unit, WoWObjectTypes.GarrisonWorkOrderNpc)},
            {87211, new ObjectEntryInfo( WoWObjectType.Unit, WoWObjectTypes.GarrisonWorkOrderNpc)},
            {87212, new ObjectEntryInfo( WoWObjectType.Unit, WoWObjectTypes.GarrisonWorkOrderNpc)},
            {87213, new ObjectEntryInfo( WoWObjectType.Unit, WoWObjectTypes.GarrisonWorkOrderNpc)},
            {87214, new ObjectEntryInfo( WoWObjectType.Unit, WoWObjectTypes.GarrisonWorkOrderNpc)},
            {87215, new ObjectEntryInfo( WoWObjectType.Unit, WoWObjectTypes.GarrisonWorkOrderNpc)},
            {87216, new ObjectEntryInfo( WoWObjectType.Unit, WoWObjectTypes.GarrisonWorkOrderNpc)},
            {87217, new ObjectEntryInfo( WoWObjectType.Unit, WoWObjectTypes.GarrisonWorkOrderNpc)},

            //Horde Trade Post
            {86803, new ObjectEntryInfo( WoWObjectType.Unit, WoWObjectTypes.GarrisonWorkOrderNpc)},
            {87112, new ObjectEntryInfo( WoWObjectType.Unit, WoWObjectTypes.GarrisonWorkOrderNpc)},
            {87113, new ObjectEntryInfo( WoWObjectType.Unit, WoWObjectTypes.GarrisonWorkOrderNpc)},
            {87114, new ObjectEntryInfo( WoWObjectType.Unit, WoWObjectTypes.GarrisonWorkOrderNpc)},
            {87115, new ObjectEntryInfo( WoWObjectType.Unit, WoWObjectTypes.GarrisonWorkOrderNpc)},
            {87116, new ObjectEntryInfo( WoWObjectType.Unit, WoWObjectTypes.GarrisonWorkOrderNpc)},
            {87117, new ObjectEntryInfo( WoWObjectType.Unit, WoWObjectTypes.GarrisonWorkOrderNpc)},
            {87118, new ObjectEntryInfo( WoWObjectType.Unit, WoWObjectTypes.GarrisonWorkOrderNpc)},
            {87119, new ObjectEntryInfo( WoWObjectType.Unit, WoWObjectTypes.GarrisonWorkOrderNpc)},
            {87120, new ObjectEntryInfo( WoWObjectType.Unit, WoWObjectTypes.GarrisonWorkOrderNpc)},
            {87121, new ObjectEntryInfo( WoWObjectType.Unit, WoWObjectTypes.GarrisonWorkOrderNpc)},

            #endregion

            #region GARRISON Finalize Plot
            {231217, new ObjectEntryInfo( WoWObjectType.GameObject, WoWObjectTypes.GarrisonFinalizePlot)},
            {231964, new ObjectEntryInfo( WoWObjectType.GameObject, WoWObjectTypes.GarrisonFinalizePlot)},
            {233248, new ObjectEntryInfo( WoWObjectType.GameObject, WoWObjectTypes.GarrisonFinalizePlot)},
            {233249, new ObjectEntryInfo( WoWObjectType.GameObject, WoWObjectTypes.GarrisonFinalizePlot)},
            {233250, new ObjectEntryInfo( WoWObjectType.GameObject, WoWObjectTypes.GarrisonFinalizePlot)},
            {233251, new ObjectEntryInfo( WoWObjectType.GameObject, WoWObjectTypes.GarrisonFinalizePlot)},
            {232651, new ObjectEntryInfo( WoWObjectType.GameObject, WoWObjectTypes.GarrisonFinalizePlot)},
            {232652, new ObjectEntryInfo( WoWObjectType.GameObject, WoWObjectTypes.GarrisonFinalizePlot)},
            {236261, new ObjectEntryInfo( WoWObjectType.GameObject, WoWObjectTypes.GarrisonFinalizePlot)},
            {236262, new ObjectEntryInfo( WoWObjectType.GameObject, WoWObjectTypes.GarrisonFinalizePlot)},
            {236263, new ObjectEntryInfo( WoWObjectType.GameObject, WoWObjectTypes.GarrisonFinalizePlot)},
            {236175, new ObjectEntryInfo( WoWObjectType.GameObject, WoWObjectTypes.GarrisonFinalizePlot)},
            {236176, new ObjectEntryInfo( WoWObjectType.GameObject, WoWObjectTypes.GarrisonFinalizePlot)},
            {236177, new ObjectEntryInfo( WoWObjectType.GameObject, WoWObjectTypes.GarrisonFinalizePlot)},
            {236185, new ObjectEntryInfo( WoWObjectType.GameObject, WoWObjectTypes.GarrisonFinalizePlot)},
            {236186, new ObjectEntryInfo( WoWObjectType.GameObject, WoWObjectTypes.GarrisonFinalizePlot)},
            {236187, new ObjectEntryInfo( WoWObjectType.GameObject, WoWObjectTypes.GarrisonFinalizePlot)},
            {236188, new ObjectEntryInfo( WoWObjectType.GameObject, WoWObjectTypes.GarrisonFinalizePlot)},
            {236190, new ObjectEntryInfo( WoWObjectType.GameObject, WoWObjectTypes.GarrisonFinalizePlot)},
            {236191, new ObjectEntryInfo( WoWObjectType.GameObject, WoWObjectTypes.GarrisonFinalizePlot)},
            {236192, new ObjectEntryInfo( WoWObjectType.GameObject, WoWObjectTypes.GarrisonFinalizePlot)},
            {236193, new ObjectEntryInfo( WoWObjectType.GameObject, WoWObjectTypes.GarrisonFinalizePlot)},
            #endregion
        };
    }
}
