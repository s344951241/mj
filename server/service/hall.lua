local skynet = require "skynet"
require "skynet.manager"
local log = require "log"

local rooms = {}

local function create(id, game)
	local conf = {id = id}
	local s = skynet.newservice(game)
	skynet.call(s, "lua", "start", conf)
	return s
end

local CMD = {}

function CMD.start()
	log.log("starting hall... ")
	-- for i=1,3 do
	local i = 1
	table.insert(rooms, {id = i, game = "mj", service = create(i, "mj")})
	-- end
end

function CMD.list()
	return rooms
end

function CMD.info(room_id)
	local info = rooms[room_id]
	if not info then
		return false
	end

	return info
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
	skynet.register("hall")
end)
