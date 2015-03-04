using System;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Herbfunk.GarrisonBase.Cache;
using Styx;
using Styx.CommonBot.Coroutines;
using Styx.CommonBot.Frames;

namespace Herbfunk.GarrisonBase
{
    public partial class Behaviors
    {
        public class BehaviorSellRepair : Behavior
        {
            public override BehaviorType Type { get { return BehaviorType.SellRepair; } }

            public BehaviorSellRepair(int npcId, WoWPoint loc) : base(loc, npcId)
            {
                
            }
            public override Func<bool> Criteria
            {
                get { return () => BaseSettings.CurrentSettings.BehaviorRepairSell && Player.Inventory.GetBagItemsVendor().Count > 0; }
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

                var poorQualityItems = Player.Inventory.GetBagItemsVendor();
                foreach (var item in poorQualityItems)
                {
                    MerchantFrame.Instance.SellItem(item.ref_WoWItem);
                    await CommonCoroutines.SleepForRandomUiInteractionTime();
                    await CommonCoroutines.SleepForLagDuration();
                    await Coroutine.Sleep(StyxWoW.Random.Next(256, 712));
                }

                return false;
            }
        }

    }
}
