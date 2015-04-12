using System.Linq;
using System.Threading.Tasks;
using Bots.Quest;
using Buddy.Coroutines;
using Herbfunk.GarrisonBase.Character;
using Herbfunk.GarrisonBase.Garrison;
using Herbfunk.GarrisonBase.Helpers;
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
            RunCondition += () =>
                    BaseSettings.CurrentSettings.ExchangePrimalSpirits &&
                    ExchangeItemInfo != null &&
                    ExchangeItemInfo.Cost <= TotalPrimalSpiritCount;

            Criteria += RunCondition;
        }


        private Movement _npcMovement;
        public override async Task<bool> BehaviorRoutine()
        {
            if (await base.BehaviorRoutine()) return true;
            if (IsDone) return false;

            if (InteractionObject == null)
                if (await StartMovement.MoveTo()) return true;



            if (GossipHelper.IsOpen)
            {
                if (GossipHelper.GossipOptions.All(o => o.Type != GossipEntry.GossipEntryType.Vendor))
                {
                    //Could not find Vendor Option!
                    return false;
                }
                var gossipEntryVendor = GossipHelper.GossipOptions.FirstOrDefault(o => o.Type == GossipEntry.GossipEntryType.Vendor);

                QuestManager.GossipFrame.SelectGossipOption(gossipEntryVendor.Index);
                await CommonCoroutines.SleepForRandomUiInteractionTime();
                return true;
            }

            if (MerchantHelper.IsOpen)
            {
                await Coroutine.Yield();

                if (ExchangeItemInfo.Cost <= TotalPrimalSpiritCount)
                {
                    if (StyxWoW.Me.IsMoving)
                        await CommonCoroutines.StopMoving();

                    await Coroutine.Sleep(StyxWoW.Random.Next(1005, 1666));
                    bool success = false;

                    await CommonCoroutines.WaitForLuaEvent("BAG_UPDATE",
                        StyxWoW.Random.Next(1255, 1777),
                        null,
                        () => success = MerchantHelper.BuyItem(ExchangeItemInfo.ItemId, 1, true));

                    await CommonCoroutines.SleepForRandomUiInteractionTime();
                    await Coroutine.Yield();

                    if (success) return true;
                }

                IsDone = true;
                return false;
            }

            if (InteractionObject != null)
            {
                if (InteractionObject.WithinInteractRange)
                {
                    if (StyxWoW.Me.IsMoving) await CommonCoroutines.StopMoving();
                    await CommonCoroutines.SleepForLagDuration();
                    InteractionObject.Interact();
                    await CommonCoroutines.SleepForRandomUiInteractionTime();
                    return true;
                }

                if (_npcMovement == null)
                    _npcMovement = new Movement(InteractionObject.Location, InteractionObject.InteractRange - 0.25f);

                await _npcMovement.MoveTo(false);
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
                        i => i.ItemId == BaseSettings.CurrentSettings.PrimalSpiritItemId);
            }
        }

        private int TotalPrimalSpiritCount
        {
            get
            {
                var count = 0;

                foreach (var cWoWItem in Character.Player.Inventory.GetCraftingReagentsById(PlayerInventory.PrimalSpiritEntryId))
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