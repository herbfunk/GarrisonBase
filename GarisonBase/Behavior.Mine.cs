using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Herbfunk.GarrisonBase.Cache;
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

            public BehaviorMine(List<WoWPoint> navPoints ) : base(navPoints.ToArray(), -1)
            {
                //List<WoWPoint> reversed = Cache.Mine_LevelOne.ToList();
                //reversed.Reverse();
                //_finalmovement = new Movement(reversed.ToArray(), 5f);
                
            }
            public override Func<bool> Criteria
            {
                get
                {
                    return () =>
                        ((!GarrisonManager.Buildings[BuildingType.Mines].FirstQuestCompleted && Player.Level >= 92 && BaseSettings.CurrentSettings.BehaviorQuests)||
                        (GarrisonManager.Buildings[BuildingType.Mines].FirstQuestCompleted && LuaCommands.CheckForDailyReset(BaseSettings.CurrentSettings.LastCheckedMine) && BaseSettings.CurrentSettings.BehaviorMineGather));
                }
            }
            public override void Initalize()
            {
                ObjectCacheManager.ShouldLoot = true;

                if ((!Player.MinimapZoneText.Contains("Mine") || !Player.MinimapZoneText.Contains("Excavation"))
                    && (MovementCache.Mine != null && !MovementCache.Mine.LocationInsidePolygon(StyxWoW.Me.Location)))
                    MovementPoints.Insert(0, MovementCache.Mine.Entrance);

                _movementQueue = new Queue<WoWPoint>();
                foreach (var p in MovementPoints)
                {
                    _movementQueue.Enqueue(p);
                }
            }
            private Movement _movement, _finalmovement, _oremovement;
            private Queue<WoWPoint> _movementQueue;
            private bool _checkedReset = false;



            public override async Task<bool> BehaviorRoutine()
            {
                if (IsDone) return false;

                if (await base.BehaviorRoutine()) return true;

                #region Quest
                if (!GarrisonManager.Buildings[BuildingType.Mines].FirstQuestCompleted)
                {
                    if (!QuestManager.QuestContainedInQuestLog(GarrisonManager.Buildings[BuildingType.Mines].FirstQuestID))
                    {
                        TreeRoot.StatusText = String.Format("Behavior {0} Quest Pickup", Type.ToString());

                        //Move to quest giver..
                        if (_movement == null)
                            _movement = new Movement(MovementCache.MinePlot59SafePoint, 10f);

                        if (await _movement.MoveTo())
                            return true;

                        //QUEST_ACCEPTED
                        if (!LuaEvents.QuestFrameOpen)
                        {
                            C_WoWObject _npc = ObjectCacheManager.GetWoWObject(GarrisonManager.Buildings[BuildingType.Mines].QuestNpcID);
                            if (_npc != null && _npc.IsValid)
                            {
                                if (_npc.WithinInteractRange)
                                {
                                    TreeRoot.StatusText = String.Format("Behavior {0} Quest Pickup NPC Interact", Type.ToString());
                                    _npc.Interact();
                                    await CommonCoroutines.SleepForRandomUiInteractionTime();
                                    return true;
                                }

                                TreeRoot.StatusText = String.Format("Behavior {0} Quest Pickup NPC Moveto", Type.ToString());
                                await CommonCoroutines.MoveTo(_npc.Location);
                                return true;
                            }
                        }
                        else
                        {
                            if (QuestFrame.Instance.IsVisible)
                            {
                                TreeRoot.StatusText = String.Format("Behavior {0} Quest Pickup NPC Accept Quest", Type.ToString());

                                QuestFrame.Instance.AcceptQuest();
                                await CommonCoroutines.SleepForRandomUiInteractionTime();
                            }
                            return true;
                        }

                    }
                    else if (QuestManager.GetQuestFromQuestLog(GarrisonManager.Buildings[BuildingType.Mines].FirstQuestID).IsCompleted)
                    {
                        TreeRoot.StatusText = String.Format("Behavior {0} Quest Completion", Type.ToString());

                        if (!LuaEvents.QuestFrameOpen)
                        {
                            C_WoWObject _npc = ObjectCacheManager.GetWoWObject(GarrisonManager.Buildings[BuildingType.Mines].QuestNpcID);
                            if (_npc != null && _npc.IsValid)
                            {
                                if (_npc.WithinInteractRange)
                                {
                                    TreeRoot.StatusText = String.Format("Behavior {0} Quest Completion NPC Interact", Type.ToString());
                                    _npc.Interact();
                                    await CommonCoroutines.SleepForRandomUiInteractionTime();
                                    return true;
                                }

                                TreeRoot.StatusText = String.Format("Behavior {0} Quest Completion NPC MoveTo", Type.ToString());
                                await CommonCoroutines.MoveTo(_npc.Location);
                                return true;
                            }
                        }
                        else
                        {
                            if (QuestFrame.Instance.IsVisible)
                            {
                                TreeRoot.StatusText = String.Format("Behavior {0} Quest Completion NPC Complete Quest", Type.ToString());

                                QuestFrame.Instance.CompleteQuest();
                                await CommonCoroutines.SleepForRandomUiInteractionTime();
                                GarrisonManager.Buildings[BuildingType.Mines].FirstQuestCompleted = true;
                                await Coroutine.Sleep(5000);
                                GarrisonManager.RefreshBuildings();
                            }
                            return true;
                        }
                    }
                }
                
                #endregion

                if (_movement == null || _movement.CurrentMovementQueue.Count == 0)
                {
                    if (_movementQueue.Count > 0)
                    {
                        _movement = new Movement(_movementQueue.Dequeue(), 5f);
                    }
                }

                //if (GarrisonManager.Buildings[BuildingType.Mines].FirstQuestCompleted)
                //{
                //    if (_nearestOre == null || !_nearestOre.IsStillValid())
                //    {
                //        _nearestOre = NearestOre;
                //    }
                //    else if (_nearestOre.Guid != NearestOre.Guid)
                //    {
                //        _nearestOre = NearestOre;
                //    }
                //    else if (_nearestOre.IsStillValid() && _nearestOre.InLineOfSight)
                //    {
                //        if (_oremovement == null || _oremovement.CurrentMovementQueue.Count == 0)
                //            _oremovement = new Movement(_nearestOre.Location, 6f);
                //    }

                //    if (_nearestOre != null)
                //    {
                //        if (_oremovement != null && _oremovement.CurrentMovementQueue.Count > 0)
                //        {
                //            if (await MineOre())
                //                return true;
                //        }
                //    }
                //}


                if (_movementQueue.Count == 0)
                {
                    //if (await _finalmovement.MoveTo())
                    //    return true;

                    //if (!Cache.CompletedMineQuest)
                    //{
                    //    return true;
                    //}
                }
                else
                {
                    if (_movement != null)
                    {
                        TreeRoot.StatusText = String.Format("Behavior {0} Movement", Type.ToString());
                        await _movement.MoveTo();
                    }

                    return true;
                }

                BaseSettings.CurrentSettings.LastCheckedMineString = LuaCommands.GetGameTime().ToString("yyyy-MM-ddTHH:mm:ss");
                BaseSettings.SerializeToXML(BaseSettings.CurrentSettings);
                ObjectCacheManager.ShouldLoot = false;

                return false;
            }

            private async Task<bool> MineOre()
            {
                if (!_nearestOre.IsStillValid()) return false;

                WoWCursorType cursorType = _nearestOre.GetCursor;
                if (cursorType == WoWCursorType.MineCursor || cursorType == WoWCursorType.InteractCursor)
                {
                    TreeRoot.StatusText = String.Format("Behavior {0} Interaction", Type.ToString());
                    if (StyxWoW.Me.IsMoving)
                    {
                        await CommonCoroutines.StopMoving();
                    }

                    await CommonCoroutines.SleepForLagDuration();
                    await Coroutine.Sleep(2000);

                    if (_nearestOre.IsStillValid())
                    {
                        await CommonCoroutines.WaitForLuaEvent(
                        "LOOT_OPENED",
                        7500,
                        () => LuaEvents.LootOpened,
                        _nearestOre.Interact);

                        ObjectCacheManager.BlacklistGuiDs.Add(_nearestOre.Guid);
                        _nearestOre = NearestOre;
                        _oremovement = null;
                    }
                    
                    return true;
                }

                TreeRoot.StatusText = String.Format("Behavior {0} IMovement", Type.ToString());
                MoveResult result = await _oremovement.MoveTowards();

                if (result == MoveResult.Failed)
                {
                    ObjectCacheManager.BlacklistGuiDs.Add(_nearestOre.Guid);
                    _nearestOre = NearestOre;
                    _oremovement = null;
                }

                return true;
            }


            private C_WoWGameObject _nearestOre;
            private C_WoWGameObject NearestOre
            {
                get
                {

                    var lOres = ObjectCacheManager.GetWoWGameObjects(CacheStaticLookUp.MineDeposits.ToArray());
                    if (lOres.Count > 0)
                    {
                        return lOres.OrderBy(o => o.CentreDistance).FirstOrDefault();
                    }

                    return null;
                }
            }

           
        }
    }
}
