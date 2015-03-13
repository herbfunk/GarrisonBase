using System;
using System.Threading.Tasks;
using Herbfunk.GarrisonBase.Cache;
using Styx;
using Styx.CommonBot;
using Styx.CommonBot.Coroutines;

namespace Herbfunk.GarrisonBase
{
    public partial class Behaviors
    {
        public class BehaviorSelectTarget : Behavior
        {
            public BehaviorSelectTarget(uint entryId)
            {
                EntryID = entryId;
            }

            private readonly uint EntryID;


            public override async Task<bool> BehaviorRoutine()
            {
                if (IsDone) return false;

                if (await base.BehaviorRoutine()) return true;

                if (StyxWoW.Me.CurrentTarget != null)
                {
                    if (StyxWoW.Me.CurrentTarget.Entry == EntryID)
                        return false;
                }

                var _units = ObjectCacheManager.GetWoWUnits(EntryID);
                if (_units.Count > 0)
                {
                    TreeRoot.StatusText = String.Format("Behavior Targeting Unit {0}", _units[0].Name);
                    _units[0].RefWoWUnit.Target();
                    await CommonCoroutines.SleepForRandomUiInteractionTime();
                    return true;
                }

                return false;
            }
        }
    }
}
