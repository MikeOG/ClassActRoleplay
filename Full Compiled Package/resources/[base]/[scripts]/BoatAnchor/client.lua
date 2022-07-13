--[[ 
	AnchorBoat
    Copyright (C) 2018  Thaisen
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as published
    by the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.
    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
 ]]



Citizen.CreateThread(function()
    while true do
        Citizen.Wait(100)
	local ped = GetPlayerPed( -1 )
	local vehicle = GetVehiclePedIsIn( ped, false )
	local vehicleClass = GetVehicleClass(vehicle)
	local model = GetDisplayNameFromVehicleModel(GetEntityModel(GetVehiclePedIsIn(GetPlayerPed(-1))))

	if not IsEntityDead( ped ) then 
		if IsPedSittingInAnyVehicle( ped )  then 
			if vehicleClass == 14 and (model ~= "SUBMERS" and model ~= "SUBMERS2")  then
				if IsControlPressed(0, 289) then
				FreezeEntityPosition(vehicle ,true)
				end 
			end 
		end 
		end
	end 
end )

--RemoveVehiclesFromGeneratorsInArea(pos['x'] - 207.19, pos['y'] - 6632.21, pos['z'] - 31.66, pos['x'] + 143.36, pos['y'] + 6563.36, pos['z'] + 126.46);

local anchored = false
Citizen.CreateThread(function()
	while true do

		Wait(0)
		local ped = GetPlayerPed(-1)
		if IsControlJustPressed(1, 182) and not IsPedInAnyVehicle(ped)  then
			local boat  = GetVehiclePedIsIn(ped, true)
			if not anchored then
				SetBoatAnchor(boat, true)
			else
				SetBoatAnchor(boat, false)
			end
			anchored = not anchored
		end
	end
end)

Citizen.CreateThread(function()
    while true do
        Citizen.Wait(120)
	local ped = GetPlayerPed( -1 )
	local vehicle = GetVehiclePedIsIn( ped, false )
	local vehicleClass = GetVehicleClass(vehicle)
	
	if not IsEntityDead( ped ) then 
		if IsPedSittingInAnyVehicle( ped )  then 
			if vehicleClass == 14 then 
				if IsControlPressed(0, 289) then
				SetTimeout(4000, function()
				FreezeEntityPosition(vehicle ,false)
				end)
			end
		end 
		end
	end 
	end
end )
