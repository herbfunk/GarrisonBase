using System;
using System.Collections.Generic;
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
        public class BehaviorSalvage: Behavior
        {
            public override BehaviorType Type { get { return BehaviorType.Salvage; } }

            public BehaviorSalvage(Building building)
                : base(building.SafeMovementPoint, building.WorkOrderNPCEntryId)
            {
                Building = building;
            }
            public override void Initalize()
            {
                MovementPoints.Add(Building.EntranceMovementPoint);
                base.Initalize();
            }
            
            public override Func<bool> Criteria
            {
                get
                {
                    return () => BaseSettings.CurrentSettings.BehaviorSalvaging && SalvagableGoods.Count > 0;
                }
            }

            public Building Building { get; set; }

            public override async Task<bool> BehaviorRoutine()
            {
                if (IsDone) return false;

                if (await base.BehaviorRoutine()) return true;

                if (await StartMovement.MoveTo()) return true;

                if (await Interaction()) return true;

                if (await EndMovement.MoveTo()) return true;

                return false;
            }

            private List<C_WoWItem> SalvagableGoods
            {
                get
                {
                    return Player.Inventory.GetBagItemsById(114116, 114120, 114119);
                }
            }


            private Movement _movement;

            public override async Task<bool> Interaction()
            {
                if (_movement == null)
                {
                    if (InteractionObject == null)
                    {
                        //Error Cannot find object!
                        IsDone = true;
                        return false;
                    }
                    _movement = new Movement(InteractionObject.Location, 3.3f);
                }

                if (await _movement.MoveTo())
                    return true;
                

                if (SalvagableGoods.Count == 0)
                    return false;

              
                foreach (C_WoWItem salvageCrate in SalvagableGoods)
                {
                    salvageCrate.Interact();
                    await CommonCoroutines.SleepForRandomUiInteractionTime();

                    await Coroutine.Wait(5000, () => !StyxWoW.Me.IsCasting);
                    await Coroutine.Yield();
                    await Coroutine.Sleep(1250);
                }

                if (StyxWoW.LastRedErrorMessage.Contains("Requires Salvage Yard"))
                {
                    WoWMovement.Move(WoWMovement.MovementDirection.Forward, new TimeSpan(0,0,0,2));
                }
                return true;
            }

            
        }
    }
}
