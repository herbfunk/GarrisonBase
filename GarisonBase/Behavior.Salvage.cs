using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Herbfunk.GarrisonBase.Cache;
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
                _finalmovement = new Movement(building.SafeMovementPoint, 3f);
            }
            public override void Initalize()
            {
                if (Building.BuildingPolygon != null && !Building.BuildingPolygon.LocationInsidePolygon(StyxWoW.Me.Location))
                    MovementPoints.Insert(0, Building.BuildingPolygon.Entrance);

                MovementPoints.Add(Building.EntranceMovementPoint);
                base.Initalize();
            }
            public Building Building { get; set; }

            public override Func<bool> Criteria
            {
                get
                {
                    return () => BaseSettings.CurrentSettings.BehaviorSalvaging && SalvagableGoods.Count > 0;
                }
            }
            private List<C_WoWItem> SalvagableGoods
            {
                get
                {
                    return Player.Inventory.GetBagItemsById(114116, 114120, 114119);
                    //return StyxWoW.Me.BagItems.Where(i => 
                    //    i.Entry == 114116 || 
                    //    i.Entry == 114120 ||
                    //    i.Entry == 114119).ToList(); 
                }
            }

            public override async Task<bool> Movement()
            {
                TreeRoot.StatusText = String.Format("Behavior {0} [{1}] Movement", Type.ToString(), Building.Type);
                if (await base.Movement())
                    return true;

                TreeRoot.StatusText = String.Format("Behavior {0} [{1}] Movement2", Type.ToString(), Building.Type);
                //Move to the interaction object (within 6.7f)
                if (_movement == null)
                {
                    if (InteractionObject == null)
                    {
                        //Error Cannot find object!
                        Building.CheckedWorkOrderPickUp = true;
                        return false;
                    }
                    _movement = new Movement(InteractionObject.Location, 3.3f);
                }

                if (await _movement.MoveTo())
                    return true;

                
                return false;
            }
            private Movement _movement;
            private readonly Movement _finalmovement;
            public override async Task<bool> Interaction()
            {
                if (SalvagableGoods.Count == 0)
                    return false;

                TreeRoot.StatusText = String.Format("Behavior {0} [{1}] Interaction", Type.ToString(), Building.Type);
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

            public override async Task<bool> BehaviorRoutine()
            {
                if (IsDone) return false;

                if (await base.BehaviorRoutine()) return true;

                if (await Movement())
                    return true;

                if (await Interaction())
                    return true;

                if (await _finalmovement.MoveTo())
                    return true;

                return false;
            }
        }
    }
}
