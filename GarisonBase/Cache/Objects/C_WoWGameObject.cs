using Herbfunk.GarrisonBase.Garrison.Objects;
using Styx;
using Styx.WoWInternals.WoWObjects;

namespace Herbfunk.GarrisonBase.Cache
{
    public class C_WoWGameObject : C_WoWObject
    {
        public readonly WoWGameObject ref_WoWGameObject;
        public WoWGameObjectType GameObjectType { get; set; }

        private bool _shouldLoot = false;
        public override bool Update()
        {
            if (SubType == WoWObjectTypes.Unknown)
            {
                if (CacheStaticLookUp.FinalizeGarrisonPlotIds.Contains(Entry))
                    SubType = WoWObjectTypes.GarrisonFinalizePlot;
                else if (CacheStaticLookUp.MineDeposits.Contains(Entry))
                {
                    SubType = WoWObjectTypes.OreVein;
                    _shouldLoot = true;
                    LootDistance = 6f;
                }
                else if (CacheStaticLookUp.HerbDeposits.Contains(Entry))
                {
                    SubType = WoWObjectTypes.Herb;
                    _shouldLoot = true;
                    LootDistance = 5f;
                }
                else if (CacheStaticLookUp.CommandTableIds.Contains(Entry))
                    SubType = WoWObjectTypes.GarrisonCommandTable;
                else if (CacheStaticLookUp.ResourceCacheIds.Contains(Entry))
                    SubType = WoWObjectTypes.GarrisonCache;
                else if (WorkOrder.WorkOrderPickupNames.ContainsValue(Name))
                    SubType= WoWObjectTypes.GarrisonWorkOrder;
                else if (CacheStaticLookUp.GarrisonsMailboxIds.Contains(Entry))
                {
                    SubType = WoWObjectTypes.Mailbox;
                }
                if (!_shouldLoot)
                    RequiresUpdate = false;
            }

            if (_shouldLoot)
            {
                if (ObjectCacheManager.ShouldLoot)
                {
                    if (ObjectCacheManager.LootableObject == null ||
                        (!ObjectCacheManager.LootableObject.Equals(this) &&
                        ObjectCacheManager.LootableObject.CentreDistance > CentreDistance))
                    {
                        if (ref_WoWGameObject.InLineOfSight)
                            ObjectCacheManager.LootableObject = this;
                    }
                }
            }

            return true;
        }

        public C_WoWGameObject(WoWGameObject obj) : base(obj)
        {
            ref_WoWGameObject = obj;
            GameObjectType = obj.SubType;

           
        }

        public WoWCursorType GetCursor
        {
            get
            {
                if (!IsStillValid()) return WoWCursorType.None;
                return ref_WoWGameObject.GetCursor();
            }
        }


    }
}