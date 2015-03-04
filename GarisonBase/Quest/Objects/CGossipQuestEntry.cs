using Styx.CommonBot.Frames;

namespace Herbfunk.GarrisonBase.Quest.Objects
{
    public class CGossipQuestEntry
    {
        public int Id { get; set; }
        public int Index { get; set; }
        public string Name { get; set; }

        public CGossipQuestEntry(GossipQuestEntry entry)
        {
            Id = entry.Id;
            Index = entry.Index;
            Name = entry.Name;
        }
    }
}