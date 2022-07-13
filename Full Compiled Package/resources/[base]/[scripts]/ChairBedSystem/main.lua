local Beds, CurrentBed, OnBed = {'v_med_bed2', 'v_med_bed1', 'v_med_emptybed','gabz_pillbox_diagnostics_bed_02','gabz_pillbox_diagnostics_bed_03'}, nil, false

Citizen.CreateThread(function()
	while true do
		Citizen.Wait(10)

		if not OnBed then
			local PlayerPed = PlayerPedId()
			local PlayerCoords = GetEntityCoords(PlayerPed)

			for k,v in pairs(Beds) do
				local ClosestBed = GetClosestObjectOfType(PlayerCoords, 1.0, GetHashKey(v), false, false)

				if ClosestBed ~= 0 and ClosestBed ~= nil then
					CurrentBed = ClosestBed
					break
				else
					CurrentBed = nil
				end
			end
		end
	end
end)

Citizen.CreateThread(function()
	while true do
		Citizen.Wait(10)

		if CurrentBed ~= nil then
			if not OnBed then
				local BedCoords = GetEntityCoords(CurrentBed)

				Draw3DText({x = BedCoords.x, y = BedCoords.y, z = (BedCoords.z+1)}, 'Press ~g~[E] ~w~to lie down', 0.35)
			end
		end
	end
end)

Citizen.CreateThread(function()
	while true do
		Citizen.Wait(10)

		if CurrentBed ~= nil then
			if OnBed then
				local BedCoords = GetEntityCoords(CurrentBed)

				Draw3DText({x = BedCoords.x, y = BedCoords.y, z = (BedCoords.z+1.6)}, 'Press ~g~[Z] ~w~to stand up', 0.35)
			end
		end
	end
end)

Citizen.CreateThread(function()
	while true do
		Citizen.Wait(10)

		if CurrentBed ~= nil and IsControlJustReleased(0, 38) then
			local PlayerPed = PlayerPedId()
			local BedCoords, BedHeading = GetEntityCoords(CurrentBed), GetEntityHeading(CurrentBed)

			LoadAnimSet('missfbi1')

			SetEntityCoords(PlayerPed, BedCoords)
			SetEntityHeading(PlayerPed, (BedHeading+180))

			TaskPlayAnim(PlayerPed, 'missfbi1', 'cpr_pumpchest_idle', 8.0, -8.0, -1, 1, 0, false, false, false)

			OnBed = true
--Start Heal Section
			local health = GetEntityHealth(GetPlayerPed(-1))
			local maxhp = 200
			local healfull = maxhp - health
			SetEntityHealth(GetPlayerPed(-1), (health + healfull))
			ClearPedBloodDamage(GetPlayerPed(-1))
--End Heal Section
		elseif IsControlJustReleased(0, 20) and IsEntityPlayingAnim(PlayerPedId(), 'missfbi1', 'cpr_pumpchest_idle', 3) then
			ClearPedTasks(PlayerPedId())

			OnBed = false
		end
	end
end)

function Draw3DText(coords, text, scale)
	local onScreen, x, y = World3dToScreen2d(coords.x, coords.y, coords.z)

	SetTextScale(scale, scale)
	SetTextOutline()
	SetTextDropShadow()
	SetTextDropshadow(2, 0, 0, 0, 255)
	SetTextFont(4)
	SetTextProportional(1)
	SetTextEntry('STRING')
	SetTextCentre(1)
	SetTextColour(255, 255, 255, 215)
	AddTextComponentString(text)
	DrawText(x, y)
end

function LoadAnimSet(AnimDict)
	if not HasAnimDictLoaded(AnimDict) then
		RequestAnimDict(AnimDict)

		while not HasAnimDictLoaded(AnimDict) do
			Citizen.Wait(10)
		end
	end
end
	Citizen.CreateThread(function()
    while OnBed do
        Citizen.Wait(10)
        SetPlayerHealthRechargeLimit(PlayerId(), 2.0)
    end
end)
	Citizen.CreateThread(function()
    while OnBed do
        Citizen.Wait(10)
        SetPlayerHealthRechargeMultiplier(PlayerId(), 0.0)
    end
end)
	Citizen.CreateThread(function()
    while OffBed do
        Citizen.Wait(10)
        SetPlayerHealthRechargeMultiplier(PlayerId(), 0.0)
    end
end)
	Citizen.CreateThread(function()
    while OffBed do
        Citizen.Wait(10)
        SetPlayerHealthRechargeLimit(PlayerId(), 0.0)
    end
end)

