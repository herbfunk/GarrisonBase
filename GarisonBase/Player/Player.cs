using System;
using System.Collections.Generic;
using Styx;
using Styx.Helpers;

namespace Herbfunk.GarrisonBase.Cache
{
    public static class Player
    {
        public static PlayerInventory Inventory;
        public static PlayerProfessions Professions;

        internal static uint MapID = 0;
        internal static bool IsAlliance = false;
        internal static int Level = 0;
        internal static int GarrisonResource = 0;

        internal static int AvailableGarrisonResource
        {
            get
            {
                var available = GarrisonResource - BaseSettings.CurrentSettings.ReservedGarrisonResources;
                return available < 0 ? 0 : available;
            }
        }
        internal static List<int> AuraSpellIds = new List<int>();
        internal static int CurrentPendingCursorSpellId = -1;

        internal static void Initalize()
        {
            MapID = StyxWoW.Me.CurrentMap.MapId;
            IsAlliance = StyxWoW.Me.IsAlliance;
            Level = StyxWoW.Me.Level;
            Inventory = new PlayerInventory();
            Professions = new PlayerProfessions();

            Update();
        }
        internal static void Update()
        {
            using (StyxWoW.Memory.AcquireFrame())
            {
                RefreshAuraIds();
                if (ShouldUpdateMinimapZoneText) _updateMinimapZoneText();
                if (ShouldUpdateCurrentPendingCursorSpellId) _updateCurrentPendingCursorSpellId();
                if (ShouldUpdateGarrisonResource) _updateGarrisonResource();
                Inventory.Update();
            }
            
        }

        internal static string MinimapZoneText = "";
        internal static bool ShouldUpdateMinimapZoneText = true;
        private static void _updateMinimapZoneText()
        {
            MinimapZoneText = StyxWoW.Me.MinimapZoneText;
            ShouldUpdateMinimapZoneText = false;
        }

        internal static bool ShouldUpdateGarrisonResource = true;
        private static void _updateGarrisonResource()
        {
            ShouldUpdateGarrisonResource = false;
            GarrisonResource = LuaCommands.GetCurrencyCount(824);
        }
        internal static void RefreshAuraIds()
        {
            AuraSpellIds.Clear();
            using (StyxWoW.Memory.AcquireFrame())
            {
                foreach (var item in StyxWoW.Me.GetAllAuras())
                {
                    AuraSpellIds.Add(item.SpellId);
                }
            }
        }

        internal static bool ShouldUpdateCurrentPendingCursorSpellId = true;
        private static void _updateCurrentPendingCursorSpellId()
        {
            ShouldUpdateCurrentPendingCursorSpellId = false;
            if (StyxWoW.Me.CurrentPendingCursorSpell == null)
            {
                CurrentPendingCursorSpellId = -1;
                return;
            }
            CurrentPendingCursorSpellId = StyxWoW.Me.CurrentPendingCursorSpell.Id;
        }

        internal static CachedValue<string> LastErrorMessage = new CachedValue<string>(GetLastErrorMessage);
        private static string GetLastErrorMessage()
        {
            string s = "";
            using (StyxWoW.Memory.AcquireFrame())
            {
                s = StyxWoW.LastRedErrorMessage;
            }
            return s;
        }
    }
}
