using System;
using System.Collections.Generic;
using System.Linq;
using Bots.Quest;
using Herbfunk.GarrisonBase.Cache;
using Herbfunk.GarrisonBase.Character;
using Herbfunk.GarrisonBase.Coroutines;
using Herbfunk.GarrisonBase.Coroutines.Behaviors;
using Herbfunk.GarrisonBase.Garrison;
using Herbfunk.GarrisonBase.Garrison.Enums;
using Herbfunk.GarrisonBase.Garrison.Objects;
using Styx;
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
            foreach (var q in QuestManager.GossipFrame.ActiveQuests)
            {
                if (q.Id==id)
                    return q.Index;
            }

            return -1;
        }
        public static int GetAvailableQuestIndexFromGossipFrame(uint id)
        {
            foreach (var q in QuestManager.GossipFrame.AvailableQuests)
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
            public List<uint> RewardIds { get; set; } 
            public CPlayerQuest(PlayerQuest quest)
            {
                
                Id = quest.Id;
                Name = quest.Name;
                IsCompleted = quest.IsCompleted;
                RewardIds = new List<uint>();
                foreach (var choice in quest.GetRewardChoices())
                {
                    RewardIds.Add(choice.ItemId);
                }
            }

            public void Update(PlayerQuest quest)
            {
                if (quest.Id != Id) return;

                IsCompleted = quest.IsCompleted;
            }

            public override string ToString()
            {
                string rewards = RewardIds.Aggregate("", (current, id) => current + id.ToString() + "\t");
                return String.Format("Name {0} Id {1} Completed {2} Rewards {3}",
                                        Name, Id, IsCompleted, rewards);
            }
        }

        
        #endregion

        internal static BehaviorArray GetDailyQuestArray(uint questid, bool alliance)
        {
            switch (questid)
            {
                case 38175:
                case 38188:
                {
                    var questNpcId = alliance ? 77377 : 79815;
                    var warmillBunker = GarrisonManager.Buildings[BuildingType.WarMillDwarvenBunker];
                    var questPickup =  new BehaviorQuestPickup(questid, warmillBunker.SpecialMovementPoints[1], questNpcId);
                    var questTurnin = new BehaviorQuestTurnin(questid, warmillBunker.SpecialMovementPoints[1], questNpcId, BaseSettings.CurrentSettings.DailyWarMillQuestSettings.RewardIndex);
                    var barray = new BehaviorArray(new Behavior[]
                    {
                        questPickup,
                        questTurnin,
                    });
                    barray.Criteria += () => BaseSettings.CurrentSettings.BehaviorQuests && 
                        BaseSettings.CurrentSettings.DailyWarMillQuestSettings.Enabled &&
                        BaseSettings.CurrentSettings.DailyWarMillQuestSettings.RewardIndex > -1 &&
                        !LuaCommands.IsQuestFlaggedCompleted(questid.ToString());

                    barray.Criteria += () =>
                    {
                        var items=Player.Inventory.GetBagItemsById(113681).Where(i => i.StackCount>24).ToList();
                        return items.Count > 0;
                    };

                    return barray;
                }
                case 37270:
                {
                    var alchemyLab = GarrisonManager.Buildings[BuildingType.AlchemyLab];

                    var questPickup = new BehaviorQuestPickup(
                        questid,
                        alchemyLab.EntranceMovementPoint,
                        0,
                        true,
                        BaseSettings.CurrentSettings.DailyAlchemyLabQuestSettings.RewardIndex);

                    //Use special method of getting the interaction object since it varies on which follower is assigned!
                    questPickup.GetInteractionObject = i =>
                    {
                        var validObjects =
                            ObjectCacheManager.GetUnitsNearPoint(alchemyLab.EntranceMovementPoint, 30f, false)
                                .Where(u => u.QuestGiverStatus == QuestGiverStatus.AvailableRepeatable && !ObjectCacheManager.QuestNpcIds.Contains(u.Entry))
                                .ToList();

                        return validObjects.Count > 0 ? validObjects[0] : null;
                    };

                    var barray = new BehaviorArray(new Behavior[]
                    {
                        questPickup,
                    });
                    barray.Criteria += () => BaseSettings.CurrentSettings.BehaviorQuests && 
                        BaseSettings.CurrentSettings.DailyAlchemyLabQuestSettings.Enabled && 
                        BaseSettings.CurrentSettings.DailyAlchemyLabQuestSettings.RewardIndex > -1 &&
                        !LuaCommands.IsQuestFlaggedCompleted(questid.ToString());

                    return barray;
                }
            }

            return null;
        }

        internal static BehaviorArray GetGarrisonBuildingFirstQuestArray(Building b, bool alliance)
        {
            if (b.Type == BuildingType.SalvageYard)
            {
                //var abandon = new Behaviors.BehaviorQuestAbandon(b.FirstQuestID);
                var pickup = new BehaviorQuestPickup(b.FirstQuestId, b.SafeMovementPoint, b.WorkOrderNpcEntryId);
                var itemInteraction = new BehaviorItemInteraction(118473);
                var turnin = new BehaviorQuestTurnin(b.FirstQuestId, b.SafeMovementPoint, b.WorkOrderNpcEntryId);
                var behaviorArray = new BehaviorArray(new Behavior[] { pickup, itemInteraction, turnin });
                behaviorArray.Criteria += () => BaseSettings.CurrentSettings.BehaviorQuests;
                return behaviorArray;
            }
            else if (b.Type == BuildingType.TradingPost)
            {
                var pickup = new BehaviorQuestPickup(b.FirstQuestId, b.SafeMovementPoint, b.WorkOrderNpcEntryId);
                var moveLoc = Player.IsAlliance
                    ? new WoWPoint(1764.08, 150.39, 76.02)
                    : new WoWPoint(5745.101, 4570.491, 138.8332);

                var moveto = new BehaviorMove(moveLoc, 7f);
                var npcId = Player.IsAlliance ? 87288 : 87260;
                var target = new BehaviorSelectTarget(moveLoc);
                var interact = new BehaviorItemInteraction(118418, true);
                var turnin = new BehaviorQuestTurnin(b.FirstQuestId, b.SafeMovementPoint, b.WorkOrderNpcEntryId);

                var behaviorArray = new BehaviorArray(new Behavior[] { pickup, moveto, target, interact, turnin });
                behaviorArray.Criteria += () => BaseSettings.CurrentSettings.BehaviorQuests;
                return behaviorArray;
            }
            else if (b.Type == BuildingType.Storehouse)
            {
                var pickup = new BehaviorQuestPickup(b.FirstQuestId, b.EntranceMovementPoint, b.FirstQuestNpcId);

                List<WoWPoint> _hotSpots = new List<WoWPoint>
                {
                        MovementCache.GarrisonEntrance,

                        MovementCache.GardenPlot63SafePoint,
                        MovementCache.MinePlot59SafePoint,

                        MovementCache.MediumPlot22SafePoint,
                        MovementCache.LargePlot23SafePoint,
                        MovementCache.LargePlot24SafePoint,
                        MovementCache.MediumPlot25SafePoint,

                        MovementCache.SmallPlot18SafePoint,
                        MovementCache.SmallPlot19SafePoint,
                        MovementCache.SmallPlot20SafePoint,
                    };

                var looting = new BehaviorHotspotRunning(_hotSpots.ToArray(), BehaviorHotspotRunning.HotSpotType.Looting, () => BehaviorManager.HasQuestAndNotCompleted(b.FirstQuestId));
                var turnin = new BehaviorQuestTurnin(b.FirstQuestId, b.EntranceMovementPoint, b.FirstQuestNpcId);

                var behaviorArray = new BehaviorArray(new Behavior[] { pickup, looting, turnin });
                behaviorArray.Criteria += () => BaseSettings.CurrentSettings.BehaviorQuests;
                return behaviorArray;
            }
            else if (b.Type == BuildingType.Lumbermill)
            {
                var pickup = new BehaviorQuestPickup(b.FirstQuestId, b.EntranceMovementPoint, b.FirstQuestNpcId);

                WoWPoint movementPoint;
                if (Player.IsAlliance)
                    movementPoint = new WoWPoint(1555.087, 173.8229, 72.59766);
                else
                    movementPoint = new WoWPoint(6082.979, 4795.821, 149.1655);

                var looting = new BehaviorHotspotRunning(new[] { movementPoint }, new uint[] { 234021, 233922 }, BehaviorHotspotRunning.HotSpotType.Looting, () => BehaviorManager.HasQuestAndNotCompleted(b.FirstQuestId));
                var turnin = new BehaviorQuestTurnin(b.FirstQuestId, b.EntranceMovementPoint, b.FirstQuestNpcId);
                var behaviorArray = new BehaviorArray(new Behavior[] { pickup, looting, turnin });
                behaviorArray.Criteria += () => BaseSettings.CurrentSettings.BehaviorQuests;
                return behaviorArray;
            }
            else if (b.Type == BuildingType.Mines)
            {
                var pickup = new BehaviorQuestPickup(b.FirstQuestId, b.EntranceMovementPoint, b.FirstQuestNpcId);

                var looting = new BehaviorHotspotRunning(Player.IsAlliance ?
                    MovementCache.Alliance_Mine_LevelOne.ToArray() :
                    MovementCache.Horde_Mine_LevelOne.ToArray(),
                    MineQuestMobIDs.ToArray(),
                    BehaviorHotspotRunning.HotSpotType.Killing,
                    () => BehaviorManager.HasQuestAndNotCompleted(b.FirstQuestId));

                var turnin = new BehaviorQuestTurnin(b.FirstQuestId, b.EntranceMovementPoint, b.FirstQuestNpcId);

                var behaviorArray = new BehaviorArray(new Behavior[] { pickup, looting, turnin });
                behaviorArray.Criteria += () => BaseSettings.CurrentSettings.BehaviorQuests;
                return behaviorArray;
            }
            else if (b.Type == BuildingType.HerbGarden)
            {
                var pickup = new BehaviorQuestPickup(b.FirstQuestId, b.EntranceMovementPoint, b.FirstQuestNpcId);

                var looting = new BehaviorHotspotRunning(Player.IsAlliance ?
                    MovementCache.Alliance_Herb_LevelOne.ToArray() :
                    MovementCache.Horde_Herb_LevelOne.ToArray(),
                    HerbQuestMobIDs.ToArray(),
                    BehaviorHotspotRunning.HotSpotType.Killing,
                    () => BehaviorManager.HasQuestAndNotCompleted(b.FirstQuestId));

                var turnin = new BehaviorQuestTurnin(b.FirstQuestId, b.EntranceMovementPoint, b.FirstQuestNpcId);

                var behaviorArray = new BehaviorArray(new Behavior[] { pickup, looting, turnin });
                behaviorArray.Criteria += () => BaseSettings.CurrentSettings.BehaviorQuests;
                return behaviorArray;
            }
            else
            {
                var pickup = new BehaviorQuestPickup(b.FirstQuestId, b.SafeMovementPoint, b.FirstQuestNpcId);
                var workorder = new BehaviorQuestWorkOrder(b);
                var workorderPickup = new BehaviorQuestWorkOrderPickup(b);
                var turnin = new BehaviorQuestTurnin(b.FirstQuestId, b.SafeMovementPoint, b.WorkOrderNpcEntryId);
                var behaviorArray = new BehaviorArray(new Behavior[] { pickup, workorder, workorderPickup, turnin });
                behaviorArray.Criteria += () => BaseSettings.CurrentSettings.BehaviorQuests;
                return behaviorArray;
            }
        }

        internal static List<uint> MineQuestMobIDs = new List<uint>
        {
            81396,
            81362,81398, //Horde
            83628, 83629, //Ally
            85294,
        };

        internal static List<uint> HerbQuestMobIDs = new List<uint>
        {
            85341, 81967,
            85412, 85411, 85410, 85408, 85409, 85407,
        };

        internal static List<uint> StoreHouseQuestIDs = new List<uint>
        {
            237257,
            237039
        };

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
         *   Cost 25 Iron horde scraps (113681) <WoWItem Name="Iron Horde Scraps" Entry="113681" />
         *   Rewards 
         *      follower token Armor (120301) (choice 1)
         *      follower token Weapon (120302) (choice 2)
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
