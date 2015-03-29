using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Herbfunk.GarrisonBase.Cache.Objects;
using Styx;
using Styx.CommonBot;
using Styx.CommonBot.Coroutines;

namespace Herbfunk.GarrisonBase.Coroutines.Behaviors
{
    public class BehaviorItemInteraction : Behavior
    {
        public BehaviorItemInteraction(int itemEntryId, bool doOnce=false)
        {
            DoOnce = doOnce;
            ItemEntryIDs.Add(itemEntryId);
        }
        public BehaviorItemInteraction(int[] itemEntryIds, bool doOnce = false)
        {
            DoOnce = doOnce;
            foreach (var id in itemEntryIds)
            {
                ItemEntryIDs.Add(id);
            }
        }

        private bool DoOnce = false;
        private bool DoneOnce = false;

        private List<int> ItemEntryIDs = new List<int>();
        private List<C_WoWItem> Items
        {
            get
            {

                return Character.Player.Inventory.GetBagItemsById(ItemEntryIDs.ToArray());
            }
        }


        public override async Task<bool> BehaviorRoutine()
        {
            if (await base.BehaviorRoutine()) return true;
            if (IsDone) return false;

            if ((DoOnce && DoneOnce) || Items.Count == 0)
                return false;

            foreach (C_WoWItem item in Items)
            {
                TreeRoot.StatusText = String.Format("Behavior Item Interaction {0}", item.Name);
                    
                item.Interact();
                await CommonCoroutines.SleepForRandomUiInteractionTime();
                await Coroutine.Wait(5000, () => !StyxWoW.Me.IsCasting);
                await Coroutine.Yield();
                await Coroutine.Sleep(1250);
            }

            DoneOnce = true;

            return false;
        }
    }
}