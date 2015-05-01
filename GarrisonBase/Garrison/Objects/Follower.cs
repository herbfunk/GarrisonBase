using System;
using System.Collections.Generic;
using System.Linq;
using Herbfunk.GarrisonBase.Cache;
using Herbfunk.GarrisonBase.Character;
using Herbfunk.GarrisonBase.Coroutines;
using Herbfunk.GarrisonBase.Coroutines.Behaviors;
using Herbfunk.GarrisonBase.Garrison.Enums;
using Herbfunk.GarrisonBase.Helpers;
using Styx;
using Styx.Pathing;
using Styx.WoWInternals.Garrison;

namespace Herbfunk.GarrisonBase.Garrison.Objects
{
    public class Follower
    {
        internal readonly GarrisonFollower _refFollower;
        public int Level { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }

        public string Quality { get; set; }
        public int ItemLevel { get; set; }
        public int XP { get; set; }
        public int LevelXP { get; set; }
        public int AssignedBuildingId { get; set; }
        public List<FollowerAbility> Abilities { get; set; }

        public GarrisonFollowerStatus Status { get; set; }

        //public Follower(int id, string name, int level, int itemlevel, int xp, int levelxp, string status,
        //    string quality, List<FollowerAbility> abilities)
        //{
        //    ID = id;
        //    Name = name;
        //    Level = level;
        //    ItemLevel = itemlevel;
        //    XP = xp;
        //    LevelXP = levelxp;

        //    Quality = quality;
        //    Abilities = abilities;
        //}

        public Follower(GarrisonFollower follower)
        {
            _refFollower = follower;
            ID = (int)follower.Id;
            Name = follower.Name;
            Level = follower.Level;
            ItemLevel = follower.ItemLevel;
            XP = follower.LevelExperience;
            Status = follower.Status;

            if (Status == GarrisonFollowerStatus.Working)
                AssignedBuildingId = follower.InsideBuildingId;
            else
                AssignedBuildingId = -1;

            Abilities = new List<FollowerAbility>();

            //foreach (var ability in follower.AllAbilities)
            //{
            //    FollowerAbility fability = FollowerAbilites.First(a => a.ID == ability.Id) ?? FollowerAbilites[0];
            //    Abilities.Add(fability);
            //}
        }
        public bool Valid
        {
            get
            {
                return
                    _refFollower != null &&
                    _refFollower.BaseAddress != IntPtr.Zero;
            }
        }

        public override string ToString()
        {
            string abilityString = Abilities.Aggregate("", (current, a) => current + String.Format("Ability {0} (ID {1}) Counters {2} CounterID {3}", a.Name, a.Id, a.Counters, a.CounterId) + "\r\n");
            return
                String.Format(
                    "{0} (ID: {1}) Level {2} (ItemLevel {3}) -- XP {4} LevelXP {5} -- Quality {6} Status {7}\r\n{8}",
                    Name, ID, Level, ItemLevel, XP, LevelXP, Quality, Status.ToString(), abilityString);
        }

        #region Ability and Traits List

        public List<FollowerAbility> FAbilities = new List<FollowerAbility>
        {
            new FollowerAbility(4, "Orcslayer", AbilityCategory.FightingRacial, 11, CounterCategory.Racial),
            new FollowerAbility(5, "Blizzard", AbilityCategory.CounterMechanic, 7, CounterCategory.Mechanics),
            new FollowerAbility(6, "Shield Wall", AbilityCategory.CounterMechanic, 2, CounterCategory.Mechanics),
            new FollowerAbility(7, "Mountaineer", AbilityCategory.Enviormental, 21, CounterCategory.Enviormental),
            new FollowerAbility(8, "Cold-Blooded", AbilityCategory.Enviormental, 23, CounterCategory.Enviormental),
            new FollowerAbility(9, "Wastelander", AbilityCategory.Enviormental, 22, CounterCategory.Enviormental),
            new FollowerAbility(10, "Leap of Faith", AbilityCategory.CounterMechanic, 6, CounterCategory.Mechanics),
            new FollowerAbility(11, "Prayer of Healing", AbilityCategory.CounterMechanic, 3, CounterCategory.Mechanics),
            new FollowerAbility(29, "Fast Learner", AbilityCategory.RewardIncrease, -1, CounterCategory.None),
            new FollowerAbility(36, "Demonslayer", AbilityCategory.FightingRacial, 16, CounterCategory.Racial),
            new FollowerAbility(37, "Beastslayer", AbilityCategory.FightingRacial, 15, CounterCategory.Racial),
            new FollowerAbility(38, "Ogreslayer", AbilityCategory.FightingRacial, 12, CounterCategory.Racial),
            new FollowerAbility(39, "Primalslayer", AbilityCategory.FightingRacial, 20, CounterCategory.Racial),
            new FollowerAbility(40, "Gronnslayer", AbilityCategory.FightingRacial, 17, CounterCategory.Racial),
            new FollowerAbility(41, "Furyslayer", AbilityCategory.FightingRacial, 18, CounterCategory.Racial),
            new FollowerAbility(42, "Voidslayer", AbilityCategory.FightingRacial, 19, CounterCategory.Racial),
            new FollowerAbility(43, "Talonslayer", AbilityCategory.FightingRacial, 14, CounterCategory.Racial),
            new FollowerAbility(44, "Naturalist", AbilityCategory.Enviormental, 26, CounterCategory.Enviormental),
            new FollowerAbility(45, "Cave Dweller", AbilityCategory.Enviormental, 24, CounterCategory.Enviormental),
            new FollowerAbility(46, "Guerilla Fighter", AbilityCategory.Enviormental, 25, CounterCategory.Enviormental),
            new FollowerAbility(47, "Master Assassin", AbilityCategory.CounterMechanic, -1, CounterCategory.None),
            new FollowerAbility(48, "Marshwalker", AbilityCategory.Enviormental, 28, CounterCategory.Enviormental),
            new FollowerAbility(49, "Plainsrunner", AbilityCategory.Enviormental, 29, CounterCategory.Enviormental),
            new FollowerAbility(52, "Mining", AbilityCategory.Profession, -1, CounterCategory.None),
            new FollowerAbility(53, "Herbalism", AbilityCategory.Profession, -1, CounterCategory.None),
            new FollowerAbility(54, "Alchemy", AbilityCategory.Profession, 60, CounterCategory.Enviormental),
            new FollowerAbility(55, "Blacksmithing", AbilityCategory.Profession, 61, CounterCategory.Enviormental),
            new FollowerAbility(56, "Enchanting", AbilityCategory.Profession, 62, CounterCategory.Enviormental),
            new FollowerAbility(57, "Engineering", AbilityCategory.Profession, 63, CounterCategory.Enviormental),
            new FollowerAbility(58, "Inscription", AbilityCategory.Profession, 67, CounterCategory.Enviormental),
            new FollowerAbility(59, "Jewelcrafting", AbilityCategory.Profession, 64, CounterCategory.Enviormental),
            new FollowerAbility(60, "Leatherworking", AbilityCategory.Profession, 65, CounterCategory.Enviormental),
            new FollowerAbility(61, "Tailoring", AbilityCategory.Profession, 66, CounterCategory.Enviormental),
            new FollowerAbility(62, "Skinning", AbilityCategory.Profession, -1, CounterCategory.None),
            new FollowerAbility(63, "Gnome-Lover", AbilityCategory.PartyRacial, -1, CounterCategory.None),
            new FollowerAbility(64, "Humanist", AbilityCategory.PartyRacial, -1),
            new FollowerAbility(65, "Dwarvenborn", AbilityCategory.PartyRacial, -1, CounterCategory.None),
            new FollowerAbility(66, "Child of the Moon", AbilityCategory.PartyRacial, -1, CounterCategory.None),
            new FollowerAbility(67, "Ally of Argus", AbilityCategory.PartyRacial, -1, CounterCategory.None),
            new FollowerAbility(68, "Canine Companion", AbilityCategory.PartyRacial, -1, CounterCategory.None),
            new FollowerAbility(69, "Brew Aficionado", AbilityCategory.PartyRacial, -1, CounterCategory.None),
            new FollowerAbility(70, "Child of Draenor", AbilityCategory.PartyRacial, -1, CounterCategory.None),
            new FollowerAbility(71, "Death Fascination", AbilityCategory.PartyRacial, -1, CounterCategory.None),
            new FollowerAbility(72, "Totemist", AbilityCategory.PartyRacial, -1, CounterCategory.None),
            new FollowerAbility(73, "Voodoo Zealot", AbilityCategory.PartyRacial, -1, CounterCategory.None),
            new FollowerAbility(74, "Elvenkind", AbilityCategory.PartyRacial, -1),
            new FollowerAbility(75, "Economist", AbilityCategory.PartyRacial, -1, CounterCategory.None),
            new FollowerAbility(76, "High Stamina", AbilityCategory.Durartion, -1, CounterCategory.None),
            new FollowerAbility(77, "Burst of Power", AbilityCategory.Durartion, -1, CounterCategory.None),
            new FollowerAbility(78, "Lone Wolf", AbilityCategory.Solo, -1, CounterCategory.None),
            new FollowerAbility(79, "Scavenger", AbilityCategory.RewardIncrease, -1, CounterCategory.None),
            new FollowerAbility(80, "Extra Training", AbilityCategory.RewardIncrease, -1, CounterCategory.None),
            new FollowerAbility(100, "Taunt", AbilityCategory.CounterMechanic, 1, CounterCategory.Mechanics),
            new FollowerAbility(101, "Multi-Shot", AbilityCategory.CounterMechanic, 7, CounterCategory.Mechanics),
            new FollowerAbility(102, "Heroic Leap", AbilityCategory.CounterMechanic, 6, CounterCategory.Mechanics),
            new FollowerAbility(103, "Rapid Fire", AbilityCategory.CounterMechanic, 10, CounterCategory.Mechanics),
            new FollowerAbility(104, "Sap", AbilityCategory.CounterMechanic, 9, CounterCategory.Mechanics),
            new FollowerAbility(105, "Kick", AbilityCategory.CounterMechanic, 8, CounterCategory.Mechanics),
            new FollowerAbility(106, "Chain Heal", AbilityCategory.CounterMechanic, 3, CounterCategory.Mechanics),
            new FollowerAbility(107, "Purify Spirit", AbilityCategory.CounterMechanic, 4, CounterCategory.Mechanics),
            new FollowerAbility(108, "Growl", AbilityCategory.CounterMechanic, 1, CounterCategory.Mechanics),
            new FollowerAbility(114, "Dark Command", AbilityCategory.CounterMechanic, 1, CounterCategory.Mechanics),
            new FollowerAbility(115, "Bone Shield", AbilityCategory.CounterMechanic, 2, CounterCategory.Mechanics),
            new FollowerAbility(116, "Death and Decay", AbilityCategory.CounterMechanic, 7, CounterCategory.Mechanics),
            new FollowerAbility(117, "Mind Freeze", AbilityCategory.CounterMechanic, 8, CounterCategory.Mechanics),
            new FollowerAbility(118, "Empower Rune Weapon", AbilityCategory.CounterMechanic, 10,
                CounterCategory.Mechanics),
            new FollowerAbility(119, "Anti-Magic Shell", AbilityCategory.CounterMechanic, 4, CounterCategory.Mechanics),
            new FollowerAbility(120, "Cleave", AbilityCategory.CounterMechanic, 7, CounterCategory.Mechanics),
            new FollowerAbility(121, "Pummel", AbilityCategory.CounterMechanic, 8, CounterCategory.Mechanics),
            new FollowerAbility(122, "Recklessness", AbilityCategory.CounterMechanic, 10, CounterCategory.Mechanics),
            new FollowerAbility(123, "Reckoning", AbilityCategory.CounterMechanic, 1, CounterCategory.Mechanics),
            new FollowerAbility(124, "Divine Shield", AbilityCategory.CounterMechanic, 2, CounterCategory.Mechanics),
            new FollowerAbility(125, "Cleanse", AbilityCategory.CounterMechanic, 4, CounterCategory.Mechanics),
            new FollowerAbility(126, "Rebuke", AbilityCategory.CounterMechanic, 8, CounterCategory.Mechanics),
            new FollowerAbility(127, "Repentance", AbilityCategory.CounterMechanic, 9, CounterCategory.Mechanics),
            new FollowerAbility(128, "Holy Radiance", AbilityCategory.CounterMechanic, 3, CounterCategory.Mechanics),
            new FollowerAbility(129, "Divine Plea", AbilityCategory.CounterMechanic, 10, CounterCategory.Mechanics),
            new FollowerAbility(130, "Divine Storm", AbilityCategory.CounterMechanic, 7, CounterCategory.Mechanics),
            new FollowerAbility(131, "Avenging Wrath", AbilityCategory.CounterMechanic, 10, CounterCategory.Mechanics),
            new FollowerAbility(132, "Barkskin", AbilityCategory.CounterMechanic, 2, CounterCategory.Mechanics),
            new FollowerAbility(133, "Innervate", AbilityCategory.CounterMechanic, 10, CounterCategory.Mechanics),
            new FollowerAbility(134, "Entangling Roots", AbilityCategory.CounterMechanic, 9, CounterCategory.Mechanics),
            new FollowerAbility(135, "Wild Growth", AbilityCategory.CounterMechanic, 3, CounterCategory.Mechanics),
            new FollowerAbility(136, "Nature's Cure", AbilityCategory.CounterMechanic, 4, CounterCategory.Mechanics),
            new FollowerAbility(137, "Hurricane", AbilityCategory.CounterMechanic, 7, CounterCategory.Mechanics),
            new FollowerAbility(138, "Berserk", AbilityCategory.CounterMechanic, 10, CounterCategory.Mechanics),
            new FollowerAbility(139, "Celestial Alignment", AbilityCategory.CounterMechanic, 10,
                CounterCategory.Mechanics),
            new FollowerAbility(140, "Provoke", AbilityCategory.CounterMechanic, 1, CounterCategory.Mechanics),
            new FollowerAbility(141, "Guard", AbilityCategory.CounterMechanic, 2, CounterCategory.Mechanics),
            new FollowerAbility(142, "Chi Wave", AbilityCategory.CounterMechanic, 3, CounterCategory.Mechanics),
            new FollowerAbility(143, "Roll", AbilityCategory.CounterMechanic, 6, CounterCategory.Mechanics),
            new FollowerAbility(144, "Paralysis", AbilityCategory.CounterMechanic, 9, CounterCategory.Mechanics),
            new FollowerAbility(145, "Detox", AbilityCategory.CounterMechanic, 4, CounterCategory.Mechanics),
            new FollowerAbility(146, "Mana Tea", AbilityCategory.CounterMechanic, 10, CounterCategory.Mechanics),
            new FollowerAbility(147, "Spear Hand Strike", AbilityCategory.CounterMechanic, 8, CounterCategory.Mechanics),
            new FollowerAbility(148, "Dispel Magic", AbilityCategory.CounterMechanic, 4, CounterCategory.Mechanics),
            new FollowerAbility(149, "Shadowfiend", AbilityCategory.CounterMechanic, 10, CounterCategory.Mechanics),
            new FollowerAbility(150, "Mind Sear", AbilityCategory.CounterMechanic, 7, CounterCategory.Mechanics),
            new FollowerAbility(151, "Dominate Mind", AbilityCategory.CounterMechanic, 9, CounterCategory.Mechanics),
            new FollowerAbility(152, "Power Infusion", AbilityCategory.CounterMechanic, 10, CounterCategory.Mechanics),
            new FollowerAbility(153, "Water Shield", AbilityCategory.CounterMechanic, 10, CounterCategory.Mechanics),
            new FollowerAbility(154, "Chain Lightning", AbilityCategory.CounterMechanic, 7, CounterCategory.Mechanics),
            new FollowerAbility(155, "Wind Shear", AbilityCategory.CounterMechanic, 8, CounterCategory.Mechanics),
            new FollowerAbility(156, "Ghost Wolf", AbilityCategory.CounterMechanic, 6, CounterCategory.Mechanics),
            new FollowerAbility(157, "Hex", AbilityCategory.CounterMechanic, 9, CounterCategory.Mechanics),
            new FollowerAbility(158, "Ascendance", AbilityCategory.CounterMechanic, 10, CounterCategory.Mechanics),
            new FollowerAbility(159, "Evasion", AbilityCategory.CounterMechanic, 2, CounterCategory.Mechanics),
            new FollowerAbility(160, "Sprint", AbilityCategory.CounterMechanic, 6, CounterCategory.Mechanics),
            new FollowerAbility(161, "Fan of Knives", AbilityCategory.CounterMechanic, 7, CounterCategory.Mechanics),
            new FollowerAbility(162, "Marked for Death", AbilityCategory.CounterMechanic, 10, CounterCategory.Mechanics),
            new FollowerAbility(163, "Feign Death", AbilityCategory.CounterMechanic, 1, CounterCategory.Mechanics),
            new FollowerAbility(164, "Deterrence", AbilityCategory.CounterMechanic, 2, CounterCategory.Mechanics),
            new FollowerAbility(165, "Disengage", AbilityCategory.CounterMechanic, 6, CounterCategory.Mechanics),
            new FollowerAbility(166, "Counter Shot", AbilityCategory.CounterMechanic, 8, CounterCategory.Mechanics),
            new FollowerAbility(167, "Freezing Trap", AbilityCategory.CounterMechanic, 9, CounterCategory.Mechanics),
            new FollowerAbility(168, "Ice Block", AbilityCategory.CounterMechanic, 2, CounterCategory.Mechanics),
            new FollowerAbility(169, "Conjure Food", AbilityCategory.CounterMechanic, 10, CounterCategory.Mechanics),
            new FollowerAbility(170, "Blink", AbilityCategory.CounterMechanic, 6, CounterCategory.Mechanics),
            new FollowerAbility(171, "Counterspell", AbilityCategory.CounterMechanic, 8, CounterCategory.Mechanics),
            new FollowerAbility(172, "Polymorph", AbilityCategory.CounterMechanic, 9, CounterCategory.Mechanics),
            new FollowerAbility(173, "Time Warp", AbilityCategory.CounterMechanic, 10, CounterCategory.Mechanics),
            new FollowerAbility(174, "Unending Resolve", AbilityCategory.CounterMechanic, 2, CounterCategory.Mechanics),
            new FollowerAbility(175, "Drain Life", AbilityCategory.CounterMechanic, 3, CounterCategory.Mechanics),
            new FollowerAbility(176, "Singe Magic", AbilityCategory.CounterMechanic, 4, CounterCategory.Mechanics),
            new FollowerAbility(177, "Metamorphosis", AbilityCategory.CounterMechanic, 10, CounterCategory.Mechanics),
            new FollowerAbility(178, "Rain of Fire", AbilityCategory.CounterMechanic, 7, CounterCategory.Mechanics),
            new FollowerAbility(179, "Spell Lock", AbilityCategory.CounterMechanic, 8, CounterCategory.Mechanics),
            new FollowerAbility(180, "Fear", AbilityCategory.CounterMechanic, 9, CounterCategory.Mechanics),
            new FollowerAbility(181, "Summon Infernal", AbilityCategory.CounterMechanic, 10, CounterCategory.Mechanics),
            new FollowerAbility(182, "Dash", AbilityCategory.CounterMechanic, 6, CounterCategory.Mechanics),
            new FollowerAbility(183, "Energizing Brew", AbilityCategory.CounterMechanic, 10, CounterCategory.Mechanics),
            new FollowerAbility(201, "Combat Experience", AbilityCategory.CounterMechanic, -1, CounterCategory.None),
            new FollowerAbility(221, "Epic Mount", AbilityCategory.Durartion, -1, CounterCategory.None),
            new FollowerAbility(227, "Angler", AbilityCategory.CounterMechanic, 28, CounterCategory.Enviormental),
            new FollowerAbility(228, "Evergreen", AbilityCategory.CounterMechanic, 28, CounterCategory.Enviormental),
            new FollowerAbility(231, "Bodyguard", AbilityCategory.CounterMechanic, -1, CounterCategory.None),
            new FollowerAbility(232, "Dancer", AbilityCategory.FightingRacial, 6, CounterCategory.Mechanics),
            new FollowerAbility(236, "Hearthstone Pro", AbilityCategory.CounterMechanic, -1, CounterCategory.None),
            new FollowerAbility(248, "Mentor", AbilityCategory.Enviormental, -1, CounterCategory.None),
            new FollowerAbility(249, "Explorer Extraordinaire", AbilityCategory.CounterMechanic, 6,
                CounterCategory.Mechanics),
            new FollowerAbility(250, "Speed of Light", AbilityCategory.CounterMechanic, -1, CounterCategory.None),
            new FollowerAbility(252, "Ogre Buddy", AbilityCategory.PartyRacial, -1, CounterCategory.None),
            new FollowerAbility(253, "Mechano Affictionado", AbilityCategory.PartyRacial, -1, CounterCategory.None),
            new FollowerAbility(254, "Bird Watcher", AbilityCategory.PartyRacial, -1, CounterCategory.None),
            new FollowerAbility(255, "Wildling", AbilityCategory.PartyRacial, -1, CounterCategory.None),
            new FollowerAbility(256, "Treasure Hunter", AbilityCategory.RewardIncrease, -1, CounterCategory.None),

        };

        #endregion

        public static BehaviorArray FollowerQuestBehaviorArray(int followerid)
        {
            #region Tormmok (193)

            if (followerid == 193)
            {//Tormmok
                //Final quest id 36037

                //Move to location
                var movementLocation = new WoWPoint(4852.024, 1390.837, 144.9443);
                var flightPath = new BehaviorUseFlightPath(movementLocation);

                var moveBehavior = new BehaviorMove(movementLocation);
                //<Hotspot X="4852.024" Y="1390.837" Z="144.9443" />
                var hotspotBehavior =
                    new BehaviorHotspotRunning(
                    new[] { movementLocation },
                    new uint[] { 83827, 83824, 83828, 83826, 83871 },
                    BehaviorHotspotRunning.HotSpotType.Killing,
                    () => BehaviorManager.ObjectNotValidOrNotFound(83820) || !BehaviorManager.CanInteractWithUnit(83820));

                //Clear area of specific mobs and wait for npc to become friendly
                //hostile ids: 83827, 83824, 83828, 83826, 83871

                //if follower is friendly and without any quest giver status
                //  -interact with him gossip select option one twice then close dialog
                var gossipBehavior = new BehaviorGossipInteract(83820, 0);
                //if follower is friendly and with avaialble quest giver status
                //  -interact with him selecting quest then completing it
                var questBehavior = new BehaviorQuestPickup(36037, movementLocation, 83820, true);

                var newArray = new BehaviorArray(new Behavior[]
                {
                    flightPath,
                    moveBehavior, 
                    hotspotBehavior, 
                    gossipBehavior, 
                    questBehavior
                });

                newArray.Criteria += () => BaseSettings.CurrentSettings.FollowerOptionalList.Contains(followerid) &&
                                           !GarrisonManager.FollowerIdsCollected.Contains(followerid);
                return newArray;
            }

            #endregion

            #region Blook (189)

            if (followerid == 189)
            { //Blook
                //Final quest id 34279


                var movementLocation = new WoWPoint(4605.237, 1690.607, 234.7461);
                var flightPath = new BehaviorUseFlightPath(movementLocation);
                var moveBehavior = new BehaviorMove(movementLocation);
                var gossipBehavior = new BehaviorGossipInteract(78030, 0, false, () => BehaviorManager.ObjectNotValidOrNotFound(78030) || BehaviorManager.CanInteractWithUnit(78030));
                var hotspotBehaviorkill =
                   new BehaviorHotspotRunning(
                   new[] { movementLocation },
                   new uint[] { 78030 },
                   BehaviorHotspotRunning.HotSpotType.Killing,
                   () => BehaviorManager.ObjectNotValidOrNotFound(78030) || !BehaviorManager.CanInteractWithUnit(78030) || !BehaviorManager.UnitHasQuestGiverStatus(78030, QuestGiverStatus.Available));

                //var sleep = new BehaviorSleep(StyxWoW.Random.Next(4444, 5555));
                var questBehavior = new BehaviorQuestPickup(34279, movementLocation, 78030, true);

                BehaviorArray newArray = new BehaviorArray(new Behavior[]
                {
                    flightPath,
                    moveBehavior, 
                    gossipBehavior, 
                    hotspotBehaviorkill,
                    //sleep, 
                    questBehavior
                });
                newArray.Criteria += () => BaseSettings.CurrentSettings.FollowerOptionalList.Contains(followerid) &&
                                            !GarrisonManager.FollowerIdsCollected.Contains(followerid);
                return newArray;
            }

            #endregion

            #region Defender Illona / Aeda Brightdawn (207)

            if (followerid == 207)
            {
                uint questId = Player.IsAlliance ? Convert.ToUInt32(34777) : Convert.ToUInt32(34776);
                var questNPC = Player.IsAlliance ? 79979 : 79978;
                uint[] mobIds = { 79970, 79977 };
                WoWPoint npcLoc = Player.IsAlliance ? new WoWPoint(2398.989, 2402.796, 126.5605)
                                                    : new WoWPoint(2327.925, 2369.851, 126.5607);

                WoWPoint killLoc = new WoWPoint(2336.869, 2409.208, 126.5612);

                var flightPath = new BehaviorUseFlightPath(killLoc);

                var moveBehavior = new BehaviorMove(npcLoc);

                //Pickup Quest
                var questPickup = new BehaviorQuestPickup(questId, npcLoc, questNPC);
                questPickup.Criteria += () => !LuaCommands.IsQuestFlaggedCompleted(questId.ToString());

                //Move to Kill Zone
                var moveKillBehavior = new BehaviorMove(killLoc);
                moveKillBehavior.Criteria += () => BehaviorManager.HasQuestAndNotCompleted(questId);

                //Interact With First NPC
                var gossipBehavior = new BehaviorGossipInteract(Convert.ToInt32(mobIds[0]), 0);
                gossipBehavior.Criteria += () => BehaviorManager.HasQuestAndNotCompleted(questId);

                //Kill until Not Attackable
                var hotspotBehavior =
                   new BehaviorHotspotRunning(
                   new[] { killLoc },
                   new uint[] { mobIds[0] },
                   BehaviorHotspotRunning.HotSpotType.Killing,
                   () => BehaviorManager.ObjectNotValidOrNotFound(mobIds[0]) ||
                         BehaviorManager.CanAttackUnit(mobIds[0]));
                hotspotBehavior.Criteria += () => BehaviorManager.HasQuestAndNotCompleted(questId);


                //Interact With Second NPC
                var gossipBehavior2 = new BehaviorGossipInteract(Convert.ToInt32(mobIds[1]), 0);
                gossipBehavior2.Criteria += () => BehaviorManager.HasQuestAndNotCompleted(questId);

                //Kill until Not Attackable
                var hotspotBehavior2 =
                   new BehaviorHotspotRunning(
                   new[] { killLoc },
                   new uint[] { mobIds[1] },
                   BehaviorHotspotRunning.HotSpotType.Killing,
                   () => BehaviorManager.ObjectNotValidOrNotFound(mobIds[1]) ||
                         BehaviorManager.CanAttackUnit(mobIds[1]));
                hotspotBehavior2.Criteria += () => BehaviorManager.HasQuestAndNotCompleted(questId);

                //Turn in Quest
                var questTurnin = new BehaviorQuestTurnin(questId, npcLoc, questNPC);
                questTurnin.Criteria += () => BehaviorManager.HasQuest(questId);

                //Final Quest
                uint finalQuestID = Player.IsAlliance ? Convert.ToUInt32(36519) : Convert.ToUInt32(36518);
                var finalquestTurnin = new BehaviorQuestPickup(finalQuestID, npcLoc, questNPC, true);
                finalquestTurnin.Criteria += () => LuaCommands.IsQuestFlaggedCompleted(questId.ToString());

                BehaviorArray newArray = new BehaviorArray(new Behavior[]
                {
                    flightPath,
                    moveBehavior, questPickup, moveKillBehavior,
                    gossipBehavior,hotspotBehavior, 
                    gossipBehavior2, hotspotBehavior2,
                    questTurnin,
                    finalquestTurnin,
                });
                newArray.Criteria += () => !QuestHelper.QuestLogFull &&
                                            BaseSettings.CurrentSettings.FollowerOptionalList.Contains(followerid) &&
                                            !GarrisonManager.FollowerIdsCollected.Contains(followerid);
                return newArray;
            }

            #endregion

            #region Fen Tao (467)

            if (followerid == 467)
            {
                var moveLoc = Player.IsAlliance
                   ? new WoWPoint(3589.48, 3935.393, 21.33015)
                   : new WoWPoint(5300.374, 3966.365, 18.41501);

                var flightPath = new BehaviorUseFlightPath(moveLoc);

                var moveBehavior = new BehaviorMove(moveLoc);

                var npcId = 91483;
                var gossipBehavior = new BehaviorGossipInteract(npcId, 0);
                var clickStaticPop = new BehaviorClickStaticPopup(1);

                BehaviorArray newArray = new BehaviorArray(new Behavior[]
                {
                    flightPath,
                    moveBehavior,
                    gossipBehavior,
                    clickStaticPop,
                });
                newArray.Criteria += () => BaseSettings.CurrentSettings.FollowerOptionalList.Contains(followerid) &&
                                            !GarrisonManager.FollowerIdsCollected.Contains(followerid);
                return newArray;
            }

            #endregion

            #region Image of Archmage Vargoth (190)

            if (followerid == 190)
            {


                /* Mysterious Ring Gorgrond
                 * <WoWItem Name="Mysterious Ring" Entry="110459" />
                 */

                uint questidRing = 34463;
                uint itemidRing = 110459;
                var locRing = new WoWPoint(7414.815, 1820.695, 88.25874);
                var flightPathRing = new BehaviorUseFlightPath(locRing);
                var questPickupRing = new BehaviorQuestPickup(questidRing, locRing, 229330);
                BehaviorArray arrayRing = new BehaviorArray(new Behavior[]
                {
                    flightPathRing,
                    questPickupRing,
                });
                arrayRing.Criteria += () =>
                    !LuaCommands.IsQuestFlaggedCompleted(questidRing.ToString()) &&
                    Player.Inventory.BagItems.Values.All(item => item.Entry != itemidRing) &&
                    Player.Inventory.BankItems.Values.All(item => item.Entry != itemidRing);



                /* Mysterious Staff Nagrand
                 */
                uint questidStaff = 34466;
                uint itemidStaff = 110471;
                var locStaff = new WoWPoint(4316.793, 6675.973, 12.18058);
                var flightPathStaff = new BehaviorUseFlightPath(locStaff);
                var questPickupStaff = new BehaviorQuestPickup(questidStaff, locStaff, 229344);
                BehaviorArray arrayStaff = new BehaviorArray(new Behavior[]
                {
                    flightPathStaff,
                    questPickupStaff,
                });
                arrayStaff.Criteria += () =>
                    !LuaCommands.IsQuestFlaggedCompleted(questidRing.ToString()) &&
                    Player.Inventory.BagItems.Values.All(item => item.Entry != itemidStaff) &&
                    Player.Inventory.BankItems.Values.All(item => item.Entry != itemidStaff);

                /* Mysterious Boots Frostfire Ridge
                 */
                uint questidBoots = 34464;
                uint itemidBoots = 110469;
                var locBoots = new WoWPoint(7503.059, 3348.046, 150.563);
                var flightPathBoots = new BehaviorUseFlightPath(locBoots);
                var questPickupBoots = new BehaviorQuestPickup(questidBoots, locBoots, 229333);
                BehaviorArray arrayBoots = new BehaviorArray(new Behavior[]
                {
                    flightPathBoots,
                    questPickupBoots,
                });
                arrayBoots.Criteria += () =>
                    !LuaCommands.IsQuestFlaggedCompleted(questidRing.ToString()) &&
                    Player.Inventory.BagItems.Values.All(item => item.Entry != itemidBoots) &&
                    Player.Inventory.BankItems.Values.All(item => item.Entry != itemidBoots);

                /*  Mysterious Hat Taladore
                 */
                uint questidHat = 34465;
                uint itemidHat = 110470;
                var locHat = new WoWPoint(2955.57, 3133.576, 33.54591);
                var flightPathHat = new BehaviorUseFlightPath(locHat);

                //Elevator
                //"X="" Y="3009.481" Z="" 
                var transportTop = new WoWPoint(2681.371, 3009.481, 233.3512);
                var transportBottom = new WoWPoint(2681.371, 3009.481, 69.04865);
                var transportWaitPointTop = new WoWPoint(2677.393, 3004.31, 234.1084);
                var transportWaitPointBottom = new WoWPoint(2685.759, 3015.352, 69.64091);
                var transportTopStandPoint = new WoWPoint(2681.655, 3009.669, 233.9413);
                var transportBottomStandPoint = new WoWPoint(2682.094, 3010.166, 69.60918);
                var transportBottomGetOffPoint = new WoWPoint(2687.095, 3016.995, 69.4741);
                var transportTopGetOffPoint = new WoWPoint(2675.066, 3001.215, 233.9413);

                var ElevatorId = 231934;
                var elevatorDown = new BehaviorTransport(ElevatorId, transportTop, transportBottom, transportWaitPointTop, transportTopStandPoint,
                    transportBottomGetOffPoint, Movement.MovementTypes.Normal, "ElevatorDown");

                var elevatorUp = new BehaviorTransport(ElevatorId, transportBottom, transportTop, transportWaitPointBottom, transportBottomStandPoint,
                     transportTopGetOffPoint, Movement.MovementTypes.Normal, "ElevatorUp");
                elevatorUp.Criteria += () => Player.Location.Distance(locHat) <= 50f;

                var movePickupHat = new BehaviorMove(locHat);
                var questPickupHat = new BehaviorQuestPickup(questidHat, locHat, 229331);



                BehaviorArray arrayHat = new BehaviorArray(new Behavior[]
                {
                    flightPathHat,
                    elevatorDown,
                    movePickupHat,
                    questPickupHat,
                });
                arrayHat.Criteria += () =>
                    !LuaCommands.IsQuestFlaggedCompleted(questidRing.ToString()) &&
                    Player.Inventory.BagItems.Values.All(item => item.Entry != itemidHat) &&
                    Player.Inventory.BankItems.Values.All(item => item.Entry != itemidHat);


                var questTurninLoc = new WoWPoint(3193.986, 764.3624, 78.33575);
                var questTurninNpcId = 86949;

                var questTurninRing = new BehaviorQuestTurnin(questidRing, questTurninLoc, questTurninNpcId);
                questTurninRing.Criteria += () =>
                                    BehaviorManager.HasQuest(questidRing) &&
                                    Player.Inventory.BagItems.Values.Any(item => item.Entry == itemidRing);

                var questTurninStaff = new BehaviorQuestTurnin(questidStaff, questTurninLoc, questTurninNpcId);
                questTurninStaff.Criteria += () =>
                                    BehaviorManager.HasQuest(questidStaff) &&
                                    Player.Inventory.BagItems.Values.Any(item => item.Entry == itemidStaff);

                var questTurninHat = new BehaviorQuestTurnin(questidHat, questTurninLoc, questTurninNpcId);
                questTurninHat.Criteria += () =>
                                    BehaviorManager.HasQuest(questidHat) &&
                                    Player.Inventory.BagItems.Values.Any(item => item.Entry == itemidHat);

                var questTurninBoots = new BehaviorQuestTurnin(questidBoots, questTurninLoc, questTurninNpcId);
                questTurninBoots.Criteria += () =>
                                   BehaviorManager.HasQuest(questidBoots) &&
                                   Player.Inventory.BagItems.Values.Any(item => item.Entry == itemidBoots);

                BehaviorArray arrayQuestTurnin = new BehaviorArray(new Behavior[]
                {
                    questTurninRing,
                    questTurninHat,
                    questTurninBoots,
                    questTurninStaff
                });


                var finalQuestLoc = new WoWPoint(3171.512, 791.6653, 76.849);
                var questPickupFinalPart1 = new BehaviorQuestPickup(34472, questTurninLoc, questTurninNpcId);
                questPickupFinalPart1.Criteria += () => !BehaviorManager.HasQuest(34472);

                var hotspotBehavior =
                    new BehaviorHotspotRunning(
                    new[] { finalQuestLoc },
                    new uint[] { },
                    BehaviorHotspotRunning.HotSpotType.Looting,
                    () => BehaviorManager.HasQuestAndNotCompleted(34472) &&
                        (BehaviorManager.ObjectNotValidOrNotFound(77853) ||
                          !BehaviorManager.UnitHasQuestGiverStatus(77853, QuestGiverStatus.TurnIn)));

                var questTurninFinal = new BehaviorQuestTurnin(34472, finalQuestLoc, 77853);

                BehaviorArray arrayQuestTurninFinalPart1 = new BehaviorArray(new Behavior[]
                {
                    questPickupFinalPart1,
                    hotspotBehavior,
                    questTurninFinal
                });


                var questPickupFinalPart2 = new BehaviorQuestPickup(36027, finalQuestLoc, 77853, true);

                BehaviorArray newArray = new BehaviorArray(new Behavior[]
                {
                    arrayRing,
                    arrayStaff,
                    arrayHat,
                    elevatorUp,
                    arrayBoots,
                    arrayQuestTurnin,
                    arrayQuestTurninFinalPart1,
                    questPickupFinalPart2,
                });
                newArray.Criteria += () => !QuestHelper.QuestLogFull &&
                                            BaseSettings.CurrentSettings.FollowerOptionalList.Contains(followerid) &&
                                            !GarrisonManager.FollowerIdsCollected.Contains(followerid);
                return newArray;
            }

            #endregion

            #region Goldmaneg the Skinner (170)

            if (followerid == 170)
            {
                //kill unit 80080 and loot for item 111863
                //if have item, open cage, and complete quest 35596 with unit 80083
                WoWPoint _killposition = new WoWPoint(2034.626, 7016.884, 51.07641);
                var flightPath = new BehaviorUseFlightPath(_killposition);

                var hotspotBehaviorkill =
                  new BehaviorHotspotRunning(
                  new[] { _killposition },
                  new uint[] { 80080 },
                  BehaviorHotspotRunning.HotSpotType.Both,
                  () => Player.Inventory.GetBagItemsById(111863).Count == 0);

                var interactCage = new BehaviorGossipInteract(80083, 0, true,
                    () => !BehaviorManager.UnitHasQuestGiverStatus(80083, QuestGiverStatus.Available));

                var questBehavior = new BehaviorQuestPickup(35596, _killposition, 80083, true);

                BehaviorArray newArray = new BehaviorArray(new Behavior[]
                {
                    flightPath,
                    hotspotBehaviorkill,
                    interactCage,
                    questBehavior
                });

                //
                newArray.Criteria += () => !GarrisonManager.FollowerIdsCollected.Contains(followerid)
                            && BaseSettings.CurrentSettings.FollowerOptionalList.Contains(followerid);

                return newArray;
            }

            #endregion

            #region Abu'gar (209)
            if (followerid == 209)
            {
                //Collect 3 items
                //Talk to NPC

                //First Item - Vitality
                //object id: 233157
                //<GameObject Name="Abu'gar's Vitality" Entry="233157" X="2609.344" Y="5572.999" Z="91.78837" />
                //item id: 114242
                int vitality_objectId = 233157;
                int vitality_itemId = 114242;
                uint vitality_questId = 35711;

                //The object is located on a cliff only accessible by jumping from a higher cliff..
                //To achieve this I setup the top cliff position as normal movement
                //Once at the top, it uses click to move to "jump" onto the below cliff.
                //To prevent stucks, I added a custom action that tests navigation -- false means it worked.
                //I also modified the click to move to end if it gets stuck for +3 seconds
                var moveto_vitality_move1 = new BehaviorMove(new WoWPoint(2596.605, 5597.822, 126.3142), 1f);
                WoWPoint vitality_loc2 = new WoWPoint(2609.613, 5579.829, 90.33648);
                var moveto_vitality_move2 = new BehaviorMove(vitality_loc2, 1f, Movement.MovementTypes.ClickToMove, true);

                var custom_vitality_check = new BehaviorCustomAction(null, null, false);
                custom_vitality_check.CustomAction = () => Behavior.ResetBehavior(custom_vitality_check);
                custom_vitality_check.CustomCondition = () => !Navigator.CanNavigateFully(Player.Location, vitality_loc2);

                var gossip_vitality = new BehaviorGossipInteract(vitality_objectId, 0, true,
                                        () => Player.Inventory.GetBagItemsById(vitality_itemId).Count == 0);


                var barray_item_vitality = new BehaviorArray(
                    new Behavior[]
                    {
                        moveto_vitality_move1, 
                        moveto_vitality_move2, 
                        custom_vitality_check,
                        gossip_vitality,
                    });
                barray_item_vitality.Criteria += () => Player.Inventory.GetBagItemsById(114242).Count == 0 &&
                                                       !LuaCommands.IsQuestFlaggedCompleted(vitality_questId.ToString());

                //Finally if we are still on the lower cliff we need to click to move into the water!
                var moveto_vitality_finished = new BehaviorMove(new WoWPoint(2614.899, 5585.659, 77.63692), 10f, Movement.MovementTypes.ClickToMove);
                moveto_vitality_finished.Criteria += () => Player.Location.Distance(vitality_loc2) <= 20f;


                //Second Item - Lure
                //<GameObject Name="Abu'Gar's Favorite Lure" Entry="233642" X="3056.673" Y="7132.38" Z="1.907172" />

                int lure_objectId = 233642;
                int lure_itemId = 114245;
                uint lure_questId = 36072;
                WoWPoint lure_moveLoc = new WoWPoint(3051.612, 7142.25, 3.229406);
                var moveto_lure_move1 = new BehaviorMove(lure_moveLoc);
                var moveto_lure_move2 = new BehaviorMove(lure_moveLoc, 2f, Movement.MovementTypes.ClickToMove);
                var gossip_lure = new BehaviorGossipInteract(lure_objectId, 0, true,
                                       () => Player.Inventory.GetBagItemsById(lure_itemId).Count == 0);

                var barray_item_lure = new BehaviorArray(
                    new Behavior[]
                    {
                        moveto_lure_move1, 
                        moveto_lure_move2, 
                        gossip_lure,
                    });
                barray_item_lure.Criteria += () => Player.Inventory.GetBagItemsById(lure_itemId).Count == 0 &&
                                                       !LuaCommands.IsQuestFlaggedCompleted(lure_questId.ToString());


                //Third Item - Reel
                //<GameObject Name="Abu'gar's Missing Reel" Entry="233506" X="3457.177" Y="4460.172" Z="160.5695" />
                int reel_objectId = 233506;
                int reel_itemId = 114243;
                uint reel_questId = 36089;

                WoWPoint reel_moveLoc = new WoWPoint(3459.47, 4459.924, 160.8241);
                var moveto_reel_move = new BehaviorMove(reel_moveLoc);
                var gossip_reel = new BehaviorGossipInteract(reel_objectId, 0, true,
                                       () => Player.Inventory.GetBagItemsById(reel_itemId).Count == 0);
                var barray_item_reel = new BehaviorArray(
                   new Behavior[]
                    {
                        moveto_reel_move,
                        gossip_reel,
                    });
                barray_item_reel.Criteria += () => Player.Inventory.GetBagItemsById(reel_itemId).Count == 0 &&
                                                       !LuaCommands.IsQuestFlaggedCompleted(reel_questId.ToString());

                //Finally, once we have all 3 items, we goto Abu'Gur and turn the quest in!
                uint final_questId = 36711;
                int final_npcId = 82746;

                WoWPoint final_loc = new WoWPoint(2802.82, 5497.004, 11.9688);
                var final_movement = new BehaviorMove(final_loc, 10f);
                var final_questPickup = new BehaviorQuestPickup(final_questId, final_loc, final_npcId, true);
                var barray_final = new BehaviorArray(
                   new Behavior[]
                    {
                        final_movement,
                        final_questPickup,
                    });
                barray_final.Criteria +=
                    () => Player.Inventory.GetBagItemsById(new[] { reel_itemId, lure_itemId, vitality_itemId }).Count == 3;

                var returnArray = new BehaviorArray(new Behavior[]
                {
                    barray_item_vitality,
                    moveto_vitality_finished,
                    barray_item_lure,
                    barray_item_reel,
                    barray_final
                });
                returnArray.Criteria += () => BaseSettings.CurrentSettings.FollowerOptionalList.Contains(followerid) &&
                                            !GarrisonManager.FollowerIdsCollected.Contains(followerid);
                return returnArray;
            }
            #endregion

            #region Dagg (32)
            if (followerid == 32)
            {
                uint daggQuestId = 34733;
                var garrisonMovePoint = Player.IsAlliance ? new WoWPoint(1740.166, 162.6373, 74.76965) :
                                                            new WoWPoint(5736.961, 4499.52, 131.8965);
                var daggGarrisonEntryid = 79492;

                var daggGarrisonMovement = new BehaviorMove(garrisonMovePoint, 15f);
                var daggQuestTurnin = new BehaviorQuestPickup(daggQuestId, garrisonMovePoint, daggGarrisonEntryid, true);
                daggQuestTurnin.Criteria += () =>
                {
                    var units = ObjectCacheManager.GetUnitsNearPoint(garrisonMovePoint, 100f);
                    return units.Count > 0 && units.Any(u => u.Entry == daggGarrisonEntryid);
                };

                var daggFinalQuestArray = new BehaviorArray(new Behavior[]
                {
                    daggGarrisonMovement,
                    daggQuestTurnin
                });
                daggFinalQuestArray.Criteria += () => !LuaCommands.IsQuestFlaggedCompleted(daggQuestId.ToString());

                //Free him from two cages found in Frostfire ridge
                //Meet him in front of garrison and complete quest

                //Cage One
                var cage1Point = new WoWPoint(5794.33, 3476.851, 130.5595);
                var dagg1Entryid = 79473;
                var cage1Entryid = 230498;

                var cage1Movement = new BehaviorMove(new WoWPoint(5791.311, 3470.227, 132.307), 5f);
                var cage1Gossip = new BehaviorGossipInteract(cage1Entryid, 7f, false);
                cage1Gossip.RunCondition += () =>
                {
                    var units = ObjectCacheManager.GetUnitsNearPoint(cage1Point, 5f);
                    return units.Count > 0 && units.Any(u => u.Entry == dagg1Entryid);
                };


                //Cage Two
                var cage2Point = new WoWPoint(7130.164, 5086.299, 91.04395);
                var dagg2Entryid = 79488;
                var cage2Entryid = 230506;

                var cage2Movement = new BehaviorMove(new WoWPoint(7134.694, 5079.974, 91.04465), 5f);
                var cage2Gossip = new BehaviorGossipInteract(cage2Entryid, 7f, false);
                cage2Gossip.RunCondition += () =>
                {
                    var units = ObjectCacheManager.GetUnitsNearPoint(cage2Point, 5f);
                    return units.Count > 0 && units.Any(u => u.Entry == dagg2Entryid);
                };



                var barray = new BehaviorArray(new Behavior[]
                {
                    daggFinalQuestArray,

                    cage1Movement,
                    cage1Gossip,

                    cage2Movement,
                    cage2Gossip,

                    daggFinalQuestArray
                });
                barray.Criteria += () => BaseSettings.CurrentSettings.FollowerOptionalList.Contains(followerid) &&
                                           !GarrisonManager.FollowerIdsCollected.Contains(followerid);

                return barray;
            }
            #endregion


            return null;
        }


        public enum AbilityCategory
        {
            None = -1,
            CounterMechanic = 0,
            FightingRacial = 1,
            PartyRacial = 4,
            Profession = 5,
            Enviormental = 6,
            RewardIncrease = 7,
            Durartion = 8,
            Solo = 9
        }
        public enum CounterCategory
        {
            None = -1,
            Enviormental = 0,
            Racial = 1,
            Mechanics = 2
        }

        public class FollowerAbility
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public CombatAbilities Counters { get; set; }
            public int CounterId { get; set; }
            public AbilityCategory Category { get; set; }
            public CounterCategory CounterCategory { get; set; }

            public FollowerAbility(int id, string name, CombatAbilities counter, int counterid = -1)
            {
                Id = id;
                Name = name;
                Counters = counter;
                CounterId = counterid;
            }

            public FollowerAbility(int id, string name, AbilityCategory category, int counterid = -1, CounterCategory countercategory = CounterCategory.None)
            {
                Id = id;
                Name = name;
                Category = category;
                CounterCategory = countercategory;
                CounterId = counterid;
            }
        }
    }
}
