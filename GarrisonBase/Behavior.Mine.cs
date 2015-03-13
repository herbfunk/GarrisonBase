using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Herbfunk.GarrisonBase.Cache;
using Herbfunk.GarrisonBase.Cache.Enums;
using Herbfunk.GarrisonBase.Cache.Objects;
using Herbfunk.GarrisonBase.Garrison;
using Herbfunk.GarrisonBase.Garrison.Enums;
using Herbfunk.GarrisonBase.Quest;
using Styx;
using Styx.CommonBot;
using Styx.CommonBot.Coroutines;
using Styx.CommonBot.Frames;
using Styx.Helpers;
using Styx.Pathing;
using Styx.WoWInternals.WoWObjects;

namespace Herbfunk.GarrisonBase
{
    public partial class Behaviors
    {
        public class BehaviorMine : Behavior
        {
            public override BehaviorType Type { get { return BehaviorType.Mining; } }

            public BehaviorMine(List<WoWPoint> navPoints)
                : base(MovementCache.MinePlot59SafePoint)
            {
                _minePoints = new List<WoWPoint>(navPoints);
            }


            public override Func<bool> Criteria
            {
                get
                {
                    return () =>
                        (GarrisonManager.Buildings[BuildingType.Mines].FirstQuestCompleted && 
                        LuaCommands.CheckForDailyReset(BaseSettings.CurrentSettings.LastCheckedMine) && 
                        BaseSettings.CurrentSettings.BehaviorMineGather);
                }
            }
            public override void Initalize()
            {
                base.Initalize();
                _movementQueue = new Queue<WoWPoint>();
                foreach (var p in _minePoints)
                {
                    _movementQueue.Enqueue(p);
                }
            }
            private Movement _movement, _minemovement;
            private Queue<WoWPoint> _movementQueue;
            private readonly List<WoWPoint> _minePoints;


            public override async Task<bool> BehaviorRoutine()
            {
                if (IsDone) return false;

                if (await base.BehaviorRoutine()) return true;

                if (await StartMovement.MoveTo()) return true;

                ObjectCacheManager.ShouldLoot = true;

                #region Quest
                //if (!GarrisonManager.Buildings[BuildingType.Mines].FirstQuestCompleted)
                //{
                //    if (!QuestManager.QuestContainedInQuestLog(GarrisonManager.Buildings[BuildingType.Mines].FirstQuestID))
                //    {
                //        //TreeRoot.StatusText = String.Format("Behavior {0} Quest Pickup", Type.ToString());

                //        //Move to quest giver..
                //        if (_movement == null)
                //            _movement = new Movement(MovementCache.MinePlot59SafePoint, 10f);

                //        if (await _movement.MoveTo())
                //            return true;

                //        //QUEST_ACCEPTED
                //        if (!LuaEvents.QuestFrameOpen)
                //        {
                //            C_WoWObject _npc = ObjectCacheManager.GetWoWObject(GarrisonManager.Buildings[BuildingType.Mines].QuestNpcID);
                //            if (_npc != null && _npc.IsValid)
                //            {
                //                if (_npc.WithinInteractRange)
                //                {
                //                    //TreeRoot.StatusText = String.Format("Behavior {0} Quest Pickup NPC Interact", Type.ToString());
                //                    _npc.Interact();
                //                    await CommonCoroutines.SleepForRandomUiInteractionTime();
                //                    return true;
                //                }

                //                //TreeRoot.StatusText = String.Format("Behavior {0} Quest Pickup NPC Moveto", Type.ToString());
                //                await CommonCoroutines.MoveTo(_npc.Location);
                //                return true;
                //            }
                //        }
                //        else
                //        {
                //            if (QuestFrame.Instance.IsVisible)
                //            {
                //                //TreeRoot.StatusText = String.Format("Behavior {0} Quest Pickup NPC Accept Quest", Type.ToString());

                //                QuestFrame.Instance.AcceptQuest();
                //                await CommonCoroutines.SleepForRandomUiInteractionTime();
                //            }
                //            return true;
                //        }

                //    }
                //    else if (QuestManager.GetQuestFromQuestLog(GarrisonManager.Buildings[BuildingType.Mines].FirstQuestID).IsCompleted)
                //    {
                //        //TreeRoot.StatusText = String.Format("Behavior {0} Quest Completion", Type.ToString());

                //        if (!LuaEvents.QuestFrameOpen)
                //        {
                //            C_WoWObject _npc = ObjectCacheManager.GetWoWObject(GarrisonManager.Buildings[BuildingType.Mines].QuestNpcID);
                //            if (_npc != null && _npc.IsValid)
                //            {
                //                if (_npc.WithinInteractRange)
                //                {
                //                    //TreeRoot.StatusText = String.Format("Behavior {0} Quest Completion NPC Interact", Type.ToString());
                //                    _npc.Interact();
                //                    await CommonCoroutines.SleepForRandomUiInteractionTime();
                //                    return true;
                //                }

                //                //TreeRoot.StatusText = String.Format("Behavior {0} Quest Completion NPC MoveTo", Type.ToString());
                //                await CommonCoroutines.MoveTo(_npc.Location);
                //                return true;
                //            }
                //        }
                //        else
                //        {
                //            if (QuestFrame.Instance.IsVisible)
                //            {
                //                //TreeRoot.StatusText = String.Format("Behavior {0} Quest Completion NPC Complete Quest", Type.ToString());
                //                if (!BaseSettings.CurrentSettings.DEBUG_FAKEFINISHQUEST)
                //                {
                //                    QuestFrame.Instance.CompleteQuest();
                //                    await CommonCoroutines.SleepForRandomUiInteractionTime();
                //                    GarrisonManager.Buildings[BuildingType.Mines].FirstQuestCompleted = true;
                //                    await Coroutine.Sleep(5000);
                //                    GarrisonManager.RefreshBuildings();
                //                }

                //                return false;
                //            }
                //            return true;
                //        }
                //    }
                //}
                
                #endregion

                ObjectCacheManager.UpdateLootableTarget();

                if (_movement == null || _movement.CurrentMovementQueue.Count == 0)
                {
                    if (_movementQueue.Count > 0)
                    {
                        _movement = new Movement(_movementQueue.Dequeue(), 5f);
                    }
                }

                if (ObjectCacheManager.FoundOreObject)
                {
                    if (_movementQueue.Count > 0)
                    {
                        if (_movement != null)
                        {
                            await _movement.MoveTo();
                        }

                        return true;
                    }
                }

                if (await EndMovement.MoveTo()) return true;

                BaseSettings.CurrentSettings.LastCheckedMineString = LuaCommands.GetGameTime().ToString("yyyy-MM-ddTHH:mm:ss");
                BaseSettings.SerializeToXML(BaseSettings.CurrentSettings);
                ObjectCacheManager.ShouldLoot = false;

                return false;
            }


           
        }
    }
}
