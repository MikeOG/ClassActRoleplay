local function stringsplit2(str, pat)
    local stringTable = {}
    local strings = "(.-)" .. pat
    local addAmount = 1
    local stringTrue, e, stringFound = str:find(strings, 1)
    while stringTrue do
        if stringTrue ~= 1 or stringFound ~= "" then
            table.insert(stringTable, stringFound)
        end
        addAmount = e + 1
        stringTrue, e, stringFound = str:find(strings, addAmount)
    end
    if addAmount <= #str then
        stringFound = str:sub(addAmount)
        table.insert(stringTable, stringFound)
    end
    return stringTable
end

local function closeUI()
	SetNuiFocus(false)
	
	SendNUIMessage({
		type = "close"
	})
	
	TransitionFromBlurred(1000.0)
end

local function showLoginScreen()
	SendNUIMessage({
		type = "showlogin",
		name = GetPlayerName(PlayerId())
	})
	
	SetNuiFocus(true, true)
	
	TransitionToBlurred(1000.0)
end

local function showCharacterSelect(charData)
	print("[LOGIN] Sending char data to nui")
	SendNUIMessage({
		type = "showCharacterSelect",
		chars = json.encode(charData)
	})

	SetNuiFocus(true, true)
end

local function doFade(fadeOutOnly)
	local isScreenFading = IsScreenFadingOut()
	local isScreenFaded = IsScreenFadedOut()
	
	if isScreenFading or isScreenFaded then
		if isScreenFading then
			while IsScreenFadingOut() do
				Citizen.Wait(0)
			end
		end

		DoScreenFadeIn(1500)
	elseif (not isScreenFading or not isScreenFaded) and not fadeOutOnly then
		DoScreenFadeOut(1200)
	end
end

AddEventHandler('onClientMapStart', function()
  exports.spawnmanager:setAutoSpawn(true)
  exports.spawnmanager:forceRespawn()
end)


RegisterNetEvent("Session.Loaded")
AddEventHandler("Session.Loaded", function()
	showLoginScreen()
end)

RegisterNetEvent("Login.RecieveCharacters")
AddEventHandler("Login.RecieveCharacters", function(chars)
	print("[LOGIN] Recieved characters")
	showCharacterSelect(chars)
	--doFade(true)
end)

RegisterNUICallback('Login.LoadCharacters', function(data, cb)
	print("[LOGIN] Recieved request to load request sending request to server")
	TriggerServerEvent("Login.RequestCharacters")
end)

RegisterNUICallback('Login.SelectChar', function(data, cb)
	--data.charId
	closeUI()
	--doFade()
	TriggerServerEvent("Login.LoadCharacter", data.charId)
end)

RegisterNUICallback('Login.DeleteChar', function(data, cb)
	--data.charId
	TriggerServerEvent("Login.DeleteCharacter", data.charId)
end)

RegisterNUICallback('Login.CreateCharacter', function(data, cb)
	--data.firstName data.lastName data.DOB
	if data.firstName == nil or data.firstName == "" then
		TriggerEvent("UI.Toast", "Please enter a valid first name")
		return
	end
		
	if data.lastName == nil or data.lastName == "" then
		TriggerEvent("UI.Toast", "Please enter a valid last name")
		return
	end

	if data.DOB == nil or data.DOB == "" then
		TriggerEvent("UI.Toast", "Please enter a valid date of birth")
		return
	end
		
	if string.find(data.firstName, " ") or string.find(data.firstName, "  ") or string.find(data.firstName, "  ") then
		TriggerEvent("UI.Toast", "Please enter a valid first name (no white spaces)")
		return
	end
		
	if string.find(data.lastName, " ") or string.find(data.lastName, "  ") or string.find(data.lastName, "  ") then
		TriggerEvent("UI.Toast", "Please enter a valid last name (no white spaces)")
		return
	end

	if string.find(data.DOB, " ") or string.find(data.DOB, "  ") or string.find(data.DOB, "  ") then
		TriggerEvent("UI.Toast", "Please enter a valid date of birth (no white spaces)")
		return
	end

	local dobData = stringsplit2(data.DOB, "/")
	if #dobData ~= 3 then
		TriggerEvent("UI.Toast", "Please enter a valid date of birth (use Day/Month/Year as the format)")
		return
	end

	for k,v in pairs(dobData) do
		if not tonumber(v) then
			TriggerEvent("UI.Toast", "Please enter a valid date of birth (use Day/Month/Year as the format)")
			return
		else
			dobData[k] = tonumber(v)
		end
	end

	if dobData[1] < 1 or dobData[1] > 31 then
		TriggerEvent("UI.Toast", "The date of birth day must be between 1 and 31")
		return
	end

	if dobData[2] < 1 or dobData[2] > 12 then
		TriggerEvent("UI.Toast", "The date of birth month must be between 1 and 12")
		return
	end

	if dobData[3] < 1900 or dobData[3] > 1999 then
		TriggerEvent("UI.Toast", "The date of birth year must be between 1900 and 1999")
		return
	end

	TriggerServerEvent("Login.CreateCharacter", data.firstName, data.lastName, data.DOB)
end)

RegisterNetEvent('UI.Close')
AddEventHandler('UI.Close', function(data, cb)
	closeUI()
	--doFade(true)
end)

RegisterNetEvent('UI.Toast')
AddEventHandler('UI.Toast', function(message, displayLength)
	SendNUIMessage({
		type = "toast",
		message = message,
		length = displayLength or 4000
	})
end)

AddEventHandler("Test.PrintJSON", function(thing)
	print(thing)
end)