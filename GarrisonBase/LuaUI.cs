using System;
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

        public class UImissionframe : UIframe
        {
            public UImissionframe() : base("GarrisonMissionFrame") { }
            private readonly UIbutton _closebutton = new UIbutton(ButtonNames.GarrisonMissionFrame_CloseButton);
            public override UIbutton Close
            {
                get { return _closebutton; }
            }

            /// <summary>
            /// Determines if the first dialog for mission complete is visible (the very first one that says # completed)
            /// </summary>
            /// <returns></returns>
            public bool IsMissionCompleteDialogVisible
            {
                get
                {
                    const string lua = "if not GarrisonMissionFrame then return false; " +
                                       "else return tostring(GarrisonMissionFrameMissions.CompleteDialog:IsVisible());end;";

                    var t = Lua.GetReturnValues(lua)[0];
                    var ret = t.ToBoolean();

                    GarrisonBase.Debug("LuaCommand: IsGarrisonMissionCompleteDialogVisible {0}", ret);
                    return ret;  
                }
            }
            /// <summary>
            /// Determines if the any of the mission complete dialogs are visible!
            /// </summary>
            /// <returns></returns>
            public bool IsMissionCompleteBackgroundVisible
            {
                get
                {
                    const string lua =
                           "if not GarrisonMissionFrame then return false; else return tostring(GarrisonMissionFrame.MissionCompleteBackground:IsVisible());end;";
                    var t = Lua.GetReturnValues(lua)[0];
                    var ret = t.ToBoolean();

                    GarrisonBase.Debug("LuaCommand: IsGarrisonMissionCompleteBackgroundVisible {0}", ret);
                    return ret;
                }
            }

            public void OpenMission(int missionId)
            {
                GarrisonBase.Debug("LuaCommand: OpenMission {0}", missionId);
                //Scroll until we see mission first
                var lua =
                    "local mission; local am = {}; C_Garrison.GetAvailableMissions(am);" +
                    String.Format(
                        "for idx = 1, #am do " +
                        "if am[idx].missionID == {0} then " +
                        "mission = am[idx];" +
                        "end;" +
                        "end;" +
                        "GarrisonMissionFrame.MissionTab.MissionList:Hide();" +
                        "GarrisonMissionFrame.MissionTab.MissionPage:Show();" +
                        "GarrisonMissionPage_ShowMission(mission);"
                        , missionId);

                Lua.DoString(lua);
            }
            public void AssignFollowers()
            {
                GarrisonBase.Debug("LuaCommand: AssignFollowers");
                string lua = String.Format("{0}('MissionPage1')", LuaEvents.ClickFunctionString);
                Lua.DoString(lua);
            }

            public readonly UIbutton StartMissionButton = new UIbutton(ButtonNames.GarrisonMissionFrame_MissionTab_MissionPage_StartMissionButton);
            public readonly UIbutton MissionCompleteViewButton = new UIbutton(ButtonNames.GarrisonMissionFrameMissions_CompleteDialog_BorderFrame_ViewButton);
            public readonly UIbutton MissionNextButton = new UIbutton(ButtonNames.GarrisonMissionFrame_MissionComplete_NextMissionButton);
        
        }
        public static UImissionframe MissionFrame = new UImissionframe();
    }
}
