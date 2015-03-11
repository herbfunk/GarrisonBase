using System;
using Herbfunk.GarrisonBase.Cache.Enums;
using Herbfunk.GarrisonBase.Garrison.Objects;
using Styx;
using Styx.Common.Helpers;
using Styx.WoWInternals.WoWObjects;

namespace Herbfunk.GarrisonBase.Cache.Objects
{
    public class C_WoWGameObject : C_WoWObject
    {
        public C_WoWGameObject(WoWGameObject obj)
            : base(obj)
        {
            ref_WoWGameObject = obj;
            GameObjectType = obj.SubType;

            if (SubType == WoWObjectTypes.Unknown)
            {
                if (CacheStaticLookUp.FinalizeGarrisonPlotIds.Contains(Entry))
                    SubType = WoWObjectTypes.GarrisonFinalizePlot;
                else if (CacheStaticLookUp.MineDeposits.Contains(Entry))
                {
                    SubType = WoWObjectTypes.OreVein;
                    _shouldLoot = true;
                    InteractRange = 6f;
                    LineofSightWaitTimer = new WaitTimer(new TimeSpan(0, 0, 0, 2, 500));
                    LineofSightWaitTimer.Stop();
                }
                else if (CacheStaticLookUp.HerbDeposits.Contains(Entry))
                {
                    SubType = WoWObjectTypes.Herb;
                    _shouldLoot = true;
                    InteractRange = 5f;
                }
                else if (CacheStaticLookUp.CommandTableIds.Contains(Entry))
                    SubType = WoWObjectTypes.GarrisonCommandTable;
                else if (CacheStaticLookUp.ResourceCacheIds.Contains(Entry))
                    SubType = WoWObjectTypes.GarrisonCache;
                else if (WorkOrder.WorkOrderPickupNames.ContainsValue(Name))
                    SubType = WoWObjectTypes.GarrisonWorkOrder;
                else if (CacheStaticLookUp.GarrisonsMailboxIds.Contains(Entry))
                {
                    SubType = WoWObjectTypes.Mailbox;
                    InteractRange = 6f;
                }
                else
                {
                    //Don't update objects we don't know..
                    RequiresUpdate = false;
                }
            }
        }

        public readonly WoWGameObject ref_WoWGameObject;
        public WoWGameObjectType GameObjectType { get; set; }

        private bool _shouldLoot = false;
        public override bool Update()
        {
            base.Update();

            if (SubType == WoWObjectTypes.Herb) ObjectCacheManager.FoundHerbObject = true;
            else if (SubType == WoWObjectTypes.OreVein) ObjectCacheManager.FoundOreObject = true;

            return true;
        }

        public override bool ValidForTargeting
        {
            get
            {
                if (!base.ValidForTargeting) return false;

                if (!_shouldLoot || (!LineOfSight)) return false;


                return true;
            }
        }


        public WoWCursorType GetCursor
        {
            get
            {
                if (!IsValid) return WoWCursorType.None;
                return ref_WoWGameObject.GetCursor();
            }
        }
        public override string ToString()
        {
            return String.Format("{0}\r\n" +
                                 "ShouldLoot {1}",
                                    base.ToString(),
                                    _shouldLoot);
        }

    }
}