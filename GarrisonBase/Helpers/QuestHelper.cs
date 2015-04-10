using System;
using System.Collections.Generic;
using System.Linq;
using Styx.Common;
using Styx.CommonBot.Frames;
using Styx.WoWInternals;

namespace Herbfunk.GarrisonBase.Helpers
{
    public static class QuestHelper
    {
        public static bool QuestFrameOpen { get; set; }
        public static bool GossipFrameOpen { get; set; }
        public static QuestFrameTypes QuestFrameType { get; set; }
        public static QuestLogChangeTypes QuestLogChangeType { get; set; }

        public static Dictionary<uint, CPlayerQuest> QuestLog = new Dictionary<uint, CPlayerQuest>();


        static QuestHelper()
        {
            LuaEvents.OnQuestLogUpdate += RefreshQuestLog;
            LuaEvents.OnQuestRemoved += () =>  QuestLogChangeType = QuestLogChangeTypes.Removed;
            LuaEvents.OnQuestWatchUpdate += () => QuestLogChangeType = QuestLogChangeTypes.Update;
            LuaEvents.OnQuestAccepted += () =>
            {
                QuestLogChangeType = QuestLogChangeTypes.Accepted;
                QuestFrameOpen = false;
            };
            LuaEvents.OnQuestDetail += () =>
            {
                QuestFrameType = QuestFrameTypes.Detail;
                QuestFrameOpen = true;
            };
            LuaEvents.OnQuestProgress += () =>
            {
                QuestFrameType = QuestFrameTypes.Progress;
                QuestFrameOpen = true;
            };
            LuaEvents.OnQuestComplete += () =>
            {
                QuestFrameType = QuestFrameTypes.Complete;
                QuestFrameOpen = true;
            };
            LuaEvents.OnQuestFinished += () =>
            {
                QuestFrameType = QuestFrameTypes.None;
                QuestFrameOpen = false;
            };


           
            //
        }


        internal static void RefreshQuestLog()
        {
            QuestLog.Clear();
            foreach (var q in Styx.WoWInternals.QuestLog.Instance.GetAllQuests())
            {
                QuestLog.Add(q.Id, new CPlayerQuest(q));
            }
        }

        public static bool QuestLogFull
        {
            get { return QuestLog.Count >= 25; }
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
            foreach (var q in Bots.Quest.QuestManager.GossipFrame.ActiveQuests)
            {
                if (q.Id==id)
                    return q.Index;
            }

            return -1;
        }
        public static int GetAvailableQuestIndexFromGossipFrame(uint id)
        {
            foreach (var q in Bots.Quest.QuestManager.GossipFrame.AvailableQuests)
            {
                if (q.Id == id)
                    return q.Index;
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


        #region Class and Enums

        public enum QuestFrameTypes
        {
            None,
            Detail,
            Progress,
            Complete
        }

        public enum QuestLogChangeTypes
        {
            None,
            Accepted,
            Removed,
            Update
        }

        public class CPlayerQuest
        {
            public uint Id { get; set; }
            public string Name { get; set; }
            public bool IsCompleted { get; set; }

            public CPlayerQuest(PlayerQuest quest)
            {
                Id = quest.Id;
                Name = quest.Name;
                IsCompleted = quest.IsCompleted;
            }

            public void Update(PlayerQuest quest)
            {
                if (quest.Id != Id) return;

                IsCompleted = quest.IsCompleted;
            }

            public override string ToString()
            {
                return String.Format("Name {0} Id {1} Completed {2}",
                                        Name, Id, IsCompleted);
            }
        }

        
        #endregion



        /* Daily Town Hall Trader Info
         * 
         * Requires level 3 Town Hall
         * 
         *  Leather
         *      QuestId: 38287
         *      NpcId: 
         *          Horde: 91033
         *          Alliance: 91024
         *      Cost: 50 raw beast hide
         *      
         * 
         *  Fur
         *      QuestId: 38293
         *      NpcId: 
         *          Horde: 91034
         *          Alliance: 91025
         *      Cost: 50 Scrumptous Fur
         *      
         * 
         *  Dust
         *      QuestId: 38290
         *      NpcId: 
         *          Horde: 91029
         *          Alliance: 91020
         *      Cost: 50 Draenic Dust
         *      
         * 
         *  Herbs
         *      QuestId: 38296
         *      NpcId: 
         *          Horde: 91031
         *          Alliance: 91404
         *      Cost: 10 Fireweed, 10 Starflower, 10 Talador Orchid, 10 Gorgond Flytrap, 10 Nagrand Arrowbloom
         *      
         * 
         *  Ore
         *      QuestId: 38243
         *      NpcId: 
         *          Horde: 91030
         *          Alliance: 90894
         *      Cost: 25 Blackrock, 25 True Iron
         */


        /* Daily Quest Info
         * 
         *      
         * 
         * 
         * Warmill (Scrap Meltdown)
         *   Requires Level 3
         *   QuestId: Alliance: 38175 Horde: 38188
         *   NpcId: Alliance: 77377 Horde: 79815
         *   Cost 25 Iron horde scraps (113681)
         *   Rewards follower token Weapon (120302) , follower token Armor (120301) 
         * 
         * Alchemy (Alchemy Experiment)
         *  Requires Assigned Follower Working
         *  QuestId: 37270
         *  NpcId: Assigned Follower
         *  Rewards: Agility (122453), Invisibility (122451), Intellect (122454), Swiftness (122452), Strength (122455), Armor (122456)
         * 
         */
    }
}
