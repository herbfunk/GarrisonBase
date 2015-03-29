using System.Collections.Generic;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Styx.CommonBot.Coroutines;
using Styx.CommonBot.Frames;

namespace Herbfunk.GarrisonBase.Coroutines
{
    public partial class Common
    {
        public static async Task<bool> CheckLootFrame()
        {
            // loot everything.
            if (!LuaEvents.LootOpened) return false;

            var lootSlotInfos = new List<LootSlotInfo>();
            for (int i = 0; i < LootFrame.Instance.LootItems; i++)
            {
                lootSlotInfos.Add(LootFrame.Instance.LootInfo(i));
            }

            if (await Coroutine.Wait(2000, () =>
            {
                LootFrame.Instance.LootAll();
                return !LuaEvents.LootOpened;
            }))
            {
                GarrisonBase.Log("Succesfully looted: ");
                foreach (LootSlotInfo lootinfo in lootSlotInfos)
                {
                    try
                    {
                        string lootQuanity = lootinfo.LootQuantity.ToString();
                        string lootName = lootinfo.LootName;
                        GarrisonBase.Log(lootQuanity + "x " + lootName);
                    }
                    catch
                    {
                        GarrisonBase.Log("exception occured");
                    }

                }
            }
            else
            {
                GarrisonBase.Err("Failed to loot from Frame.");
            }
            await CommonCoroutines.SleepForLagDuration();
            return true;
        }

    }
}
