using System;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Herbfunk.GarrisonBase.Cache;
using Herbfunk.GarrisonBase.Cache.Objects;
using Herbfunk.GarrisonBase.Garrison.Objects;
using Styx;
using Styx.CommonBot;
using Styx.CommonBot.Coroutines;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace Herbfunk.GarrisonBase
{
    public partial class Behaviors
    {
        public class BehaviorFinalizePlots : Behavior
        {
            public override BehaviorType Type { get { return BehaviorType.FinalizeBuilding; } }

            public BehaviorFinalizePlots(Building building)
                : base(building.SafeMovementPoint, building.WorkOrderObjectEntryId)
            {
                Building = building;
            }
            public override void Initalize()
            {
                base.Initalize();
            }
            public Building Building { get; set; }

            public override Func<bool> Criteria
            {
                get
                {
                    return () => Building.CanActivate;
                }
            }
            public override async Task<bool> BehaviorRoutine()
            {
                if (IsDone) return false;

                if (await base.BehaviorRoutine()) return true;

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
                else
                    Building.CanActivate = false;
                
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
}
