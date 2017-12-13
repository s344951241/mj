local skynet = require "skynet"
local Env = require "env"
local Room = require "room"
local Log = require "log"
local match = require "match"
local Tab = require "tab"
local socket = require "socket"
local utils = require "utils"
local timer_mgr = require "timer_mgr"

local CMD = {}
local has_inited = false
local fd_list = {}
local HEART_BEAT_TIMEOUT = 60 * 5
local stop_server = false
local function match_loop()
	print("match loop")

	for k,v in pairs(socket) do
		print (k,v)
	end

	-- repeat
	-- 	local p1, p2, p3, p4 = match:peek()
	-- 	if p1 then
	-- 		local table = Tab.new()
	-- 		table:init(p1,p2,p3,p4)
	-- 		Env.room:add_table(table)
	-- 	end

	-- 	skynet.sleep(100)
	-- until(false)
end

local function save_msg_time(info)
	if info ~= nil then
		if info.fd ~= nil then
			fd_list[info.fd] = skynet.time()
		end
	end
end

function CMD.init()
	local function on_timer()
		local now = skynet.time()
		--print("heartbeat check, now = ", now)
		for fd,time in pairs(fd_list) do			
			local delta_time = now - time
			if delta_time > HEART_BEAT_TIMEOUT then
				skynet.call("watchdog", "lua", "close", fd)
				fd_list[fd] = nil
			end

		end
	end

	if has_inited == false then
   		local timer_mgr = timer_mgr.new()
 		timer_mgr:add(30*1000, -1, function() on_timer() end)
 		has_inited = true		
	end
end



function CMD.start(conf)
	Log.log("starting room %d", conf.id)
	Env.id = conf.id	
	Env.room = Room.new()
	Env.room:init()
	Env.match = match
	CMD.init()
	-- skynet.fork(match_loop)
	return true
end

function CMD.enter(info)
	--save_msg_time(info)
	if Env.room:enter(info) then
		return true
	else
		return false
	end
end

function CMD.leave(id)
	Env.room:leave(id)
end

function CMD.match(info)
	match:add(info)
	return 
end



function CMD.rejoin(info)
	local t = Env.room:get_table_by_id(info.tab_id)
	if t == nil then --房间不存在
		local resp = {}
		resp.name = "Table.JoinRsp"
		resp.msg = {err_no = 1, roles = {}, owner = 0, rule = {}}   --err_no =0, 成功 1，房间不存在，2，人数已满,3，已经在座位上，可能别处登录
		--print("重新连接房间不存在.."..info.tab_id)
		Log.log("重新连接房间不存在.. id = %d tab_id = %d", info.id, info.tab_id)
		Env.room:remove_player_id_tab_force(info.id)
		return resp
	end
	return t:rejoin(info)
	--end	
end

function CMD.create(info, msg)
	if stop_server == true then
		local resp1 = {}
		resp1.name = "Table.SysErrorRsp"
		resp1.msg = {err_info = "服务器即将维护停机，暂时无法创建房间", err_type = 2}
		return resp1	
	end


	local table = Tab.new(info)
	Env.room:add_table(table)
	table:init_rules(msg)
	-- table.join(info)
	local resp = {}
	resp.name = "Table.CreateRsp"
	resp.msg = {tab_id = table.id}
	save_msg_time(info)
	return resp
end

function CMD.join(info)
	local t = Env.room:get_table_by_id(info.tab_id)
	save_msg_time(info)
	if t == nil then --房间不存在
		local resp = {}
		resp.name = "Table.JoinRsp"
		resp.msg = {err_no = 1, roles = {}, owner = 0, rule = {}}  --err_no =0, 成功 1，房间不存在，2，人数已满,3，已经在座位上，可能别处登录
		--print("join房间不存在.."..info.tab_id)
		Log.log("加入房间不存在.. id = %d tab_id = %d", info.id, info.tab_id)
		return resp
	end

	Env.room:set_player_id_tab(info.id, info.tab_id)
	local resp = t:join(info)
	return resp
end

function CMD.ready(info)
	local t = Env.room:get_table_by_player_id(info.id)
	save_msg_time(info)
	if t == nil then
		local resp = {}
		resp.name = "Table.SysErrorRsp"
		resp.msg = {err_info = "准备错误.房间不存在", err_type = 2}
		return resp
	end
	t:heartbeat(info, msg)
	return t:ready(info)
end

function CMD.quit(info)
	local t = Env.room:get_table_by_player_id(info.id)
	save_msg_time(info)
	if t == nil then
		local resp = {}
		resp.name = "Table.SysErrorRsp"
		resp.msg = {err_info = "退出错误.房间不存在", err_type = 2}
		return resp
	end	
	return t:quit(info)
end

function CMD.play(info, msg)
	local t = Env.room:get_table_by_player_id(info.id)
	save_msg_time(info)
	if t == nil then
		local resp = {}
		resp.name = "Table.SysErrorRsp"
		resp.msg = {err_info = "打牌错误.房间不存在", err_type = 2}
		return resp
	end
	t:heartbeat(info, msg)
	return t:play(info, msg)
end

function CMD.pass(info, msg)
	local t = Env.room:get_table_by_player_id(info.id)
	save_msg_time(info)
	if t == nil then
		local resp = {}
		resp.name = "Table.SysErrorRsp"
		resp.msg = {err_info = "过错误.房间不存在", err_type = 2}
		return resp
	end	
	t:heartbeat(info, msg)
	return t:pass(info, msg)
end

function CMD.peng(info, msg)
	local t = Env.room:get_table_by_player_id(info.id)
	save_msg_time(info)
	if t == nil then
		local resp = {}
		resp.name = "Table.SysErrorRsp"
		resp.msg = {err_info = "碰错误.房间不存在", err_type = 2}
		return resp
	end	
	return t:newpeng(info, msg)
	--return t:peng(info, msg)
end

function CMD.gang(info, msg)
	local t = Env.room:get_table_by_player_id(info.id)
	save_msg_time(info)
	if t == nil then
		local resp = {}
		resp.name = "Table.SysErrorRsp"
		resp.msg = {err_info = "杠错误.房间不存在", err_type = 2}
		return resp
	end	
	return t:newgang(info, msg)
	--return t:gang(info, msg)
end

function CMD.kick(info, msg)
	local t = Env.room:get_table_by_player_id(info.id)
	save_msg_time(info)
	if t == nil then
		local resp = {}
		resp.name = "Table.SysErrorRsp"
		resp.msg = {err_info = "踢人错误.房间不存在", err_type = 2}
		return resp
	end	
	return t:kick(info, msg)
end

function CMD.angang(info, msg)
	local t = Env.room:get_table_by_player_id(info.id)
	save_msg_time(info)
	return t:angang(info, msg)
end

function CMD.passhu(info, msg)
	local t = Env.room:get_table_by_player_id(info.id)	
	save_msg_time(info)
	if t == nil then
		local resp = {}
		resp.name = "Table.SysErrorRsp"
		resp.msg = {err_info = "过胡错误.房间不存在", err_type = 2}
		return resp
	end	
	return t:passhu(info, msg)
end

function CMD.canhu(info, msg)
	local t = Env.room:get_table_by_player_id(info.id)	
	save_msg_time(info)
	if t == nil then
		local resp = {}
		resp.name = "Table.SysErrorRsp"
		resp.msg = {err_info = "canhu错误.房间不存在", err_type = 2}
		return resp
	end	
	return t:canhu(info, msg)
end

function CMD.hu(info, msg)
	local t = Env.room:get_table_by_player_id(info.id)	
	save_msg_time(info)
	if t == nil then
		local resp = {}
		resp.name = "Table.SysErrorRsp"
		resp.msg = {err_info = "胡错误.房间不存在", err_type = 2}
		return resp
	end	
	return t:hu(info, msg)
end

function CMD.ChangeAutoReq(info, msg)
	local t = Env.room:get_table_by_player_id(info.id)	
	save_msg_time(info)
	if t == nil then
		local resp = {}
		resp.name = "Table.SysErrorRsp"
		resp.msg = {err_info = "进入自动错误.房间不存在", err_type = 2}
		return resp
	end	
	return t:ChangeAutoReq(info, msg)
end

function CMD.CancelAutoReq(info, msg)
	local t = Env.room:get_table_by_player_id(info.id)	
	save_msg_time(info)
	if t == nil then
		local resp = {}
		resp.name = "Table.SysErrorRsp"
		resp.msg = {err_info = "取消自动错误.房间不存在", err_type = 2}
		return resp
	end	
	return t:cancelAutoReq(info, msg)
end

function CMD.ReadyNextReq(info, msg)
	local t = Env.room:get_table_by_player_id(info.id)	
	save_msg_time(info)
	if t == nil then
		local resp = {}
		resp.name = "Table.SysErrorRsp"
		resp.msg = {err_info = "继续游戏错误.房间不存在", err_type = 2}
		return resp
	end	
	t:heartbeat(info, msg)
	return t:readyNextReq(info, msg)
end

function CMD.GMDestroyRoomReq(info)
	local t = Env.room:get_table_by_id(info.tab_id)
	print('GMDestroyRoomReq = ', info.tab_id)
	if t ~= nil then
		t:removeTableWithMsg()
	else
		local resp = {}
		resp.name = "Table.SysErrorRsp"
		resp.msg = {err_info = "解散房间失败.房间不存在", err_type = 2}
		return resp		
	end
end

function CMD.GMStopServerReq(info, msg)
	-- body
	local resp = {}
	resp.name = "Table.SysErrorRsp"
	

	stop_server = msg.stop
	if msg.stop == true then
		resp.msg = {err_info = "创建房间功能已经关闭", err_type = 2}
	else
		resp.msg = {err_info = "创建房间功能已经开启", err_type = 2}
	end
	return resp

end

function CMD.heartbeat(info, msg)
	save_msg_time(info)	
	local t = Env.room:get_table_by_player_id(info.id)	
	if t ~= nil then
		t:heartbeat(info, msg)
		return
	end	
	
end

function CMD.quickmsg(info, msg)
	local t = Env.room:get_table_by_player_id(info.id)	
	if t == nil then
		local resp = {}
		resp.name = "Table.SysErrorRsp"
		resp.msg = {err_info = "快捷消息错误.房间不存在", err_type = 2}
		return resp
	end	
	return t:quickmsg(info, msg)	
end

function CMD.voicechat(info, msg)
	local t = Env.room:get_table_by_player_id(info.id)
	if t == nil then
		local resp = {}
		resp.name = "Table.SysErrorRsp"
		resp.msg = {err_info = "语音错误.房间不存在", err_type = 2}
		return resp
	end	
	return t:voicechat(info, msg)	
end

function CMD.VoteQuitReq(info, msg)
	local t = Env.room:get_table_by_player_id(info.id)	
	save_msg_time(info)
	if t == nil then
		local resp = {}
		resp.name = "Table.SysErrorRsp"
		resp.msg = {err_info = "投票错误.房间不存在", err_type = 2}
		return resp
	end	

	return t:VoteQuitReq(info, msg)
end

function CMD.StartVoteReq(info, msg)
	local t = Env.room:get_table_by_player_id(info.id)	
	save_msg_time(info)
	if t == nil then
		local resp = {}
		resp.name = "Table.SysErrorRsp"
		resp.msg = {err_info = "发起投票错误.房间不存在", err_type = 2}
		return resp
	end	

	return t:StartVoteReq(info, msg)
end

function CMD.GMQuerySysInfoReq(info, msg)
	local table_list = Env.room:getTableList()
	local resp = {}
	resp.name = "Table.GMQuerySysInfoRsp"
	resp.msg = {tablelist = table_list}
	return resp
end

--检测玩家是否在房间
function CMD.player_table_id(id)
	local t = Env.room:get_player_table_id(id)
	--print("is_player_in_room = ", t)
	if t == nil then
		return 0
	end
	return t
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
end)
