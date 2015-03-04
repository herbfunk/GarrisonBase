using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Herbfunk.GarrisonBase.Cache;
using Herbfunk.GarrisonBase.Garrison;
using Herbfunk.GarrisonBase.Garrison.Enums;
using Styx;
using Styx.CommonBot;
using Styx.CommonBot.Coroutines;
using Styx.Helpers;

namespace Herbfunk.GarrisonBase
{
    public partial class Behaviors
    {
        public class BehaviorDisenchant : Behavior
        {
            public override BehaviorType Type { get { return BehaviorType.Disenchanting; } }

            public BehaviorDisenchant()
                : base(
                    new []{GarrisonManager.Buildings[BuildingType.EnchantersStudy].SafeMovementPoint, 
                           GarrisonManager.Buildings[BuildingType.EnchantersStudy].EntranceMovementPoint},
                    GarrisonManager.DisenchantingEntryId)
            {
                _finalmovement = new Movement(GarrisonManager.Buildings[BuildingType.EnchantersStudy].SafeMovementPoint, 3f);
            }
            public override Func<bool> Criteria
            {
                get
                {
                    return () => BaseSettings.CurrentSettings.BehaviorDisenchanting &&
                        Player.Inventory.GetBagItemsDisenchantable().Count > 0;
                }
            }
            public C_WoWGameObject GarrisonResourceCacheObject
            {
                get { return ObjectCacheManager.GetWoWGameObjects(GarrisonManager.DisenchantingEntryId).FirstOrDefault(); }
            }
            public override async Task<bool> Movement()
            {
                if (GarrisonResourceCacheObject != null)
                {
                    if (GarrisonResourceCacheObject.GetCursor == WoWCursorType.InteractCursor)
                    {
                        return false;
                    }
                }

                TreeRoot.StatusText = String.Format("Behavior {0} Movement", Type.ToString());
                if (await base.Movement()) return true;

                TreeRoot.StatusText = String.Format("Behavior {0} Movement2", Type.ToString());
                if (GarrisonResourceCacheObject == null)
                {
                    //Error Cannot find object!
                    IsDone = true;
                    return true;
                }

                //Move to the interaction object (within 6.7f)
                if (_movement == null)
                {
                    _movement = new Movement(GarrisonResourceCacheObject.Location, 6.7f);
                }

                return await _movement.MoveTo();
            }
            private Movement _movement;
            private readonly Movement _finalmovement;
            public override async Task<bool> Interaction()
            {
                var items = Player.Inventory.GetBagItemsDisenchantable();
                if (items.Count == 0) return false;

                /*
                 * Check Cursor Spell ID
                 * Click Interaction
                 * Await for event CURRENT_SPELL_CAST_CHANGED
                 *      -If event did not fire then check for confirmation popup
                 * 
                 * Item Interaction
                 * 
                 * 
                 */

                if (Player.CurrentPendingCursorSpellId == 160201)
                {
                    //Item Interaction!
                    GarrisonBase.Log("Disenchant Cursor!");
                   
                    if (items.Count > 0)
                    {
                        var item = items[0];
                        GarrisonBase.Log("Disenchanting {0}", item.Name);
                        bool bagChanged = await CommonCoroutines.WaitForLuaEvent(
                                            "BAG_UPDATE",
                                            6200,
                                            null,
                                            item.Use);

                        Player.Inventory.ItemDisenchantingBlacklistedGuids.Add(item.Guid);
                        return true;
                    }
                    return false;
                }

                bool cursorChanged=await CommonCoroutines.WaitForLuaEvent(
                                            "CURRENT_SPELL_CAST_CHANGED", 
                                            StyxWoW.Random.Next(555,2002), 
                                            null,
                                            GarrisonResourceCacheObject.Interact);


                await CommonCoroutines.SleepForRandomUiInteractionTime();
                await Coroutine.Yield();


                if (!cursorChanged)
                {
                    if (LuaCommands.IsStaticPopupVisible())
                    {
                        LuaCommands.ClickStaticPopupButton(1);
                        await CommonCoroutines.SleepForRandomUiInteractionTime();
                        return true;
                    }
                    GarrisonBase.Err("Could not find static popup confirmation frame!");
                }

                    
                return true;
            }

            public override async Task<bool> BehaviorRoutine()
            {
                if (IsDone) return false;

                if (await base.BehaviorRoutine()) return true;

                if (await Movement())
                    return true;

                if (await Interaction())
                    return true;

                if (await _finalmovement.MoveTo())
                    return true;

                return false;
            }
        }
    }
}
