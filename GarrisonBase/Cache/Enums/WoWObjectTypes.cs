using System;

namespace Herbfunk.GarrisonBase.Cache.Enums
{
    [Flags]
    public enum WoWObjectTypes
    {
        Unknown=0,
        Unit=1,
        OreVein=2,
        Herb=4,
        GarrisonCache=8,
        GarrisonShipment=16,
        GarrisonFinalizePlot=32,
        GarrisonCommandTable=64,
        GarrisonWorkOrderNpc=128,
        Mailbox = 256,
        PrimalTrader = 512,
        Vendor = 1024,
        Follower = 2048,
        Trappable = 4096,
        Trap = 8192,
        Timber = 16384,
    }
}
