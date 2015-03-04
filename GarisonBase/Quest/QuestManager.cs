using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Herbfunk.GarrisonBase.Quest.Objects;
using Styx.Common;
using Styx.CommonBot.Frames;

namespace Herbfunk.GarrisonBase.Quest
{
    public static class QuestManager
    {
        public static Dictionary<uint, CPlayerQuest> QuestLog = new Dictionary<uint, CPlayerQuest>();
        public static GossipFrameInfo GossipFrameInfo = new GossipFrameInfo();

        internal static void RefreshQuestLog()
        {
            QuestLog.Clear();
            foreach (var q in Styx.WoWInternals.QuestLog.Instance.GetAllQuests())
            {
                QuestLog.Add(q.Id, new CPlayerQuest(q));
            }
        }

        public static bool QuestLogFull()
        {
            return QuestLog.Count >= 25;
        }
        public static bool QuestContainedInQuestLog(uint id)
        {
            return QuestLog.Keys.Any(q => q == id);
        }
        public static CPlayerQuest GetQuestFromQuestLog(uint id)
        {
            if (QuestLog.ContainsKey(id))
                return QuestLog[id];
            return null;
        }

        public static int GetActiveQuestIndexFromGossipFrame(uint id)
        {
            foreach (var q in GossipFrameInfo.ActiveQuests)
            {
                if (q.Key == id)
                    return q.Value.Index;
            }

            return -1;
        }
        public static int GetAvailableQuestIndexFromGossipFrame(uint id)
        {
            foreach (var q in GossipFrameInfo.AvailableQuests)
            {
                if (q.Key == id)
                    return q.Value.Index;
            }

            return -1;
        }
        public static int GetQuestIndexFromQuestFrame(uint id)
        {

            foreach (var q in QuestFrame.Instance.AvailableQuests)
            {
                Logging.Write("{0}", q.Id);
                if (q.Id == id)
                    return q.Index;
            }

            return -1;
        }
    }
}
