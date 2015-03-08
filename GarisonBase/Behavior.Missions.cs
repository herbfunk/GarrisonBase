using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Herbfunk.GarrisonBase.Cache;
using Herbfunk.GarrisonBase.Garrison;
using Herbfunk.GarrisonBase.Garrison.Objects;
using Styx;
using Styx.Common;
using Styx.Common.Helpers;
using Styx.CommonBot;
using Styx.CommonBot.Coroutines;
using Styx.TreeSharp;
using Styx.WoWInternals;

namespace Herbfunk.GarrisonBase
{
    public partial class Behaviors
    {
        public class BehaviorMissions : Behavior
        {
            public override BehaviorType Type { get { return BehaviorType.MissionsComplete; } }

            public BehaviorMissions()
                : base()
            {

            }
            public override void Initalize()
            {
                if (Player.MinimapZoneText != "Town Hall" || CommandTable == null)
                    MovementPoints.Insert(0, MovementCache.GarrisonEntrance);

                base.Initalize();
            }
            public override Func<bool> Criteria
            {
                get
                {
                    return () => GarrisonManager.CompletedMissionIds.Count > 0;
                }
            }
            public C_WoWObject CommandTable
            {
                get { return ObjectCacheManager.GetWoWObjects(WoWObjectTypes.GarrisonCommandTable).FirstOrDefault(); }
            }

            public override async Task<bool> Movement()
            {

                TreeRoot.StatusText = String.Format("Behavior {0} Movement", Type.ToString());
                if (await base.Movement())
                    return true;


                TreeRoot.StatusText = String.Format("Behavior {0} Movement2", Type.ToString());


                if (CommandTable == null)
                {
                    //Error Cannot find object!
                    GarrisonBase.Err("Could Not Find Command Table!");
                    IsDone = true;
                    return false;
                }

                if (CommandTable.CentreDistance < 4.56 && CommandTable.InLineOfSight)
                {
                    if (LuaEvents.MissionFrameOpen)
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
                TreeRoot.StatusText = String.Format("Behavior {0} Interaction", Type.ToString());

                GarrisonManager.CompletedMissionIds = LuaCommands.GetCompletedMissionIds();
                if (GarrisonManager.CompletedMissionIds.Count == 0)
                {
                    LuaCommands.CloseGarrisonMissionFrame();
                    await CommonCoroutines.SleepForRandomUiInteractionTime();
                    return false;
                }

                if (!_commandtableCompletemissionsWaittimer.IsFinished)
                {
                    await Coroutine.Sleep(_Waitmilliseconds);
                }

                //Check if the first dialog is visible..
                if (LuaCommands.IsGarrisonMissionCompleteDialogVisible())
                {
                    //LuaCommands.ClickMissionCompleteDialogNextButton();
                    LuaCommands.ClickButton(LuaCommands.ButtonNames.GarrisonMissionFrameMissions_CompleteDialog_BorderFrame_ViewButton);
                    _commandtableCompletemissionsWaittimer.Reset();
                    return true;
                }

                //Check if mission complete dialogs are even visible..
                if (!LuaCommands.IsGarrisonMissionCompleteBackgroundVisible())
                {
                    LuaCommands.ClickButton(LuaCommands.ButtonNames.GarrisonMissionFrame_CloseButton);
                    //LuaCommands.CloseGarrisonMissionFrame();
                    await CommonCoroutines.SleepForRandomUiInteractionTime();
                    return true;
                }

                //Find the current completed mission!
                if (_currentMission == null)
                {
                    //LuaCommands.MissionCompleteNextButtonEnabled()
                    //Click next..
                    if (LuaCommands.IsButtonEnabled(LuaCommands.ButtonNames.GarrisonMissionFrame_MissionComplete_NextMissionButton))
                    {
                        LuaCommands.ClickButton(LuaCommands.ButtonNames.GarrisonMissionFrame_MissionComplete_NextMissionButton);
                        //LuaCommands.ClickMissionCompleteNextButton();
                        _commandtableCompletemissionsWaittimer.Reset();
                        return true;
                    }

                    foreach (var id in GarrisonManager.CompletedMissionIds)
                    {
                        Mission newmission = LuaCommands.GetMission(id);
                        if (newmission.State == 0) //State of zero means its the current one.
                        {
                            _currentMission = newmission;
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

                if (_currentMission == null)
                {
                    //
                    //LuaCommands.MissionCompleteNextButtonEnabled()
                    //Click next..
                    if (LuaCommands.IsButtonEnabled(LuaCommands.ButtonNames.GarrisonMissionFrame_MissionComplete_NextMissionButton))
                    {
                     //   LuaCommands.ClickMissionCompleteNextButton();
                        LuaCommands.ClickButton(LuaCommands.ButtonNames.GarrisonMissionFrame_MissionComplete_NextMissionButton);
                    }
                    return false;
                }
                _currentMission.Update();



                if (!_currentMission.MarkedAsCompleted)
                {
                    await CommonCoroutines.SleepForRandomUiInteractionTime();
                    Logging.Write("Current Mission Marking as completed");
                    _currentMission.MarkAsCompleted();
                }
                if (!_currentMission.BonusRolled)
                {
                    await CommonCoroutines.SleepForRandomUiInteractionTime();
                    Logging.Write("Current Mission Bonus Rolled");
                    _currentMission.BonusRoll();
                }

                await Coroutine.Yield();

                if (LuaCommands.IsButtonEnabled(LuaCommands.ButtonNames.GarrisonMissionFrame_MissionComplete_NextMissionButton))
                {
                    await CommonCoroutines.SleepForRandomUiInteractionTime();
                    //LuaCommands.ClickMissionCompleteNextButton();
                    LuaCommands.ClickButton(LuaCommands.ButtonNames.GarrisonMissionFrame_MissionComplete_NextMissionButton);

                    _commandtableCompletemissionsWaittimer.Reset();
                    GarrisonManager.CompletedMissionIds.Remove(_currentMission.Id);
                    _currentMission = null;
                    return true;
                }


                return true;
            }

            public override async Task<bool> BehaviorRoutine()
            {
                if (IsDone) return false;

                if (await base.BehaviorRoutine()) return true;

                if (await Movement())
                    return true;

                if (await Interaction())
                    return true;

                return false;
            }
        }


        public class BehaviorNewMissions : Behavior
        {
            public override BehaviorType Type { get { return BehaviorType.MissionsStart; } }

            public BehaviorNewMissions()
                : base()
            {
                //MovementCache.GarrisonEntrance, GarrisonManager.CommandTableEntryId
            }
            public override void Initalize()
            {
                if (Player.MinimapZoneText != "Town Hall" || CommandTable == null)
                    MovementPoints.Insert(0, MovementCache.GarrisonEntrance);

                base.Initalize();
            }
            public override Func<bool> Criteria
            {
                get
                {
                    return () => BaseSettings.CurrentSettings.BehaviorMissionStart && GarrisonManager.AvailableMissionIds.Count > 0;
                }
            }
            public C_WoWObject CommandTable
            {
                get { return ObjectCacheManager.GetWoWObjects(WoWObjectTypes.GarrisonCommandTable).FirstOrDefault(); }
            }
            public override async Task<bool> Movement()
            {
                if (LuaEvents.MissionFrameOpen)
                    return false;


                TreeRoot.StatusText = String.Format("Behavior {0} Movement", Type.ToString());
                if (await base.Movement())
                    return true;



                TreeRoot.StatusText = String.Format("Behavior {0} Movement2", Type.ToString());
                if (CommandTable == null)
                {
                    //Error Cannot find object!
                    return false;
                }
                if (CommandTable.CentreDistance < 4.56 && CommandTable.InLineOfSight)
                {
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

            private readonly WaitTimer _commandtableCompletemissionsWaittimer = WaitTimer.FiveSeconds;
            private Mission _currentMission = null;
            private int _interactattempts = 0;

            public override async Task<bool> Interaction()
            {
                TreeRoot.StatusText = String.Format("Behavior {0} Interaction", Type.ToString());

                if (!_commandtableCompletemissionsWaittimer.IsFinished) return true;
                _commandtableCompletemissionsWaittimer.Reset();

                if (GarrisonManager.AvailableMissionIds.Count == 0)
                {
                    await Coroutine.Yield();
                    return false;
                }

                if (LuaCommands.IsGarrisonMissionCompleteDialogVisible())
                {
                    //Complete Dialog is visible..
                    //We need to add complete missions to coroutines
                    //Add it again..
                    //Finish this behavior
                    GarrisonBase.Log("Garrison Mission Complete Is Visible!");
                    Coroutines.Behaviors.Insert(1, new BehaviorMissions());
                    Coroutines.Behaviors.Insert(2, new BehaviorNewMissions());
                    LuaCommands.CloseGarrisonMissionFrame();
                    await CommonCoroutines.SleepForRandomUiInteractionTime();
                    await Coroutine.Yield();
                    return false;
                }

                //Create the actual mission cache objects..
                if (GarrisonManager.AvailableMissions.Count != GarrisonManager.AvailableMissionIds.Count)
                {
                    GarrisonManager.AvailableMissions.Clear();

                    foreach (var m in GarrisonManager.AvailableMissionIds)
                    {
                        Mission mission = LuaCommands.GetMissionInfo(m);
                        GarrisonManager.AvailableMissions.Add(mission);
                    }
                    //Cache.AvailableMissions = Cache.AvailableMissions.OrderByDescending(m => m.ItemLevel).ThenByDescending(m => m.Level).ThenBy(m => m.Duration).ThenByDescending(m => m.Name).ToList();
                    GarrisonManager.AvailableMissions = GarrisonManager.AvailableMissions.OrderByDescending(m => m.Priority).ToList();

                    //foreach (var m in GarrisonManager.AvailableMissions)
                    //{
                    //    GarrisonBase.Log("Mission: {0}", m.ToString());
                    //}
                }

                //Find our next mission.. if any.
                double _successChance = 0;
                if (_currentMission == null)
                {
                    int[] followerIds;
                    List<int> removalList = new List<int>();
                    for (int i = 0; i < GarrisonManager.AvailableMissions.Count; i++)
                    {
                        var mission = GarrisonManager.AvailableMissions[i];
                        if (mission.Priority>0 && mission.Cost <= Player.AvailableGarrisonResource)
                        {
                            _successChance = LuaCommands.GetMissionBestSuccessAttempt(mission.Id, out followerIds);
                            if (_successChance < mission.SuccessRate)
                            {
                                removalList.Add(i);
                                continue;
                            }
                            _currentMission = mission;
                            break;
                        }

                        removalList.Add(i);
                    }

                    if (removalList.Count > 0)
                    {
                        foreach (var k in removalList.OrderByDescending(k => k))
                        {
                            int id = GarrisonManager.AvailableMissions[k].Id;
                            GarrisonManager.AvailableMissionIds.Remove(id);
                            GarrisonManager.AvailableMissions.RemoveAt(k);
                        }
                    }
                }

                if (_currentMission != null)
                {
                    TreeRoot.StatusText = "[GarrisonBase] Starting new mission " + _currentMission.Name;
                    GarrisonBase.Log("Starting Mission {0} Rewards {1} Priority {2} Success {3}",
                        _currentMission.Name, _currentMission.RewardTypes, _currentMission.Priority, _successChance);



                    //GARRISON_FOLLOWER_LIST_UPDATE
                    await
                        CommonCoroutines.WaitForLuaEvent("GARRISON_FOLLOWER_LIST_UPDATE", 7500, null, Action);

                    await Coroutine.Sleep(2218);

                    await
                        CommonCoroutines.WaitForLuaEvent("GARRISON_MISSION_STARTED", 7500, null, LuaCommands.ClickMissionStageStartButton);

                    await Coroutine.Sleep(2500);
                    //LuaCommands.CloseMission();
                    //await Coroutine.Sleep(1500);
                    GarrisonManager.AvailableMissionIds.Remove(_currentMission.Id);
                    GarrisonManager.AvailableMissions.Remove(_currentMission);
                    _currentMission = null;
                    await Coroutine.Yield();
                    return true;
                }

                _currentMission = null;
                GarrisonManager.AvailableMissions.Clear();
                GarrisonManager.AvailableMissionIds.Clear();
                return false;
            }

            private void Action()
            {
                LuaCommands.OpenMission(_currentMission.Id);
                LuaCommands.AssignFollowers();
            }

            public override async Task<bool> BehaviorRoutine()
            {
                if (IsDone) return false;

                if (await base.BehaviorRoutine()) return true;

                if (await Movement())
                    return true;

                if (await Interaction())
                    return true;

                return false;
            }
        }

    }
}
