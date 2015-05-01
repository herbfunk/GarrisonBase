using System;
using System.Collections.Generic;
using Herbfunk.GarrisonBase.Garrison.Enums;
using Styx;
using Styx.Helpers;
using Styx.WoWInternals;
using Styx.WoWInternals.Garrison;

namespace Herbfunk.GarrisonBase.Garrison.Objects
{
    public class Mission
    {
        internal readonly GarrisonMission refGarrisonMission;
        public string Name { get; set; }
        public string Description { get; set; }
        public int Cost { get; set; }
        public int Duration { get; set; }
        public int Level { get; set; }
        public string Type { get; set; }
        public int State { get; set; }
        public int ItemLevel { get; set; }
        public string Location { get; set; }
        public bool Rare { get; set; }

        public int Id { get; set; }

        public int Followers { get; set; }
        public int Rewards { get; set; }
        public string Xp { get; set; }
        public string Material { get; set; }
        public string SuccessChance { get; set; }
        public int XpBonus { get; set; }
        public int GarrisonReward { get; set; }
        public int XpReward { get; set; }
        public int GoldReward { get; set; }
        public int ApexisReward { get; set; }
        public int ItemIdReward { get; set; }
        public int ItemIdReward2 { get; set; }
        public bool XPOnlyReward { get; set; }
        public bool Success { get; set; }
        public RewardTypes RewardTypes
        {
            get { return _rewardTypes; }
            set { _rewardTypes = value; }
        }
        private RewardTypes _rewardTypes = RewardTypes.None;

        private List<CombatAbilities> Abilities = new List<CombatAbilities>();

        public int SuccessRate
        {
            get
            {

                if (ItemIdReward > 0)
                {
                    if (RewardTypes.HasFlag(RewardTypes.CharacterToken))
                        return BaseSettings.CurrentSettings.CharacterTokens.SuccessRate;
                    if (RewardTypes.HasFlag(RewardTypes.FollowerToken))
                        return BaseSettings.CurrentSettings.FollowerTokens.SuccessRate;
                    if (RewardTypes.HasFlag(RewardTypes.FollowerTrait))
                        return BaseSettings.CurrentSettings.FollowerTraits.SuccessRate;
                    if (RewardTypes.HasFlag(RewardTypes.FollowerContract))
                        return BaseSettings.CurrentSettings.FollowerContracts.SuccessRate;
                    if (RewardTypes.HasFlag(RewardTypes.RetrainingCertificate))
                        return BaseSettings.CurrentSettings.FollowerRetraining.SuccessRate;
                    if (RewardTypes.HasFlag(RewardTypes.RushOrder))
                        return BaseSettings.CurrentSettings.RushOrders.SuccessRate;
                    if (RewardTypes.HasFlag(RewardTypes.AbrogatorStone))
                        return BaseSettings.CurrentSettings.AbrogatorStone.SuccessRate;
                    if (RewardTypes.HasFlag(RewardTypes.ElementalRune))
                        return BaseSettings.CurrentSettings.ElementalRune.SuccessRate;
                    if (RewardTypes.HasFlag(RewardTypes.SavageBlood))
                        return BaseSettings.CurrentSettings.SavageBlood.SuccessRate;
                    if (RewardTypes.HasFlag(RewardTypes.PrimalSpirit))
                        return BaseSettings.CurrentSettings.PrimalSpirit.SuccessRate;

                    return BaseSettings.CurrentSettings.Items.SuccessRate;
                }



                if (RewardTypes.HasFlag(RewardTypes.ApexisCrystal))
                    return BaseSettings.CurrentSettings.ApexisCrystal.SuccessRate;

                if (RewardTypes.HasFlag(RewardTypes.Gold))
                    return BaseSettings.CurrentSettings.Gold.SuccessRate;

                if (RewardTypes.HasFlag(RewardTypes.Garrison))
                    return BaseSettings.CurrentSettings.GarrisonResources.SuccessRate;

                if (RewardTypes.HasFlag(RewardTypes.XP))
                    return BaseSettings.CurrentSettings.FollowerExperience.SuccessRate;

                if (RewardTypes.HasFlag(RewardTypes.SealOfTemperedFate))
                    return BaseSettings.CurrentSettings.SealOfTemperedFate.SuccessRate;

                if (RewardTypes.HasFlag(RewardTypes.HonorPoints))
                    return BaseSettings.CurrentSettings.HonorPoints.SuccessRate;


                return 100;
            }
        }
        public int Priority
        {
            get
            {
                if (_priority == -1)
                {
                    _priority++;


                    if (RewardTypes.HasFlag(RewardTypes.CharacterToken))
                    {
                        if (DictItemRewards_CharacterTokens.ContainsKey(ItemIdReward) &&
                            DictItemRewards_CharacterTokens[ItemIdReward].Item2<BaseSettings.CurrentSettings.MissionReward_CharacterToken_ItemLevel)
                        {
                            _priority = 0;
                        }
                        else
                        {
                            _priority += BaseSettings.CurrentSettings.CharacterTokens.Priority;
                        }
                        
                    }
                    else if (RewardTypes.HasFlag(RewardTypes.FollowerToken))
                    {
                        if (ItemRewards_FollowerToken_Armor615 == ItemIdReward &&
                            !BaseSettings.CurrentSettings.MissionReward_FollowerToken_ArmorSet615)
                        {
                            _priority = 0;
                        }
                        else if (ItemRewards_FollowerToken_Armor630 == ItemIdReward &&
                            !BaseSettings.CurrentSettings.MissionReward_FollowerToken_ArmorSet630)
                        {
                            _priority = 0;
                        }
                        else if (ItemRewards_FollowerToken_Armor645 == ItemIdReward &&
                            !BaseSettings.CurrentSettings.MissionReward_FollowerToken_ArmorSet645)
                        {
                            _priority = 0;
                        }
                        else if (ItemRewards_FollowerToken_Weapon615 == ItemIdReward &&
                            !BaseSettings.CurrentSettings.MissionReward_FollowerToken_WeaponSet615)
                        {
                            _priority = 0;
                        }
                        else if (ItemRewards_FollowerToken_Weapon630 == ItemIdReward &&
                            !BaseSettings.CurrentSettings.MissionReward_FollowerToken_WeaponSet630)
                        {
                            _priority = 0;
                        }
                        else if (ItemRewards_FollowerToken_Weapon645 == ItemIdReward &&
                            !BaseSettings.CurrentSettings.MissionReward_FollowerToken_WeaponSet645)
                        {
                            _priority = 0;
                        }
                        else
                        {
                            _priority += BaseSettings.CurrentSettings.FollowerTokens.Priority;
                        }
                    }
                    else if (RewardTypes.HasFlag(RewardTypes.FollowerTrait))
                        _priority += BaseSettings.CurrentSettings.FollowerTraits.Priority;
                    else if (RewardTypes.HasFlag(RewardTypes.FollowerContract))
                        _priority += BaseSettings.CurrentSettings.FollowerContracts.Priority;
                    else if (RewardTypes.HasFlag(RewardTypes.RetrainingCertificate))
                        _priority += BaseSettings.CurrentSettings.FollowerRetraining.Priority;
                    else if (RewardTypes.HasFlag(RewardTypes.RushOrder))
                        _priority += BaseSettings.CurrentSettings.RushOrders.Priority;
                    else if (RewardTypes.HasFlag(RewardTypes.SavageBlood))
                        _priority += BaseSettings.CurrentSettings.SavageBlood.Priority;
                    else if (RewardTypes.HasFlag(RewardTypes.AbrogatorStone))
                        _priority += BaseSettings.CurrentSettings.AbrogatorStone.Priority;
                    else if (RewardTypes.HasFlag(RewardTypes.ElementalRune))
                        _priority += BaseSettings.CurrentSettings.ElementalRune.Priority;
                    else if (RewardTypes.HasFlag(RewardTypes.PrimalSpirit))
                        _priority += BaseSettings.CurrentSettings.PrimalSpirit.Priority;
                    else if (RewardTypes.HasFlag(RewardTypes.Items))
                    {
                        _priority += BaseSettings.CurrentSettings.Items.Priority;
                    }
                    

                    if (RewardTypes.HasFlag(RewardTypes.SealOfTemperedFate))
                        _priority += BaseSettings.CurrentSettings.SealOfTemperedFate.Priority;

                    if (RewardTypes.HasFlag(RewardTypes.XP))
                        _priority += BaseSettings.CurrentSettings.FollowerExperience.Priority;

                    if (RewardTypes.HasFlag(RewardTypes.ApexisCrystal))
                        _priority += BaseSettings.CurrentSettings.ApexisCrystal.Priority;

                    if (RewardTypes.HasFlag(RewardTypes.Gold))
                        _priority += BaseSettings.CurrentSettings.Gold.Priority;

                    if (RewardTypes.HasFlag(RewardTypes.Garrison))
                        _priority += BaseSettings.CurrentSettings.GarrisonResources.Priority;

                    if (RewardTypes.HasFlag(RewardTypes.HonorPoints))
                        _priority += BaseSettings.CurrentSettings.HonorPoints.Priority;




                }
                return _priority;
            }
            set { _priority = value; }
        }

        public int MinimumLevel
        {
            get
            {

                if (ItemIdReward > 0)
                {
                    if (RewardTypes.HasFlag(RewardTypes.CharacterToken))
                        return BaseSettings.CurrentSettings.CharacterTokens.MinimumLevel;
                    if (RewardTypes.HasFlag(RewardTypes.FollowerToken))
                        return BaseSettings.CurrentSettings.FollowerTokens.MinimumLevel;
                    if (RewardTypes.HasFlag(RewardTypes.FollowerTrait))
                        return BaseSettings.CurrentSettings.FollowerTraits.MinimumLevel;
                    if (RewardTypes.HasFlag(RewardTypes.FollowerContract))
                        return BaseSettings.CurrentSettings.FollowerContracts.MinimumLevel;
                    if (RewardTypes.HasFlag(RewardTypes.RetrainingCertificate))
                        return BaseSettings.CurrentSettings.FollowerRetraining.MinimumLevel;
                    if (RewardTypes.HasFlag(RewardTypes.RushOrder))
                        return BaseSettings.CurrentSettings.RushOrders.MinimumLevel;

                    if (RewardTypes.HasFlag(RewardTypes.AbrogatorStone))
                        return BaseSettings.CurrentSettings.AbrogatorStone.MinimumLevel;
                    if (RewardTypes.HasFlag(RewardTypes.ElementalRune))
                        return BaseSettings.CurrentSettings.ElementalRune.MinimumLevel;
                    if (RewardTypes.HasFlag(RewardTypes.SavageBlood))
                        return BaseSettings.CurrentSettings.SavageBlood.MinimumLevel;
                    if (RewardTypes.HasFlag(RewardTypes.PrimalSpirit))
                        return BaseSettings.CurrentSettings.PrimalSpirit.MinimumLevel;

                    return BaseSettings.CurrentSettings.Items.MinimumLevel;
                }



                if (RewardTypes.HasFlag(RewardTypes.ApexisCrystal))
                    return BaseSettings.CurrentSettings.ApexisCrystal.MinimumLevel;

                if (RewardTypes.HasFlag(RewardTypes.Gold))
                    return BaseSettings.CurrentSettings.Gold.MinimumLevel;

                if (RewardTypes.HasFlag(RewardTypes.Garrison))
                    return BaseSettings.CurrentSettings.GarrisonResources.MinimumLevel;

                if (RewardTypes.HasFlag(RewardTypes.XP))
                    return BaseSettings.CurrentSettings.FollowerExperience.MinimumLevel;

                if (RewardTypes.HasFlag(RewardTypes.SealOfTemperedFate))
                    return BaseSettings.CurrentSettings.SealOfTemperedFate.MinimumLevel;

                if (RewardTypes.HasFlag(RewardTypes.HonorPoints))
                    return BaseSettings.CurrentSettings.HonorPoints.MinimumLevel;

                return 90;
            }
        }

        private int _priority = -1;

        public Mission(int cost, string description, int durationSeconds, int level, int iLevel,
            bool isRare, string location, int missionId, string name, int numFollowers, int numRewards, int state,
            string type, string xp, string material, int garrisonReward, int xpReward, int goldReward, int apexisReward, int itemIdReward, int itemIdReward2, int currencyId, int currencyAmount)
        {
            Cost = cost;
            Description = description;
            Duration = durationSeconds;
            ItemLevel = iLevel;
            Rare = isRare;
            Level = level;
            Location = location;
            Id = missionId;
            Name = name;
            Followers = numFollowers;
            Rewards = numRewards;
            State = state;
            Type = type;
            Xp = xp.ToString();
            Material = material;
            //Abilities = abilities;
            GarrisonReward = garrisonReward;
            XpReward = xpReward;
            GoldReward = goldReward;
            ApexisReward = apexisReward;
            ItemIdReward = itemIdReward;
            ItemIdReward2 = itemIdReward2;

            if (GarrisonReward > 0) RewardTypes |= RewardTypes.Garrison;
            if (XpReward > 0) RewardTypes |= RewardTypes.XP;
            if (GoldReward > 0) RewardTypes |= RewardTypes.Gold;
            if (ApexisReward > 0) RewardTypes |= RewardTypes.ApexisCrystal;
            if (ItemIdReward > 0)
            {
                if (DictItemRewards_CharacterTokens.ContainsKey(ItemIdReward))
                    RewardTypes |= RewardTypes.CharacterToken;
                else if (ItemRewards_FollowerTokens.Contains(ItemIdReward))
                    RewardTypes |= RewardTypes.FollowerToken;
                else if (ItemRewards_FollowerRetraining.Contains(ItemIdReward))
                    RewardTypes |= RewardTypes.RetrainingCertificate;
                else if (ItemRewards_Contracts.Contains(ItemIdReward))
                    RewardTypes |= RewardTypes.FollowerContract;
                else if (ItemRewards_FollowerTraits.Contains(ItemIdReward))
                    RewardTypes |= RewardTypes.FollowerTrait;
                else if (ItemRewards_RushOrders.Contains(ItemIdReward))
                    RewardTypes |= RewardTypes.RushOrder;
            }
            if (ItemIdReward2 > 0)
            {
                if (DictItemRewards_CharacterTokens.ContainsKey(ItemIdReward2))
                    RewardTypes |= RewardTypes.CharacterToken;
                else if (ItemRewards_FollowerTokens.Contains(ItemIdReward2))
                    RewardTypes |= RewardTypes.FollowerToken;
                else if (ItemRewards_FollowerRetraining.Contains(ItemIdReward2))
                    RewardTypes |= RewardTypes.RetrainingCertificate;
                else if (ItemRewards_Contracts.Contains(ItemIdReward2))
                    RewardTypes |= RewardTypes.FollowerContract;
                else if (ItemRewards_FollowerTraits.Contains(ItemIdReward2))
                    RewardTypes |= RewardTypes.FollowerTrait;
                else if (ItemRewards_RushOrders.Contains(ItemIdReward2))
                    RewardTypes |= RewardTypes.RushOrder;
            }
            if (currencyId > 0)
            {
                if (currencyId == 994) RewardTypes |= RewardTypes.SealOfTemperedFate;
                if (currencyId == 392) RewardTypes |= RewardTypes.HonorPoints;
                //
            }
        }

        public Mission(GarrisonMission mission, bool completed=false)
        {
            refGarrisonMission = mission;
            Id = mission.Id;
            Name = mission.Name;
            Followers = mission.MaxFollowers;
            if (completed) return;


            foreach (var reward in mission.RewardRecords)
            {
                if (reward.CurrencyQuantity > 0)
                {
                    if ((int) reward.CurrencyType == 0)
                        RewardTypes |= RewardTypes.Gold;
                    else if (reward.CurrencyType == WoWCurrencyType.GarrisonResources)
                        RewardTypes |= RewardTypes.Garrison;
                    else if (reward.CurrencyType == WoWCurrencyType.ApexisCrystal)
                        RewardTypes |= RewardTypes.ApexisCrystal;
                    else if (reward.CurrencyType == WoWCurrencyType.HonorPoints)
                        RewardTypes |= RewardTypes.HonorPoints;
                    else if (reward.CurrencyType == WoWCurrencyType.SealOfTemperedFate)
                        RewardTypes |= RewardTypes.SealOfTemperedFate;
                }
                
                if (reward.FollowerXP>0)
                    RewardTypes |= RewardTypes.XP;

                if (reward.ItemId <= 0) continue;

                if (ItemIdReward == 0)
                    ItemIdReward = reward.ItemId;
                else
                    ItemIdReward2 = reward.ItemId;

                if (DictItemRewards_CharacterTokens.ContainsKey(reward.ItemId))
                    RewardTypes |= RewardTypes.CharacterToken;
                else if (ItemRewards_FollowerTokens.Contains(reward.ItemId))
                    RewardTypes |= RewardTypes.FollowerToken;
                else if (ItemRewards_FollowerRetraining.Contains(reward.ItemId))
                    RewardTypes |= RewardTypes.RetrainingCertificate;
                else if (ItemRewards_Contracts.Contains(reward.ItemId))
                    RewardTypes |= RewardTypes.FollowerContract;
                else if (ItemRewards_FollowerTraits.Contains(reward.ItemId))
                    RewardTypes |= RewardTypes.FollowerTrait;
                else if (ItemRewards_RushOrders.Contains(reward.ItemId))
                    RewardTypes |= RewardTypes.RushOrder;
                else if (ItemReward_AbrogatorStone==reward.ItemId)
                    RewardTypes |= RewardTypes.AbrogatorStone;
                else if (ItemReward_ElementalRune == reward.ItemId)
                    RewardTypes |= RewardTypes.ElementalRune;
                else if (ItemReward_SavageBlood == reward.ItemId)
                    RewardTypes |= RewardTypes.SavageBlood;
                else if (ItemReward_PrimalSpirit == reward.ItemId)
                    RewardTypes |= RewardTypes.PrimalSpirit;
                else
                    RewardTypes |= RewardTypes.Items;
            }

            var newMission = LuaCommands.GetMissionInfo(Id);
            Cost = newMission.Cost;
            Description = newMission.Description;
            Duration = newMission.Duration;
            ItemLevel = newMission.ItemLevel;
            Rare = newMission.Rare;
            Level = newMission.Level;
            Location = newMission.Location;
            Rewards = newMission.Rewards;
            State = newMission.State;
            Type = newMission.Type;
            Xp = newMission.Xp;
            Material = newMission.Material;
        }



        public bool Valid
        {
            get
            {
                return 
                    refGarrisonMission != null &&
                    refGarrisonMission.BaseAddress != IntPtr.Zero;
            }
        }

        public bool MarkedAsCompleted { get; set; }
        public void MarkAsCompleted()
        {
            LuaCommands.MissionCompleteMarkComplete(Id);
            MarkedAsCompleted = true;
        }

        public bool BonusRolled { get; set; }




        public void BonusRoll()
        {
            LuaCommands.MissionCompleteRollChest(Id);
            BonusRolled = true;
        }

        public bool CanOpenMissionChest()
        {
            String lua = String.Format("return tostring(C_Garrison.CanOpenMissionChest(\"{0}\"))", Id);
            var ret = Lua.GetReturnValues(lua);
            if (ret == null || ret[0] == null) return false;
            return ret[0].ToBoolean();
        }
        public int GetBonusChance()
        {
            return LuaCommands.GetMissionBonusChance(Id);
        }
        public override string ToString()
        {
            string abilities = "";
            foreach (var a in Abilities)
            {
                abilities = abilities + a.ToString() + "\r\n";
            }
            return String.Format(
                "Name {7} (iLevel {6}) Level {3} (ID {10})\r\n" +
                "{0}\r\n" +
                "Cost: {1} Duration: {2}  Type {4} State {5} Location {8}\r\n" +
                "Rare {9} Followers {11} \r\n" +
                "Rewards {12} [{27}] GarrisonReward {21} XpReward {22} Apexis {23} Gold {24} ItemId {25} {26}\r\n" +
                "XP {13} Material {14} SuccessChance {15} XpBonus {16} Success {17}\r\n" +
                "{18} {19}\r\n" +
                "Abilities\r\n{20}",
                Description, Cost, Duration, Level, Type, State, ItemLevel, Name, Location, Rare, Id,
                Followers, Rewards, Xp, Material, SuccessChance, XpBonus, Success,
                !MarkedAsCompleted ? "" : "Marked as Completed",
                !BonusRolled ? "" : "Rolled Bonus",
                abilities,
                GarrisonReward, XpReward, ApexisReward, GoldReward,
                ItemIdReward, ItemIdReward2 > 0 ? ItemIdReward2.ToString() : "",
                RewardTypes.ToString());
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public override bool Equals(object obj)
        {
            //Check for null and compare run-time types. 
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            Mission p = (Mission)obj;
            return (Id == p.Id);
        }


        #region Static Cache Item Rewards
        public static readonly Dictionary<int, Tuple<InventoryType, int>> DictItemRewards_CharacterTokens = new Dictionary
           <int, Tuple<InventoryType, int>>
        {
            {114100, new Tuple<InventoryType, int>(InventoryType.Shoulder, 610)},
            {114105, new Tuple<InventoryType, int>(InventoryType.Trinket, 600)},
            {114097, new Tuple<InventoryType, int>(InventoryType.Hand, 590)},
            {114099, new Tuple<InventoryType, int>(InventoryType.Legs, 580)},
            {114094, new Tuple<InventoryType, int>(InventoryType.Wrist, 570)},
            {114108, new Tuple<InventoryType, int>(InventoryType.Weapon, 560)},
            {114096, new Tuple<InventoryType, int>(InventoryType.Feet, 550)},
            {114098, new Tuple<InventoryType, int>(InventoryType.Head, 540)},
            {114101, new Tuple<InventoryType, int>(InventoryType.Waist, 530)},
            {114053, new Tuple<InventoryType, int>(InventoryType.Hand, 512)},
            {114052, new Tuple<InventoryType, int>(InventoryType.Finger, 519)},

            ////615
            {114063, new Tuple<InventoryType, int>(InventoryType.Shoulder, 615)},
            {114057, new Tuple<InventoryType, int>(InventoryType.Wrist, 615)},
            {114058, new Tuple<InventoryType, int>(InventoryType.Chest, 615)},
            {114068, new Tuple<InventoryType, int>(InventoryType.Trinket, 615)},
            {114066, new Tuple<InventoryType, int>(InventoryType.Neck, 615)},
            {114109, new Tuple<InventoryType, int>(InventoryType.Weapon, 615)},
            {114059, new Tuple<InventoryType, int>(InventoryType.Feet, 615)},

            ////630
            {114080, new Tuple<InventoryType, int>(InventoryType.Trinket, 630)},
            {114071, new Tuple<InventoryType, int>(InventoryType.Feet, 630)},
            {114075, new Tuple<InventoryType, int>(InventoryType.Shoulder, 630)},
            {114078, new Tuple<InventoryType, int>(InventoryType.Neck, 630)},
            {114110, new Tuple<InventoryType, int>(InventoryType.Weapon, 630)},
            {114070, new Tuple<InventoryType, int>(InventoryType.Chest, 630)},
            {114069, new Tuple<InventoryType, int>(InventoryType.Wrist, 630)},

            ////645
            {114085, new Tuple<InventoryType, int>(InventoryType.Shoulder, 645)},
            {114083, new Tuple<InventoryType, int>(InventoryType.Chest, 645)},
            {114084, new Tuple<InventoryType, int>(InventoryType.Feet, 645)},
            {114082, new Tuple<InventoryType, int>(InventoryType.Wrist, 645)},
            {114112, new Tuple<InventoryType, int>(InventoryType.Weapon, 645)},
            {114087, new Tuple<InventoryType, int>(InventoryType.Trinket, 645)},
            {114086, new Tuple<InventoryType, int>(InventoryType.Neck, 645)},

            ////Highmaul Caches
            {118531, new Tuple<InventoryType, int>(InventoryType.None, 685)},
            {118530, new Tuple<InventoryType, int>(InventoryType.None, 670)},
            {118529, new Tuple<InventoryType, int>(InventoryType.None, 665)},
        };

        public static int ItemRewards_FollowerToken_ArmorEnhancement = 120301;
        public static int ItemRewards_FollowerToken_WeaponEnhancement = 120302;
        public static int ItemRewards_FollowerToken_Armor615 = 114807;
        public static int ItemRewards_FollowerToken_Weapon615 = 114616;
        public static int ItemRewards_FollowerToken_Armor630 = 114806;
        public static int ItemRewards_FollowerToken_Weapon630 = 114081;
        public static int ItemRewards_FollowerToken_Armor645 = 114746;
        public static int ItemRewards_FollowerToken_Weapon645 = 114622;

        public static readonly List<int> ItemRewards_FollowerTokens = new List<int>
        {
            ItemRewards_FollowerToken_ArmorEnhancement, //Armor Enhancement Token
            ItemRewards_FollowerToken_WeaponEnhancement, //Weapon Enhancement Token

            ItemRewards_FollowerToken_Weapon615, //war-ravaged-weaponry
            ItemRewards_FollowerToken_Armor615, //war-ravaged-armor-set

            ItemRewards_FollowerToken_Armor630, //blackrock-armor-set 
            ItemRewards_FollowerToken_Weapon630, //blackrock-weaponry

            ItemRewards_FollowerToken_Armor645, //goredrenched-armor-set
            ItemRewards_FollowerToken_Weapon630, //goredrenched-weaponry

            114822, //heavily-reinforced-armor-enhancement (+9)
            114131, //power-overrun-weapon-enhancement (+9)
        };

        public static readonly List<int> ItemRewards_Contracts = new List<int>
        {
            114825, //Contract: Ulna Thresher
            112848, //Contract: Daleera Moonfang
            114826, //Contract: Bruma Swiftstone
            112737, //Contract: Ka'la of the Frostwolves
        };

        //118354 = Follower Re-training Certificate
        //122272 = Follower Ability Retraining
        //122273 = Follower Trait Retraining
        //123858 = Follower Retraining Scroll Case
        public static readonly int ItemReward_FollowerRetrainingCertificate = 118354;
        public static readonly int ItemReward_FollowerAbilityRetrainingManual = 122272;
        public static readonly int ItemReward_FollowerTraitRetrainingGuide = 122273;
        public static readonly int ItemReward_FollowerRetrainingScrollCase = 123858;
        //
        public static readonly List<int> ItemRewards_FollowerRetraining = new List<int>
        {
            ItemReward_FollowerRetrainingCertificate,
            ItemReward_FollowerAbilityRetrainingManual,
            ItemReward_FollowerTraitRetrainingGuide,
            ItemReward_FollowerRetrainingScrollCase
        };

        public static readonly int ItemReward_TraitDancing = 118474;
        public static readonly int ItemReward_TraitHearthstone = 118475;
        public static readonly int ItemReward_TraitSuntouchedFeather = 122275;
        public static readonly int ItemReward_TraitOgreBuddyHandbook = 122580;
        public static readonly int ItemReward_TraitGreaseMonkeyGuide = 122583;
        public static readonly int ItemReward_TraitWinningwithWildlings = 122584;
        public static readonly int ItemReward_TraitGuidetoArakkoaRelations = 122582;

        public static readonly List<int> ItemRewards_FollowerTraits = new List<int>
        {
            ItemReward_TraitDancing,
            ItemReward_TraitHearthstone,
            ItemReward_TraitSuntouchedFeather,
            ItemReward_TraitOgreBuddyHandbook,
            ItemReward_TraitGreaseMonkeyGuide,
            ItemReward_TraitWinningwithWildlings,
            ItemReward_TraitGuidetoArakkoaRelations
        };

        public static readonly int ItemReward_ElementalRune = 115510;
        public static readonly int ItemReward_AbrogatorStone = 115280;
        public static readonly int ItemReward_PrimalSpirit = 120945;
        public static readonly int ItemReward_SavageBlood = 118472;

        //115510 = Elemental Rune
        //115280 = Abrogator Stone
        //120945 = Primal Spirit
        //118472 = Salvage Blood



        //118474 = Supreme Manual of Dance
        //118475 = Hearthstone Strategy Guide
        //122275 = Sun-touched Feather of Rukhmar
        public static readonly List<int> ItemRewards_RushOrders = new List<int>
        {
            (int)RushOrders.AlchemyLab,
            (int)RushOrders.EnchantersStudy,
            (int)RushOrders.Forge,
            (int)RushOrders.Tailoring,
            (int)RushOrders.Gem,
            (int)RushOrders.Tannery,
            (int)RushOrders.Scribe,
            (int)RushOrders.Engineering,
        };
        #endregion
    }
}