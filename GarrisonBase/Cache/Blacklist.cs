using System;
using System.Collections.Generic;
using Styx.Common.Helpers;
using Styx.WoWInternals;

namespace Herbfunk.GarrisonBase.Cache
{
    public static class Blacklist
    {
        public static HashSet<uint> BlacklistEntryIDs = new HashSet<uint>();
        public static HashSet<uint> TempBlacklistEntryIDs = new HashSet<uint>();
        public static HashSet<WoWGuid> TempBlacklistGuids = new HashSet<WoWGuid>();
        private static WaitTimer TempBlacklistEntryIDsTimer, TempBlacklistGuidsTimer;

        public static void Initalize(bool IsAlliance)
        {
            BlacklistEntryIDs = new HashSet<uint>(_blacklistEntryIds);

            if (IsAlliance)
                BlacklistEntryIDs.UnionWith(_blacklistEntryIds_Alliance);
            else
                BlacklistEntryIDs.UnionWith(_blacklistEntryIds_Horde);

            TempBlacklistEntryIDsTimer = new WaitTimer(new TimeSpan(0, 0, 1, 0));
            TempBlacklistGuidsTimer = new WaitTimer(new TimeSpan(0, 0, 1, 0));
        }

        internal static void CheckTempBlacklists()
        {
            if (TempBlacklistEntryIDsTimer.IsFinished)
            {
                TempBlacklistEntryIDsTimer.Reset();
                TempBlacklistEntryIDs.Clear();
            }

            if (TempBlacklistGuidsTimer.IsFinished)
            {
                TempBlacklistGuidsTimer.Reset();
                TempBlacklistGuids.Clear();
            }
        }

        #region Alliance Blacklist Collection

        private static readonly HashSet<uint> _blacklistEntryIds_Alliance = new HashSet<uint>
        {
            //NPCs
            77361, //Miner
            77370, //Lunarfall Footman
            85312, //Lunarfall Rifleman
            85782, //Druid of the Talon
            88564, //Druid of the Talon
            77903, //Pug
            77376, //Lunarfall Woodcutter
            77617, //Lunarfall Worker
            81653, //Lunarfall Laborer

            79603, //Emote Bunny
            81163, //Garrison - Horde - Fishing Shack - Fish Toss -  Invis Stalker
            80026, //Debug - Garrison - Bunny - Boss Emotes
            2110, //Black Rat
            82333, //Mine Guard
            85730, //Lunarfall Rifleman
            89298, //Lunarfall Rifleman
            85737, //Lunarfall Rifleman
            89259, //Lunarfall Laborer
            82586, //Porter
            85736, //Lunarfall Smith
            85722, //Rat

            237721, //Dark Iron Mole Machine
            230365, //Lumber (Flavor)
            228585, //Potion
            237060, //Chair
            228587, //Table
            227916, //Table
            230377, //Tool Box
            228592, //Book Stack
            228593, //Book Stack
            232249, //Bone
            230772, //Junk
            230378, //Boot and Barrel
            230773, //Crates
            233537, //Grave
            233540, //Grave
            233541, //Grave
            233542, //Grave
            233543, //Grave
            233544, //Grave
            233545, //Grave
            233543, //Grave
            233544, //Grave
            233546, //Grave
            233547, //Grave
            233548, //Grave
            227915, //Forge
            232535, //Forge
            232530, //Forge
            227910, //Anvil
            237612, //Anvil
            237611, //Anvil
            237617, //Anvil
            237616, //Anvil
            237607, //Anvil
            239433, //Anvil
            227917, //Iron Bar
            228550, //Bench Clamp
            227693, //Book Stack
            233835, //Rose Bush
            240593, //Doodad_FirewoodPile05
            240594, //Doodad_FirewoodPile06
            237407, //Gate
            237408, //Gate
            230865, //List of Ingredients
            232529, //Thermal Anvil
            232534, //Thermal Anvil
            230121, //Bonfire
            228594, //Enchanting Supplies
            228583, //Alchemy Kit
            232297, //Campfire
            237316, //Garrison - Armory - Alliance - V2 - Small Cannon
            237317, //Garrison - Armory - Alliance - V2 - Small Cannon
            237318, //Garrison - Armory - Alliance - V2 - Small Cannon
            230994, //Campfire
            235978, //Cozy Fire
            235979, //Cozy Fire
            227881, //The Incinerator

            232380, //Monument Base
            233177, //Monument Base
            232379, //Monument Base
            239031, //Monument
            239032, //Monument
            239033, //Monument

            //Garrison
            230867, //Under Construction Cloud
            232283, //Medium Plot
            230286, //Medium Plot
            232282, //Small Plot
            232270, //Small Plot
            237223, //Fishing Shack
            230993, //Fishing Shack
            232286, //Garden House
            235373, //Pet Menagerie
            230985, //Garrison Building Alliance Fishing V1
            230987, //Garrison Building Alliance Fishing V2
            230989, //Garrison Building Alliance Fishing V3
            224813, //Garrison Building Alliance Pet Stable V1
            224814, //Garrison Building Alliance Pet Stable V2
            230475, //Garrison Building Alliance Salvage Tent V2
            230480, //Garrison Building Alliance Sparring Arena V1
            230486, //Garrison Building Alliance Sparring Arena V2
            230487, //Garrison Building Alliance Sparring Arena V3
            224854, //Garrison Building Alliance Storehouse V1
            234678, //Garrison Building Alliance Storehouse V2
            234679, //Garrison Building Alliance Storehouse V3
            227179, //Garrison Building Alchemy Level 1
            227590, //Garrison Building Alchemy Level 2
            227591, //Garrison Building Alchemy Level 3
            224548, //Garrison Building Armory V1
            224549, //Garrison Building Armory V2
            224550, //Garrison Building Armory V3
            224795, //Garrison Building Barn V1
            224796, //Garrison Building Barn V2
            233186, //Garrison Building Barn V3
            224797, //Garrison Building Barracks V1
            224798, //Garrison Building Barracks V2
            224799, //Garrison Building Barracks V3
            225537, //Garrison Building Blacksmith Level 1
            227588, //Garrison Building Blacksmith Level 2
            227589, //Garrison Building Blacksmith Level 3
            227073, //Garrison Building Enchanting Level 1
            227596, //Garrison Building Enchanting Level 2
            227597, //Garrison Building Enchanting Level 3
            227072, //Garrison Building Engineering Level 1
            227594, //Garrison Building Engineering Level 2
            227595, //Garrison Building Engineering Level 3
            224800, //Garrison Building Farm V1
            224801, //Garrison Building Farm V2
            235990, //Garrison Building Farm V2
            235991, //Garrison Building Farm V3
            225541, //Garrison Building Farmhouse
            224802, //Garrison Building Infirmary V1
            224803, //Garrison Building Infirmary V2
            224804, //Garrison Building Infirmary V3
            224805, //Garrison Building Inn V1
            224806, //Garrison Building Inn V2
            224807, //Garrison Building Inn V3
            227074, //Garrison Building Inscription Level 1
            227600, //Garrison Building Inscription Level 2
            227601, //Garrison Building Inscription Level 3
            227075, //Garrison Building Jewelcrafting Level 1
            227602, //Garrison Building Jewelcrafting Level 2
            227603, //Garrison Building Jewelcrafting Level 3
            227070, //Garrison Building Leatherworking Level 1
            227592, //Garrison Building Leatherworking Level 2
            227593, //Garrison Building Leatherworking Level 3
            224808, //Garrison Building Mage Tower V1
            224809, //Garrison Building Mage Tower V2
            224810, //Garrison Building Mage Tower V3
            224811, //Garrison Building Mill V1
            224812, //Garrison Building Mill V2
            233267, //Garrison Building Mill V3
            225538, //Garrison Building Mine 1
            225539, //Garrison Building Mine 2
            225540, //Garrison Building Mine 3
            235992, //Garrison Building Pet Stable V2
            235993, //Garrison Building Pet Stable V3
            224853, //Garrison Building Salvage Tent
            230476, //Garrison Building Salvage Yard V3
            225577, //Garrison Building Stable 1
            225578, //Garrison Building Stable 2
            225579, //Garrison Building Stable 3
            227180, //Garrison Building Tailoring Level 1
            227598, //Garrison Building Tailoring Level 2
            227599, //Garrison Building Tailoring Level 3
            227673, //Garrison Building Trading Post V1
            227674, //Garrison Building Trading Post V2
            233189, //Garrison Building Trading Post V3
            233957, //Garrison Building Under Construction V1
            233958, //Garrison Building Under Construction V1
            232373, //Garrison Building Under Construction V2
            232409, //Garrison Building Under Construction V2
            232410, //Garrison Building Under Construction V3
            232411, //Garrison Building Under Construction V3
            230492, //Garrison Building Workshop V1
            230493, //Garrison Building Workshop V2
            230494, //Garrison Building Workshop V3
        };
        
        #endregion

        #region Horde Blacklist Collection

        private static readonly HashSet<uint> _blacklistEntryIds_Horde = new HashSet<uint>
        {
            //NPCs
            79837, //Miner
            79781, //Frostwall Grunt
            81368, //Frostwall Grunt
            80299, //Frostwall Axe Thrower
            86783, //Frostwall Peon
            80288, //Frostwall Peon
            79925, //Frostwall Peon
            78467, //Frostwall Peon
            84806, //Senior Peon II
            80437, //Frostwall Rat
            80438, //Tundra Hare
            80436, //Cave Crab
            44880, //Sea Gull
            62953, //Sea Gull
            2110, //Black Rat
            80440, //Ridge Condor
            80444, //Goblin Excavation Blast Stalker
            79603, //Emote Bunny
            80442, //Excavation Worker
            87630, //Frostwall Smith
            78320, //Frostwolf Rylak
            79605, //Frostwall Peon
            80026, //Debug - Garrison - Bunny - Boss Emotes
            81163, //Garrison - Horde - Fishing Shack - Fish Toss -  Invis Stalker
            80569, //Barracks Grunt
            80571, //Barracks Recruit
            80572, //Frostwall Wolf




            //Objects
            237548, //Brazier
            237549, //Brazier
            237550, //Brazier
            237551, //Brazier
            237552, //Brazier
            237553, //Brazier
            237554, //Brazier
            237137, //Table
            233169, //Table
            233170, //Table
            238791, //Cage
            238790, //Cage
            231694, //Campfire
            231708, //Campfire
            232454, //Campfire
            232440, //Campfire
            232443, //Campfire
            232439, //Campfire
            237129, //Coal Bin
            239082, //Spices
            231706, //Menagerie
            233633, //Unused Wood Pile
            233827, //Monument Base
            233828, //Monument Base
            233829, //Monument Base
            237803, //Prison Door
            237804, //Prison Door
            237805, //Prison Door
            237806, //Prison Door
            237439, //Anvil
            237440, //Anvil
            237128, //Anvil
            225952, //Anvil
            241116, //Anvil
            237125, //Anvil
            231696, //Bonfire
            232444, //Bonfire
            237415, //Doodad_Orc_Cannon_004
            237416, //Doodad_Orc_Cannon_003
            237417, //Doodad_Orc_Cannon_002
            237418, //Doodad_Orc_Cannon_001
            237130, //Forge
            233442, //Forge
            237126, //Forge
            237124, //Coal Bin
            233441, //Thermal Anvil
            239036, //Horde Garrison Monument 01 Plaque
            239035, //Horde Garrison Monument 02 Plaque
            239037, //Horde Garrison Monument 03 Plaque

            

            //Garrison
            230443, //Garrison Building Horde Alchemy V1
            230444, //Garrison Building Horde Alchemy V2
            230445, //Garrison Building Horde Alchemy V3
            230406, //Garrison Building Horde Armory V1
            230407, //Garrison Building Horde Armory V2
            230409, //Garrison Building Horde Armory V3
            230410, //Garrison Building Horde Barn V1
            230411, //Garrison Building Horde Barn V2
            233188, //Garrison Building Horde Barn V3
            230412, //Garrison Building Horde Barracks V1
            230413, //Garrison Building Horde Barracks V2
            230414, //Garrison Building Horde Barracks V3
            230448, //Garrison Building Horde Blacksmith V1
            230449, //Garrison Building Horde Blacksmith V2
            230450, //Garrison Building Horde Blacksmith V3
            230451, //Garrison Building Horde Enchanting V1
            230452, //Garrison Building Horde Enchanting V2
            230453, //Garrison Building Horde Enchanting V3
            230454, //Garrison Building Horde Engineering V1
            230455, //Garrison Building Horde Engineering V2
            230456, //Garrison Building Horde Engineering V3
            230415, //Garrison Building Horde Farm V1
            236448, //Garrison Building Horde Farm V2
            236449, //Garrison Building Horde Farm V3
            230984, //Garrison Building Horde Fishing V1
            230986, //Garrison Building Horde Fishing V2
            230990, //Garrison Building Horde Fishing V3
            230416, //Garrison Building Horde Inn V1
            230417, //Garrison Building Horde Inn V2
            230418, //Garrison Building Horde Inn V3
            230427, //Garrison Building Horde Inscription V1
            230430, //Garrison Building Horde Inscription V2
            230432, //Garrison Building Horde Inscription V3
            230460, //Garrison Building Horde Jewelcrafting V1
            230461, //Garrison Building Horde Jewelcrafting V2
            230462, //Garrison Building Horde Jewelcrafting V3
            230457, //Garrison Building Horde Leatherworking V1
            230458, //Garrison Building Horde Leatherworking V2
            230459, //Garrison Building Horde Leatherworking V3
            230419, //Garrison Building Horde Mage Tower V1
            230420, //Garrison Building Horde Mage Tower V2
            230421, //Garrison Building Horde Mage Tower V3
            230422, //Garrison Building Horde Mill V1
            230423, //Garrison Building Horde Mill V2
            233266, //Garrison Building Horde Mill V3
            230466, //Garrison Building Horde Mine V1
            230467, //Garrison Building Horde Mine V2
            230468, //Garrison Building Horde Mine V3
            230426, //Garrison Building Horde Pet Stable V1
            236180, //Garrison Building Horde Pet Stable V2
            236181, //Garrison Building Horde Pet Stable V3
            230440, //Garrison Building Horde Salvage Yard V1
            230441, //Garrison Building Horde Salvage Yard V2
            230442, //Garrison Building Horde Salvage Yard V3
            230477, //Garrison Building Horde Sparring Arena V1
            230478, //Garrison Building Horde Sparring Arena V2
            230479, //Garrison Building Horde Sparring Arena V3
            230469, //Garrison Building Horde Stable V1
            230470, //Garrison Building Horde Stable V2
            230471, //Garrison Building Horde Stable V3
            230437, //Garrison Building Horde Storehouse V1
            230438, //Garrison Building Horde Storehouse V2
            230439, //Garrison Building Horde Storehouse V3
            230463, //Garrison Building Horde Tailoring V1
            230464, //Garrison Building Horde Tailoring V2
            230465, //Garrison Building Horde Tailoring V3
            230472, //Garrison Building Horde Trading Post V1
            230473, //Garrison Building Horde Trading Post V2
            230474, //Garrison Building Horde Trading Post V3
            230489, //Garrison Building Horde Workshop V1
            230490, //Garrison Building Horde Workshop V2
            230491, //Garrison Building Horde Workshop V3
        };
        
        #endregion

        private static readonly HashSet<uint> _blacklistEntryIds = new HashSet<uint>
        {

            //Garrison
            230315, //FW - Garrison - Large building 01
            231027, //FW - Garrison - Large Building 01
            232487, //FW - Garrison - Large Building 01
            230306, //FW - Garrison - Large building 02
            232464, //FW - Garrison - Large Building 02
            230305, //FW - Garrison - Large building 03
            232473, //FW - Garrison - Large Building 03
            230313, //FW - Garrison - Medium building 01
            231026, //FW - Garrison - Medium Building 01
            232480, //FW - Garrison - Medium Building 01
            230316, //FW - Garrison - Medium building 02
            232474, //FW - Garrison - Medium Building 02
            230304, //FW - Garrison - Medium building 03
            232461, //FW - Garrison - Medium Building 03
            230309, //FW - Garrison - Small building 01
            231023, //FW - Garrison - Small Building 01
            232471, //FW - Garrison - Small Building 01
            230308, //FW - Garrison - Small building 02
            231031, //FW - Garrison - Small Building 02
            232472, //FW - Garrison - Small Building 02
            230307, //FW - Garrison - Small building 03
            232468, //FW - Garrison - Small Building 03
            230310, //FW - Garrison - Small building 04
            232465, //FW - Garrison - Small Building 04


            //Followers
            85776, //Ahm
            85081, //Admiral Taylor
            85759, //Soulbinder Tuulani
            88166, //Hulda Shadowblade
            83947, //Kimzee Pinchwhistle
            86514, //Rangari Erdanii
            80733, //Magister Serena
            82495, //Rulkan
            79607, //Dagg
            89075, //Delvar Ironfist
            88009, //Millhouse Manastorm

        };
    }
}
