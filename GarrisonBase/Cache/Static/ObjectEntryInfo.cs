using Herbfunk.GarrisonBase.Cache.Enums;
using Styx;

namespace Herbfunk.GarrisonBase.Cache.Static
{
    public class ObjectEntryInfo
    {
        public WoWObjectType Type { get; set; }
        public WoWObjectTypes SubType { get; set; }

        public ObjectEntryInfo()
        {
            Type= WoWObjectType.None;
            SubType= WoWObjectTypes.Unknown;
        }

        public ObjectEntryInfo(WoWObjectType type, WoWObjectTypes subtype)
        {
            Type = type;
            SubType = subtype;
        }
    }
}
