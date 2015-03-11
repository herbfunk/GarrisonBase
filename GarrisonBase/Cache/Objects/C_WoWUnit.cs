using System;
using System.Windows.Shapes;
using Herbfunk.GarrisonBase.Cache.Enums;
using Herbfunk.GarrisonBase.Garrison;
using Herbfunk.GarrisonBase.Garrison.Objects;
using Styx.CommonBot;
using Styx.WoWInternals.WoWObjects;

namespace Herbfunk.GarrisonBase.Cache.Objects
{
    public class C_WoWUnit : C_WoWObject
    {
        public C_WoWUnit(WoWUnit obj)
            : base(obj)
        {
            ref_WoWUnit = obj;

            if (SubType == WoWObjectTypes.Unknown)
            {
                SubType = WoWObjectTypes.Unit;
                if (WorkOrder.WorkOrderNpcIds.Contains(Entry))
                {
                    SubType = WoWObjectTypes.GarrisonWorkOrderNpc;
                    InteractRange = 5.4f;
                }
                else if (CacheStaticLookUp.CommandTableIds.Contains(Entry))
                {
                    SubType = WoWObjectTypes.GarrisonCommandTable;
                    InteractRange = 4.55f;
                }
                else if (CacheStaticLookUp.MineQuestMobIDs.Contains(Entry) ||
                         CacheStaticLookUp.HerbQuestMobIDs.Contains(Entry))
                {
                    Targeting.Instance.TargetList.Add(ref_WoWUnit);
                    LootTargeting.Instance.TargetList.Add(ref_WoWUnit);
                    InteractRange = 5f;
                    _shouldLoot = true;
                }
                else if (Entry == GarrisonManager.PrimalTraderID)
                {
                    SubType = WoWObjectTypes.PrimalTrader;
                }
            }
        }

        public readonly WoWUnit ref_WoWUnit;
        public bool IsDead { get; set; }
        public bool Lootable { get; set; }
        private bool _shouldLoot { get; set; }




        public override bool Update()
        {
            base.Update();
            Location = ref_WoWUnit.Location;
            IsDead=ref_WoWUnit.IsDead;


            if (IsDead)
            {
                if (_shouldLoot)
                {
                    Lootable = ref_WoWUnit.Lootable;
                }
                else
                {
                    RequiresUpdate = false;
                }
            }

            return true;
        }

        public override bool ValidForTargeting
        {
            get
            {
                if (!base.ValidForTargeting) return false;

                if (IsDead && !Lootable) return false;

                if (!_shouldLoot || !LineOfSight) return false;

                return true;
            }
        }

        public override string ToString()
        {
            return String.Format("{0}\r\n" +
                                 "IsDead {1} Lootable {2} ShouldLoot {3}",
                                    base.ToString(),
                                    IsDead, Lootable, _shouldLoot);
        }
    }
}