using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Herbfunk.GarrisonBase.Cache;
using Herbfunk.GarrisonBase.Cache.Enums;
using Herbfunk.GarrisonBase.Cache.Objects;
using Herbfunk.GarrisonBase.Garrison;
using Herbfunk.GarrisonBase.Garrison.Objects;
using Styx.Common.Helpers;
using Styx.CommonBot.Coroutines;

namespace Herbfunk.GarrisonBase.Coroutines.Behaviors
{
    public class BehaviorMissionStartup : Behavior
    {
        public override BehaviorType Type { get { return BehaviorType.MissionsStart; } }

        public BehaviorMissionStartup()
            : base()
        {
            if (Character.Player.MinimapZoneText != "Town Hall" || CommandTable == null)
                MovementPoints.Insert(0, MovementCache.GarrisonEntrance);

            Criteria += () => 
                BaseSettings.CurrentSettings.BehaviorMissionStart && 
                GarrisonManager.AvailableMissionIds.Count > 0 &&
                GarrisonManager.CurrentActiveFollowers <= GarrisonManager.MaxActiveFollowers;
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

            if (await Movement()) return true;

            if (await Interaction())  return true;

            return false;
        }


        private async Task<bool> Movement()
        {
            if (LuaUI.MissionFrame.IsOpen)
                return false;

            if (CommandTable == null)
            {
                //Error Cannot find object!
                return false;
            }

            if (CommandTable.Distance < 4.56 && CommandTable.LineOfSight)
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
                _movement = new Movement(CommandTable.Location, 4.55f, name: "CommandTable");

            await _movement.MoveTo(false);
            return true;
        }
        private Movement _movement;

        private readonly WaitTimer _commandtableCompletemissionsWaittimer = WaitTimer.FiveSeconds;
        private Mission _currentMission = null;
        private int _interactattempts = 0;

        private async Task<bool> Interaction()
        {
            if (!_commandtableCompletemissionsWaittimer.IsFinished) return true;
            _commandtableCompletemissionsWaittimer.Reset();

            if (GarrisonManager.AvailableMissionIds.Count == 0)
            {
                await Coroutine.Yield();
                return false;
            }

            if (LuaUI.MissionFrame.IsMissionCompleteDialogVisible)
            {
                //Complete Dialog is visible..
                //We need to add complete missions to coroutines
                //Add it again..
                //Finish this behavior
                GarrisonBase.Log("Garrison Mission Complete Is Visible!");
                BehaviorManager.Behaviors.Insert(1, new BehaviorMissionComplete());
                BehaviorManager.Behaviors.Insert(2, new BehaviorMissionStartup());
                LuaUI.MissionFrame.Close.Click();
                await CommonCoroutines.SleepForRandomUiInteractionTime();
                await Coroutine.Yield();
                return false;
            }

            //Find our next mission.. if any.
            double _successChance = 0;
            if (_currentMission == null)
            {
                int[] followerIds;
                var removalList = new List<int>();
                for (int i = 0; i < GarrisonManager.AvailableMissions.Count; i++)
                {
                    var mission = GarrisonManager.AvailableMissions[i];

                    if (mission.Level >= mission.MinimumLevel && 
                        mission.Priority > 0 && 
                        mission.Cost <= Character.Player.AvailableGarrisonResource)
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
                    foreach (var mission in removalList.OrderByDescending(k => k).Select(k => GarrisonManager.AvailableMissions[k]))
                    {
                        GarrisonManager.AvailableMissionIds.Remove(mission.Id);
                        GarrisonManager.AvailableMissions.Remove(mission);
                    }
                }
            }

            if (_currentMission != null)
            {
                GarrisonBase.Log("Starting Mission {0} Rewards {1} Priority {2} Success {3}",
                    _currentMission.Name, _currentMission.RewardTypes, _currentMission.Priority, _successChance);



                //GARRISON_FOLLOWER_LIST_UPDATE
                await
                    CommonCoroutines.WaitForLuaEvent("GARRISON_FOLLOWER_LIST_UPDATE", 7500, null, Action);

                await Coroutine.Sleep(2218);

                await
                    CommonCoroutines.WaitForLuaEvent("GARRISON_MISSION_STARTED", 
                    7500, 
                    null,
                    LuaUI.MissionFrame.StartMissionButton.Click);

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
            LuaUI.MissionFrame.OpenMission(_currentMission.Id);
            LuaUI.MissionFrame.AssignFollowers();
        }

         
    }
}