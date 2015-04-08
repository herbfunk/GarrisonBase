using System.Threading.Tasks;
using Buddy.Coroutines;
using Styx;
using Styx.CommonBot.Coroutines;

namespace Herbfunk.GarrisonBase.Coroutines.Behaviors
{
    public class BehaviorPrechecks : Behavior
    {
        public override BehaviorType Type { get { return BehaviorType.None; } }
        internal bool DisabledMasterPlanAddon { get; set; }
        public BehaviorPrechecks()
        {
                
        }
          


        public override async Task<bool> BehaviorRoutine()
        {
            if (IsDone) return false;

            if (await CheckHearthStone())
                return true;

            if (await CheckAddons())
                return true;

            IsDone = true;

            return false;
        }

        private bool attemptedFlightPath = false;
        private BehaviorUseFlightPath FlightPathBehavior;
        private async Task<bool> CheckHearthStone()
        {
            //<WoWItem Name="Garrison Hearthstone" Entry="110560" />
            if (StyxWoW.Me.CurrentMap.IsGarrison) return false;

            if (FlightPathBehavior != null && !FlightPathBehavior.IsDone && await FlightPathBehavior.BehaviorRoutine())
                return true;

            if (!StyxWoW.Me.IsCasting)
            {
                var hearthstone = Character.Player.Inventory.GarrisonHearthstone;
                if (hearthstone != null)
                {

                    bool oncooldown = false;
                    var cooldownTimeLeft = hearthstone.ref_WoWItem.CooldownTimeLeft;
                    if (cooldownTimeLeft.TotalSeconds > 0)
                    {
                        oncooldown = true;
                        GarrisonBase.Log("Garrison Hearthstone on cooldown! {0} seconds left",
                            cooldownTimeLeft.TotalSeconds);

                        if (cooldownTimeLeft.TotalSeconds < 60)
                        {
                            await Coroutine.Sleep((int)cooldownTimeLeft.TotalMilliseconds);
                            return true;
                        }
                    }



                    if (!oncooldown && hearthstone.ref_WoWItem.Usable)
                    {
                        if (StyxWoW.Me.IsMoving) await CommonCoroutines.StopMoving();
                        hearthstone.Use();
                        await CommonCoroutines.SleepForRandomUiInteractionTime();
                        await Coroutine.Wait(10000, () => StyxWoW.Me.IsCasting);
                        if (!await Coroutine.Wait(25000, () => CacheStaticLookUp.GarrisonsZonesId.Contains(StyxWoW.Me.ZoneId)))
                        {
                            GarrisonBase.Err("Used garrison hearthstone but not in garrison yet.");
                            return false;
                        }
                    }
                    else
                    {
                        if (StyxWoW.Me.CurrentMap.ExpansionId != 5)
                        {
                            //Not in draenor!
                            GarrisonBase.Err("Garrison Hearthstone is on cooldown and not currently in draenor!");
                            return false;
                        }

                        if (!attemptedFlightPath)
                        {
                            FlightPathBehavior = new BehaviorUseFlightPath("Frostwall Garrison", new[] { "Lunarfall" });
                            attemptedFlightPath = true;
                        }
                                
                        return true;
                    }
                }
            }

            await Coroutine.Sleep(StyxWoW.Random.Next(1232, 3323));
            return true;
        }

        private async Task<bool> CheckAddons()
        {
            await Coroutine.Yield();
            await Coroutine.Sleep(StyxWoW.Random.Next(1200, 2522));
            if (!LuaCommands.IsAddonLoaded("MasterPlan")) return false;
            
            LuaCommands.DisableAddon("MasterPlan");
            LuaCommands.ReloadUI();
            DisabledMasterPlanAddon = true;
            await Coroutine.Wait(6000, () => StyxWoW.IsInGame);
            return true;
        }

    }
}