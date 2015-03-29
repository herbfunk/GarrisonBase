using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Windows.Media;
using Bots.Professionbuddy.Dynamic;
using CommonBehaviors.Actions;
using Herbfunk.GarrisonBase.Cache;
using Herbfunk.GarrisonBase.Coroutines;
using Herbfunk.GarrisonBase.Garrison;
using Styx.Common;
using Styx.CommonBot;
using Styx.Helpers;
using Styx.TreeSharp;

namespace Herbfunk.GarrisonBase
{
    public class GarrisonBase : BotBase
    {
        public static HBRelogApi HbRelogApi;
        internal static readonly Version Version = new Version(1,2,0,0);
        public static GarrisonBase Instance { get; private set; }
        public GarrisonBase()
        {
            Instance = this;
        }

        public override string Name
        {
            get { return "GarrisonBase"; }
        }

        public override Composite Root
        {
             get { return _root ?? (_root = new ActionRunCoroutine(ctx => BehaviorManager.RootLogic())); }
        }
        private Composite _root;

        public override PulseFlags PulseFlags
        {
            get { return PulseFlags.All; }
        }

        public override void Start()
        {
            Debug("BotEvent OnStart");
            HbRelogApi = new HBRelogApi();
            CacheStaticLookUp.Reset();

            if (!LuaEvents.LuaEventsAttached)
                LuaEvents.AttachLuaEventHandlers();
        }

        public override void Stop()
        {
            Debug("BotEvent OnStop");

            

            if (LuaEvents.LuaEventsAttached)
                LuaEvents.DetachLuaEventHandlers();

            BehaviorManager.Reset();
  
            CacheStaticLookUp.InitalizedCache = false;
            ObjectCacheManager.ResetCache();
            GarrisonManager.Initalized = false;
            LuaEvents.ResetFrameVariables();

            TreeRoot.Stop();

            if (BaseSettings.CurrentSettings.HBRelog_SkipToNextTask)
            {
                if (HbRelogApi.IsConnected)
                {
                    HbRelogApi.SkipCurrentTask(HbRelogApi.CurrentProfileName);
                }
            }
        }

        public override void Initialize()
        {
            Debug("BotEvent Initialize");
            BaseSettings.LoadSettings();
        }

        public override void OnSelected()
        {
            Debug("BotEvent OnSelected");
            Log("Selected GarrisonBase v{0}", Version.ToString());
            BaseSettings.LoadSettings();
            Character.Player.Initalize();
        }

        public override void OnDeselected()
        {
            Debug("BotEvent OnDeselected");
            if (LuaEvents.LuaEventsAttached)
                LuaEvents.DetachLuaEventHandlers();
        }
        
        public override Form ConfigurationForm
        {
            get { return new Config.Config(); }
        }


        internal static string HBPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        internal static string GarrisonBasePath
        {
            get
            {
                return Path.Combine(HBPath, GlobalSettings.Instance.BotsPath, "GarrisonBase");
            }
            
        }


        internal static void Log(string format, params object[] args)
        {
            Logging.Write(Colors.YellowGreen, String.Format("GarrisonBase: {0}", format), args);
        }

        internal static void Err(string format, params object[] args)
        {
            Logging.Write(Colors.Red, String.Format("GarrisonBase: {0}", format), args);
        }

        internal static void Debug(string format, params object[] args)
        {
            Logging.WriteDiagnostic(Colors.DodgerBlue, String.Format("GarrisonBase: {0}", format), args);
        }
        internal static void DebugLuaEvent(string format, params object[] args)
        {
            Logging.WriteDiagnostic(Colors.Salmon, String.Format("{0}", format), args);
        }
        public static bool TextIsAllNumerical(string text)
        {
            foreach (var c in text.ToCharArray())
            {
                if (!Char.IsNumber(c)) return false;
            }

            return true;
        }

        private static readonly Random Rand = new Random();
        public static string RandomString
        {
            get
            {
                int size = Rand.Next(6, 15);
                var sb = new StringBuilder(size);
                for (int i = 0; i < size; i++)
                {
                    // random upper/lowercase character using ascii code
                    sb.Append((char)(Rand.Next(2) == 1 ? Rand.Next(65, 91) + 32 : Rand.Next(65, 91)));
                }
                return sb.ToString();
            }
        }
     
    }
}
