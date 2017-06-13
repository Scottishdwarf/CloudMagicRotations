local Healingbuffs =  "Riptide"
local RaidBuffFrame = {}
local raidSizeFrame = nil
local targetInfoFrame = nil
local timerDBMFrames = {}
local party_units = {}
local raid_units = {}
local raidheal_cache = {}
local raidHealthFrame = {}
local RaidRole = {}
local RaidRange = {}
local lasthp = {}
local charUnit = {}
local raidBuff = {}
local raidBufftime = {}
local partySize = 0
local setBonusFrame = nil
local debufftarget = {locX = 0,locY = 30, debufftargetframes = {},dispellType = {
	[250] = {type1= 'Magic'},
	[251] = {type1= 'Magic'},
	[252] = {type1= 'Magic'},
	[577] = {type1= 'Magic'},
	[581] = {type1= 'Magic'},
	[102] = {type1= 'Magic'},
	[103] = {type1= 'Magic'},
	[104] = {type1= 'Magic'},
	[105] = {type1= 'Magic'},
	[253] =  {type1= 'Magic'},
	[254] = {type1= 'Magic'},
	[255] =  {type1= 'Magic'},
	[62] = {type1= 'Magic'},
	[63] =  {type1= 'Magic'},
	[64] = {type1= 'Magic'},
	[268] = {type1= 'Magic'},
	[269] = {type1= 'Magic'},
	[270] = {type1= 'Magic'},
	[65] = {type1= 'Magic'},
	[66] = {type1= 'Magic'},
	[70] = {type1= 'Magic'},
	[256] = {type1= 'Magic'},
	[257]= {type1= 'Magic'},
	[257] = {type1= 'Magic'},
	[259] = {type1= 'Magic'},
	[260] = {type1= 'Magic'},
	[261] =  {type1= 'Magic'},
	[262] =  {type1= 'Magic'},
    [263] = {type1= 'Magic'},
	[264] = {type1= 'Magic'},
	[265] =  {type1= 'Magic'},
	[71] = {type1= 'Magic'},
	[72] = {type1= 'Magic'},
	[73] = {type1= 'Magic'},
	[266] = {type1= 'Magic'},
    [267] = {type1= 'Magic'}},
	debuff = {targetdebuff}}
	local debuffraid = {locX = 0,locY = 30, debuffraidframes = {},
dispellType = {
	[250] = {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[251] = {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[252] = {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[577] = {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[581] = {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[102] = {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[103] = {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[104] = {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[105] = {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[253] =  {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[254] = {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[255] =  {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[62] = {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[63] =  {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[64] = {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[268] = {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[269] = {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[270] = {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[65] = {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[66] = {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[70] = {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[256] = {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[257]= {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[257] = {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[259] = {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[260] = {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[261] =  {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[262] =  {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
    [263] = {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[264] = {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[265] =  {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[71] = {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[72] = {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[73] = {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
	[266] = {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""},
    [267] = {type1= 'Magic', type2 = 'Curse', type3= "", type4 = ""}},
	debuff = {} }
local frame = CreateFrame("frame", "", parent)
frame:RegisterEvent("NAME_PLATE_UNIT_ADDED")
frame:RegisterEvent("UNIT_HEALTH_FREQUENT")
frame:RegisterEvent("RAID_ROSTER_UPDATE")
frame:RegisterEvent("GROUP_ROSTER_UPDATE")
frame:RegisterUnitEvent("UNIT_SPELL_HASTE","player")
frame:RegisterUnitEvent("UNIT_POWER","player")
frame:RegisterEvent("PLAYER_REGEN_DISABLED")
frame:RegisterEvent("PLAYER_REGEN_ENABLED")
frame:RegisterEvent("PLAYER_ENTERING_WORLD")
frame:RegisterEvent("CHAT_MSG_ADDON")
frame:RegisterUnitEvent("UNIT_HEALTH","player")
frame:RegisterEvent("PLAYER_EQUIPMENT_CHANGED")
frame:RegisterEvent("PLAYER_TARGET_CHANGED")
frame:RegisterEvent("PLAYER_ENTER_COMBAT")
frame:RegisterEvent("PLAYER_LEAVE_COMBAT")
frame:RegisterEvent("PLAYER_CONTROL_LOST")
frame:RegisterEvent("PLAYER_CONTROL_GAINED")
frame:RegisterEvent("ACTIONBAR_UPDATE_STATE")
frame:RegisterUnitEvent("UNIT_SPELLCAST_START","player")
frame:RegisterEvent("CURRENT_SPELL_CAST_CHANGED")
frame:RegisterUnitEvent("UNIT_SPELLCAST_SUCCEEDED","player")

local function updateFlag(self, event)
	if event == "PLAYER_CONTROL_GAINED" then
		flagFrame.t:SetColorTexture(0,0,0,alphaColor)
	end
	if event == "PLAYER_CONTROL_LOST" then
		flagFrame.t:SetColorTexture(1,0,0,alphaColor)
	end
end

local function UnitIsPartyUnit(unit)
	--print("checking :", unit)
	for _, v in next, party_units do
		if unit == v then return true end
	end
end

local function UnitIsRaidUnit(unit)
	for _, v in next, raid_units do
		if unit == v then return true end
	end
end

local function HealthChangedEvent(unit)
	local h = UnitHealth(unit)
	if h==lasthp[unit] then return end
	lasthp[unit]=h
	local m = UnitHealthMax(unit);
	h = (h / m)
	raidheal_cache[unit] = h
end

local function RangeCheck(unit)
	if LibStub("SpellRange-1.0").IsSpellInRange("Healing Wave", unit) == 1 then
		RaidRange[unit] = 1;
	else
		RaidRange[unit] = .5;
	end
end

local function RaidRoleCheck(unit)
	if UnitGroupRolesAssigned(unit) == "TANK" then
		RaidRole[unit] = 1;
	elseif UnitGroupRolesAssigned(unit) == "HEALER" then
		RaidRole[unit] = .5;
	else
		RaidRole[unit] = 0;
	end
end

local function UpdateRaidIndicators(unit)
	if UnitIsRaidUnit(unit) or UnitIsPartyUnit(unit) then
		if UnitInParty ("player") and not UnitInRaid ("player") then
		--print(unit,"needs heals")
			for i, key in pairs(party_units) do
				if key == unit then
					HealthChangedEvent(unit)
					RangeCheck(unit)
					RaidRoleCheck(unit)
					--print(unit, "is at :", raidheal_cache[unit])
					raidHealthFrame[i].t:SetColorTexture(raidheal_cache[unit], RaidRange[unit], RaidRole[unit], alphaColor)
				end	
			end
		end
		if UnitInRaid ("player") then 
			for i, key in pairs(raid_units) do
				if key == unit then
					HealthChangedEvent(unit)
					RangeCheck(unit)
					RaidRoleCheck(unit)
					--print(unit, "is at :", raidheal_cache[unit], " and At : ", i)
					raidHealthFrame[i].t:SetColorTexture(raidheal_cache[unit], RaidRange[unit], RaidRole[unit], alphaColor)
				end	
			end
		end
		if not UnitInRaid ("player") and not UnitInParty ("player") then
			for i = 1, 30 do
				raidHealthFrame[i].t:SetColorTexture(1, 0, 0, alphaColor)
			end
		end
	end
end

local function updateTotemsFrame()
	Totemsframe = 0
	TotemDuration = 0 
	for i = 1, 4 do
		haveTotem, name, startTime, duration, icon = GetTotemInfo(i)
		local Quesatime =  startTime + duration
		if haveTotem then
			if (name == "Spirit Wolf" or name == "Totem Mastery")
			and(startTime + duration - GetTime() > 1.5 ) then
				Totemsframe = 1;
				TotemDuration = startTime + duration - GetTime()
			end
		end
	end
	totemsFrame.t:SetColorTexture(Totemsframe, TotemDuration,0, alphaColor)
	
end


local function UpdateRaidBuffIndicators(unit)
		if select(7, UnitBuff(unit, Healingbuffs, "player")) == nil  then return end
		UpdateRaidBuffslot(unit,expires)
end

local function UpdateRaidBuffslot(unit,expires)
	for i = 1, 4 do 
		if raidBuff[i] == 0 then
			local slot = string.match (unit, "%d+")
			UpdateBuffTime(unit,expires,i)
			if i >= 10 then
				raidBuff[i] = tonumber("0." .. slot)
			else 
				raidBuff[i] = tonumber("0.0" .. slot)
			end
		end
	end
end

local function UpdateBuffTime(unit,expires,location)
	local remainingTime = math.floor(expires -  GetTime() + 0.5)
	if(remainingTime >= 10) then
		raidBufftime[i] = tonumber("0."..remainingTime);
	else
		raidBufftime[i] = tonumber("0.0"..remainingTime);
	end
end

local function updateRaidBuff(self, event)
	if not UnitInRaid ("player") and  UnitInParty ("player") then
		for key, _ in pairs(party_units) do
			UpdateRaidBuffIndicators(key)
		end
	end	
	if UnitInRaid ("player") then
		for key, _ in pairs(raid_units) do
			UpdateRaidBuffIndicators(key)
		end
	end
	for i=1, 4 do 
		if raidBuff[i] ~= nil then
			RaidBuffFrame[i].t:SetColorTexture(raidBuff[i], 1, raidBufftime[i], alphaColor)
			RaidBuffFrame[i].t:SetAllPoints(false)
		else
			RaidBuffFrame[i].t:SetColorTexture(1, 1, 1, alphaColor)
			RaidBuffFrame[i].t:SetAllPoints(false)
			raidBuff[i] = nil
			raidBufftime[i] = nil
		end
	end
end
local function register_unit(tbl, unit)
		table.insert(tbl, unit)
end
do
	for i = 1, 5 do
		register_unit(party_units, ("party%d"):format(i))
	end
	for	i = 1,30 do
		register_unit(raid_units, ("raid%d"):format(i))
	end
end



local function UdateRaidSizeFrame(self, event)
	partySize = GetNumGroupMembers() ;
	partySize = partySize /100
	--print("Party Size: ",partySize)
	if partySize > .30 then
		partySize = .30
	end
	--print("Partyupdate :",partySize)
	--print("Name plates :", PlatesOn)
	raidSizeFrame.t:SetColorTexture(partySize, 0, 0, alphaColor)
end
local DBMTIMER = {}

local function updateDBMFrames(elapsed)
	if(pullvalue and tickerdbm >= 0) then
		tickerdbm = tickerdbm - elapsed
		timerDBMFrames.t:SetColorTexture( 1,tickerdbm/10,0, alphaColor)
	else
		timerDBMFrames.t:SetColorTexture( 0,0,0, alphaColor)
	end
end
local timer 
local function DBMPull(prefix,msg,sender)
	_, _,_,_,_, _,_, instanceMapID, _ = GetInstanceInfo()
	if prefix == "D4" and select(1,strsplit("\t", msg)) == "PT"
    and (UnitInRaid(Ambiguate(sender, "short")) or UnitInParty ( Ambiguate (sender, "short") ) )
   	and tonumber ( select(3, strsplit("\t", msg) ) ) == instanceMapID  then
		local time = select(2,strsplit("\t", msg) )
		time =  tonumber(time)


		if time ~= 0 then
			print("DBM pull timer")
			tickerdbm = time
			pullvalue = true
		end
		if time == 0 then
			tickerdbm = time
			pullvalue = false
		end

	end
end
local function updatetargetInfoFrame()

	currentSpec = GetSpecialization()
	currentSpecId = currentSpec and select(1, GetSpecializationInfo( GetSpecialization())) or 0
	targetexist = 0
	rangetargetexist = 0
	if UnitExists("target") then
		if	LibStub("SpellRange-1.0").IsSpellInRange(Spec.Spell[currentSpecId], "target") == 1  then
			targetexist = 1
		end
	end
	if UnitExists("mouseover") and UnitAffectingCombat("mouseover") and LibStub("SpellRange-1.0").IsSpellInRange(Spec.Id[currentSpecId], "mouseover") == 1 then
			rangetargetexist = 1
	end
	
	--print("Info ", targetexist, " ", rangetargetexist )
	targetInfoFrame.t:SetColorTexture(targetexist,rangetargetexist, 0 ,alphaColor)
end

local function InitializeThree()
	for i = 1, 4 do 
		raidBuff[i] = 1
		raidBufftime[i] =1
	end
		--print ("Initialising raid Health Frames")
	for i = 1, 20 do	
		raidHealthFrame[i] = CreateFrame("frame", "", parent)
		raidHealthFrame[i]:SetSize(size, size)
		raidHealthFrame[i]:SetPoint("TOPLEFT", size*(i-1), -size *21 )   --  row 1-20,  column 19
		raidHealthFrame[i].t = raidHealthFrame[i]:CreateTexture()        
		raidHealthFrame[i].t:SetColorTexture(1, 1, 1, alphaColor)
		raidHealthFrame[i].t:SetAllPoints(raidHealthFrame[i])
		raidHealthFrame[i]:Show()
	end
	for i = 21, 30 do		
		raidHealthFrame[i] = CreateFrame("frame", "", parent)
		raidHealthFrame[i]:SetSize(size, size)
		raidHealthFrame[i]:SetPoint("TOPLEFT", size*(i-20), -size *22 )   --  row 1-10,  column 20
		raidHealthFrame[i].t = raidHealthFrame[i]:CreateTexture()        
		raidHealthFrame[i].t:SetColorTexture(1, 1, 1, alphaColor)
		raidHealthFrame[i].t:SetAllPoints(raidHealthFrame[i])
		raidHealthFrame[i]:Show()
	end
		raidSizeFrame = CreateFrame("frame", "", parent)
		raidSizeFrame:SetSize(size, size)
		raidSizeFrame:SetPoint("TOPLEFT", size*(10), -size *22 )   --  row 11,  column 20
		raidSizeFrame.t = raidSizeFrame:CreateTexture()        
		raidSizeFrame.t:SetColorTexture(1, 1, 1, alphaColor)
		raidSizeFrame.t:SetAllPoints(raidSizeFrame)
		raidSizeFrame:Show()
		raidSizeFrame:SetScript("OnUpdate",NameplateFrameUPDATE)
		
	for i = 1, 4 do		
		RaidBuffFrame[i] = CreateFrame("frame", "", parent)
		RaidBuffFrame[i]:SetSize(size, size)
		RaidBuffFrame[i]:SetPoint("TOPLEFT", size*(10 + i), -size *22 )   --  row 12-15,  column 20
		RaidBuffFrame[i].t = RaidBuffFrame[i]:CreateTexture()        
		RaidBuffFrame[i].t:SetColorTexture(1, 1, 1, alphaColor)
		RaidBuffFrame[i].t:SetAllPoints(RaidBuffFrame[i])
		RaidBuffFrame[i]:Show()
		
	end
		timerDBMFrames = CreateFrame("frame", "", parent)
		timerDBMFrames:SetSize(size, size);
		timerDBMFrames:SetPoint("TOPLEFT", size * 6, -(size * 23))           -- column 6 row 21
		timerDBMFrames.t = timerDBMFrames:CreateTexture()        
		timerDBMFrames.t:SetColorTexture(0, 0, 0, alphaColor)
		timerDBMFrames.t:SetAllPoints(timerDBMFrames)
		timerDBMFrames:Show()	


		totemsFrame = CreateFrame("frame", "", parent)
		totemsFrame:SetSize(size, size);
		totemsFrame:SetPoint("TOPLEFT", size * 7, -(size * 23))           -- column 7 row 21
		totemsFrame.t = totemsFrame:CreateTexture()        
		totemsFrame.t:SetColorTexture(0, 0, 0, alphaColor)
		totemsFrame.t:SetAllPoints(totemsFrame)
		totemsFrame:Show()

		targetInfoFrame = CreateFrame("frame", "", parent)
		targetInfoFrame:SetSize(size, size);
		targetInfoFrame:SetPoint("TOPLEFT", size * 8, -(size * 23))           -- column 8 row 21
		targetInfoFrame.t = targetInfoFrame:CreateTexture()        
		targetInfoFrame.t:SetColorTexture(0, 0, 0, alphaColor)
		targetInfoFrame.t:SetAllPoints(targetInfoFrame)
		targetInfoFrame:Show()
end

is_casting = false
local function HealinEventHandler(self,event, ...)
	if event == "UNIT_HEALTH_FREQUENT" then
	
		if (select(1,...) ~= "player") then
			UpdateRaidIndicators(select(1,...))
		end
	end
    if event == "UNIT_SPELLCAST_SUCCEEDED" then
		if not is_casting then
			for _, spellId in pairs(cooldowns) do
				if spellId == select(5,...) then
                    timeDiff = GetTime() - sendTime
                    timeDiff = timeDiff > select(4, GetNetStats())/ 500  and timeDiff or select(4, GetNetStats())/ 500
				end
            end
        end
    is_casting = false
	end
	if event == "UNIT_SPELLCAST_START" then
		if not is_casting then
			for _, spellId in pairs(cooldowns) do
				if spellId == select(5,...) then
                    timeDiff = GetTime() - sendTime
                    timeDiff = timeDiff > select(4, GetNetStats())/ 500  and timeDiff or select(4, GetNetStats())/ 500
				end
            end
        end
    is_casting = false
	end
	if event == "CURRENT_SPELL_CAST_CHANGED" then
        sendTime = GetTime()
	end
    if event == "UNIT_SPELLCAST_FAILED" then
        is_casting = false
	end
	if event == "RAID_ROSTER_UPDATE" or event == "GROUP_ROSTER_UPDATE" then
	   UdateRaidSizeFrame()
    end
	if event == "CHAT_MSG_ADDON"then 
		--select(1,...) --prefix
		--select(2,...) --msg
		--select(4,...) sender
		DBMPull(select(1,...),select(2,...),select(4,...))
	end

	if event == "PLAYER_TARGET_CHANGED" then
		updatetargetInfoFrame()
	end
if event == "PLAYER_ENTER_COMBAT" or event == "PLAYER_LEAVE_COMBAT" then
		updatetargetInfoFrame()
	end

end

local GlobalTimer = 0
local function onUpDateFunction(self,elapsed)
	
			updateRaidBuff()
			updateDBMFrames(elapsed)
            

			if (classIndex == 6 or                                  -- DeathKnight   
				classIndex == 3 or                                  -- Hunter
				classIndex == 9 or                                  -- Warlock
				classIndex == 8 or                                  -- Mage
				classIndex == 7)                                    -- Shaman (Enh. Needs it for Wolves)
				then
					updateTotemsFrame()
			end
			
end	
frame:SetScript("OnEvent",HealinEventHandler)
frame:SetScript("OnUpdate",onUpDateFunction)