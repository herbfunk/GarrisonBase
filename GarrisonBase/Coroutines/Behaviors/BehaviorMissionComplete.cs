using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Herbfunk.GarrisonBase.Cache;
using Herbfunk.GarrisonBase.Cache.Enums;
using Herbfunk.GarrisonBase.Cache.Objects;
using Herbfunk.GarrisonBase.Garrison;
using Herbfunk.GarrisonBase.Garrison.Objects;
using Styx.Common;
using Styx.Common.Helpers;
using Styx.CommonBot.Coroutines;
using Styx.WoWInternals.Garrison;

namespace Herbfunk.GarrisonBase.Coroutines.Behaviors
{
    public class BehaviorMissionComplete : Behavior
    {
        public override BehaviorType Type { get { return BehaviorType.MissionsComplete; } }

        public BehaviorMissionComplete()
            : base()
        {
            if (Character.Player.MinimapZoneText != "Town Hall" || CommandTable == null)
                MovementPoints.Insert(0, MovementCache.GarrisonEntrance);

            Criteria += () => GarrisonManager.CompletedMissionIds.Count > 0;
        }

        public override void Initalize()
        {
            base.Initalize();
        }


        public C_WoWObject CommandTable
        {
            get { return ObjectCacheManager.GetWoWObjects(WoWObjectTypes.GarrisonCommandTable).FirstOrDefault(); }
        }

        public override async Task<bool> BehaviorRoutine()
        {
            if (await base.BehaviorRoutine()) return true;
            if (IsDone) return false;

            if (await StartMovement.MoveTo()) return true;

            if (await Movement())
                return true;

            if (await Interaction())
                return true;

            return false;
        }

        public override async Task<bool> Movement()
        {
            if (CommandTable == null)
            {
                //Error Cannot find object!
                GarrisonBase.Err("Could Not Find Command Table!");
                IsDone = true;
                return false;
            }

            if (CommandTable.Distance < 4.56 && CommandTable.LineOfSight)
            {
                if (LuaUI.MissionFrame.IsOpen)
                {
                    _interactattempts = 0;
                    return false;
                }

                CommandTable.Interact();
                _interactattempts++;
                await CommonCoroutines.SleepForRandomUiInteractionTime();

                if (_interactattempts > 3)
                {
                    GarrisonBase.Err("Attempted over 3 interactions with command table! Starting mvoement..");
                    StartMovement.CurrentMovementQueue.Enqueue(MovementCache.GarrisonEntrance);
                    _interactattempts = 0;
                }

                return true;
            }



            if (_movement == null)
                _movement = new Movement(CommandTable.Location, 4.55f);

            await _movement.MoveTo();
            return true;
        }
        private Movement _movement;
        private int _interactattempts = 0;

        private WaitTimer _commandtableCompletemissionsWaittimer = WaitTimer.FiveSeconds;
        private int _Waitmilliseconds = 5000;
        private Mission _currentMission = null;
        public override async Task<bool> Interaction()
        {
            GarrisonManager.RefreshMissions();
            //GarrisonManager.CompletedMissionIds = LuaCommands.GetCompletedMissionIds();
            if (GarrisonManager.CompletedMissionIds.Count == 0)
            {
                LuaUI.MissionFrame.Close.Click();
                await CommonCoroutines.SleepForRandomUiInteractionTime();
                return false;
            }

            if (!_commandtableCompletemissionsWaittimer.IsFinished)
            {
                await Coroutine.Sleep(_Waitmilliseconds);
            }

            
            //Check if the first dialog is visible..
            if (LuaUI.MissionFrame.IsMissionCompleteDialogVisible)
            {

                LuaUI.MissionFrame.MissionCompleteViewButton.Click();
                //LuaCommands.ClickButton(LuaCommands.ButtonNames.GarrisonMissionFrameMissions_CompleteDialog_BorderFrame_ViewButton);
                _commandtableCompletemissionsWaittimer.Reset();
                return true;
            }
            
            //Check if mission complete dialogs are even visible..
            if (!LuaUI.MissionFrame.IsMissionCompleteBackgroundVisible)
            {
                LuaUI.MissionFrame.Close.Click();
                //LuaCommands.ClickButton(LuaCommands.ButtonNames.GarrisonMissionFrame_CloseButton);
                await CommonCoroutines.SleepForRandomUiInteractionTime();
                return true;
            }

            //Find the current completed mission!
            if (_currentMission == null)
            {
                
                //Click next..
                //if (LuaCommands.IsButtonEnabled(LuaCommands.ButtonNames.GarrisonMissionFrame_MissionComplete_NextMissionButton))
                if(LuaUI.MissionFrame.MissionNextButton.IsEnabled())
                {
                    LuaUI.MissionFrame.MissionNextButton.Click();
                    //LuaCommands.ClickButton(LuaCommands.ButtonNames.GarrisonMissionFrame_MissionComplete_NextMissionButton);
                    _commandtableCompletemissionsWaittimer.Reset();
                    return true;
                }

                foreach (var id in GarrisonManager.CompletedMissions)
                {
                    //Mission newmission = LuaCommands.GetMission(id);

                    if (id.Valid && id.refGarrisonMission.State== MissionState.Complete) //State of zero means its the current one.
                    {
                        _currentMission = id;
                        break;
                    }
                }

                if (_currentMission != null)
                {
                    if (!_currentMission.CanOpenMissionChest())
                        _currentMission.BonusRolled = true;

                    //Add sleep here since a mission can vary before we can click the treasure.
                    if (_currentMission.Followers == 2)
                        await Coroutine.Sleep(10000);
                    else if (_currentMission.Followers == 3)
                        await Coroutine.Sleep(15000);
                    else
                        await Coroutine.Sleep(5000);

                    _commandtableCompletemissionsWaittimer.Reset();
                    Logging.Write("{0}", _currentMission.ToString());
                    return true;
                }
            }

            _commandtableCompletemissionsWaittimer = WaitTimer.OneSecond;
            _Waitmilliseconds = 1000;

            if (_currentMission == null || !_currentMission.Valid)
            {
                //Click next..
                //if (LuaCommands.IsButtonEnabled(LuaCommands.ButtonNames.GarrisonMissionFrame_MissionComplete_NextMissionButton))
                if (LuaUI.MissionFrame.MissionNextButton.IsEnabled())
                {
                    LuaUI.MissionFrame.MissionNextButton.Click();
                    //LuaCommands.ClickButton(LuaCommands.ButtonNames.GarrisonMissionFrame_MissionComplete_NextMissionButton);
                }
                return true;
            }


            if (!_currentMission.MarkedAsCompleted)
            {
                await CommonCoroutines.SleepForRandomUiInteractionTime();
                GarrisonBase.Debug("Current Mission Marking as completed");
                _currentMission.MarkAsCompleted();
            }
            if (!_currentMission.BonusRolled)
            {
                await CommonCoroutines.SleepForRandomUiInteractionTime();
                GarrisonBase.Debug("Current Mission Bonus Rolled");
                _currentMission.BonusRoll();
            }

            await Coroutine.Yield();

           // if (LuaCommands.IsButtonEnabled(LuaCommands.ButtonNames.GarrisonMissionFrame_MissionComplete_NextMissionButton))
            if (LuaUI.MissionFrame.MissionNextButton.IsEnabled())
            {
                
                LuaUI.MissionFrame.MissionNextButton.Click();
                //LuaCommands.ClickButton(LuaCommands.ButtonNames.GarrisonMissionFrame_MissionComplete_NextMissionButton);
                await CommonCoroutines.SleepForRandomUiInteractionTime();
                _commandtableCompletemissionsWaittimer.Reset();
                GarrisonManager.CompletedMissionIds.Remove(_currentMission.Id);
                GarrisonManager.CompletedMissions.Remove(_currentMission);
                _currentMission = null;
                return true;
            }


            return true;
        }


    }
}