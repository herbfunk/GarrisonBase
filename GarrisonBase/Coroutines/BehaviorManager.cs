using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Herbfunk.GarrisonBase.Cache;
using Herbfunk.GarrisonBase.Character;
using Herbfunk.GarrisonBase.Coroutines.Behaviors;
using Herbfunk.GarrisonBase.Garrison;
using Herbfunk.GarrisonBase.Garrison.Enums;
using Herbfunk.GarrisonBase.Garrison.Objects;
using Herbfunk.GarrisonBase.Helpers;
using Herbfunk.GarrisonBase.TargetHandling;
using Styx;
using Styx.CommonBot;

namespace Herbfunk.GarrisonBase.Coroutines
{
    public class BehaviorManager
    {
        internal static Behavior CurrentBehavior { get; private set; }
        internal static List<Behavior> Behaviors = new List<Behavior>();


        /// <summary>
        /// Used to switch the current behavior
        /// </summary>
        internal static List<Behavior> SwitchBehaviors = new List<Behavior>();
        internal static Behavior SwitchBehavior { get; set; }

        public static async Task<bool> RootLogic()
        {

            if (!CacheStaticLookUp.InitalizedCache)
            {
                CacheStaticLookUp.Update();
                ObjectCacheManager.Initalize();
                TargetManager.Initalize();
                Movement.IgnoreTaxiCheck = false;

                //Simple Check if garrison can be accessed!
                if (Player.Level < 90 || Player.Inventory.GarrisonHearthstone == null)
                {
                    GarrisonBase.Log("No access to garrison!");
                    TreeRoot.Stop("No Access to Garrison");
                    return false;
                }
            }

            if (await Common.CheckCommonCoroutines())
                return true;

            //Do we need to hearth or fly to our garrison?
            if (await Common.PreChecks.BehaviorRoutine())
                return true;

            //Disable and reload UI if master plan is enabled..
            //if (await Common.PreChecks.CheckAddons()) return true;

            //We need "open" the garrison up and initalize it.. (so we don't get errors trying to inject!)
            if (await Common.PreChecks.InitalizeGarrisonManager()) return true;

            ////Inject our lua addon code for mission success function
            //if (!LuaEvents.LuaAddonInjected)
            //{
            //    if (LuaCommands.TestLuaInjectionCode())
            //    {//Prevent multiple injections by checking simple function return!
            //        LuaEvents.LuaAddonInjected = true;
            //    }
            //    else
            //    {
            //        await LuaEvents.InjectLuaAddon();
            //        return true;
            //    }
            //}




            if (!InitalizedBehaviorList)
                InitalizeBehaviorsList();

            //Check for next behavior
            if (CurrentBehavior == null)
            {
                while (Behaviors.Count > 0)
                {
                    if (!Behaviors[0].CheckCriteria())
                        Behaviors.RemoveAt(0);
                    else
                    {
                        CurrentBehavior = Behaviors[0];
                        CurrentBehavior.Initalize();
                        break;
                    }
                }
            }




            if (CurrentBehavior != null)
            {
                //Check for any switch behaviors.. (override current behavior temporarly)
                if (SwitchBehaviors.Count > 0 && SwitchBehavior == null)
                {
                    while (SwitchBehaviors.Count > 0)
                    {
                        if (!SwitchBehaviors[0].CheckCriteria())
                            SwitchBehaviors.RemoveAt(0);
                        else
                        {
                            SwitchBehavior = SwitchBehaviors[0];
                            break;
                        }
                    }
                }

                if (SwitchBehavior != null && CurrentBehavior != SwitchBehavior)
                {
                    GarrisonBase.Debug("Switching behaviors to {0}", SwitchBehavior.Type);
                    CurrentBehavior = SwitchBehavior;
                    CurrentBehavior.Initalize();
                }

                bool x = await CurrentBehavior.BehaviorRoutine();

                if (x && !CurrentBehavior.IsDone) return true;

                GarrisonBase.Debug(
                    !x ? "Finishing Behavior {0} because it returned false!"
                        : "Finishing Behavior {0} because IsDone is true", CurrentBehavior.Type);

                if (!CurrentBehavior.Disposed) CurrentBehavior.Dispose();

                if (SwitchBehavior != null && CurrentBehavior.Equals(SwitchBehavior))
                {
                    CurrentBehavior = Behaviors[0];
                    CurrentBehavior.Initalize();

                    SwitchBehaviors.RemoveAt(0);
                    SwitchBehavior = null;
                }
                else
                {
                    Behaviors.RemoveAt(0);
                    CurrentBehavior = null;
                }


                return true;
            }

            //if (Common.PreChecks.DisabledMasterPlanAddon)
            //{
            //    Common.PreChecks.ShouldCheckAddons = false;
            //    LuaCommands.EnableAddon("MasterPlan");
            //    LuaCommands.ReloadUI();
            //    Common.PreChecks.DisabledMasterPlanAddon = false;
            //    await Coroutine.Wait(6000, () => StyxWoW.IsInGame);
            //    return true;
            //}



            TreeRoot.StatusText = "GarrisonBase is finished!";
            TreeRoot.Stop();
            return false;
        }


        internal static bool InitalizedBehaviorList = false;
        internal static void InitalizeBehaviorsList()
        {
            GarrisonBase.Debug("Initalize Behaviors List..");
            //Insert new missions behavior at beginning!
            Behaviors.Clear();
            SwitchBehaviors.Clear();
            CurrentBehavior = null;
            SwitchBehavior = null;

            //Garrison Related!
            Behaviors.AddRange(GarrisonManager.GetGarrisonBehaviors());

            InitalizedBehaviorList = true;


        }

        internal static void Reset()
        {
            InitalizedBehaviorList = false;

            Behaviors.ForEach(b => b.IsDone = true);
            foreach (var behavior in Behaviors)
            {
                behavior.Dispose();
            }
        }


        #region Behavior Trapping Leather Shadowmoon Valley

        internal static readonly BehaviorArray BehaviorArray_Trapping_Leather_ShadowmoonVally = new BehaviorArray(new Behavior[]
        {
            //
            new BehaviorMove(new WoWPoint(1234.118, -1664.432, 44.69962), 250f),

            new BehaviorCustomAction(() =>
            {
                TargetManager.PullDistance = 5;
                TargetManager.CombatType = TargetManager.CombatFlags.Trapping;
                TargetManager.LootType = TargetManager.LootFlags.Units | TargetManager.LootFlags.Skinning;
                Movement.IgnoreTaxiCheck = true;
            }),

            new BehaviorHotspotRunning(
                MovementCache.Trapping_Leather_ShadowMoonValley,
                CacheStaticLookUp.Trap_UnitIds_Elekk, 
                BehaviorHotspotRunning.HotSpotType.Both, 
                () => true),

        }, "Trapping Leather Shadowmoon Vally")
        {
            Criteria = () => BaseSettings.CurrentSettings.TrapSettings_NonElite_Leather.Enabled,
            RunCondition = () =>
                BaseSettings.CurrentSettings.TrapSettings_NonElite_Leather.MaximumItemCount >
                PlayerInventory.GetTotalStackCount(
                    Player.Inventory.GetCraftingReagentsById(
                        (int)BaseSettings.CurrentSettings.TrapSettings_NonElite_Leather.ItemType).ToArray()),

            DisposalAction = () =>
            {
                TargetManager.PullDistance = Targeting.PullDistance;
                TargetManager.CombatType = TargetManager.CombatFlags.Normal;
                TargetManager.LootType = TargetManager.LootFlags.None;
                Movement.IgnoreTaxiCheck = false;
            }

        };

        #endregion

        #region Behavior Trapping Leather Frostfire Ridge

        internal static readonly BehaviorArray BehaviorArray_Trapping_Leather_FrostfireRidge = new BehaviorArray(new Behavior[]
        {
            //
            new BehaviorMove(new WoWPoint(6311.582, 6030.6, 168.9311), 200f),

            new BehaviorCustomAction(() =>
            {
                TargetManager.PullDistance = 5;
                TargetManager.CombatType = TargetManager.CombatFlags.Trapping;
                TargetManager.LootType = TargetManager.LootFlags.Units | TargetManager.LootFlags.Skinning;
                Movement.IgnoreTaxiCheck = true;
            }),

            new BehaviorHotspotRunning(
                MovementCache.Trapping_Leather_FrostfireRidge,
                CacheStaticLookUp.Trap_UnitIds_Clefthoof, 
                BehaviorHotspotRunning.HotSpotType.Both, 
                () => true),

        }, "Trapping Leather Frostfire Ridge")
        {
            Criteria = () => BaseSettings.CurrentSettings.TrapSettings_NonElite_Leather.Enabled,
            RunCondition = () =>
                BaseSettings.CurrentSettings.TrapSettings_NonElite_Leather.MaximumItemCount >
                PlayerInventory.GetTotalStackCount(
                    Player.Inventory.GetCraftingReagentsById(
                        (int)BaseSettings.CurrentSettings.TrapSettings_NonElite_Leather.ItemType).ToArray()),

            DisposalAction = () =>
            {
                TargetManager.PullDistance = Targeting.PullDistance;
                TargetManager.CombatType = TargetManager.CombatFlags.Normal;
                TargetManager.LootType = TargetManager.LootFlags.None;
                Movement.IgnoreTaxiCheck = false;
            }

        };

        #endregion

        #region Behavior Trapping Fur Nagrand Horde

        internal static readonly BehaviorArray BehaviorArray_Trapping_Fur_Nagrand_Horde = new BehaviorArray(new Behavior[]
        {
            //
            new BehaviorMove(new WoWPoint(3222.156, 4476.665, 142.0599), 100f),

            new BehaviorCustomAction(() =>
            {
                TargetManager.PullDistance = 5;
                TargetManager.CombatType = TargetManager.CombatFlags.Trapping;
                TargetManager.LootType = TargetManager.LootFlags.Units | TargetManager.LootFlags.Skinning;
                Movement.IgnoreTaxiCheck = true;
            }),

            new BehaviorHotspotRunning(
                MovementCache.Trapping_Fur_Nagrand_Horde,
                CacheStaticLookUp.Trap_UnitIds_Wolves.Concat(CacheStaticLookUp.Trap_UnitIds_Talbulk).ToArray(), 
                BehaviorHotspotRunning.HotSpotType.Both, 
                () => true),


        }, "Trapping Fur Nagrand")
        {
            Criteria = () => BaseSettings.CurrentSettings.TrapSettings_NonElite_Fur.Enabled,
            RunCondition = () =>
                BaseSettings.CurrentSettings.TrapSettings_NonElite_Fur.MaximumItemCount >
                PlayerInventory.GetTotalStackCount(
                    Player.Inventory.GetCraftingReagentsById(
                        (int)BaseSettings.CurrentSettings.TrapSettings_NonElite_Fur.ItemType).ToArray()),

            DisposalAction = () =>
            {
                TargetManager.PullDistance = Targeting.PullDistance;
                TargetManager.CombatType = TargetManager.CombatFlags.Normal;
                TargetManager.LootType = TargetManager.LootFlags.None;
                Movement.IgnoreTaxiCheck = false;
            }

        };

        #endregion

        #region Behavior Trapping Fur Shadowmoon Valley

        internal static readonly BehaviorArray BehaviorArray_Trapping_Fur_ShadowmoonValley = new BehaviorArray(new Behavior[]
        {
            //
            new BehaviorMove(MovementCache.Trapping_Fur_ShadowmoonValley[0], 200f),

            new BehaviorCustomAction(() =>
            {
                TargetManager.PullDistance = 5;
                TargetManager.CombatType = TargetManager.CombatFlags.Trapping;
                TargetManager.LootType = TargetManager.LootFlags.Units | TargetManager.LootFlags.Skinning;
                Movement.IgnoreTaxiCheck = true;
            }),

            new BehaviorHotspotRunning(
                MovementCache.Trapping_Fur_ShadowmoonValley,
                CacheStaticLookUp.Trap_UnitIds_Wolves, 
                BehaviorHotspotRunning.HotSpotType.Both, 
                () => true),


        }, "Trapping Fur Shadowmoon Valley")
        {
            Criteria = () => BaseSettings.CurrentSettings.TrapSettings_NonElite_Fur.Enabled,
            RunCondition = () =>
                BaseSettings.CurrentSettings.TrapSettings_NonElite_Fur.MaximumItemCount >
                PlayerInventory.GetTotalStackCount(
                    Player.Inventory.GetCraftingReagentsById(
                        (int)BaseSettings.CurrentSettings.TrapSettings_NonElite_Fur.ItemType).ToArray()),

            DisposalAction = () =>
            {
                TargetManager.PullDistance = Targeting.PullDistance;
                TargetManager.CombatType = TargetManager.CombatFlags.Normal;
                TargetManager.LootType = TargetManager.LootFlags.None;
                Movement.IgnoreTaxiCheck = false;
            }

        };

        #endregion

        #region Behavior Trapping Meat Gorgond

        internal static readonly BehaviorArray BehaviorArray_Trapping_Boars_Gorgond = new BehaviorArray(new Behavior[]
        {
            //
            new BehaviorMove(MovementCache.Trapping_Meat_Gorgond[0], 200f),

            new BehaviorCustomAction(() =>
            {
                TargetManager.PullDistance = 5;
                TargetManager.CombatType = TargetManager.CombatFlags.Trapping;
                TargetManager.LootType = TargetManager.LootFlags.Units | TargetManager.LootFlags.Skinning;
                Movement.IgnoreTaxiCheck = true;
            }),
            
            new BehaviorHotspotRunning(
                MovementCache.Trapping_Meat_Gorgond, 
                CacheStaticLookUp.Trap_UnitIds_Boars,
                BehaviorHotspotRunning.HotSpotType.Both, 
                () => true),

        }, "Trapping Meat Nagrand")
        {
            Criteria = () => BaseSettings.CurrentSettings.TrapSettings_NonElite_Meat.Enabled,
            RunCondition = () =>
                BaseSettings.CurrentSettings.TrapSettings_NonElite_Meat.MaximumItemCount >
                PlayerInventory.GetTotalStackCount(
                    Player.Inventory.GetCraftingReagentsById(
                        (int)BaseSettings.CurrentSettings.TrapSettings_NonElite_Meat.ItemType).ToArray()),

            DisposalAction = () =>
            {
                TargetManager.PullDistance = Targeting.PullDistance;
                TargetManager.CombatType = TargetManager.CombatFlags.Normal;
                TargetManager.LootType = TargetManager.LootFlags.None;
                Movement.IgnoreTaxiCheck = false;
            }

        };

        #endregion

        #region Behavior Trapping Elite Wolves Nagrand

        internal static readonly BehaviorArray BehaviorArray_Trapping_Elites_Nagrand = new BehaviorArray(new Behavior[]
        {
            //
            new BehaviorMove(MovementCache.Trapping_Elites_Nagrand[0], 200f),

            new BehaviorCustomAction(() =>
            {
                TargetManager.PullDistance = 5;
                TargetManager.CombatType = TargetManager.CombatFlags.Trapping;
                TargetManager.LootType = TargetManager.LootFlags.Units | TargetManager.LootFlags.Skinning;
                Movement.IgnoreTaxiCheck = true;
            }),
            
            new BehaviorHotspotRunning(
                MovementCache.Trapping_Elites_Nagrand, 
                CacheStaticLookUp.Trap_UnitIds_Wolves.Concat(CacheStaticLookUp.Trap_UnitIds_Clefthoof).Concat(CacheStaticLookUp.Trap_UnitIds_Talbulk).ToArray(),
                BehaviorHotspotRunning.HotSpotType.Both, 
                () => true),


        }, "Trapping Elites Nagrand")
        {
            Criteria = () => BaseSettings.CurrentSettings.TrapSettings_Elite_Leather.Enabled,
            RunCondition = () =>
                BaseSettings.CurrentSettings.TrapSettings_Elite_Leather.MaximumItemCount >
                PlayerInventory.GetTotalStackCount(
                    Player.Inventory.GetCraftingReagentsById(
                        (int)BaseSettings.CurrentSettings.TrapSettings_Elite_Leather.ItemType).ToArray()),

            DisposalAction = () =>
            {
                TargetManager.PullDistance = Targeting.PullDistance;
                TargetManager.CombatType = TargetManager.CombatFlags.Normal;
                TargetManager.LootType = TargetManager.LootFlags.None;
                Movement.IgnoreTaxiCheck = false;
            }

        };

        #endregion





        internal static bool HasQuest(uint questId)
        {
            return QuestHelper.QuestContainedInQuestLog(questId);
        }
        internal static bool HasQuestAndNotCompleted(uint questId)
        {
            return
                HasQuest(questId) &&
                !QuestHelper.GetQuestFromQuestLog(questId).IsCompleted;
        }
        internal static bool ObjectNotValidOrNotFound(uint id)
        {
            var obj = ObjectCacheManager.GetWoWObject(id);
            return obj == null || !obj.IsValid;
        }
        internal static bool CanInteractWithUnit(uint id)
        {
            var unit = ObjectCacheManager.GetWoWUnits(id).FirstOrDefault();
            return unit != null && unit.IsValid && unit.RefWoWUnit.CanInteract;
        }
        internal static bool CanAttackUnit(uint id)
        {
            var unit = ObjectCacheManager.GetWoWUnits(id).FirstOrDefault();
            return unit != null && unit.IsValid && unit.RefWoWUnit.Attackable;
        }
        internal static bool UnitHasQuestGiverStatus(uint id, QuestGiverStatus status)
        {
            var unit = ObjectCacheManager.GetWoWUnits(id).FirstOrDefault();
            return unit != null && unit.IsValid && unit.RefWoWUnit.QuestGiverStatus == status;
        }
    }
}
