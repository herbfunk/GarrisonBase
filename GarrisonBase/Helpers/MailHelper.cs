using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Styx.CommonBot.Frames;

namespace Herbfunk.GarrisonBase.Helpers
{
    public static class MailHelper
    {
        public static bool IsOpen { get; set; }

        static MailHelper()
        {
            IsOpen = MailFrame.Instance.IsVisible;
            LuaEvents.OnMailShow += () => IsOpen = true;
            LuaEvents.OnMailClosed += () => IsOpen = false;
        }
    }
}
