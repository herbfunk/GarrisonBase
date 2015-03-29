using System;
using System.Threading.Tasks;
using Herbfunk.GarrisonBase.Cache;
using Styx;
using Styx.CommonBot;
using Styx.CommonBot.Coroutines;

namespace Herbfunk.GarrisonBase.Coroutines.Behaviors
{
    public class BehaviorSelectTarget : Behavior
    {
        public BehaviorSelectTarget(uint entryId)
        {
            _entryId = entryId;
        }
        public BehaviorSelectTarget(WoWPoint location)
        {
            _location = location;
        }
        private readonly uint _entryId;
        private readonly WoWPoint _location=WoWPoint.Zero;

        public override async Task<bool> BehaviorRoutine()
        {
            if (await base.BehaviorRoutine()) return true;
            if (IsDone) return false;
                

            if (_entryId != 0)
            {
                if (StyxWoW.Me.CurrentTarget != null)
                {
                    if (StyxWoW.Me.CurrentTarget.Entry == _entryId)
                    {
                        IsDone = true;
                        return false;
                    }
                }

                var _units = ObjectCacheManager.GetWoWUnits(_entryId);
                if (_units.Count > 0)
                {
                    TreeRoot.StatusText = String.Format("Behavior Targeting Unit {0}", _units[0].Name);
                    _units[0].RefWoWUnit.Target();
                    await CommonCoroutines.SleepForRandomUiInteractionTime();
                    return true;
                }
            }
            else
            {
                var _units = ObjectCacheManager.GetUnitsNearPoint(_location, 5f);
                if (_units.Count > 0)
                {
                    if (StyxWoW.Me.CurrentTarget != null)
                    {
                        if (StyxWoW.Me.CurrentTarget.Entry == _units[0].Entry)
                        {
                            IsDone = true;
                            return false;
                        }
                    }

                    TreeRoot.StatusText = String.Format("Behavior Targeting Unit {0}", _units[0].Name);
                    _units[0].RefWoWUnit.Target();
                    await CommonCoroutines.SleepForRandomUiInteractionTime();
                    return true;
                }
            }
                

            return false;
        }
    }
}