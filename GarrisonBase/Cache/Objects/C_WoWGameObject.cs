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
        public WoWGameObject RefWoWGameObject;
        public override void UpdateReference(WoWObject obj)
        {
            base.UpdateReference(obj);
            RefWoWGameObject = obj.ToGameObject();
        }

        public WoWGameObjectType GameObjectType { get; set; }


        public C_WoWGameObject(WoWGameObject obj)
            : base(obj)
        {
            RefWoWGameObject = obj;
            GameObjectType = obj.SubType;

            if (SubType == WoWObjectTypes.Unknown)
            {
                if (CacheStaticLookUp.FinalizeGarrisonPlotIds.Contains(Entry))
                    SubType = WoWObjectTypes.GarrisonFinalizePlot;
                else if (CacheStaticLookUp.MineDeposits.Contains(Entry))
                {
                    SubType = WoWObjectTypes.OreVein;
                    ShouldLoot = true;
                    IgnoresRemoval = true;
                    InteractRange = 6f;
                    LineofSightWaitTimer = new WaitTimer(new TimeSpan(0, 0, 0, 2, 500));
                    LineofSightWaitTimer.Stop();
                }
                else if (CacheStaticLookUp.HerbDeposits.Contains(Entry))
                {
                    SubType = WoWObjectTypes.Herb;
                    ShouldLoot = true;
                    IgnoresRemoval = true;
                    InteractRange = 5f;
                }
                else if (CacheStaticLookUp.CommandTableIds.Contains(Entry))
                {
                    SubType = WoWObjectTypes.GarrisonCommandTable;
                    IgnoresRemoval = true;
                }
                else if (CacheStaticLookUp.ResourceCacheIds.Contains(Entry))
                {
                    SubType = WoWObjectTypes.GarrisonCache;
                    IgnoresRemoval = true;
                }
                else if (WorkOrder.WorkOrderPickupNames.ContainsValue(Name))
                {
                    SubType = WoWObjectTypes.GarrisonWorkOrder;
                    InteractRange = 5.8f;
                    IgnoresRemoval = true;
                }
                else if (CacheStaticLookUp.GarrisonsMailboxIds.Contains(Entry))
                {
                    SubType = WoWObjectTypes.Mailbox;
                    InteractRange = 6f;
                    IgnoresRemoval = true;
                }
                else if (CacheStaticLookUp.StoreHouseQuestIDs.Contains(Entry))
                {
                    ShouldLoot = true;
                    InteractRange = 5f;
                }
                else if (ObjectCacheManager.LootableEntryIds.Contains(Entry))
                {
                    ShouldLoot = true;
                }
                else
                {
                    //Don't update objects we don't know..
                    RequiresUpdate = false;
                }
            }
        }



        public override bool Update()
        {
            if (!base.Update()) return false;

            if (SubType == WoWObjectTypes.Herb) ObjectCacheManager.FoundHerbObject = true;
            else if (SubType == WoWObjectTypes.OreVein) ObjectCacheManager.FoundOreObject = true;

            return true;
        }

        public override bool ValidForLooting
        {
            get
            {
                if (!base.ValidForTargeting) return false;

                if (!ShouldLoot || !LineOfSight) return false;

                return true;
            }
        }

        public WoWCursorType GetCursor
        {
            get
            {
                if (!IsValid) return WoWCursorType.None;
                return RefWoWGameObject.GetCursor();
            }
        }


        public override string ToString()
        {
            return String.Format("{0}\r\n" +
                                 "ShouldLoot {1}",
                                    base.ToString(),
                                    ShouldLoot);
        }

    }
}