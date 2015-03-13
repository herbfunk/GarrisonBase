using System.IO;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Herbfunk.GarrisonBase.Cache;
using Styx;
using Styx.CommonBot.Coroutines;
using Styx.WoWInternals;

namespace Herbfunk.GarrisonBase
{
    public partial class Behaviors
    {
        public class BehaviorPrechecks : Behavior
        {
            public override BehaviorType Type { get { return BehaviorType.None; } }

            public BehaviorPrechecks()
            {
                
            }
          


            public override async Task<bool> BehaviorRoutine()
            {
                if (IsDone) return false;

                if (await CheckHearthStone())
                    return true;

                return false;
            }


            private async Task<bool> CheckHearthStone()
            {
                //<WoWItem Name="Garrison Hearthstone" Entry="110560" />
                if (!StyxWoW.Me.CurrentMap.IsGarrison)
                {
                    if (!StyxWoW.Me.IsCasting)
                    {
                        var hearthstone = Player.Inventory.GarrisonHearthstone;
                        if (hearthstone != null)
                        {
                            if (hearthstone.ref_WoWItem.CooldownTimeLeft.TotalSeconds > 0)
                            {
                                GarrisonBase.Log("Garrison Hearthstone on cooldown! {0} seconds left",
                                    hearthstone.ref_WoWItem.CooldownTimeLeft.TotalSeconds);
                                return true;
                            }

                            if (StyxWoW.Me.IsMoving)
                                WoWMovement.MoveStop();
                            await Coroutine.Wait(1000, () => !StyxWoW.Me.IsMoving);

                            if (hearthstone.ref_WoWItem.Usable)
                            {
                                hearthstone.Use();
                            }

                            if (!await Coroutine.Wait(60000, () => CacheStaticLookUp.GarrisonsZonesId.Contains(StyxWoW.Me.ZoneId)))
                            {
                                GarrisonBase.Err("Used garrison hearthstone but not in garrison yet.");
                                return false;
                            }
                        }
                    }

                    await Coroutine.Sleep(StyxWoW.Random.Next(1232, 3323));
                    return true;
                }


                IsDone = true;
                return false;
            }

        }
    }
}
