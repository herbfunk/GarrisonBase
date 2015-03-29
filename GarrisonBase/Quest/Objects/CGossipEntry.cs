using Styx.CommonBot.Frames;

namespace Herbfunk.GarrisonBase.Quest.Objects
{
    public class CGossipEntry
    {
        public string Text { get; set; }
        public int Index { get; set; }
        public GossipEntry.GossipEntryType Type { get; set; }
        public CGossipEntry(GossipEntry entry)
        {
            Index = entry.Index;
            Type = entry.Type;
            Text = entry.Text;
        }
    }
}
