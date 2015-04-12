using System;
using Herbfunk.GarrisonBase.Cache.Enums;
using Herbfunk.GarrisonBase.Garrison.Objects;
using Herbfunk.GarrisonBase.Helpers;
using Styx;
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
        public bool CanInteract { get; set; }
        public QuestGiverStatus QuestGiverStatus= QuestGiverStatus.None;

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
                else if (CacheStaticLookUp.PrimalTraderIds.Contains(Entry))
                {
                    SubType = WoWObjectTypes.PrimalTrader | WoWObjectTypes.Vendor;
                    IgnoresRemoval = true;
                }

                if (MerchantHelper.GarrisonVendorEntryIds.Contains(Entry))
                {
                    if (SubType== WoWObjectTypes.Unknown)
                        SubType = WoWObjectTypes.Vendor;
                    else
                        SubType |= WoWObjectTypes.Vendor;

                    IgnoresRemoval = true;
                }

                if (ObjectCacheManager.QuestNpcIds.Contains(Entry))
                {
                    InteractRange = 5.4f;
                    IsQuestNpc = true;
                }

                if (ObjectCacheManager.LootIds.Contains(Entry))
                {
                    ShouldLoot = true;
                }

                if (ObjectCacheManager.CombatIds.Contains(Entry))
                {
                    ShouldKill = true;
                }

            }
        }

  
        public override bool Update()
        {
            if (!base.Update()) return false;
            
            

            Location = RefWoWUnit.Location;
            IsDead=RefWoWUnit.IsDead;

            if (ObjectCacheManager.IsQuesting || IsQuestNpc)
            {
                CanInteract = RefWoWUnit.CanInteract;
                QuestGiverStatus = RefWoWUnit.QuestGiverStatus;
            }
           
            
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

                if (!IsDead || !Lootable) return false;

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
                                 "Lootable {2} ShouldLoot {3}\r\n" +
                                 "ShouldKill {6} Attackable {7} IsDead {1}\r\n" +
                                 "IsQuestNPC {8} CanInteract {4} QuestGiverStatus {5}",
                                    base.ToString(),
                                    IsDead, Lootable, ShouldLoot,
                                    CanInteract, QuestGiverStatus,
                                    ShouldKill, Attackable, IsQuestNpc);
        }
    }
}