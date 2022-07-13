--SetEntityCoords(PlayerPedId(),1957.7397460938,5172.4497070313,47.9102439880379)


--1  Coords: 892.2587, -960.8538, 38.18458
--2 Coords: -1366.676, -316.9358, 38.28989
--3 Coords: 1957.7397460938,5172.4497070313,47.910243988037
--4 Coords: -341.86242675781,-2444.3217773438,6.000337600708

--FD05J2SX
local invehicle = false
local HudStage = 1
local tokovoipstate = 1
local isTalking = false

RegisterNetEvent('torchizm-hud:toggleTokoVOIP')
AddEventHandler('torchizm-hud:toggleTokoVOIP', function(state)
    tokovoipstate = state
end)

RegisterNetEvent('torchizm-hud:setTalkingState')
AddEventHandler('torchizm-hud:setTalkingState', function(state)
    isTalking = state
end)

local lastValues = {}
local currentValues = {
	["health"] = 100,
	["armor"] = 100,
	["hunger"] = 100,
	["thirst"] = 100,
	["oxy"] = 100,
	["dev"] = false,
	["devdebug"] = false,
	["state"] = 1,
	["talking"] = false
}

RegisterNetEvent('car:windowsdown')
AddEventHandler('car:windowsdown', function()
	local veh = GetVehiclePedIsUsing(PlayerPedId())	
	for i = -1, 6 do
		if GetPedInVehicleSeat(veh, i) == PlayerPedId() then
			RollDownWindow(veh, i+1)
		end
	end	
end)

currentValues["hunger"] = 100
currentValues["thirst"] = 100

hunger = "Full"
thirst = "Sustained"
local cruise = {enabled = false, speed = 0, airTime = 0}

function getVehicleInDirection(coordFrom, coordTo)
	local offset = 0
	local rayHandle
	local vehicle

	for i = 0, 100 do
		rayHandle = CastRayPointToPoint(coordFrom.x, coordFrom.y, coordFrom.z, coordTo.x, coordTo.y, coordTo.z + offset, 10, PlayerPedId(), 0)	
		a, b, c, d, vehicle = GetRaycastResult(rayHandle)
		
		offset = offset - 1

		if vehicle ~= 0 then break end
	end
	
	local distance = Vdist2(coordFrom, GetEntityCoords(vehicle))
	
	if distance > 3000 then vehicle = nil end

    return vehicle ~= nil and vehicle or 0
end

RegisterNetEvent("disableHUD")
AddEventHandler("disableHUD", function(passedinfo)
	HudStage = passedinfo
end)

speedoON = false
RegisterNetEvent('stopSpeedo')
AddEventHandler('stopSpeedo', function()
	if speedoON then
		speedoON = false

		return
	end
end)

oxyOn = false
attachedProp = 0
attachedProp2 = 0

function removeAttachedProp2()
	DeleteEntity(attachedProp2)
	attachedProp2 = 0
end

function removeAttachedProp()
	DeleteEntity(attachedProp)
	attachedProp = 0
end

function attachProp2(attachModelSent,boneNumberSent,x,y,z,xR,yR,zR)
	removeAttachedProp2()
	attachModel = GetHashKey(attachModelSent)
	boneNumber = boneNumberSent
	local bone = GetPedBoneIndex(PlayerPedId(), boneNumberSent)
	--local x,y,z = table.unpack(GetEntityCoords(PlayerPedId(), true))
	RequestModel(attachModel)
	while not HasModelLoaded(attachModel) do
		Citizen.Wait(100)
	end
	attachedProp2 = CreateObject(attachModel, 1.0, 1.0, 1.0, 1, 1, 0)
	exports["isPed"]:GlobalObject(attachedProp2)
	AttachEntityToEntity(attachedProp2, PlayerPedId(), bone, x, y, z, xR, yR, zR, 1, 1, 0, 0, 2, 1)
	SetModelAsNoLongerNeeded(attachModel)
end

function attachProp(attachModelSent,boneNumberSent,x,y,z,xR,yR,zR)
	removeAttachedProp()
	attachModel = GetHashKey(attachModelSent)
	boneNumber = boneNumberSent 
	local bone = GetPedBoneIndex(PlayerPedId(), boneNumberSent)
	--local x,y,z = table.unpack(GetEntityCoords(PlayerPedId(), true))
	RequestModel(attachModel)
	while not HasModelLoaded(attachModel) do
		Citizen.Wait(100)
	end
	attachedProp = CreateObject(attachModel, 1.0, 1.0, 1.0, 1, 1, 0)
	exports["isPed"]:GlobalObject(attachedProp)
	AttachEntityToEntity(attachedProp, PlayerPedId(), bone, x, y, z, xR, yR, zR, 1, 1, 0, 0, 2, 1)
	SetModelAsNoLongerNeeded(attachModel)
end

dstamina = 0

sitting = false

imdead = 0

RegisterNetEvent('hadtreat')
AddEventHandler('hadtreat', function(arg1,arg2,arg3)
	local model = GetEntityModel(PlayerPedId())
    if model ~= GetHashKey("a_c_chop") then return end

    dstamina = 400
    SetRunSprintMultiplierForPlayer(PlayerId(), 1.25)

    while dstamina > 0 do

        Citizen.Wait(1000)
        RestorePlayerStamina(PlayerId(), 1.0)
        dstamina = dstamina - 1

    end

    dstamina = 0

    if IsPedRunning(PlayerPedId()) then
        SetPedToRagdoll(PlayerPedId(),1000,1000, 3, 0, 0, 0)
    end

    SetRunSprintMultiplierForPlayer(PlayerId(), 1.0)
    RevertToStressMultiplier()
end)

function camOn()
	if(not DoesCamExist(cam)) then
		cam = CreateCam('DEFAULT_SCRIPTED_CAMERA', true)
	
		SetCamActive(cam,  true)

		RenderScriptCams(true,  false,  0,  true,  true)		
	end	
end

function camOff()
	RenderScriptCams(false, false, 0, 1, 0)
	DestroyCam(cam, false)
end

opacity = 0
fadein = false

local opacityBars = 0 
local Addition = 0.0


local GodEnabled = false
RegisterNetEvent("carandplayerhud:godCheck")
AddEventHandler("carandplayerhud:godCheck",function(arg)
	GodEnabled = arg
end)

local voip = {}
voip['default'] = {name = 'default', setting = 18.0}
voip['local'] = {name = 'local', setting = 18.0}
voip['whisper'] = {name = 'whisper', setting = 4.0}
voip['yell'] = {name = 'yell', setting = 35.0}


RegisterNetEvent('voip:settizng')
AddEventHandler('voip:settizng', function(voipDistance)
	if (voipDistance == 1) then
		distanceSetting = 18.0
	elseif (voipDistance == 2) then
		distanceSetting = 4.0
	elseif (voipDistance == 3) then
		distanceSetting = 35.0
	end		
end)

AddEventHandler('np-base:playerSessionStarted', function()
	NetworkSetTalkerProximity(voip['default'].setting)
end)

RegisterNetEvent('pv:voip')
AddEventHandler('pv:voip', function(voipDistance)

	NotificationMessage("Your VOIP is now ~b~" .. voipsetting ..".")
	NetworkSetTalkerProximity(distanceSetting)
		
end)

distanceSetting = 18.0
NetworkSetTalkerProximity(18.0)

voipsetting = "Normal"
RegisterNetEvent('pv:voip1')
AddEventHandler('pv:voip1', function(voipDistance)
	distanceSetting = 4.0
	voipsetting = "Whisper"
end)

RegisterNetEvent('pv:voip2')
AddEventHandler('pv:voip2', function(voipDistance)
	distanceSetting = 18.0
	voipsetting = "Normal"
end)

RegisterNetEvent('pv:voip3')
AddEventHandler('pv:voip3', function(voipDistance)
	distanceSetting = 35.0
	voipsetting = "Yell"
end)

local colorblind = false
RegisterNetEvent('option:colorblind')
AddEventHandler('option:colorblind',function()
    colorblind = not colorblind
end)

local voipTypes = {
	
	{type = "Whisper", event = "pv:voip1"},
	{type = "Normal", event = "pv:voip2"},
	{type = "Yell", event = "pv:voip3"}
}

function lerp(min, max, amt)
	return (1 - amt) * min + amt * max
end
function rangePercent(min, max, amt)
	return (((amt - min) * 100) / (max - min)) / 100
end

RegisterNetEvent("gen-admin:currentDevmode")
AddEventHandler("gen-admin:currentDevmode", function(devmode)
    currentValues["dev"] = devmode
end)

RegisterNetEvent("gen-admin:currentDebug")
AddEventHandler("gen-admin:currentDebug", function(debugToggle)
    currentValues["devdebug"] = debugToggle
end)


Citizen.CreateThread(function()
    while true do
        
        TriggerEvent('esx_status:getStatus', 'hunger', function(hunger)
            TriggerEvent('esx_status:getStatus', 'thirst', function(thirst)

                local myhunger = hunger.getPercent()
                local mythirst = thirst.getPercent()

                SendNUIMessage({
                    action = "updateStatusHud",
					varSetHunger = myhunger,
					varSetThirst = mythirst,
                })
            end)
        end)
        Citizen.Wait(5000)
    end
end)

local stresslevel = 0

Citizen.CreateThread(function()
    while true do
        Citizen.Wait(60000)
        if DoesEntityExist(GetPlayerPed(-1)) and stresslevel <= 100 then
            stresslevel = stresslevel + 0.1
        end
    end
end)

Citizen.CreateThread(function()
	while true do
		if stresslevel >= 25 then
			ShakeGameplayCam('SMALL_EXPLOSION_SHAKE', 0.02)
		elseif stresslevel >= 50 then
			ShakeGameplayCam('SMALL_EXPLOSION_SHAKE', 0.07)
		elseif stresslevel >= 75 then
			ShakeGameplayCam('MEDIUM_EXPLOSION_SHAKE', 0.07)
		end
		Citizen.Wait(2000)
	end
end)

RegisterNetEvent('client:useCig')
AddEventHandler('client:useCig', function()
	TriggerEvent('DoLongHudText', "You feel a short buzz", 1)
	if IsPedInAnyVehicle(PlayerPedId(), false) then
		Citizen.Wait(5)
	else
		TaskStartScenarioInPlace(PlayerPedId(), "WORLD_HUMAN_SMOKING", -1, true)
	end
	exports["gen-taskbar"]:taskBar(15000, "Smoking Cigarette")
		if stresslevel <= 0.001 then
			Citizen.Wait(5)
		else
			stresslevel = stresslevel - 2.0
		end
		TriggerEvent('DoLongHudText', "Stress slightly relieved", 1)
		if IsPedInAnyVehicle(PlayerPedId(), false) then
			Citizen.Wait(5)
		else
			ClearPedTasksImmediately(PlayerPedId())
		end
end)

RegisterNetEvent("client:updateStress")
AddEventHandler("client:updateStress",function()
	stresslevel = 0
end)

RegisterNetEvent('client:newStress')
AddEventHandler('client:newStress', function(toggle)
	if toggle == true then
		stresslevel = stresslevel + 0.1
		TriggerEvent('DoLongHudText', "Stress increased")
	else
		stresslevel = stresslevel - 0.1
		TriggerEvent('DoLongHudText', "Stress reduced")
	end
end)

currentValues["hunger"] = 0

currentValues["thirst"] = 0

Citizen.CreateThread(function()
	
	while true do
		Citizen.Wait(5000)
		
		TriggerEvent('esx_status:getStatus', 'hunger', function(status)
			currentValues["hunger"]  = status.val/1000000*100
		end)
		TriggerEvent('esx_status:getStatus', 'thirst', function(status)
			currentValues["thirst"] = status.val/1000000*100
		end)
	end
end)

oxyOn = false
attachedProp = 0
attachedProp2 = 0

function removeAttachedProp2()
	DeleteEntity(attachedProp2)
	attachedProp2 = 0
end

function removeAttachedProp()
	DeleteEntity(attachedProp)
	attachedProp = 0
end

function attachProp2(attachModelSent,boneNumberSent,x,y,z,xR,yR,zR)
	removeAttachedProp2()
	attachModel = GetHashKey(attachModelSent)
	boneNumber = boneNumberSent
	local bone = GetPedBoneIndex(PlayerPedId(), boneNumberSent)
	--local x,y,z = table.unpack(GetEntityCoords(PlayerPedId(), true))
	RequestModel(attachModel)
	while not HasModelLoaded(attachModel) do
		Citizen.Wait(100)
	end
	attachedProp2 = CreateObject(attachModel, 1.0, 1.0, 1.0, 1, 1, 0)
	exports["isPed"]:GlobalObject(attachedProp2)
	AttachEntityToEntity(attachedProp2, PlayerPedId(), bone, x, y, z, xR, yR, zR, 1, 1, 0, 0, 2, 1)
	SetModelAsNoLongerNeeded(attachModel)
end

function attachProp(attachModelSent,boneNumberSent,x,y,z,xR,yR,zR)
	removeAttachedProp()
	attachModel = GetHashKey(attachModelSent)
	boneNumber = boneNumberSent 
	local bone = GetPedBoneIndex(PlayerPedId(), boneNumberSent)
	--local x,y,z = table.unpack(GetEntityCoords(PlayerPedId(), true))
	RequestModel(attachModel)
	while not HasModelLoaded(attachModel) do
		Citizen.Wait(100)
	end
	attachedProp = CreateObject(attachModel, 1.0, 1.0, 1.0, 1, 1, 0)
	exports["isPed"]:GlobalObject(attachedProp)
	AttachEntityToEntity(attachedProp, PlayerPedId(), bone, x, y, z, xR, yR, zR, 1, 1, 0, 0, 2, 1)
	SetModelAsNoLongerNeeded(attachModel)
end

currentValues["oxy"] = 25.0

RegisterNetEvent("OxyMenu")
AddEventHandler("OxyMenu",function()
	if currentValues["oxy"] > 25.0 then
		--RemoveOxyTank
		TriggerEvent('sendToGui','Remove Oxy Tank','RemoveOxyTank')
	end
end)

RegisterNetEvent("RemoveOxyTank")
AddEventHandler("RemoveOxyTank",function()
	if currentValues["oxy"] > 25.0 then
		currentValues["oxy"] = 25.0
		TriggerEvent('menu:hasOxygenTank', false)
	end
end)

RegisterNetEvent("UseOxygenTank")
AddEventHandler("UseOxygenTank",function()
	currentValues["oxy"] = 100.0
	TriggerEvent('menu:hasOxygenTank', true)
end)

local lastDamageTrigger = 0

RegisterNetEvent("fire:damageUser")
AddEventHandler("fire:damageUser", function(Reqeuester)
	local attacker = GetPlayerFromServerId(Reqeuester)
	local Attackerped = GetPlayerPed(attacker)

	if IsPedShooting(Attackerped) then
		local name = GetSelectedPedWeapon(Attackerped)
        if name == `WEAPON_FIREEXTINGUISHER` and not exports["isPed"]:isPed("dead") then
        	lastDamageTrigger = GetGameTimer()
        	currentValues["oxy"] = currentValues["oxy"] - 15
        end
	end
end)

Citizen.CreateThread(function()

	while true do
		Wait(1)
		if currentValues["oxy"] > 0 and IsPedSwimmingUnderWater(PlayerPedId()) then
			SetPedDiesInWater(PlayerPedId(), false)
			if currentValues["oxy"] > 25.0 then
				currentValues["oxy"] = currentValues["oxy"] - 0.003125
			else
				currentValues["oxy"] = currentValues["oxy"] - 1
			end
		else
			if IsPedSwimmingUnderWater(PlayerPedId()) then
				currentValues["oxy"] = currentValues["oxy"] - 0.01
				SetPedDiesInWater(PlayerPedId(), true)
			end
		end

		if not IsPedSwimmingUnderWater( PlayerPedId() ) and currentValues["oxy"] < 25.0 then
			if GetGameTimer() - lastDamageTrigger > 3000 then
				currentValues["oxy"] = currentValues["oxy"] + 1
				if currentValues["oxy"] > 25.0 then
					currentValues["oxy"] = 25.0
				end
			else
				if currentValues["oxy"] <= 0 then
					
					if exports["isPed"]:isPed("dead") then
						lastDamageTrigger = -7000
						currentValues["oxy"] = 25.0
					else
						SetEntityHealth(PlayerPedId(), GetEntityHealth(PlayerPedId()) - 20)
					end
				end
			end
		end

		if currentValues["oxy"] > 25.0 and not oxyOn then
			oxyOn = true
			attachProp("p_s_scuba_tank_s", 24818, -0.25, -0.25, 0.0, 180.0, 90.0, 0.0)
			attachProp2("p_s_scuba_mask_s", 12844, 0.0, 0.0, 0.0, 180.0, 90.0, 0.0)
		elseif oxyOn and currentValues["oxy"] <= 25.0 then
			oxyOn = false
			removeAttachedProp()
			removeAttachedProp2()
		end
		if not oxyOn then
			Wait(1000)
		end
	end
end)

-- this should just use nui instead of drawrect - it literally ass fucks usage.
Citizen.CreateThread(function()
	local minimap = RequestScaleformMovie("minimap")
    SetRadarBigmapEnabled(true, false)
    Wait(0)
	SetRadarBigmapEnabled(false, false)
	
	local counter = 0
	local get_ped = PlayerPedId() -- current ped
	local get_ped_veh = GetVehiclePedIsIn(get_ped,false) -- Current Vehicle ped is in
	local plate_veh = GetVehicleNumberPlateText(get_ped_veh) -- Vehicle Plate
	local veh_stop = IsVehicleStopped(get_ped_veh) -- Parked or not
	local veh_engine_health = GetVehicleEngineHealth(get_ped_veh) -- Vehicle Engine Damage 
	local veh_body_health = GetVehicleBodyHealth(get_ped_veh)
	local veh_burnout = IsVehicleInBurnout(get_ped_veh) -- Vehicle Burnout
	local thespeed = GetEntitySpeed(get_ped_veh) * 3.6
	currentValues["health"] = (GetEntityHealth(get_ped) - 100)
	currentValues["voice"] = 0
	currentValues["armor"] = GetPedArmour(get_ped)
	currentValues["parachute"] = HasPedGotWeapon(get_ped, `gadget_parachute`, false)
	currentValues["talking"] = isTalking
	currentValues["state"] = tokovoipstate
	if currentValues["oxy"] <= 0 then currentValues["oxy"] = 0 end
	while true do

		if sleeping then
			if IsControlJustReleased(0,38) then
				sleeping = false
				DetachEntity(PlayerPedId(), 1, true)
			end
		end

		Citizen.Wait(1)
		
		if GetEntityMaxHealth(GetPlayerPed(-1)) ~= 200 then
			SetEntityMaxHealth(GetPlayerPed(-1), 200)
			SetEntityHealth(GetPlayerPed(-1), 200)
		end

		if counter == 0 then
			
			 -- current ped
			get_ped = PlayerPedId()
			SetPedSuffersCriticalHits(get_ped,false)
			get_ped_veh = GetVehiclePedIsIn(get_ped,false) -- Current Vehicle ped is in
			plate_veh = GetVehicleNumberPlateText(get_ped_veh) -- Vehicle Plate
			veh_stop = IsVehicleStopped(get_ped_veh) -- Parked or not
			veh_engine_health = GetVehicleEngineHealth(get_ped_veh) -- Vehicle Engine Damage 
			veh_body_health = GetVehicleBodyHealth(get_ped_veh)
			veh_burnout = IsVehicleInBurnout(get_ped_veh) -- Vehicle Burnout
			thespeed = GetEntitySpeed(get_ped_veh) * 3.6
			currentValues["health"] = GetEntityHealth(get_ped) - 100
			currentValues["armor"] = GetPedArmour(get_ped)
			currentValues["parachute"] = HasPedGotWeapon(get_ped, `gadget_parachute`, false)
			currentValues["talking"] = isTalking
			currentValues["state"] = tokovoipstate

			if stresslevel > 100 then stresslevel = 100 end

			if distanceSetting == 4.0 then
				-- currentValues["voice"] = 0.027 * 0.1
				currentValues["voice"] = 1
			elseif distanceSetting == 18.0 then
				-- currentValues["voice"] = 0.027 * 0.5
				currentValues["voice"] = 2
			elseif distanceSetting == 35.0 then
				-- currentValues["voice"] = 0.027
				currentValues["voice"] = 3
			end

			if currentValues["hunger"] < 0 then
				currentValues["hunger"] = 0
			end
			if currentValues["thirst"] < 0 then
				currentValues["thirst"] = 0
			end

			if currentValues["hunger"] > 100 then currentValues["hunger"] = 100 end

			if currentValues["health"] < 1 then currentValues["health"] = 100 end
			if currentValues["thirst"] > 100 then currentValues["thirst"] = 100 end
			if currentValues["oxy"] <= 0 then currentValues["oxy"] = 0 end
			local valueChanged = false

			for k,v in pairs(currentValues) do
				if lastValues[k] == nil or lastValues[k] ~= v then
					valueChanged = true
					lastValues[k] = v
				end
			end

			if valueChanged then
				SendNUIMessage({
					type = "updateStatusHud",
					hasParachute = currentValues["parachute"],
					varSetHealth = currentValues["health"],
					varSetArmor = currentValues["armor"],
					varSetHunger = currentValues["hunger"],
					varSetThirst = currentValues["thirst"],
					varSetOxy = currentValues["oxy"],
					varSetStress = stresslevel,
					colorblind = colorblind,
					varSetVoice = currentValues["voice"],
					varDev = currentValues["dev"],
					varDevDebug = currentValues["devdebug"],
					talking = isTalking,
					state = tokovoipstate
				})
			end

			counter = 25

		end

		counter = counter - 1

        if get_ped_veh ~= 0 then
            local model = GetEntityModel(get_ped_veh)
            local roll = GetEntityRoll(get_ped_veh)
  
            if not IsThisModelABoat(model) and not IsThisModelAHeli(model) and not IsThisModelAPlane(model) and IsEntityInAir(get_ped_veh) or (roll < -50 or roll > 50) then
                DisableControlAction(0, 59) -- leaning left/right
                DisableControlAction(0, 60) -- leaning up/down
            end

            if GetPedInVehicleSeat(GetVehiclePedIsIn(get_ped, false), 0) == get_ped then
				if GetIsTaskActive(get_ped, 165) then
					SetPedIntoVehicle(get_ped, GetVehiclePedIsIn(get_ped, false), 0)
				end
			end

			DisplayRadar(1)
        	SetRadarZoom(1000)
        else
        	DisplayRadar(0)
        end

		BeginScaleformMovieMethod(minimap, "SETUP_HEALTH_ARMOUR")
        ScaleformMovieMethodAddParamInt(3)
        EndScaleformMovieMethod()
	end
end)

Citizen.CreateThread(function()
	while true do
		Citizen.Wait(500)
		ped = PlayerPedId()
		if not IsPedInAnyVehicle(plyr, false) then 
			if IsPedUsingActionMode(ped) then
				SetPedUsingActionMode(ped, -1, -1, 1)
			end
		else
			Citizen.Wait(3000)
		end
    end
end)

Citizen.CreateThread( function()

	local resetcounter = 0
	local jumpDisabled = false
  	
  	while true do 
    Citizen.Wait(100)

  --  if IsRecording() then
  --      StopRecordingAndDiscardClip()
  --  end     

		if jumpDisabled and resetcounter > 0 and IsPedJumping(PlayerPedId()) then
			
			SetPedToRagdoll(PlayerPedId(), 1000, 1000, 3, 0, 0, 0)

			resetcounter = 0
		end

		if not jumpDisabled and IsPedJumping(PlayerPedId()) then

			jumpDisabled = true
			resetcounter = 10
			Citizen.Wait(1200)
		end

		if resetcounter > 0 then
			resetcounter = resetcounter - 1
		else
			if jumpDisabled then
				resetcounter = 0
				jumpDisabled = false
			end
		end
	end
end)

Citizen.CreateThread( function()
	
	while true do 

		 if IsPedArmed(PlayerPedId(), 6) then
		 	Citizen.Wait(1)
		 else
		 	Citizen.Wait(1500)
		 end  

	    if IsPedShooting(PlayerPedId()) then
	    	local ply = PlayerPedId()
	    	local GamePlayCam = GetFollowPedCamViewMode()
	    	local Vehicled = IsPedInAnyVehicle(ply, false)
	    	local MovementSpeed = math.ceil(GetEntitySpeed(ply))

	    	if MovementSpeed > 69 then
	    		MovementSpeed = 69
	    	end

	        local _,wep = GetCurrentPedWeapon(ply)

	        local group = GetWeapontypeGroup(wep)

	        local p = GetGameplayCamRelativePitch()

	        local cameraDistance = #(GetGameplayCamCoord() - GetEntityCoords(ply))

	        local recoil = math.random(130,140+(math.ceil(MovementSpeed*1.5)))/100
	        local rifle = false

          	if group == 970310034 or group == 1159398588 then
          		rifle = true
          	end

          	if cameraDistance < 5.3 then
          		cameraDistance = 1.5
          	else
          		if cameraDistance < 8.0 then
          			cameraDistance = 4.0
          		else
          			cameraDistance = 7.0
          		end
          	end

	        if Vehicled then
	        	recoil = recoil + (recoil * cameraDistance)
	        else
	        	recoil = recoil * 0.3
	        end

	        if GamePlayCam == 4 then

	        	recoil = recoil * 0.7
		        if rifle then
		        	recoil = recoil * 0.1
		        end
	        end

	        if rifle then
	        	recoil = recoil * 0.1
	        end

	        local rightleft = math.random(4)
	        local h = GetGameplayCamRelativeHeading()
	        local hf = math.random(10,40+MovementSpeed)/100

	        if Vehicled then
	        	hf = hf * 2.0
	        end

	        if rightleft == 1 then
	        	SetGameplayCamRelativeHeading(h+hf)
	        elseif rightleft == 2 then
	        	SetGameplayCamRelativeHeading(h-hf)
	        end 
        
	        local set = p+recoil

	       	SetGameplayCamRelativePitch(set,0.8)    	       		       	
	      -- 	print(GetGameplayCamRelativePitch())
	    end
	end
end)

--[[RegisterNetEvent('gen-armour:SetPlayerArmour')
AddEventHandler('gen-armour:SetPlayerArmour', function(armour)
    Citizen.Wait(6000) 
    SetPedArmour(PlayerPedId(), tonumber(armour))
end)

Citizen.CreateThread(function()
    while true do
        Citizen.Wait(0)
        TriggerServerEvent('gen-armour:RefreshCurrentArmour', GetPedArmour(PlayerPedId()))
        Citizen.Wait(1000)
    end
end)]]
