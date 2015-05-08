using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Herbfunk.GarrisonBase.Cache.Objects;
using Styx;
using Styx.Common;
using Styx.CommonBot;
using Styx.WoWInternals;

namespace Herbfunk.GarrisonBase.Character
{
    public class PlayerProfessions
    {
        public Dictionary<SkillLine, ProfessionSkill> ProfessionSkills = new Dictionary<SkillLine, ProfessionSkill>(); 
        public bool Skinning = false;
        public PlayerProfessions()
        {
            RefreshProfessionSkills();
            Skinning = ProfessionSkills.ContainsKey(SkillLine.Skinning);
            GarrisonBase.Debug("Found a total of {0} professions!", ProfessionSkills.Count);


        }

        
        private void RefreshProfessionSkills()
        {
            ProfessionSkills.Clear();

            foreach (var skillLine in SkillsList)
            {
                var spell = StyxWoW.Me.GetSkill(skillLine);
                if (spell.MaxValue > 1)
                {
                    ProfessionSkills.Add(skillLine, new ProfessionSkill(skillLine, spell));
                }
            }
        }
        /// <summary> 
        /// List of all the professions in World of Warcraft. 
        /// </summary> 
        private ReadOnlyCollection<SkillLine> SkillsList
        {
            get
            {
                return new List<SkillLine> 
                           { 
                               SkillLine.Alchemy, 
                               //SkillLine.Archaeology, 
                               SkillLine.Blacksmithing, 
                               SkillLine.Cooking, 
                               SkillLine.Enchanting, 
                               SkillLine.Engineering, 
                               //SkillLine.FirstAid, 
                               SkillLine.Fishing, 
                               SkillLine.Herbalism, 
                               SkillLine.Inscription, 
                               SkillLine.Jewelcrafting, 
                               SkillLine.Leatherworking, 
                               SkillLine.Mining, 
                               SkillLine.Skinning, 
                               SkillLine.Tailoring 
                           }.AsReadOnly();
            }
        }

        internal static Dictionary<SkillLine, int[]> ProfessionDailyCooldownSpellIds = new Dictionary<SkillLine, int[]>
        {
            {SkillLine.Blacksmithing, new[] {171690, 176090}},
            {SkillLine.Alchemy, new[] {156587, 175880}},
            {SkillLine.Leatherworking, new[] {171391, 176089}},
            {SkillLine.Tailoring, new[] {168835, 176058}},
            {SkillLine.Jewelcrafting, new[] {170700, 176087}},
            {SkillLine.Engineering, new[] {169080, 177054}},
            {SkillLine.Inscription, new[] {169081, 177045}},
            {SkillLine.Enchanting, new[] {169092, 177043}},
        };

        internal static Dictionary<SkillLine, int> DraenorProfessionSpellIds = new Dictionary<SkillLine, int>
        {
            {SkillLine.Blacksmithing, 158737},
            {SkillLine.Alchemy, 156606},
            {SkillLine.Leatherworking, 158752},
            {SkillLine.Tailoring, 158758},
            {SkillLine.Jewelcrafting, 158750},
            {SkillLine.Engineering, 158739},
            {SkillLine.Inscription, 158748},
            {SkillLine.Enchanting, 158716},
            {SkillLine.Cooking, 158765},
        };

        internal static Tuple<CraftingReagents, int>[] GetWorkOrderItemAndQuanityRequired(int spellId)
        {
            if (spellId == 171690) //TrueSteel Ingot
                return new[] { new Tuple<CraftingReagents, int>(CraftingReagents.TrueIronOre, 20), new Tuple<CraftingReagents, int>(CraftingReagents.BlackrockOre, 10) };
            if (spellId == 176090) //secrets of draenor blacksmithing
                return new[] { new Tuple<CraftingReagents, int>(CraftingReagents.TrueIronOre, 5) };

            if (spellId == 156587) //Alchemical Catalyst
                return new[] { new Tuple<CraftingReagents, int>(CraftingReagents.Frostweed, 20), new Tuple<CraftingReagents, int>(CraftingReagents.BlackrockOre, 10) };
            if (spellId == 175880) //secrets of draenor alchemy
                return new[] { new Tuple<CraftingReagents, int>(CraftingReagents.Frostweed, 5) };

            if (spellId == 171391) //Burnished Leather
                return new[] { new Tuple<CraftingReagents, int>(CraftingReagents.RawBeastHide, 20), new Tuple<CraftingReagents, int>(CraftingReagents.GorgrondFlytrap, 10) };
            if (spellId == 176089) //secrets of draenor leatherworking
                return new[] { new Tuple<CraftingReagents, int>(CraftingReagents.RawBeastHide, 5) };

            if (spellId == 168835) //Hexweave Cloth
                return new[] { new Tuple<CraftingReagents, int>(CraftingReagents.SumptuousFur, 20), new Tuple<CraftingReagents, int>(CraftingReagents.GorgrondFlytrap, 10) };
            if (spellId == 176058) //secrets of draenor tailoring
                return new[] { new Tuple<CraftingReagents, int>(CraftingReagents.SumptuousFur, 5) };

            if (spellId == 170700) //Taladite Crystal
                return new[] { new Tuple<CraftingReagents, int>(CraftingReagents.BlackrockOre, 20), new Tuple<CraftingReagents, int>(CraftingReagents.TrueIronOre, 10) };
            if (spellId == 176087) //secrets of draenor jewelcrafting
                return new[] { new Tuple<CraftingReagents, int>(CraftingReagents.BlackrockOre, 5) };

            if (spellId == 169080) //Gearspring Parts
                return new[] { new Tuple<CraftingReagents, int>(CraftingReagents.TrueIronOre, 10), new Tuple<CraftingReagents, int>(CraftingReagents.BlackrockOre, 10) };
            if (spellId == 177054) //secrets of draenor Engineering
                return new[] { new Tuple<CraftingReagents, int>(CraftingReagents.TrueIronOre, 5) };


            if (spellId == 169081) //War paints
                return new[] { new Tuple<CraftingReagents, int>(CraftingReagents.CeruleanPigment, 10) };
            if (spellId == 177045) //secrets of draenor Inscription
                return new[] { new Tuple<CraftingReagents, int>(CraftingReagents.CeruleanPigment, 2) };

            if (spellId == 169092) //Temporal Crystal
                return new[] { new Tuple<CraftingReagents, int>(CraftingReagents.LuminousShard, 1) };
            if (spellId == 177043) //secrets of draenor enchanting
                return new[] { new Tuple<CraftingReagents, int>(CraftingReagents.DraenicDust, 3) };

            return null;
        }

        internal static string GetProfessionCraftingName(int spellId)
        {
            if (spellId == 171690) //TrueSteel Ingot
                return "TrueSteel Ingot";
            if (spellId == 176090) //secrets of draenor blacksmithing
                return "Secrets of Draenor Blacksmithing";

            if (spellId == 156587) //Alchemical Catalyst
                return "Alchemical Catalyst";
            if (spellId == 175880) //secrets of draenor alchemy
                return "Secrets of Draenor Alchemy";

            if (spellId == 171391) //Burnished Leather
                return "Burnished Leather";
            if (spellId == 176089) //secrets of draenor leatherworking
                return "Secrets of Draenor Leatherworking";

            if (spellId == 168835) //Hexweave Cloth
                return "Hexweave Cloth";
            if (spellId == 176058) //secrets of draenor tailoring
                return "Secrets of Draenor Tailoring";

            if (spellId == 170700) //Taladite Crystal
                return "Taladite Crystal";
            if (spellId == 176087) //secrets of draenor jewelcrafting
                return "Secrets of Draenor Jewelcrafting";

            if (spellId == 169080) //Gearspring Parts
                return "Gearspring Parts";
            if (spellId == 177054) //secrets of draenor Engineering
                return "Secrets of Draenor Engineering";


            if (spellId == 169081) //War paints
                return "War paints";
            if (spellId == 177045) //secrets of draenor Inscription
                return "Secrets of Draenor Inscription";

            if (spellId == 169092) //Temporal Crystal
                return "Temporal Crystal";
            if (spellId == 177043) //secrets of draenor enchanting
                return "Secrets of Draenor Enchanting";

            return "Unknown";
        }


        public static bool HasRequiredCurrency(Tuple<CraftingReagents, int>[] Currency)
        {

            if (Currency == null) return false;

            long buyableCount = 0;
            foreach (var c in Currency)
            {
                List<C_WoWItem> items = Character.Player.Inventory.GetBagItemsById((int)c.Item1);
                if (items.Count == 0)
                {
                    return false;
                }
                long totalItems = items.Aggregate<C_WoWItem, long>(0, (current, i) => current + i.StackCount);
                if (totalItems < c.Item2)
                {
                    return false;
                }

                buyableCount += totalItems / c.Item2;
            }
            return (buyableCount / Currency.Length)>=1;
        }

        public static int DisenchantRequiredEnchantingSkill(C_WoWItem item)
        {
            var ilevel = item.Level;
            if (ilevel < 61)
            {
                if (ilevel < 21) return 1;
                if (ilevel < 26) return 25;
                if (ilevel < 31) return 50;
                if (ilevel < 36) return 75;
                if (ilevel < 41) return 100;
                if (ilevel < 46) return 125;
                if (ilevel < 51) return 150;
                if (ilevel < 56) return 175;
                return 200;
            }

            var quality = item.Quality;
            if (quality == WoWItemQuality.Uncommon)
            {
                if (ilevel < 100) return 225;
                if (ilevel < 121) return 275;
                if (ilevel < 151) return 325;
                if (ilevel < 183) return 350;
                if (ilevel < 334) return 425;
                if (ilevel < 438) return 475;
            }
            else if (quality == WoWItemQuality.Rare)
            {
                if (ilevel < 100) return 225;
                if (ilevel < 121) return 275;
                if (ilevel < 201) return 325;
                if (ilevel < 378) return 450;
                if (ilevel < 477) return 550;
            }
            else if (quality == WoWItemQuality.Epic)
            {
                if (ilevel < 90) return 225;
                if (ilevel < 152) return 300;
                if (ilevel < 278) return 375;
                if (ilevel < 417) return 475;
                if (ilevel < 529) return 575;
            }

            return 1;
        }


        public class ProfessionSkill
        {
            public SkillLine Skill { get; set; }
            public int MaxValue { get; set; }
            public int CurrentValue { get; set; }

            public int SpellbookId { get; set; }
            public int[] DailyCooldownSpellIds { get; set; }

            public ProfessionSkill(SkillLine skill,WoWSkill spell)
            {
                Skill = skill;
                MaxValue = spell.MaxValue;
                CurrentValue = spell.CurrentValue;
                DailyCooldownSpellIds = null;

                if (MaxValue >= 700 && ProfessionDailyCooldownSpellIds.ContainsKey(Skill))
                    DailyCooldownSpellIds = ProfessionDailyCooldownSpellIds[Skill];

                if (DraenorProfessionSpellIds.ContainsKey(Skill))
                    SpellbookId = DraenorProfessionSpellIds[Skill];
            }

            public override string ToString()
            {
                var dailycooldownStr = "Daily Cooldown Spells: ";
                if (DailyCooldownSpellIds != null)
                {
                    dailycooldownStr = DailyCooldownSpellIds.Aggregate(dailycooldownStr, (current, id) => current + "Id: " + id);
                }

                return String.Format("{0} {1} / {2} -- SpellBookId {3} {4}",
                    Skill.ToString(), CurrentValue, MaxValue, SpellbookId, dailycooldownStr);
            }
        }
    }
}
