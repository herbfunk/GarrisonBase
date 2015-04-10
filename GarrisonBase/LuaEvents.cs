using System;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Herbfunk.GarrisonBase.Helpers;
using Herbfunk.GarrisonBase.Properties;
using Styx.WoWInternals;

namespace Herbfunk.GarrisonBase
{


    public static class LuaEvents
    {
        static LuaEvents()
        {
            TestFunctionString = StringHelper.RandomString;
            SuccessFunctionString = StringHelper.RandomString;
            ClickFunctionString = StringHelper.RandomString;
        }

        public static bool ShipmentOrderFrameOpen { get; set; }
        public static bool MailOpen { get; set; }
        public static bool LootOpened { get; set; }
        public static bool TradeSkillFrameOpen { get; set; }


        public static void ResetFrameVariables()
        {
            ShipmentOrderFrameOpen = false;
            MailOpen = false;
            LootOpened = false;
            TradeSkillFrameOpen = false;
        }

        public delegate void LuaEventFired();

        public static event LuaEventFired OnGarrisonMissionStarted;
        public static event LuaEventFired OnGarrisonMissionListUpdated;
        public static event LuaEventFired OnGarrisonFollowerListUpdated;
        public static event LuaEventFired OnGarrisonMissionNpcOpened;
        public static event LuaEventFired OnGarrisonMissionNpcClosed;
        public static event LuaEventFired OnGarrisonLandingPageShow;
        public static event LuaEventFired OnGarrisonMissionFinished;
        public static event LuaEventFired OnGarrisonBuildingActivatable;
        public static void GARRISON_MISSION_STARTED(object sender, LuaEventArgs args)
        {
            GarrisonBase.DebugLuaEvent(String.Format("LuaEvent: {0}", "Garrison Mission Started!"));
            if (OnGarrisonMissionStarted != null) OnGarrisonMissionStarted();
        }
        public static void GARRISON_MISSION_LIST_UPDATE(object sender, LuaEventArgs args)
        {
            GarrisonBase.DebugLuaEvent(String.Format("LuaEvent: {0}", "Garrison Mission List Updated!"));
            if (OnGarrisonMissionListUpdated != null) OnGarrisonMissionListUpdated();
        }
        public static void GARRISON_FOLLOWER_LIST_UPDATE(object sender, LuaEventArgs args)
        {
            GarrisonBase.DebugLuaEvent(String.Format("LuaEvent: {0}", "Garrison Follower List Updated!"));
            if (OnGarrisonFollowerListUpdated != null) OnGarrisonFollowerListUpdated();
        }
        public static void GARRISON_MISSION_NPC_OPENED(object sender, LuaEventArgs args)
        {
            GarrisonBase.DebugLuaEvent(String.Format("LuaEvent: {0}", "Garrison Mission NPC Opened!"));
            if (OnGarrisonMissionNpcOpened != null) OnGarrisonMissionNpcOpened();
        }
        public static void GARRISON_MISSION_NPC_CLOSED(object sender, LuaEventArgs args)
        {
            GarrisonBase.DebugLuaEvent(String.Format("LuaEvent: {0}", "Garrison Mission NPC Closed!"));
            if (OnGarrisonMissionNpcClosed != null) OnGarrisonMissionNpcClosed();
        }
        public static void GARRISON_SHOW_LANDING_PAGE(object sender, LuaEventArgs args)
        {
            GarrisonBase.DebugLuaEvent(String.Format("LuaEvent: {0}", "Garrison Landing Page Shown!"));
            if (OnGarrisonLandingPageShow != null) OnGarrisonLandingPageShow();
        }
        public static void GARRISON_MISSION_FINISHED(object sender, LuaEventArgs args)
        {
            GarrisonBase.DebugLuaEvent("LuaEvent: GARRISON_MISSION_FINISHED");
            if (OnGarrisonMissionFinished != null) OnGarrisonMissionFinished();
        }
        public static void GARRISON_BUILDING_ACTIVATABLE(object sender, LuaEventArgs args)
        {
            GarrisonBase.DebugLuaEvent("LuaEvent: GARRISON_BUILDING_ACTIVATABLE");
            if (OnGarrisonBuildingActivatable != null) OnGarrisonBuildingActivatable();
        }
        //

        public static event LuaEventFired OnShipmentCrafterOpened;
        public static event LuaEventFired OnShipmentCrafterClosed;
        public static void SHIPMENT_CRAFTER_OPENED(object sender, LuaEventArgs args)
        {
            GarrisonBase.DebugLuaEvent("LuaEvent: SHIPMENT_CRAFTER_OPENED");
            ShipmentOrderFrameOpen = true;
            if (OnShipmentCrafterOpened != null) OnShipmentCrafterOpened();
        }
        public static void SHIPMENT_CRAFTER_CLOSED(object sender, LuaEventArgs args)
        {
            GarrisonBase.DebugLuaEvent("LuaEvent: SHIPMENT_CRAFTER_CLOSED");
            ShipmentOrderFrameOpen = false;
            if (OnShipmentCrafterClosed != null) OnShipmentCrafterClosed();
        }

        public static event LuaEventFired OnLootClosed;
        public static event LuaEventFired OnLootOpened;
        public static void LOOT_CLOSED(object sender, LuaEventArgs args)
        {
            GarrisonBase.DebugLuaEvent("LuaEvent: LOOT_CLOSED");
            LootOpened = false;
            if (OnLootClosed != null) OnLootClosed();
        }
        public static void LOOT_OPENED(object sender, LuaEventArgs args)
        {
            GarrisonBase.DebugLuaEvent("LuaEvent: LOOT_OPENED");
            LootOpened = true;
            if (OnLootOpened != null) OnLootOpened();
        }

        public static event LuaEventFired OnQuestDetail;
        public static event LuaEventFired OnQuestProgress;
        public static event LuaEventFired OnQuestComplete;
        public static event LuaEventFired OnQuestFinished;
        public static event LuaEventFired OnQuestLogUpdate;
        public static event LuaEventFired OnQuestRemoved;
        public static event LuaEventFired OnQuestAccepted;
        public static event LuaEventFired OnQuestWatchUpdate;
        public static void QUEST_DETAIL(object sender, LuaEventArgs args)
        {
            GarrisonBase.DebugLuaEvent("LuaEvent: QUEST_DETAIL");
 
            if (OnQuestDetail != null) OnQuestDetail();
        }
        public static void QUEST_PROGRESS(object sender, LuaEventArgs args)
        {
            GarrisonBase.DebugLuaEvent("LuaEvent: QUEST_PROGRESS");
            
            if (OnQuestProgress != null) OnQuestProgress();
        }
        public static void QUEST_COMPLETE(object sender, LuaEventArgs args)
        {
            GarrisonBase.DebugLuaEvent("LuaEvent: QUEST_COMPLETE");
            
            if (OnQuestComplete != null) OnQuestComplete();
        }
        public static void QUEST_FINISHED(object sender, LuaEventArgs args)
        {
            GarrisonBase.DebugLuaEvent("LuaEvent: QUEST_FINISHED");
            
            if (OnQuestFinished != null) OnQuestFinished();
        }
        public static void QUEST_LOG_UPDATE(object sender, LuaEventArgs args)
        {
            GarrisonBase.DebugLuaEvent("LuaEvent: QUEST_LOG_UPDATE");
            if (OnQuestLogUpdate != null) OnQuestLogUpdate();
        }
        public static void QUEST_REMOVED(object sender, LuaEventArgs args)
        {
            GarrisonBase.DebugLuaEvent("LuaEvent: QUEST_REMOVED");
            
            if (OnQuestRemoved != null) OnQuestRemoved();
        }
        public static void QUEST_ACCEPTED(object sender, LuaEventArgs args)
        {
            GarrisonBase.DebugLuaEvent("LuaEvent: QUEST_ACCEPTED");
            
            if (OnQuestAccepted != null) OnQuestAccepted();
        }
        public static void QUEST_WATCH_UPDATE(object sender, LuaEventArgs args)
        {
            GarrisonBase.DebugLuaEvent("LuaEvent: QUEST_WATCH_UPDATE");
            
            if (OnQuestWatchUpdate != null) OnQuestWatchUpdate();
        }


        public static event LuaEventFired OnGossipShow;
        public static event LuaEventFired OnGossipClosed;
        public static void GOSSIP_SHOW(object sender, LuaEventArgs args)
        {
            GarrisonBase.DebugLuaEvent("LuaEvent: GOSSIP_SHOW");
            
            if (OnGossipShow != null) OnGossipShow();
        }
        public static void GOSSIP_CLOSED(object sender, LuaEventArgs args)
        {
            GarrisonBase.DebugLuaEvent("LuaEvent: GOSSIP_CLOSED");
            
            if (OnGossipClosed != null) OnGossipClosed();
        }

        public static event LuaEventFired OnTradeSkillShow;
        public static event LuaEventFired OnTradeSkillClosed;
        public static void TRADE_SKILL_CLOSE(object sender, LuaEventArgs args)
        {
            GarrisonBase.DebugLuaEvent("LuaEvent: TRADE_SKILL_CLOSE");
            TradeSkillFrameOpen = false;
            if (OnTradeSkillClosed != null) OnTradeSkillClosed();
        }
        public static void TRADE_SKILL_SHOW(object sender, LuaEventArgs args)
        {
            GarrisonBase.DebugLuaEvent("LuaEvent: TRADE_SKILL_SHOW");
            TradeSkillFrameOpen = true;
            if (OnTradeSkillShow != null) OnTradeSkillShow();
        }

        public static event LuaEventFired OnMerchantShow;
        public static event LuaEventFired OnMerchantClosed;
        public static void MERCHANT_SHOW(object sender, LuaEventArgs args)
        {
            GarrisonBase.DebugLuaEvent("LuaEvent: MERCHANT_SHOW");
            if (OnMerchantShow != null) OnMerchantShow();
        }
        public static void MERCHANT_CLOSED(object sender, LuaEventArgs args)
        {
            GarrisonBase.DebugLuaEvent("LuaEvent: MERCHANT_CLOSED");
            if (OnMerchantClosed != null) OnMerchantClosed();
        }

        public static event LuaEventFired OnBagUpdate;
        public static event LuaEventFired OnPlayerBankSlotsChanged;
        public static event LuaEventFired OnPlayerReagentsBankSlotsChanged;
        public static void BAG_UPDATE(object sender, LuaEventArgs args)
        {
            GarrisonBase.DebugLuaEvent("LuaEvent: BAG_UPDATE");
            if (OnBagUpdate != null) OnBagUpdate();
        }
        public static void PLAYERBANKSLOTS_CHANGED(object sender, LuaEventArgs args)
        {
            GarrisonBase.DebugLuaEvent("LuaEvent: PLAYERBANKSLOTS_CHANGED");
            if (OnPlayerBankSlotsChanged != null) OnPlayerBankSlotsChanged();
        }
        public static void PLAYERREAGENTBANKSLOTS_CHANGED(object sender, LuaEventArgs args)
        {
            GarrisonBase.DebugLuaEvent("LuaEvent: PLAYERREAGENTBANKSLOTS_CHANGED");
            if (OnPlayerReagentsBankSlotsChanged != null) OnPlayerReagentsBankSlotsChanged();
        }

        public static event LuaEventFired OnZoneChanged;
        public static void ZONE_CHANGED(object sender, LuaEventArgs args)
        {
            GarrisonBase.DebugLuaEvent("LuaEvent: ZONE_CHANGED");

            if (OnZoneChanged != null) OnZoneChanged();
        }
        public static event LuaEventFired OnZoneChangedNewArea;
        public static void ZONE_CHANGED_NEW_AREA(object sender, LuaEventArgs args)
        {
            GarrisonBase.DebugLuaEvent("LuaEvent: ZONE_CHANGED_NEW_AREA");

            if (OnZoneChangedNewArea != null) OnZoneChangedNewArea();
        }
        //
        public static event LuaEventFired OnCurrentSpellCastChanged;
        public static void CURRENT_SPELL_CAST_CHANGED(object sender, LuaEventArgs args)
        {
            GarrisonBase.DebugLuaEvent("LuaEvent: CURRENT_SPELL_CAST_CHANGED");

            if (OnCurrentSpellCastChanged != null) OnCurrentSpellCastChanged();
        }

        public static event LuaEventFired OnUiErrorMessage;
        public static void UI_ERROR_MESSAGE(object sender, LuaEventArgs args)
        {
            GarrisonBase.DebugLuaEvent("LuaEvent: UI_ERROR_MESSAGE");

            if (OnUiErrorMessage != null) OnUiErrorMessage();
        }

        public static event LuaEventFired OnCurrencyDisplayUpdate;
        public static void CURRENCY_DISPLAY_UPDATE(object sender, LuaEventArgs args)
        {
            GarrisonBase.DebugLuaEvent("LuaEvent: CURRENCY_DISPLAY_UPDATE");

            if (OnCurrencyDisplayUpdate != null) OnCurrencyDisplayUpdate();
        }

        public static event LuaEventFired OnMailShow;
        public static event LuaEventFired OnMailClosed;
        public static void MAIL_SHOW(object sender, LuaEventArgs args)
        {
            GarrisonBase.DebugLuaEvent("LuaEvent: MAIL_SHOW");
            MailOpen = true;
            if (OnMailShow != null) OnMailShow();
        }
        public static void MAIL_CLOSED(object sender, LuaEventArgs args)
        {
            GarrisonBase.DebugLuaEvent("LuaEvent: MAIL_CLOSED");
            MailOpen = false;
            if (OnMailClosed != null) OnMailClosed();
        }

        public static event LuaEventFired OnTaxiMapOpened;
        public static void TAXIMAP_OPENED(object sender, LuaEventArgs args)
        {
            GarrisonBase.DebugLuaEvent("LuaEvent: TAXIMAP_OPENED");
            if (OnTaxiMapOpened != null) OnTaxiMapOpened();
        }
        public static event LuaEventFired OnTaxiMapClosed;
        public static void TAXIMAP_CLOSED(object sender, LuaEventArgs args)
        {
            GarrisonBase.DebugLuaEvent("LuaEvent: TAXIMAP_CLOSED");
            if (OnTaxiMapClosed != null) OnTaxiMapClosed();
        }
        //
        internal static bool LuaEventsAttached = false;
        internal static void AttachLuaEventHandlers()
        {
            GarrisonBase.Log("Attaching LUA Event Handlers");
            Lua.Events.AttachEvent("GARRISON_MISSION_LIST_UPDATE", GARRISON_MISSION_LIST_UPDATE);
            //Lua.Events.AttachEvent("GARRISON_FOLLOWER_LIST_UPDATE", GarrisonFollowerListUpdate);
            Lua.Events.AttachEvent("GARRISON_MISSION_STARTED", GARRISON_MISSION_STARTED);
            Lua.Events.AttachEvent("GARRISON_MISSION_FINISHED", GARRISON_MISSION_FINISHED);

            Lua.Events.AttachEvent("GARRISON_MISSION_NPC_OPENED", GARRISON_MISSION_NPC_OPENED);
            Lua.Events.AttachEvent("GARRISON_MISSION_NPC_CLOSED", GARRISON_MISSION_NPC_CLOSED);
            Lua.Events.AttachEvent("GARRISON_SHOW_LANDING_PAGE", GARRISON_SHOW_LANDING_PAGE);
            Lua.Events.AttachEvent("GARRISON_BUILDING_ACTIVATABLE", GARRISON_BUILDING_ACTIVATABLE);

            Lua.Events.AttachEvent("SHIPMENT_CRAFTER_OPENED", SHIPMENT_CRAFTER_OPENED);
            Lua.Events.AttachEvent("SHIPMENT_CRAFTER_CLOSED", SHIPMENT_CRAFTER_CLOSED);

            Lua.Events.AttachEvent("LOOT_CLOSED", LOOT_CLOSED);
            Lua.Events.AttachEvent("LOOT_OPENED", LOOT_OPENED);


            Lua.Events.AttachEvent("QUEST_COMPLETE", QUEST_COMPLETE);
            Lua.Events.AttachEvent("QUEST_PROGRESS", QUEST_PROGRESS);
            Lua.Events.AttachEvent("QUEST_DETAIL", QUEST_DETAIL);
            Lua.Events.AttachEvent("QUEST_FINISHED", QUEST_FINISHED);

            Lua.Events.AttachEvent("QUEST_LOG_UPDATE", QUEST_LOG_UPDATE);
            Lua.Events.AttachEvent("QUEST_REMOVED", QUEST_REMOVED);
            Lua.Events.AttachEvent("QUEST_ACCEPTED", QUEST_ACCEPTED);
            Lua.Events.AttachEvent("QUEST_WATCH_UPDATE", QUEST_WATCH_UPDATE);
            Lua.Events.AttachEvent("GOSSIP_SHOW", GOSSIP_SHOW);
            Lua.Events.AttachEvent("GOSSIP_CLOSED", GOSSIP_CLOSED);
            Lua.Events.AttachEvent("TRADE_SKILL_CLOSE", TRADE_SKILL_CLOSE);
            Lua.Events.AttachEvent("TRADE_SKILL_SHOW", TRADE_SKILL_SHOW);
            Lua.Events.AttachEvent("BAG_UPDATE", BAG_UPDATE);
            Lua.Events.AttachEvent("PLAYERBANKSLOTS_CHANGED", PLAYERBANKSLOTS_CHANGED);
            Lua.Events.AttachEvent("PLAYERREAGENTBANKSLOTS_CHANGED", PLAYERREAGENTBANKSLOTS_CHANGED);
            Lua.Events.AttachEvent("ZONE_CHANGED", ZONE_CHANGED);
            Lua.Events.AttachEvent("ZONE_CHANGED_NEW_AREA", ZONE_CHANGED_NEW_AREA);

            Lua.Events.AttachEvent("MERCHANT_SHOW", MERCHANT_SHOW);
            Lua.Events.AttachEvent("MERCHANT_CLOSED", MERCHANT_CLOSED);

            Lua.Events.AttachEvent("CURRENT_SPELL_CAST_CHANGED", CURRENT_SPELL_CAST_CHANGED);

            Lua.Events.AttachEvent("UI_ERROR_MESSAGE", UI_ERROR_MESSAGE);

            Lua.Events.AttachEvent("CURRENCY_DISPLAY_UPDATE", CURRENCY_DISPLAY_UPDATE);

            Lua.Events.AttachEvent("MAIL_SHOW", MAIL_SHOW);
            Lua.Events.AttachEvent("MAIL_CLOSED", MAIL_CLOSED);

            Lua.Events.AttachEvent("TAXIMAP_OPENED", TAXIMAP_OPENED);
            Lua.Events.AttachEvent("TAXIMAP_CLOSED", TAXIMAP_CLOSED);

            //
            //GARRISON_BUILDING_ACTIVATABLE
            //CINEMATIC_START
            //CINEMATIC_STOP

            //MAIL_SEND_INFO_UPDATE
            //MAIL_SEND_SUCCESS
            //MAIL_SUCCESS

            LuaEventsAttached = true;

        }
        internal static void DetachLuaEventHandlers()
        {
            GarrisonBase.Log("Detaching LUA Event Handlers");
            //Lua.Events.DetachEvent("GARRISON_FOLLOWER_LIST_UPDATE", Coroutines.GarrisonFollowerListUpdate);
            Lua.Events.DetachEvent("GARRISON_MISSION_LIST_UPDATE", GARRISON_MISSION_LIST_UPDATE);
            Lua.Events.DetachEvent("GARRISON_MISSION_STARTED", GARRISON_MISSION_STARTED);
            Lua.Events.DetachEvent("GARRISON_MISSION_FINISHED", GARRISON_MISSION_FINISHED);

            Lua.Events.DetachEvent("GARRISON_MISSION_NPC_OPENED", GARRISON_MISSION_NPC_OPENED);
            Lua.Events.DetachEvent("GARRISON_MISSION_NPC_CLOSED", GARRISON_MISSION_NPC_CLOSED);
            Lua.Events.DetachEvent("GARRISON_SHOW_LANDING_PAGE", GARRISON_SHOW_LANDING_PAGE);
            Lua.Events.DetachEvent("GARRISON_BUILDING_ACTIVATABLE", GARRISON_BUILDING_ACTIVATABLE);

            Lua.Events.DetachEvent("SHIPMENT_CRAFTER_OPENED", SHIPMENT_CRAFTER_OPENED);
            Lua.Events.DetachEvent("SHIPMENT_CRAFTER_CLOSED", SHIPMENT_CRAFTER_CLOSED);

            Lua.Events.DetachEvent("LOOT_CLOSED", LOOT_CLOSED);
            Lua.Events.DetachEvent("LOOT_OPENED", LOOT_OPENED);

            Lua.Events.DetachEvent("QUEST_COMPLETE", QUEST_COMPLETE);
            Lua.Events.DetachEvent("QUEST_PROGRESS", QUEST_PROGRESS);
            Lua.Events.DetachEvent("QUEST_DETAIL", QUEST_DETAIL);
            Lua.Events.DetachEvent("QUEST_FINISHED", QUEST_FINISHED);
            Lua.Events.DetachEvent("QUEST_LOG_UPDATE", QUEST_LOG_UPDATE);
            Lua.Events.DetachEvent("QUEST_REMOVED", QUEST_REMOVED);
            Lua.Events.DetachEvent("QUEST_ACCEPTED", QUEST_ACCEPTED);
            Lua.Events.DetachEvent("QUEST_WATCH_UPDATE", QUEST_WATCH_UPDATE);

            Lua.Events.DetachEvent("GOSSIP_SHOW", GOSSIP_SHOW);
            Lua.Events.DetachEvent("GOSSIP_CLOSED", GOSSIP_CLOSED);

            Lua.Events.DetachEvent("TRADE_SKILL_CLOSE", TRADE_SKILL_CLOSE);
            Lua.Events.DetachEvent("TRADE_SKILL_SHOW", TRADE_SKILL_SHOW);

            Lua.Events.DetachEvent("BAG_UPDATE", BAG_UPDATE);
            Lua.Events.DetachEvent("PLAYERBANKSLOTS_CHANGED", PLAYERBANKSLOTS_CHANGED);
            Lua.Events.DetachEvent("PLAYERREAGENTBANKSLOTS_CHANGED", PLAYERREAGENTBANKSLOTS_CHANGED);

            Lua.Events.DetachEvent("ZONE_CHANGED", ZONE_CHANGED);
            Lua.Events.DetachEvent("ZONE_CHANGED_NEW_AREA", ZONE_CHANGED_NEW_AREA);

            Lua.Events.DetachEvent("MERCHANT_SHOW", MERCHANT_SHOW);
            Lua.Events.DetachEvent("MERCHANT_CLOSED", MERCHANT_CLOSED);

            Lua.Events.DetachEvent("CURRENT_SPELL_CAST_CHANGED", CURRENT_SPELL_CAST_CHANGED);

            Lua.Events.DetachEvent("UI_ERROR_MESSAGE", UI_ERROR_MESSAGE);

            Lua.Events.DetachEvent("CURRENCY_DISPLAY_UPDATE", CURRENCY_DISPLAY_UPDATE);

            Lua.Events.DetachEvent("MAIL_SHOW", MAIL_SHOW);
            Lua.Events.DetachEvent("MAIL_CLOSED", MAIL_CLOSED);

            Lua.Events.DetachEvent("TAXIMAP_OPENED", TAXIMAP_OPENED);
            Lua.Events.DetachEvent("TAXIMAP_CLOSED", TAXIMAP_CLOSED);


            LuaEventsAttached = false;

        }

        internal static bool LuaAddonInjected = false;
        internal static string TestFunctionString = StringHelper.RandomString;
        internal static string SuccessFunctionString = StringHelper.RandomString;
        internal static string ClickFunctionString = StringHelper.RandomString;

        internal static async Task<bool> InjectLuaAddon()
        {
            

            GarrisonBase.Debug("Injecting Lua Code..");

            String luaCode = String.Format("{0} " +
                                           "function {4}() {1} " +
                                           "function {5}(button_name) {2} " +
                                           "function {6}(mission_id) {3}",
                Resources.LuaStringAddon,
                Resources.LuaStringAddonTest,
                Resources.LuaStringAddonClickButton,
                Resources.LuaStringAddonSuccess,
                TestFunctionString,
                ClickFunctionString,
                SuccessFunctionString);

            GarrisonBase.Debug("TestFunctionString {0}\r\nClickFunctionString {1}\r\nSuccessFunctionString {2}",
                TestFunctionString,ClickFunctionString,SuccessFunctionString);

            await Coroutine.Yield();
            Lua.DoString(luaCode, "clicky.lua");
            await Coroutine.Sleep(1000);
            return false;
        }
    }
}
