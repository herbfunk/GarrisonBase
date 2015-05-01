using System;
using Herbfunk.GarrisonBase.Cache.Enums;
using Herbfunk.GarrisonBase.Cache.Static;
using Herbfunk.GarrisonBase.Garrison.Objects;
using Herbfunk.GarrisonBase.Helpers;
using Styx;
using Styx.WoWInternals.WoWObjects;

namespace Herbfunk.GarrisonBase.Cache.Objects
{
    public class EntryBase
    {
        public EntryBase(WoWObject obj)
        {
            EntryBase outValue;
            if (ObjectCacheManager.EntryCache.TryGetValue(obj.Entry, out outValue))
            {
                Entry = outValue.Entry;
                Name = outValue.Name;
                Type = outValue.Type;
                SubType = outValue.SubType;
            }
            else
            {
                Entry = obj.Entry;
                Name = obj.Name;

                ObjectEntryInfo outInfo;
                if (EntryCache.ObjectEntries.TryGetValue(Entry, out outInfo))
                {
                    Type = outInfo.Type;
                    SubType = outInfo.SubType;
                }
                else
                {
                    Type = obj.Type;
                    UpdateSubType(obj);
                }

                ObjectCacheManager.EntryCache.Add(this);
            }
        }

        public uint Entry { get; set; }
        public string Name { get; set; }
        public WoWObjectType Type { get; set; }
        public WoWObjectTypes SubType { get; set; }

        private void UpdateSubType(WoWObject obj)
        {
            SubType = WoWObjectTypes.Unknown;

            switch (Type)
            {
                case WoWObjectType.Unit:
                    {
                        WoWUnit unitobj = ((WoWUnit)obj);
                        if (unitobj.IsVendor)
                        {
                            SubType |= WoWObjectTypes.Vendor;
                        }

                        break;
                    }

                case WoWObjectType.GameObject:
                    {
                        WoWGameObject gameobj = ((WoWGameObject)obj);
                        WoWGameObjectType subtype = gameobj.SubType;
                        if (subtype == WoWGameObjectType.GarrisonShipment)
                        {
                            SubType = WoWObjectTypes.GarrisonShipment;
                        }
                        else if (subtype == WoWGameObjectType.Mailbox)
                        {
                            SubType = WoWObjectTypes.Mailbox;
                        }

                        break;
                    }
            }
        }

        public override string ToString()
        {
            return String.Format("Entry {0} Name {1} Type {2} SubType {3}\r\n",
                Entry, Name, Type, SubType);
        }
    }


}
