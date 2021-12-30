function w3wfunc(source, args, rawCommand)
	if args[1] == nil then
		TriggerClientEvent("What3Words:ShowCurrentWord", source)
	else
		TriggerClientEvent("What3Words:CoordsFromWord", source, tostring(args[1]))
	end
end

RegisterCommand('w3w', w3wfunc, false)