using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Herbfunk.GarrisonBase.Cache.Objects;
using Herbfunk.GarrisonBase.Garrison;
using Herbfunk.GarrisonBase.Garrison.Enums;
using Herbfunk.GarrisonBase.Garrison.Objects;
using Styx;
using Styx.CommonBot.Coroutines;
using Styx.WoWInternals;

namespace Herbfunk.GarrisonBase.Coroutines.Behaviors
{
    public class BehaviorSalvage: Behavior
    {
        public override BehaviorType Type { get { return BehaviorType.Salvage; } }

        public BehaviorSalvage()
            : base()
        {
            Building = GarrisonManager.Buildings[BuildingType.SalvageYard];
            MovementPoints.Add(Building.SafeMovementPoint);
            MovementPoints.Add(Building.EntranceMovementPoint);
            MovementPoints.Add(Building.SpecialMovementPoints[0]);
            
            InteractionEntryId = Building.WorkOrderNPCEntryId;
            Criteria += () => BaseSettings.CurrentSettings.BehaviorSalvaging &&
                             !Building.CanActivate && !Building.IsBuilding &&
                             SalvagableGoods.Count > 0;
        }
        public override void Initalize()
        {
            _movement = new Movement(Building.SpecialMovementPoints[0], 2.5f);
            base.Initalize();
        }


        public Building Building { get; set; }

        public override async Task<bool> BehaviorRoutine()
        {
            if (await base.BehaviorRoutine()) return true;
            if (IsDone) return false;

            if (await StartMovement.MoveTo()) return true;

            if (await Interaction()) return true;

            if (await EndMovement.MoveTo()) return true;

            return false;
        }

        private List<C_WoWItem> SalvagableGoods
        {
            get
            {
                return Character.Player.Inventory.GetBagItemsById(114116, 114120, 114119);
            }
        }


        private Movement _movement;

        public override async Task<bool> Interaction()
        {
            if (await _movement.ClickToMove())
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