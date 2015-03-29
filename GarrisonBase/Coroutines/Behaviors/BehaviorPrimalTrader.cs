using System;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Herbfunk.GarrisonBase.Character;
using Herbfunk.GarrisonBase.Garrison;
using Styx;
using Styx.CommonBot.Coroutines;
using Styx.CommonBot.Frames;

namespace Herbfunk.GarrisonBase.Coroutines.Behaviors
{
    public class BehaviorPrimalTrader : Behavior
    {
        public override BehaviorType Type { get { return BehaviorType.PrimalTrader; } }

        public BehaviorPrimalTrader()
            : base(GarrisonManager.PrimalTraderPoint, GarrisonManager.PrimalTraderID)
        {

        }


        public override Func<bool> Criteria
        {
            get
            {
                return () => 
                    BaseSettings.CurrentSettings.ExchangePrimalSpirits &&
                    ExchangeItemInfo!=null &&
                    ExchangeItemInfo.Cost<=TotalPrimalSpiritCount;
            }
        }

        public override async Task<bool> BehaviorRoutine()
        {
            if (await base.BehaviorRoutine()) return true;
            if (IsDone) return false;

            if (await StartMovement.MoveTo()) return true;

            if (InteractionObject == null)
            {
                //Failed!
                return false;
            }


            if (!LuaEvents.MerchantFrameOpen)
            {
                if (!StyxWoW.Me.IsMoving)
                    InteractionObject.Interact();

                return true;
            }

            if (ExchangeItemInfo.Cost <= TotalPrimalSpiritCount)
            {
                MerchantFrame.Instance.BuyItem(ExchangeItemInfo.Name, 1);
                await CommonCoroutines.SleepForRandomUiInteractionTime();
                await Coroutine.Yield();
                return true;
            }


            return false;
        }

        private GarrisonManager.PrimalTraderItem ExchangeItemInfo
        {
            get
            {
                return
                    GarrisonManager.PrimalTraderItems.FirstOrDefault(
                        i => i.Name == BaseSettings.CurrentSettings.PrimalSpiritItem);
            }
        }

        private int TotalPrimalSpiritCount
        {
            get
            {
                var count = 0;

                foreach (var cWoWItem in Character.Player.Inventory.GetBagItemsById(PlayerInventory.PrimalSpiritEntryId))
                {
                    count += (int)cWoWItem.StackCount;
                }
                //foreach (var cWoWItem in Player.Inventory.GetReagentBankItemsById(PlayerInventory.PrimalSpiritEntryId))
                //{
                //    count += (int)cWoWItem.StackCount;
                //}

                return count;
            }
        }

           
    }
}