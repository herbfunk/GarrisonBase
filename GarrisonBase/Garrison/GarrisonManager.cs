using System;
using System.Collections.Generic;
using System.Linq;
using Herbfunk.GarrisonBase.Character;
using Herbfunk.GarrisonBase.Coroutines;
using Herbfunk.GarrisonBase.Coroutines.Behaviors;
using Herbfunk.GarrisonBase.Garrison.Enums;
using Herbfunk.GarrisonBase.Garrison.Objects;
using Herbfunk.GarrisonBase.Helpers;
using Herbfunk.GarrisonBase.TargetHandling;
using Styx;
using Styx.Common.Helpers;
using Styx.WoWInternals.DB;
using Styx.WoWInternals.Garrison;

namespace Herbfunk.GarrisonBase.Garrison
{
    /// <summary>
    /// Object Manager for garrison related things:
    /// -Missions
    /// -Followers
    /// -Buildings
    /// </summary>
    public static class GarrisonManager
    {

        public static int GarrisonLevel { get; set; }
        public static int CommandTableEntryId { get; set; }
        public static int SellRepairNpcId { get; set; }
        public static int GarrisonResourceCacheEntryId { get; set; }

        /// <summary>
        /// Buildings contain enchanting
        /// </summary>
        public static bool HasDisenchant = false;
        internal static int DisenchantingEntryId = 0;
        /// <summary>
        /// Buildings contains a forge (blacksmithing)
        /// </summary>
        public static bool HasForge = false;
        public static BuildingType ForgeBuilding { get; set; }

        public static int PrimalTraderID { get; set; }
        public static WoWPoint PrimalTraderPoint { get; set; }

        public static int FlightPathNpcId { get; set; }



        internal static bool Initalized = false;
        internal static void Initalize()
        {
            GarrisonBase.Debug("Initalizing GarrisonManager..");

            RefreshFollowers();

            RefreshBuildings();

            UpdateMissionIds(true);

            

            if (!Player.IsAlliance)
            {
                GarrisonLevel = Player.MapId == 1153? 3: Player.MapId == 1330 ? 2: 1;

                CommandTableEntryId = Player.MapId == 1153 ? 85805 : 86031;
                //1153 == level 3 garrison
                //1330 == level 2 garrison

                GarrisonResourceCacheEntryId = 237191;
                //237720
                SellRepairNpcId = 76872;
                FlightPathNpcId = 79407;
                PrimalTraderID = 84967;
                PrimalTraderPoint = GarrisonLevel == 2 ? MovementCache.HordePrimalTraderLevel2: MovementCache.HordePrimalTraderLevel3;

                //BlackspotManager.AddBlackspots(MovementCache.HordeBlackSpots);

                //ProfileManager.LoadNew(GarrisonBase.GarrisonBasePath + @"\Profiles\HordeGarrison.xml", false);
            }
            else
            {
                GarrisonLevel = Player.MapId == 1159 ? 3 : Player.MapId == 1331 ? 2 : 1;

                CommandTableEntryId = Player.MapId == 1331 ? 84224 : 84698;
                //1331 == level 2 garrison
                //1159 == level 3 garrison
                
                GarrisonResourceCacheEntryId = 236916;
                SellRepairNpcId = 81346;
                FlightPathNpcId = 81103;
                PrimalTraderID = 84246;
                PrimalTraderPoint = GarrisonLevel == 2 ? MovementCache.AlliancePrimalTraderLevel2 : MovementCache.AlliancePrimalTraderLevel3;

                //BlackspotManager.AddBlackspots(MovementCache.AllianceBlackSpots);

                //if (Character.Player.MapId == 1331)
                //    ProfileManager.LoadNew(GarrisonBase.GarrisonBasePath + @"\Profiles\AllianceGarrison.xml", false);
                //else
                //    ProfileManager.LoadNew(GarrisonBase.GarrisonBasePath + @"\Profiles\AllianceGarrisonLevel3.xml", false);
            }

            GarrisonBase.Debug("Has Forge {0}", HasForge);
            GarrisonBase.Debug("Has Disenchanting {0} {1}", HasDisenchant,
                DisenchantingEntryId != 0 ? DisenchantingEntryId.ToString() : "");

            LuaEvents.OnGarrisonMissionListUpdated += () => UpdateMissionIds();
            LuaEvents.OnGarrisonMissionFinished += () => UpdateMissionIds();

            Initalized = true;

           
        }

        internal static void Reset()
        {
            Initalized = false;
            Buildings.Clear();
            Followers.Clear();
            AvailableMissions.Clear();
            CompletedMissionIds.Clear();

        }

        public static Dictionary<int, Follower> Followers = new Dictionary<int, Follower>();
        public static int MaxActiveFollowers = 20;
        public static int CurrentActiveFollowers = 0;
        internal static List<int> BuildingIdsWithFollowerWorking = new List<int>(); 

        //internal static void RefreshFollowerIds()
        //{
        //    GarrisonBase.Debug("Refreshing Follower Ids..");

        //    FollowerIdsCollected.Clear();
        //    FollowerIdsNotCollected.Clear();
        //    BuildingIdsWithFollowerWorking.Clear();
        //    CurrentActiveFollowers = 0;
        //    FollowerIdsAll = LuaCommands.GetAllFollowerIDs();
        //    foreach (var id in FollowerIdsAll)
        //    {
        //        if (LuaCommands.IsFollowerCollected(id))
        //            FollowerIdsCollected.Add(id);
        //        else
        //            FollowerIdsNotCollected.Add(id);
        //    }

        //    GarrisonBase.Log("Found a total of {0} collected followers!", FollowerIdsCollected.Count);
        //}

        internal static void RefreshFollowers()
        {
            GarrisonBase.Debug("Refreshing Followers..");

            BuildingIdsWithFollowerWorking.Clear();
            CurrentActiveFollowers = 0;

            foreach (var follower in GarrisonInfo.Followers)
            {
                var followerId = Convert.ToInt32(follower.Id);
                var f = new Follower(follower);
                
                Followers.Add(followerId, f);
                
                if (f.Status != GarrisonFollowerStatus.Inactive)
                    CurrentActiveFollowers++;
                if (f.AssignedBuildingId > -1)
                    BuildingIdsWithFollowerWorking.Add(f.AssignedBuildingId);
            }

            GarrisonBase.Log("Followers Active {0} / {1}", CurrentActiveFollowers, MaxActiveFollowers);
        }


        public static List<Mission> AvailableMissions = new List<Mission>();
        public static List<Mission> CompletedMissions = new List<Mission>();
        public static List<int> AvailableMissionIds = new List<int>();
        public static List<int> CompletedMissionIds = new List<int>();
        private static readonly WaitTimer UpdateMissionsWaitTimer = WaitTimer.OneSecond;
        internal static void UpdateMissionIds(bool force=false)
        {
            if (!force && !UpdateMissionsWaitTimer.IsFinished) return;
            UpdateMissionsWaitTimer.Reset();

            GarrisonBase.Debug("Updating Mission Ids..");

            CompletedMissionIds.Clear();
            AvailableMissionIds.Clear();
            var completedMissionIds = new List<int>();
            var availableMissionIds = new List<int>();
            var completedMissions = new List<Mission>();
            var availableMissions = new List<Mission>();

            foreach (var mission in GarrisonInfo.Missions)
            {
                if (mission.State == MissionState.None)
                {
                    availableMissionIds.Add(mission.Id);
                    availableMissions.Add(new Mission(mission));
                }
                else if (mission.State == MissionState.Complete || 
                    (mission.State== MissionState.InProgress && mission.MissionTimeLeft.TotalSeconds == 0))
                {
                    completedMissionIds.Add(mission.Id);
                    completedMissions.Add(new Mission(mission, true));
                }
            }
            CompletedMissionIds = completedMissionIds;
            CompletedMissions = completedMissions;
            AvailableMissionIds = availableMissionIds;
            AvailableMissions = availableMissions.OrderByDescending(m => m.Priority).ThenByDescending(m => m.ItemLevel).ThenByDescending(m => m.Level).ToList(); 
        }

        internal static void RefreshMissions()
        {
            GarrisonBase.Debug("Refreshing Missions..");

            var removalList = new List<int>();

            if (AvailableMissions.Count > 0)
            {
                for (var i = 0; i < AvailableMissions.Count; i++)
                {
                    var mission = AvailableMissions[i];

                    if (!mission.Valid || mission.refGarrisonMission.State != MissionState.None)
                    {
                        removalList.Add(i);
                    }

                }

                if (removalList.Count > 0)
                {
                    foreach (var k in removalList.OrderByDescending(k => k))
                    {
                        var id = AvailableMissions[k].Id;
                        AvailableMissionIds.Remove(id);
                        AvailableMissions.RemoveAt(k);
                        GarrisonBase.Debug("Removing Available Mission {0}", id);
                    }
                }
            }

            if (CompletedMissions.Count <= 0) return;

            removalList.Clear();

            for (var i = 0; i < CompletedMissions.Count; i++)
            {
                var mission = CompletedMissions[i];

                if (!mission.Valid ||  //reference not valid
                    (mission.refGarrisonMission.State == MissionState.None) ||  //state not completed
                    (mission.refGarrisonMission.State == MissionState.InProgress && 
                        mission.refGarrisonMission.MissionTimeLeft.TotalSeconds>0)) //or in progress but with time left
                {
                    removalList.Add(mission.Id);
                }
            }

            if (removalList.Count <= 0) return;

            foreach (var k in removalList)
            {
                GarrisonBase.Debug("Removing Completed Mission {0}", k);

                CompletedMissionIds.Remove(k);

                var mission = CompletedMissions.FirstOrDefault(m => m.Id == k);
                if (mission == null) continue;
                var index = CompletedMissions.IndexOf(mission);
                CompletedMissions.RemoveAt(index);
            }
        }

        internal static void SimulateMission(int missionId)
        {
            RefreshBuildings();
            UpdateMissionIds();

            Mission mission = AvailableMissions.FirstOrDefault(m => m.Id == missionId);
            SimulateMission(mission);
        }
        internal static void SimulateMission(Mission mission)
        {
            GarrisonBase.Log("Simulating Mission {0}", mission.Name);
            RefreshFollowers();

            var firstFollowers = 
                Followers.Values.Where(follower => 
                    (follower.Status == GarrisonFollowerStatus.Idle ||follower.Status == GarrisonFollowerStatus.InParty) &&
                     follower.Level>=mission.Level && follower.ItemLevel>=mission.ItemLevel).ToList();

            var firstFollowerIds = firstFollowers.Select(f => f.ID).ToList();

            var idleFollowers =
                Followers.Values.Where(follower => !firstFollowerIds.Contains(follower.ID) &&
                    (follower.Status == GarrisonFollowerStatus.Idle || 
                        follower.Status == GarrisonFollowerStatus.InParty)).OrderByDescending(f => f.ItemLevel).ThenByDescending(f => f.Level).ToList();

            int followerCount = idleFollowers.Count;
            int missionSlots = mission.Followers;
            MissionSimulatorOptions options = new MissionSimulatorOptions(true, false,
                false, 
                false, true, true, false,
                false, 
                false);
            //mission.RewardTypes.HasFlag(RewardTypes.XP), 
            var results = new List<MissionSimulatorResults>();
            foreach (var follower in firstFollowers)
            {
                //int followerIndex1 = idleFollowers.IndexOf(follower);

                //if (followerIndex1 > followerCount - missionSlots)
                //    break;
                
                GarrisonFollower follower1 = follower._refFollower;
                GarrisonFollower follower2, follower3;
                if (missionSlots > 1)
                {
                    for (int i = 0; i < (missionSlots>2?followerCount - 1:followerCount); i++)
                    {
                        follower2 = idleFollowers[i]._refFollower;
                        if (missionSlots > 2)
                        {
                            for (int j = i + 1; j < followerCount; j++)
                            {
                                follower3 = idleFollowers[j]._refFollower;
                                results.Add(SimulatorResults(mission, new[] { follower1, follower2, follower3 }, null));
                                //break;
                            }
                        }
                        else
                        {
                            results.Add(SimulatorResults(mission, new[] { follower1, follower2 }, null));
                        }

                        //break;
                    }
                }
                else
                {
                    results.Add(SimulatorResults(mission, new[] { follower1 }, null));
                }

                //break;
            }

            //foreach (var result in results)
            //{
            //    string followernames = result.Followers.Aggregate(string.Empty, (current, f) => current + f.Name + "\t");
            //    GarrisonBase.Log("Result: Success {0}%, Followers {1}", result.SuccessChance, followernames);
            //}
        }

        private static readonly MissionSimulatorOptions SuccessOnlyOptions = 
            new MissionSimulatorOptions(true, false,false, false, false, false, false,false, false);

        private static MissionSimulatorResults SimulatorResults(Mission mission, GarrisonFollower[] followers, MissionSimulatorOptions options)
        {
            var _options = options ?? SuccessOnlyOptions;
            var result = GarrisonMissionSimulator.Simulate(mission.refGarrisonMission, followers, _options);
           
            string followernames = result.Followers.Aggregate(String.Empty, (current, f) => current + f.Name + "\t");
            GarrisonBase.Log("Result: Success {0}%, Followers {1}", result.SuccessChance, followernames);

            return result;
        }
        

        /// <summary>
        /// Dictionary of building objects!
        /// </summary>
        public static Dictionary<BuildingType, Building> Buildings = new Dictionary<BuildingType, Building>();

        public static List<int> BuildingIDs = new List<int>();

        internal static void RefreshBuildings()
        {
            GarrisonBase.Debug("Refreshing Buildings..");

            BuildingIDs.Clear();
            Buildings.Clear();
            HasDisenchant = false;
            HasForge = false;
            ForgeBuilding = BuildingType.Unknown;

            foreach (var building in GarrisonInfo.OwnedBuildings)
            {
                var buildingId = Convert.ToInt32(building.GarrBuildingId);
                BuildingIDs.Add(buildingId);

                var b = new Building(buildingId);

                Buildings.Add(b.Type, b);
                if (b.Type == BuildingType.TheForge && !b.IsBuilding && !b.CanActivate)
                {
                    HasForge = true;
                    ForgeBuilding = BuildingType.TheForge;
                }
                if (b.Type == BuildingType.EngineeringWorks && !b.IsBuilding && !b.CanActivate)
                {
                    HasForge = true;
                    ForgeBuilding = BuildingType.EngineeringWorks;
                }

                if (b.Type == BuildingType.EnchantersStudy && !b.IsBuilding && !b.CanActivate)
                {
                    HasDisenchant = true;

                    if (!Player.IsAlliance)
                    {
                        if (b.Level == 1)
                            DisenchantingEntryId = 237132;
                        else if (b.Level == 2)
                            DisenchantingEntryId = 237134;
                        else
                            DisenchantingEntryId = 237136;
                    }
                    else
                    {
                        if (b.Level == 1)
                            DisenchantingEntryId = 237335;
                        else if (b.Level == 2)
                            DisenchantingEntryId = 228618;
                        else if (b.Level == 3)
                            DisenchantingEntryId = 228591;
                    }
                }

                if (b.Type == BuildingType.Barracks && b.Level == 3)
                    MaxActiveFollowers = 25;
            }


            //if not completed the quest to unlock than we manually insert the buildings (since they do not show up in the building ids)
            if (!Buildings.ContainsKey(BuildingType.Mines))
                Buildings.Add(BuildingType.Mines, new Building(61));
            if (!Buildings.ContainsKey(BuildingType.HerbGarden))
                Buildings.Add(BuildingType.HerbGarden, new Building(29));
        }

        public enum PrimalTraderItemTypes
        {
            SorcerousAir = 113264,
            SorcerousFire = 113261,
            SorcerousWater = 113262,
            SorcerousEarth = 113263,
            SavageBlood = 118472,
            AlchemicalCatalyst = 108996,
            TruesteelIngot = 108257,
            GearspringParts = 111366,
            WarPaints = 112377,
            TaladiteCrystal = 115524,
            BurnishedLeather = 110611,
            HexweaveCloth = 111556,
        }
        public class PrimalTraderItem
        {
            public PrimalTraderItemTypes Type { get; set; }
            public string Name { get; set; }
            public int Cost { get; set; }

            public readonly uint ItemId;

            public PrimalTraderItem(PrimalTraderItemTypes type, string name, int cost)
            {
                Type = type;
                Name = name;
                Cost = cost;
                ItemId = Convert.ToUInt32((int) Type);
            }
        }

        public static readonly List<PrimalTraderItem> PrimalTraderItems = new List<PrimalTraderItem>
        {
            new PrimalTraderItem(PrimalTraderItemTypes.SorcerousAir, "Sorcerous Air", 25),
            new PrimalTraderItem(PrimalTraderItemTypes.SorcerousEarth, "Sorcerous Earth", 25),
            new PrimalTraderItem(PrimalTraderItemTypes.SorcerousFire, "Sorcerous Fire", 25),
            new PrimalTraderItem(PrimalTraderItemTypes.SorcerousWater, "Sorcerous Water", 25),

            new PrimalTraderItem(PrimalTraderItemTypes.SavageBlood, "Savage Blood", 50),

            new PrimalTraderItem(PrimalTraderItemTypes.AlchemicalCatalyst, "Alchemical Catalyst", 10),
            new PrimalTraderItem(PrimalTraderItemTypes.TruesteelIngot, "Truesteel Ingot", 10),
            new PrimalTraderItem(PrimalTraderItemTypes.GearspringParts, "Gearspring Parts", 10),
            new PrimalTraderItem(PrimalTraderItemTypes.WarPaints, "War Paints", 10),
            new PrimalTraderItem(PrimalTraderItemTypes.TaladiteCrystal, "Taladite Crystal", 10),
            new PrimalTraderItem(PrimalTraderItemTypes.BurnishedLeather, "Burnished Leather", 10),
            new PrimalTraderItem(PrimalTraderItemTypes.HexweaveCloth, "Hexweave Cloth", 10),
        };

        internal static readonly List<uint> GarrisonZoneIds = new List<uint>
        {
            7004, //Frostwall
            7078, //Lunarfall
        };

        internal static readonly List<uint> GarrisonMineZoneIds = new List<uint>
        {
            7327, //Frostwall Mine 1
            7328, //Frostwall Mine 2
            7329, //Frostwall Mine 3
            7324, //Lunarfall Excavation 1
            7325, //Lunarfall Excavation 2
            7326, //Lunarfall Excavation 3
        };

        internal static Behavior[] GetGarrisonBehaviors()
        {
            var retBehaviorList = new List<Behavior>
            {
                new BehaviorGetMail(),
                new BehaviorMissionComplete(),
                new BehaviorCache()
            };

            //Finalize Plots
            foreach (var b in Buildings.Values.Where(b => b.CanActivate)) retBehaviorList.Add(new BehaviorFinalizePlots(b));

            #region Building First Quest Behaviors
            foreach (var b in Buildings.Values.Where(b => !b.FirstQuestCompleted && !b.IsBuilding && b.FirstQuestNpcId > -1 && b.FirstQuestId > -1))
            {
                retBehaviorList.Add(QuestHelper.GetGarrisonBuildingFirstQuestArray(b, Player.IsAlliance));
            }

            #endregion

            #region Herbing and Mining

            if (Buildings.Values.Any(b => b.Type == BuildingType.Mines))
            {
                var miningMoveTo = new BehaviorMove(MovementCache.MinePlot59SafePoint);
                miningMoveTo.Criteria += () => !GarrisonMineZoneIds.Contains(Player.ZoneId.Value);

                var miningArray = new BehaviorArray(new Behavior[]
                {
                    new BehaviorCustomAction(() =>
                    {
                        TargetManager.LootType = TargetManager.LootFlags.Units | TargetManager.LootFlags.Ore;
                    }),

                    miningMoveTo,
                    new BehaviorMine()
                });
                miningArray.Criteria += () => (!Buildings[BuildingType.Mines].IsBuilding &&
                                               !Buildings[BuildingType.Mines].CanActivate &&
                                               Buildings[BuildingType.Mines].FirstQuestCompleted &&
                                               LuaCommands.CheckForDailyReset(BaseSettings.CurrentSettings.LastCheckedMine) &&
                                               BaseSettings.CurrentSettings.BehaviorMineGather);
                miningArray.DisposalAction = () =>
                {
                    TargetManager.LootType = TargetManager.LootFlags.None;
                };

                retBehaviorList.Add(miningArray);
                retBehaviorList.Add(new BehaviorWorkOrderPickUp(Buildings[BuildingType.Mines]));
                retBehaviorList.Add(new BehaviorWorkOrderStartUp(Buildings[BuildingType.Mines]));
            }

            if (Buildings.Values.Any(b => b.Type == BuildingType.HerbGarden))
            {
                var herbingArray = new BehaviorArray(new Behavior[]
                {
                    new BehaviorCustomAction(() =>
                    {
                        TargetManager.LootType = TargetManager.LootFlags.Units | TargetManager.LootFlags.Herbs;
                    }),

                    new BehaviorMove(MovementCache.GardenPlot63SafePoint),
                    new BehaviorHerb()
                });
                herbingArray.Criteria += () => (!Buildings[BuildingType.HerbGarden].IsBuilding &&
                               !Buildings[BuildingType.HerbGarden].CanActivate &&
                               Buildings[BuildingType.HerbGarden].FirstQuestCompleted &&
                               LuaCommands.CheckForDailyReset(BaseSettings.CurrentSettings.LastCheckedHerb) &&
                               BaseSettings.CurrentSettings.BehaviorHerbGather);
                herbingArray.DisposalAction = () =>
                {
                    TargetManager.LootType = TargetManager.LootFlags.None;
                };

                retBehaviorList.Add(herbingArray);
                retBehaviorList.Add(new BehaviorWorkOrderPickUp(Buildings[BuildingType.HerbGarden]));
                retBehaviorList.Add(new BehaviorWorkOrderStartUp(Buildings[BuildingType.HerbGarden]));
            }

            #endregion

            #region Professions
            foreach (var profession in Player.Professions.ProfessionSkills.Values.Where(p => p.DailyCooldownSpellIds!=null))
            {
                if (profession.Skill == SkillLine.Inscription)
                    retBehaviorList.Add(new BehaviorMilling());

                int[] spellIds = profession.DailyCooldownSpellIds;
                retBehaviorList.Add(new BehaviorCraftingProfession(profession.Skill, spellIds[1]));
                retBehaviorList.Add(new BehaviorCraftingProfession(profession.Skill, spellIds[0]));
            }
            #endregion

            #region Salvaging
            if (Buildings.Values.Any(b => b.Type == BuildingType.SalvageYard))
            {
                retBehaviorList.Add(new BehaviorSalvage());
            }
            #endregion

            #region Work Order Pickup && Startup
            foreach (var b in Buildings.Values.Where(b => b.FirstQuestId <= 0 || b.FirstQuestCompleted).OrderBy(b => b.Plot))
            {
                if (b.Type != BuildingType.Mines || b.Type != BuildingType.HerbGarden)
                {
                    if (b.WorkOrder != null)
                    {
                        retBehaviorList.Add(new BehaviorWorkOrderPickUp(b));

                        if (b.Type == BuildingType.EnchantersStudy)
                            retBehaviorList.Add(new BehaviorDisenchant()); //Disenchanting!
                        else if (b.Type == BuildingType.ScribesQuarters)
                            retBehaviorList.Add(new BehaviorMilling()); //Milling!
                        else if (b.Type == BuildingType.Barn)
                        {
                            if (Player.Inventory.Trap != null)
                            {
                                GarrisonBase.Log("Adding Trapping Behavior");

                                if (Player.Inventory.Trap.TrapRank > 1)
                                {
                                    retBehaviorList.Add(BehaviorManager.BehaviorArray_Trapping_Elites_Nagrand.Clone());
                                }

                                if (Player.IsAlliance)
                                {
                                    retBehaviorList.Add(BehaviorManager.BehaviorArray_Trapping_Leather_ShadowmoonVally.Clone());
                                    retBehaviorList.Add(BehaviorManager.BehaviorArray_Trapping_Fur_ShadowmoonValley.Clone());
                                }
                                else
                                {
                                    retBehaviorList.Add(BehaviorManager.BehaviorArray_Trapping_Leather_FrostfireRidge.Clone());
                                    retBehaviorList.Add(BehaviorManager.BehaviorArray_Trapping_Fur_Nagrand_Horde.Clone());
                                }

                                retBehaviorList.Add(BehaviorManager.BehaviorArray_Trapping_Boars_Gorgond.Clone());
                            }
                        }

                        retBehaviorList.Add(new BehaviorWorkOrderStartUp(b));

                        if (b.Type == BuildingType.WarMillDwarvenBunker && b.Level == 3)
                        {
                            var questid = Player.IsAlliance ? 38175 : 38188;
                            retBehaviorList.Add(QuestHelper.GetDailyQuestArray(Convert.ToUInt32(questid), Player.IsAlliance));
                        }
                        else if (b.Type == BuildingType.AlchemyLab && b.HasFollowerWorking)
                        {
                            retBehaviorList.Add(QuestHelper.GetDailyQuestArray(Convert.ToUInt32(37270), Player.IsAlliance));
                        }
                    }
                }
            }
            #endregion

            //Primal Spirit Exchange
            retBehaviorList.Add(new BehaviorPrimalTrader());

            var forceBagCheck=new BehaviorCustomAction(() =>
            {
                Common.ForceBagCheck = true;
            });
            retBehaviorList.Add(forceBagCheck);

            //Finally, start some new missions!
            retBehaviorList.Add(new BehaviorMissionStartup());

            //Optional follower behaviors (to unlock)
            retBehaviorList.Add(BehaviorArrayFollowers.Clone());

            return retBehaviorList.ToArray();
        }

        internal static readonly BehaviorArray BehaviorArrayFollowers = new BehaviorArray(new Behavior[]
        {
            Follower.FollowerQuestBehaviorArray(209), //Abu'Gur (Nagrand)
            Follower.FollowerQuestBehaviorArray(170), //Goldmaeng (Nagrand)

            Follower.FollowerQuestBehaviorArray(189), //Blook (Gorgrond)
            Follower.FollowerQuestBehaviorArray(193), //Tormok (Gorgrond)

            Follower.FollowerQuestBehaviorArray(207), //Defender (Talador)

            Follower.FollowerQuestBehaviorArray(467), //Fen Tao (Ashran)

            Follower.FollowerQuestBehaviorArray(32), //Dagg (Frostfire)
                
            Follower.FollowerQuestBehaviorArray(190), //Arch Mage (4 different areas)

            //new BehaviorCustomAction(() => Common.PreChecks.IgnoreHearthing=false),
            //new BehaviorUseFlightPath(MovementCache.GarrisonEntrance)
        }, "Followers");

        /*
         * Primal Trader EntryID
         * Horde - 84967
         * Alliance - 84246
         */

        /*  Daily NPCs
         * 
         * Dust Trade
         *  -Daily Quest (50 Draenic Dust)
         *  -5 Draenic Dust for 1 Primal Spirit (merchant index 1)
         * 
         * Ore Trade
         *  -Daily Quest (25 Blackrock + 25 True Iron)
         *  -5 True Iron for 1 primal spirit (merchant index 2)
         *  -5 Blackrock for 1 primal spirit (merchant index 1)
         *  
         * Fur Trader
         *  -Daily Quest 50 Scrumptous Fur
         *  -5 Scrumptous Fur for 1 Primal Spirit (
         *  
         * Leather Trader
         *  -Daily Quest (50 raw beast hide)
         *  -5 raw beast hide for 1 Primal Spirit
         *  
         * Herb Trader
         *  -Daily Quest (10 Fireweed, 10 Starflower, 10 Talador Orchid, 10 Gorgond Flytrap, 10 Nagrand Arrowbloom)
         *  -5x of any herb for 1 Primal Spirit
         * 
         *  Horde
         *  Dust Trader - 91029
         *  Ore Trader - 91030
         *  Fur Trader - 91034
         *  Leather Trader - 91033
         *  Herb Trader - 91031
         *  
         *  Alliance
         *  Dust Trader - 91020
         *  Ore Trader - 90894
         *  Fur Trader - 91025
         *  Leather Trader - 91024
         *  Herb Trader - 91404
         */
    }
}
