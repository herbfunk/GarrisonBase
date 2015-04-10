using System;
using System.Collections.Generic;
using Styx.Common;
using Styx.CommonBot.Frames;
using Styx.Helpers;
using Styx.WoWInternals;

namespace Herbfunk.GarrisonBase.Helpers
{
    public static class GossipHelper
    {
        public static bool IsOpen { get; set; }

        public static CachedValue<int> NumGossipOptions = new CachedValue<int>(GetGossipEntryCount);
        public static CachedValue<int> NumGossipAvailableQuests = new CachedValue<int>(GetGossipAvailableQuestsCount);
        public static CachedValue<int> NumGossipActiveQuests = new CachedValue<int>(GetGossipActiveQuestsCount);

        static GossipHelper()
        {
            IsOpen = GossipFrame.Instance.IsVisible;

            LuaEvents.OnGossipShow += () =>
            {
                IsOpen = true;
  
                _gossipOptions.Clear();
                _activeQuests.Clear();
                _availableQuests.Clear();

                NumGossipActiveQuests.Reset();
                NumGossipAvailableQuests.Reset();
                NumGossipOptions.Reset();
            };

            LuaEvents.OnGossipClosed += () =>
            {
                IsOpen = false;
            };
        }

        public static Dictionary<int, GossipOptionQuestEntry> ActiveQuests
        {
            get
            {
                if (NumGossipActiveQuests == _activeQuests.Count) return _activeQuests;

                _activeQuests.Clear();
                foreach (var q in GossipFrame.Instance.ActiveQuests)
                {
                    _activeQuests.Add(q.Id, new GossipOptionQuestEntry(q));
                }

                return _activeQuests;
            }
            set { _activeQuests = value; }
        }
        private static Dictionary<int, GossipOptionQuestEntry> _activeQuests=new Dictionary<int, GossipOptionQuestEntry>();

        public static Dictionary<int, GossipOptionQuestEntry> AvailableQuests
        {
            get
            {
                if (NumGossipAvailableQuests == _availableQuests.Count) return _availableQuests;

                _availableQuests.Clear();
                foreach (var q in GossipFrame.Instance.AvailableQuests)
                {
                    _availableQuests.Add(q.Id, new GossipOptionQuestEntry(q));
                }

                return _availableQuests;
            }
            set { _availableQuests = value; }
        }
        private static Dictionary<int, GossipOptionQuestEntry> _availableQuests = new Dictionary<int, GossipOptionQuestEntry>();

        public static List<GossipOptionEntry> GossipOptions
        {
            get
            {
                if (NumGossipOptions == _gossipOptions.Count) return _gossipOptions;

                _gossipOptions.Clear();
                foreach (var q in GossipFrame.Instance.GossipOptionEntries)
                {
                    _gossipOptions.Add(new GossipOptionEntry(q));
                }

                return _gossipOptions;
            }
            set { _gossipOptions = value; }
        }
        private static List<GossipOptionEntry> _gossipOptions=new List<GossipOptionEntry>();
        
        
        //
        private static int GetGossipAvailableQuestsCount()
        {
            GarrisonBase.Debug("LuaCommand: GetNumGossipAvailableQuests");
            string lua = String.Format("return GetNumGossipAvailableQuests()");
            List<string> retvalues = Lua.GetReturnValues(lua);
            return retvalues[0].ToInt32();
        }
        private static int GetGossipActiveQuestsCount()
        {
            GarrisonBase.Debug("LuaCommand: GetNumGossipActiveQuests");
            string lua = String.Format("return GetNumGossipActiveQuests()");
            List<string> retvalues = Lua.GetReturnValues(lua);
            return retvalues[0].ToInt32();
        }
        private static int GetGossipEntryCount()
        {
            GarrisonBase.Debug("LuaCommand: GetNumGossipOptions");
            string lua = String.Format("return GetNumGossipOptions()");
            List<string> retvalues = Lua.GetReturnValues(lua);
            return retvalues[0].ToInt32();
        }


        private static void RefreshGossipEntries()
        {
            _gossipOptions.Clear();

            List<string> retvalues = Lua.GetReturnValues("return GetGossipOptions()");
            
            for (int i = 0; i < retvalues.Count; i++)
            {
                var text = retvalues[i];
                var type = retvalues[i + 1];
                _gossipOptions.Add(new GossipOptionEntry(text, type));
                i++;
            }

        }

        private static void RefreshAvailableQuestEntries()
        {

            List<string> retvalues = Lua.GetReturnValues("return GetGossipAvailableQuests()");

            for (int i = 0; i < retvalues.Count; i+=6)
            {
                var name = retvalues[i];
                var level = retvalues[i + 1];
                var unknownBool1 = retvalues[i + 2];
                var maxTimes = retvalues[i + 3];
                var unknownBool2 = retvalues[i + 4];
                var unknownBool3 = retvalues[i + 5];

                Logging.Write("{0} {1} {2} {3} {4} {5}", name, level, unknownBool1, maxTimes, unknownBool2, unknownBool3);
            }

            //SelectGossipAvailableQuest(1)
            //GetQuestID()

        }


        public class GossipOptionEntry
        {
            public string Text { get; set; }
            public int Index { get; set; }
            public GossipEntry.GossipEntryType Type { get; set; }
            public GossipOptionEntry(string text, string type)
            {
                Text = text;
                try
                {
                    Type = (GossipEntry.GossipEntryType)Enum.Parse(typeof(GossipEntry.GossipEntryType), type, true);
                }
                catch (Exception)
                {
                    GarrisonBase.Err("Failed to parse gossip entry type from {0}", type);
                    Type = GossipEntry.GossipEntryType.Unknown;
                }
            }

            public GossipOptionEntry(GossipEntry entry)
            {
                Text = entry.Text;
                Index = entry.Index;
                Type = entry.Type;
            }
        }

        public class GossipOptionQuestEntry
        {
            public int Id { get; set; }
            public int Index { get; set; }
            public string Name { get; set; }

            public GossipOptionQuestEntry(GossipQuestEntry entry)
            {
                Id = entry.Id;
                Index = entry.Index;
                Name = entry.Name;
            }
        }
    }
}
