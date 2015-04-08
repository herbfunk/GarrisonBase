using System;
using System.Collections.Generic;
using Herbfunk.GarrisonBase.Garrison.Objects;
using Styx.Helpers;
using Styx.WoWInternals;

namespace Herbfunk.GarrisonBase
{
    static partial class LuaCommands
    {

       
        public static void MissionCompleteRollChest(int missionId)
        {
            GarrisonBase.Debug("LuaCommand: MissionCompleteRollChest");
            Lua.DoString(String.Format("C_Garrison.MissionBonusRoll(\"{0}\")", missionId));
        }

        public static void MissionCompleteMarkComplete(int missionId)
        {
            GarrisonBase.Debug("LuaCommand: MissionCompleteMarkComplete");
            Lua.DoString(String.Format("C_Garrison.MarkMissionComplete(\"{0}\")", missionId));
        }


        public static Mission GetMissionInfo(int missionId)
        {
            GarrisonBase.Debug("LuaCommand: GetMissionInfo {0}", missionId);
            String lua =
                "local b = {}; local am = {}; local RetInfo = {}; local cpt = 0; C_Garrison.GetAvailableMissions(am);" +
                String.Format(
                    "local location, xp, environment, environmentDesc, environmentTexture, locPrefix, isExhausting, enemies = C_Garrison.GetMissionInfo(\"{0}\");" +
                    "for idx = 1, #am do " +
                    "if am[idx].missionID == {0} then " +
                    "b[0] = am[idx].description;" +
                    "b[1] = am[idx].cost;" +
                    "b[2] = am[idx].duration;" +
                    "b[3] = am[idx].durationSeconds;" +
                    "b[4] = am[idx].level;" +
                    "b[5] = am[idx].type;" +
                    "b[6] = am[idx].locPrefix;" +
                    "b[7] = am[idx].state;" +
                    "b[8] = am[idx].iLevel;" +
                    "b[9] = am[idx].name;" +
                    "b[10] = am[idx].location;" +
                    "b[11] = am[idx].isRare;" +
                    "b[12] = am[idx].typeAtlas;" +
                    "b[13] = am[idx].missionID;" +
                    "b[14] = am[idx].numFollowers;" +
                    "b[15] = xp;" +
                    "b[16] = am[idx].numRewards;" +
                    "local itemID=0;"+
                    "local itemID2=0;" +
                   "local goldReward=0;"+
                   "local followerXP=0;"+
                   "local apexReward=0;"+
                   "local garrisonReward=0;" +
                   "local currencyId=0;" +
                   "local currencyAmount=0;" +
                    "b[17] = environment;" +
                    "for _, reward in pairs(am[idx].rewards) do " +
                        "if reward.itemID then " +
                            "if itemID == 0 then " +
                                "itemID=reward.itemID;" +
                            "else " +
                                "itemID2=reward.itemID;" +
                            "end;" +
                        "end;" +
                        "if reward.title and reward.title == 'Money Reward' then goldReward=reward.quantity; end;" +
                        "if reward.title and reward.title == 'Bonus Follower XP' then followerXP=reward.followerXP; end;" +
                        "if reward.title and reward.title == 'Currency Reward' then " +
                            "if reward.currencyID==823 then " +
                            "   apexReward=reward.quantity;" +
                            "end;" +
                            "if reward.currencyID==824 then " +
                            "   garrisonReward=reward.quantity;" +
                            "end;" +
                            "if reward.currencyID~=823 and reward.currencyID~=824 then " +
                            "   currencyAmount=reward.quantity;" +
                            "   currencyId=reward.currencyID;" +
                            "end;" +
                        "end;" +
                     "end;" +
                     "b[18] = itemID;" +
                     "b[19] = itemID2;" +
                     "b[20] = goldReward;" +
                     "b[21] = followerXP;" +
                     "b[22] = apexReward;" +
                     "b[23] = garrisonReward;" +
                     "b[24] = currencyId;" +
                     "b[25] = currencyAmount;" +
                    "cpt = 25;" +
                    "end;" +
                    "end;"
                    , missionId) +
                "for j_=0,cpt do table.insert(RetInfo,tostring(b[j_]));end; " +
                "return unpack(RetInfo)";
            //
            List<string> mission = Lua.GetReturnValues(lua);

            string description = mission[0];
            int cost = mission[1].ToInt32();
            //mission[2] = this.duration;
            int durationSeconds = mission[3].ToInt32();
            int level = mission[4].ToInt32();
            string type = mission[5];
            //mission[6] = this.locPrefix; 
            int state = mission[7].ToInt32();
            int ilevel = mission[8].ToInt32();
            string name = mission[9];
            string location = mission[10];
            bool isRare = mission[11].ToBoolean();
            //mission[12] = this.typeAtlas; 
            string missionID = mission[13];
            int numFollowers = mission[14].ToInt32();
            string xp = mission[15];
            int numRewards = mission[16].ToInt32();
            string environment = mission[17];

            //"b[18] = itemID;" +
            //         "b[19] = goldReward;" +
            //         "b[20] = followerXP;" +
            //         "b[21] = apexReward;" +
            //         "b[22] = garrisonReward;" +

            int itemID = mission[18].ToInt32();
            int itemID2 = mission[19].ToInt32();
            int goldReward = mission[20].ToInt32();
            int followerXP = mission[21].ToInt32();
            int apexReward = mission[22].ToInt32();
            int garrisonReward = mission[23].ToInt32();
            int currencyId = mission[24].ToInt32();
            int currencyAmount = mission[25].ToInt32();

            //List<string> enemies = GetMissionAbilities(missionId);
            //List<CombatAbilities> abilities = new List<CombatAbilities>();
            //foreach (var a in enemies)
            //{
            //    int value_int = Convert.ToInt32(a);
            //    if (Enum.IsDefined(typeof(CombatAbilities), value_int))
            //    {
            //        abilities.Add((CombatAbilities)value_int);
            //    }
            //}

            return new Mission(cost, description,
                durationSeconds, level, ilevel,
                isRare, location, missionId,
                name, numFollowers, numRewards,
                state, type, xp, environment, garrisonReward, followerXP, goldReward, apexReward, itemID, itemID2, currencyId, currencyAmount);
        }


        public static int GetMissionBonusChance(int missionId)
        {
            GarrisonBase.Debug("LuaCommand: GetMissionBonusChance");
            String lua = String.Format("return tostring(C_Garrison.GetRewardChance(\"{0}\"))", missionId);
            var ret = Lua.GetReturnValues(lua);
            if (ret[0] == "nil") return 0;
            return Convert.ToInt32(ret[0]);
        }

        public static double GetMissionBestSuccessAttempt(int missionId, out int[] followerIds)
        {
            GarrisonBase.Debug("LuaCommand: GetMissionBestSuccessAttempt");
            followerIds = new[] { 0, 0, 0 };
            string lua = String.Format(
                "local yieldInfo = {0}({1});",LuaEvents.SuccessFunctionString, missionId) +
                        "local Ret = {};" +
                        "table.insert(Ret,tonumber(yieldInfo.totalfollowers));" +
                        "table.insert(Ret,tonumber(yieldInfo[1].garrFollowerID));" +
                        "if (yieldInfo.totalfollowers>1) then table.insert(Ret,tonumber(yieldInfo[2].garrFollowerID)); end;" +
                        "if (yieldInfo.totalfollowers>2) then table.insert(Ret,tonumber(yieldInfo[3].garrFollowerID)); end;" +
                        "table.insert(Ret,yieldInfo.successChance);" +
                        "table.insert(Ret,tostring(yieldInfo.gr_rewards));" +
                        "table.insert(Ret,tostring(yieldInfo.goldReward));" +
                        "table.insert(Ret,tostring(yieldInfo.itemID));" +
                        "table.insert(Ret,tostring(yieldInfo.currencyID));" +
                        "table.insert(Ret,tostring(yieldInfo.followerXP));" +
                        "table.insert(Ret,tostring(yieldInfo.apexReward));" +
                         "return unpack(Ret)";
            List<string> retvalues = Lua.GetReturnValues(lua, "clicky.lua");
            double successChance = 0;
            try
            {
                if (retvalues.Count == 0)
                {
                    //failed!
                }
                else
                {
                    int totalfollowers = retvalues[0].ToInt32();
                    followerIds[0] = Convert.ToInt32(retvalues[1]);
                    int nextIndex = 2;
                    if (totalfollowers > 1)
                    {
                        followerIds[1] = Convert.ToInt32(retvalues[2]);
                        nextIndex++;

                        if (totalfollowers > 2)
                        {
                            followerIds[2] = Convert.ToInt32(retvalues[3]);
                            nextIndex++;
                        }
                    }
                    successChance = Convert.ToDouble(retvalues[nextIndex]);
                }
            }
            catch (Exception)
            {
                GarrisonBase.Debug("LuaCommandException: GetMissionBestSuccessAttempt");
            }
            

            return successChance;
        }
    }
}
