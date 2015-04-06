using System;
using System.Threading.Tasks;
using Herbfunk.GarrisonBase.Garrison;
using Styx;
using Styx.CommonBot.Coroutines;
using Styx.CommonBot.Frames;

namespace Herbfunk.GarrisonBase.Coroutines.Behaviors
{
    public class BehaviorSellRepair : Behavior
    {
        public override BehaviorType Type { get { return BehaviorType.SellRepair; } }

        public BehaviorSellRepair()
            : base(MovementCache.SellRepairNpcLocation, GarrisonManager.SellRepairNpcId)
        {
            Criteria += () => BaseSettings.CurrentSettings.BehaviorRepairSell && 
                Character.Player.Inventory.GetBagItemsVendor().Count > 0;
        }


        public override void Initalize()
        {
            _npcMovement = null;
            base.Initalize();
        }

        private Movement _npcMovement;
        public override async Task<bool> BehaviorRoutine()
        {
            if (await base.BehaviorRoutine()) return true;
            if (IsDone) return false;
                

            if (InteractionObject == null || !InteractionObject.IsValid || InteractionObject.Collision || !InteractionObject.LineOfSight)
            {
                if (await StartMovement.MoveTo()) return true;
            }

            if (InteractionObject == null)
            {
                //Could not find object!
                return false;
            }

            if (!InteractionObject.WithinInteractRange)
            {
                if (_npcMovement == null)
                    _npcMovement = new Movement(InteractionObject.Location, InteractionObject.InteractRange - 0.25f);

                await _npcMovement.ClickToMove(false);
                return true;
            }



            if (!LuaEvents.MerchantFrameOpen)
            {
                if (!StyxWoW.Me.IsMoving)
                    InteractionObject.Interact();
                    
                return true;
            }

            var poorQualityItems = Character.Player.Inventory.GetBagItemsVendor();
            foreach (var item in poorQualityItems)
            {
                MerchantFrame.Instance.SellItem(item.ref_WoWItem);
                await CommonCoroutines.SleepForRandomUiInteractionTime();
                // await CommonCoroutines.SleepForLagDuration();
                // await Coroutine.Sleep(StyxWoW.Random.Next(256, 712));
            }

            return false;
        }
    }
}