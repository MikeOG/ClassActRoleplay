--[[
Los Santos Customs V1.1 
Credits - MythicalBro
/////License/////
Do not reupload/re release any part of this script without my permission
]]
local tbl = {
[1] = {locked = false, player = nil},
[2] = {locked = false, player = nil},
[3] = {locked = false, player = nil},
[4] = {locked = false, player = nil},
[5] = {locked = false, player = nil},
[6] = {locked = false, player = nil},
[7] = {locked = false, player = nil},
[8] = {locked = false, player = nil},
[9] = {locked = false, player = nil},
[10] = {locked = false, player = nil},
[11] = {locked = false, player = nil},

}
RegisterServerEvent('lockGarage')
AddEventHandler('lockGarage', function(b,garage)
	tbl[tonumber(garage)].locked = b
	if not b then
		tbl[tonumber(garage)].player = nil
	else
		tbl[tonumber(garage)].player = source
	end
	TriggerClientEvent('lockGarage',-1,tbl)
	--print(json.encode(tbl))
end)
RegisterServerEvent('getGarageInfo')
AddEventHandler('getGarageInfo', function()
	TriggerClientEvent('lockGarage',-1,tbl)
	--print(json.encode(tbl))
end)
AddEventHandler('playerDropped', function()
	for i,g in pairs(tbl) do
		if g.player then
			if source == g.player then
				g.locked = false
				g.player = nil
				TriggerClientEvent('lockGarage',-1,tbl)
			end
		end
	end
end)

local repairPrices = {}
RegisterServerEvent('setPlayerRepairPrice')
AddEventHandler('setPlayerRepairPrice', function(price)
	repairPrices[source] = price
end)

--RegisterServerEvent("LSC:buttonSelected")
--AddEventHandler("LSC:buttonSelected", function(name, button)
--	local itemCost = 100
--	if button.name == "Repair vehicle" then
--		itemCost = tonumber(repairPrices[source])
--		TriggerEvent("Customs.AttemptPaymentRepair-local", source, itemCost, name, button)
--	else
--		itemCost = 0
--		TriggerClientEvent("LSC:buttonSelected", source, name, button, true)
--		print ("purchased")
--	end
--end)


RegisterServerEvent("LSC:buttonSelectedPolice")
AddEventHandler("LSC:buttonSelectedPolice", function(name, button)
	local itemCost = 100
	if button.name == "Repair vehicle" then
		itemCost = tonumber(repairPrices[source])
		if IsPlayerAceAllowed(source, "customs.mechanicprice") then
			itemCost = itemCost * 0.5
		end
		TriggerEvent("Customs.AttemptPaymentRepair-local", source, itemCost, name, button)
	else
		itemCost = 0
		TriggerClientEvent("LSC:buttonSelected", source, name, button, true)

	end
end)

RegisterServerEvent("LSC:mechaniccheck")
AddEventHandler("LSC:mechaniccheck", function(src)
	if IsPlayerAceAllowed(source, "customs.mechanicprice") then
		TriggerClientEvent("LSC:mechanicreturn", source)
		TriggerClientEvent("LSC:menumechanicreturn", source)
	else
		TriggerClientEvent("LSC:mechanicreturnfalse", source)
		TriggerClientEvent("LSC:menumechanicreturnfalse", source)
	end
end)

RegisterServerEvent("LSC:magicmechanic")
AddEventHandler("LSC:magicmechanic", function(src)
		TriggerClientEvent("LSC:mechanicreturn", source)
		TriggerClientEvent("LSC:menumechanicreturn", source)
end)

RegisterServerEvent("LSC:buttonSelected")
AddEventHandler("LSC:buttonSelected", function(name, button)
	local itemCost = 100
	if button.name == "Repair vehicle" then
		itemCost = tonumber(repairPrices[source])
		TriggerEvent("Customs.AttemptPaymentRepair-local", source, itemCost, name, button)
	elseif button.price then
		TriggerEvent("Customs.AttemptPaymentRepair-local", source, button.price, name, button)
	else
		itemCost = 0
		TriggerClientEvent("LSC:buttonSelected", source, name, button, false)
	end
end)

RegisterServerEvent("LSC:finished")
AddEventHandler("LSC:finished", function(veh)
	local model = veh.model --Display name from vehicle model(comet2, entityxf)
	local mods = veh.mods
	--[[
	mods[0].mod - spoiler
	mods[1].mod - front bumper
	mods[2].mod - rearbumper
	mods[3].mod - skirts
	mods[4].mod - exhaust
	mods[5].mod - roll cage
	mods[6].mod - grille
	mods[7].mod - hood
	mods[8].mod - fenders
	mods[10].mod - roof
	mods[11].mod - engine
	mods[12].mod - brakes
	mods[13].mod - transmission
	mods[14].mod - horn
	mods[15].mod - suspension
	mods[16].mod - armor
	mods[23].mod - tires
	mods[23].variation - custom tires
	mods[24].mod - tires(Just for bikes, 23:front wheel 24:back wheel)
	mods[24].variation - custom tires(Just for bikes, 23:front wheel 24:back wheel)
	mods[25].mod - plate holder
	mods[26].mod - vanity plates
	mods[27].mod - trim design
	mods[28].mod - ornaments
	mods[29].mod - dashboard
	mods[30].mod - dial design
	mods[31].mod - doors
	mods[32].mod - seats
	mods[33].mod - steering wheels
	mods[34].mod - shift leavers
	mods[35].mod - plaques
	mods[36].mod - speakers
	mods[37].mod - trunk
	mods[38].mod - hydraulics
	mods[39].mod - engine block
	mods[40].mod - cam cover
	mods[41].mod - strut brace
	mods[42].mod - arch cover
	mods[43].mod - aerials
	mods[44].mod - roof scoops
	mods[45].mod - tank
	mods[46].mod - doors
	mods[48].mod - liveries
	
	--Toggle mods
	mods[20].mod - tyre smoke
	mods[22].mod - headlights
	mods[18].mod - turbo
	
	--]]
	local color = veh.color
	local extracolor = veh.extracolor
	local neoncolor = veh.neoncolor
	local smokecolor = veh.smokecolor
	local plateindex = veh.plateindex
	local windowtint = veh.windowtint
	local wheeltype = veh.wheeltype
	local bulletProofTyres = veh.bulletProofTyres
	--Do w/e u need with all this stuff when vehicle drives out of lsc
end)

--Temp serverside checking of buying vehicles
shopVehs = {} 
shopVehs[#shopVehs + 1] = {name = "9F Cabrio", model =  "ninef2", costs = 95000}
shopVehs[#shopVehs + 1] = {name = "9F", model =  "ninef", costs = 90000}
shopVehs[#shopVehs + 1] = {name = "Adder", model =  "adder", costs = 750000}
shopVehs[#shopVehs + 1] = {name = "Airtug", model =  "airtug", costs = 3000}
shopVehs[#shopVehs + 1] = {name = "Akuma", model =  "akuma", costs = 7500}
shopVehs[#shopVehs + 1] = {name = "Alpha", model =  "alpha", costs = 90000}
shopVehs[#shopVehs + 1] = {name = "Ardent", model =  "ardent", costs = 125000}
shopVehs[#shopVehs + 1] = {name = "Asea", model =  "asea", costs = 10000}
shopVehs[#shopVehs + 1] = {name = "Audi R8 2013", model =  "r8ppi", costs = 175000}
shopVehs[#shopVehs + 1] = {name = "Autarch", model =  "autarch", costs = 950000}
shopVehs[#shopVehs + 1] = {name = "Avarus", model =  "avarus", costs = 18000}
shopVehs[#shopVehs + 1] = {name = "Bagger", model =  "bagger", costs = 15000}
shopVehs[#shopVehs + 1] = {name = "Baller", model =  "baller", costs = 30000}
shopVehs[#shopVehs + 1] = {name = "Banshee 900r", model =  "banshee2", costs = 125000}
shopVehs[#shopVehs + 1] = {name = "Banshee", model =  "banshee", costs = 85000}
shopVehs[#shopVehs + 1] = {name = "Bati 801", model =  "bati", costs = 14000}
shopVehs[#shopVehs + 1] = {name = "Bati 801RR", model =  "bati2", costs = 16000}
shopVehs[#shopVehs + 1] = {name = "Benson", model =  "benson", costs = 25000}
shopVehs[#shopVehs + 1] = {name = "Bestia GTS", model =  "bestiagts", costs = 250000}
shopVehs[#shopVehs + 1] = {name = "BF400", model =  "bf400", costs = 9000}
shopVehs[#shopVehs + 1] = {name = "Biff", model =  "biff", costs = 25000}
shopVehs[#shopVehs + 1] = {name = "Bifta", model =  "bifta", costs = 8000}
shopVehs[#shopVehs + 1] = {name = "Bison", model =  "bison", costs = 12000}
shopVehs[#shopVehs + 1] = {name = "BJ Injection", model =  "bfinjection", costs = 12000}
shopVehs[#shopVehs + 1] = {name = "BJXL", model =  "bjxl", costs = 32000}
shopVehs[#shopVehs + 1] = {name = "Blade", model =  "blade", costs = 6500}
shopVehs[#shopVehs + 1] = {name = "Blazer Flame", model =  "blazer3", costs = 5000}
shopVehs[#shopVehs + 1] = {name = "Blazer", model =  "blazer", costs = 3500}
shopVehs[#shopVehs + 1] = {name = "Blista Compact", model =  "blista2", costs = 14500}
shopVehs[#shopVehs + 1] = {name = "Blista", model =  "blista", costs = 18000}
shopVehs[#shopVehs + 1] = {name = "BMX", model =  "bmx", costs = 400}
shopVehs[#shopVehs + 1] = {name = "Bobcat XL", model =  "bobcatxl", costs = 12000}
shopVehs[#shopVehs + 1] = {name = "Bodhi", model =  "bodhi2", costs = 16000}
shopVehs[#shopVehs + 1] = {name = "Boxville", model =  "boxville", costs = 26000}
shopVehs[#shopVehs + 1] = {name = "Brawler", model =  "brawler", costs = 75000}
shopVehs[#shopVehs + 1] = {name = "Brioso R/A", model =  "brioso", costs = 15000}
shopVehs[#shopVehs + 1] = {name = "Buccaneer", model =  "buccaneer", costs = 15000}
shopVehs[#shopVehs + 1] = {name = "Buffalo S", model =  "buffalo2", costs = 48000}
shopVehs[#shopVehs + 1] = {name = "Buffalo", model =  "buffalo", costs = 32000}
shopVehs[#shopVehs + 1] = {name = "Bullet", model =  "bullet", costs = 100000}
shopVehs[#shopVehs + 1] = {name = "Burrito", model =  "burrito", costs = 14000}
shopVehs[#shopVehs + 1] = {name = "Caddy 2", model =  "caddy2", costs = 2000}
shopVehs[#shopVehs + 1] = {name = "Caddy 3", model =  "caddy3", costs = 3500}
shopVehs[#shopVehs + 1] = {name = "Caddy", model =  "caddy", costs = 5500}
shopVehs[#shopVehs + 1] = {name = "Camper", model =  "camper", costs = 12000}
shopVehs[#shopVehs + 1] = {name = "Carbon RS", model =  "carbonrs", costs = 18000}
shopVehs[#shopVehs + 1] = {name = "Carbonizzare", model =  "carbonizzare", costs = 175000}
shopVehs[#shopVehs + 1] = {name = "Casco", model =  "casco", costs = 150000}
shopVehs[#shopVehs + 1] = {name = "Cavalcade", model =  "cavalcade", costs = 35000}
shopVehs[#shopVehs + 1] = {name = "Cheburek", model =  "cheburek", costs = 13000}
shopVehs[#shopVehs + 1] = {name = "Cheburek", model =  "cheburek", costs = 13000}
shopVehs[#shopVehs + 1] = {name = "Cheetah Classic", model =  "cheetah2", costs = 125000}
shopVehs[#shopVehs + 1] = {name = "Cheetah", model =  "cheetah", costs = 750000}
shopVehs[#shopVehs + 1] = {name = "Chimera", model =  "chimera", costs = 20000}
shopVehs[#shopVehs + 1] = {name = "Chino 2", model =  "chino2", costs = 26000}
shopVehs[#shopVehs + 1] = {name = "Chino", model =  "chino", costs = 18000}
shopVehs[#shopVehs + 1] = {name = "Cliffhanger", model =  "cliffhanger", costs = 12000}
shopVehs[#shopVehs + 1] = {name = "Cog55", model =  "cog55", costs = 70000}
shopVehs[#shopVehs + 1] = {name = "Cognoscenti Cabrio", model =  "cogcabrio", costs = 45000}
shopVehs[#shopVehs + 1] = {name = "Cognoscenti", model =  "cognoscenti", costs = 75000}
shopVehs[#shopVehs + 1] = {name = "Comet Retro", model =  "comet3", costs = 95000}
shopVehs[#shopVehs + 1] = {name = "Comet SR", model =  "comet5", costs = 95000}
shopVehs[#shopVehs + 1] = {name = "Comet", model =  "comet2", costs = 80000}
shopVehs[#shopVehs + 1] = {name = "Contender", model =  "contender", costs = 60000}
shopVehs[#shopVehs + 1] = {name = "Coquette Classic", model =  "coquette2", costs = 80000}
shopVehs[#shopVehs + 1] = {name = "Coquette", model =  "coquette", costs = 65000}
shopVehs[#shopVehs + 1] = {name = "Cruiser", model =  "cruiser", costs = 200}
shopVehs[#shopVehs + 1] = {name = "Daemon", model =  "daemon", costs = 18000}
shopVehs[#shopVehs + 1] = {name = "Defiler", model =  "defiler", costs = 10000}
shopVehs[#shopVehs + 1] = {name = "Diablous Custom", model =  "diablous2", costs = 16000}
shopVehs[#shopVehs + 1] = {name = "Diablous", model =  "diablous", costs = 16000}
shopVehs[#shopVehs + 1] = {name = "Diablous", model =  "diablous", costs = 16000}
shopVehs[#shopVehs + 1] = {name = "Dilettante", model =  "dilettante", costs = 17000}
shopVehs[#shopVehs + 1] = {name = "Dloader", model =  "dloader", costs = 24000}
shopVehs[#shopVehs + 1] = {name = "Docktug", model =  "docktug", costs = 4000}
shopVehs[#shopVehs + 1] = {name = "Dodge Viper 16", model =  "viper", costs = 210000}
shopVehs[#shopVehs + 1] = {name = "Dodge Viper 99", model =  "99viper", costs = 130000}
shopVehs[#shopVehs + 1] = {name = "Dominator GTX", model =  "dominator", costs = 45000}
shopVehs[#shopVehs + 1] = {name = "Dominator", model =  "dominator3", costs = 30000}
shopVehs[#shopVehs + 1] = {name = "Double T", model =  "double", costs = 15500}
shopVehs[#shopVehs + 1] = {name = "Dozer", model =  "bulldozer", costs = 70000}
shopVehs[#shopVehs + 1] = {name = "Dubsta 6x6", model =  "dubsta3", costs = 210000}
shopVehs[#shopVehs + 1] = {name = "Dubsta", model =  "dubsta", costs = 75000}
shopVehs[#shopVehs + 1] = {name = "Dukes", model =  "dukes", costs = 29000}
shopVehs[#shopVehs + 1] = {name = "Dune", model =  "dune", costs = 9000  }
shopVehs[#shopVehs + 1] = {name = "Elegy Retro", model =  "elegy", costs = 60000}
shopVehs[#shopVehs + 1] = {name = "Elegy", model =  "ELEGY2", costs = 60000}
shopVehs[#shopVehs + 1] = {name = "Ellie", model =  "ellie", costs = 32000}
shopVehs[#shopVehs + 1] = {name = "Emperor 2", model =  "emperor2", costs = 7000}
shopVehs[#shopVehs + 1] = {name = "Emperor", model =  "emperor", costs = 9000}
shopVehs[#shopVehs + 1] = {name = "Enduro", model =  "enduro", costs = 6000}
shopVehs[#shopVehs + 1] = {name = "Entity XF", model =  "entity", costs = 160000}
shopVehs[#shopVehs + 1] = {name = "Entity XXR", model =  "entity2", costs = 250000}
shopVehs[#shopVehs + 1] = {name = "Esskey", model =  "esskey", costs = 9500}
shopVehs[#shopVehs + 1] = {name = "ETR1", model =  "sheava", costs = 250000}
shopVehs[#shopVehs + 1] = {name = "Exemplar", model =  "exemplar", costs = 38000}
shopVehs[#shopVehs + 1] = {name = "F620", model =  "f620", costs = 75000}
shopVehs[#shopVehs + 1] = {name = "Faction Custom 2", model =  "faction3", costs = 26000}
shopVehs[#shopVehs + 1] = {name = "Faction Custom", model =  "faction2", costs = 18000}
shopVehs[#shopVehs + 1] = {name = "Faction", model =  "faction", costs = 9000}
shopVehs[#shopVehs + 1] = {name = "Fagaloa", model =  "fagaloa", costs = 16000}
shopVehs[#shopVehs + 1] = {name = "Fagaloa", model =  "fagaloa", costs = 16000}
shopVehs[#shopVehs + 1] = {name = "Faggio 2", model =  "faggio2", costs = 2000}
shopVehs[#shopVehs + 1] = {name = "Faggio 3", model =  "faggio3", costs = 2400}
shopVehs[#shopVehs + 1] = {name = "Faggio", model =  "faggio", costs = 2500}
shopVehs[#shopVehs + 1] = {name = "FCR Custom", model =  "fcr2", costs = 10000}
shopVehs[#shopVehs + 1] = {name = "FCR", model =  "fcr", costs = 10000}
shopVehs[#shopVehs + 1] = {name = "Felon GT", model =  "felon2", costs = 60000}
shopVehs[#shopVehs + 1] = {name = "Felon", model =  "felon", costs = 50000}
shopVehs[#shopVehs + 1] = {name = "Feltzer", model =  "feltzer2", costs = 85000}
shopVehs[#shopVehs + 1] = {name = "Fixter", model =  "fixter", costs = 500}
shopVehs[#shopVehs + 1] = {name = "Flash GT", model =  "flashgt", costs = 60000}
shopVehs[#shopVehs + 1] = {name = "Forklift", model =  "forklift", costs = 9000}
shopVehs[#shopVehs + 1] = {name = "FQ2", model =  "fq2", costs = 35000}
shopVehs[#shopVehs + 1] = {name = "Fugitive", model =  "fugitive", costs = 24000}
shopVehs[#shopVehs + 1] = {name = "Furore GT", model =  "furoregt", costs = 75000}
shopVehs[#shopVehs + 1] = {name = "Fusilade", model =  "fusilade", costs = 20000}
shopVehs[#shopVehs + 1] = {name = "Futo", model =  "futo", costs = 23000}
shopVehs[#shopVehs + 1] = {name = "Gargoyle", model =  "gargoyle", costs = 16000}
shopVehs[#shopVehs + 1] = {name = "Gauntlet", model =  "gauntlet", costs = 32000}
shopVehs[#shopVehs + 1] = {name = "GB200", model =  "gb200", costs = 32000}
shopVehs[#shopVehs + 1] = {name = "Glendale", model =  "glendale", costs = 14000}
shopVehs[#shopVehs + 1] = {name = "Granger", model =  "granger", costs = 32000}
shopVehs[#shopVehs + 1] = {name = "Gresley", model =  "gresley", costs = 22000}
shopVehs[#shopVehs + 1] = {name = "GT500", model =  "gt500", costs = 55000}
shopVehs[#shopVehs + 1] = {name = "Guardian", model =  "guardian", costs = 45000}
shopVehs[#shopVehs + 1] = {name = "Habanero", model =  "habanero", costs = 35000}
shopVehs[#shopVehs + 1] = {name = "Hakuchou Drag", model =  "hakuchou2", costs = 28000}
shopVehs[#shopVehs + 1] = {name = "Hakuchou", model =  "hakuchou", costs = 22500}
shopVehs[#shopVehs + 1] = {name = "Hauler", model =  "hauler", costs = 25000}
shopVehs[#shopVehs + 1] = {name = "Hermes", model =  "hermes", costs = 32000}
shopVehs[#shopVehs + 1] = {name = "Hexer", model =  "hexer", costs = 18000}
shopVehs[#shopVehs + 1] = {name = "Honda City Turbo", model =  "city85", costs = 13500}
shopVehs[#shopVehs + 1] = {name = "Honda Integra", model =  "dc2", costs = 45000}
shopVehs[#shopVehs + 1] = {name = "Honda S2000", model =  "ap2", costs = 72500}
shopVehs[#shopVehs + 1] = {name = "Hotknife", model =  "hotknife", costs = 27000}
shopVehs[#shopVehs + 1] = {name = "Hotring", model =  "hotring", costs = 54000}
shopVehs[#shopVehs + 1] = {name = "Huntley", model =  "huntley", costs = 30000}
shopVehs[#shopVehs + 1] = {name = "Hustler", model =  "hustler", costs = 35000}
shopVehs[#shopVehs + 1] = {name = "Infernus Classic", model =  "infernus2", costs = 120000}
shopVehs[#shopVehs + 1] = {name = "Infernus", model =  "infernus", costs = 225000}
shopVehs[#shopVehs + 1] = {name = "Ingot", model =  "ingot", costs = 8500}
shopVehs[#shopVehs + 1] = {name = "Innovation", model =  "innovation", costs = 20000}
shopVehs[#shopVehs + 1] = {name = "Intruder", model =  "intruder", costs = 23000}
shopVehs[#shopVehs + 1] = {name = "Issi Retro", model =  "issi3", costs = 18000}
shopVehs[#shopVehs + 1] = {name = "Issi", model =  "issi2", costs = 16000}
shopVehs[#shopVehs + 1] = {name = "Italigtb", model =  "italigtb", costs = 120000}
shopVehs[#shopVehs + 1] = {name = "Italigtb2", model =  "italigtb2", costs = 160000}
shopVehs[#shopVehs + 1] = {name = "Jackal", model =  "jackal", costs = 28000}
shopVehs[#shopVehs + 1] = {name = "Jester Retro", model =  "jester3", costs = 105000}
shopVehs[#shopVehs + 1] = {name = "Jester", model =  "jester", costs = 90000}
shopVehs[#shopVehs + 1] = {name = "Journey", model =  "journey", costs = 12000}
shopVehs[#shopVehs + 1] = {name = "Kalahari", model =  "kalahari", costs = 18000}
shopVehs[#shopVehs + 1] = {name = "Kamacho", model =  "kamacho", costs = 60000}
shopVehs[#shopVehs + 1] = {name = "Khamelion", model =  "KHAMELION", costs = 110000}
shopVehs[#shopVehs + 1] = {name = "Kuruma", model =  "kuruma", costs = 40000}
shopVehs[#shopVehs + 1] = {name = "Landstalker", model =  "landstalker", costs = 32000}
shopVehs[#shopVehs + 1] = {name = "Lawnmower", model =  "mower", costs = 4000}
shopVehs[#shopVehs + 1] = {name = "Lectro", model =  "lectro", costs = 18000}
shopVehs[#shopVehs + 1] = {name = "Lurcher", model =  "lurcher", costs = 29000}
shopVehs[#shopVehs + 1] = {name = "Lynx", model =  "lynx", costs = 60000}
shopVehs[#shopVehs + 1] = {name = "Mamba", model =  "mamba", costs = 95000}
shopVehs[#shopVehs + 1] = {name = "Manana", model =  "manana", costs = 18000}
shopVehs[#shopVehs + 1] = {name = "Manchez", model =  "manchez", costs = 8000}
shopVehs[#shopVehs + 1] = {name = "Massacro (Racecar)", model =  "massacro2", costs = 175000}
shopVehs[#shopVehs + 1] = {name = "Massacro", model =  "massacro", costs = 150000}
shopVehs[#shopVehs + 1] = {name = "Mesa 2", model =  "mesa3", costs = 30000}
shopVehs[#shopVehs + 1] = {name = "Mesa", model =  "mesa", costs = 24000}
shopVehs[#shopVehs + 1] = {name = "Michelli GT", model =  "michelli", costs = 80000}
shopVehs[#shopVehs + 1] = {name = "Minivan", model =  "minivan", costs = 18000}
shopVehs[#shopVehs + 1] = {name = "Mitsubishi Eclipse", model =  "eclipse", costs = 55000}
shopVehs[#shopVehs + 1] = {name = "Mitsubishi Lancer Evo IX", model =  "evo9", costs = 75000}
shopVehs[#shopVehs + 1] = {name = "Mitsubishi Lancer Evo VI", model =  "cp9a", costs = 65000}
shopVehs[#shopVehs + 1] = {name = "Mitsubishi Lancer Evo X", model =  "lancerevox", costs = 90000}
shopVehs[#shopVehs + 1] = {name = "Mixer", model =  "mixer2", costs = 25000}
shopVehs[#shopVehs + 1] = {name = "Monroe", model =  "monroe", costs = 175000}
shopVehs[#shopVehs + 1] = {name = "Moonbeam", model =  "moonbeam", costs = 10000}
shopVehs[#shopVehs + 1] = {name = "Motocompo Scooter", model =  "motoc", costs = 2100}
shopVehs[#shopVehs + 1] = {name = "Mule", model =  "mule", costs = 25000}
shopVehs[#shopVehs + 1] = {name = "Nemesis", model =  "nemesis", costs = 10000}
shopVehs[#shopVehs + 1] = {name = "Neon", model =  "neon", costs = 180000}
shopVehs[#shopVehs + 1] = {name = "Nero Custom", model =  "nero", costs = 780000}
shopVehs[#shopVehs + 1] = {name = "Nero", model =  "nero", costs = 750000}
shopVehs[#shopVehs + 1] = {name = "Nightblade", model =  "nightblade", costs = 18000}
shopVehs[#shopVehs + 1] = {name = "Nightshade", model =  "nightshade", costs = 27000}
shopVehs[#shopVehs + 1] = {name = "Nissan 180sx", model =  "180sx", costs = 83500}
shopVehs[#shopVehs + 1] = {name = "Nissan 370z", model =  "370z", costs = 110500}
shopVehs[#shopVehs + 1] = {name = "Omnis", model =  "omnis", costs = 125000}
shopVehs[#shopVehs + 1] = {name = "Oracle XS", model =  "oracle2", costs = 42000}
shopVehs[#shopVehs + 1] = {name = "Oracle", model =  "oracle", costs = 38000}
shopVehs[#shopVehs + 1] = {name = "Osiris", model =  "osiris", costs = 525000}
shopVehs[#shopVehs + 1] = {name = "Packer", model =  "packer", costs = 25000}
shopVehs[#shopVehs + 1] = {name = "Packer", model =  "packer", costs = 25000}
shopVehs[#shopVehs + 1] = {name = "Panto", model =  "panto", costs = 11000}
shopVehs[#shopVehs + 1] = {name = "Paradise", model =  "paradise", costs = 12000}
shopVehs[#shopVehs + 1] = {name = "Pariah", model =  "pariah", costs = 70000}
shopVehs[#shopVehs + 1] = {name = "Patriot", model =  "patriot", costs = 25000}
shopVehs[#shopVehs + 1] = {name = "PCJ 675", model =  "pcj", costs = 7000}
shopVehs[#shopVehs + 1] = {name = "Penetrator", model =  "penetrator", costs = 250000}
shopVehs[#shopVehs + 1] = {name = "Penumbra", model =  "penumbra", costs = 23000}
shopVehs[#shopVehs + 1] = {name = "Peyote", model =  "peyote", costs = 30000}
shopVehs[#shopVehs + 1] = {name = "Pfister811", model =  "pfister811", costs = 350000}
shopVehs[#shopVehs + 1] = {name = "Phantom", model =  "phantom", costs = 25000}
shopVehs[#shopVehs + 1] = {name = "Phoenix", model =  "phoenix", costs = 25000}
shopVehs[#shopVehs + 1] = {name = "Picador", model =  "picador", costs = 6000}
shopVehs[#shopVehs + 1] = {name = "Pigalle", model =  "pigalle", costs = 35000}
shopVehs[#shopVehs + 1] = {name = "Plymouth Superbird 70", model =  "superbird", costs = 55500}
shopVehs[#shopVehs + 1] = {name = "Pony", model =  "pony", costs = 14000}
shopVehs[#shopVehs + 1] = {name = "Pounder", model =  "pounder", costs = 25000}
shopVehs[#shopVehs + 1] = {name = "Prairie", model =  "prairie", costs = 14000}
shopVehs[#shopVehs + 1] = {name = "Premier", model =  "premier", costs = 12000}
shopVehs[#shopVehs + 1] = {name = "Primo Custom", model =  "primo2", costs = 13000}
shopVehs[#shopVehs + 1] = {name = "Primo", model =  "primo", costs = 8000}
shopVehs[#shopVehs + 1] = {name = "Radius", model =  "radi", costs = 16000}
shopVehs[#shopVehs + 1] = {name = "Raiden", model =  "raiden", costs = 90000}
shopVehs[#shopVehs + 1] = {name = "Rancher XL", model =  "rancherxl", costs = 14000}
shopVehs[#shopVehs + 1] = {name = "Rapid GT Classic", model =  "rapidgt3", costs = 35000}
shopVehs[#shopVehs + 1] = {name = "Rapid GT", model =  "rapidgt", costs = 85000}
shopVehs[#shopVehs + 1] = {name = "Raptor", model =  "RAPTOR", costs = 58000}
shopVehs[#shopVehs + 1] = {name = "Rat-Loader", model =  "ratloader", costs = 28000}
shopVehs[#shopVehs + 1] = {name = "Rat-Truck", model =  "ratloader2", costs = 35000}
shopVehs[#shopVehs + 1] = {name = "Ratbike", model =  "ratbike", costs = 7000}
shopVehs[#shopVehs + 1] = {name = "RE-7B", model =  "le7b", costs = 750000}
shopVehs[#shopVehs + 1] = {name = "Reaper", model =  "reaper", costs = 160000}
shopVehs[#shopVehs + 1] = {name = "Rebel 2", model =  "rebel2", costs = 16000}
shopVehs[#shopVehs + 1] = {name = "Rebel", model =  "rebel", costs = 12500}
shopVehs[#shopVehs + 1] = {name = "Regina", model =  "regina", costs = 12000}
shopVehs[#shopVehs + 1] = {name = "Retinue", model =  "retinue", costs = 80000}
shopVehs[#shopVehs + 1] = {name = "Rhapsody", model =  "rhapsody", costs = 12000}
shopVehs[#shopVehs + 1] = {name = "Riata", model =  "riata", costs = 25000}
shopVehs[#shopVehs + 1] = {name = "Rocoto", model =  "rocoto", costs = 34000}
shopVehs[#shopVehs + 1] = {name = "Romero", model =  "romero", costs = 28000}
shopVehs[#shopVehs + 1] = {name = "Rubble", model =  "rubble", costs = 25000}
shopVehs[#shopVehs + 1] = {name = "Ruffian", model =  "ruffian", costs = 9000}
shopVehs[#shopVehs + 1] = {name = "Ruiner", model =  "ruiner", costs = 12000}
shopVehs[#shopVehs + 1] = {name = "Rumpo", model =  "rumpo", costs = 8000}
shopVehs[#shopVehs + 1] = {name = "Ruston", model =  "RUSTON", costs = 62000}
shopVehs[#shopVehs + 1] = {name = "Sabre Custom", model =  "sabregt2", costs = 24000}
shopVehs[#shopVehs + 1] = {name = "Sabre Turbo", model =  "sabregt", costs = 24000}
shopVehs[#shopVehs + 1] = {name = "Sanchez", model =  "sanchez", costs = 7000}
shopVehs[#shopVehs + 1] = {name = "Sanctus", model =  "sanctus", costs = 28000}
shopVehs[#shopVehs + 1] = {name = "Sandking 2", model =  "sandking2", costs = 24000}
shopVehs[#shopVehs + 1] = {name = "Sandking", model =  "sandking", costs = 28000}
shopVehs[#shopVehs + 1] = {name = "SC1", model =  "sc1", costs = 150000}
shopVehs[#shopVehs + 1] = {name = "Schwarzer", model =  "Schwarzer", costs = 40000}
shopVehs[#shopVehs + 1] = {name = "Scorcher",  model = "scorcher", costs = 450}
shopVehs[#shopVehs + 1] = {name = "Scrap Truck", model =  "SCRAP", costs = 12000}
shopVehs[#shopVehs + 1] = {name = "Seminole", model =  "seminole", costs = 32000}
shopVehs[#shopVehs + 1] = {name = "Sentinel Classic", model =  "sentinel3", costs = 35000}
shopVehs[#shopVehs + 1] = {name = "Sentinel XS", model =  "sentinel2", costs = 42000}
shopVehs[#shopVehs + 1] = {name = "Sentinel", model =  "sentinel", costs = 35000}
shopVehs[#shopVehs + 1] = {name = "Serrano", model =  "serrano", costs = 22000}
shopVehs[#shopVehs + 1] = {name = "Seven-70", model =  "SEVEN70", costs = 425000}
shopVehs[#shopVehs + 1] = {name = "Slamvan Custom", model =  "slamvan3", costs = 24000}
shopVehs[#shopVehs + 1] = {name = "Slamvan Lost", model =  "slamvan2", costs = 24000}
shopVehs[#shopVehs + 1] = {name = "Slamvan", model =  "slamvan", costs = 20000}
shopVehs[#shopVehs + 1] = {name = "Sovereign", model =  "sovereign", costs = 16000}
shopVehs[#shopVehs + 1] = {name = "Specter Custom", model =  "specter", costs = 135000}
shopVehs[#shopVehs + 1] = {name = "Specter", model =  "specter", costs = 125000}
shopVehs[#shopVehs + 1] = {name = "Speedo", model =  "speedo", costs = 10000}
shopVehs[#shopVehs + 1] = {name = "Stalion", model =  "stalion", costs = 30000}
shopVehs[#shopVehs + 1] = {name = "Stanier", model =  "stanier", costs = 11000}
shopVehs[#shopVehs + 1] = {name = "Stinger", model =  "stinger", costs = 425000}
shopVehs[#shopVehs + 1] = {name = "Stratum", model =  "stratum", costs = 12000}
shopVehs[#shopVehs + 1] = {name = "Streiter", model =  "streiter", costs = 40000}
shopVehs[#shopVehs + 1] = {name = "Stretch", model =  "stretch", costs = 48000}
shopVehs[#shopVehs + 1] = {name = "Sultan RS", model =  "sultanrs", costs = 60000}
shopVehs[#shopVehs + 1] = {name = "Sultan", model =  "sultan", costs = 15000}
shopVehs[#shopVehs + 1] = {name = "Surano", model =  "surano", costs = 55000}
shopVehs[#shopVehs + 1] = {name = "Surfer", model =  "surfer", costs = 14000}
shopVehs[#shopVehs + 1] = {name = "Surge", model =  "surge", costs = 24000}
shopVehs[#shopVehs + 1] = {name = "Swinger", model =  "winger", costs = 120000}
shopVehs[#shopVehs + 1] = {name = "T20", model =  "t20", costs = 125000}
shopVehs[#shopVehs + 1] = {name = "Tailgater", model =  "tailgater", costs = 28000}
shopVehs[#shopVehs + 1] = {name = "Taipan", model =  "taipan", costs = 650000}
shopVehs[#shopVehs + 1] = {name = "Tampa 2", model =  "tampa2", costs = 35000}
shopVehs[#shopVehs + 1] = {name = "Tampa", model =  "tampa", costs = 25000}
shopVehs[#shopVehs + 1] = {name = "Tempesta", model =  "tempesta", costs = 180000}
shopVehs[#shopVehs + 1] = {name = "Tezeract", model =  "tezeract", costs = 1600000}
shopVehs[#shopVehs + 1] = {name = "Thrust", model =  "thrust", costs = 8000}
shopVehs[#shopVehs + 1] = {name = "Tipper 2", model =  "tiptruck2", costs = 25000}
shopVehs[#shopVehs + 1] = {name = "Tipper", model =  "tiptruck", costs = 25000}
shopVehs[#shopVehs + 1] = {name = "Toreno", model =  "torero", costs = 250000}
shopVehs[#shopVehs + 1] = {name = "Tornado 2", model =  "tornado2", costs = 50000}
shopVehs[#shopVehs + 1] = {name = "Tornado 3", model =  "tornado3", costs = 20000}
shopVehs[#shopVehs + 1] = {name = "Tornado 4", model =  "tornado4", costs = 22000}
shopVehs[#shopVehs + 1] = {name = "Tornado 5", model =  "tornado5", costs = 55000}
shopVehs[#shopVehs + 1] = {name = "Tour Bus", model =  "tourbus", costs = 17000}
shopVehs[#shopVehs + 1] = {name = "Tow Truck 2", model =  "towtruck2", costs = 17000}
shopVehs[#shopVehs + 1] = {name = "Tow Truck", model =  "towtruck", costs = 20000}
shopVehs[#shopVehs + 1] = {name = "Toyota FJ40 Hardtop", model =  "fj40b", costs = 34500}
shopVehs[#shopVehs + 1] = {name = "Toyota FJ40 Softtop", model =  "fj40", costs = 33000}
shopVehs[#shopVehs + 1] = {name = "Toyota Supra", model =  "jza80", costs = 80500}
shopVehs[#shopVehs + 1] = {name = "Tractor (Fieldmaster)", model =  "tractor2", costs = 9000}
shopVehs[#shopVehs + 1] = {name = "Tractor (Vintage)", model =  "tractor", costs = 3300}
shopVehs[#shopVehs + 1] = {name = "Triathlon bike 1", model =  "tribike", costs = 1000}
shopVehs[#shopVehs + 1] = {name = "Triathlon bike 2", model =  "tribike2", costs = 1000}
shopVehs[#shopVehs + 1] = {name = "Triathlon bike 3", model =  "tribike3", costs = 1000}
shopVehs[#shopVehs + 1] = {name = "Trophy Truck", model =  "trophytruck", costs = 30000}
shopVehs[#shopVehs + 1] = {name = "Tropos", model =  "tropos", costs = 125000}
shopVehs[#shopVehs + 1] = {name = "Turismo Classic", model =  "turismo2", costs = 55000}
shopVehs[#shopVehs + 1] = {name = "Turismo R", model =  "turismor", costs = 1250000}
shopVehs[#shopVehs + 1] = {name = "Tyrant", model =  "tyrant", costs = 1600000}
shopVehs[#shopVehs + 1] = {name = "Tyrus", model =  "tyrus", costs = 1600000}
shopVehs[#shopVehs + 1] = {name = "Utility Pickup Truck", model =  "utillitruck3", costs = 30000}
shopVehs[#shopVehs + 1] = {name = "Utility Truck 2", model =  "utillitruck2", costs = 25000}
shopVehs[#shopVehs + 1] = {name = "Utility Truck", model =  "utillitruck", costs = 25000}
shopVehs[#shopVehs + 1] = {name = "Vacca", model =  "vacca", costs = 135000}
shopVehs[#shopVehs + 1] = {name = "Vader", model =  "vader", costs = 9000}
shopVehs[#shopVehs + 1] = {name = "Vagner", model =  "vagner", costs = 1000000}
shopVehs[#shopVehs + 1] = {name = "Verkierer", model =  "verlierer2", costs = 79000}
shopVehs[#shopVehs + 1] = {name = "Vigero", model =  "vigero", costs = 32000}
shopVehs[#shopVehs + 1] = {name = "Vindicator", model =  "vindicator", costs = 7000}
shopVehs[#shopVehs + 1] = {name = "Virgo", model =  "virgo", costs = 23000}
shopVehs[#shopVehs + 1] = {name = "Visione", model =  "visione", costs = 800000}
shopVehs[#shopVehs + 1] = {name = "Voltic", model =  "voltic", costs = 125000}
shopVehs[#shopVehs + 1] = {name = "Volvo 242", model =  "v242", costs = 7800}
shopVehs[#shopVehs + 1] = {name = "Voodoo", model =  "voodoo", costs = 16000}
shopVehs[#shopVehs + 1] = {name = "Vortex", model =  "vortex", costs = 21000}
shopVehs[#shopVehs + 1] = {name = "VW Beetle 74", model =  "beetle74", costs = 23500}
shopVehs[#shopVehs + 1] = {name = "Warrener", model =  "warrener", costs = 75000}
shopVehs[#shopVehs + 1] = {name = "Washington", model =  "washington", costs = 12000}
shopVehs[#shopVehs + 1] = {name = "Windsor Drop", model =  "windsor2", costs = 140000}
shopVehs[#shopVehs + 1] = {name = "Windsor", model =  "windsor", costs = 120000}
shopVehs[#shopVehs + 1] = {name = "Wolfsbane", model =  "wolfsbane", costs = 10000}
shopVehs[#shopVehs + 1] = {name = "X80 Proto", model =  "prototipo", costs = 1500000}
shopVehs[#shopVehs + 1] = {name = "XA21", model =  "xa21", costs = 750000}
shopVehs[#shopVehs + 1] = {name = "XLS", model =  "xls", costs = 35000}
shopVehs[#shopVehs + 1] = {name = "Yosemite", model =  "yosemite", costs = 24000}
shopVehs[#shopVehs + 1] = {name = "Youga", model =  "youga", costs = 9000}
shopVehs[#shopVehs + 1] = {name = "Zentorno", model =  "zentorno", costs = 350000}
shopVehs[#shopVehs + 1] = {name = "Zion Cabrio", model =  "zion2", costs = 40000}
shopVehs[#shopVehs + 1] = {name = "Zion", model =  "zion", costs = 34000}
shopVehs[#shopVehs + 1] = {name = "Zombie A", model =  "zombiea", costs = 12000}
shopVehs[#shopVehs + 1] = {name = "Zombie B", model =  "zombieb", costs = 10000}

RegisterServerEvent('getVehPricing')
AddEventHandler('getVehPricing', function()
	TriggerClientEvent('sendCustomPricing', source, shopVehs)
end)