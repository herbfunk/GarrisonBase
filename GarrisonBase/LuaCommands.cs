using System;
using System.Collections.Generic;
using System.Linq;
using Herbfunk.GarrisonBase.Garrison.Enums;
using Herbfunk.GarrisonBase.Garrison.Objects;
using Styx;
using Styx.Common;
using Styx.Helpers;
using Styx.WoWInternals;

namespace Herbfunk.GarrisonBase
{
    static partial class LuaCommands
    {
        


        public static bool TestLuaInjectionCode()
        {
            GarrisonBase.Debug("LuaCommand: {0}", String.Format("return {0}()", LuaEvents.TestFunctionString));
            string lua = String.Format("return {0}()", LuaEvents.TestFunctionString);
            List<string> retvalues = Lua.GetReturnValues(lua, "clicky.lua");
            return retvalues != null;
        }

        public static int GetNumberContainerSlots(int index)
        {
            GarrisonBase.Debug("LuaCommand: GetNumberContainerSlots");
            string lua = String.Format("return GetContainerNumSlots({0})", index);
            List<string> retvalues = Lua.GetReturnValues(lua);
            return retvalues[0].ToInt32();
        }

        public static int GetCurrencyCount(int currencyId)
        {
            GarrisonBase.Debug("LuaCommand: GetCurrencyInfo");
            string lua = String.Format("return GetCurrencyInfo({0})", currencyId);
            List<string> retvalues = Lua.GetReturnValues(lua);
            return retvalues[1].ToInt32();
        }
        public static bool GetBagSlotFlag(int bagIndex, int flag)
        {
            GarrisonBase.Debug("LuaCommand: GetBagSlotFlag");
            string lua = String.Format("return tostring(GetBagSlotFlag(\"{0}\", \"{1}\"))", bagIndex, flag);
            List<string> retList = Lua.GetReturnValues(lua);
            return retList[0].ToBoolean();
        }
        public static void ClickGarrisonMinimapButton()
        {
            GarrisonBase.Debug("LuaCommand: ClickGarrisonMinimapButton");
            Lua.DoString("GarrisonLandingPageMinimapButton:Click()");
        }

        public static void CloseTradeSkillFrame()
        {
            GarrisonBase.Debug("LuaCommand: CloseTradeSkillFrame");
            Lua.DoString("TradeSkillFrameCloseButton:Click()");
        }


        public static WorkOrder GetWorkOrder(int buildingId)
        {
            GarrisonBase.Debug("LuaCommand: GetWorkOrder {0}", buildingId);
            List<string> workorderinfo = Lua.GetReturnValues(String.Format("return C_Garrison.GetLandingPageShipmentInfo(\"{0}\")", buildingId));
            if (workorderinfo.Count < 2) return null;

            BuildingType buildingType = Building.GetBuildingTypeUsingId(buildingId);
            WorkOrderType workorderType = WorkOrder.GetWorkorderType(buildingType);
            int MaxOrders = workorderinfo[2].ToInt32();
            Tuple<CraftingReagents, int>[] price = WorkOrder.GetWorkOrderItemAndQuanityRequired(workorderType);
            int PendingOrders = 0;
            int PickupOrders = 0;
            if (workorderinfo.Count > 3)
            {
                PickupOrders = workorderinfo[3].ToInt32();
                PendingOrders = workorderinfo[4].ToInt32();
            }

            if (price == null)
            {
                PendingOrders = 0;
                PickupOrders = 0;
                MaxOrders = 0;
                price = new[] {new Tuple<CraftingReagents, int>(CraftingReagents.None,9999) };
            }

            return new WorkOrder(buildingId, buildingType, workorderType, MaxOrders, price, PendingOrders,PickupOrders);
        }
        public static bool IsGarrisonCapacitiveDisplayFrame()
        {
            const string lua =
                "if not GarrisonCapacitiveDisplayFrame then return false; else return tostring(GarrisonCapacitiveDisplayFrame:IsVisible());end;";
            string t = Lua.GetReturnValues(lua)[0];
            bool ret = t.ToBoolean();



            GarrisonBase.Debug("LuaCommand: IsGarrisonCapacitiveDisplayFrame {0}", ret);
            return ret;
        }


        public static bool IsStaticPopupVisible()
        {
            GarrisonBase.Debug("LuaCommand: IsStaticPopupVisible");
            const string lua = "return tostring(StaticPopup1:IsVisible())";
            string t = Lua.GetReturnValues(lua)[0];
            bool ret = t.ToBoolean();
            return ret;
        }

        public static void PickupContainerItem(int bagIndex, int bagSlot)
        {
            GarrisonBase.Debug("LuaCommand: PickupContainerItem {0} / {1}", bagIndex, bagSlot);
            Lua.DoString(String.Format("PickupContainerItem(\"{0}\"" + ", \"{1}\");", bagIndex, bagSlot));
        }
        public static bool CursorHasItem()
        {
            GarrisonBase.Debug("LuaCommand: CursorHasItem");
            return Lua.GetReturnValues(String.Format("return CursorHasItem()"))[0].ToInt32()==1;
        }

        public static void ClearCursor()
        {
            GarrisonBase.Debug("LuaCommand: ClearCursor");
            Lua.DoString("ClearCursor()");
        }

        public static void SplitContainerItem(int bagindex, int bagslot, int count)
        {
            GarrisonBase.Debug("LuaCommand: SplitContainerItem {0} / {1} - Count {2}", bagindex, bagslot, count);
            Lua.DoString(String.Format("SplitContainerItem(\"{0}\"" + ", \"{1}\"" + ", \"{2}\");", bagindex, bagslot, count));
        }

        public static void UseContainerItem(int bag, int index)
        {
            //
            
            GarrisonBase.Debug("LuaCommand: UseContainerItem {0} / {1}", bag, index);
            Lua.DoString(String.Format("UseContainerItem(\"{0}\"" + ", \"{1}\");", bag, index));
        }


        public static bool OpenMailFrameIsVisible()
        {
            GarrisonBase.Debug("LuaCommand: OpenMailFrameIsVisible");
            string t = Lua.GetReturnValues("return tostring(OpenMailFrame:IsVisible())")[0];
            bool ret = t.ToBoolean();
            return ret;
        }

        public static bool HasNewMail()
        {
            GarrisonBase.Debug("LuaCommand: HasNewMail");
            return Lua.GetReturnValues(String.Format("return HasNewMail()"))[0].ToInt32() == 1;
        }
        public static void ClickMailItemButton(int index)
        {
            //
            GarrisonBase.Debug("LuaCommand: ClickMailItemButton {0}", index);
            Lua.DoString(String.Format("MailItem{0}Button:Click()", index));
        }

        public static void ClickOpenMailAttachmentButton(int index)
        {
            GarrisonBase.Debug("LuaCommand: ClickOpenMailAttachmentButton {0}", index);
            Lua.DoString(String.Format("OpenMailAttachmentButton{0}:Click()", index));
        }

        public static void ClickSendMailButton()
        {
            GarrisonBase.Debug("LuaCommand: SetSendMailRecipient");
            Lua.DoString("SendMailMailButton:Click();");
        }
        public static void SetSendMailRecipient(string name)
        {
            GarrisonBase.Debug("LuaCommand: SetSendMailRecipient");
            Lua.DoString(String.Format("SendMailNameEditBox:SetText(\"{0}\")", name));
        }
        public static bool IsSendMailFrameVisible()
        {
            //SendMailFrame
            GarrisonBase.Debug("LuaCommand: IsSendMailFrameVisible");
            const string lua = "return tostring(SendMailFrame:IsVisible())";
            string t = Lua.GetReturnValues(lua)[0];
            bool ret = t.ToBoolean();
            return ret;
        }
        public static void ClickSendMailTab()
        {
            //
            GarrisonBase.Debug("LuaCommand: ClickSendMailTab");
            Lua.DoString("MailFrameTab2:Click()");
        }

        public static void ClickStaticPopupButton(int buttonNumber)
        {
            GarrisonBase.Debug("LuaCommand: ClickStaticPopupButton");
            Lua.DoString(String.Format("StaticPopup1Button{0}:Click()", buttonNumber));
        }
        public static void ClickStartOrderButton()
        {
            GarrisonBase.Debug("LuaCommand: ClickStartOrderButton");
            Lua.DoString("GarrisonCapacitiveDisplayFrame.StartWorkOrderButton:Click()");
        }

        public static bool ClickStartOrderButtonEnabled()
        {
            //CreateAllWorkOrdersButton
            String lua = "return tostring(GarrisonCapacitiveDisplayFrame.StartWorkOrderButton:IsEnabled())";
            var ret = Lua.GetReturnValues(lua)[0].ToBoolean();
            GarrisonBase.Debug("LuaCommand: ClickStartOrderButtonEnabled {0}", ret);
            return ret;
        }

      


        public static void GetBuildingInfo(int buildingId, out string plotId, out bool canActivate, out int shipCap, out int shipReady, out int shipTotal, out bool isBuilding)
        {
            
            String lua =
                "C_Garrison.RequestLandingPageShipmentInfo();" +
                "local RetInfo = {}; Temp = {}; local buildings = C_Garrison.GetBuildings();" +
                String.Format(
                    "for i = 1, #buildings do " +
                    "local buildingID = buildings[i].buildingID;" +
                    "if (buildingID == {0}) then " +
                    "local nameShipment, texture, shipmentCapacity, shipmentsReady, shipmentsTotal, creationTime, duration, timeleftString, itemName, itemIcon, itemQuality, itemID = C_Garrison.GetLandingPageShipmentInfo(buildingID);" +
                    "local id, name, texPrefix, icon, rank, isBuilding, timeStart, buildTime, canActivate, canUpgrade, isPrebuilt = C_Garrison.GetOwnedBuildingInfoAbbrev(buildings[i].plotID);" +
                    "Temp[0] = buildings[i].buildingID;" +
                    "Temp[1] = buildings[i].plotID;" +
                    "Temp[2] = buildings[i].buildingLevel;" +
                    "Temp[3] = name;" +
                    "Temp[4] = rank;" +
                    "if (not isBuilding) then Temp[5] =  0; else Temp[5] = isBuilding;end;" +
                    "Temp[6] = timeStart;" +
                    "Temp[7] = buildTime;" +
                    "if (not canActivate) then Temp[8] =  0; else Temp[8] = canActivate;end;" +
                    "Temp[9] = canUpgrade;" +
                    "Temp[11] = isPrebuilt;" +
                // Info on shipments
                    "Temp[12] = nameShipment;" +
                    "if (not shipmentCapacity) then Temp[13] =  0; else Temp[13] = shipmentCapacity;end;" +
                    "if (not shipmentsReady) then Temp[14] = 0; else Temp[14] = shipmentsReady;end;" +
                    "if (not shipmentsTotal) then Temp[15] =  0; else Temp[15] = shipmentsTotal;end;" +
                    "Temp[16] = creationTime;" +
                    "Temp[17] = duration;" +
                    "Temp[18] = itemName;" +
                    "Temp[19] = itemQuality;" +
                    "Temp[20] = itemID;" +
                    "end;" +
                    "end;" +
                    "for j_=0,20 do table.insert(RetInfo,tostring(Temp[j_]));end; " +
                    "return unpack(RetInfo)", buildingId);
            List<String> building = Lua.GetReturnValues(lua);
            int id = building[0].ToInt32();

            plotId = building[1];
            canActivate = building[8].ToBoolean();
            isBuilding = building[5].ToBoolean();
            string shipmentsReady = building[15];
            string shipmentsTotal = building[14];
            string shipmentsCap = building[13];
            shipCap = Convert.ToInt32(shipmentsCap);
            shipReady = Convert.ToInt32(shipmentsReady);
            shipTotal = Convert.ToInt32(shipmentsTotal);

            GarrisonBase.Debug("LuaCommand: GetBuildingInfo {0} canActivate {1} " +
                               "isBuilding {2} shipmentsReady {3} shipmentsTotal {4} " +
                               "shipmentsCap {5}",
                               buildingId, canActivate, isBuilding, shipmentsReady, shipmentsTotal, shipmentsCap);
        }
        public static List<int> GetBuildingIds()
        {
            GarrisonBase.Debug("LuaCommand: GetBuildingIds");
            String lua =
                "local RetInfo = {}; local buildings = C_Garrison.GetBuildings();" +
                "for i = 1, #buildings do " +
                "table.insert(RetInfo,tostring(buildings[i].buildingID));" +
                "end;" +
                "return unpack(RetInfo)";
            List<string> retValues=Lua.GetReturnValues(lua);
            return retValues.Select(value => value.ToInt32()).ToList();
        }


        public static bool IsQuestFlaggedCompleted(string ID)
        {
            GarrisonBase.Debug("LuaCommand: IsQuestFlaggedCompleted {0}", ID);
            string lua = String.Format("return tostring(IsQuestFlaggedCompleted(\"{0}\"))", ID);
            List<string> retList = Lua.GetReturnValues(lua);
            return retList[0].ToBoolean();
        }

        public static DateTime GetGameTime()
        {
            string lua = String.Format("return GetGameTime()");
            List<string> retvalues = Lua.GetReturnValues(lua);

            int hour = retvalues[0].ToInt32();
            int minute = retvalues[1].ToInt32();
            DateTime now = DateTime.Now;
            DateTime gameTime=new DateTime(now.Year, now.Month, now.Day, hour, minute, 0);
            GarrisonBase.Debug("LuaCommand: GetGameTime {0}", gameTime.ToString());

            return gameTime;
        }

        public static bool CheckForDailyReset(DateTime date)
        {
            DateTime now = DateTime.Now;
            DateTime today = new DateTime(now.Year, now.Month, now.Day, 6, 0, 0);
            DateTime yesterday = today.Subtract(new TimeSpan(1, 0, 0, 0));
            int yesterday_compare = DateTime.Compare(yesterday, date);
            int today_compare = DateTime.Compare(today, date);

            if (yesterday_compare > 0)
            {
                //passed!
                return true;
            }

            //Reset has not occured today..
            if (now.Hour < 6)
            {
                return false;
            }
            //Test Date exceedes the 6am reset
            if (date.Hour > 6 && (date.Year == now.Year && date.Month == now.Year && date.Day == now.Day))
            {
                return false;
            }
            //
            if (today_compare > 0)
            {
                return true;
            }

            return false;
        }

        public static bool IsAddonLoaded(string AddonName)
        {
            var luastr = String.Format("return GetAddOnEnableState(\"{0}\",\"{1}\")", StyxWoW.Me.Name, AddonName);
            List<string> retList = Lua.GetReturnValues(luastr);
            var retValue = retList[0].ToInt32();
            GarrisonBase.Debug("LuaCommand: GetAddOnEnableState {0} == ({1})", AddonName, retValue);
            return retValue > 0;
        }
        public static void DisableAddon(string AddonName)
        {
            GarrisonBase.Debug("LuaCommand: DisableAddOn {0}", AddonName);
            Lua.DoString(String.Format("DisableAddOn(\"{0}\")", AddonName));
        }
        public static void EnableAddon(string AddonName)
        {
            GarrisonBase.Debug("LuaCommand: EnableAddOn {0}", AddonName);
            Lua.DoString(String.Format("EnableAddOn(\"{0}\")", AddonName));
        }
        public static void ReloadUI()
        {
            
            GarrisonBase.Debug("LuaCommand: ReloadUI");
            Lua.DoString("ReloadUI()");
        }
    }
}
