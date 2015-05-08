using System.Collections.Generic;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Herbfunk.GarrisonBase.Cache;
using Herbfunk.GarrisonBase.Cache.Objects;
using Herbfunk.GarrisonBase.Character;
using Herbfunk.GarrisonBase.Garrison;
using Herbfunk.GarrisonBase.Garrison.Enums;
using Styx;
using Styx.CommonBot;
using Styx.CommonBot.Coroutines;
using Styx.Pathing;
using Styx.WoWInternals;

namespace Herbfunk.GarrisonBase.Coroutines
{
    public static class DisenchantBehavior
    {
        private const int DisenchantSpellId = 13262;

        internal static bool IsDisenchanting { get; set; }
        private static C_WoWGameObject _disenchantobject;
        private static Movement _disenchantMovement;
        private static bool _checkedDisenchantingSkill = false;
        private static bool _usingDisenchantingSkill = false;
        internal static bool ShouldDisenchant
        {
            get
            {
                return 
                    (BaseSettings.CurrentSettings.BehaviorDisenchanting &&
                        (GarrisonManager.Initalized &&
                        GarrisonManager.HasDisenchant && 
                        Player.InsideGarrison &&
                        GetDisenchantForgeItems.Count > 0)
                        ||
                        (BaseSettings.CurrentSettings.DisenchantingProfessionSkill &&
                        Player.Professions.ProfessionSkills.ContainsKey(SkillLine.Enchanting) &&
                        GetDisenchantingItems.Count>0));
            }
        }

        internal static async Task<bool> Disenchanting()
        {
            if (IsDisenchanting)
            {
                if (!_checkedDisenchantingSkill)
                {
                    _checkedDisenchantingSkill = true;
                    _usingDisenchantingSkill = 
                                    Player.Professions.ProfessionSkills.ContainsKey(SkillLine.Enchanting) &&
                                    GetDisenchantingItems.Count > 0;
                }

                if (_usingDisenchantingSkill)
                {
                    var items = GetDisenchantingItems;
                    if (items.Count == 0)
                    {
                        if (await Common.CloseFrames()) return true;
                        ResetDisenchanting();
                        return true;
                    }

                    await DisenchantInteraction(items);
                    return true;
                }

                #region Garrison Disenchanting

                var forgeItems = GetDisenchantForgeItems;
                if (forgeItems.Count == 0)
                {
                    if (await Common.CloseFrames()) return true;
                    ResetDisenchanting();
                    return true;
                }

                if (_disenchantobject == null)
                {
                    var disenchantObjects = DisenchantingForgeObjects;
                    if (disenchantObjects.Count > 0)
                    {
                        _disenchantobject = disenchantObjects[0];
                        GarrisonBase.Debug("Disenchant Behavior found object at {0}", _disenchantobject.Location);
                    }
                }

                if (_disenchantMovement == null || _disenchantMovement.CurrentMovementQueue.Count == 0)
                {
                    if (_disenchantobject == null)
                    {
                        var mailboxWoWPoint = GarrisonManager.Buildings[BuildingType.EnchantersStudy].EntranceMovementPoint;
                        _disenchantMovement = new Movement(new[] { mailboxWoWPoint }, 20f, "Enchanters Study Entrance");
                    }
                    else
                    {
                        _disenchantMovement = new Movement(_disenchantobject, _disenchantobject.InteractRange - 0.25f, "Disenchant Object Movement");
                    }
                }


                if (_disenchantobject != null)
                {
                    if (_disenchantobject.WithinInteractRange)
                    {
                        bool disenchanting = await DisenchantForgeInteraction(_disenchantobject, forgeItems);
                        if (!disenchanting)
                        {
                            ResetDisenchanting();
                            return true;
                        }

                        return true;
                    }
                }

                if (_disenchantMovement != null)
                {
                    bool movement = await DisenchantMovement();
                    if (!movement)
                    {
                        //Failed to move?
                        GarrisonBase.Debug("Failed to move to disenchant location!");
                        return false;
                    }
                }

                return true;
                
                #endregion
            }

            return false;
        }

        private static async Task<bool> DisenchantMovement()
        {
            TreeRoot.StatusText = "Disenchanting Movement Behavior";
            var result = await _disenchantMovement.MoveTo_Result();

            if (result == MoveResult.Failed)
            {
                GarrisonBase.Debug("Behavior Disenchant Movement FAILED!");
                _disenchantMovement = null;
                return false;
            }

            return true;
        }
        
        private static async Task<bool> DisenchantForgeInteraction(C_WoWGameObject disenchantobject, List<C_WoWItem> items)
        {
            if (disenchantobject.GetCursor != WoWCursorType.InteractCursor)
            {
                GarrisonBase.Debug("Disenchant Interaction failed -- Cursor was not InteractCursor!");
                return false;
            }

            if (Player.CurrentPendingCursorSpellId == 160201)
            {
                //Item Interaction!
                GarrisonBase.Log("Disenchant Cursor!");

                if (items.Count > 0)
                {
                    var item = items[0];
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

            bool cursorChanged = await CommonCoroutines.WaitForLuaEvent(
                "CURRENT_SPELL_CAST_CHANGED",
                StyxWoW.Random.Next(555, 2002),
                null,
                disenchantobject.Interact);


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
            }


            return true;
        }

        private static async Task<bool> DisenchantInteraction(List<C_WoWItem> items)
        {

            if (Player.CurrentPendingCursorSpellId == DisenchantSpellId)
            {
                //Item Interaction!
                GarrisonBase.Log("Disenchant Cursor!");

                if (items.Count > 0)
                {
                    var item = items[0];
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

            var spell = DisenchantSpell;
            if (spell != null)
            {
                bool cursorChanged = await CommonCoroutines.WaitForLuaEvent(
                    "CURRENT_SPELL_CAST_CHANGED",
                    StyxWoW.Random.Next(555, 2002),
                    null,
                    spell.Cast);

                await CommonCoroutines.SleepForRandomUiInteractionTime();
                await Coroutine.Yield();
                await Coroutine.Sleep(StyxWoW.Random.Next(1222, 2222));
            }
            else
            {
                GarrisonBase.Err("Disenchanting Spell returned null!");
                return false;
            }

            


            return true;
        }

        internal static void ResetDisenchanting()
        {
            _disenchantMovement = null;
            IsDisenchanting = false;
            _disenchantobject = null;
            _checkedDisenchantingSkill = false;
            _usingDisenchantingSkill = false;
        }

        private static List<C_WoWItem> GetDisenchantForgeItems
        {
            get { return Player.Inventory.GetBagDisenchantingForgeItems(); }
        }
        private static List<C_WoWItem> GetDisenchantingItems
        {
            get { return Player.Inventory.GetBagDisenchantingItems(); }
        }
        private static List<C_WoWGameObject> DisenchantingForgeObjects
        {
            get { return ObjectCacheManager.GetWoWGameObjects(GarrisonManager.DisenchantingEntryId); }
        }
        private static WoWSpell DisenchantSpell
        {
            get { return WoWSpell.FromId(DisenchantSpellId); }
        }
       

    }
}
