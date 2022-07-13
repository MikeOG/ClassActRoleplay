local blackout = true

-- Switches blackout on or off
Citizen.CreateThread(function()
	while true do
		Citizen.Wait(1)
		if blackout == true then
			SetBlackout(false) --True = blackout false = power on
		else
			SetBlackout(false)
		end
	end
end)

function DisplayHelpText(str)
    SetTextComponentFormat("STRING")
    AddTextComponentString(str)
    DisplayHelpTextFromStringLabel(0, 0, 1, -1)
end