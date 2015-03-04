using System;
using System.Collections.Generic;
using System.Linq;
using Herbfunk.GarrisonBase.Garrison.Objects;
using Styx.Common;
using Styx.Helpers;
using Styx.WoWInternals;

namespace Herbfunk.GarrisonBase
{
    static partial class LuaCommands
    {
        public static int StringToInt(string str)
        {
            return str.ToInt32();
        }

        public static Follower GetFollowerInfo(int followerId)
        {
            GarrisonBase.Debug("LuaCommand: GetFollowerInfo {0}", followerId);
            String lua =
                "local RetInfo = {}; Temp = {}; local followers = C_Garrison.GetFollowers();" +
                String.Format(
                    "for i,f in ipairs(followers) do " +
                    "local followerID = (f.garrFollowerID) and tonumber(f.garrFollowerID) or f.followerID;" +
                    "if (followerID == {0}) then " +
                    "Temp[0] = followerID;" +
                    "Temp[1] = f.name;" +
                    "Temp[2] = f.status;" +
                    "Temp[3] = f.ClassSpecName;" +
                    "Temp[4] = f.quality;" +
                    "Temp[5] = f.level;" +
                    "Temp[6] = f.isCollected ;" +
                    "Temp[7] = f.iLevel;" +
                    "Temp[8] = f.levelXP;" +
                    "Temp[9] = f.xp;" +
                    "end;" +
                    "end;" +
                    "for j_=0,9 do table.insert(RetInfo,tostring(Temp[j_]));end; " +
                    "return unpack(RetInfo)", followerId);
            List<String> follower = Lua.GetReturnValues(lua);
            String Name = follower[1];
            String Status = follower[2];
            String ClassSpecName = follower[3];
            String quality = follower[4];
            int level = follower[5].ToInt32();
            bool isCollected = follower[6].ToBoolean();
            int iLevel = follower[7].ToInt32();
            int xp = follower[8].ToInt32();
            int levelXp = follower[9].ToInt32();
            List<FollowerAbility> abilities = GetFollowerAbilities(followerId);
            return new Follower(followerId, Name, level, iLevel, xp, levelXp, Status, quality, abilities);
        }
        public static List<FollowerAbility> GetFollowerAbilities(int followerId)
        {
            GarrisonBase.Debug("LuaCommand: GetFollowerAbilities {0}", followerId);

            string lua = String.Format("local abilities = C_Garrison.GetFollowerAbilities(\"{0}\");", followerId) +
                  "local RetInfo = {};" +
                  "for a = 1, #abilities do " +
                    "local ability= abilities[a];" +
                    "table.insert(RetInfo,tostring(ability.id));" +
                  "end;" +
                  "return unpack(RetInfo)";
            List<string> info = Lua.GetReturnValues(lua);

            List<FollowerAbility> Abilities = new List<FollowerAbility>();
            foreach (var s in info)
            {
                int iS = Convert.ToInt32(s);
                FollowerAbility ability = Follower.FollowerAbilites.FirstOrDefault(a => a.ID == iS);
                Abilities.Add(ability);
            }

            return Abilities;
        }
        public static List<int> GetAllFollowerIDs()
        {
            GarrisonBase.Debug("LuaCommand: GetAllFollowerIDs");

            String lua =
                "local RetInfo = {}; local followers = C_Garrison.GetFollowers();" +
                 "for i,f in ipairs(followers) do " +
                    "local followerID = (f.garrFollowerID) and tonumber(f.garrFollowerID) or f.followerID;" +
                    "table.insert(RetInfo,tostring(followerID));" +
                "end;" +
                "return unpack(RetInfo)";
            List<int> info = Lua.GetReturnValues(lua).ConvertAll(s => s.ToInt32());
            return info;
        }

        public static bool IsFollowerCollected(int id)
        {
            GarrisonBase.Debug("LuaCommand: IsFollowerCollected {0}", id);
            String lua = String.Format("return C_Garrison.IsFollowerCollected(\"{0}\");", id);
            var info = Lua.GetReturnValues(lua).FirstOrDefault().ToInt32();
            return info == 1;
        }
    }
}
