using System;
using System.Linq;
using Herbfunk.GarrisonBase.Cache;
using Herbfunk.GarrisonBase.Garrison.Enums;
using Styx;
using Styx.Helpers;
using Styx.WoWInternals.WoWObjects;

namespace Herbfunk.GarrisonBase.Garrison.Objects
{
    public class Building
    {
        public string ID { get; set; }
        public BuildingType Type { get; set; }
        public PlotSize Plot { get; set; }
        public int PlotId { get; set; }
        public int Level { get; set; }
        public bool CanActivate { get; set; }
        public bool IsBuilding { get; set; }
        public WorkOrderType WorkOrderType { get; set; }
        public int WorkOrderObjectEntryId { get; set; }
        public string WorkOrderObjectName { get; set; }


        public WoWGameObject WorkOrderObject
        {
            get
            {
                if (_workOrderObject == null)
                    _workOrderObject = CacheStaticLookUp.GetWoWObject(WorkOrderObjectName).ToGameObject();
                else if (!_workOrderObject.IsValid)
                    _workOrderObject = null;
                return _workOrderObject;
            }
            set { _workOrderObject = value; }
        }
        private WoWGameObject _workOrderObject;

        public bool CheckedWorkOrderPickUp { get; set; }
        public bool CheckedWorkOrderStartUp { get; set; }

        public int WorkOrderNPCEntryId
        {
            get
            {
                if (Type == BuildingType.TradingPost && _workorderNpcEntryid==-1)
                {
                    uint[] entryIds = Player.IsAlliance
                        ? WorkOrder.AllianceTradePostNpcIds.ToArray()
                        : WorkOrder.HordeTradePostNpcIds.ToArray();

                    var validunit = CacheStaticLookUp.GetWoWUnits(entryIds).FirstOrDefault();
                    if (validunit != null)
                        _workorderNpcEntryid = Convert.ToInt32(validunit.Entry);
                }
                return _workorderNpcEntryid;
            }
            set { _workorderNpcEntryid = value; }
        }
        private int _workorderNpcEntryid = -1;

        public WoWPoint SafeMovementPoint { get; set; }
        public WoWPoint EntranceMovementPoint { get; set; }
        public WoWPoint WorkOrderShipmentPoint { get; set; }

        public uint FirstQuestID { get; set; }
        public bool FirstQuestCompleted { get; set; }
        public int QuestNpcID { get; set; }



        private WorkOrder _workorder;
        public WorkOrder WorkOrder
        {
            get
            {
                if (WorkOrderType == WorkOrderType.None)
                    return null;

                if (_workorder == null)
                    _workorder = LuaCommands.GetWorkOrder(ID);   

                return _workorder;
            }
        }
        public Building(string id)
        {
            ID = id;
            Type = GetBuildingTypeUsingId(Convert.ToInt32(id));
            Level = GetBuildingUpgradeLevel(Convert.ToInt32(id));
            Plot = GetBuildingPlotSize(Type);
                
            WorkOrderType = WorkOrder.GetWorkorderType(Type);

            if (WorkOrderType != WorkOrderType.None)
            {
                WorkOrderObjectEntryId = WorkOrder.WorkOrderPickupEntryIds[WorkOrderType];
                WorkOrderObjectName = WorkOrder.WorkOrderPickupNames[WorkOrderType];
                if (WorkOrderType == WorkOrderType.WarMillDwarvenBunker && Player.IsAlliance )
                    WorkOrderObjectName = WorkOrder.WorkOrderPickupNames[WorkOrderType.DwarvenBunker];
            }
            else
            {
                WorkOrderObjectEntryId = -1;
                WorkOrderObjectName = String.Empty;
            }

            WorkOrderNPCEntryId = WorkOrder.GetWorkOrderNpcEntryId(Type, Player.IsAlliance);


            if (GarrisonManager.BuildingIDs.Contains(ID))
            {
                string plotid;
                bool canActivate, isBuilding;
                int shipTotal, shipCap, shipReady;

                LuaCommands.GetBuildingInfo(id, out plotid, out canActivate, out shipCap, out shipReady, out shipTotal, out isBuilding);
                if (WorkOrderType != WorkOrderType.None)
                    _workorder = new WorkOrder(ID, Type, WorkOrderType, shipCap,
                        WorkOrder.GetWorkOrderItemAndQuanityRequired(WorkOrderType), shipReady, shipTotal);

                PlotId = plotid.ToInt32();
                CanActivate = canActivate;
                IsBuilding = isBuilding;

                int _plotId = Convert.ToInt32(PlotId);
                SafeMovementPoint = MovementCache.GetBuildingSafeMovementPoint(_plotId);
                EntranceMovementPoint = MovementCache.GetBuildingEntranceMovementPoint(_plotId);
            }
            else if (Type == BuildingType.HerbGarden || Type == BuildingType.Mines)
            {//if not completed first quest than we must insert temp info.
                PlotId = Type == BuildingType.HerbGarden ? 63 : 59;
                int _plotId = Convert.ToInt32(PlotId);
                SafeMovementPoint = MovementCache.GetBuildingSafeMovementPoint(_plotId);
                EntranceMovementPoint = MovementCache.GetBuildingEntranceMovementPoint(_plotId);
                _workorder = new WorkOrder(ID, Type, WorkOrderType, 0,
                       WorkOrder.GetWorkOrderItemAndQuanityRequired(WorkOrderType));

            }


            int firstquestID = GetBuildingFirstQuestId(Type, Player.IsAlliance);
            if (firstquestID > 0)
            {
                FirstQuestID = Convert.ToUInt32(firstquestID);
                FirstQuestCompleted = LuaCommands.IsQuestFlaggedCompleted(FirstQuestID.ToString());

                if (Type == BuildingType.TradingPost)
                    QuestNpcID = WorkOrderNPCEntryId;
                else
                    QuestNpcID = GetBuildingFirstQuestNpcId(Type, Player.IsAlliance);
            }
            else
            {
                FirstQuestID = 0;
                QuestNpcID = 0;
                FirstQuestCompleted = true;
            }
        }

        public override string ToString()
        {
            return String.Format("Building Id: {0} Type {1} Level {10} WorkOrder {2} PlotId {6}\r\n" +
                                 "CanActivate {8} IsBuilding {9}\r\n" +
                                 "WorkOrderNPCEntryId {14} WorkOrderEntryId {3} CheckedWorkOrderPickUp {4} CheckedWorkOrderStartUp {5}\r\n" +
                                 "FirstQuestID {11} QuestNpcID {12} FirstQuestCompleted {13}" +
                                 "{7}",
                ID, Type, WorkOrderType, WorkOrderObjectEntryId, CheckedWorkOrderPickUp, CheckedWorkOrderStartUp, PlotId, WorkOrder!=null?"\r\n" + WorkOrder.ToString():"",
                CanActivate, IsBuilding, Level, FirstQuestID, QuestNpcID, FirstQuestCompleted, WorkOrderNPCEntryId);
        }


        #region Static Methods

        public static BuildingType GetBuildingTypeUsingId(int id)
        {
            switch (id)
            {
                case 8:
                case 9:
                case 10:
                    return BuildingType.WarMillDwarvenBunker;
                case 24:
                case 25:
                case 133:
                    return BuildingType.Barn;
                case 26:
                case 27:
                case 28:
                    return BuildingType.Barracks;
                case 162:
                case 163:
                case 164:
                    return BuildingType.GoblinWorkshop;
                case 37:
                case 38:
                case 39:
                    return BuildingType.SpiritLodge;
                case 65:
                case 66:
                case 67:
                    return BuildingType.Stables;
                case 159:
                case 160:
                case 161:
                    return BuildingType.GladiatorsSanctum;
                case 40:
                case 41:
                case 138:
                    return BuildingType.Lumbermill;
                case 34:
                case 35:
                case 36:
                    return BuildingType.FrostwallTavern;
                case 111:
                case 144:
                case 145:
                    return BuildingType.TradingPost;
                case 76:
                case 119:
                case 120:
                    return BuildingType.AlchemyLab;
                case 93:
                case 125:
                case 126:
                    return BuildingType.EnchantersStudy;
                case 91:
                case 123:
                case 124:
                    return BuildingType.EngineeringWorks;
                case 96:
                case 131:
                case 132:
                    return BuildingType.GemBoutique;
                case 52:
                case 140:
                case 141:
                    return BuildingType.SalvageYard;
                case 95:
                case 129:
                case 130:
                    return BuildingType.ScribesQuarters;
                case 51:
                case 142:
                case 143:
                    return BuildingType.Storehouse;
                case 94:
                case 127:
                case 128:
                    return BuildingType.TailoringEmporium;
                case 60:
                case 117:
                case 118:
                    return BuildingType.TheForge;
                case 90:
                case 121:
                case 122:
                    return BuildingType.TheTannery;
                case 29:
                case 136:
                case 137:
                    return BuildingType.HerbGarden;
                case 31:
                case 134:
                case 135:
                    return BuildingType.FishingShack;
                case 61:
                case 62:
                case 63:
                    return BuildingType.Mines;
                case 42:
                case 167:
                case 168:
                    return BuildingType.PetMenagerie;
                case 64:
                    return BuildingType.FishingShack;
            }

            return BuildingType.Unknown;
        }

        public static int GetBuildingUpgradeLevel(int plotid)
        {
            switch (plotid)
            {
                //level 1
                case 8:
                case 24:
                case 26:
                case 162:
                case 37:
                case 65:
                case 159:
                case 40:
                case 34:
                case 111:
                case 76:
                case 93:
                case 91:
                case 96:
                case 52:
                case 95:
                case 51:
                case 94:
                case 60:
                case 90:
                case 29:
                case 31:
                case 61:
                case 42:
                case 64:
                    return 1;

                //Level 2
                case 9:
                case 25:
                case 27:
                case 163:
                case 38:
                case 66:
                case 160:
                case 41:
                case 35:
                case 144:
                case 167:
                case 62:
                case 134:
                case 136:
                case 121:
                case 117:
                case 127:
                case 142:
                case 129:
                case 140:
                case 131:
                case 123:
                case 125:
                case 119:
                    return 2;

                //Level 3
                case 10:
                case 133:
                case 28:
                case 164:
                case 39:
                case 67:
                case 161:
                case 138:
                case 36:
                case 145:
                case 120:
                case 126:
                case 124:
                case 132:
                case 141:
                case 130:
                case 143:
                case 128:
                case 118:
                case 122:
                case 137:
                case 135:
                case 63:
                case 168:
                    return 3;
            }

            return -1;
        }

        public static PlotSize GetBuildingPlotSize(BuildingType type)
        {
            switch (type)
            {
                case BuildingType.WarMillDwarvenBunker:
                case BuildingType.Barracks:
                case BuildingType.GoblinWorkshop:
                case BuildingType.GnomishGearworks:
                case BuildingType.SpiritLodge:
                case BuildingType.MageTower:
                case BuildingType.Stables:
                    return PlotSize.Large;
                case BuildingType.Lumbermill:
                case BuildingType.Barn:
                case BuildingType.FrostwallTavern:
                case BuildingType.GladiatorsSanctum:
                case BuildingType.TradingPost:
                    return PlotSize.Medium;
                case BuildingType.TailoringEmporium:
                case BuildingType.ScribesQuarters:
                case BuildingType.TheTannery:
                case BuildingType.TheForge:
                case BuildingType.EnchantersStudy:
                case BuildingType.EngineeringWorks:
                case BuildingType.GemBoutique:
                case BuildingType.AlchemyLab:
                case BuildingType.Storehouse:
                case BuildingType.SalvageYard:
                    return PlotSize.Small;
                case BuildingType.FishingShack:
                case BuildingType.HerbGarden:
                    return PlotSize.Herb;
                case BuildingType.Mines:
                    return PlotSize.Mine;
                case BuildingType.PetMenagerie:
                    return PlotSize.Pet;
            }
            return PlotSize.Unknown;
        }


        public static int GetBuildingFirstQuestId(BuildingType type, bool isAlly)
        {
            switch (type)
            {
                case BuildingType.WarMillDwarvenBunker:
                    break;
                case BuildingType.Barracks:
                    break;
                case BuildingType.GoblinWorkshop:
                    break;
                case BuildingType.GnomishGearworks:
                    break;
                case BuildingType.SpiritLodge:
                    break;
                case BuildingType.MageTower:
                    break;
                case BuildingType.Stables:
                    break;
                case BuildingType.Lumbermill:
                    return isAlly ? 36189 : 36137;
                case BuildingType.Barn:
                    break;
                case BuildingType.FrostwallTavern:
                    break;
                case BuildingType.GladiatorsSanctum:
                    break;
                case BuildingType.TradingPost:
                    return isAlly ? 37088 : 37062;
                case BuildingType.TailoringEmporium:
                    return isAlly ? 36643 : 37575;
                case BuildingType.ScribesQuarters:
                    return isAlly ? 36647 : 37572;
                case BuildingType.TheTannery:
                    return isAlly ? 36642 : 37574;
                case BuildingType.TheForge:
                    return isAlly ? 35168 : 37569;
                case BuildingType.EnchantersStudy:
                    return isAlly ? 36645 : 37570;
                case BuildingType.EngineeringWorks:
                    return isAlly ? 36646 : 37571;
                case BuildingType.GemBoutique:
                    return isAlly ? 36644 : 37573;
                case BuildingType.AlchemyLab:
                    return isAlly ? 36641 : 37568;
                case BuildingType.Storehouse:
                    return isAlly ? 37087 : 37060;
                case BuildingType.SalvageYard:
                    return isAlly ? 37086 : 37045;
                case BuildingType.HerbGarden:
                    return isAlly ? 36404 : 34193;
                case BuildingType.Mines:
                    return isAlly ? 34192 : 35154;
            }

            return -1;
        }

        public static int GetBuildingFirstQuestNpcId(BuildingType type, bool isAlly)
        {
            switch (type)
            {
                case BuildingType.WarMillDwarvenBunker:
                    break;
                case BuildingType.Barracks:
                    break;
                case BuildingType.GoblinWorkshop:
                    break;
                case BuildingType.GnomishGearworks:
                    break;
                case BuildingType.SpiritLodge:
                    break;
                case BuildingType.MageTower:
                    break;
                case BuildingType.Stables:
                    break;
                case BuildingType.Lumbermill:
                    return isAlly ? 84248 : 84247;
                case BuildingType.Barn:
                    break;
                case BuildingType.FrostwallTavern:
                    break;
                case BuildingType.GladiatorsSanctum:
                    break;
                case BuildingType.TradingPost:
                    return isAlly ? 37086 : 37062;
                case BuildingType.TailoringEmporium:
                    return isAlly ? 77382 : 79864;
                case BuildingType.ScribesQuarters:
                    return isAlly ? 77372 : 79829;
                case BuildingType.TheTannery:
                    return isAlly ? 77383 : 79834;
                case BuildingType.TheForge:
                    return isAlly ? 77359 : 79867;
                case BuildingType.EnchantersStudy:
                    return isAlly ? 77354 : 79821;
                case BuildingType.EngineeringWorks:
                    return isAlly ? 77365 : 79826;
                case BuildingType.GemBoutique:
                    return isAlly ? 77356 : 79832;
                case BuildingType.AlchemyLab:
                    return isAlly ? 77363 : 79813;
                case BuildingType.Storehouse:
                    return isAlly ? 84857 : 79862;
                case BuildingType.SalvageYard:
                    return isAlly ? 77378 : 79857;
                case BuildingType.HerbGarden:
                    return isAlly ? 85344 : 81981;
                case BuildingType.Mines:
                    return isAlly ? 77730 : 81688;
            }

            return -1;
        }
        
        #endregion

    }
}