local skynet = require "skynet"
require "skynet.manager"
local log = require "log"

local id_table = {}

local CMD = {}

function CMD.start()
	log.log("starting id... ")
	id_table["role"] = 1
end

function CMD.generate( tag )
	local id = id_table[tag]
	id_table[tag] = id + 1
	return id
end

skynet.start(function ()
	skynet.dispatch("lua", function (_, _, cmd, ...)
		local f = CMD[cmd]
		if f then
			skynet.ret(skynet.pack(f(...)))
		else
			skynet.ret(skynet.pack(nil, "cant find handle of "..cmd))
		end
	end)
	skynet.register("id")
end)