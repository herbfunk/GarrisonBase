using System;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Herbfunk.GarrisonBase.Character;
using Herbfunk.GarrisonBase.Garrison;
using Styx;
using Styx.CommonBot.Coroutines;
using Styx.CommonBot.Frames;
using Styx.WoWInternals;

namespace Herbfunk.GarrisonBase.Coroutines.Behaviors
{
    public class BehaviorCraftingProfession : Behavior
    {
        public override BehaviorType Type { get { return BehaviorType.ProfessionCrafting; } }

        internal readonly SkillLine Skill;
        internal readonly int SkillId;
        private readonly int SkillBookId;
        private readonly Tuple<CraftingReagents, int>[] Currency;
        public BehaviorCraftingProfession(SkillLine skill, int skillid)
        {
            Skill = skill;
            SkillId = skillid;
            SkillBookId = PlayerProfessions.DraenorProfessionSpellIds[Skill];
            Currency = PlayerProfessions.GetWorkOrderItemAndQuanityRequired(skillid);
        }
        public override string GetStatusText
        {
            get { return base.GetStatusText + Skill.ToString() + " id[" + SkillId + "]"; }
        }
        public override Func<bool> Criteria
        {
            get
            {
                return () =>
                    (BaseSettings.CurrentSettings.BehaviorProfessions &&
                     BaseSettings.CurrentSettings.ProfessionSpellIds.Contains(SkillId) &&
                     PlayerProfessions.HasRequiredCurrency(Currency) &&
                     !Spell.Cooldown);
            }
        }

        public override async Task<bool> BehaviorRoutine()
        {
            if (await base.BehaviorRoutine()) return true;
            if (IsDone) return false;

            if (Skill == SkillLine.Blacksmithing || Skill == SkillLine.Engineering)
            {
                if (!GarrisonManager.HasForge)
                    return false;

                if (GarrisonManager.Buildings[GarrisonManager.ForgeBuilding].CanActivate ||
                    GarrisonManager.Buildings[GarrisonManager.ForgeBuilding].IsBuilding)
                    return false;

                if (_movement==null)
                    _movement=new Movement(GarrisonManager.Buildings[GarrisonManager.ForgeBuilding].EntranceMovementPoint, 2f);

                if (await _movement.MoveTo())
                    return true;
            }

            Frame f = new Frame("TradeSkillFrame");
            //PreCheck (close any tradeskill frames!)
            if (!_prechecks)
            {
                if (f.IsVisible)
                {
                    LuaCommands.CloseTradeSkillFrame();
                    await CommonCoroutines.SleepForRandomUiInteractionTime();
                    await Coroutine.Sleep(StyxWoW.Random.Next(750, 2222));
                }
                _prechecks = true;
                await CommonCoroutines.SleepForRandomUiInteractionTime();
            }
               
            //Open Crafting Spellbook
            if (!LuaEvents.TradeSkillFrameOpen)
            {
                WoWSpell.FromId(SkillBookId).Cast();
                await CommonCoroutines.SleepForRandomUiInteractionTime();
                await Coroutine.Sleep(StyxWoW.Random.Next(1777, 3333));
                return true;
            }

            //Craft Item
            if (!Spell.Cooldown && Spell.CanCast)
            {
                await CommonCoroutines.WaitForLuaEvent(
                    "BAG_UPDATE",
                    10000,
                    () => false,
                    Spell.Cast);

                return true;
            }

            //Close UI
            if (f.IsVisible)
            {
                LuaCommands.CloseTradeSkillFrame();
                await CommonCoroutines.SleepForRandomUiInteractionTime();
            }
                
            return false;
        }

        private bool _prechecks = false;
        private Movement _movement;

        private WoWSpell Spell
        {
            get { return WoWSpell.FromId(SkillId); }
        }
    }
}