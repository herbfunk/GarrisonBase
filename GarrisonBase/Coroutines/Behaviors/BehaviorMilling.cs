using System;
using System.Linq;
using System.Threading.Tasks;
using Herbfunk.GarrisonBase.Character;
using Styx;
using Styx.CommonBot.Coroutines;
using Styx.WoWInternals;

namespace Herbfunk.GarrisonBase.Coroutines.Behaviors
{
    public class BehaviorMilling : Behavior
    {
        public BehaviorMilling()
        {

            Criteria += () => BaseSettings.CurrentSettings.MillingEnabled && StyxWoW.Me.KnowsSpell(51005);
            RunCondition += () => ItemCount < BaseSettings.CurrentSettings.MillingMinimum;
        }

        private int ItemCount
        {
            get
            {
                return Player.Inventory.GetBagItemsById((int)CraftingReagents.CeruleanPigment).Sum(item => Convert.ToInt32(item.StackCount));
            }
        }

        public override async Task<bool> BehaviorRoutine()
        {
            if (await base.BehaviorRoutine()) return true;
            if (IsDone) return false;

            var millingHerbs = Player.Inventory.GetBagItemsMilling();
            if (millingHerbs.Count == 0)
            {
                IsDone = true;
                return false;
            }

            if (Player.CurrentPendingCursorSpellId != 51005)
            {
                if (!MillingSpell.Cooldown && MillingSpell.CanCast)
                {
                    await CommonCoroutines.WaitForLuaEvent(
                        "CURRENT_SPELL_CAST_CHANGED",
                        10000,
                        () => false,
                        MillingSpell.Cast);
                }
                await CommonCoroutines.SleepForRandomUiInteractionTime();
                return true;
            }

            await CommonCoroutines.WaitForLuaEvent(
                "BAG_UPDATE",
                10000,
                () => false,
                millingHerbs[0].Use);

            await CommonCoroutines.SleepForRandomUiInteractionTime();
            return true;
        }


        private WoWSpell MillingSpell
        {
            get { return WoWSpell.FromId(51005); }
        }

    }
}
