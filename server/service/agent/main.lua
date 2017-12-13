local skynet = require "skynet"
local socket = require "socket"
local dispatcher = require "dispatcher"
local protopack = require "protopack"
local login = require "handler.login"
local room = require "handler.room"
local table = require "handler.table"
local env = require "env"
local player = require "player"
local log = require "log"
local utils = require "utils"
local CONF

env.send_msg = function (name, msg)
	local data = protopack.pack(name, msg)
	socket.write(CONF.fd, data)
end

local sock_dispatcher = dispatcher.new()
env.dispatcher = sock_dispatcher

skynet.register_protocol {
	name = "client",
	id = skynet.PTYPE_CLIENT,
	unpack = function (data, sz)
		print("agent recv socket data",CONF.fd, sz)
		return skynet.tostring(data,sz)
	end,
	dispatch = function (_, _, str)
		local name, msg = protopack.unpack(str)
		sock_dispatcher:dispatch(name, msg)
	end
}

local CMD = {}

function CMD.start(conf)
	CONF = conf
	-- local id = skynet.call("id", "lua", "generate", "role")
	-- env.id = id

	env.account = conf.account
	env.name = conf.name
	env.url = conf.url
	env.id = conf.id
	env.money = conf.money
	--print("agent main start...", env.account, env.name, env.url, env.id, env.money)
	log.log("agent main start... %s, %d, %d", env.account, env.id, env.money)
	player:load(conf)
	login.register()
	room.register()
	table.register()
	skynet.call(conf.gate, "lua", "forward", conf.fd)
	return skynet.self()
end


function CMD.MoneyChange(newmoney)
	player.money = newmoney
	env.send_msg("Table.MoneyChange", {mymoney = player.money})	
end

function CMD.cost_money(moneyneed)
	player.money = player.money - moneyneed
	skynet.call("db", "lua", "updateMoney", {id = player.id, money = player.money})
	env.send_msg("Table.MoneyChange", {mymoney = player.money})	
end

function CMD.disconnect()
	--print("agent main disconnect()", player.id, player.fd)
	--log.log("agent main disconnect() = %d"..player.id)
	skynet.exit()
end

function CMD.send(name, msg)
	env.send_msg(name, msg)
end

skynet.start(function()
	skynet.dispatch("lua", function(_,_, command, ...)
		local f = CMD[command]
		skynet.ret(skynet.pack(f(...)))
	end)
end)
