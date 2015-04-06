using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Styx.Helpers;
using Styx.WoWInternals;

namespace Herbfunk.GarrisonBase
{

    public static class LuaUI
    {
        public enum ButtonNames
        {
            GarrisonCapacitiveDisplayFrame_CreateAllWorkOrdersButton,
            GarrisonLandingPageMinimapButton,
            TradeSkillFrameCloseButton,
            GarrisonCapacitiveDisplayFrame_StartWorkOrderButton,
            GarrisonCapacitiveDisplayFrameCloseButton,
            GarrisonMissionFrame_CloseButton,
            GarrisonMissionFrame_MissionTab_MissionPage_StartMissionButton,
            GarrisonMissionFrameMissions_CompleteDialog_BorderFrame_ViewButton,
            GarrisonMissionFrame_MissionComplete_NextMissionButton,
            InboxNextPageButton,
            InboxPrevPageButton,
            OpenMailFrameCloseButton,
        }

        public static UIbutton GarrisonMinimap = new UIbutton(ButtonNames.GarrisonLandingPageMinimapButton);
        public static UIbutton TradeSkillClose = new UIbutton(ButtonNames.TradeSkillFrameCloseButton);
        public static UIbutton MissionClose = new UIbutton(ButtonNames.GarrisonMissionFrame_CloseButton);
        public static UIbutton MissionStart = new UIbutton(ButtonNames.GarrisonMissionFrame_MissionTab_MissionPage_StartMissionButton);
        public static UIbutton MissionCompleteView = new UIbutton(ButtonNames.GarrisonMissionFrameMissions_CompleteDialog_BorderFrame_ViewButton);
        public static UIbutton MissionNext = new UIbutton(ButtonNames.GarrisonMissionFrame_MissionComplete_NextMissionButton);
        public static UIbutton InboxNextPage = new UIbutton(ButtonNames.InboxNextPageButton);
        public static UIbutton InboxPreviousPage = new UIbutton(ButtonNames.InboxPrevPageButton);
        public static UIbutton InboxClose = new UIbutton(ButtonNames.OpenMailFrameCloseButton);

        public class UIbutton
        {
            public readonly string Name;

            public UIbutton(string name)
            {
                Name = name;
            }
            public UIbutton(ButtonNames buttonName)
            {
                Name = buttonName.ToString().Replace("_", ".");
            }

            public void Click()
            {
                GarrisonBase.Debug("LuaCommand: ClickButton {0}", Name);
                String lua = String.Format("{0}:Click()", Name);
                Lua.DoString(lua);
            }
            public bool IsEnabled()
            {
                String lua = String.Format("return tostring({0}:IsEnabled())", Name);
                var ret = Lua.GetReturnValues(lua)[0].ToBoolean();
                GarrisonBase.Debug("LuaCommand: IsButtonEnabled {0} {1}", Name, ret);
                return ret;
            }
        }

        public abstract class UIframe
        {
            public string Name { get; set; }
            public virtual UIbutton Close
            {
                get { return null; }
            }

            public UIframe(string name)
            {
                Name = name;
            }

            public bool IsVisible()
            {
                String lua = String.Format("return tostring({0}:IsVisible())", Name);
                string t = Lua.GetReturnValues(lua)[0];
                bool ret = t.ToBoolean();
                GarrisonBase.Debug("LuaCommand: IsFrameVisible {0} {1}", Name, ret);
                return ret;
            }
        }

        public class UIworkorder : UIframe
        {
            public UIworkorder() : base("GarrisonCapacitiveDisplayFrame") { }

            private readonly UIbutton _closebutton = new UIbutton(ButtonNames.GarrisonCapacitiveDisplayFrameCloseButton);
            public override UIbutton Close
            {
                get { return _closebutton; }
            }

            public UIbutton CreateAllWorkOrder = new UIbutton(ButtonNames.GarrisonCapacitiveDisplayFrame_CreateAllWorkOrdersButton);
            public UIbutton StartWorkOrder = new UIbutton(ButtonNames.GarrisonCapacitiveDisplayFrame_StartWorkOrderButton);
        }
        public static UIworkorder WorkOrder = new UIworkorder();

    }
}
