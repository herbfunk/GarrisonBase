using System.Collections.Generic;
using System.Linq;
using Herbfunk.GarrisonBase.Cache;
using Herbfunk.GarrisonBase.Cache.Objects;
using Styx.CommonBot;

namespace Herbfunk.GarrisonBase.TargetHandling
{
    public static class TargetManager
    {
        public enum CombatFlags
        {
            Normal,
            Trapping,
        }

        #region Properties
        public static List<C_WoWObject> ValidLootableObjects = new List<C_WoWObject>();
        public static List<C_WoWUnit> ValidCombatObjects = new List<C_WoWUnit>();
       
        internal static C_WoWObject LootableObject { get; set; }
        internal static C_WoWUnit CombatObject { get; set; }

        public static bool ShouldLoot
        {
            get
            {
                if (BaseSettings.CurrentSettings.LootAnyMobs) return true;
                return _shouldLoot;
            }
            set
            {
                _shouldLoot = value;
                GarrisonBase.Debug("Targeting Should Loot set to {0}", value);
            }
        }
        private static bool _shouldLoot;

        public static bool ShouldKill
        {
            get { return _shouldKill; }
            set
            {
                _shouldKill = value;
                GarrisonBase.Debug("Targeting Should Kill set to {0}", value);
            }
        }
        private static bool _shouldKill;

        public static double KillDistance
        {
            get { return _killDistance; }
            set
            {
                _killDistance = value;
                GarrisonBase.Debug("Targeting Kill Distance set to {0}", value);
            }
        }
        private static double _killDistance = 100;

        public static double LootDistance
        {
            get { return _lootDistance; }
            set
            {
                _lootDistance = value;
                GarrisonBase.Debug("Targeting Loot Distance set to {0}", value);
            }
        }
        private static double _lootDistance = 100;

        public static double PullDistance
        {
            get { return _pullDistance; }
            set
            {
                _pullDistance = value;
                GarrisonBase.Debug("Targeting Pull Distance set to {0}", value);
            }
        }
        private static double _pullDistance = Targeting.PullDistance;

        public static CombatFlags CombatType
        {
            get { return _combatType; }
            set
            {
                _combatType = value;
                GarrisonBase.Debug("Targeting Combat Type set to {0}", value.ToString());
            }
        }
        private static CombatFlags _combatType = CombatFlags.Normal;

        #endregion

        internal static void Initalize()
        {
            Reset();
        }

        internal static void Reset()
        {
            ValidLootableObjects.Clear();
            ValidCombatObjects.Clear();
            LootableObject = null;
            CombatObject = null;
            ShouldLoot = false;
            ShouldKill = false;
            KillDistance = 100;
            LootDistance = 100;
            PullDistance = Targeting.PullDistance;
            CombatType = CombatFlags.Normal;
        }

        /// <summary>
        /// Refreshes Lootable Objects and Current Lootable Object to be used with Coroutines.LootObject() method
        /// </summary>
        internal static void UpdateLootableTarget()
        {
            if (!ShouldLoot)
            {
                LootableObject = null;
                return;
            }

            if (LootableObject != null)
            {
                //Test if lootable object should remain current..
                if (!LootableObject.IsValid || !LootableObject.IgnoredTimer.IsFinished || LootableObject.NeedsRemoved || !LootableObject.ValidForLooting)
                    LootableObject = null;
                else
                    return;
            }

            ValidLootableObjects = ObjectCacheManager.ObjectCollection.Values.
                Where(obj => obj.ValidForLooting && obj.Distance <= LootDistance).
                OrderBy(obj => obj.Distance).ToList();

            foreach (var target in ValidLootableObjects)
            {
                LootableObject = target;
                break;
            }
        }

        /// <summary>
        /// Refreshes Combat Objects and Current Combat Object to be used with Coroutines.EngageObject() method
        /// </summary>
        internal static void UpdateCombatTarget()
        {
            if (!ShouldKill)
            {
                CombatObject = null;
                return;
            }

            if (CombatObject != null)
            {
                //Test if lootable object should remain current..
                if (!CombatObject.ValidForCombat)
                    CombatObject = null;
                else
                    return;
            }

            ValidCombatObjects = ObjectCacheManager.ObjectCollection.Values.OfType<C_WoWUnit>().
                Where(obj => obj.ValidForCombat && obj.Distance <= KillDistance).
                //OrderBy(obj => obj.LineOfSight).
                OrderBy(obj => obj.Distance).ToList();

            foreach (var target in ValidCombatObjects)
            {
                CombatObject = target;
                break;
            }
        }
    }
}
