using System;
using System.Collections.Generic;
using Herbfunk.GarrisonBase.Garrison.Enums;
using Styx.Helpers;
using Styx.WoWInternals;

namespace Herbfunk.GarrisonBase.Garrison.Objects
{
    public class Mission
    {
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
                        return BaseSettings.CurrentSettings.MissionRewardSuccessCharacterItems;
                    if (RewardTypes.HasFlag(RewardTypes.FollowerToken))
                        return BaseSettings.CurrentSettings.MissionRewardSuccessFollowerTokens;
                    if (RewardTypes.HasFlag(RewardTypes.FollowerTrait))
                        return BaseSettings.CurrentSettings.MissionRewardSuccessFollowerTraits;
                    if (RewardTypes.HasFlag(RewardTypes.FollowerContract))
                        return BaseSettings.CurrentSettings.MissionRewardSuccessContracts;
                    if (RewardTypes.HasFlag(RewardTypes.RetrainingCertificate))
                        return BaseSettings.CurrentSettings.MissionRewardSuccessFollowerRetraining;
                    if (RewardTypes.HasFlag(RewardTypes.RushOrder))
                        return BaseSettings.CurrentSettings.MissionRewardSuccessRushOrders;

                    return BaseSettings.CurrentSettings.MissionRewardSuccessItems;
                }



                if (RewardTypes.HasFlag(RewardTypes.ApexisCrystal))
                    return BaseSettings.CurrentSettings.MissionRewardSuccessApexis;

                if (RewardTypes.HasFlag(RewardTypes.Gold))
                    return BaseSettings.CurrentSettings.MissionRewardSuccessGold;

                if (RewardTypes.HasFlag(RewardTypes.Garrison))
                    return BaseSettings.CurrentSettings.MissionRewardSuccessGarrison;

                if (RewardTypes.HasFlag(RewardTypes.XP))
                    return BaseSettings.CurrentSettings.MissionRewardSuccessXp;




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


                    if (ItemIdReward > 0)
                    {
                        if (RewardTypes.HasFlag(RewardTypes.CharacterToken))
                            _priority += BaseSettings.CurrentSettings.MissionRewardPriorityCharacterItems;
                        else if (RewardTypes.HasFlag(RewardTypes.FollowerToken))
                            _priority += BaseSettings.CurrentSettings.MissionRewardPriorityFollowerTokens;
                        else if (RewardTypes.HasFlag(RewardTypes.FollowerTrait))
                            _priority += BaseSettings.CurrentSettings.MissionRewardPriorityFollowerTraits;
                        else if (RewardTypes.HasFlag(RewardTypes.FollowerContract))
                            _priority += BaseSettings.CurrentSettings.MissionRewardPriorityContracts;
                        else if (RewardTypes.HasFlag(RewardTypes.RetrainingCertificate))
                            _priority += BaseSettings.CurrentSettings.MissionRewardPriorityFollowerRetraining;
                        else if (RewardTypes.HasFlag(RewardTypes.RushOrder))
                            _priority += BaseSettings.CurrentSettings.MissionRewardPriorityRushOrders;
                        else
                        {
                            _priority += BaseSettings.CurrentSettings.MissionRewardPriorityItems;
                        }
                    }


                    if (RewardTypes.HasFlag(RewardTypes.XP))
                        _priority += BaseSettings.CurrentSettings.MissionRewardPriorityXp;

                    if (RewardTypes.HasFlag(RewardTypes.ApexisCrystal))
                        _priority += BaseSettings.CurrentSettings.MissionRewardPriorityApexis;

                    if (RewardTypes.HasFlag(RewardTypes.Gold))
                        _priority += BaseSettings.CurrentSettings.MissionRewardPriorityGold;

                    if (RewardTypes.HasFlag(RewardTypes.Garrison))
                        _priority += BaseSettings.CurrentSettings.MissionRewardPriorityGarrison;





                }
                return _priority;
            }
            set { _priority = value; }
        }
        private int _priority = -1;
        public Mission(int cost, string description, int durationSeconds, int level, int iLevel,
            bool isRare, string location, int missionId, string name, int numFollowers, int numRewards, int state,
            string type, int xp, int material, string succesChance, int xpBonus, bool success)
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
            Material = material.ToString();
            SuccessChance = succesChance;
            XpBonus = xpBonus;
            Success = success;
        }

        public Mission(int cost, string description, int durationSeconds, int level, int iLevel,
            bool isRare, string location, int missionId, string name, int numFollowers, int numRewards, int state,
            string type, string xp, string material, int garrisonReward, int xpReward, int goldReward, int apexisReward, int itemIdReward, int itemIdReward2, int currencyId, int currencyAmount, List<CombatAbilities> abilities)
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
            Abilities = abilities;
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
                if (CacheStaticLookUp.DictItemRewards_CharacterTokens.ContainsKey(ItemIdReward))
                    RewardTypes |= RewardTypes.CharacterToken;
                else if (CacheStaticLookUp.ItemRewards_FollowerTokens.Contains(ItemIdReward))
                    RewardTypes |= RewardTypes.FollowerToken;
                else if (CacheStaticLookUp.ItemRewards_FollowerRetraining.Contains(ItemIdReward))
                    RewardTypes |= RewardTypes.RetrainingCertificate;
                else if (CacheStaticLookUp.ItemRewards_Contracts.Contains(ItemIdReward))
                    RewardTypes |= RewardTypes.FollowerContract;
                else if (CacheStaticLookUp.ItemRewards_FollowerTraits.Contains(ItemIdReward))
                    RewardTypes |= RewardTypes.FollowerTrait;
                else if (CacheStaticLookUp.ItemRewards_RushOrders.Contains(ItemIdReward))
                    RewardTypes |= RewardTypes.RushOrder;
            }
            if (ItemIdReward2 > 0)
            {
                if (CacheStaticLookUp.DictItemRewards_CharacterTokens.ContainsKey(ItemIdReward2))
                    RewardTypes |= RewardTypes.CharacterToken;
                else if (CacheStaticLookUp.ItemRewards_FollowerTokens.Contains(ItemIdReward2))
                    RewardTypes |= RewardTypes.FollowerToken;
                else if (CacheStaticLookUp.ItemRewards_FollowerRetraining.Contains(ItemIdReward2))
                    RewardTypes |= RewardTypes.RetrainingCertificate;
                else if (CacheStaticLookUp.ItemRewards_Contracts.Contains(ItemIdReward2))
                    RewardTypes |= RewardTypes.FollowerContract;
                else if (CacheStaticLookUp.ItemRewards_FollowerTraits.Contains(ItemIdReward2))
                    RewardTypes |= RewardTypes.FollowerTrait;
                else if (CacheStaticLookUp.ItemRewards_RushOrders.Contains(ItemIdReward2))
                    RewardTypes |= RewardTypes.RushOrder;
            }
            if (currencyId > 0)
            {
                if (currencyId == 994) RewardTypes |= RewardTypes.SealOfTemperedFate;
                if (currencyId == 392) RewardTypes |= RewardTypes.HonorPoints;
                //
            }
        }

        public void Update()
        {
            Mission update = LuaCommands.GetMission(Id);
            Level = update.Level;
            Name = update.Name;
            Rewards = update.Rewards;
            State = update.State;
            Material = update.Material;
            SuccessChance = update.SuccessChance;
            XpBonus = update.XpBonus;
            Success = update.Success;
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
    }
}