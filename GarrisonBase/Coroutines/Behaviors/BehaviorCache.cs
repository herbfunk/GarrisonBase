using System;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Herbfunk.GarrisonBase.Cache;
using Herbfunk.GarrisonBase.Cache.Objects;
using Herbfunk.GarrisonBase.Garrison;
using Styx;
using Styx.CommonBot.Coroutines;

namespace Herbfunk.GarrisonBase.Coroutines.Behaviors
{
    public class BehaviorCache: Behavior
    {
        public override BehaviorType Type { get { return BehaviorType.Cache; } }

        public BehaviorCache()
            : base()
        {
            InteractionEntryId = GarrisonManager.GarrisonResourceCacheEntryId;
            MovementPoints.Add(MovementCache.GarrisonEntrance);
            Criteria += () => (BaseSettings.CurrentSettings.BehaviorLootCache &&
                              GarrisonResourceCacheObject != null &&
                              GarrisonResourceCacheObject.RefWoWObject.IsValid);
        }



        public C_WoWGameObject GarrisonResourceCacheObject
        {
            get { return ObjectCacheManager.GetWoWGameObjects(CacheStaticLookUp.ResourceCacheIds.ToArray()).FirstOrDefault(); }
        }

        public override async Task<bool> Movement()
        {
            if (_movement == null)
                _movement = new Movement(GarrisonResourceCacheObject.Location, 5.75f);
                
            if (await _movement.MoveTo())  return true;

            return false;
        }
        private Movement _movement;

        public override async Task<bool> Interaction()
        {
            if (GarrisonResourceCacheObject != null && GarrisonResourceCacheObject.RefWoWObject.IsValid && GarrisonResourceCacheObject.GetCursor == WoWCursorType.InteractCursor)
            {
                GarrisonResourceCacheObject.Interact();
                await CommonCoroutines.SleepForRandomUiInteractionTime();
                await CommonCoroutines.SleepForRandomReactionTime();
                await Coroutine.Yield();
                return true;
            }

            return false;
        }

        public override async Task<bool> BehaviorRoutine()
        {
            if (await base.BehaviorRoutine()) return true;
            if (IsDone) return false;

            if (await StartMovement.MoveTo()) return true;

            if (await Movement()) return true;

            if (await Interaction()) return true;

            return false;
        }
    }
}