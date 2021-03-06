﻿using System.Collections.Generic;
using Herbfunk.GarrisonBase.Character;
using Herbfunk.GarrisonBase.Garrison.Enums;
using Herbfunk.GarrisonBase.Garrison.Objects;
using Styx;

namespace Herbfunk.GarrisonBase
{
    public static partial class MovementCache
    {
        public static void Initalize(bool alliance)
        {
            if (!alliance)
            {
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
                FlightPathNpcLocation = HordeFlightPathNpc;
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
                FlightPathNpcLocation = AllianceFlightPathNpc;
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

        internal static List<WoWPoint> GetSpecialMovementPoints(Building building)
        {
            return GetSpecialMovementPoints(building.Type, building.PlotId, building.Level, Player.IsAlliance);
        }
        internal static List<WoWPoint> GetSpecialMovementPoints(BuildingType type, int plotId, int level, bool alliance)
        {
            switch (type)
            {
                case BuildingType.SalvageYard:
                    if (alliance)
                    {
                        if (plotId == 18) return new List<WoWPoint> {AllianceSalvageYardPlot18};
                        if (plotId == 19) return new List<WoWPoint> { AllianceSalvageYardPlot19 };
                        if (plotId == 20) return new List<WoWPoint> { AllianceSalvageYardPlot20 };
                    }
                    else
                    {
                        if (plotId == 18) return new List<WoWPoint> { HordeSalvageYardPlot18 };
                        if (plotId == 19) return new List<WoWPoint> { HordeSalvageYardPlot19 };
                        if (plotId == 20) return new List<WoWPoint> { HordeSalvageYardPlot20 };
                    }
                    break;
                case BuildingType.WarMillDwarvenBunker:
                    if (alliance)
                    {
                        if (plotId == 23)
                        {
                            if (level == 2) return Alliance_WarMill_Plot23_Level2;
                            if (level == 3) return Alliance_WarMill_Plot23_Level2;
                        }
                        if (plotId == 24)
                        {
                            if (level == 2) return Alliance_WarMill_Plot24_Level2;
                            if (level == 3) return Alliance_WarMill_Plot24_Level2;
                        }
                    }
                    else
                    {
                        if (plotId == 23)
                        {
                            if (level == 2) return Horde_Plot23_WarMill_Level2;
                            if (level == 3) return Horde_Plot23_WarMill_Level3;
                        }
                        if (plotId == 24)
                        {
                            if (level == 2) return Horde_Plot24_WarMill_Level2;
                            if (level == 3) return Horde_Plot24_WarMill_Level3;
                        }
                    }
                    break;
                case BuildingType.TradingPost:
                     if (alliance)
                    {
                        if (plotId == 22)
                        {
                            if (level == 1) return Alliance_Plot22_TradePost_Level1;
                            if (level == 2) return Alliance_Plot22_TradePost_Level2;
                            if (level == 3) return Alliance_Plot22_TradePost_Level3;
                        }
                        if (plotId == 25)
                        {
                            if (level == 1) return Alliance_Plot25_TradePost_Level1;
                            if (level == 2) return Alliance_Plot25_TradePost_Level2;
                            if (level == 3) return Alliance_Plot25_TradePost_Level3;
                        }
                    }
                    else
                    {
                        if (plotId == 22)
                        {
                            if (level == 1) return Horde_Plot22_TradePost_Level1;
                            if (level == 2) return Horde_Plot22_TradePost_Level2;
                            if (level == 3) return Horde_Plot22_TradePost_Level3;
                        }
                        if (plotId == 25)
                        {
                            if (level == 1) return Horde_Plot25_TradePost_Level1;
                            if (level == 2) return Horde_Plot25_TradePost_Level2;
                            if (level == 3) return Horde_Plot25_TradePost_Level3;
                        }
                    }
                    break;
                case BuildingType.ScribesQuarters:
                    if (alliance)
                    {
                        if (plotId == 18)
                        {
                            if (level == 1) return Alliance_Plot18_ScribesQuarter_Level1;
                            if (level == 2) return Alliance_Plot18_ScribesQuarter_Level2;
                            if (level == 3) return Alliance_Plot18_ScribesQuarter_Level3;
                        }
                        if (plotId == 19)
                        {
                            if (level == 1) return Alliance_Plot19_ScribesQuarter_Level1;
                            if (level == 2) return Alliance_Plot19_ScribesQuarter_Level2;
                            if (level == 3) return Alliance_Plot19_ScribesQuarter_Level3;
                        }
                        if (plotId == 20)
                        {
                            if (level == 1) return Alliance_Plot20_ScribesQuarter_Level1;
                            if (level == 2) return Alliance_Plot20_ScribesQuarter_Level2;
                            if (level == 3) return Alliance_Plot20_ScribesQuarter_Level3;
                        }
                    }
                    else
                    {
                        if (plotId == 18)
                        {
                            if (level == 1) return Horde_Plot18_ScribesQuarter_Level1;
                            if (level == 2) return Horde_Plot18_ScribesQuarter_Level2;
                            if (level == 3) return Horde_Plot18_ScribesQuarter_Level2;
                        }
                        if (plotId == 19)
                        {
                            if (level == 1) return Horde_Plot19_ScribesQuarter_Level1;
                            if (level == 2) return Horde_Plot19_ScribesQuarter_Level2;
                            if (level == 3) return Horde_Plot19_ScribesQuarter_Level2;
                        }
                        if (plotId == 20)
                        {
                            if (level == 1) return Horde_Plot20_ScribesQuarter_Level1;
                            if (level == 2) return Horde_Plot20_ScribesQuarter_Level2;
                            if (level == 3) return Horde_Plot20_ScribesQuarter_Level2;
                        }
                    }
                    break;
                case BuildingType.TailoringEmporium:
                    if (alliance)
                    {
                        if (plotId == 18)
                        {
                            if (level == 1) return Alliance_Plot18_TailoringEmporium_Level1;
                            if (level == 2) return Alliance_Plot18_TailoringEmporium_Level2;
                            if (level == 3) return Alliance_Plot18_TailoringEmporium_Level3;
                        }
                        if (plotId == 19)
                        {
                            if (level == 1) return Alliance_Plot19_TailoringEmporium_Level1;
                            if (level == 2) return Alliance_Plot19_TailoringEmporium_Level2;
                            if (level == 3) return Alliance_Plot19_TailoringEmporium_Level3;
                        }
                        if (plotId == 20)
                        {
                            if (level == 1) return Alliance_Plot20_TailoringEmporium_Level1;
                            if (level == 2) return Alliance_Plot20_TailoringEmporium_Level2;
                            if (level == 3) return Alliance_Plot20_TailoringEmporium_Level3;
                        }
                    }
                    else
                    {
                        if (plotId == 18)
                        {
                            if (level == 1) return Horde_Plot18_TailoringEmporium_Level1;
                            if (level == 2) return Horde_Plot18_TailoringEmporium_Level2;
                            if (level == 3) return Horde_Plot18_TailoringEmporium_Level3;
                        }
                        if (plotId == 19)
                        {
                            if (level == 1) return Horde_Plot19_TailoringEmporium_Level1;
                            if (level == 2) return Horde_Plot19_TailoringEmporium_Level2;
                            if (level == 3) return Horde_Plot19_TailoringEmporium_Level3;
                        }
                        if (plotId == 20)
                        {
                            if (level == 1) return Horde_Plot20_TailoringEmporium_Level1;
                            if (level == 2) return Horde_Plot20_TailoringEmporium_Level2;
                            if (level == 3) return Horde_Plot20_TailoringEmporium_Level3;
                        }
                    }
                    break;
                case BuildingType.TheTannery:
                    if (alliance)
                    {
                        if (plotId == 18)
                        {
                            if (level == 1) return Alliance_Plot18_Tannery_Level1;
                            if (level == 2) return Alliance_Plot18_Tannery_Level2;
                            if (level == 3) return Alliance_Plot18_Tannery_Level3;
                        }
                        if (plotId == 19)
                        {
                            if (level == 1) return Alliance_Plot19_Tannery_Level1;
                            if (level == 2) return Alliance_Plot19_Tannery_Level2;
                            if (level == 3) return Alliance_Plot19_Tannery_Level3;
                        }
                        if (plotId == 20)
                        {
                            if (level == 1) return Alliance_Plot20_Tannery_Level1;
                            if (level == 2) return Alliance_Plot20_Tannery_Level2;
                            if (level == 3) return Alliance_Plot20_Tannery_Level3;
                        }
                    }
                    else
                    {
                        if (plotId == 18)
                        {
                            if (level == 1) return Horde_Plot18_Tannery_Level1;
                            if (level == 2) return Horde_Plot18_Tannery_Level2;
                            if (level == 3) return Horde_Plot18_Tannery_Level3;
                        }
                        if (plotId == 19)
                        {
                            if (level == 1) return Horde_Plot19_Tannery_Level1;
                            if (level == 2) return Horde_Plot19_Tannery_Level2;
                            if (level == 3) return Horde_Plot19_Tannery_Level3;
                        }
                        if (plotId == 20)
                        {
                            if (level == 1) return Horde_Plot20_Tannery_Level1;
                            if (level == 2) return Horde_Plot20_Tannery_Level2;
                            if (level == 3) return Horde_Plot20_Tannery_Level3;
                        }
                    }
                    break;
                case BuildingType.TheForge:
                    if (alliance)
                    {
                        if (plotId == 18)
                        {
                            if (level == 1) return Alliance_Plot18_Forge_Level1;
                            if (level == 2) return Alliance_Plot18_Forge_Level2;
                            if (level == 3) return Alliance_Plot18_Forge_Level3;
                        }
                        if (plotId == 19)
                        {
                            if (level == 1) return Alliance_Plot19_Forge_Level1;
                            if (level == 2) return Alliance_Plot19_Forge_Level2;
                            if (level == 3) return Alliance_Plot19_Forge_Level3;
                        }
                        if (plotId == 20)
                        {
                            if (level == 1) return Alliance_Plot20_Forge_Level1;
                            if (level == 2) return Alliance_Plot20_Forge_Level2;
                            if (level == 3) return Alliance_Plot20_Forge_Level3;
                        }
                    }
                    else
                    {
                        if (plotId == 18)
                        {
                            if (level == 1) return Horde_Plot18_Forge_Level1;
                            if (level == 2) return Horde_Plot18_Forge_Level2;
                            if (level == 3) return Horde_Plot18_Forge_Level3;
                        }
                        if (plotId == 19)
                        {
                            if (level == 1) return Horde_Plot19_Forge_Level1;
                            if (level == 2) return Horde_Plot19_Forge_Level2;
                            if (level == 3) return Horde_Plot19_Forge_Level3;
                        }
                        if (plotId == 20)
                        {
                            if (level == 1) return Horde_Plot20_Forge_Level1;
                            if (level == 2) return Horde_Plot20_Forge_Level2;
                            if (level == 3) return Horde_Plot20_Forge_Level3;
                        }
                    }
                    break;
                case BuildingType.EnchantersStudy:
                    if (alliance)
                    {
                        if (plotId == 18)
                        {
                            if (level == 1) return Alliance_Plot18_EnchantersStudy_Level1;
                            if (level == 2) return Alliance_Plot18_EnchantersStudy_Level2;
                            if (level == 3) return Alliance_Plot18_EnchantersStudy_Level3;
                        }
                        if (plotId == 19)
                        {
                            if (level == 1) return Alliance_Plot19_EnchantersStudy_Level1;
                            if (level == 2) return Alliance_Plot19_EnchantersStudy_Level2;
                            if (level == 3) return Alliance_Plot19_EnchantersStudy_Level3;
                        }
                        if (plotId == 20)
                        {
                            if (level == 1) return Alliance_Plot20_EnchantersStudy_Level1;
                            if (level == 2) return Alliance_Plot20_EnchantersStudy_Level2;
                            if (level == 3) return Alliance_Plot20_EnchantersStudy_Level3;
                        }
                    }
                    else
                    {
                        if (plotId == 18)
                        {
                            if (level == 1) return Horde_Plot18_EnchantersStudy_Level1;
                            if (level == 2) return Horde_Plot18_EnchantersStudy_Level2;
                            if (level == 3) return Horde_Plot18_EnchantersStudy_Level3;
                        }
                        if (plotId == 19)
                        {
                            if (level == 1) return Horde_Plot19_EnchantersStudy_Level1;
                            if (level == 2) return Horde_Plot19_EnchantersStudy_Level2;
                            if (level == 3) return Horde_Plot19_EnchantersStudy_Level3;
                        }
                        if (plotId == 20)
                        {
                            if (level == 1) return Horde_Plot20_EnchantersStudy_Level1;
                            if (level == 2) return Horde_Plot20_EnchantersStudy_Level2;
                            if (level == 3) return Horde_Plot20_EnchantersStudy_Level3;
                        }
                    }
                    break;
                case BuildingType.EngineeringWorks:
                    if (alliance)
                    {
                        if (plotId == 18)
                        {
                            if (level == 1) return Alliance_Plot18_EngineeringWorks_Level1;
                            if (level == 2) return Alliance_Plot18_EngineeringWorks_Level2;
                            if (level == 3) return Alliance_Plot18_EngineeringWorks_Level3;
                        }
                        if (plotId == 19)
                        {
                            if (level == 1) return Alliance_Plot19_EngineeringWorks_Level1;
                            if (level == 2) return Alliance_Plot19_EngineeringWorks_Level2;
                            if (level == 3) return Alliance_Plot19_EngineeringWorks_Level3;
                        }
                        if (plotId == 20)
                        {
                            if (level == 1) return Alliance_Plot20_EngineeringWorks_Level1;
                            if (level == 2) return Alliance_Plot20_EngineeringWorks_Level2;
                            if (level == 3) return Alliance_Plot20_EngineeringWorks_Level3;
                        }
                    }
                    else
                    {
                        if (plotId == 18)
                        {
                            if (level == 1) return Horde_Plot18_EngineeringWorks_Level1;
                            if (level == 2) return Horde_Plot18_EngineeringWorks_Level2;
                            if (level == 3) return Horde_Plot18_EngineeringWorks_Level3;
                        }
                        if (plotId == 19)
                        {
                            if (level == 1) return Horde_Plot19_EngineeringWorks_Level1;
                            if (level == 2) return Horde_Plot19_EngineeringWorks_Level2;
                            if (level == 3) return Horde_Plot19_EngineeringWorks_Level3;
                        }
                        if (plotId == 20)
                        {
                            if (level == 1) return Horde_Plot20_EngineeringWorks_Level1;
                            if (level == 2) return Horde_Plot20_EngineeringWorks_Level2;
                            if (level == 3) return Horde_Plot20_EngineeringWorks_Level3;
                        }
                    }
                    break;
                case BuildingType.GemBoutique:
                    if (alliance)
                    {
                        if (plotId == 18)
                        {
                            if (level == 1) return Alliance_Plot18_GemBoutique_Level1;
                            if (level == 2) return Alliance_Plot18_GemBoutique_Level2;
                            if (level == 3) return Alliance_Plot18_GemBoutique_Level3;
                        }
                        if (plotId == 19)
                        {
                            if (level == 1) return Alliance_Plot19_GemBoutique_Level1;
                            if (level == 2) return Alliance_Plot19_GemBoutique_Level2;
                            if (level == 3) return Alliance_Plot19_GemBoutique_Level3;
                        }
                        if (plotId == 20)
                        {
                            if (level == 1) return Alliance_Plot20_GemBoutique_Level1;
                            if (level == 2) return Alliance_Plot20_GemBoutique_Level2;
                            if (level == 3) return Alliance_Plot20_GemBoutique_Level3;
                        }
                    }
                    else
                    {
                        if (plotId == 18)
                        {
                            if (level == 1) return Horde_Plot18_GemBoutique_Level1;
                            if (level == 2) return Horde_Plot18_GemBoutique_Level2;
                            if (level == 3) return Horde_Plot18_GemBoutique_Level3;
                        }
                        if (plotId == 19)
                        {
                            if (level == 1) return Horde_Plot19_GemBoutique_Level1;
                            if (level == 2) return Horde_Plot19_GemBoutique_Level2;
                            if (level == 3) return Horde_Plot19_GemBoutique_Level3;
                        }
                        if (plotId == 20)
                        {
                            if (level == 1) return Horde_Plot20_GemBoutique_Level1;
                            if (level == 2) return Horde_Plot20_GemBoutique_Level2;
                            if (level == 3) return Horde_Plot20_GemBoutique_Level3;
                        }
                    }
                    break;
                case BuildingType.AlchemyLab:
                    if (alliance)
                    {
                        if (plotId == 18)
                        {
                            if (level == 1) return Alliance_Plot18_AlchemyLab_Level1;
                            if (level == 2) return Alliance_Plot18_AlchemyLab_Level2;
                            if (level == 3) return Alliance_Plot18_AlchemyLab_Level3;
                        }
                        if (plotId == 19)
                        {
                            if (level == 1) return Alliance_Plot19_AlchemyLab_Level1;
                            if (level == 2) return Alliance_Plot19_AlchemyLab_Level2;
                            if (level == 3) return Alliance_Plot19_AlchemyLab_Level3;
                        }
                        if (plotId == 20)
                        {
                            if (level == 1) return Alliance_Plot20_AlchemyLab_Level1;
                            if (level == 2) return Alliance_Plot20_AlchemyLab_Level2;
                            if (level == 3) return Alliance_Plot20_AlchemyLab_Level3;
                        }
                    }
                    else
                    {
                        if (plotId == 18)
                        {
                            if (level == 1) return Horde_Plot18_AlchemyLab_Level1;
                            if (level == 2) return Horde_Plot18_AlchemyLab_Level2;
                            if (level == 3) return Horde_Plot18_AlchemyLab_Level3;
                        }
                        if (plotId == 19)
                        {
                            if (level == 1) return Horde_Plot19_AlchemyLab_Level1;
                            if (level == 2) return Horde_Plot19_AlchemyLab_Level2;
                            if (level == 3) return Horde_Plot19_AlchemyLab_Level3;
                        }
                        if (plotId == 20)
                        {
                            if (level == 1) return Horde_Plot20_AlchemyLab_Level1;
                            if (level == 2) return Horde_Plot20_AlchemyLab_Level2;
                            if (level == 3) return Horde_Plot20_AlchemyLab_Level3;
                        }
                    }
                    break;
                case BuildingType.Storehouse:
                    if (alliance)
                    {
                        if (plotId == 18)
                        {
                            if (level == 1) return Alliance_Plot18_StoreHouse_Level1;
                            if (level == 2) return Alliance_Plot18_StoreHouse_Level2;
                            if (level == 3) return Alliance_Plot18_StoreHouse_Level3;
                        }
                        if (plotId == 19)
                        {
                            if (level == 1) return Alliance_Plot19_StoreHouse_Level1;
                            if (level == 2) return Alliance_Plot19_StoreHouse_Level2;
                            if (level == 3) return Alliance_Plot19_StoreHouse_Level3;
                        }
                        if (plotId == 20)
                        {
                            if (level == 1) return Alliance_Plot20_StoreHouse_Level1;
                            if (level == 2) return Alliance_Plot20_StoreHouse_Level2;
                            if (level == 3) return Alliance_Plot20_StoreHouse_Level3;
                        }
                    }
                    else
                    {
                        if (plotId == 18)
                        {
                            if (level == 1) return Horde_Plot18_StoreHouse_Level1;
                            if (level == 2) return Horde_Plot18_StoreHouse_Level2;
                            if (level == 3) return Horde_Plot18_StoreHouse_Level3;
                        }
                        if (plotId == 19)
                        {
                            if (level == 1) return Horde_Plot19_StoreHouse_Level1;
                            if (level == 2) return Horde_Plot19_StoreHouse_Level2;
                            if (level == 3) return Horde_Plot19_StoreHouse_Level3;
                        }
                        if (plotId == 20)
                        {
                            if (level == 1) return Horde_Plot20_StoreHouse_Level1;
                            if (level == 2) return Horde_Plot20_StoreHouse_Level2;
                            if (level == 3) return Horde_Plot20_StoreHouse_Level3;
                        }
                    }
                    break;
                case BuildingType.Barn:
                    if (alliance)
                    {
                        if (plotId == 22)
                        {
                            if (level == 2) return Alliance_Plot22_Barn_Level2;
                            if (level == 3) return Alliance_Plot22_Barn_Level3;
                        }
                        if (plotId == 25)
                        {
                            if (level == 2) return Alliance_Plot25_Barn_Level2;
                            if (level == 3) return Alliance_Plot25_Barn_Level3;
                        }
                    }
                    else
                    {
                        if (plotId == 22)
                        {
                            if (level == 3) return Horde_Plot22_Barn_Level3;
                        }
                        if (plotId == 25)
                        {
                            if (level == 3) return Horde_Plot25_Barn_Level3;
                        }
                    }
                    break;

            }

            return null;
        }

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
        private static WoWPoint _sellRepairNpcLocation=WoWPoint.Zero;

        public static WoWPoint FlightPathNpcLocation
        {
            get { return _flightPathNpcLocation; }
            set { _flightPathNpcLocation = value; }
        }
        private static WoWPoint _flightPathNpcLocation;


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



        public static WoWPoint[] Trapping_Leather_ShadowMoonValley =
        {
            new WoWPoint(1163.465, -1726.961, 20.1808),
            new WoWPoint(1051.413, -1855.072, 3.917787),
            new WoWPoint(1239.585, -1999.448, 24.20251),
            new WoWPoint(1055.259, -2070.717, 19.75828),
            new WoWPoint(760.4904, -1945.726, 26.6032),
            new WoWPoint(765.1276, -1757.81, 16.09372),
            new WoWPoint(971.8748, -1454.826, 4.029249),
        };

        public static WoWPoint[] Trapping_Leather_FrostfireRidge =
        {
            new WoWPoint(6344.586, 5875.258, 114.4052),
            new WoWPoint(6492.797, 5977.817, 114.3966),
            new WoWPoint(6607.841, 6073.028, 114.3967),
        };

        public static WoWPoint[] Trapping_Fur_FrostfireRidge =
        {
            new WoWPoint(7547.808, 5324.521, 122.8218),
            new WoWPoint(7548.854, 5421.045, 122.6935),
        };

        public static WoWPoint[] Trapping_Elites_Nagrand =
        {
            new WoWPoint(3538.439, 6571.7, 4.126019),
            new WoWPoint(3511.338, 6334.348, 20.06054),
            new WoWPoint(3447.621, 6211.164, 9.633522),
            new WoWPoint(3327.353, 6402.812, 14.92476),
        };

        public static WoWPoint[] Trapping_Meat_Gorgond =
        {
            new WoWPoint(7493.459, 1599.028, 75.61114),
            new WoWPoint(7540.536, 1479.492, 62.68249),
            new WoWPoint(7607.809, 1558.483, 74.49989),
        };

        public static WoWPoint[] Trapping_Fur_Nagrand_Horde =
        {
            new WoWPoint(3092.758, 4383.859, 151.2671),
            new WoWPoint(3094.84, 4285.589, 165.1676),
            new WoWPoint(3033.612, 4455.098, 146.5908),
        };

        public static WoWPoint[] Trapping_Fur_ShadowmoonValley =
        {
            new WoWPoint(1259.823, 76.60919, 74.58547),
            new WoWPoint(1339.218, 152.0948, 63.06612),
            new WoWPoint(1230.288, 276.2763, 42.80136),
            new WoWPoint(1275.678, 379.322, 60.00982),
            new WoWPoint(1394.757, 508.1465, 71.78506),

        };


    }
}
