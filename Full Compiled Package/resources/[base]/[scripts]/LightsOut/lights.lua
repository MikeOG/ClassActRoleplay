local fireObjects = {
	"prop_beach_fire",
	-- "prop_hobo_stove_01", -- Doesn't work
	"prop_ld_rubble_01",
}

local inRange = false

closeObjects = {}
closeVehicles = {}

currentCount = 0
count = 0

RegisterNetEvent("SetLights")
AddEventHandler("SetLights", function(daylight)
	day = daylight
end)

local entityEnumerator = {
  __gc = function(enum)

    if enum.destructor and enum.handle then
	  enum.destructor(enum.handle)
	  
    end
    enum.destructor = nil
    enum.handle = nil
  end
}

local function EnumerateEntities(initFunc, moveFunc, disposeFunc)
  return coroutine.wrap(function()
    local iter, id = initFunc()
    if not id or id == 0 then
      disposeFunc(iter)
      return
    end
    
    local enum = {handle = iter, destructor = disposeFunc}
    setmetatable(enum, entityEnumerator)
    
    local next = true

    repeat
      coroutine.yield(id)
      next, id = moveFunc(iter)
    until not next

    enum.destructor, enum.handle = nil, nil
    disposeFunc(iter)
  end)
end

function EnumerateObjects()
	return EnumerateEntities(FindFirstObject, FindNextObject, EndFindObject)
end

function EnumerateVehicles()
	return EnumerateEntities(FindFirstVehicle, FindNextVehicle, EndFindVehicle)
end

-- Fire area lights
Citizen.CreateThread(function()
	while true do
		playerX, playerY, playerZ = table.unpack(GetEntityCoords(PlayerPedId(), true))
		for object in EnumerateObjects() do
			playerX, playerY, playerZ = table.unpack(GetEntityCoords(GetPlayerPed(-1), true))
			objectX, objectY, objectZ = table.unpack(GetEntityCoords(object, true))

			if day == false then
				for i, fireObject in ipairs(fireObjects) do
					if GetEntityModel(object) == GetHashKey(fireObject) then
						if(Vdist(playerX, playerY, playerZ, objectX, objectY, objectZ) <= 100.0)then
							table.insert(closeObjects, object)
						end

					end
				end
			end
		end

		if day == false then
			for i, object in pairs(closeObjects) do
				playerX, playerY, playerZ = table.unpack(GetEntityCoords(GetPlayerPed(-1), true))
				objectX, objectY, objectZ = table.unpack(GetEntityCoords(object, true))

				if DoesEntityExist(object) == false then
					table.remove(closeObjects, i)
				end

				if objectX < playerX - 100 or objectX > playerX + 100 or objectY < playerY - 100 or objectY > playerY + 100 then
					table.remove(closeObjects, i)
				end

			end
		end

		Citizen.Wait(500)
		count = #closeObjects
		for i=0, count do
			closeObjects[i] = nil
		end

	end
end)

Citizen.CreateThread(function()
	while true do
		Citizen.Wait(0)
		if day == false then
			for i, object in ipairs(closeObjects) do
			    playerX, playerY, playerZ = table.unpack(GetEntityCoords(PlayerPedId(), true))
				objectX, objectY, objectZ = table.unpack(GetEntityCoords(object, true))

				if(Vdist(playerX, playerY, playerZ, objectX, objectY, objectZ) <= 250.0)then
					DrawLightWithRangeAndShadow(objectX, objectY, objectZ, 255, 209, 100, 10.0, 0.1, 10.0)
				end

			end
		end
	end
end)



-- Vehicle headlights
Citizen.CreateThread(function()
	while true do
		playerX, playerY, playerZ = table.unpack(GetEntityCoords(PlayerPedId(), true))

		for vehicle in EnumerateVehicles() do
			playerX, playerY, playerZ = table.unpack(GetEntityCoords(GetPlayerPed(-1), true))
			vehX, vehY, vehZ = table.unpack(GetEntityCoords(vehicle, true))

			if(Vdist(playerX, playerY, playerZ, vehX, vehY, vehZ) <= 250.0)then
				table.insert(closeVehicles, vehicle)
			end
		end

		for i, vehicle in pairs(closeVehicles) do

			playerX, playerY, playerZ = table.unpack(GetEntityCoords(GetPlayerPed(-1), true))
			vehX, vehY, vehZ = table.unpack(GetEntityCoords(vehicle, true))

			if DoesEntityExist(vehicle) == false then
				table.remove(closeVehicles, i)
			end

			if vehX < playerX - 250 or vehX > playerX + 250 or vehY < playerY - 250 or vehY > playerY + 250 then
				table.remove(closeVehicles, i)
			end
		end

		Citizen.Wait(500)
		count = #closeVehicles
		for i=0, count do
			closeVehicles[i] = nil
		end

	end
end)

Citizen.CreateThread(function()
	while true do
		Citizen.Wait(1)

		for i, vehicle in ipairs(closeVehicles) do
			carX, carY, carZ = table.unpack(GetEntityCoords(vehicle, false))
			dirX, dirY, dirZ = table.unpack(GetEntityForwardVector(vehicle))	

			if GetIsVehicleEngineRunning(vehicle) then
				-- FreezeEntityPosition(vehicle, false)
				DrawSpotLight(carX, carY, carZ, dirX, dirY, dirZ, 255, 255, 255, 50.0, 0.1, 0.9, 25.0, 0.95)
			else
				-- FreezeEntityPosition(vehicle, true)
			end

		end
	end
end)

Citizen.CreateThread(function()
	while true do
		playerX, playerY, playerZ = table.unpack(GetEntityCoords(PlayerPedId(), true))

		for vehicle in EnumerateVehicles() do
			playerX, playerY, playerZ = table.unpack(GetEntityCoords(GetPlayerPed(-1), true))
			vehX, vehY, vehZ = table.unpack(GetEntityCoords(vehicle, true))

			if(Vdist(playerX, playerY, playerZ, vehX, vehY, vehZ) <= 250.0)then
				table.insert(closeVehicles, vehicle)
			end
		end

		for i, vehicle in pairs(closeVehicles) do

			playerX, playerY, playerZ = table.unpack(GetEntityCoords(GetPlayerPed(-1), true))
			vehX, vehY, vehZ = table.unpack(GetEntityCoords(vehicle, true))

			if DoesEntityExist(vehicle) == false then
				table.remove(closeVehicles, i)
			end

			if vehX < playerX - 250 or vehX > playerX + 250 or vehY < playerY - 250 or vehY > playerY + 250 then
				table.remove(closeVehicles, i)
			end
		end

		Citizen.Wait(500)
		count = #closeVehicles
		for i=0, count do
			closeVehicles[i] = nil
		end

	end
end)

Citizen.CreateThread(function()
	while true do
		Citizen.Wait(1)

		for i, vehicle in ipairs(closeVehicles) do
			carX, carY, carZ = table.unpack(GetEntityCoords(vehicle, false))
			dirX, dirY, dirZ = table.unpack(GetEntityForwardVector(vehicle))	

			if GetIsVehicleEngineRunning(vehicle) then
				-- FreezeEntityPosition(vehicle, false)
				DrawSpotLight(carX, carY, carZ, dirX, dirY, dirZ, 255, 255, 255, 50.0, 0.1, 0.9, 25.0, 0.95)
			else
				-- FreezeEntityPosition(vehicle, true)
			end

		end
	end
end)



-- Area lights
Citizen.CreateThread(function()
	while true do

		Citizen.Wait(1)
		x, y, z = table.unpack(GetEntityCoords(PlayerPedId(), false))
		dirX, dirY, dirZ = table.unpack(GetEntityForwardVector(PlayerPedId()))	

		for k,v in pairs(lightAreas) do
			DrawLightWithRangeAndShadow(v.x, v.y, v.z, 255, 255, 255, v.radius, 0.025, 10.0)
		end

	end
end)