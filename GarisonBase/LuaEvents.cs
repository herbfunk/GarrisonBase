using System;
using System.Linq;
using Herbfunk.GarrisonBase.Cache;
using Herbfunk.GarrisonBase.Garrison;
using Herbfunk.GarrisonBase.Properties;
using Herbfunk.GarrisonBase.Quest;
using Herbfunk.GarrisonBase.Quest.Objects;
using Styx.CommonBot.Frames;
using Styx.WoWInternals;

namespace Herbfunk.GarrisonBase
{
    public enum QuestFrameTypes
    {
        None,
        Detail,
        Progress,
        Complete
    }

    public enum QuestLogChangeType
    {
        None,
        Accepted,
        Removed,
        Update
    }

    public static class LuaEvents
    {
        
        public static bool MissionFrameOpen { get; set; }
        public static bool ShipmentOrderFrameOpen { get; set; }
        public static bool MailOpen { get; set; }
        public static bool LootOpened { get; set; }
        public static bool QuestFrameOpen { get; set; }
        public static QuestFrameTypes QuestFrameType { get; set; }
        public static QuestLogChangeType QuestLogChangeType { get; set; }
        public static bool GossipFrameOpen { get; set; }
        public static bool TradeSkillFrameOpen { get; set; }
        public static bool MerchantFrameOpen { get; set; }

        public static void ResetFrameVariables()
        {
            MissionFrameOpen = false;
            ShipmentOrderFrameOpen = false;
            MailOpen = false;
            LootOpened = false;
            QuestFrameOpen = false;
            GossipFrameOpen = false;
            TradeSkillFrameOpen = false;
            MerchantFrameOpen = false;
        }

        public delegate void LuaEventFired();

        public static event LuaEventFired OnGarrisonMissionStarted;
        public static event LuaEventFired OnGarrisonMissionListUpdated;
        public static event LuaEventFired OnGarrisonFollowerListUpdated;
        public static event LuaEventFired OnGarrisonMissionNpcOpened;
        public static event LuaEventFired OnGarrisonMissionNpcClosed;
        public static event LuaEventFired OnGarrisonLandingPageShow;
        public static event LuaEventFired OnGarrisonMissionFinished;
        public static void GARRISON_MISSION_STARTED(object sender, LuaEventArgs args)
        {
            GarrisonBase.DebugLuaEvent(String.Format("LuaEvent: {0}", "Garrison Mission Started!") + "\r\n{0}", args.ToString());
            if (OnGarrisonMissionStarted != null) OnGarrisonMissionStarted();
        }
        public static void GARRISON_MISSION_LIST_UPDATE(object sender, LuaEventArgs args)
        {
            GarrisonBase.DebugLuaEvent(String.Format("LuaEvent: {0}", "Garrison Mission List Updated!") + "\r\n{0}", args.ToString());
            GarrisonManager.UpdateMissionIds();
            if (OnGarrisonMissionListUpdated != null) OnGarrisonMissionListUpdated();
        }
        public static void GARRISON_FOLLOWER_LIST_UPDATE(object sender, LuaEventArgs args)
        {
            GarrisonBase.DebugLuaEvent(String.Format("LuaEvent: {0}", "Garrison Follower List Updated!") + "\r\n{0}", args.ToString());
            if (OnGarrisonFollowerListUpdated != null) OnGarrisonFollowerListUpdated();
        }
        public static void GARRISON_MISSION_NPC_OPENED(object sender, LuaEventArgs args)
        {
            GarrisonBase.DebugLuaEvent(String.Format("LuaEvent: {0}", "Garrison Mission NPC Opened!") + "\r\n{0}", args.ToString());
            MissionFrameOpen = true;
            if (OnGarrisonMissionNpcOpened != null) OnGarrisonMissionNpcOpened();
        }
        public static void GARRISON_MISSION_NPC_CLOSED(object sender, LuaEventArgs args)
        {
            GarrisonBase.DebugLuaEvent(String.Format("LuaEvent: {0}", "Garrison Mission NPC Closed!") + "\r\n{0}", args.ToString());
            MissionFrameOpen = false;
            if (OnGarrisonMissionNpcClosed != null) OnGarrisonMissionNpcClosed();
        }
        public static void GARRISON_SHOW_LANDING_PAGE(object sender, LuaEventArgs args)
        {
            GarrisonBase.DebugLuaEvent(String.Format("LuaEvent: {0}", "Garrison Landing Page Shown!") + "\r\n{0}", args.ToString());
            if (OnGarrisonLandingPageShow != null) OnGarrisonLandingPageShow();
        }
        public static void GARRISON_MISSION_FINISHED(object sender, LuaEventArgs args)
        {
            GarrisonBase.DebugLuaEvent("LuaEvent: GARRISON_MISSION_FINISHED");
            if (OnGarrisonMissionFinished != null) OnGarrisonMissionFinished();
        }

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
            QuestFrameType = QuestFrameTypes.Detail;
            QuestFrameOpen = true;
            if (OnQuestDetail != null) OnQuestDetail();
        }
        public static void QUEST_PROGRESS(object sender, LuaEventArgs args)
        {
            GarrisonBase.DebugLuaEvent("LuaEvent: QUEST_PROGRESS");
            QuestFrameType = QuestFrameTypes.Progress;
            QuestFrameOpen = true;
            if (OnQuestProgress != null) OnQuestProgress();
        }
        public static void QUEST_COMPLETE(object sender, LuaEventArgs args)
        {
            GarrisonBase.DebugLuaEvent("LuaEvent: QUEST_COMPLETE");
            QuestFrameType = QuestFrameTypes.Complete;
            QuestFrameOpen = true;
            if (OnQuestComplete != null) OnQuestComplete();
        }
        public static void QUEST_FINISHED(object sender, LuaEventArgs args)
        {
            GarrisonBase.DebugLuaEvent("LuaEvent: QUEST_FINISHED");
            QuestFrameType= QuestFrameTypes.None;
            QuestFrameOpen = false;
            if (OnQuestFinished != null) OnQuestFinished();
        }
        public static void QUEST_LOG_UPDATE(object sender, LuaEventArgs args)
        {
            GarrisonBase.DebugLuaEvent("LuaEvent: QUEST_LOG_UPDATE");
            QuestManager.RefreshQuestLog();
            if (OnQuestLogUpdate != null) OnQuestLogUpdate();
        }
        public static void QUEST_REMOVED(object sender, LuaEventArgs args)
        {
            GarrisonBase.DebugLuaEvent("LuaEvent: QUEST_REMOVED");
            QuestLogChangeType = QuestLogChangeType.Removed;
            if (OnQuestRemoved != null) OnQuestRemoved();
        }
        public static void QUEST_ACCEPTED(object sender, LuaEventArgs args)
        {
            GarrisonBase.DebugLuaEvent("LuaEvent: QUEST_ACCEPTED");
            QuestLogChangeType = QuestLogChangeType.Accepted;
            if (OnQuestAccepted != null) OnQuestAccepted();
        }
        public static void QUEST_WATCH_UPDATE(object sender, LuaEventArgs args)
        {
            GarrisonBase.DebugLuaEvent("LuaEvent: QUEST_WATCH_UPDATE");
            QuestLogChangeType = QuestLogChangeType.Update;
            if (OnQuestWatchUpdate != null) OnQuestWatchUpdate();
        }


        public static event LuaEventFired OnGossipShow;
        public static event LuaEventFired OnGossipClosed;
        public static void GOSSIP_SHOW(object sender, LuaEventArgs args)
        {
            GarrisonBase.DebugLuaEvent("LuaEvent: GOSSIP_SHOW");
            GossipFrameOpen = true;
            QuestManager.GossipFrameInfo = new GossipFrameInfo(GossipFrame.Instance);
            if (OnGossipShow != null) OnGossipShow();
        }
        public static void GOSSIP_CLOSED(object sender, LuaEventArgs args)
        {
            GarrisonBase.DebugLuaEvent("LuaEvent: GOSSIP_CLOSED");
            GossipFrameOpen = false;
            QuestManager.GossipFrameInfo = new GossipFrameInfo();
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
            MerchantFrameOpen = true;
            if (OnMerchantShow != null) OnMerchantShow();
        }
        public static void MERCHANT_CLOSED(object sender, LuaEventArgs args)
        {
            GarrisonBase.DebugLuaEvent("LuaEvent: MERCHANT_CLOSED");
            MerchantFrameOpen = false;
            if (OnMerchantClosed != null) OnMerchantClosed();
        }

        public static event LuaEventFired OnBagUpdate;
        public static event LuaEventFired OnPlayerBankSlotsChanged;
        public static event LuaEventFired OnPlayerReagentsBankSlotsChanged;
        public static void BAG_UPDATE(object sender, LuaEventArgs args)
        {
            GarrisonBase.DebugLuaEvent("LuaEvent: BAG_UPDATE");
            Player.Inventory.ShouldUpdateBagItems = true;
            if (OnBagUpdate != null) OnBagUpdate();
        }
        public static void PLAYERBANKSLOTS_CHANGED(object sender, LuaEventArgs args)
        {
            GarrisonBase.DebugLuaEvent("LuaEvent: PLAYERBANKSLOTS_CHANGED");
            Player.Inventory.ShouldUpdateBankItems = true;
            if (OnPlayerBankSlotsChanged != null) OnPlayerBankSlotsChanged();
        }
        public static void PLAYERREAGENTBANKSLOTS_CHANGED(object sender, LuaEventArgs args)
        {
            GarrisonBase.DebugLuaEvent("LuaEvent: PLAYERREAGENTBANKSLOTS_CHANGED");
            Player.Inventory.ShouldUpdateBankReagentItems = true;
            if (OnPlayerReagentsBankSlotsChanged != null) OnPlayerReagentsBankSlotsChanged();
        }

        public static event LuaEventFired OnZoneChanged;
        public static void ZONE_CHANGED(object sender, LuaEventArgs args)
        {
            GarrisonBase.DebugLuaEvent("LuaEvent: ZONE_CHANGED");
            Player.ShouldUpdateMinimapZoneText = true;
            if (OnZoneChanged != null) OnZoneChanged();
        }

        public static event LuaEventFired OnCurrentSpellCastChanged;
        public static void CURRENT_SPELL_CAST_CHANGED(object sender, LuaEventArgs args)
        {
            GarrisonBase.DebugLuaEvent("LuaEvent: CURRENT_SPELL_CAST_CHANGED");
            Player.ShouldUpdateCurrentPendingCursorSpellId = true;
            if (OnCurrentSpellCastChanged != null) OnCurrentSpellCastChanged();
        }

        public static event LuaEventFired OnUiErrorMessage;
        public static void UI_ERROR_MESSAGE(object sender, LuaEventArgs args)
        {
            GarrisonBase.DebugLuaEvent("LuaEvent: UI_ERROR_MESSAGE");
            Player.LastErrorMessage.Reset();
            if (OnUiErrorMessage != null) OnUiErrorMessage();
        }

        public static event LuaEventFired OnCurrencyDisplayUpdate;
        public static void CURRENCY_DISPLAY_UPDATE(object sender, LuaEventArgs args)
        {
            GarrisonBase.DebugLuaEvent("LuaEvent: CURRENCY_DISPLAY_UPDATE");
            Player.ShouldUpdateGarrisonResource = true;
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

            Lua.Events.AttachEvent("MERCHANT_SHOW", MERCHANT_SHOW);
            Lua.Events.AttachEvent("MERCHANT_CLOSED", MERCHANT_CLOSED);

            Lua.Events.AttachEvent("CURRENT_SPELL_CAST_CHANGED", CURRENT_SPELL_CAST_CHANGED);

            Lua.Events.AttachEvent("UI_ERROR_MESSAGE", UI_ERROR_MESSAGE);

            Lua.Events.AttachEvent("CURRENCY_DISPLAY_UPDATE", CURRENCY_DISPLAY_UPDATE);

            Lua.Events.AttachEvent("MAIL_SHOW", MAIL_SHOW);
            Lua.Events.AttachEvent("MAIL_CLOSED", MAIL_CLOSED);

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

            Lua.Events.DetachEvent("MERCHANT_SHOW", MERCHANT_SHOW);
            Lua.Events.DetachEvent("MERCHANT_CLOSED", MERCHANT_CLOSED);

            Lua.Events.DetachEvent("CURRENT_SPELL_CAST_CHANGED", CURRENT_SPELL_CAST_CHANGED);

            Lua.Events.DetachEvent("UI_ERROR_MESSAGE", UI_ERROR_MESSAGE);

            Lua.Events.DetachEvent("CURRENCY_DISPLAY_UPDATE", CURRENCY_DISPLAY_UPDATE);

            Lua.Events.DetachEvent("MAIL_SHOW", MAIL_SHOW);
            Lua.Events.DetachEvent("MAIL_CLOSED", MAIL_CLOSED);


            LuaEventsAttached = false;

        }

        internal static bool LuaAddonInjected = false;

        internal static void InjectLuaAddon()
        {
            GarrisonBase.Log("Injecting Lua Code");
            Lua.DoString(Resources.LuaString);
            LuaAddonInjected = true;
        }
    }
}
