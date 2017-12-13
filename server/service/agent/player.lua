local skynet = require "skynet"

local Player = {}

Player.__index = Player

function Player.new()
	local o = {}
	setmetatable(o, Player)
	return o
end

-- 数据库加载玩家数据
function Player:load(conf)
	self.url = conf.url
	self.id = conf.id
	self.account = conf.account
	self.name = conf.name
	self.agent = skynet.self()
	self.money = conf.money
	self.usertype = conf.usertype
	self.fd = conf.fd
	self.sex = conf.sex
end

function Player:rejoin()
	local info = {
		tab_id = msg.tab_id,
		id = self.id,
		agent = skynet.self(),
		name = player.name,
		headurl = player.url,
		fd = player.fd
	}
	local resp = skynet.call(env.room, "lua", "rejoin", info)
	if resp then
		env.send_msg(resp.name, resp.msg)
	end

end

return Player
