using System;

namespace Herbfunk.GarrisonBase.Garrison.Enums
{
    [Flags]
    public enum WorkOrderType
    {
        None=0,
        Enchanting=1,
        Inscription=2,
        Mining=4,
        Herbalism=8,
        Blacksmith=16,
        Lumbermill=32,
        Tradepost=64,
        Alchemy=128,
        Engineering=256,
        Jewelcrafting=512,
        Tailoring=1024,
        Leatherworking=2048,
        WarMillDwarvenBunker=4096,
        DwarvenBunker = 8192,
        Barn = 16384,

        All = Enchanting|Inscription|Mining|Herbalism|Blacksmith|Lumbermill|Tradepost|Alchemy|
                Engineering|Jewelcrafting|Tailoring|Leatherworking|WarMillDwarvenBunker|Barn,
    }
}