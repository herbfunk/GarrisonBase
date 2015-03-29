using System;
using System.Collections.Generic;
using System.Linq;
using Herbfunk.GarrisonBase.Cache.Enums;
using Herbfunk.GarrisonBase.Cache.Objects;
using Styx;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace Herbfunk.GarrisonBase.Cache
{
    public static class ObjectCacheManager
    {
        [Flags]
        public enum ObjectFlags
        {
            None=0,
            Combat=1,
            Loot=2,
            Quest=4,
        }

        public static void Initalize()
        {
            LootIds = new EntryList();
            LootIds.OnItemAdded += OnLootIdAdded;
            LootIds.OnItemRemoved += OnLootIdRemoved;

            CombatIds = new EntryList();
            CombatIds.OnItemAdded += OnCombatIdAdded;
            CombatIds.OnItemRemoved += OnCombatIdRemoved;

            QuestNpcIds = new EntryList();
            QuestNpcIds.OnItemAdded += OnQuestNpcIdAdded;
            QuestNpcIds.OnItemRemoved += OnQuestNpcIdRemoved;

            _shouldLoot = false;
            _shouldKill = false;
        }

        public static List<C_WoWObject> ValidLootableObjects = new List<C_WoWObject>();
        public static List<C_WoWUnit> ValidCombatObjects = new List<C_WoWUnit>();
        internal static C_WoWObject LootableObject { get; set; }
        internal static C_WoWUnit CombatObject { get; set; }
        internal static bool FoundOreObject { get; set; }
        internal static bool FoundHerbObject { get; set; }
       
        internal static bool CheckFlag(WoWObjectTypes property, WoWObjectTypes flag)
        {
            return (property & flag) != 0;
        }
        internal static CacheCollection ObjectCollection = new CacheCollection();
        private static DateTime _lastUpdatedCacheCollection = DateTime.Today;
        internal static bool ShouldUpdateObjectCollection
        {
            get
            {
                return DateTime.Now.Subtract(_lastUpdatedCacheCollection).TotalMilliseconds > 150;
            }
        }

        public static bool ShouldLoot
        {
            get { return _shouldLoot; }
            set
            {
                _shouldLoot = value;
                GarrisonBase.Debug("Cache Should Loot set to {0}", value);
            }
        }
        private static bool _shouldLoot;

        public static bool ShouldKill
        {
            get { return _shouldKill; }
            set
            {
                _shouldKill = value;
                GarrisonBase.Debug("Cache Should Kill set to {0}", value);
            }
        }
        private static bool _shouldKill;

        public static double KillDistance
        {
            get { return _killDistance; }
            set
            {
                _killDistance = value;
                GarrisonBase.Debug("Cache Kill Distance set to {0}", value);
            }
        }
        private static double _killDistance = 100;

        public static double LootDistance
        {
            get { return _lootDistance; }
            set
            {
                _lootDistance = value;
                GarrisonBase.Debug("Cache Loot Distance set to {0}", value);
            }
        }
        private static double _lootDistance = 100;

        public static EntryList LootIds { get; set; }
        internal static void OnLootIdAdded(uint item)
        {
            foreach (var obj in GetWoWObjects(item))
            {
                obj.ShouldLoot = true;
            }
        }
        internal static void OnLootIdRemoved(uint item)
        {
            foreach (var obj in GetWoWObjects(item))
            {
                obj.ShouldLoot = false;
            }
        }

        public static EntryList CombatIds { get; set; }
        internal static void OnCombatIdAdded(uint item)
        {
            foreach (var obj in GetWoWObjects(item))
            {
                obj.ShouldKill = true;
            }
        }
        internal static void OnCombatIdRemoved(uint item)
        {
            foreach (var obj in GetWoWObjects(item))
            {
                obj.ShouldKill = false;
            }
        }

        public static EntryList QuestNpcIds { get; set; }
        internal static void OnQuestNpcIdAdded(uint item)
        {
            foreach (var obj in GetWoWObjects(item))
            {
                obj.IsQuestNpc = true;
            }
        }
        internal static void OnQuestNpcIdRemoved(uint item)
        {
            foreach (var obj in GetWoWObjects(item))
            {
                obj.IsQuestNpc = false;
            }
        }


        public static void ResetCache()
        {
            _updateLoopCounter = 0;

            LootableObject = null;
            CombatObject = null;
            
            QuestNpcIds.Clear();
            CombatIds.Clear();
            LootIds.Clear();

            _shouldKill = false;
            _shouldLoot = false;
            _killDistance = 100;
            _lootDistance = 100;
            FoundOreObject = false;
            FoundHerbObject = false;

            ValidLootableObjects.Clear();
            ValidCombatObjects.Clear();
            ObjectCollection.Clear();

            BlacklistedGuids.Clear();
            BlacklistedEntryIds.Clear();

        }

        public static void AddObjectToEntryList(uint id, ObjectFlags flags)
        {
            if (flags.HasFlag(ObjectFlags.Combat))
                CombatIds.Add(id);
            if (flags.HasFlag(ObjectFlags.Loot))
                LootIds.Add(id);
            if (flags.HasFlag(ObjectFlags.Quest))
                QuestNpcIds.Add(id);
        }

        private static int _updateLoopCounter;
        private static readonly List<WoWGuid> BlacklistedGuids=new List<WoWGuid>();
        private static readonly List<uint> BlacklistedEntryIds = new List<uint>(); 
        internal static void UpdateCache()
        {
            Character.Player.Update();

            FoundOreObject = false;
            FoundHerbObject = false;

            //GarrisonBase.Debug("Updating Object Cache");
            List<WoWGuid> GuidsSeenThisLoop = new List<WoWGuid>();
            using (StyxWoW.Memory.AcquireFrame())
            {
                ObjectManager.Update();
                foreach (var obj in ObjectManager.ObjectList)
                {
                    uint tmp_Entry = obj.Entry;
                    if (BlacklistedEntryIds.Contains(tmp_Entry)) continue;
                    if (Blacklist.BlacklistEntryIDs.Contains(tmp_Entry))
                    {
                        //Styx.CommonBot.Blacklist.Add(obj, BlacklistFlags.All, new TimeSpan(1, 0, 0, 0), "Perma Blacklist");
                        continue;
                    }

                    WoWGuid tmp_Guid = obj.Guid;
                    if (GuidsSeenThisLoop.Contains(tmp_Guid)) continue;
                    GuidsSeenThisLoop.Add(tmp_Guid);

                    if (BlacklistedGuids.Contains(tmp_Guid)) continue;

                    C_WoWObject wowObj;
                    if (!ObjectCollection.TryGetValue(tmp_Guid, out wowObj))
                    {
                        //Create new object!
                        if (obj.Type == WoWObjectType.Unit)
                        {
                            C_WoWUnit objUnit = new C_WoWUnit(obj.ToUnit());
                            ObjectCollection.Add(tmp_Guid, objUnit);
                            wowObj = ObjectCollection[tmp_Guid];
                        }
                        else if (obj.Type == WoWObjectType.GameObject)
                        {
                            WoWGameObject gameObject = obj.ToGameObject();
                            if (CacheStaticLookUp.BlacklistedGameObjectTypes.Contains(gameObject.SubType))
                            {
                                BlacklistedEntryIds.Add(tmp_Entry);
                                continue;
                            }
                            C_WoWGameObject objGame = new C_WoWGameObject(gameObject);
                            ObjectCollection.Add(tmp_Guid, objGame);
                            wowObj = ObjectCollection[tmp_Guid];
                        }
                        else
                        {
                            //Don't care for anything else!
                            BlacklistedEntryIds.Add(tmp_Entry);
                            continue;
                        }
                    }

                    if (!wowObj.IsValid && wowObj.IgnoresRemoval)
                    {
                        wowObj.UpdateReference(obj);
                    }

                    wowObj.LoopsUnseen = 0;
                    if (wowObj.RequiresUpdate) wowObj.Update();
                }
            }

            //Tally up unseen objects.
            var unseenObjects = ObjectCollection.Keys.Where(o => !GuidsSeenThisLoop.Contains(o)).ToList();
            if (unseenObjects.Any())
            {
                for (int i = 0; i < unseenObjects.Count(); i++)
                {
                    ObjectCollection[unseenObjects[i]].LoopsUnseen++;
                }
            }

            //Trim our collection every 5th refresh.
            _updateLoopCounter++;
            if (_updateLoopCounter > 4)
            {
                _updateLoopCounter = 0;
                //Now flag any objects not seen for 5 loops. Gold/Globe only 1 loop.
                foreach (var item in ObjectCollection.Values.Where(CO => CO.LoopsUnseen >= 5 && !CO.IgnoresRemoval))
                {
                    item.NeedsRemoved = true;
                }

                CheckForCacheRemoval();
            }
            
            //if (_shouldLoot) UpdateLootableTarget();
            //if (_shouldKill) UpdateCombatTarget();

            _lastUpdatedCacheCollection = DateTime.Now;
        }

        /// <summary>
        /// Refreshes Lootable Objects and Current Lootable Object to be used with Coroutines.LootObject() method
        /// </summary>
        internal static void UpdateLootableTarget()
        {
            if (!_shouldLoot)
            {
                LootableObject = null;
                return;
            }

            if (LootableObject != null)
            {
                //Test if lootable object should remain current..
                if (!LootableObject.IsValid || !LootableObject.IgnoredTimer.IsFinished || LootableObject.NeedsRemoved)
                    LootableObject = null;
                else
                    return;
            }

            ValidLootableObjects = ObjectCollection.Values.Where(obj => obj.ValidForLooting && obj.Distance<=LootDistance).OrderBy(obj => obj.Distance).ToList();

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
            if (!_shouldKill)
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

            ValidCombatObjects = ObjectCollection.Values.OfType<C_WoWUnit>().Where(obj => obj.ValidForCombat && obj.Distance<=KillDistance).OrderBy(obj => obj.Distance).ToList();

            foreach (var target in ValidCombatObjects)
            {
                CombatObject = target;
                break;
            }
        }

        ///<summary>
        ///Used to flag when Init should iterate and remove the objects
        ///</summary>
        internal static bool RemovalCheck = false;
        internal static void CheckForCacheRemoval()
        {
            //Check Cached Object Removal flag
            if (RemovalCheck)
            {
                //Remove flagged objects
                var RemovalObjs = (from objs in ObjectCollection.Values
                                   where objs.NeedsRemoved 
                                   select objs.Guid).ToList();

                foreach (var item in RemovalObjs)
                {
                    C_WoWObject thisObj = ObjectCollection[item];
                    //Blacklist flag check
                    if (thisObj.BlacklistType == BlacklistType.Entry)
                    {
                        BlacklistedEntryIds.Add(thisObj.Entry);
                    }
                    else if (thisObj.BlacklistType == BlacklistType.Guid)
                    {
                        BlacklistedGuids.Add(thisObj.Guid);
                    }

                    ObjectCollection.Remove(thisObj.Guid);
                }

                RemovalCheck = false;
            }
        }
        internal static List<WoWGuid> BlacklistGuiDs = new List<WoWGuid>();

       

        public static C_WoWObject GetWoWObject(uint entryId)
        {
            var ret = ObjectCollection.Values.FirstOrDefault(obj => obj.Entry == entryId && !BlacklistGuiDs.Contains(obj.Guid));
            return ret;
        }
        public static C_WoWObject GetWoWObject(int entryId)
        {
            var ret = ObjectCollection.Values.FirstOrDefault(obj => obj.Entry == entryId && !BlacklistGuiDs.Contains(obj.Guid));
            return ret;
        }
        public static C_WoWObject GetWoWObject(string name)
        {
            var ret = ObjectCollection.Values.FirstOrDefault(obj => obj.Name == name && !BlacklistGuiDs.Contains(obj.Guid));
            return ret;
        }

        public static List<C_WoWObject> GetWoWObjects(params uint[] args)
        {
            var ids = new List<uint>(args);
            return ObjectCollection.Values.Where(obj => ids.Contains(obj.Entry) && !BlacklistGuiDs.Contains(obj.Guid)).ToList();
        }
        public static List<C_WoWObject> GetWoWObjects(WoWObjectTypes type)
        {
            return ObjectCollection.Values.Where(obj => obj.SubType==type && !BlacklistGuiDs.Contains(obj.Guid)).ToList();
        }
        public static List<C_WoWObject> GetWoWObjects(int id)
        {
            return ObjectCollection.Values.Where(obj => obj.Entry == id && !BlacklistGuiDs.Contains(obj.Guid)).ToList();
        }
        public static List<C_WoWObject> GetWoWObjects(string name)
        {
            return ObjectCollection.Values.Where(obj => obj.Name == name && !BlacklistGuiDs.Contains(obj.Guid)).ToList();
        }
        public static List<C_WoWGameObject> GetWoWGameObjects(params uint[] args)
        {
            var ids = new List<uint>(args);
            return
                ObjectCollection.Values.OfType<C_WoWGameObject>()
                    .Where(obj => ids.Contains(obj.Entry) && !BlacklistGuiDs.Contains(obj.Guid) && obj.IsValid)
                    .ToList();
        }
        public static List<C_WoWGameObject> GetWoWGameObjects(WoWObjectTypes type)
        {
            return
                ObjectCollection.Values.OfType<C_WoWGameObject>()
                    .Where(obj => obj.SubType == type && !BlacklistGuiDs.Contains(obj.Guid) && obj.IsValid)
                    .ToList();
        }
        public static List<C_WoWGameObject> GetWoWGameObjects(int id)
        {
            return
                ObjectCollection.Values.OfType<C_WoWGameObject>()
                    .Where(obj => obj.Entry == id && !BlacklistGuiDs.Contains(obj.Guid) && obj.IsValid)
                    .ToList();
        }
        public static List<C_WoWGameObject> GetWoWGameObjects(string name)
        {
            return
                ObjectCollection.Values.OfType<C_WoWGameObject>()
                    .Where(obj => obj.Name == name && !BlacklistGuiDs.Contains(obj.Guid) && obj.IsValid)
                    .ToList();
        }
        public static List<C_WoWUnit> GetWoWUnits(params uint[] args)
        {
            var ids = new List<uint>(args);
            return
                ObjectCollection.Values.OfType<C_WoWUnit>()
                    .Where(obj => ids.Contains(obj.Entry) && !BlacklistGuiDs.Contains(obj.Guid))
                    .ToList();
        }
        public static List<C_WoWUnit> GetWoWUnits(int id)
        {
            return
                ObjectCollection.Values.OfType<C_WoWUnit>()
                    .Where(obj => obj.Entry == id && !BlacklistGuiDs.Contains(obj.Guid) && obj.IsValid)
                    .ToList();
        }
        public static List<C_WoWUnit> GetWoWUnits(WoWObjectTypes type)
        {
            return
                ObjectCollection.Values.OfType<C_WoWUnit>()
                    .Where(obj => obj.SubType == type && !BlacklistGuiDs.Contains(obj.Guid) && obj.IsValid)
                    .ToList();
        }
        public static List<C_WoWUnit> GetWoWUnits(string name)
        {
            return
                ObjectCollection.Values.OfType<C_WoWUnit>()
                    .Where(obj => obj.Name == name && !BlacklistGuiDs.Contains(obj.Guid) && obj.IsValid)
                    .ToList();
        }
        public static List<C_WoWUnit> GetUnitsNearPoint(WoWPoint location, float maxdistance, bool validOnly = true)
        {
            return
                ObjectCollection.Values.OfType<C_WoWUnit>()
                    .Where(obj => location.Distance(obj.Location) <= maxdistance && (!validOnly || obj.IsValid))
                    .ToList();
        }
        public static List<C_WoWObject> GetObjectsNearPoint(WoWPoint location, float maxdistance, bool validOnly=true)
        {
            return
                ObjectCollection.Values
                    .Where(obj => location.Distance(obj.Location) <= maxdistance && (!validOnly || obj.IsValid))
                    .ToList();
        }
    }
}
