using System.Collections.Generic;
using Styx;

namespace Herbfunk.GarrisonBase
{
    public static partial class MovementCache
    {
        public static void Initalize(bool alliance)
        {
            if (!alliance)
            {
                Garrison = Horde_Garrison;
                SmallPlots = Horde_Center;
                SoloLargePlot = Horde_LargeCenter;
                SoloMediumPlot = Horde_CenterMedium;
                LargeMediumPlots = Horde_North;
                Garden = Horde_Garden;
                Mine = Horde_Mine;
                FishShack = Horde_FishShack;
                MovementPolygons = new List<Polygon>
                {
                    Garrison,
                    SmallPlots,
                    SoloLargePlot,
                    SoloMediumPlot,
                    LargeMediumPlots,
                    Garden,
                    Mine,
                    FishShack,
                };

                SmallPlot18SafePoint = HordeSmallPlot18SafePoint;
                SmallPlot19SafePoint = HordeSmallPlot19SafePoint;
                SmallPlot20SafePoint = HordeSmallPlot20SafePoint;
                MediumPlot22SafePoint = HordeMediumPlot22SafePoint;
                MediumPlot25SafePoint = HordeMediumPlot25SafePoint;
                LargePlot23SafePoint = HordeLargePlot23SafePoint;
                LargePlot24SafePoint = HordeLargePlot24SafePoint;
                GardenPlot63SafePoint = HordeHerbSafePoint;
                MinePlot59SafePoint = HordeMineSafePoint;
                SellRepairNpcLocation = HordeSellRepairNpc;
                GarrisonEntrance = HordeGarrisonEntrance;

                SmallPlot18Entrance = HordeSmallPlot18Entrance;
                SmallPlot19Entrance = HordeSmallPlot19Entrance;
                SmallPlot20Entrance = HordeSmallPlot20Entrance;
                MediumPlot22Entrance = HordeMediumPlot22Entrance;
                MediumPlot25Entrance = HordeMediumPlot25Entrance;
                LargePlot23Entrance = HordeLargePlot23Entrance;
                LargePlot24Entrance = HordeLargePlot24Entrance;

            }
            else
            {
                SmallPlot18SafePoint = AllianceSmallPlot18SafePoint;
                SmallPlot19SafePoint = AllianceSmallPlot19SafePoint;
                SmallPlot20SafePoint = AllianceSmallPlot20SafePoint;
                MediumPlot22SafePoint = AllianceMediumPlot22SafePoint;
                MediumPlot25SafePoint = AllianceMediumPlot25SafePoint;
                LargePlot23SafePoint = AllianceLargePlot23SafePoint;
                LargePlot24SafePoint = AllianceLargePlot24SafePoint;
                GardenPlot63SafePoint = AllianceHerbSafePoint;
                MinePlot59SafePoint = AllianceMineSafePoint;
                SellRepairNpcLocation = AllianceSellRepairNpc;
                GarrisonEntrance = AllianceGarrisonEntrance;

                SmallPlot18Entrance = AllianceSmallPlot18Entrance;
                SmallPlot19Entrance = AllianceSmallPlot19Entrance;
                SmallPlot20Entrance = AllianceSmallPlot20Entrance;
                MediumPlot22Entrance = AllianceMediumPlot22Entrance;
                MediumPlot25Entrance = AllianceMediumPlot25Entrance;
                LargePlot23Entrance = AllianceLargePlot23Entrance;
                LargePlot24Entrance = AllianceLargePlot24Entrance;
            }


        }

        internal static List<Polygon> MovementPolygons = new List<Polygon>();

        internal static WoWPoint GetBuildingSafeMovementPoint(int plotId)
        {
            if (plotId == 18)
                return SmallPlot18SafePoint;
            if (plotId == 19)
                return SmallPlot19SafePoint;
            if (plotId == 20)
                return SmallPlot20SafePoint;

            if (plotId == 22)
                return MediumPlot22SafePoint;
            if (plotId == 23)
                return LargePlot23SafePoint;
            if (plotId == 24)
                return LargePlot24SafePoint;
            if (plotId == 25)
                return MediumPlot25SafePoint;

            if (plotId == 59)
                return MinePlot59SafePoint;
            if (plotId == 63)
                return GardenPlot63SafePoint;


            return WoWPoint.Zero;
        }
        internal static WoWPoint GetBuildingEntranceMovementPoint(int plotId)
        {
            if (plotId == 18)
                return SmallPlot18Entrance;
            if (plotId == 19)
                return SmallPlot19Entrance;
            if (plotId == 20)
                return SmallPlot20Entrance;

            if (plotId == 22)
                return MediumPlot22Entrance;
            if (plotId == 23)
                return LargePlot23Entrance;
            if (plotId == 24)
                return LargePlot24Entrance;
            if (plotId == 25)
                return MediumPlot25Entrance;

            if (plotId == 59)
                return MinePlot59SafePoint;
            if (plotId == 63)
                return GardenPlot63SafePoint;


            return WoWPoint.Zero;
        }

        public static Polygon Garrison
        {
            get { return _garrison; }
            set { _garrison = value; }
        }
        private static Polygon _garrison;
        public static Polygon SmallPlots
        {
            get { return _smallPlots; }
            set { _smallPlots = value; }
        }
        private static Polygon _smallPlots;
        public static Polygon SoloLargePlot
        {
            get { return _soloLargePlot; }
            set { _soloLargePlot = value; }
        }
        private static Polygon _soloLargePlot;
        public static Polygon SoloMediumPlot
        {
            get { return _soloMediumPlot; }
            set { _soloMediumPlot = value; }
        }
        private static Polygon _soloMediumPlot;
        public static Polygon LargeMediumPlots
        {
            get { return _largeMediumPlots; }
            set { _largeMediumPlots = value; }
        }
        private static Polygon _largeMediumPlots;
        public static Polygon Garden
        {
            get { return _garden; }
            set { _garden = value; }
        }
        private static Polygon _garden;
        public static Polygon Mine
        {
            get { return _mine; }
            set { _mine = value; }
        }
        private static Polygon _mine;
        public static Polygon FishShack
        {
            get { return _fishShack; }
            set { _fishShack = value; }
        }
        private static Polygon _fishShack;

        //MediumPlot22

        public static WoWPoint SmallPlot18SafePoint
        {
            get { return _smallPlot18SafePoint; }
            set { _smallPlot18SafePoint = value; }
        }
        private static WoWPoint _smallPlot18SafePoint;

        public static WoWPoint SmallPlot19SafePoint
        {
            get { return _smallPlot19SafePoint; }
            set { _smallPlot19SafePoint = value; }
        }
        private static WoWPoint _smallPlot19SafePoint;

        public static WoWPoint SmallPlot20SafePoint
        {
            get { return _smallPlot20SafePoint; }
            set { _smallPlot20SafePoint = value; }
        }
        private static WoWPoint _smallPlot20SafePoint;

        public static WoWPoint MediumPlot22SafePoint
        {
            get { return _mediumPlot22SafePoint; }
            set { _mediumPlot22SafePoint = value; }
        }
        private static WoWPoint _mediumPlot22SafePoint;

        public static WoWPoint MediumPlot25SafePoint
        {
            get { return _mediumPlot25SafePoint; }
            set { _mediumPlot25SafePoint = value; }
        }
        private static WoWPoint _mediumPlot25SafePoint;

        public static WoWPoint LargePlot23SafePoint
        {
            get { return _largePlot23SafePoint; }
            set { _largePlot23SafePoint = value; }
        }
        private static WoWPoint _largePlot23SafePoint;

        public static WoWPoint LargePlot24SafePoint
        {
            get { return _largePlot24SafePoint; }
            set { _largePlot24SafePoint = value; }
        }
        private static WoWPoint _largePlot24SafePoint;

        public static WoWPoint MinePlot59SafePoint
        {
            get { return _minePlot59SafePoint; }
            set { _minePlot59SafePoint = value; }
        }
        private static WoWPoint _minePlot59SafePoint;

        public static WoWPoint GardenPlot63SafePoint
        {
            get { return _gardenPlot63SafePoint; }
            set { _gardenPlot63SafePoint = value; }
        }
        private static WoWPoint _gardenPlot63SafePoint;

        public static WoWPoint SellRepairNpcLocation
        {
            get { return _sellRepairNpcLocation; }
            set { _sellRepairNpcLocation = value; }
        }
        private static WoWPoint _sellRepairNpcLocation;

        public static WoWPoint GarrisonEntrance
        {
            get { return _garrisonEntrance; }
            set { _garrisonEntrance = value; }
        }
        private static WoWPoint _garrisonEntrance;


        public static WoWPoint SmallPlot18Entrance { get; set; }
        public static WoWPoint SmallPlot19Entrance { get; set; }
        public static WoWPoint SmallPlot20Entrance { get; set; }
        public static WoWPoint MediumPlot22Entrance { get; set; }
        public static WoWPoint MediumPlot25Entrance { get; set; }
        public static WoWPoint LargePlot23Entrance { get; set; }
        public static WoWPoint LargePlot24Entrance { get; set; }

    }
}
