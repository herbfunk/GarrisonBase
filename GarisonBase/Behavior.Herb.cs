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
using Styx.CommonBot.Routines;
using Styx.WoWInternals.WoWObjects;

namespace Herbfunk.GarrisonBase
{
    public partial class Behaviors
    {
        public class BehaviorHerb : Behavior
        {
            public override BehaviorType Type { get { return BehaviorType.Herbing; } }

            public BehaviorHerb(List<WoWPoint> navPoints) : base(navPoints.ToArray(), -1)
            {
              
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
                if (MovementCache.Garden != null && !MovementCache.Garden.LocationInsidePolygon(StyxWoW.Me.Location))
                    MovementPoints.Insert(0, MovementCache.Garden.Entrance);

                List<WoWPoint> reversed = MovementPoints.ToList();
                reversed.Reverse();
                _finalmovement = new Movement(reversed.ToArray(), 5f);
                _movementQueue = new Queue<WoWPoint>();
                foreach (var p in MovementPoints)
                {
                    _movementQueue.Enqueue(p);
                }
            }

            private Movement _movement, _finalmovement, _herbmovement;
            private Queue<WoWPoint> _movementQueue;
            private bool _checkedReset = false;


            public override async Task<bool> BehaviorRoutine()
            {
                if (IsDone) return false;

                if (await base.BehaviorRoutine()) return true;

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

                if (_movement == null || _movement.CurrentMovementQueue.Count == 0)
                {
                    if (_movementQueue.Count > 0)
                    {
                        _movement = new Movement(_movementQueue.Dequeue(), 5f);
                    }
                }

                if (_movement != null)
                {
                    TreeRoot.StatusText = String.Format("Behavior {0} Movement", Type.ToString());
                    if (await _movement.MoveTo())
                        return true;
                }

                if (GarrisonManager.Buildings[BuildingType.HerbGarden].FirstQuestCompleted)
                {
                    if (_nearestHerb == null || !_nearestHerb.IsStillValid())
                    {
                        _nearestHerb = NearestHerb;
                    }
                    else if (_nearestHerb.Guid != NearestHerb.Guid)
                    {
                        _nearestHerb = NearestHerb;
                    }
                    else if (_nearestHerb.IsStillValid() && _nearestHerb.InLineOfSight)
                    {
                        if (_herbmovement == null || _herbmovement.CurrentMovementQueue.Count == 0)
                            _herbmovement = new Movement(_nearestHerb.Location, 5f);
                    }

                    if (_nearestHerb != null)
                    {
                        if (_herbmovement != null && _herbmovement.CurrentMovementQueue.Count > 0)
                        {
                            if (await HarvestHerb(_nearestHerb))
                                return true;
                        }

                        return true;
                    }
                }
                else
                {
                    var nearestQuestNpc = NearestQuestNPC;
                    if (nearestQuestNpc != null && nearestQuestNpc.IsValid && nearestQuestNpc.InLineOfSight)
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
                
                

                if (_movementQueue.Count == 0)
                {
                    if (await _finalmovement.MoveTo())
                        return true;

                    if (!GarrisonManager.Buildings[BuildingType.HerbGarden].FirstQuestCompleted)
                    {
                        return true;
                    }
                }


                BaseSettings.CurrentSettings.LastCheckedHerbString = LuaCommands.GetGameTime().ToString("yyyy-MM-ddTHH:mm:ss");
                BaseSettings.SerializeToXML(BaseSettings.CurrentSettings);
                return false;
            }

            private async Task<bool> HarvestHerb(C_WoWGameObject herb)
            {
                if (!herb.IsStillValid()) return false;


                if (herb.WithinInteractRange)
                {
                    TreeRoot.StatusText = String.Format("Behavior {0} Interaction", Type.ToString());
                    if (StyxWoW.Me.IsMoving)
                    {
                        await CommonCoroutines.StopMoving();
                    }

                    await CommonCoroutines.SleepForLagDuration();
                    await Coroutine.Sleep(2000);

                    if (herb.IsStillValid())
                    {
                        await CommonCoroutines.WaitForLuaEvent(
                        "LOOT_OPENED",
                        7500,
                        () => LuaEvents.LootOpened,
                        herb.Interact);
                    }
                    
                    return true;
                }
                //<WoWUnit Name="Frostwall Nibbler" Entry="81967" X="5433.446" Y="4571.565" Z="136.082" />
                TreeRoot.StatusText = String.Format("Behavior {0} IMovement", Type.ToString());
                await _herbmovement.MoveTowards();
                

                return true;
            }



            private C_WoWGameObject NearestHerb
            {
                get
                {
                    var lOres = ObjectCacheManager.GetWoWGameObjects(CacheStaticLookUp.HerbDeposits.ToArray());
                    if (lOres.Count > 0)
                    {
                        return lOres.OrderBy(o => o.CentreDistance).FirstOrDefault();
                    }

                    return null;
                }
            }
            private C_WoWGameObject _nearestHerb;
            private C_WoWUnit NearestQuestNPC
            {
                get
                {
                    var units =ObjectCacheManager.GetWoWUnits(CacheStaticLookUp.HerbQuestMobIDs.ToArray()).Where(u => !u.IsDead).ToList();
                   
                    if (units.Count > 0)
                    {
                        return units.OrderBy(o => o.CentreDistance).FirstOrDefault();
                    }

                    return null;
                }
            }

        }
    }
}
