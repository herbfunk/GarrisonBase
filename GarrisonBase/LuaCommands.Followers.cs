using System;
using System.Collections.Generic;
using Styx.Helpers;
using Styx.WoWInternals;

namespace Herbfunk.GarrisonBase
{
    static partial class LuaCommands
    {

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
            var info = Lua.GetReturnVal<int>(lua, 0);
            return info == 1;
        }
    }
}
