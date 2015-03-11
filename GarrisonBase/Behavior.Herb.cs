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
using Styx.CommonBot.Routines;
using Styx.WoWInternals.WoWObjects;

namespace Herbfunk.GarrisonBase
{
    public partial class Behaviors
    {
        public class BehaviorHerb : Behavior
        {
            public override BehaviorType Type { get { return BehaviorType.Herbing; } }

            public BehaviorHerb(List<WoWPoint> navPoints)
                : base(MovementCache.GardenPlot63SafePoint)
            {
                _herbPoints = new List<WoWPoint>(navPoints);
            }
            public override Func<bool> Criteria
            {
                get
                {
                    return () =>
                        ((!GarrisonManager.Buildings[BuildingType.HerbGarden].FirstQuestCompleted && Player.Level>=96 && BaseSettings.CurrentSettings.BehaviorQuests) ||
                        (GarrisonManager.Buildings[BuildingType.HerbGarden].FirstQuestCompleted && LuaCommands.CheckForDailyReset(BaseSettings.CurrentSettings.LastCheckedHerb) && BaseSettings.CurrentSettings.BehaviorHerbGather));
                }
            }
            public override void Initalize()
            {
                base.Initalize();
                _movementQueue = new Queue<WoWPoint>();
                foreach (var p in _herbPoints)
                {
                    _movementQueue.Enqueue(p);
                }
            }

            private Movement _movement;
            private Queue<WoWPoint> _movementQueue;
            private readonly List<WoWPoint> _herbPoints;

            public override async Task<bool> BehaviorRoutine()
            {
                if (IsDone) return false;

                if (await base.BehaviorRoutine()) return true;

                if (await base.StartMovement.MoveTo()) return true;

                #region Quest
                if (!GarrisonManager.Buildings[BuildingType.HerbGarden].FirstQuestCompleted)
                {
                    //Move to quest giver..
                    if (_movement == null)
                        _movement = new Movement(MovementCache.GardenPlot63SafePoint, 10f);

                    if (await _movement.MoveTo())
                        return true;

                    if (!QuestManager.QuestContainedInQuestLog(GarrisonManager.Buildings[BuildingType.HerbGarden].FirstQuestID))
                    {
                        TreeRoot.StatusText = String.Format("Behavior {0} Quest Pickup", Type.ToString());

                        //QUEST_ACCEPTED
                        if (!LuaEvents.QuestFrameOpen)
                        {
                            C_WoWObject _npc =ObjectCacheManager.GetWoWObject(GarrisonManager.Buildings[BuildingType.HerbGarden].QuestNpcID);
                         
                            if (_npc != null && _npc.ref_WoWObject.IsValid)
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
                    else if (QuestManager.GetQuestFromQuestLog(GarrisonManager.Buildings[BuildingType.HerbGarden].FirstQuestID).IsCompleted)
                    {
                        TreeRoot.StatusText = String.Format("Behavior {0} Quest Completion", Type.ToString());

                        if (!LuaEvents.QuestFrameOpen)
                        {
                            C_WoWObject _npc = ObjectCacheManager.GetWoWObject(GarrisonManager.Buildings[BuildingType.HerbGarden].QuestNpcID);
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
                                GarrisonManager.Buildings[BuildingType.HerbGarden].FirstQuestCompleted = true;
                                await Coroutine.Sleep(5000);
                                GarrisonManager.RefreshBuildings();
                            }
                            return true;
                        }
                    }
                }
                
                #endregion

                if (!GarrisonManager.Buildings[BuildingType.HerbGarden].FirstQuestCompleted)
                {
                    var nearestQuestNpc = NearestQuestNPC;
                    if (nearestQuestNpc != null && nearestQuestNpc.IsValid && nearestQuestNpc.LineOfSight)
                    {

                        if (StyxWoW.Me.CurrentTarget==null || StyxWoW.Me.CurrentTarget.Guid != nearestQuestNpc.Guid)
                        {
                            nearestQuestNpc.ref_WoWUnit.Target();
                            await CommonCoroutines.SleepForRandomUiInteractionTime();
                            if (!StyxWoW.Me.IsAutoAttacking)
                            {
                                StyxWoW.Me.ToggleAttack();
                                await CommonCoroutines.SleepForRandomUiInteractionTime();
                            }

                            Targeting.Instance.TargetList.Add(nearestQuestNpc.ref_WoWUnit);
                        }

                        await RoutineManager.Current.CombatBehavior.ExecuteCoroutine();
                        return true;
                    }
                }


                ObjectCacheManager.ShouldLoot = true;

                if (_movement == null || _movement.CurrentMovementQueue.Count == 0)
                {
                    if (_movementQueue.Count > 0)
                    {
                        _movement = new Movement(_movementQueue.Dequeue(), 5f);
                    }
                }

                if (ObjectCacheManager.FoundHerbObject)
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

                BaseSettings.CurrentSettings.LastCheckedHerbString = LuaCommands.GetGameTime().ToString("yyyy-MM-ddTHH:mm:ss");
                BaseSettings.SerializeToXML(BaseSettings.CurrentSettings);
                ObjectCacheManager.ShouldLoot = false;

                return false;
            }

            private C_WoWUnit NearestQuestNPC
            {
                get
                {
                    var units =ObjectCacheManager.GetWoWUnits(CacheStaticLookUp.HerbQuestMobIDs.ToArray()).Where(u => !u.IsDead).ToList();
                   
                    if (units.Count > 0)
                    {
                        return units.OrderBy(o => o.Distance).FirstOrDefault();
                    }

                    return null;
                }
            }

        }
    }
}
