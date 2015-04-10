using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Herbfunk.GarrisonBase.Cache;
using Herbfunk.GarrisonBase.Cache.Objects;
using Herbfunk.GarrisonBase.Character;
using Herbfunk.GarrisonBase.Garrison;
using Herbfunk.GarrisonBase.Garrison.Enums;
using Styx;
using Styx.CommonBot.Coroutines;

namespace Herbfunk.GarrisonBase.Coroutines.Behaviors
{
    public class BehaviorDisenchant : Behavior
    {
        public override BehaviorType Type { get { return BehaviorType.Disenchanting; } }

        public BehaviorDisenchant()
            : base()
        {
            Criteria += () => BaseSettings.CurrentSettings.BehaviorDisenchanting &&
                             GarrisonManager.HasDisenchant &&
                             !GarrisonManager.Buildings[BuildingType.EnchantersStudy].IsBuilding &&
                             !GarrisonManager.Buildings[BuildingType.EnchantersStudy].CanActivate &&
                             Character.Player.Inventory.DisenchantItems.Count > 0;

            RunCondition += () => BaseSettings.CurrentSettings.BehaviorDisenchanting && 
                                Character.Player.Inventory.DisenchantItems.Count > 0;
        }

        public override void Initalize()
        {
            ObjectCacheManager.ShouldKill = false;
            ObjectCacheManager.ShouldLoot = false;
            MovementPoints.Add(GarrisonManager.Buildings[BuildingType.EnchantersStudy].SafeMovementPoint);
            MovementPoints.Add(GarrisonManager.Buildings[BuildingType.EnchantersStudy].EntranceMovementPoint);
            InteractionEntryId = GarrisonManager.DisenchantingEntryId;

            _movement = null;
            base.Initalize();
        }


        public C_WoWGameObject DisenchantingObject
        {
            get { return ObjectCacheManager.GetWoWGameObjects(GarrisonManager.DisenchantingEntryId).FirstOrDefault(); }
        }

        public override async Task<bool> BehaviorRoutine()
        {
            if (await base.BehaviorRoutine()) return true;
            if (IsDone) return false;

            if (await StartMovement.MoveTo()) return true;

            if (await Movement()) return true;

            if (await Interaction()) return true;

            if (await EndMovement.MoveTo())
                return true;

            return false;
        }

        public override async Task<bool> Movement()
        {

            if (DisenchantingObject == null)
            {
                //Error Cannot find object!
                IsDone = true;
                return true;
            }


            if (DisenchantingObject.GetCursor == WoWCursorType.InteractCursor)
            {
                return false;
            }

            //Move to the interaction object (within 6.7f)
            if (_movement == null)
                _movement = new Movement(DisenchantingObject.Location, 6.7f);
                
            return await _movement.ClickToMove();
        }
        private Movement _movement;

        public override async Task<bool> Interaction()
        {


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
                   
                if (Player.Inventory.DisenchantItems.Count > 0)
                {
                    var item = Player.Inventory.DisenchantItems[0];
                    GarrisonBase.Debug("Disenchanting Item {0} ({1}) Quality {2}", item.Name, item.Entry, item.Quality);
                    bool bagChanged = await CommonCoroutines.WaitForLuaEvent(
                        "BAG_UPDATE",
                        6200,
                        null,
                        item.Use);

                    PlayerInventory.ItemDisenchantingBlacklistedGuids.Add(item.Guid);

                    //Force update if bag didn't change.. (so we ignore this item now)
                    if (!bagChanged)
                        Player.Inventory.UpdateBagItems();

                    return true;
                }
                return false;
            }

            bool cursorChanged=await CommonCoroutines.WaitForLuaEvent(
                "CURRENT_SPELL_CAST_CHANGED", 
                StyxWoW.Random.Next(555,2002), 
                null,
                DisenchantingObject.Interact);


            await CommonCoroutines.SleepForRandomUiInteractionTime();
            await Coroutine.Yield();
            await Coroutine.Sleep(StyxWoW.Random.Next(1222, 2222));

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


    }
}