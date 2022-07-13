local first = true
AddEventHandler('onServerResourceStart', function (resource)
    if resource == "mysql-async" then
        exports['mysql-async']:mysql_configure()

        Citizen.CreateThread(function ()
            Citizen.Wait(0)
            TriggerEvent('GHMattiMySQLStarted')
            first = false
        end)
    elseif resource == "magicallitymain" and not first then
    	exports['mysql-async']:mysql_configure()

        Citizen.CreateThread(function ()
            Citizen.Wait(0)
            TriggerEvent('GHMattiMySQLStarted')
            first = false
        end)
    end
end)


