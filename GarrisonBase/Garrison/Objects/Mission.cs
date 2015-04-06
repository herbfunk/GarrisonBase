using System;
using System.Collections.Generic;
using System.Reflection;
using Herbfunk.GarrisonBase.Garrison.Enums;
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
                        if (CacheStaticLookUp.DictItemRewards_CharacterTokens.ContainsKey(ItemIdReward) &&
                            CacheStaticLookUp.DictItemRewards_CharacterTokens[ItemIdReward].Item2<BaseSettings.CurrentSettings.MissionReward_CharacterToken_ItemLevel)
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
                        if (CacheStaticLookUp.ItemRewards_FollowerToken_Armor615 == ItemIdReward &&
                            !BaseSettings.CurrentSettings.MissionReward_FollowerToken_ArmorSet615)
                        {
                            _priority = 0;
                        }
                        else if (CacheStaticLookUp.ItemRewards_FollowerToken_Armor630 == ItemIdReward &&
                            !BaseSettings.CurrentSettings.MissionReward_FollowerToken_ArmorSet630)
                        {
                            _priority = 0;
                        }
                        else if (CacheStaticLookUp.ItemRewards_FollowerToken_Armor645 == ItemIdReward &&
                            !BaseSettings.CurrentSettings.MissionReward_FollowerToken_ArmorSet645)
                        {
                            _priority = 0;
                        }
                        else if (CacheStaticLookUp.ItemRewards_FollowerToken_Weapon615 == ItemIdReward &&
                            !BaseSettings.CurrentSettings.MissionReward_FollowerToken_WeaponSet615)
                        {
                            _priority = 0;
                        }
                        else if (CacheStaticLookUp.ItemRewards_FollowerToken_Weapon630 == ItemIdReward &&
                            !BaseSettings.CurrentSettings.MissionReward_FollowerToken_WeaponSet630)
                        {
                            _priority = 0;
                        }
                        else if (CacheStaticLookUp.ItemRewards_FollowerToken_Weapon645 == ItemIdReward &&
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

                if (CacheStaticLookUp.DictItemRewards_CharacterTokens.ContainsKey(reward.ItemId))
                    RewardTypes |= RewardTypes.CharacterToken;
                else if (CacheStaticLookUp.ItemRewards_FollowerTokens.Contains(reward.ItemId))
                    RewardTypes |= RewardTypes.FollowerToken;
                else if (CacheStaticLookUp.ItemRewards_FollowerRetraining.Contains(reward.ItemId))
                    RewardTypes |= RewardTypes.RetrainingCertificate;
                else if (CacheStaticLookUp.ItemRewards_Contracts.Contains(reward.ItemId))
                    RewardTypes |= RewardTypes.FollowerContract;
                else if (CacheStaticLookUp.ItemRewards_FollowerTraits.Contains(reward.ItemId))
                    RewardTypes |= RewardTypes.FollowerTrait;
                else if (CacheStaticLookUp.ItemRewards_RushOrders.Contains(reward.ItemId))
                    RewardTypes |= RewardTypes.RushOrder;
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
    }
}