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

        public WoWUnit RefWoWUnit;
        public override void UpdateReference(WoWObject obj)
        {
            base.UpdateReference(obj);
            RefWoWUnit = obj.ToUnit();
        }

        public bool IsDead { get; set; }
        public bool Attackable { get; set; }
        public bool Lootable { get; set; }


        public C_WoWUnit(WoWUnit obj)
            : base(obj)
        {
            RefWoWUnit = obj;

            if (SubType == WoWObjectTypes.Unknown)
            {
                SubType = WoWObjectTypes.Unit;
                if (WorkOrder.WorkOrderNpcIds.Contains(Entry))
                {
                    SubType = WoWObjectTypes.GarrisonWorkOrderNpc;
                    InteractRange = 5.4f;
                    IgnoresRemoval = true;
                }
                else if (CacheStaticLookUp.CommandTableIds.Contains(Entry))
                {
                    SubType = WoWObjectTypes.GarrisonCommandTable;
                    InteractRange = 4.55f;
                    IgnoresRemoval = true;
                }
                else if (CacheStaticLookUp.MineQuestMobIDs.Contains(Entry) ||
                         CacheStaticLookUp.HerbQuestMobIDs.Contains(Entry))
                {
                    InteractRange = 5f;
                    ShouldLoot = true;
                    ShouldKill = true;
                }
                else if (Entry == GarrisonManager.PrimalTraderID)
                {
                    SubType = WoWObjectTypes.PrimalTrader;
                    IgnoresRemoval = true;
                }
                else if (Entry == GarrisonManager.SellRepairNpcId)
                {
                    SubType= WoWObjectTypes.RepairVendor;
                    IgnoresRemoval = true;
                }
                else if (ObjectCacheManager.KillableEntryIds.Contains(Entry))
                {
                    ShouldKill = true;
                    ShouldLoot = true;
                }
            }
        }

  
        public override bool Update()
        {
            if (!base.Update()) return false;
            
            

            Location = RefWoWUnit.Location;
            IsDead=RefWoWUnit.IsDead;


            if (IsDead)
            {
                if (ShouldLoot)
                {
                    Lootable = RefWoWUnit.Lootable;
                }
            }
            else if(ShouldKill)
            {
                Attackable = RefWoWUnit.Attackable;
            }

            return true;
        }

        public override bool ValidForLooting
        {
            get
            {
                if (!base.ValidForTargeting) return false;

                if (IsDead && !Lootable) return false;

                if (!ShouldLoot || !LineOfSight) return false;

                return true;
            }
        }

        public override bool ValidForCombat
        {
            get
            {
                if (!base.ValidForCombat) return false;

                if (IsDead || !Attackable) return false;

                if (!ShouldKill || !LineOfSight) return false;

                return true;
            }
        }


        public override string ToString()
        {
            return String.Format("{0}\r\n" +
                                 "IsDead {1} Lootable {2} ShouldLoot {3}",
                                    base.ToString(),
                                    IsDead, Lootable, ShouldLoot);
        }
    }
}