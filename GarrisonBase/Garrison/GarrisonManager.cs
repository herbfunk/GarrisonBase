using System.Collections.Generic;
using System.Linq;
using Herbfunk.GarrisonBase.Garrison.Enums;
using Herbfunk.GarrisonBase.Garrison.Objects;
using Styx;
using Styx.Common.Helpers;
using Styx.Pathing;
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

            RefreshBuildings();

            UpdateMissionIds(true);

            RefreshFollowerIds();
            RefreshFollowers();

            if (!Character.Player.IsAlliance)
            {
                GarrisonLevel = Character.Player.MapId == 1153? 3: Character.Player.MapId == 1330 ? 2: 1;

                CommandTableEntryId = Character.Player.MapId == 1153 ? 85805 : 86031;
                //1153 == level 3 garrison
                //1330 == level 2 garrison

                GarrisonResourceCacheEntryId = 237191;
                //237720
                SellRepairNpcId = 76872;
                FlightPathNpcId = 79407;
                PrimalTraderID = 84967;
                PrimalTraderPoint = GarrisonLevel == 2 ? MovementCache.HordePrimalTraderLevel2: MovementCache.HordePrimalTraderLevel3;

                BlackspotManager.AddBlackspots(MovementCache.HordeBlackSpots);

                //ProfileManager.LoadNew(GarrisonBase.GarrisonBasePath + @"\Profiles\HordeGarrison.xml", false);
            }
            else
            {
                GarrisonLevel = Character.Player.MapId == 1159 ? 3 : Character.Player.MapId == 1331 ? 2 : 1;

                CommandTableEntryId = Character.Player.MapId == 1331 ? 84224 : 84698;
                //1331 == level 2 garrison
                //1159 == level 3 garrison
                
                GarrisonResourceCacheEntryId = 236916;
                SellRepairNpcId = 81346;
                FlightPathNpcId = 81103;
                PrimalTraderID = 84246;
                PrimalTraderPoint = GarrisonLevel == 2 ? MovementCache.AlliancePrimalTraderLevel2 : MovementCache.AlliancePrimalTraderLevel3;

                BlackspotManager.AddBlackspots(MovementCache.AllianceBlackSpots);

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
        internal static List<int> FollowerIdsAll = new List<int>();
        internal static List<int> FollowerIdsCollected = new List<int>();
        internal static List<int> FollowerIdsNotCollected = new List<int>();

        internal static void RefreshFollowerIds()
        {
            GarrisonBase.Debug("Refreshing Follower Ids..");

            FollowerIdsCollected.Clear();
            FollowerIdsNotCollected.Clear();
            CurrentActiveFollowers = 0;
            FollowerIdsAll = LuaCommands.GetAllFollowerIDs();
            foreach (var id in FollowerIdsAll)
            {
                if (LuaCommands.IsFollowerCollected(id))
                    FollowerIdsCollected.Add(id);
                else
                    FollowerIdsNotCollected.Add(id);
            }

            GarrisonBase.Log("Found a total of {0} collected followers!", FollowerIdsCollected.Count);
        }
        internal static void RefreshFollowers()
        {
            GarrisonBase.Debug("Refreshing Followers..");

            foreach (var f in FollowerIdsCollected)
            {
                if (!Followers.ContainsKey(f))
                {
                    var gfollower = GarrisonInfo.Followers.First(follower => follower.GarrFollowerId == f);
                    if (gfollower != null)
                    {
                        Followers.Add(f, new Follower(gfollower));

                        if (gfollower.Status != GarrisonFollowerStatus.Inactive)
                            CurrentActiveFollowers++;
                    }
                }
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
            RefreshFollowerIds();

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
           
            string followernames = result.Followers.Aggregate(string.Empty, (current, f) => current + f.Name + "\t");
            GarrisonBase.Log("Result: Success {0}%, Followers {1}", result.SuccessChance, followernames);

            return result;
        }
        

        /// <summary>
        /// Dictionary of building objects!
        /// </summary>
        public static Dictionary<BuildingType, Building> Buildings = new Dictionary<BuildingType, Building>();

        public static List<string> BuildingIDs = new List<string>();

        internal static void RefreshBuildingIDs()
        {
            GarrisonBase.Debug("Refreshing Building Ids..");

            BuildingIDs.Clear();
            BuildingIDs = LuaCommands.GetBuildingIds();
            GarrisonBase.Log("Found a total of {0} building IDs", BuildingIDs.Count);
        }
        internal static void RefreshBuildings()
        {
            GarrisonBase.Debug("Refreshing Buildings..");

            Buildings.Clear();
            HasDisenchant = false;
            HasForge = false;
            ForgeBuilding = BuildingType.Unknown;

            RefreshBuildingIDs();


            foreach (var id in BuildingIDs)
            {
                var b = new Building(id);
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

                    if (!Character.Player.IsAlliance)
                    {
                        if(b.Level==1)
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
                        else if(b.Level == 2)
                            DisenchantingEntryId = 228618;
                        else if (b.Level == 3)
                            DisenchantingEntryId = 228591;
                    }
                }

                if (b.Type == BuildingType.Barracks && b.Level == 3)
                    MaxActiveFollowers = 25;
                //GarrisonBase.Log("{0}", b.ToString());
            }

            //if not completed the quest to unlock than we manually insert the buildings (since they do not show up in the building ids)
            if (!Buildings.ContainsKey(BuildingType.Mines))
                Buildings.Add(BuildingType.Mines, new Building("61"));
            if (!Buildings.ContainsKey(BuildingType.HerbGarden))
                Buildings.Add(BuildingType.HerbGarden, new Building("29"));
        }

        public enum PrimalTraderItemTypes
        {
            SorcerousAir,
            SorcerousFire,
            SorcerousEarth,
            SorcerousWater,
            SavageBlood, 
            AlchemicalCatalyst,
            TruesteelIngot,
            GearspringParts,
            WarPaints,
            TaladiteCrystal,
            BurnishedLeather,
            HexweaveCloth,
        }
        public class PrimalTraderItem
        {
            public PrimalTraderItemTypes Type { get; set; }
            public string Name { get; set; }
            public int Cost { get; set; }

            public PrimalTraderItem(PrimalTraderItemTypes type, string name, int cost)
            {
                Type = type;
                Name = name;
                Cost = cost;
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
