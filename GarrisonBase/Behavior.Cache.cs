using System;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Herbfunk.GarrisonBase.Cache;
using Herbfunk.GarrisonBase.Cache.Objects;
using Herbfunk.GarrisonBase.Garrison;
using Styx;
using Styx.CommonBot;
using Styx.CommonBot.Coroutines;
using Styx.WoWInternals.WoWObjects;

namespace Herbfunk.GarrisonBase
{
    public partial class Behaviors
    {
        public class BehaviorCache: Behavior
        {
            public override BehaviorType Type { get { return BehaviorType.Cache; } }

            public BehaviorCache()
                : base(MovementCache.GarrisonEntrance, GarrisonManager.GarrisonResourceCacheEntryId)
            {
            }

            public override void Initalize()
            {
                base.Initalize();
            }
            public override Func<bool> Criteria
            {
                get
                {
                    return () => (BaseSettings.CurrentSettings.BehaviorLootCache &&
                                    GarrisonResourceCacheObject != null &&
                                    GarrisonResourceCacheObject.RefWoWObject.IsValid);
                }
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
                if (IsDone) return false;

                if (await base.BehaviorRoutine()) return true;

                if (await StartMovement.MoveTo()) return true;

                if (await Movement()) return true;

                if (await Interaction()) return true;

                return false;
            }
        }
    }
}
