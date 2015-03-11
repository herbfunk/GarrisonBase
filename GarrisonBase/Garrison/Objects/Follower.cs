using System;
using System.Collections.Generic;
using System.Linq;
using Herbfunk.GarrisonBase.Garrison.Enums;

namespace Herbfunk.GarrisonBase.Garrison.Objects
{
    public class Follower
    {
        public int Level { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Quality { get; set; }
        public int ItemLevel { get; set; }
        public int XP { get; set; }
        public int LevelXP { get; set; }
        public List<FollowerAbility> Abilities { get; set; }

        public Follower(int id, string name, int level, int itemlevel, int xp, int levelxp, string status,
            string quality, List<FollowerAbility> abilities)
        {
            ID = id;
            Name = name;
            Level = level;
            ItemLevel = itemlevel;
            XP = xp;
            LevelXP = levelxp;
            Status = status;
            Quality = quality;
            Abilities = abilities;
        }

        public override string ToString()
        {
            string abilityString = Abilities.Aggregate("", (current, a) => current + String.Format("Ability {0} (ID {1}) Counters {2} CounterID {3}", a.Name, a.ID, a.Counters, a.CounterID) + "\r\n");
            return
                String.Format(
                    "{0} (ID: {1}) Level {2} (ItemLevel {3}) -- XP {4} LevelXP {5} -- Quality {6} Status {7}\r\n{8}",
                    Name,ID, Level, ItemLevel, XP, LevelXP, Quality, Status, abilityString);
        }


        #region Follower Abilities List
        public static readonly List<FollowerAbility> FollowerAbilites = new List<FollowerAbility>
        {
            new FollowerAbility(5,"Blizzard",CombatAbilities.MinionSwarms,7),
            new FollowerAbility(6,"Shield Wall",CombatAbilities.MassiveStrike,2),
            new FollowerAbility(10,"Leap of Faith",CombatAbilities.DangerZones,6),
            new FollowerAbility(11,"Prayer of Healing",CombatAbilities.GroupDamage,3),
            new FollowerAbility(100,"Taunt",CombatAbilities.WildAggression,1),
            new FollowerAbility(101,"Multi-Shot",CombatAbilities.MinionSwarms,7),
            new FollowerAbility(102,"Heroic Leap",CombatAbilities.DangerZones,6),
            new FollowerAbility(103,"Rapid Fire",CombatAbilities.TimedBattle,10),
            new FollowerAbility(104,"Sap",CombatAbilities.DeadlyMinions,9),
            new FollowerAbility(105,"Kick",CombatAbilities.PowerfulSpell,8),
            new FollowerAbility(106,"Chain Heal",CombatAbilities.GroupDamage,3),
            new FollowerAbility(107,"Purify Spirit",CombatAbilities.MagicDebuff,4),
            new FollowerAbility(108,"Growl",CombatAbilities.WildAggression,1),
            new FollowerAbility(114,"Dark Command",CombatAbilities.WildAggression,1),
            new FollowerAbility(115,"Bone Shield",CombatAbilities.MassiveStrike,2),
            new FollowerAbility(116,"Death and Decay",CombatAbilities.MinionSwarms,7),
            new FollowerAbility(117,"Mind Freeze",CombatAbilities.PowerfulSpell,8),
            new FollowerAbility(118,"Empower Rune Weapon",CombatAbilities.TimedBattle,10),
            new FollowerAbility(119,"Anti-Magic Shell",CombatAbilities.MagicDebuff,4),
            new FollowerAbility(120,"Cleave",CombatAbilities.MinionSwarms,7),
            new FollowerAbility(121,"Pummel",CombatAbilities.PowerfulSpell,8),
            new FollowerAbility(122,"Recklessness",CombatAbilities.TimedBattle,10),
            new FollowerAbility(123,"Reckoning",CombatAbilities.WildAggression,1),
            new FollowerAbility(124,"Divine Shield",CombatAbilities.MassiveStrike,2),
            new FollowerAbility(125,"Cleanse",CombatAbilities.MagicDebuff,4),
            new FollowerAbility(126,"Rebuke",CombatAbilities.PowerfulSpell,8),
            new FollowerAbility(127,"Repentance",CombatAbilities.DeadlyMinions,9),
            new FollowerAbility(128,"Holy Radiance",CombatAbilities.GroupDamage,3),
            new FollowerAbility(129,"Divine Plea",CombatAbilities.TimedBattle,10),
            new FollowerAbility(130,"Divine Storm",CombatAbilities.MinionSwarms,7),
            new FollowerAbility(131,"Avenging Wrath",CombatAbilities.TimedBattle,10),
            new FollowerAbility(132,"Barkskin",CombatAbilities.MassiveStrike,2),
            new FollowerAbility(133,"Innervate",CombatAbilities.TimedBattle,10),
            new FollowerAbility(134,"Entangling Roots",CombatAbilities.DeadlyMinions,9),
            new FollowerAbility(135,"Wild Growth",CombatAbilities.GroupDamage,3),
            new FollowerAbility(136,"Nature's Cure",CombatAbilities.MagicDebuff,4),
            new FollowerAbility(137,"Hurricane",CombatAbilities.MinionSwarms,7),
            new FollowerAbility(138,"Berserk",CombatAbilities.TimedBattle,10),
            new FollowerAbility(139,"Celestial Alignment",CombatAbilities.TimedBattle,10),
            new FollowerAbility(140,"Provoke",CombatAbilities.WildAggression,1),
            new FollowerAbility(141,"Guard",CombatAbilities.MassiveStrike,2),
            new FollowerAbility(142,"Chi Wave",CombatAbilities.GroupDamage,3),
            new FollowerAbility(143,"Roll",CombatAbilities.DangerZones,6),
            new FollowerAbility(144,"Paralysis",CombatAbilities.DeadlyMinions,9),
            new FollowerAbility(145,"Detox",CombatAbilities.MagicDebuff,4),
            new FollowerAbility(146,"Mana Tea",CombatAbilities.TimedBattle,10),
            new FollowerAbility(147,"Spear Hand Strike",CombatAbilities.PowerfulSpell,8),
            new FollowerAbility(148,"Dispel Magic",CombatAbilities.MagicDebuff,4),
            new FollowerAbility(149,"Shadowfiend",CombatAbilities.TimedBattle,10),
            new FollowerAbility(150,"Mind Sear",CombatAbilities.MinionSwarms,7),
            new FollowerAbility(151,"Dominate Mind",CombatAbilities.DeadlyMinions,9),
            new FollowerAbility(152,"Power Infusion",CombatAbilities.TimedBattle,10),
            new FollowerAbility(153,"Water Shield",CombatAbilities.TimedBattle,10),
            new FollowerAbility(154,"Chain Lightning",CombatAbilities.MinionSwarms,7),
            new FollowerAbility(155,"Wind Shear",CombatAbilities.PowerfulSpell,8),
            new FollowerAbility(156,"Ghost Wolf",CombatAbilities.DangerZones,6),
            new FollowerAbility(157,"Hex",CombatAbilities.DeadlyMinions,9),
            new FollowerAbility(158,"Ascendance",CombatAbilities.TimedBattle,10),
            new FollowerAbility(159,"Evasion",CombatAbilities.MassiveStrike,2),
            new FollowerAbility(160,"Sprint",CombatAbilities.DangerZones,6),
            new FollowerAbility(161,"Fan of Knives",CombatAbilities.MinionSwarms,7),
            new FollowerAbility(162,"Marked for Death",CombatAbilities.TimedBattle,10),
            new FollowerAbility(163,"Feign Death",CombatAbilities.WildAggression,1),
            new FollowerAbility(164,"Deterrence",CombatAbilities.MassiveStrike,2),
            new FollowerAbility(165,"Disengage",CombatAbilities.DangerZones,6),
            new FollowerAbility(166,"Counter Shot",CombatAbilities.PowerfulSpell,8),
            new FollowerAbility(167,"Freezing Trap",CombatAbilities.DeadlyMinions,9),
            new FollowerAbility(168,"Ice Block",CombatAbilities.MassiveStrike,2),
            new FollowerAbility(169,"Conjure Food",CombatAbilities.TimedBattle,10),
            new FollowerAbility(170,"Blink",CombatAbilities.DangerZones,6),
            new FollowerAbility(171,"Counterspell",CombatAbilities.PowerfulSpell,8),
            new FollowerAbility(172,"Polymorph",CombatAbilities.DeadlyMinions,9),
            new FollowerAbility(173,"Time Warp",CombatAbilities.TimedBattle,10),
            new FollowerAbility(174,"Unending Resolve",CombatAbilities.MassiveStrike,2),
            new FollowerAbility(175,"Drain Life",CombatAbilities.GroupDamage,3),
            new FollowerAbility(176,"Singe Magic",CombatAbilities.MagicDebuff,4),
            new FollowerAbility(177,"Metamorphosis",CombatAbilities.TimedBattle,10),
            new FollowerAbility(178,"Rain of Fire",CombatAbilities.MinionSwarms,7),
            new FollowerAbility(179,"Spell Lock",CombatAbilities.PowerfulSpell,8),
            new FollowerAbility(180,"Fear",CombatAbilities.DeadlyMinions,9),
            new FollowerAbility(181,"Summon Infernal",CombatAbilities.TimedBattle,10),
            new FollowerAbility(182,"Dash",CombatAbilities.DangerZones,6),
            new FollowerAbility(183,"Energizing Brew",CombatAbilities.TimedBattle,10),

            //Enviorment, Race, and Misc
            new FollowerAbility(4,"Orcslayer",CombatAbilities.Orc,11),
            new FollowerAbility(7,"Mountaineer",CombatAbilities.Mountains,21),
            new FollowerAbility(8,"Cold-Blooded",CombatAbilities.Snow,23),
            new FollowerAbility(9,"Wastelander",CombatAbilities.Desert,22),
            new FollowerAbility(29,"Fast Learner",CombatAbilities.FastLearner),
            new FollowerAbility(36,"Demonslayer",CombatAbilities.Demon,16),
            new FollowerAbility(37,"Beastslayer",CombatAbilities.Beast,15),
            new FollowerAbility(38,"Ogreslayer",CombatAbilities.Ogre,12),
            new FollowerAbility(39,"Primalslayer",CombatAbilities.Primal,20),
            new FollowerAbility(40,"Gronnslayer",CombatAbilities.Breaker,17),
            new FollowerAbility(41,"Furyslayer",CombatAbilities.Fury,18),
            new FollowerAbility(42,"Voidslayer",CombatAbilities.Undead,19),
            new FollowerAbility(43,"Talonslayer",CombatAbilities.Arakkoa,14),
            new FollowerAbility(44,"Naturalist",CombatAbilities.Forest,26),
            new FollowerAbility(45,"Cave Dweller",CombatAbilities.Underground,24),
            new FollowerAbility(46,"Guerilla Fighter",CombatAbilities.Jungle,25),
            new FollowerAbility(47,"Urbanite",CombatAbilities.Town,27),
            new FollowerAbility(48,"Marshwalker",CombatAbilities.Swamp,28),
            new FollowerAbility(49,"Plainsrunner",CombatAbilities.Plains,29),
            new FollowerAbility(52,"Mining",CombatAbilities.Mining),
            new FollowerAbility(53,"Herbalism",CombatAbilities.Herbalism),
            new FollowerAbility(54,"Alchemy",CombatAbilities.Alchemy),
            new FollowerAbility(55,"Blacksmithing",CombatAbilities.Blacksmithing),
            new FollowerAbility(56,"Enchanting",CombatAbilities.Enchanting),
            new FollowerAbility(57,"Engineering",CombatAbilities.Engineering),
            new FollowerAbility(58,"Inscription",CombatAbilities.Inscription),
            new FollowerAbility(59,"Jewelcrafting",CombatAbilities.Jewelcrafting),
            new FollowerAbility(60,"Leatherworking",CombatAbilities.Leatherworking),
            new FollowerAbility(61,"Tailoring",CombatAbilities.Tailoring),
            new FollowerAbility(62,"Skinning",CombatAbilities.Skinning),
            new FollowerAbility(63,"Gnome-Lover",CombatAbilities.GnomeLover),
            new FollowerAbility(64,"Humanist",CombatAbilities.Humanist),
            new FollowerAbility(65,"Dwarvenborn",CombatAbilities.Dwarvenborn),
            new FollowerAbility(66,"Child of the Moon",CombatAbilities.ChildoftheMoon),
            new FollowerAbility(67,"Ally of Argus",CombatAbilities.AllyofArgus),
            new FollowerAbility(68,"Canine Companion",CombatAbilities.CanineCompanion),
            new FollowerAbility(69,"Brew Aficionado",CombatAbilities.BrewAficionado),
            new FollowerAbility(70,"Child of Draenor",CombatAbilities.ChildofDraenor),
            new FollowerAbility(71,"Death Fascination",CombatAbilities.DeathFascination),
            new FollowerAbility(72,"Totemist",CombatAbilities.Totemist),
            new FollowerAbility(73,"Voodoo Zealot",CombatAbilities.VoodooZealot),
            new FollowerAbility(74,"Elvenkind",CombatAbilities.Elvenkind),
            new FollowerAbility(75,"Economist",CombatAbilities.Economist),
            new FollowerAbility(76,"High Stamina",CombatAbilities.HighStamina),
            new FollowerAbility(77,"Burst of Power",CombatAbilities.BurstofPower),
            new FollowerAbility(78,"Lone Wolf",CombatAbilities.LoneWolf),
            new FollowerAbility(79,"Scavenger",CombatAbilities.Scavenger),
            new FollowerAbility(80,"Extra Training",CombatAbilities.ExtraTraining),
            new FollowerAbility(201,"Combat Experience",CombatAbilities.CombatExperience),
            new FollowerAbility(221,"Epic Mount",CombatAbilities.EpicMount),
            new FollowerAbility(227,"Angler",CombatAbilities.Swamp,28),
            new FollowerAbility(228,"Evergreen",CombatAbilities.Swamp,28),
            new FollowerAbility(231,"Bodyguard",CombatAbilities.Bodyguard),
            new FollowerAbility(232,"Dancer",CombatAbilities.DangerZones,6),
            new FollowerAbility(236,"Hearthstone Pro",CombatAbilities.HearthstonePro),
        };

        #endregion
    }
}
