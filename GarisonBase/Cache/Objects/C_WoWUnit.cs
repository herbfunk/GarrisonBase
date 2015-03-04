using System;
using Herbfunk.GarrisonBase.Garrison.Objects;
using Styx.CommonBot;
using Styx.WoWInternals.WoWObjects;

namespace Herbfunk.GarrisonBase.Cache
{
    public class C_WoWUnit : C_WoWObject
    {
        public readonly WoWUnit ref_WoWUnit;
        public bool IsDead { get; set; }
        public bool Lootable { get; set; }
        private bool _shouldLoot { get; set; }

        public C_WoWUnit(WoWUnit obj) : base(obj)
        {
            ref_WoWUnit = obj;
        }
        public override bool IsStillValid()
        {
            if (ref_WoWUnit == null || !ref_WoWUnit.IsValid || ref_WoWUnit.BaseAddress == IntPtr.Zero)
                return false;

            return base.IsStillValid();
        }
        public override bool RequiresUpdate
        {
            get { return _requiresUpdate; }
        }
        private bool _requiresUpdate = true;

        public override bool Update()
        {
            if (SubType == WoWObjectTypes.Unknown)
            {
                SubType = WoWObjectTypes.Unit;
                if (WorkOrder.WorkOrderNpcIds.Contains(Entry))
                    SubType = WoWObjectTypes.GarrisonWorkOrderNpc;
                else if (CacheStaticLookUp.CommandTableIds.Contains(Entry))
                {
                    SubType = WoWObjectTypes.GarrisonCommandTable;
                    _requiresUpdate = false;
                }
                else if (CacheStaticLookUp.MineQuestMobIDs.Contains(Entry) ||
                         CacheStaticLookUp.HerbQuestMobIDs.Contains(Entry))
                {
                    Targeting.Instance.TargetList.Add(ref_WoWUnit);
                    LootTargeting.Instance.TargetList.Add(ref_WoWUnit);
                    _shouldLoot = true;
                }
            }
            Location = ref_WoWUnit.Location;
            IsDead=ref_WoWUnit.IsDead;
            if (IsDead && _shouldLoot) LootTargeting.Instance.TargetList.Add(ref_WoWUnit);
            Lootable = ref_WoWUnit.Lootable;

            return true;
        }

        public override string ToString()
        {
            return String.Format("{0}\r\n" +
                                 "IsDead {1} Lootable {2}",
                                    base.ToString(),
                                    IsDead, Lootable);
        }
    }
}