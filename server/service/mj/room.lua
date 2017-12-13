local Tab = require "tab"
local utils = require "utils"
local timer_mgr = require "timer_mgr"
local log = require "log"
local M = {}

M.__index = M

function M.new()
	local o = {
		players = {},
		tables = {},
		tablestime = {}, --table创建时间
		player_2_tab = {},
		has_inited = false,
		id = 1
	}
	setmetatable(o, M)
	return o
end



function M:init()
	if self.has_inited == false then
   		local timer_mgr = timer_mgr.new()
 		timer_mgr:add(1*1000, -1, function() self:on_timer() end)
 		self.has_inited = true		
	end
end

function M:getTableList()
	local ret = {}
	for id,t in pairs(self.tables) do
		--print("getTableList id = ", id)
		table.insert(ret, id)
	end
	return ret
end

function M:on_timer()
	for id,table in pairs(self.tables) do
		--print(k,v)
		table:on_timer()
	end
	--local now = skynet.time()
end

function M:get_tab_id()
	--self.id = self.id+1
	--return self.id
	local randnum = math.random(0, 500000) + 100000
	for i=1,10 do
		if self.tables[randnum] == nil then --房间存在再取一个
			break
		else
			randnum = math.random(0, 500000) + 100000
		end
	end
	return randnum
end

function M:enter(info)
	local old = self.players[info.id]
    if old then
		return false
	end

	self.players[info.id] = info
	
	return true
end

function M:add_table(tab)
	--print("add table:", tab.id)
	self.tables[tab.id] = tab
	--self.tablestime[tab.id] = skynet.time()
	-- self.player_2_tab[tab.p1.id] = tab.id
	-- self.player_2_tab[tab.p2.id] = tab.id
	-- self.player_2_tab[tab.p3.id] = tab.id
	-- self.player_2_tab[tab.p4.id] = tab.id
end

function M:test(player_info)
	print("call Room test "..player_info.id)
end

function M:remove_table(tab_id)
	log.log("remove_table = %d", tab_id)
	self.tables[tab_id] = nil
	self.tablestime[tab_id] = nil
end 

function M:remove_player_id_tab(id,tab_id)
	log.log("remove_player_id_tab = %d tab_id = %d", id, tab_id)
	if self.player_2_tab[id] == tab_id then
		self.player_2_tab[id] = nil
	end
end

function M:remove_player_id_tab_force(id)
	self.player_2_tab[id] = nil
end

function M:set_player_id_tab(id, tab_id)
	--print("set player tab id:", id, tab_id)
	self.player_2_tab[id] = tab_id
end

function M:get_player_table_id(id)
	return self.player_2_tab[id]
end

function M:leave(id)
	self.players[id] = nil
end

function M:get_table_by_id(tab_id)
	--print("get table:", tab_id, type(tab_id))
	-- print(self)
	-- print(self.tables[tab_id])
	return self.tables[tab_id]
end

function M:get_table_by_player_id(id)
	local tab_id = self.player_2_tab[id]
	--utils.print(self.player_2_tab)	
	if tab_id == nil then
		return nil
	end

	return self.tables[tab_id]
end

return M
