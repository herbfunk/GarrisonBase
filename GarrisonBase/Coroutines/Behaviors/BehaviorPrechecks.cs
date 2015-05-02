using System.Threading.Tasks;
using Buddy.Coroutines;
using Herbfunk.GarrisonBase.Character;
using Herbfunk.GarrisonBase.Garrison;
using Styx;
using Styx.CommonBot.Coroutines;
using Styx.WoWInternals;

namespace Herbfunk.GarrisonBase.Coroutines.Behaviors
{
    public class BehaviorPrechecks : Behavior
    {
        public override BehaviorType Type { get { return BehaviorType.None; } }
        internal bool DisabledMasterPlanAddon { get; set; }
        internal bool ShouldCheckAddons { get; set; }
        internal bool IgnoreHearthing { get; set; }
        public BehaviorPrechecks()
        {
            IgnoreHearthing = true;
        }



        public override async Task<bool> BehaviorRoutine()
        {
            if (IsDone) return false;

            if (!VerifyContinentIsDraenor())
                IgnoreHearthing = false;
            else if (StyxWoW.Me.CurrentMap.IsGarrison)
                IgnoreHearthing = true;

            if (!IgnoreHearthing && !BaseSettings.CurrentSettings.DEBUG_IGNOREHEARTHSTONE && await CheckHearthStone())
                return true;

            return false;
        }

        private bool attemptedFlightPath = false;
        private BehaviorUseFlightPath FlightPathBehavior;
        private Movement MovementBehavior;
        private async Task<bool> CheckHearthStone()
        {
            if (await Common.CheckCommonCoroutines()) return true;

            if (FlightPathBehavior != null && !FlightPathBehavior.IsDone && await FlightPathBehavior.BehaviorRoutine())
                return true;

            if (MovementBehavior != null && MovementBehavior.CurrentMovementQueue.Count > 0 && await MovementBehavior.MoveTo())
                return true;
            

            if (!StyxWoW.Me.IsCasting)
            {
                var hearthstone = Player.Inventory.GarrisonHearthstone;
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
                        await Coroutine.Yield();
                        if (!await Coroutine.Wait(25000, () => Player.InsideGarrison))
                        {
                            GarrisonBase.Err("Used garrison hearthstone but not in garrison yet.");
                            return false;
                        }
                    }
                    else
                    {
                        if (Player.MapExpansionId != 5)
                        {
                            //Not in draenor!
                            GarrisonBase.Err("Garrison Hearthstone is on cooldown and not currently in draenor!");
                            return false;
                        }

                        if (!attemptedFlightPath)
                        {
                            GarrisonBase.Debug("Attempting flight path to garrison");
                            FlightPathBehavior = new BehaviorUseFlightPath(MovementCache.GarrisonEntrance);
                            attemptedFlightPath = true;
                        }
                        else
                        {
                            GarrisonBase.Debug("Attempting movement to garrison");
                            if (MovementBehavior == null)
                                MovementBehavior = new Movement(MovementCache.GarrisonEntrance, 50f, name: "GarrisonMovement");

                        }

                        return true;
                    }
                }
            }

            await Coroutine.Sleep(StyxWoW.Random.Next(1232, 3323));
            return true;
        }

        internal bool VerifyContinentIsDraenor()
        {
            var playerMapId = !Player.MapIsContinent
                   ? Player.ParentMapId
                   : (int)Player.MapId.Value;

            return playerMapId == 1116;
        }

        //private bool _checkedMasterPlanAddon = false;
        //internal async Task<bool> CheckAddons()
        //{
        //    if (_checkedMasterPlanAddon) return false;
        //    if (DisabledMasterPlanAddon) return false;

        //    await Coroutine.Yield();
        //    await Coroutine.Sleep(StyxWoW.Random.Next(1200, 2522));
        //    if (!BaseSettings.CurrentSettings.DisableMasterPlanAddon || !LuaCommands.IsAddonLoaded("MasterPlan"))
        //    {
        //        _checkedMasterPlanAddon = true;
        //        return false;
        //    }

        //    LuaCommands.DisableAddon("MasterPlan");
        //    LuaCommands.ReloadUI();
        //    DisabledMasterPlanAddon = true;
        //    _checkedMasterPlanAddon = true;
        //    await Coroutine.Wait(6000, () => !StyxWoW.IsInGame);
        //    return true;
        //}

        internal async Task<bool> InitalizeGarrisonManager()
        {
            if (GarrisonManager.Initalized) return false;

            await CommonCoroutines.WaitForLuaEvent("GARRISON_SHOW_LANDING_PAGE", 2500, null, LuaCommands.ClickGarrisonMinimapButton);
            await CommonCoroutines.SleepForRandomUiInteractionTime();
            await Coroutine.Sleep(StyxWoW.Random.Next(1234, 2331));
            Lua.DoString("GarrisonLandingPage.CloseButton:Click()");
            await CommonCoroutines.SleepForRandomUiInteractionTime();

            GarrisonManager.Initalize();
            return true;
        }

    }
}