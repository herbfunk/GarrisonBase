local wipe = wipe 
local next = next 
local pairs = pairs 
local GARRISON_CURRENCY = GARRISON_CURRENCY 
local GarrisonMissionFrame = GarrisonMissionFrame 
local After = C_Timer.After 
local GARRISON_FOLLOWER_IN_PARTY = GARRISON_FOLLOWER_IN_PARTY 
local GetFramesRegisteredForEvent = GetFramesRegisteredForEvent 
local CANCEL = CANCEL 
local HybridScrollFrame_GetOffset = HybridScrollFrame_GetOffset 
local GetCurrencyInfo = GetCurrencyInfo 
local MissionPageFollowers = GarrisonMissionFrame.MissionTab.MissionPage.Followers 
local RED_FONT_COLOR_CODE = RED_FONT_COLOR_CODE 
local FONT_COLOR_CODE_CLOSE = FONT_COLOR_CODE_CLOSE 
local GARRISON_FOLLOWER_MAX_LEVEL = GARRISON_FOLLOWER_MAX_LEVEL 
local _, _, garrison_currency_texture = GetCurrencyInfo(GARRISON_CURRENCY) 
garrison_currency_texture = "|T" .. garrison_currency_texture .. ":0|t" 
local time_texture = "|TInterface\\Icons\\spell_holy_borrowedtime:0|t" 
local button_suffixes = { '', 'Yield' } 
local top_for_mission = {} 
local top_for_mission_dirty = true 
local filtered_followers = {} 
local filtered_followers_count 
local filtered_followers_dirty = true 
local lastmissionId=0 
local event_frame = CreateFrame("Frame") 
local RegisterEvent = event_frame.RegisterEvent 
local UnregisterEvent = event_frame.UnregisterEvent 
local CheckPartyForProfessionFollowers 
local gmm_buttons = {} 
local gmm_frames = {} 
local mission_page_pending_click 
local min, max = {}, {} 
local top = {{}, {}, {}, {}} 
local top_yield = {{}, {}, {}, {}} 
local best_modes = { "success" } 
local preserve_mission_page_followers = {} 
local events_filtered_followers_dirty = { GARRISON_FOLLOWER_LIST_UPDATE = true,GARRISON_FOLLOWER_XP_CHANGED = true, GARRISON_FOLLOWER_REMOVED = true, } 
local events_top_for_mission_dirty = { GARRISON_MISSION_NPC_OPENED = true,GARRISON_MISSION_LIST_UPDATE = true } 
event_frame:SetScript("OnEvent", function(self, event, arg1) 
   if events_top_for_mission_dirty[event] or events_filtered_followers_dirty[event] then 
      top_for_mission_dirty = true 
      filtered_followers_dirty = true 
      lastmissionId=0 
   end 
   if event == "GARRISON_LANDINGPAGE_SHIPMENTS" then 
      event_frame:UnregisterEvent("GARRISON_LANDINGPAGE_SHIPMENTS") 
   end 
end) 
for event in pairs(events_top_for_mission_dirty) do event_frame:RegisterEvent(event) end 
for event in pairs(events_filtered_followers_dirty) do event_frame:RegisterEvent(event) end 
local function FindBestFollowersForMission(mission, followers, mode) 
   local followers_count = #followers 
   for idx = 1, 3 do 
      wipe(top[idx]) 
      wipe(top_yield[idx]) 
   end 
   local slots = mission.numFollowers 
   if slots > followers_count then return end 
   local event_handlers = { GetFramesRegisteredForEvent("GARRISON_FOLLOWER_LIST_UPDATE") } 
   for idx = 1, #event_handlers do UnregisterEvent(event_handlers[idx], "GARRISON_FOLLOWER_LIST_UPDATE") end 
   local mission_id = mission.missionID 
   local party_followers_count = #MissionPageFollowers 
   if party_followers_count > 0 then 
      for party_idx = 1, party_followers_count do 
         preserve_mission_page_followers[party_idx] = MissionPageFollowers[party_idx].info 
      end 
   end 
   if C_Garrison.GetNumFollowersOnMission(mission_id) > 0 then 
      for idx = 1, #followers do 
         C_Garrison.RemoveFollowerFromMission(mission_id, followers[idx].followerID) 
      end 
   end 
   for idx = 1, slots do 
      max[idx] = followers_count - slots + idx 
      min[idx] = nil 
   end 
   for idx = slots+1, 3 do 
      max[idx] = followers_count + 1 
      min[idx] = followers_count + 1 
   end 
   local best_modes_count = 1 
   local gr_rewards 
   local xp_only_rewards 
   local currencyID=0 
   local itemID=0 
   local itemID2=0 
   local goldReward=0 
   local followerXP=0 
   local apexReward=0 
   local RewardCurrenciesIDs = {}
   local RewardCurrenciesAmounts = {}
   local RewardItems = {}
   
   for _, reward in pairs(mission.rewards) do 
      if reward.currencyID == GARRISON_CURRENCY then gr_rewards = true end 
      if reward.followerXP and xp_only_rewards == nil then xp_only_rewards = true end 
      if not reward.followerXP then xp_only_rewards = false end 

	  if reward.currencyID and reward.title and reward.title ~= "Money Reward" then 
		table.insert(RewardCurrenciesIDs, reward.currencyID) 
		table.insert(RewardCurrenciesAmounts, reward.quantity) 
	  end 
	  if reward.itemID then 
		table.insert(RewardItems, reward.itemID) 
	  end 
      if reward.itemID then 
        if itemID == 0 then 
            itemID=reward.itemID 
        else 
            itemID2=reward.itemID 
        end 
      end 
      if reward.title and reward.title == "Money Reward" then goldReward=reward.quantity end 
      if reward.title and reward.title == "Bonus Follower XP" then followerXP=reward.followerXP end 
      if reward.title and reward.title == "Currency Reward" then 
        if reward.currencyID==823 then 
            apexReward=reward.quantity 
        end 
      end 
   end 
   if gr_rewards and mode ~= "mission_list" then 
      best_modes_count = best_modes_count + 1 
      best_modes[best_modes_count] = "gr_yield" 
   end 
   for i1 = 1, max[1] do 
      local follower1 = followers[i1] 
      local follower1_id = follower1.followerID 
      local follower1_maxed = follower1.levelXP == 0 and 1 or 0 
      local follower1_level = follower1.level if follower1_level == GARRISON_FOLLOWER_MAX_LEVEL then follower1_level = follower1.iLevel end 
      for i2 = min[2] or (i1 + 1), max[2] do 
         local follower2_maxed = 0 
         local follower2 = followers[i2] 
         local follower2_id 
         local follower2_level = 0 
         if follower2 then 
            follower2_id = follower2.followerID 
            if follower2.levelXP == 0 then follower2_maxed = 1 end 
            follower2_level = follower2.level if follower2_level == GARRISON_FOLLOWER_MAX_LEVEL then follower2_level = follower2.iLevel end 
         end 
         for i3 = min[3] or (i2 + 1), max[3] do 
            local follower3_maxed = 0 
            local follower3 = followers[i3] 
            local follower3_id 
            local follower3_level = 0 
            if follower3 then 
               follower3_id = follower3.followerID 
               if follower3.levelXP == 0 then follower3_maxed = 1 end 
               follower3_level = follower3.level if follower3_level == GARRISON_FOLLOWER_MAX_LEVEL then follower3_level = follower3.iLevel end 
            end 
            local followers_maxed = follower1_maxed + follower2_maxed + follower3_maxed 
            local follower_level_total = follower1_level + follower2_level + follower3_level 
            if xp_only_rewards and slots == followers_maxed then break end 

            -- Assign followers to mission
            if not C_Garrison.AddFollowerToMission(mission_id, follower1_id) then --[[ error handling! ]] end 
            if follower2 and not C_Garrison.AddFollowerToMission(mission_id, follower2_id) then --[[ error handling! ]] end 
            if follower3 and not C_Garrison.AddFollowerToMission(mission_id, follower3_id) then --[[ error handling! ]] end 
            -- Calculate result
            local totalTimeString, totalTimeSeconds, isMissionTimeImproved, successChance, partyBuffs, isEnvMechanicCountered, xpBonus, materialMultiplier = C_Garrison.GetPartyMissionInfo(mission_id) 
            isEnvMechanicCountered = isEnvMechanicCountered and 1 or 0 
            local buffCount = #partyBuffs 

            for best_modes_idx = 1, best_modes_count do 
               local mode = best_modes[best_modes_idx] 
               local gr_yield 
               if mode == 'gr_yield' then 
                  gr_yield = materialMultiplier * successChance 
               end 

               for idx = 1, 3 do 
                  local top_list 
                  if mode == 'gr_yield' then 
                     top_list = top_yield 
                  else 
                     top_list = top 
                  end 
                  local current = top_list[idx] 

                  local found 
                  repeat 
                     if slots == followers_maxed then xpBonus = 0 end 
                     if mode == "gr_yield" and materialMultiplier == 1 then 
                        break 
                     end 
                     if not current[1] then found = true break end 

                     if mode == 'gr_yield' then 
                        local c_gr_yield = current.gr_yield 
                        if c_gr_yield < gr_yield then found = true break end 
                        if c_gr_yield > gr_yield then break end 
                     end 

                     local cSuccessChance = current.successChance 
                     if cSuccessChance < successChance then found = true break end 
                     if cSuccessChance > successChance then break end 

                     if gr_rewards then 
                        local cMaterialMultiplier = current.materialMultiplier 
                        if cMaterialMultiplier < materialMultiplier then found = true break end 
                        if cMaterialMultiplier > materialMultiplier then break end 
                     end 

                     local c_followers_maxed = current.followers_maxed 
                     if c_followers_maxed > followers_maxed then found = true break end 
                     if c_followers_maxed < followers_maxed then break end 

                     local cXpBonus = current.xpBonus 
                     if cXpBonus < xpBonus then found = true break end 
                     if cXpBonus > xpBonus then break end 

                     local cTotalTimeSeconds = current.totalTimeSeconds 
                     if cTotalTimeSeconds > totalTimeSeconds then found = true break end 
                     if cTotalTimeSeconds < totalTimeSeconds then break end 

                     local c_follower_level_total = current.follower_level_total 
                     if c_follower_level_total > follower_level_total then found = true break end 
                     if c_follower_level_total < follower_level_total then break end 

                     local cBuffCount = current.buffCount 
                     if cBuffCount > buffCount then found = true break end 
                     if cBuffCount < buffCount then break end 

                     local cIsEnvMechanicCountered = current.isEnvMechanicCountered 
                     if cIsEnvMechanicCountered > isEnvMechanicCountered then found = true break end 
                     if cIsEnvMechanicCountered < isEnvMechanicCountered then break end 
                  until true 
                  if found then 
                     local new = top_list[4] 
                     new[1] = follower1 
                     new[2] = follower2 
                     new[3] = follower3 
                     local totalfollowers=1 
                     if new[2] then 
                        totalfollowers=2 
                        if new[3] then 
                            totalfollowers=3 
                        end 
                     end 
                      
                     new.rewards=mission.rewards 
                     new.apexReward=apexReward 
                     new.followerXP=followerXP 
                     new.currencyID=currencyID 
                     new.goldReward=goldReward 
                     new.itemID=itemID 
                     new.itemID2=itemID2 
                     new.totalfollowers=totalfollowers 
                     new.successChance = successChance 
                     new.materialMultiplier = materialMultiplier 
                     new.gr_rewards = gr_rewards 
                     new.xpBonus = xpBonus 
                     new.totalTimeSeconds = totalTimeSeconds 
                     new.isMissionTimeImproved = isMissionTimeImproved 
                     new.followers_maxed = followers_maxed 
                     new.buffCount = buffCount 
                     new.isEnvMechanicCountered = isEnvMechanicCountered 
                     new.gr_yield = gr_yield 
                     new.no_reward = xp_only_rewards and slots == followers_maxed 
                     new.follower_level_total = follower_level_total 
					 new.RewardItems = RewardItems 
					 new.RewardCurrenciesAmounts = RewardCurrenciesAmounts 
					 new.RewardCurrenciesIDs = RewardCurrenciesIDs 
                     table.insert(top_list, idx, new) 
                     top_list[5] = nil 
                     break 
                  end 
               end 
            end 

            -- Unasssign
            C_Garrison.RemoveFollowerFromMission(mission_id, follower1_id) 
            if follower2 then C_Garrison.RemoveFollowerFromMission(mission_id, follower2_id) end 
            if follower3 then C_Garrison.RemoveFollowerFromMission(mission_id, follower3_id) end 
         end 
      end 
   end 
   top.gr_rewards = gr_rewards 

   if party_followers_count > 0 then 
      for party_idx = 1, party_followers_count do 
         if preserve_mission_page_followers[party_idx] then 
            GarrisonMissionPage_SetFollower(MissionPageFollowers[party_idx], preserve_mission_page_followers[party_idx]) 
         end 
      end 
   end 

   for idx = 1, #event_handlers do RegisterEvent(event_handlers[idx], "GARRISON_FOLLOWER_LIST_UPDATE") end 
   lastmissionId=mission.missionID 
end 

local function SortFollowersByLevel(a, b) 
   local a_level = a.level 
   local b_level = b.level 
   if a_level ~= b_level then return a_level > b_level end 
   return a.iLevel > b.iLevel 
end 

function GetFilteredFollowers() 
   if not filtered_followers_dirty then 
      return filtered_followers, filtered_followers_count 
   end 
   local followers = C_Garrison.GetFollowers() 
   wipe(filtered_followers) 
   filtered_followers_count = 0 
   for idx = 1, #followers do 
      local follower = followers[idx] 
      repeat 
         if not follower.isCollected then break end 

         local status = follower.status 
         if status and status ~= GARRISON_FOLLOWER_IN_PARTY then break end 

         filtered_followers_count = filtered_followers_count + 1 
         filtered_followers[filtered_followers_count] = follower 
      until true 
   end 
   table.sort(filtered_followers, SortFollowersByLevel) 
   filtered_followers_dirty = false 
   top_for_mission_dirty = true 
   return filtered_followers, filtered_followers_count 
end 

local function SetTeamButtonText(button, top_entry) 
   if top_entry.successChance then 
      local xp_bonus 
      if top_entry.no_reward then 
         xp_bonus = 'NO' 
      else 
         xp_bonus = top_entry.xpBonus > 0 and top_entry.xpBonus or '' 
      end 
      local xp_bonus_icon = xp_bonus ~= '' and " |TInterface\\Icons\\XPBonus_Icon:0|t" or '' 
      local material_multiplier = top_entry.gr_rewards and top_entry.materialMultiplier > 1 and top_entry.materialMultiplier or '' 
      local material_multiplier_icon = material_multiplier ~= '' and garrison_currency_texture or '' 

      button:SetFormattedText( 
         "%d%%\n%s%s%s%s%s", 
         top_entry.successChance, 
         xp_bonus, xp_bonus_icon, 
         material_multiplier, material_multiplier_icon, 
         top_entry.isMissionTimeImproved and time_texture or "" 
      ) 
   else 
      button:SetText("") 
   end 
end 

local available_missions = {} 
local function BestForCurrentSelectedMission() 
   local missionInfo = GarrisonMissionFrame.MissionTab.MissionPage.missionInfo 
   local mission_id = missionInfo.missionID 

   -- print("Mission ID:", mission_id)

   local filtered_followers, filtered_followers_count = GetFilteredFollowers() 

   C_Garrison.GetAvailableMissions(available_missions) 
   local mission 
   for idx = 1, #available_missions do 
      if available_missions[idx].missionID == mission_id then 
         mission = available_missions[idx] 
         break 
      end 
   end 


   FindBestFollowersForMission(mission, filtered_followers) 

   for suffix_idx = 1, #button_suffixes do 
      local suffix = button_suffixes[suffix_idx] 
      for idx = 1, 3 do 
         local button = gmm_buttons['MissionPage' .. suffix .. idx] 
         local top_entry 
         if suffix == 'Yield' then 
            if top.gr_rewards then 
               top_entry = top_yield[idx] 
            else 
               top_entry = false 
            end 
         else 
            top_entry = top[idx] 
         end 

         if top_entry ~= false then 
            button[1] = top_entry[1] and top_entry[1].followerID or nil 
            button[2] = top_entry[2] and top_entry[2].followerID or nil 
            button[3] = top_entry[3] and top_entry[3].followerID or nil 
            SetTeamButtonText(button, top_entry) 
            button:Hide() 
         end 
      end 
   end 

   if mission_page_pending_click then 
      gmm_buttons['MissionPage' .. mission_page_pending_click]:Click() 
      mission_page_pending_click = nil 
   end 
end 

function GetBestYield(mission_id) 
	local filtered_followers, filtered_followers_count = GetFilteredFollowers() 
	C_Garrison.GetAvailableMissions(available_missions) 
   local mission 
   for idx = 1, #available_missions do 
      if available_missions[idx].missionID == mission_id then 
         mission = available_missions[idx] 
         break 
      end 
   end 



   FindBestFollowersForMission(mission, filtered_followers, "mission_list") 
   return top[1] 
end 

function GarrisonBaseTest() 
	return 1 
end 

local function MissionPage_PartyButtonOnClick(self) 
   if self[1] then 
      event_frame:UnregisterEvent("GARRISON_FOLLOWER_LIST_UPDATE") 
      for idx = 1, #MissionPageFollowers do 
         GarrisonMissionPage_ClearFollower(MissionPageFollowers[idx]) 
      end 
      for idx = 1, #MissionPageFollowers do 
         local followerFrame = MissionPageFollowers[idx] 
         local follower = self[idx] 
         if follower then 
            local followerInfo = C_Garrison.GetFollowerInfo(follower) 
            GarrisonMissionPage_SetFollower(followerFrame, followerInfo) 
         end 
      end 
      event_frame:RegisterEvent("GARRISON_FOLLOWER_LIST_UPDATE") 
   end 
   GarrisonMissionPage_UpdateMissionForParty() 
end 

local function MissionList_PartyButtonOnClick(self) 
   return self:GetParent():Click() 
end 
local function GarrisonMissionList_Update_More() 
   local self = GarrisonMissionFrame.MissionTab.MissionList 
   local scrollFrame = self.listScroll 
   local buttons = scrollFrame.buttons 
   local numButtons = #buttons 
   if self.showInProgress then 
      for i = 1, numButtons do 
         gmm_buttons['MissionList' .. i]:Hide() 
         buttons[i]:SetAlpha(1) 
      end 
      return 
   end 
   local missions = self.availableMissions 
   local numMissions = #missions 
   if numMissions == 0 then return end 
   if top_for_mission_dirty then 
      wipe(top_for_mission) 
      top_for_mission_dirty = false 
   end 
   local missions = self.availableMissions 
   local offset = HybridScrollFrame_GetOffset(scrollFrame) 
   local filtered_followers, filtered_followers_count = GetFilteredFollowers() 
   local more_missions_to_cache 
   local _, garrison_resources = GetCurrencyInfo(GARRISON_CURRENCY) 
   for i = 1, numButtons do 
      local button = buttons[i] 
      local alpha = 1 
      local index = offset + i 
      if index <= numMissions then 
         local mission = missions[index] 
         local gmm_button = gmm_buttons['MissionList' .. i] 
         if (mission.numFollowers > filtered_followers_count) or (mission.cost > garrison_resources) then 
            button:SetAlpha(0.3) 
            gmm_button:SetText("") 
         else 
            local top_for_this_mission = top_for_mission[mission.missionID] 
            if not top_for_this_mission then 
               if more_missions_to_cache then 
                  more_missions_to_cache = more_missions_to_cache + 1 
               else 
                  more_missions_to_cache = 0 
                  FindBestFollowersForMission(mission, filtered_followers, "mission_list") 
                  local top1 = top[1] 
                  top_for_this_mission = {} 
                  top_for_this_mission.successChance = top1.successChance 
                  if top_for_this_mission.successChance then 
                     top_for_this_mission.materialMultiplier = top1.materialMultiplier 
                     top_for_this_mission.gr_rewards = top1.gr_rewards 
                     top_for_this_mission.xpBonus = top1.xpBonus 
                     top_for_this_mission.isMissionTimeImproved = top1.isMissionTimeImproved 
                     top_for_this_mission.no_reward = top1.no_reward 
                  end 
                  top_for_mission[mission.missionID] = top_for_this_mission 
               end 
            end 

            if top_for_this_mission then 
               SetTeamButtonText(gmm_button, top_for_this_mission) 
            else 
               gmm_button:SetText("...") 
            end 
            button:SetAlpha(1) 
         end 
         gmm_button:Hide() 
      end 
   end 

   if more_missions_to_cache and more_missions_to_cache > 0 then 
      After(0.001, GarrisonMissionList_Update_More) 
   end 
end 
hooksecurefunc("GarrisonMissionList_Update", GarrisonMissionList_Update_More) 
hooksecurefunc(GarrisonMissionFrame.MissionTab.MissionList.listScroll, "update", GarrisonMissionList_Update_More) 


local function MissionPage_ButtonsInit() 
   local prev 
   for suffix_idx = 1, #button_suffixes do 
      local suffix = button_suffixes[suffix_idx] 
      for idx = 1, 3 do 
         local name = 'MissionPage' .. suffix .. idx 
         if not gmm_buttons[name] then 
            local set_followers_button = CreateFrame("Button", nil, GarrisonMissionFrame.MissionTab.MissionPage, "UIPanelButtonTemplate") 
            set_followers_button:SetText(idx) 
            set_followers_button:SetWidth(20) 
            set_followers_button:SetHeight(20) 
            if not prev then 
               set_followers_button:SetPoint("TOPLEFT", GarrisonMissionFrame.MissionTab.MissionPage, "TOPRIGHT", 0, 0) 
            else 
               set_followers_button:SetPoint("TOPLEFT", prev, "BOTTOMLEFT", 0, 0) 
            end 
            set_followers_button:SetScript("OnClick", MissionPage_PartyButtonOnClick) 
            prev = set_followers_button 
            gmm_buttons[name] = set_followers_button 
         end 
      end 
   end 
   gmm_buttons['MissionPageYield1']:SetPoint("TOPLEFT", gmm_buttons['MissionPage3'], "BOTTOMLEFT", 0, -50) 
end 

local function MissionList_ButtonsInit() 
   local level_anchor = GarrisonMissionFrame.MissionTab.MissionList.listScroll 
   local blizzard_buttons = GarrisonMissionFrame.MissionTab.MissionList.listScroll.buttons 
   for idx = 1, #blizzard_buttons do 
      if not gmm_buttons['MissionList' .. idx] then 
         local blizzard_button = blizzard_buttons[idx] 
         local reward = blizzard_button.Rewards[1] 
         for point_idx = 1, reward:GetNumPoints() do 
            local point, relative_to, relative_point, x, y = reward:GetPoint(point_idx) 
            if point == "RIGHT" then 
               x = x - 60 
               reward:SetPoint(point, relative_to, relative_point, x, y) 
               break 
            end 
         end 
         local set_followers_button = CreateFrame("Button", nil, blizzard_button, "UIPanelButtonTemplate") 
         set_followers_button:SetText(idx) 
         set_followers_button:SetWidth(20) 
         set_followers_button:SetHeight(20) 
         set_followers_button:SetPoint("LEFT", blizzard_button, "RIGHT", -65, 0) 
         set_followers_button:SetScript("OnClick", MissionList_PartyButtonOnClick) 
         gmm_buttons['MissionList' .. idx] = set_followers_button 
      end 
   end 
end 



MissionPage_ButtonsInit() 
MissionList_ButtonsInit() 
hooksecurefunc("GarrisonMissionPage_ShowMission", BestForCurrentSelectedMission) 



   
-- Globals deliberately exposed for people outside
function GMM_Click(button_name) 
   local button = gmm_buttons[button_name] 
   if button then button:Click() end 
end 
