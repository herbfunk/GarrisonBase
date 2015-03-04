using System;
using System.Collections.Generic;
using System.Linq;
using Styx;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace Herbfunk.GarrisonBase.Cache
{
    public static class ObjectCacheManager
    {
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
        private static int UpdateLoopCounter;
        private static List<WoWGuid> _blacklistedGuids=new List<WoWGuid>();
        private static List<uint> _blacklistedEntryIds = new List<uint>(); 
        internal static void UpdateCache()
        {
            Player.Update();

            List<WoWGuid> GuidsSeenThisLoop = new List<WoWGuid>();
            using (StyxWoW.Memory.AcquireFrame())
            {
                ObjectManager.Update();
                foreach (var obj in ObjectManager.ObjectList)
                {
                    WoWGuid tmp_Guid = obj.Guid;
                    if (GuidsSeenThisLoop.Contains(tmp_Guid)) continue;
                    GuidsSeenThisLoop.Add(tmp_Guid);

                    if (_blacklistedGuids.Contains(tmp_Guid)) continue;

                    uint tmp_Entry = obj.Entry;
                    if (_blacklistedEntryIds.Contains(tmp_Entry)) continue;

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
                                _blacklistedEntryIds.Add(tmp_Entry);
                                continue;
                            }
                            C_WoWGameObject objGame = new C_WoWGameObject(gameObject);
                            ObjectCollection.Add(tmp_Guid, objGame);
                            wowObj = ObjectCollection[tmp_Guid];
                        }
                        else
                        {
                            //Don't care for anything else!
                            _blacklistedEntryIds.Add(tmp_Entry);
                            continue;
                        }
                    }

                    if (!wowObj.IsStillValid())
                    {
                        wowObj.NeedsRemoved = true;
                        wowObj.BlacklistType= BlacklistType.Guid;
                        continue;
                    }

                    wowObj.LoopsUnseen = 0;
                    if (wowObj.RequiresUpdate)
                        wowObj.Update();
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
            UpdateLoopCounter++;
            if (UpdateLoopCounter > 4)
            {
                UpdateLoopCounter = 0;
                //Now flag any objects not seen for 5 loops. Gold/Globe only 1 loop.
                foreach (var item in ObjectCollection.Values.Where(CO =>
                    CO.LoopsUnseen >= 5))
                {
                    item.NeedsRemoved = true;
                }

                CheckForCacheRemoval();
            }

            _lastUpdatedCacheCollection = DateTime.Now;
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
                        _blacklistedEntryIds.Add(thisObj.Entry);
                    }
                    else if (thisObj.BlacklistType == BlacklistType.Guid)
                    {
                        _blacklistedGuids.Add(thisObj.Guid);
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
                    .Where(obj => ids.Contains(obj.Entry) && !BlacklistGuiDs.Contains(obj.Guid) && obj.IsStillValid())
                    .ToList();
        }
        public static List<C_WoWGameObject> GetWoWGameObjects(WoWObjectTypes type)
        {
            return
                ObjectCollection.Values.OfType<C_WoWGameObject>()
                    .Where(obj => obj.SubType == type && !BlacklistGuiDs.Contains(obj.Guid) && obj.IsStillValid())
                    .ToList();
        }
        public static List<C_WoWGameObject> GetWoWGameObjects(int id)
        {
            return
                ObjectCollection.Values.OfType<C_WoWGameObject>()
                    .Where(obj => obj.Entry == id && !BlacklistGuiDs.Contains(obj.Guid) && obj.IsStillValid())
                    .ToList();
        }
        public static List<C_WoWGameObject> GetWoWGameObjects(string name)
        {
            return
                ObjectCollection.Values.OfType<C_WoWGameObject>()
                    .Where(obj => obj.Name == name && !BlacklistGuiDs.Contains(obj.Guid) && obj.IsStillValid())
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
                    .Where(obj => obj.Entry == id && !BlacklistGuiDs.Contains(obj.Guid) && obj.IsStillValid())
                    .ToList();
        }
        public static List<C_WoWUnit> GetWoWUnits(WoWObjectTypes type)
        {
            return
                ObjectCollection.Values.OfType<C_WoWUnit>()
                    .Where(obj => obj.SubType == type && !BlacklistGuiDs.Contains(obj.Guid) && obj.IsStillValid())
                    .ToList();
        }
        public static List<C_WoWUnit> GetWoWUnits(string name)
        {
            return
                ObjectCollection.Values.OfType<C_WoWUnit>()
                    .Where(obj => obj.Name == name && !BlacklistGuiDs.Contains(obj.Guid) && obj.IsStillValid())
                    .ToList();
        }
    }
}
