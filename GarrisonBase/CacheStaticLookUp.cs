using System;
using System.Collections.Generic;
using System.Linq;
using Herbfunk.GarrisonBase.Garrison;
using Herbfunk.GarrisonBase.Quest;
using Styx;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace Herbfunk.GarrisonBase
{
    public enum CraftingReagents
    {
        None = 0,

        TrueIronOre = 109119,
        BlackrockOre = 109118,


        TaladorOrchid = 109129,
        Starflower = 109127,
        NagrandArrowbloom = 109128,
        Frostweed = 109124,
        GorgrondFlytrap = 109126,
        Fireweed = 109125,


        RawBeastHide = 110609,
        SumptuousFur = 111557,
        
        
        DraenicDust = 109693,
        LuminousShard = 111245,
        TemporalCrystal=113588,
        FracturedTemporalCrystal=115504,

        CeruleanPigment = 114931,

        SorcerousAir = 113264,
        SorcerousFire = 113261,
        SorcerousWater = 113262,
        SorcerousEarth = 113263,

        SavageBlood = 118472,

        CrescentSaberfishFlesh=109137,
        

        //bop
        DraenicStone = 115508,
        DraenicSeeds = 116053,
        Timber = 114781,

        FurryCagedBeast = 119813,
        LeatheryCagedBeast=119814,
        MeatyCagedBeast=119810,
        CagedMightyClefthoof=119819,
        CagedMightyRiverbeast=119817,
        CagedMightyWolf=119815,
    }
    public static class CacheStaticLookUp
    {


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

        internal static readonly int MinersCoffeeItemId = 118897;
        internal static readonly int MinersCoffeeSpellId = 176049;
        internal static readonly int PreservedMiningPickItemId = 118903;
        internal static readonly int PreservedMiningPickSpellId = 176061;

        internal static List<WoWGuid> Blacklist_GUIDs = new List<WoWGuid>();

        internal static bool InitalizedCache = false;
        internal static void Update()
        {
            Character.Player.Initalize();
            MovementCache.Initalize(Character.Player.IsAlliance);
            QuestManager.RefreshQuestLog();
            Cache.Blacklist.Initalize(Character.Player.IsAlliance);
            InitalizedCache = true;
        }


        public static WoWObject GetWoWObject(uint entryId)
        {
            ObjectManager.Update();
            var ret = ObjectManager.ObjectList.FirstOrDefault(obj => obj.Entry == entryId && !Blacklist_GUIDs.Contains(obj.Guid));
            return ret;
        }
        public static WoWObject GetWoWObject(int entryId)
        {
            ObjectManager.Update();
            WoWObject ret = ObjectManager.ObjectList.FirstOrDefault(obj => obj.Entry == entryId && !Blacklist_GUIDs.Contains(obj.Guid));
            return ret;
        }
        public static WoWObject GetWoWObject(string name)
        {
            ObjectManager.Update();
            WoWObject ret = ObjectManager.ObjectList.FirstOrDefault(obj => obj.Name == name && !Blacklist_GUIDs.Contains(obj.Guid));
            return ret;
        }
        public static List<WoWUnit> GetWoWUnits(params uint[] args)
        {
            var ids = new List<uint>(args);
            ObjectManager.Update();
            return ObjectManager.GetObjectsOfType<WoWUnit>().Where(obj => ids.Contains(obj.Entry) && !Blacklist_GUIDs.Contains(obj.Guid) && obj.IsValid && !obj.IsDead).ToList();
        }

        internal static readonly List<uint> RepairVendorIds = new List<uint>
        {
            //Horde
            76872,
            //Alliance
            81346,
        };
        internal static readonly List<uint> PrimalTraderIds = new List<uint>
        {
            84967,//Horde
            84246,//Alliance
        };
        internal static readonly List<uint> FollowerEntryIds = new List<uint>
        {
           83820, //High Centurion Tormmok
        };
        internal static readonly List<uint> MineDeposits = new List<uint>
        {
            232544, //True Iron
            232542, //Blackrock
            232541, //Mine Cart
            232543, //Rich Blackrock
            232545, //Rich True iron
        };
        internal static readonly List<uint> HerbDeposits = new List<uint>
        {
            235390, //Nagrand Arrowbloom
            235388, //Gorgrond Flytrap
            235391, //Talador Orchid
            235387, //Fireweed
            235389, //Starflower
            235376, //Frostweed

        };
        internal static readonly List<uint> GarrisonsZonesId = new List<uint>
        {
            7078, // Lunarfall - Ally
            7004, // Frostwall - Horde
        };
        internal static readonly List<uint> GarrisonsMailboxIds = new List<uint>
        {
            230127, //Ally level 3
            232284, //Ally level 2

            231710, //Horde level 3
            233515, //Horde level 2
        };
        internal static readonly List<uint> CommandTableIds = new List<uint>
        {
            85805, 
            86031, 
            84224,
            84698,
        };
        internal static readonly List<uint> ResourceCacheIds = new List<uint>
        {
            237191, 236916, 237720, 237722, 237724, 237723,
        };
        
        internal static readonly List<uint> FinalizeGarrisonPlotIds = new List<uint>
        {
            231217,
            231964,
            233248,
            233249,
            233250,
            233251,
            232651,
            232652,
            236261,
            236262,
            236263,
            236175,
            236176,
            236177,
            236185,
            236186,
            236187,
            236188,
            236190,
            236191,
            236192,
            236193,
        };

        public static readonly List<WoWGameObjectType> BlacklistedGameObjectTypes = new List<WoWGameObjectType>
        {
            WoWGameObjectType.AreaDamage,
            WoWGameObjectType.AuraGenerator,
            WoWGameObjectType.Chair,
        };

        public static readonly Dictionary<int, Tuple<InventoryType, int>> DictItemRewards_CharacterTokens = new Dictionary
            <int, Tuple<InventoryType, int>>
        {
            {114100, new Tuple<InventoryType, int>(InventoryType.Shoulder, 610)},
            {114105, new Tuple<InventoryType, int>(InventoryType.Trinket, 600)},
            {114097, new Tuple<InventoryType, int>(InventoryType.Hand, 590)},
            {114099, new Tuple<InventoryType, int>(InventoryType.Legs, 580)},
            {114094, new Tuple<InventoryType, int>(InventoryType.Wrist, 570)},
            {114108, new Tuple<InventoryType, int>(InventoryType.Weapon, 560)},
            {114096, new Tuple<InventoryType, int>(InventoryType.Feet, 550)},
            {114098, new Tuple<InventoryType, int>(InventoryType.Head, 540)},
            {114101, new Tuple<InventoryType, int>(InventoryType.Waist, 530)},
            {114053, new Tuple<InventoryType, int>(InventoryType.Hand, 512)},
            {114052, new Tuple<InventoryType, int>(InventoryType.Finger, 519)},

            ////615
            {114063, new Tuple<InventoryType, int>(InventoryType.Shoulder, 615)},
            {114057, new Tuple<InventoryType, int>(InventoryType.Wrist, 615)},
            {114058, new Tuple<InventoryType, int>(InventoryType.Chest, 615)},
            {114068, new Tuple<InventoryType, int>(InventoryType.Trinket, 615)},
            {114066, new Tuple<InventoryType, int>(InventoryType.Neck, 615)},
            {114109, new Tuple<InventoryType, int>(InventoryType.Weapon, 615)},
            {114059, new Tuple<InventoryType, int>(InventoryType.Feet, 615)},

            ////630
            {114080, new Tuple<InventoryType, int>(InventoryType.Trinket, 630)},
            {114071, new Tuple<InventoryType, int>(InventoryType.Feet, 630)},
            {114075, new Tuple<InventoryType, int>(InventoryType.Shoulder, 630)},
            {114078, new Tuple<InventoryType, int>(InventoryType.Neck, 630)},
            {114110, new Tuple<InventoryType, int>(InventoryType.Weapon, 630)},
            {114070, new Tuple<InventoryType, int>(InventoryType.Chest, 630)},
            {114069, new Tuple<InventoryType, int>(InventoryType.Wrist, 630)},

            ////645
            {114085, new Tuple<InventoryType, int>(InventoryType.Shoulder, 645)},
            {114083, new Tuple<InventoryType, int>(InventoryType.Chest, 645)},
            {114084, new Tuple<InventoryType, int>(InventoryType.Feet, 645)},
            {114082, new Tuple<InventoryType, int>(InventoryType.Wrist, 645)},
            {114112, new Tuple<InventoryType, int>(InventoryType.Weapon, 645)},
            {114087, new Tuple<InventoryType, int>(InventoryType.Trinket, 645)},
            {114086, new Tuple<InventoryType, int>(InventoryType.Neck, 645)},

            ////Highmaul Caches
            {118531, new Tuple<InventoryType, int>(InventoryType.None, 685)},
            {118530, new Tuple<InventoryType, int>(InventoryType.None, 670)},
            {118529, new Tuple<InventoryType, int>(InventoryType.None, 665)},
        };
    
        public static readonly List<int> ItemRewards_CharacterTokens = new List<int>
        {
            114100, //Shoulders 610
            114105, //Trinket 600
            114097, //Gloves 590
            114099, //Pants 580
            114094, //Bracers 570
            114108, //Weapon 560
            114096, //Boots 550
            114098, //Helm 540
            114101, //Belt 530
            114053, //Gloves 512
            114052, //Ring 519

            //615
            114063, //Shoulders
            114057, //Bracers
            114058, //Chest
            114068, //Trinket
            114066, //Neck
            114109, //Weapon
            114059, //Boots

            //630
            114080, //Trinket
            114071, //Boots
            114075, //Shoulders
            114078, //Neck
            114110, //Weapon
            114070, //Chest
            114069, //Bracers

            //645
            114085, //Shoulders
            114083, //Chest
            114084, //Boots
            114082, //Bracers
            114112, //Weapon
            114087, //Trinket
            114086, //Neck

            //Highmaul Caches
            118531, //Mythic (685)
            118529, //Normal (665)
            118530, //Heroic (670)
        };

        public static int ItemRewards_FollowerToken_ArmorEnhancement = 120301;
        public static int ItemRewards_FollowerToken_WeaponEnhancement = 120302;
        public static int ItemRewards_FollowerToken_Armor615 = 114807;
        public static int ItemRewards_FollowerToken_Weapon615 = 114616;
        public static int ItemRewards_FollowerToken_Armor630 = 114806;
        public static int ItemRewards_FollowerToken_Weapon630 = 114081;
        public static int ItemRewards_FollowerToken_Armor645 = 114746;
        public static int ItemRewards_FollowerToken_Weapon645 = 114622;

        public static readonly List<int> ItemRewards_FollowerTokens = new List<int>
        {
            ItemRewards_FollowerToken_ArmorEnhancement, //Armor Enhancement Token
            ItemRewards_FollowerToken_WeaponEnhancement, //Weapon Enhancement Token

            ItemRewards_FollowerToken_Weapon615, //war-ravaged-weaponry
            ItemRewards_FollowerToken_Armor615, //war-ravaged-armor-set

            ItemRewards_FollowerToken_Armor630, //blackrock-armor-set 
            ItemRewards_FollowerToken_Weapon630, //blackrock-weaponry

            ItemRewards_FollowerToken_Armor645, //goredrenched-armor-set
            ItemRewards_FollowerToken_Weapon630, //goredrenched-weaponry

            114822, //heavily-reinforced-armor-enhancement (+9)
            114131, //power-overrun-weapon-enhancement (+9)
        };

        public static readonly List<int> ItemRewards_Contracts = new List<int>
        {
            114825, //Contract: Ulna Thresher
            112848, //Contract: Daleera Moonfang
            114826, //Contract: Bruma Swiftstone
            112737, //Contract: Ka'la of the Frostwolves
        };

        //118354 = Follower Re-training Certificate
        //122272 = Follower Ability Retraining
        //122273 = Follower Trait Retraining
        //123858 = Follower Retraining Scroll Case
        public static readonly int ItemReward_FollowerRetrainingCertificate = 118354;
        public static readonly int ItemReward_FollowerAbilityRetrainingManual = 122272;
        public static readonly int ItemReward_FollowerTraitRetrainingGuide = 122273;
        public static readonly int ItemReward_FollowerRetrainingScrollCase = 123858;
        //
        public static readonly List<int> ItemRewards_FollowerRetraining = new List<int>
        {
            ItemReward_FollowerRetrainingCertificate,
            ItemReward_FollowerAbilityRetrainingManual,
            ItemReward_FollowerTraitRetrainingGuide,
            ItemReward_FollowerRetrainingScrollCase
        };

        public static readonly int ItemReward_TraitDancing = 118474;
        public static readonly int ItemReward_TraitHearthstone = 118475;
        public static readonly int ItemReward_TraitSuntouchedFeather = 122275;
        public static readonly int ItemReward_TraitOgreBuddyHandbook = 122580;
        public static readonly int ItemReward_TraitGreaseMonkeyGuide = 122583;
        public static readonly int ItemReward_TraitWinningwithWildlings = 122584;
        public static readonly int ItemReward_TraitGuidetoArakkoaRelations = 122582;

        public static readonly List<int> ItemRewards_FollowerTraits = new List<int>
        {
            ItemReward_TraitDancing,
            ItemReward_TraitHearthstone,
            ItemReward_TraitSuntouchedFeather,
            ItemReward_TraitOgreBuddyHandbook,
            ItemReward_TraitGreaseMonkeyGuide,
            ItemReward_TraitWinningwithWildlings,
            ItemReward_TraitGuidetoArakkoaRelations
        };

        public static readonly int ItemReward_ElementalRune = 115510;
        public static readonly int ItemReward_AbrogatorStone = 115280;

        //115510 = Elemental Rune
        //115280 = Abrogator Stone
        //120945 = Primal Spirit
        //118472 = Salvage Blood


        //118474 = Supreme Manual of Dance
        //118475 = Hearthstone Strategy Guide
        //122275 = Sun-touched Feather of Rukhmar
        public enum RushOrders
        {
            None,
            EnchantersStudy=122590,
            AlchemyLab=122576,
            Forge=122595,
            Tailoring=122594,
            Gem=122592,
            Tannery=122596,
            Scribe=122593,
            Engineering=122591,
            HerbGarden = 122496,
            Mines = 122502,
            Barn = 122307,
            WarMill = 122491,
            GladiatorsSanctum = 122487,
            WorkShop = 122501,
        }
        public static readonly List<int> ItemRewards_RushOrders = new List<int>
        {
            (int)RushOrders.AlchemyLab,
            (int)RushOrders.EnchantersStudy,
            (int)RushOrders.Forge,
            (int)RushOrders.Tailoring,
            (int)RushOrders.Gem,
            (int)RushOrders.Tannery,
            (int)RushOrders.Scribe,
            (int)RushOrders.Engineering,
            //122590, //Enchanter's Study
            //122576, //Alchemy Lab
            //122595, //Forge
            //122594, //Tailoring
            //122592, //Gem
            //122596, //Tannery
            //122593, //Scribe
            //122591, //Engineering
        };
    }
}
