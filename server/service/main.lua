local skynet = require "skynet"
local utils = require "utils"

local max_client = 20000

skynet.start(function()
	skynet.error("Server start")

	skynet.newservice("console")
	skynet.newservice("debug_console",8000)

	local watchdog = skynet.newservice("watchdog")
	skynet.call(watchdog, "lua", "start", {
		port = 8888,
		maxclient = max_client,
		nodelay = true,
	})
	skynet.error("Watchdog listen on", 8888)

	local service = skynet.newservice("pbc")

	--db:init()

	service = skynet.newservice("hall")
	skynet.call(service, "lua", "start")

	service = skynet.newservice("db")
	skynet.call(service, "lua", "start")

	service = skynet.uniqueservice("id")
	skynet.call(service, "lua", "start")


	skynet.exit()
end)
