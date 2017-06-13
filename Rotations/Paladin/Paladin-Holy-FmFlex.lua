local raidHealthFrame = {}
local Party1Buffs = { }
local Party2Buffs = { }
local Party3Buffs = { }
local Party4Buffs = { }
local buffLastStateParty1  = { }
local buffLastStateParty2  = { }
local buffLastStateParty3  = { }
local buffLastStateParty4  = { }
local Party1Debuff = {}
local Party2Debuff = {}
local Party3Debuff = {}
local Party4Debuff = {}
local debuffLastStateParty1  = { }
local debuffLastStateParty2  = { }
local debuffLastStateParty3  = { }
local debuffLastStateParty4  = { }


local function updateRaidHealth(self, event)
		local size = GetNumGroupMembers()
			if(GetNumGroupMembers() > 24) then
				size=24
			end
		local isRaid = IsInRaid ("player");
		if(not isRaid) then
			size = size -1;
		end
		for i = 1, size do
			unit="party"..i
			if(isRaid) then
				unit ="raid"..i
			end
				
			local role = UnitGroupRolesAssigned(unit)
			local guid = UnitGUID(unit)
			local health = 0;		
			local maxHealth = 100;
			local percHealth = 0;
			local blue = 0; 
			local red = 0;
			local green = 0;
			local range = false;
			if (guid ~= nil) then
				health = UnitHealth(unit);		
				maxHealth = UnitHealthMax(unit);
				percHealth = ceil((health / maxHealth) * 100)
			end	
			range = UnitInRange(unit);
			if range then 
				green = 0;
			else
				green = 1;
			end
			if role == "TANK" then
				local isTanking = UnitThreatSituation(unit);
				if isTanking == 2 or isTanking == 3 then
					blue = .3;
				else
					blue = 0;
				end
			elseif role == "HEALER" then
				blue = .5;
			else
				blue = 1;
			end
			local strHealth = "0.0" .. percHealth;
			if (percHealth >= 10) then
				strHealth = "0." .. percHealth;
			end
				red = tonumber(strHealth)
			if (percHealth == 100) then
				raidHealthFrame[i].t:SetColorTexture(255, green, blue, alphaColor)
			else
				raidHealthFrame[i].t:SetColorTexture(red, green, blue, alphaColor)
			end		
		end	
end

local function updateParty1Buffs()
	for _, auraId in pairs(buffs) do
        local buff = "Unitbuff";
        local auraName = GetSpellInfo(auraId)

        if auraName == nil then
            if (buffLastStateParty1[auraId] ~= "BuffOff") then
                Party1Buffs[auraId].t:SetColorTexture(1, 1, 1, alphaColor)
                Party1Buffs[auraId].t:SetAllPoints(false)
                buffLastStateParty1[auraId] = "BuffOff"          
            end    
            return
        end

		local name, rank, icon, count, dispelType, duration, expires, caster, isStealable, nameplateShowPersonal, spellID, canApplyAura, isBossDebuff, _, nameplateShowAll, timeMod, value1, value2, value3 = UnitBuff("party1", auraName)

		if (name == auraName) then -- We have Aura up and Aura ID is matching our list
			local getTime = GetTime()
			local remainingTime = math.floor(expires - getTime + 0.5) 	

			if (buffLastStateParty1[auraId] ~= "BuffOn" .. count..remainingTime) then
				local green = 0
					local blue = 0
					local strcount = "0.0"..count;
				local strbluecount = "0.0"..remainingTime;
						
				if(remainingTime <= 0 or remainingTime <= -0 or remainingTime == 0) then 
					blue = 0
					strbluecount = 0
				end

				if (count >= 10) then
					strcount = "0."..count;
				end

				if(remainingTime >= 10) then
				   strbluecount = "0."..remainingTime;
				end

				green = tonumber(strcount)
				blue = tonumber(strbluecount)

				Party1Buffs[auraId].t:SetColorTexture(0, green, blue, alphaColor)
				Party1Buffs[auraId].t:SetAllPoints(false)
				buffLastStateParty1[auraId] = "BuffOn" .. count..remainingTime
            end
        else
            if (buffLastStateParty1[auraId] ~= "BuffOff") then
                Party1Buffs[auraId].t:SetColorTexture(1, 1, 1, alphaColor)
                Party1Buffs[auraId].t:SetAllPoints(false)
                buffLastStateParty1[auraId] = "BuffOff"              
            end
        end
    end
end


local function updateParty2Buffs()
	for _, auraId in pairs(buffs) do
        local buff = "Unitbuff";
        local auraName = GetSpellInfo(auraId)

        if auraName == nil then
            if (buffLastStateParty2[auraId] ~= "BuffOff") then
                Party2Buffs[auraId].t:SetColorTexture(1, 1, 1, alphaColor)
                Party2Buffs[auraId].t:SetAllPoints(false)
                buffLastStateParty2[auraId] = "BuffOff"          
            end    
            return
        end

		local name, rank, icon, count, dispelType, duration, expires, caster, isStealable, nameplateShowPersonal, spellID, canApplyAura, isBossDebuff, _, nameplateShowAll, timeMod, value1, value2, value3 = UnitBuff("party2", auraName)

		if (name == auraName) then -- We have Aura up and Aura ID is matching our list
			local getTime = GetTime()
			local remainingTime = math.floor(expires - getTime + 0.5) 	

			if (buffLastStateParty2[auraId] ~= "BuffOn" .. count..remainingTime) then
				local green = 0
					local blue = 0
					local strcount = "0.0"..count;
				local strbluecount = "0.0"..remainingTime;
						
				if(remainingTime <= 0 or remainingTime <= -0 or remainingTime == 0) then 
					blue = 0
					strbluecount = 0
				end

				if (count >= 10) then
					strcount = "0."..count;
				end

				if(remainingTime >= 10) then
				   strbluecount = "0."..remainingTime;
				end

				green = tonumber(strcount)
				blue = tonumber(strbluecount)

				Party2Buffs[auraId].t:SetColorTexture(0, green, blue, alphaColor)
				Party2Buffs[auraId].t:SetAllPoints(false)
				buffLastStateParty2[auraId] = "BuffOn" .. count..remainingTime
            end
        else
            if (buffLastStateParty2[auraId] ~= "BuffOff") then
                Party2Buffs[auraId].t:SetColorTexture(1, 1, 1, alphaColor)
                Party2Buffs[auraId].t:SetAllPoints(false)
                buffLastStateParty2[auraId] = "BuffOff"              
            end
        end
    end
end

local function updateParty3Buffs()
	for _, auraId in pairs(buffs) do
        local buff = "Unitbuff";
        local auraName = GetSpellInfo(auraId)

        if auraName == nil then
            if (buffLastStateParty3[auraId] ~= "BuffOff") then
                Party3Buffs[auraId].t:SetColorTexture(1, 1, 1, alphaColor)
                Party3Buffs[auraId].t:SetAllPoints(false)
                buffLastStateParty3[auraId] = "BuffOff"          
            end    
            return
        end

		local name, rank, icon, count, dispelType, duration, expires, caster, isStealable, nameplateShowPersonal, spellID, canApplyAura, isBossDebuff, _, nameplateShowAll, timeMod, value1, value2, value3 = UnitBuff("party3", auraName)

		if (name == auraName) then -- We have Aura up and Aura ID is matching our list
			local getTime = GetTime()
			local remainingTime = math.floor(expires - getTime + 0.5) 	

			if (buffLastStateParty3[auraId] ~= "BuffOn" .. count..remainingTime) then
				local green = 0
					local blue = 0
					local strcount = "0.0"..count;
				local strbluecount = "0.0"..remainingTime;
						
				if(remainingTime <= 0 or remainingTime <= -0 or remainingTime == 0) then 
					blue = 0
					strbluecount = 0
				end

				if (count >= 10) then
					strcount = "0."..count;
				end

				if(remainingTime >= 10) then
				   strbluecount = "0."..remainingTime;
				end

				green = tonumber(strcount)
				blue = tonumber(strbluecount)

				Party3Buffs[auraId].t:SetColorTexture(0, green, blue, alphaColor)
				Party3Buffs[auraId].t:SetAllPoints(false)
				buffLastStateParty3[auraId] = "BuffOn" .. count..remainingTime
            end
        else
            if (buffLastStateParty3[auraId] ~= "BuffOff") then
                Party3Buffs[auraId].t:SetColorTexture(1, 1, 1, alphaColor)
                Party3Buffs[auraId].t:SetAllPoints(false)
                buffLastStateParty3[auraId] = "BuffOff"              
            end
        end
    end
end

local function updateParty4Buffs()
	for _, auraId in pairs(buffs) do
        local buff = "Unitbuff";
        local auraName = GetSpellInfo(auraId)

        if auraName == nil then
            if (buffLastStateParty4[auraId] ~= "BuffOff") then
                Party4Buffs[auraId].t:SetColorTexture(1, 1, 1, alphaColor)
                Party4Buffs[auraId].t:SetAllPoints(false)
                buffLastStateParty4[auraId] = "BuffOff"          
            end    
            return
        end

		local name, rank, icon, count, dispelType, duration, expires, caster, isStealable, nameplateShowPersonal, spellID, canApplyAura, isBossDebuff, _, nameplateShowAll, timeMod, value1, value2, value3 = UnitBuff("party4", auraName)

		if (name == auraName) then -- We have Aura up and Aura ID is matching our list
			local getTime = GetTime()
			local remainingTime = math.floor(expires - getTime + 0.5) 	

			if (buffLastStateParty4[auraId] ~= "BuffOn" .. count..remainingTime) then
				local green = 0
					local blue = 0
					local strcount = "0.0"..count;
				local strbluecount = "0.0"..remainingTime;
						
				if(remainingTime <= 0 or remainingTime <= -0 or remainingTime == 0) then 
					blue = 0
					strbluecount = 0
				end

				if (count >= 10) then
					strcount = "0."..count;
				end

				if(remainingTime >= 10) then
				   strbluecount = "0."..remainingTime;
				end

				green = tonumber(strcount)
				blue = tonumber(strbluecount)

				Party4Buffs[auraId].t:SetColorTexture(0, green, blue, alphaColor)
				Party4Buffs[auraId].t:SetAllPoints(false)
				buffLastStateParty4[auraId] = "BuffOn" .. count..remainingTime
            end
        else
            if (buffLastStateParty4[auraId] ~= "BuffOff") then
                Party4Buffs[auraId].t:SetColorTexture(1, 1, 1, alphaColor)
                Party4Buffs[auraId].t:SetAllPoints(false)
                buffLastStateParty4[auraId] = "BuffOff"              
            end
        end
    end
end

local function updateParty1Debuffs(self, event)
    
	for _, auraId in pairs(debuffs) do
        local buff = "UnitDebuff";
		local auraName = GetSpellInfo(auraId)

        if auraName == nil then
            if (debuffLastStateParty1[auraId] ~= "DebuffOff") then
                Party1Debuff[auraId].t:SetColorTexture(1, 1, 1, alphaColor)
                Party1Debuff[auraId].t:SetAllPoints(false)
                debuffLastStateParty1[auraId] = "DebuffOff"               
            end
    
            return
        end
        
		--print("Getting debuff for Id = " .. auraName)
		
        local name, rank, icon, count, debuffType, duration, expirationTime, unitCaster, isStealable, shouldConsolidate, spellId, canApplyAura, isBossDebuff, value1, value2, value3 = UnitDebuff("party1", auraName, nil, "PLAYER|HARMFUL")

		if (name == auraName) then -- We have Aura up and Aura ID is matching our list					
            local getTime = GetTime()
            local remainingTime = math.floor(expirationTime - getTime + 0.5) 	

			if (debuffLastStateParty1[auraId] ~= "DebuffOn" .. count .. remainingTime) then
                local green = 0
                local blue = 0             
                local strcount = "0.0" .. count;
                local strbluecount = "0.0" .. remainingTime;
                
                if (count >= 10) then
                    strcount = "0." .. count;
                end

                if(remainingTime >= 10) then
                   strbluecount = "0." .. remainingTime
                end

                green = tonumber(strcount)
                blue = tonumber(strbluecount)

                Party1Debuff[auraId].t:SetColorTexture(0, green, blue, alphaColor)
				Party1Debuff[auraId].t:SetAllPoints(false)
                --print("[" .. buff .. "] " .. auraName.. " " .. count .. " Green: " .. green .. " Blue: " .. blue)
                debuffLastStateParty1[auraId] = "DebuffOn" .. count .. remainingTime
            end
        else
            if (debuffLastStateParty1[auraId] ~= "DebuffOff") then
                Party1Debuff[auraId].t:SetColorTexture(1, 1, 1, alphaColor)
                Party1Debuff[auraId].t:SetAllPoints(false)
                debuffLastStateParty1[auraId] = "DebuffOff"
                --print("[" .. buff .. "] " .. auraName.. " Off")
            end
        end

    end
end

local function updateParty2Debuffs(self, event)
    
	for _, auraId in pairs(debuffs) do
        local buff = "UnitDebuff";
		local auraName = GetSpellInfo(auraId)

        if auraName == nil then
            if (debuffLastStateParty2[auraId] ~= "DebuffOff") then
                Party2Debuff[auraId].t:SetColorTexture(1, 1, 1, alphaColor)
                Party2Debuff[auraId].t:SetAllPoints(false)
                debuffLastStateParty2[auraId] = "DebuffOff"               
            end
    
            return
        end
        
		--print("Getting debuff for Id = " .. auraName)
		
        local name, rank, icon, count, debuffType, duration, expirationTime, unitCaster, isStealable, shouldConsolidate, spellId, canApplyAura, isBossDebuff, value1, value2, value3 = UnitDebuff("party2", auraName, nil, "PLAYER|HARMFUL")

		if (name == auraName) then -- We have Aura up and Aura ID is matching our list					
            local getTime = GetTime()
            local remainingTime = math.floor(expirationTime - getTime + 0.5) 	

			if (debuffLastStateParty2[auraId] ~= "DebuffOn" .. count .. remainingTime) then
                local green = 0
                local blue = 0             
                local strcount = "0.0" .. count;
                local strbluecount = "0.0" .. remainingTime;
                
                if (count >= 10) then
                    strcount = "0." .. count;
                end

                if(remainingTime >= 10) then
                   strbluecount = "0." .. remainingTime
                end

                green = tonumber(strcount)
                blue = tonumber(strbluecount)

                Party2Debuff[auraId].t:SetColorTexture(0, green, blue, alphaColor)
				Party2Debuff[auraId].t:SetAllPoints(false)
                --print("[" .. buff .. "] " .. auraName.. " " .. count .. " Green: " .. green .. " Blue: " .. blue)
                debuffLastStateParty2[auraId] = "DebuffOn" .. count .. remainingTime
            end
        else
            if (debuffLastStateParty2[auraId] ~= "DebuffOff") then
                Party2Debuff[auraId].t:SetColorTexture(1, 1, 1, alphaColor)
                Party2Debuff[auraId].t:SetAllPoints(false)
                debuffLastStateParty2[auraId] = "DebuffOff"
                --print("[" .. buff .. "] " .. auraName.. " Off")
            end
        end

    end
end

local function updateParty3Debuffs(self, event)
    
	for _, auraId in pairs(debuffs) do
        local buff = "UnitDebuff";
		local auraName = GetSpellInfo(auraId)

        if auraName == nil then
            if (debuffLastStateParty3[auraId] ~= "DebuffOff") then
                Party3Debuff[auraId].t:SetColorTexture(1, 1, 1, alphaColor)
                Party3Debuff[auraId].t:SetAllPoints(false)
                debuffLastStateParty3[auraId] = "DebuffOff"               
            end
    
            return
        end
        
		--print("Getting debuff for Id = " .. auraName)
		
        local name, rank, icon, count, debuffType, duration, expirationTime, unitCaster, isStealable, shouldConsolidate, spellId, canApplyAura, isBossDebuff, value1, value2, value3 = UnitDebuff("party3", auraName, nil, "PLAYER|HARMFUL")

		if (name == auraName) then -- We have Aura up and Aura ID is matching our list					
            local getTime = GetTime()
            local remainingTime = math.floor(expirationTime - getTime + 0.5) 	

			if (debuffLastStateParty3[auraId] ~= "DebuffOn" .. count .. remainingTime) then
                local green = 0
                local blue = 0             
                local strcount = "0.0" .. count;
                local strbluecount = "0.0" .. remainingTime;
                
                if (count >= 10) then
                    strcount = "0." .. count;
                end

                if(remainingTime >= 10) then
                   strbluecount = "0." .. remainingTime
                end

                green = tonumber(strcount)
                blue = tonumber(strbluecount)

                Party3Debuff[auraId].t:SetColorTexture(0, green, blue, alphaColor)
				Party3Debuff[auraId].t:SetAllPoints(false)
                --print("[" .. buff .. "] " .. auraName.. " " .. count .. " Green: " .. green .. " Blue: " .. blue)
                debuffLastStateParty3[auraId] = "DebuffOn" .. count .. remainingTime
            end
        else
            if (debuffLastStateParty3[auraId] ~= "DebuffOff") then
                Party3Debuff[auraId].t:SetColorTexture(1, 1, 1, alphaColor)
                Party3Debuff[auraId].t:SetAllPoints(false)
                debuffLastStateParty3[auraId] = "DebuffOff"
                --print("[" .. buff .. "] " .. auraName.. " Off")
            end
        end

    end
end

local function updateParty4Debuffs(self, event)
    
	for _, auraId in pairs(debuffs) do
        local buff = "UnitDebuff";
		local auraName = GetSpellInfo(auraId)

        if auraName == nil then
            if (debuffLastStateParty4[auraId] ~= "DebuffOff") then
                Party4Debuff[auraId].t:SetColorTexture(1, 1, 1, alphaColor)
                Party4Debuff[auraId].t:SetAllPoints(false)
                debuffLastStateParty4[auraId] = "DebuffOff"               
            end
    
            return
        end
        
		--print("Getting debuff for Id = " .. auraName)
		
        local name, rank, icon, count, debuffType, duration, expirationTime, unitCaster, isStealable, shouldConsolidate, spellId, canApplyAura, isBossDebuff, value1, value2, value3 = UnitDebuff("party4", auraName, nil, "PLAYER|HARMFUL")

		if (name == auraName) then -- We have Aura up and Aura ID is matching our list					
            local getTime = GetTime()
            local remainingTime = math.floor(expirationTime - getTime + 0.5) 	

			if (debuffLastStateParty4[auraId] ~= "DebuffOn" .. count .. remainingTime) then
                local green = 0
                local blue = 0             
                local strcount = "0.0" .. count;
                local strbluecount = "0.0" .. remainingTime;
                
                if (count >= 10) then
                    strcount = "0." .. count;
                end

                if(remainingTime >= 10) then
                   strbluecount = "0." .. remainingTime
                end

                green = tonumber(strcount)
                blue = tonumber(strbluecount)

                Party4Debuff[auraId].t:SetColorTexture(0, green, blue, alphaColor)
				Party4Debuff[auraId].t:SetAllPoints(false)
                --print("[" .. buff .. "] " .. auraName.. " " .. count .. " Green: " .. green .. " Blue: " .. blue)
                debuffLastStateParty4[auraId] = "DebuffOn" .. count .. remainingTime
            end
        else
            if (debuffLastStateParty4[auraId] ~= "DebuffOff") then
                Party4Debuff[auraId].t:SetColorTexture(1, 1, 1, alphaColor)
                Party4Debuff[auraId].t:SetAllPoints(false)
                debuffLastStateParty4[auraId] = "DebuffOff"
                --print("[" .. buff .. "] " .. auraName.. " Off")
            end
        end

    end
end

local function updateRaidSize(self, event)
	partySize = GetNumGroupMembers();
	if GetNumGroupMembers() > 24 then
		partySize = 24
	end
	local red = 0
    local green = 0	
	local blue = 1
	local strSize = "0.0" .. partySize;
	
	if IsInRaid("player") then 
		green = 0;
	else
		green = 1;
	end
	if IsSpellInRange("Judgment", "targettarget") or IsSpellInRange("Judgment", "target") then
		blue = 0;
	else
		blue = 1;
	end
	if (GetNumGroupMembers() >= 10) then
		strSize = "0." .. partySize;
	end
	red = tonumber(strSize)
	if (partySize == 0) then
		raidHealthFrame[0].t:SetColorTexture(255, green, blue, alphaColor)
	else
		raidHealthFrame[0].t:SetColorTexture(red, green, blue, alphaColor)
	end		
	
end
local function InitializeThree()
	local i = 0
		raidHealthFrame[i] = CreateFrame("frame", "", parent)
		raidHealthFrame[i]:SetSize(size, size)
		raidHealthFrame[i]:SetPoint("TOPLEFT", size*i, -size * 20 )   --  row 1-20,  column 19
		raidHealthFrame[i].t = raidHealthFrame[i]:CreateTexture()        
		raidHealthFrame[i].t:SetColorTexture(1, 1, 1, alphaColor)
		raidHealthFrame[i].t:SetAllPoints(raidHealthFrame[i])
		raidHealthFrame[i]:Show()
		raidHealthFrame[i]:SetScript("OnUpdate", updateRaidSize)
	for i = 1, 24 do		
		raidHealthFrame[i] = CreateFrame("frame", "", parent)
		raidHealthFrame[i]:SetSize(size, size)
		raidHealthFrame[i]:SetPoint("TOPLEFT", size*i, -size * 20 )   --  row 1-20,  column 19
		raidHealthFrame[i].t = raidHealthFrame[i]:CreateTexture()        
		raidHealthFrame[i].t:SetColorTexture(1, 1, 1, alphaColor)
		raidHealthFrame[i].t:SetAllPoints(raidHealthFrame[i])
		raidHealthFrame[i]:Show()
		raidHealthFrame[i]:SetScript("OnUpdate", updateRaidHealth)
	end
	
	i = 0
	for _, buffId in pairs(buffs) do
		Party1Buffs[buffId] = CreateFrame("frame","", parent)
        Party1Buffs[buffId]:SetSize(size, size)
        Party1Buffs[buffId]:SetPoint("TOPLEFT", i * size, -size * 12)                            -- column 13 [Target Buffs]
		Party1Buffs[buffId].t = Party1Buffs[buffId]:CreateTexture()
        Party1Buffs[buffId].t:SetColorTexture(1, 1, 1, alphaColor)
        Party1Buffs[buffId].t:SetAllPoints(Party1Buffs[buffId])
        Party1Buffs[buffId]:Show()
        Party1Buffs[buffId]:SetScript("OnUpdate", updateParty1Buffs)
        i = i + 1
	end
	
	i = 0
	for _, buffId in pairs(buffs) do
		Party2Buffs[buffId] = CreateFrame("frame","", parent)
        Party2Buffs[buffId]:SetSize(size, size)
        Party2Buffs[buffId]:SetPoint("TOPLEFT", i * size, -size * 13)                            -- column 14 [Target Buffs]
		Party2Buffs[buffId].t = Party2Buffs[buffId]:CreateTexture()
        Party2Buffs[buffId].t:SetColorTexture(1, 1, 1, alphaColor)
        Party2Buffs[buffId].t:SetAllPoints(Party2Buffs[buffId])
        Party2Buffs[buffId]:Show()
        Party2Buffs[buffId]:SetScript("OnUpdate", updateParty2Buffs)
        i = i + 1
	end
	
	i = 0
	for _, buffId in pairs(buffs) do
		Party3Buffs[buffId] = CreateFrame("frame","", parent)
        Party3Buffs[buffId]:SetSize(size, size)
        Party3Buffs[buffId]:SetPoint("TOPLEFT", i * size, -size * 14)                            -- column 15 [Target Buffs]
		Party3Buffs[buffId].t = Party3Buffs[buffId]:CreateTexture()
        Party3Buffs[buffId].t:SetColorTexture(1, 1, 1, alphaColor)
        Party3Buffs[buffId].t:SetAllPoints(Party3Buffs[buffId])
        Party3Buffs[buffId]:Show()
        Party3Buffs[buffId]:SetScript("OnUpdate", updateParty3Buffs)
        i = i + 1
	end
	
	i = 0
	for _, buffId in pairs(buffs) do
		Party4Buffs[buffId] = CreateFrame("frame","", parent)
        Party4Buffs[buffId]:SetSize(size, size)
        Party4Buffs[buffId]:SetPoint("TOPLEFT", i * size, -size * 15)                            -- column 13 [Target Buffs]
		Party4Buffs[buffId].t = Party4Buffs[buffId]:CreateTexture()
        Party4Buffs[buffId].t:SetColorTexture(1, 1, 1, alphaColor)
        Party4Buffs[buffId].t:SetAllPoints(Party4Buffs[buffId])
        Party4Buffs[buffId]:Show()
        Party4Buffs[buffId]:SetScript("OnUpdate", updateParty4Buffs)
        i = i + 1
	end
	
	i = 0
	for _, debuffId in pairs(debuffs) do
		Party1Debuff[debuffId] = CreateFrame("frame","", parent)
		Party1Debuff[debuffId]:SetSize(size, size)
		Party1Debuff[debuffId]:SetPoint("TOPLEFT", i * size, -size * 16)         -- row 4, column 1+ [Spell In Range]
		Party1Debuff[debuffId].t = Party1Debuff[debuffId]:CreateTexture()        
		Party1Debuff[debuffId].t:SetColorTexture(1, 1, 1, alphaColor)
		Party1Debuff[debuffId].t:SetAllPoints(Party1Debuff[debuffId])
		Party1Debuff[debuffId]:Show()		               
		Party1Debuff[debuffId]:SetScript("OnUpdate", updateParty1Debuffs)
		i = i + 1
	end
	
	i = 0
	for _, debuffId in pairs(debuffs) do
		Party2Debuff[debuffId] = CreateFrame("frame","", parent)
		Party2Debuff[debuffId]:SetSize(size, size)
		Party2Debuff[debuffId]:SetPoint("TOPLEFT", i * size, -size * 17)         -- row 4, column 1+ [Spell In Range]
		Party2Debuff[debuffId].t = Party2Debuff[debuffId]:CreateTexture()        
		Party2Debuff[debuffId].t:SetColorTexture(1, 1, 1, alphaColor)
		Party2Debuff[debuffId].t:SetAllPoints(Party2Debuff[debuffId])
		Party2Debuff[debuffId]:Show()		               
		Party2Debuff[debuffId]:SetScript("OnUpdate", updateParty2Debuffs)
		i = i + 1
	end
	
	i = 0
	for _, debuffId in pairs(debuffs) do
		Party3Debuff[debuffId] = CreateFrame("frame","", parent)
		Party3Debuff[debuffId]:SetSize(size, size)
		Party3Debuff[debuffId]:SetPoint("TOPLEFT", i * size, -size * 18)         -- row 4, column 1+ [Spell In Range]
		Party3Debuff[debuffId].t = Party3Debuff[debuffId]:CreateTexture()        
		Party3Debuff[debuffId].t:SetColorTexture(1, 1, 1, alphaColor)
		Party3Debuff[debuffId].t:SetAllPoints(Party3Debuff[debuffId])
		Party3Debuff[debuffId]:Show()		               
		Party3Debuff[debuffId]:SetScript("OnUpdate", updateParty3Debuffs)
		i = i + 1
	end
	
	i = 0
	for _, debuffId in pairs(debuffs) do
		Party4Debuff[debuffId] = CreateFrame("frame","", parent)
		Party4Debuff[debuffId]:SetSize(size, size)
		Party4Debuff[debuffId]:SetPoint("TOPLEFT", i * size, -size * 19)         -- row 4, column 1+ [Spell In Range]
		Party4Debuff[debuffId].t = Party4Debuff[debuffId]:CreateTexture()        
		Party4Debuff[debuffId].t:SetColorTexture(1, 1, 1, alphaColor)
		Party4Debuff[debuffId].t:SetAllPoints(Party4Debuff[debuffId])
		Party4Debuff[debuffId]:Show()		               
		Party4Debuff[debuffId]:SetScript("OnUpdate", updateParty4Debuffs)
		i = i + 1
	end
end