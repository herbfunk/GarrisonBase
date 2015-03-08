using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Herbfunk.GarrisonBase.Cache;
using Herbfunk.GarrisonBase.Garrison;
using Styx;
using Styx.CommonBot.Coroutines;
using Styx.CommonBot.Frames;

namespace Herbfunk.GarrisonBase
{
    public partial class Behaviors
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

                    foreach (var cWoWItem in Player.Inventory.GetBagItemsById(PlayerInventory.PrimalSpiritEntryId))
                    {
                        count += (int)cWoWItem.StackCount;
                    }
                    foreach (var cWoWItem in Player.Inventory.GetReagentBankItemsById(PlayerInventory.PrimalSpiritEntryId))
                    {
                        count += (int)cWoWItem.StackCount;
                    }

                    return count;
                }
            }

            public override async Task<bool> BehaviorRoutine()
            {
                if (IsDone) return false;

                if (await base.BehaviorRoutine()) return true;

                if (await base.Movement()) return true;

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
        }
    }
}
