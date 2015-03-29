using System.Collections.Generic;
using Styx.CommonBot.Frames;

namespace Herbfunk.GarrisonBase.Quest.Objects
{
    public abstract class FrameInfo
    {
        public Dictionary<int, CGossipQuestEntry> ActiveQuests = new Dictionary<int, CGossipQuestEntry>();
        public Dictionary<int, CGossipQuestEntry> AvailableQuests = new Dictionary<int, CGossipQuestEntry>();
        public List<CGossipEntry> GossipOptionEntries = new List<CGossipEntry>();

        protected FrameInfo(QuestFrame frame)
        {
            ActiveQuests.Clear();
            foreach (var q in frame.ActiveQuests)
            {
                ActiveQuests.Add(q.Id, new CGossipQuestEntry(q));
            }
            AvailableQuests.Clear();
            foreach (var q in frame.AvailableQuests)
            {
                AvailableQuests.Add(q.Id, new CGossipQuestEntry(q));
            }
        }
        protected FrameInfo(GossipFrame frame)
        {
            
            ActiveQuests.Clear();
            foreach (var q in frame.ActiveQuests)
            {
                ActiveQuests.Add(q.Id, new CGossipQuestEntry(q));
            }
            AvailableQuests.Clear();
            foreach (var q in frame.AvailableQuests)
            {
                AvailableQuests.Add(q.Id, new CGossipQuestEntry(q));
            }

            GossipOptionEntries.Clear();

            if (frame.GossipOptionEntries != null)
            {
                foreach (var g in frame.GossipOptionEntries)
                {
                    GossipOptionEntries.Add(new CGossipEntry(g));
                }
            }



        }

        protected FrameInfo() { }
    }
}