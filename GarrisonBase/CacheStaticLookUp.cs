using System;
using System.Collections.Generic;
using System.Linq;
using Herbfunk.GarrisonBase.Helpers;
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
        TemporalCrystal = 113588,
        FracturedTemporalCrystal = 115504,

        CeruleanPigment = 114931,

        SorcerousAir = 113264,
        SorcerousFire = 113261,
        SorcerousWater = 113262,
        SorcerousEarth = 113263,

        SavageBlood = 118472,

        CrescentSaberfishFlesh = 109137,


        //bop
        DraenicStone = 115508,
        DraenicSeeds = 116053,
        Timber = 114781,

        FurryCagedBeast = 119813,
        LeatheryCagedBeast = 119814,
        MeatyCagedBeast = 119810,
        CagedMightyClefthoof = 119819,
        CagedMightyRiverbeast = 119817,
        CagedMightyWolf = 119815,

        AlchemicalCatalyst = 108996,
        TruesteelIngot = 108257,
        GearspringParts = 111366,
        WarPaints = 112377,
        TaladiteCrystal = 115524,
        BurnishedLeather = 110611,
        HexweaveCloth = 111556,
    }
    public enum RushOrders
    {
        None,
        EnchantersStudy = 122590,
        AlchemyLab = 122576,
        Forge = 122595,
        Tailoring = 122594,
        Gem = 122592,
        Tannery = 122596,
        Scribe = 122593,
        Engineering = 122591,
        HerbGarden = 122496,
        Mines = 122502,
        Barn = 122307,
        WarMill = 122491,
        GladiatorsSanctum = 122487,
        WorkShop = 122501,
    }

    public static class CacheStaticLookUp
    {

        internal static bool InitalizedCache = false;
        internal static void Update()
        {
            Character.Player.Initalize();
            MovementCache.Initalize(Character.Player.IsAlliance);
            QuestHelper.RefreshQuestLog();
            Cache.Blacklist.Initalize(Character.Player.IsAlliance);

            TrappingEntryIds.Clear();
            TrappingEntryIds.AddRange(Trap_UnitIds_Boars);
            TrappingEntryIds.AddRange(Trap_UnitIds_Clefthoof);
            TrappingEntryIds.AddRange(Trap_UnitIds_Elekk);
            TrappingEntryIds.AddRange(Trap_UnitIds_Riverbeasts);
            TrappingEntryIds.AddRange(Trap_UnitIds_Talbulk);
            TrappingEntryIds.AddRange(Trap_UnitIds_Wolves);

            InitalizedCache = true;
        }




        internal static readonly int MinersCoffeeItemId = 118897;
        internal static readonly int MinersCoffeeSpellId = 176049;
        internal static readonly int PreservedMiningPickItemId = 118903;
        internal static readonly int PreservedMiningPickSpellId = 176061;




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
        internal static readonly List<uint> TimberEntryIds=new List<uint>
        {
            233635, // Large Timber
            233634, // Timber
            233604, // Small Timber
        };



        public static readonly List<WoWGameObjectType> BlacklistedGameObjectTypes = new List<WoWGameObjectType>
        {
            WoWGameObjectType.AreaDamage,
            WoWGameObjectType.AuraGenerator,
            WoWGameObjectType.Chair,
        };

       
        


        #region Trapping

        #region Riverbeasts
        public static readonly uint[] Trap_UnitIds_Riverbeasts = 
        {
            //85907,
            //83841,
            //77715,
            //85762,
            //85743,
            //77431,
            //83388,
            //82905,
            //88669,
            //79587,
            //88586,
            //87666,
            //86780,
            //85906,
            //79662,
            //79577,
            //72606,
            //88075,
            //82310,
            //83455,
            //75468,
            //87020,
            //87021,
            //86848,
        };
        
        #endregion

        #region Boars

        public static readonly uint[] Trap_UnitIds_Boars = 
        {
            88508,
            86850,
            //82728,
            //75241,
            //77994,
            //75416,
            //79756,
            //76914,
            //77124,
            //77129,
            //84893,
            //86153,
            //80174,
            //88589,
            //83897,
            //86150,
            //86151,
            //88508,
            //77478,
            //82617,
            //82726,
            //72934,
            //77298,
            //75037,
            //83719,
            //86850,
        };
        
        #endregion

        #region Wolves

        public static readonly uint[] Trap_UnitIds_Wolves = 
        {
            74748,
            76707,
            76705,
            86932,
            86931,
            81902,
            82308,
            //84045,
            //80261,
            //86932,
            //86931,
            //74748,
            //77886,
            //73619,
            //76150,
            //76337,
            //84793,
            //76593,
            //10981,
            //76597,
            //76707,
            //76705,
            //81000,
            //74712,
            //74169,
            //82912,
            //81774,
            //74206,
            //74208,
            //82209,
            //82205,
            //87107,
            //81902,
            //86414,
            //81001,
            //72991,
            //76181,
            //80160,
            //79755,
            //84044,
            //82307,
            //82308,
            //80263,
            //58456,
            //82535,
            //81718,
            //86851,
        };

        #endregion

        #region Talbulk

        public static readonly uint[] Trap_UnitIds_Talbulk = 
        {
            86727,
            78277,
            78278,
            //66605,
            //78277,
            //83843,
            //78279,
            //83470,
            //78278,
            //86729,
            //58454,
            //82778,
            //76442,
            //82031,
            //62763,
            //78274,
            //78276,
            //78275,
            //82116,
            //86728,
            //86727,
            //82513,
            //86801,
        };

        #endregion

        #region clefthoof

        public static readonly uint[] Trap_UnitIds_Clefthoof = 
        {
            72162,
            86730,
            86731,
            //78574,
            //76711,
            //76710,
            //73234,
            //82119,
            //86732,
            //72881,
            //76326,
            //77513,
            //83483,
            //75680,
            //86000,
            //77519,
            //85537,
            //76576,
            //78920,
            //78918,
            //84798,
            //80420,
            //78919,
            //78364,
            //86731,
            //86730,
            //81898,
            //78576,
            //78575,
            //85031,
            //80136,
            //50990,
            //86520,
            //79034,
            //76389,
            //75771,
            //78528,
            //78572,
            //78570,
            //78571,
            //72162,
            //86847,

        };

        #endregion

        #region Elekk

        public static readonly uint[] Trap_UnitIds_Elekk = 
        {
            87700,
            87698,
            80175,
            86741,
        };

        #endregion

        public static List<uint> TrappingEntryIds = new List<uint>();

        #endregion
    }
}
