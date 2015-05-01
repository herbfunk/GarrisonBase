using System.Collections.Generic;
using Herbfunk.GarrisonBase.Garrison;
using Styx;
using Styx.Helpers;
using Styx.WoWInternals;

namespace Herbfunk.GarrisonBase.Character
{
    public static class Player
    {
        public static PlayerInventory Inventory;
        public static PlayerProfessions Professions;

        
        internal static bool IsAlliance = false;
        internal static int Level = 0;
        internal static WoWClass Class= WoWClass.None;
        internal static bool Combat = false;
        internal static bool ActuallyInCombat = false;
        internal static WoWPoint Location = WoWPoint.Zero;
        internal static float Rotation = 0f;
        internal static WoWPoint TraceLinePosition = WoWPoint.Zero;
        internal static WoWGuid PlayerGuid = WoWGuid.Empty;
        internal static WoWGuid CurrentTargetGuid = WoWGuid.Empty;

        internal static CachedValue<uint> MapId;
        internal static CachedValue<int> ParentMapId;
        internal static CachedValue<uint> MapExpansionId;
        internal static CachedValue<bool> MapIsContinent;
        internal static CachedValue<uint> ZoneId;

        internal static bool InsideGarrison
        {
            get
            {
                var id = ZoneId.Value;
                return GarrisonManager.GarrisonZoneIds.Contains(id) || GarrisonManager.GarrisonMineZoneIds.Contains(id);
            }
        }

        internal static CachedValue<int> GarrisonResource;
        internal static CachedValue<string> MinimapZoneText;
        internal static CachedValue<int> CurrentPendingCursorSpellId;
        internal static CachedValue<string> LastErrorMessage;

        internal static int AvailableGarrisonResource
        {
            get
            {
                var available = GarrisonResource - BaseSettings.CurrentSettings.ReservedGarrisonResources;
                return available < 0 ? 0 : available;
            }
        }
        internal static List<int> AuraSpellIds = new List<int>();

        internal static void Initalize()
        {
            PlayerGuid = StyxWoW.Me.Guid;
            IsAlliance = StyxWoW.Me.IsAlliance;
            Level = StyxWoW.Me.Level;
            Location = StyxWoW.Me.Location;
            Rotation = StyxWoW.Me.Rotation;
            Class = StyxWoW.Me.Class;
            CurrentTargetGuid = StyxWoW.Me.CurrentTargetGuid;
            Combat = StyxWoW.Me.Combat;
            ActuallyInCombat = StyxWoW.Me.IsActuallyInCombat;
            TraceLinePosition = StyxWoW.Me.GetTraceLinePos();
            Inventory = new PlayerInventory();
            Professions = new PlayerProfessions();

            MinimapZoneText = new CachedValue<string>(() => StyxWoW.Me.MinimapZoneText);
            MapId = new CachedValue<uint>(() => StyxWoW.Me.CurrentMap.MapId);
            ParentMapId = new CachedValue<int>(() => StyxWoW.Me.CurrentMap.ParentMapId);
            MapExpansionId = new CachedValue<uint>(() => StyxWoW.Me.CurrentMap.ExpansionId);
            MapIsContinent = new CachedValue<bool>(() => StyxWoW.Me.CurrentMap.IsContinent);
            ZoneId = new CachedValue<uint>(() => StyxWoW.Me.ZoneId);
            // 
            GarrisonResource = new CachedValue<int>(() => LuaCommands.GetCurrencyCount(824));
            CurrentPendingCursorSpellId = new CachedValue<int>(_updateCurrentPendingCursorSpellId);
            LastErrorMessage = new CachedValue<string>(GetLastErrorMessage);

            LuaEvents.OnZoneChangedNewArea += () =>
            {
                MapId.Reset();
                ParentMapId.Reset();
                MapExpansionId.Reset();
                MapIsContinent.Reset();
                MinimapZoneText.Reset();
                ZoneId.Reset();
            };

            LuaEvents.OnZoneChanged += () => MinimapZoneText.Reset();
            LuaEvents.OnCurrencyDisplayUpdate += () => GarrisonResource.Reset();
            LuaEvents.OnCurrentSpellCastChanged += () => CurrentPendingCursorSpellId.Reset();
            LuaEvents.OnUiErrorMessage += () => LastErrorMessage.Reset();

            Update();
        }

        internal static void Update()
        {
            using (StyxWoW.Memory.AcquireFrame())
            {
                Location = StyxWoW.Me.Location;
                Rotation = StyxWoW.Me.Rotation;
                TraceLinePosition = StyxWoW.Me.GetTraceLinePos();
                Combat = StyxWoW.Me.Combat;
                ActuallyInCombat = StyxWoW.Me.IsActuallyInCombat;
                CurrentTargetGuid = StyxWoW.Me.CurrentTargetGuid;
                RefreshAuraIds();
                Inventory.Update();
            }
            
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

        private static int _updateCurrentPendingCursorSpellId()
        {
            if (StyxWoW.Me.CurrentPendingCursorSpell == null)
            {
                return -1;
            }
            return StyxWoW.Me.CurrentPendingCursorSpell.Id;
        }

        
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
