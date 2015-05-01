using System;
using Herbfunk.GarrisonBase.Cache.Enums;
using Herbfunk.GarrisonBase.Character;
using Herbfunk.GarrisonBase.Garrison.Objects;
using Herbfunk.GarrisonBase.Helpers;
using Herbfunk.GarrisonBase.TargetHandling;
using Styx;
using Styx.WoWInternals;
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


        public uint Flags { get; set; }
        public bool IsEvadeRunningBack { get; set; }
        public bool CanSelect { get; set; }
        public bool InCombat { get; set; }
        public bool IsDead { get; set; }
        public bool Attackable { get; set; }
        public bool Lootable { get; set; }
        public bool Skinnable { get; set; }
        public bool ShouldSkin { get; set; }
        public bool CanInteract { get; set; }
        public bool IsTagged { get; set; }
        public bool TaggedByMe { get; set; }
        public WoWGuid TargetGuid { get; set; }

        public bool IsTargetingMe
        {
            get { return TargetGuid == Player.PlayerGuid; }
        }
        public QuestGiverStatus QuestGiverStatus = QuestGiverStatus.None;
        public bool Trappable
        {
            get
            {
                if (!_trappable.HasValue)
                    _trappable = CacheStaticLookUp.TrappingEntryIds.Contains(Entry);

                return _trappable.Value;
            }
        }
        private bool? _trappable;
        public uint? MaxHealth { get; set; }
        public uint CurrentHealth { get; set; }
        public double CurrentHealthPercent { get; set; }
        public double LastCurrentHealthPercent { get; set; }

        public C_WoWUnit(WoWUnit obj)
            : base(obj)
        {
            RefWoWUnit = obj;
            TargetGuid = obj.CurrentTargetGuid;
            switch (SubType)
            {
                case WoWObjectTypes.GarrisonCommandTable:
                    InteractRange = 4.55f;
                    IgnoresRemoval = true;
                    return;
                case WoWObjectTypes.GarrisonWorkOrderNpc:
                    InteractRange = 5.4f;
                    IgnoresRemoval = true;
                    return;
                case WoWObjectTypes.PrimalTrader:
                    IgnoresRemoval = true;
                    return;
                case WoWObjectTypes.Vendor:
                    IgnoresRemoval = Player.InsideGarrison;
                    return;
                case WoWObjectTypes.Trap:
                    Location.Normalize();
                    return;
            }

            if (ObjectCacheManager.QuestNpcIds.Contains(Entry))
            {
                InteractRange = 5.4f;
                IsQuestNpc = true;
            }

            if (TargetManager.CheckLootFlag(TargetManager.LootFlags.Units) || ObjectCacheManager.LootIds.Contains(Entry))
            {
                ShouldLoot = true;
            }

            if (ObjectCacheManager.CombatIds.Contains(Entry))
            {
                ShouldKill = true;
            }
        }


        public override bool Update()
        {
            if (!base.Update()) return false;


            Flags = RefWoWUnit.Flags;
            Location = RefWoWUnit.Location;
            IsDead = RefWoWUnit.IsDead;
            UpdateHitPoints();



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

                if (TargetManager.CheckLootFlag(TargetManager.LootFlags.Skinning))
                {
                    Skinnable = RefWoWUnit.Skinnable;
                }
            }
            else
            {
                TargetGuid = RefWoWUnit.CurrentTargetGuid;

                if (ShouldKill)
                {
                    InCombat = RefWoWUnit.Combat;
                    CanSelect = RefWoWUnit.CanSelect;
                    IsEvadeRunningBack = RefWoWUnit.IsEvadeRunningBack;
                    Attackable = RefWoWUnit.Attackable;
                    TaggedByMe = RefWoWUnit.TaggedByMe;
                    IsTagged = RefWoWUnit.IsTagged;
                }
            }

            return true;
        }

        ///<summary>
        ///This only updates hitpoints every 5th call to help reduce CPU!
        ///</summary>
        public bool UpdateHitPoints()
        {
            //Last Target skips the counter checks
            if (TargetManager.CombatObject != null && Equals(TargetManager.CombatObject))
            {
                UpdateCurrentHitPoints();
                return true;
            }


            _healthChecks++;

            if (_healthChecks > 6)
                _healthChecks = 1;

            if (_healthChecks == 1)
                UpdateCurrentHitPoints();

            return true;
        }
        private int _healthChecks = 0;

        private void UpdateCurrentHitPoints()
        {
            uint currentHealth;


            try
            {
                currentHealth = RefWoWUnit.CurrentHealth;
            }
            catch
            {
                GarrisonBase.Debug("Failure to get hitpoint current {0}", this.ToString());
                //Logger.Write(LogLevel.Cache, "Failure to get hitpoint current {0}", DebugStringSimple);
                //CurrentHealthPct = null;
                return;
            }

            CurrentHealth = currentHealth;

            if (!MaxHealth.HasValue)
            {
                try
                {
                    MaxHealth = RefWoWUnit.MaxHealth;
                }
                catch
                {
                    //NeedsRemoved = true;
                    GarrisonBase.Debug("Failure to get max hitpoints {0}", this.ToString());
                    return;
                }
            }

            var dCurrentHealthPct = CurrentHealth / (float)MaxHealth.Value;
            CurrentHealthPercent = dCurrentHealthPct;

            //if (dCurrentHealthPct != CurrentHealthPercent)
            //{
            //    LastCurrentHealthPercent = CurrentHealthPercent;
            //    CurrentHealthPercent = dCurrentHealthPct;
            //}
        }

        public override bool ValidForLooting
        {
            get
            {
                if (!base.ValidForTargeting) return false;

                if (!IsDead) return false;

                if ((!ShouldLoot || !TargetManager.CheckLootFlag(TargetManager.LootFlags.Units) || !Lootable) &&
                    (!ShouldSkin || !TargetManager.CheckLootFlag(TargetManager.LootFlags.Skinning) || !Skinnable))
                {
                    return false;
                }

                if (!LineOfSight) return false;

                return true;
            }
        }

        public override bool ValidForCombat
        {
            get
            {
                if (!base.ValidForCombat) return false;

                if (IsDead || !CanSelect || !Attackable) return false;

                if (!ShouldKill || !LineOfSight) return false;

                if (InCombat && IsTagged && !TaggedByMe) return false;

                return true;
            }
        }




        public override string ToString()
        {
            return String.Format("{0}\r\n" +
                                 "Lootable {2} ShouldLoot {3}\r\n" +
                                 "ShouldKill {6} Attackable {7} IsDead {1} InCombat{15}\r\n" +
                                 "IsQuestNPC {8} CanInteract {4} QuestGiverStatus {5}\r\n" +
                                 "Flags {9} IsEvadingRunningBack {10}\r\n" +
                                 "CurrHPs {11} MaxHPs {12} CurrHP {13}% LastHP {14}",
                                    base.ToString(),
                                    IsDead, Lootable, ShouldLoot,
                                    CanInteract, QuestGiverStatus,
                                    ShouldKill, Attackable, IsQuestNpc,
                                    Flags.ToString(), IsEvadeRunningBack,
                                    CurrentHealth,
                                    MaxHealth.HasValue ? MaxHealth.Value.ToString() : "?",
                                    CurrentHealthPercent,
                                    LastCurrentHealthPercent.ToString(),
                                    InCombat);
        }
    }
}