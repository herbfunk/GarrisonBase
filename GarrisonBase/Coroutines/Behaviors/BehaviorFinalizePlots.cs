using System;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Herbfunk.GarrisonBase.Cache;
using Herbfunk.GarrisonBase.Cache.Objects;
using Herbfunk.GarrisonBase.Garrison.Enums;
using Herbfunk.GarrisonBase.Garrison.Objects;
using Styx;
using Styx.CommonBot.Coroutines;

namespace Herbfunk.GarrisonBase.Coroutines.Behaviors
{
    public class BehaviorFinalizePlots : Behavior
    {
        public override BehaviorType Type { get { return BehaviorType.FinalizeBuilding; } }

        public BehaviorFinalizePlots(Building building)
            : base()
        {
            Building = building;
            MovementPoints.Add(Building.SafeMovementPoint);
            Criteria += () => Building.CanActivate;
        }

        public Building Building { get; set; }


        public override async Task<bool> BehaviorRoutine()
        {
            if (await base.BehaviorRoutine()) return true;
            if (IsDone) return false;

            if (await StartMovement.MoveTo()) return true;

            if (await Interaction())
                return true;

            return false;
        }


        private Movement _movement;
        private uint entryId=0;
        public override async Task<bool> Interaction()
        {
            if (InteractionObject != null && InteractionObject.IsValid)
            {
                if (entryId == 0) entryId = InteractionObject.Entry;
                GarrisonBase.Log("Activating " + InteractionObject.Name + ", waiting...");
                InteractionObject.Interact();
                await CommonCoroutines.SleepForRandomUiInteractionTime();

                await Coroutine.Wait(5000, () => !StyxWoW.Me.IsCasting && CacheStaticLookUp.GetWoWObject(entryId) == null);
                await Coroutine.Sleep(StyxWoW.Random.Next(1999, 3001));
                return true;
            }
                
            Building.CanActivate = false;

            if (Building.Type == BuildingType.HerbGarden || Building.Type== BuildingType.Mines)
            {//Since activating herb/mine building resets the nodes, we should reset our last checked so we can redo it..
                if (Building.Type == BuildingType.HerbGarden)
                    BaseSettings.CurrentSettings.LastCheckedHerbString = "0001-01-01T00:00:00";
                else
                    BaseSettings.CurrentSettings.LastCheckedMineString = "0001-01-01T00:00:00";

                BaseSettings.SerializeToXML(BaseSettings.CurrentSettings);
            }
                
            return false;
        }

        private C_WoWObject _interactionObject;
        public override C_WoWObject InteractionObject
        {
            get
            {
                if (_interactionObject == null)
                {
                    var objs = ObjectCacheManager.GetWoWGameObjects(CacheStaticLookUp.FinalizeGarrisonPlotIds.ToArray());
                    if (objs.Count > 0)
                        _interactionObject = objs.OrderBy(o => o.Distance).First();
                }
                return _interactionObject;
            }
        }

         
    }
}