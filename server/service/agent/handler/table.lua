local skynet = require "skynet"
local env = require "env"
local player = require "player"
local utils = require "utils"
local mjconfig = require "mjconfig"
local log = require "log"
--require "global"
local M = {}

function M.create(msg)
	
	local info = {
		id = player.id,
		token = msg.token,
		name = player.name,
		headurl = player.url,
		fd = player.fd
	}
	--print("STOP_SERVER = ", STOP_SERVER)

	--if STOP_SERVER == true then
	--	env.send_msg("Table.SysErrorRsp",  {err_info = "服务器即将维护停机，暂时无法创建房间", err_type = 0})
	--	return
	--end

	--utils.print(info)
	local playermoney = player.money
	local moneyneed = 10
	if msg.times == 0 then 
		moneyneed = mjconfig.MONEY_ROUND8
	else
		moneyneed = mjconfig.MONEY_ROUND16
	end

	if playermoney < moneyneed then
		--print("create table ,not enough money :", playermoney)
		env.send_msg("Table.SysErrorRsp",  {err_info = "钻石不足,创建失败", err_type = 0})
		return
	end
	--player.money = player.money - moneyneed
	--skynet.call("db", "lua", "updateMoney", {id = player.id, money = player.money})
	--env.send_msg("Table.MoneyChange", {mymoney = player.money})
	--print("create table: ", playermoney, player.money)	
	log.log("create called id = %d", player.id)
	if env.room == nil then
		log.log("create = nil id = %d", player.id)
	end


	local resp = skynet.call(env.room, "lua", "create", info, msg)
	if resp then
		env.send_msg(resp.name, resp.msg)
	end
end

function M.rejoin(msg)
	local info = {
		tab_id = msg.tab_id,
		id = player.id,
		agent = skynet.self(),
		name = player.name,
		headurl = player.url,
		sex = player.sex,
		fd = player.fd
	}
	if env.room == nil then
		log.log("rejoin = nil id = %d", player.id)
	end
	local resp = skynet.call(env.room, "lua", "rejoin", info)
	if resp then
		env.send_msg(resp.name, resp.msg)
	end
end

function M.join(msg)
	local info = {
		tab_id = msg.tab_id,
		id = player.id,
		agent = skynet.self(),
		name = player.name,
		headurl = player.url,
		sex = player.sex,
		fd = player.fd
	}
	log.log("join called id = %d tabid=%d", player.id, msg.tab_id)
	if env.room == nil then
		log.log("join = nil id = %d", player.id)
	end

	local resp = skynet.call(env.room, "lua", "join", info)
	if resp then
		env.send_msg(resp.name, resp.msg)
	end
end

function M.quit(msg)
	local info = {
		tab_id = msg.tab_id,
		id = player.id,
		agent = skynet.self(),
		name = player.name,
		fd = player.fd
	}
	local resp = skynet.call(env.room, "lua", "quit", info)
	if resp then
		env.send_msg(resp.name, resp.msg)
	end
end

--[[function M.match()
	local info = {
		id = player.id,
		agent = skynet.self(),
		name = player.name
	}
	local resp = skynet.call(env.room, "lua","match", info)
	if resp then
		env.send_msg(resp.name, resp.msg)
	end
end]]

function M.ready()
	local info = {
		id = player.id,
		agent = skynet.self(),
		name = player.name,
		fd = player.fd
	}
	--print("table.lua ready called , id = "..player.id)
	local resp = skynet.call(env.room, "lua", "ready", info)
	if resp then
		env.send_msg(resp.name, resp.msg)
	end	
end

function M.play(msg)
	local info = {
		id = player.id,
		agent = skynet.self(),
		name = player.name,
		fd = player.fd
	}

	--utils.print(msg)	
	local resp = skynet.call(env.room, "lua","play", info, msg)
	if resp then
		env.send_msg(resp.name, resp.msg)
	end	
end

function M.peng(msg)
	local info = {
		id = player.id,
		agent = skynet.self(),
		name = player.name,
		fd = player.fd
	}
	local resp = skynet.call(env.room, "lua","peng", info, msg)
	if resp then
		env.send_msg(resp.name, resp.msg)
	end	
end

function M.gang(msg)
	local info = {
		id = player.id,
		agent = skynet.self(),
		name = player.name,
		fd = player.fd
	}
	--print("1111111 M:gang called = ", msg)
	local resp = skynet.call(env.room, "lua","gang", info, msg)

	if resp then
		env.send_msg(resp.name, resp.msg)
	end	
end

function M.angang(msg)
	local info = {
		id = player.id,
		agent = skynet.self(),
		name = player.name,
		fd = player.fd
	}
	local resp = skynet.call(env.room, "lua","angang", info, msg)
	if resp then
		env.send_msg(resp.name, resp.msg)
	end	
end

function M.kick(msg)
	local info = {
		id = player.id,
		agent = skynet.self(),
		name = player.name,
		fd = player.fd
	}

	local resp = skynet.call(env.room, "lua","kick", info, msg)
	if resp then
		env.send_msg(resp.name, resp.msg)
	end		
end

function M.passhu(msg)
	local info = {
		id = player.id,
		agent = skynet.self(),
		name = player.name,
		fd = player.fd
	}

	local resp = skynet.call(env.room, "lua","passhu", info, msg)
	if resp then
		env.send_msg(resp.name, resp.msg)
	end		
end

function M.canhu(msg)
	local info = {
		id = player.id,
		agent = skynet.self(),
		name = player.name,
		fd = player.fd
	}

	--utils.print(msg)	
	local resp = skynet.call(env.room, "lua","canhu", info, msg)

	if resp then
		env.send_msg(resp.name, resp.msg)
	end	
end

function M.hu(msg)
	local info = {
		id = player.id,
		agent = skynet.self(),
		name = player.name,
		fd = player.fd
	}

	--utils.print(msg)	
	local resp = skynet.call(env.room, "lua","hu", info, msg)

	if resp then
		env.send_msg(resp.name, resp.msg)
	end	
end

function M.pass(msg)
	local info = {
		id = player.id,
		agent = skynet.self(),
		name = player.name,
		fd = player.fd
	}

	local resp = skynet.call(env.room, "lua","pass", info, msg)
	if resp then
		env.send_msg(resp.name, resp.msg)
	end	
end

function M.heartbeat(msg)
	local info = {
		id = player.id,
		agent = skynet.self(),
		name = player.name,
		fd = player.fd
	}

	local resp = skynet.call(env.room, "lua","heartbeat", info, msg)
	if resp then
		env.send_msg(resp.name, resp.msg)
	end	
end

function M.voicechat(msg)
	local info = {
		id = player.id,
		agent = skynet.self(),
		name = player.name,
		fd = player.fd
	}
	local resp = skynet.call(env.room, "lua","voicechat", info, msg)
	if resp then
		env.send_msg(resp.name, resp.msg)
	end	
end

function M.quickmsg(msg)
	local info = {
		id = player.id,
		agent = skynet.self(),
		name = player.name,
		fd = player.fd
	}
	local resp = skynet.call(env.room, "lua","quickmsg", info, msg)
	if resp then
		env.send_msg(resp.name, resp.msg)
	end	
end

function M.ChangeAutoReq(msg)
	local info = {
		id = player.id,
		agent = skynet.self(),
		name = player.name,
		fd = player.fd
	}
	local resp = skynet.call(env.room, "lua","ChangeAutoReq", info, msg)
	if resp then
		env.send_msg(resp.name, resp.msg)
	end	
end


function M.CancelAutoReq(msg)
	local info = {
		id = player.id,
		agent = skynet.self(),
		name = player.name,
		fd = player.fd
	}
	local resp = skynet.call(env.room, "lua","CancelAutoReq", info, msg)
	if resp then
		env.send_msg(resp.name, resp.msg)
	end	
end

function M.GMQuerySysInfoReq(msg)
	local info = {
		id = player.id
	}
	local resp = skynet.call(env.room, "lua","GMQuerySysInfoReq", info, msg)
	if resp then
		env.send_msg(resp.name, resp.msg)
	end		
end

function M.StartVoteReq(msg)
	local info = {
		id = player.id
	}
	local resp = skynet.call(env.room, "lua","StartVoteReq", info, msg)
	if resp then
		env.send_msg(resp.name, resp.msg)
	end		
end

function M.VoteQuitReq(msg)
	local info = {
		id = player.id
	}
	local resp = skynet.call(env.room, "lua","VoteQuitReq", info, msg)
	if resp then
		env.send_msg(resp.name, resp.msg)
	end		
end

function M.ReadyNextReq(msg)
	local info = {
		id = player.id,
	}	

	local resp = skynet.call(env.room, "lua","ReadyNextReq", info, msg)
	if resp then
		env.send_msg(resp.name, resp.msg)
	end	
end

function M.HistoryTableReq(msg)
	local table_list_data = skynet.call("db", "lua", "get_history_table_list", {id = player.id, page = msg.page})
	if table_list_data == nil then
		--print("HistoryTableReq res is nil")
		--env.send_msg("Table.SysErrorRsp",  {err_info = "没有记录", err_type = 0})
		return
	end
	--local table_list_data = res --HistoryTable 数组结构

	local data = {id = player.id, page = msg.page, maxpage = table_list_data[2], table_list = table_list_data[1]}
	--print("HistoryTableReq=", data)
	env.send_msg("Table.HistoryTableRsp", data)
end

function M.HistoryTableDetailReq(msg)
	local res = skynet.call("db", "lua", "get_table_score", msg.index)
	if res == nil then
		env.send_msg("Table.SysErrorRsp",  {err_info = "数据不存在", err_type = 2})
		return
	end	
	--print("HistoryTableDetailReq=", #res, msg.id)
	--utils.print(res)
	env.send_msg("Table.HistoryTableDetailRsp", res)
end

function M.GMDestroyRoomReq(msg)
	local info = {
		id = player.id,
		agent = skynet.self(),
		name = player.name,
		tab_id = msg.tabid,
		fd = player.fd
	}
	local resp = skynet.call(env.room, "lua","GMDestroyRoomReq", info)
	if resp then
		env.send_msg(resp.name, resp.msg)
	end		
end


function M.GMStopServerReq(msg)
	--STOP_SERVER = msg.stop
	--print("STOP_SERVER = ", STOP_SERVER)
	--if msg.stop == true then
	--	env.send_msg("Table.SysErrorRsp",  {err_info = "创建房间功能已经关闭", err_type = 0})
	--else
	--	env.send_msg("Table.SysErrorRsp",  {err_info = "创建房间功能已经开启", err_type = 0})
	--end
	local info = {
		id = player.id,
		agent = skynet.self(),
		name = player.name,
		fd = player.fd
	}
	local resp = skynet.call(env.room, "lua","GMStopServerReq", info, msg)
	if resp then
		env.send_msg(resp.name, resp.msg)
	end		
end

function M.GMQuerySinglePlayerReq(msg)
	local res = skynet.call("db", "lua", "gm_query_player", msg)
	if res[1] == nil  then
		env.send_msg("Table.SysErrorRsp",  {err_info = "用户不存在", err_type = 0})
		return
	end
	env.send_msg("Table.GMPlayerInfoRsp", {player_info_list = res})

end


function M.GMQueryAgentListReq(msg)
	local res = skynet.call("db", "lua", "gm_query_agentlist", msg)
	if res[1] == nil  then
		env.send_msg("Table.SysErrorRsp",  {err_info = "没有结果", err_type = 0})
		return
	end	
	env.send_msg("Table.GMPlayerInfoRsp", {player_info_list = res})	
end

function M.GMQueryPlayerListReq(msg)
	local res = skynet.call("db", "lua", "gm_query_player_bymoney", msg)
	if res[1] == nil  then
		env.send_msg("Table.SysErrorRsp",  {err_info = "没有结果", err_type = 0})
		return
	end	
	env.send_msg("Table.GMPlayerInfoRsp", {player_info_list = res})	
end

function M.GMQueryMoneyLogReq(msg)--gm_query_money_log
	local res = skynet.call("db", "lua", "gm_query_money_log", msg)
	if res[1] == nil  then
		env.send_msg("Table.SysErrorRsp",  {err_info = "没有记录", err_type = 0})
		return
	end	
	env.send_msg("Table.GMMoneyLogRsp", {money_log_list = res})	

end

function M.GMQuerySingleGMLogReq(msg)
	local res = skynet.call("db", "lua", "gm_query_money_log_by_index", msg)
	if res[1] == nil  then
		env.send_msg("Table.SysErrorRsp",  {err_info = "没有记录", err_type = 0})
		return
	end	
	env.send_msg("Table.GMMoneyLogRsp", {money_log_list = res})		
end

function M.GMAddMoneyReq(msg)
	local newmsg = {fromid = player.id, toid = msg.id, money = msg.value}
	local res = skynet.call("db", "lua", "gm_add_money", newmsg)
	--0 -成功， 1-非大gm, 2-代理，操作失败，3目标玩家不存在, 4-非Gm,非代理，5非法用户， 6-代理不够钱转出，7代理不能转负数， 8不能自己转给自己
	local error_data = {
							[0] = "加钱成功",
							[1] = "非总GM",
							[2] = "操作失败",
							[3] = "目标不存在",
							[4] = "没有权限",
							[5] = "非法用户",
							[6] = "不够钱转出",
							[7] = "不能为负数",
							[8] = "不能给自己转"
						}

	local err = error_data[res]
	if err == nil then
		err = "非法操作"
	end
	env.send_msg("Table.SysErrorRsp",  {err_info = err, err_type = 0})

end

function M.GMAddAgentReq(msg)
	
	local newmsg = {
		fromid = player.id,
		id = msg.id,
		yes = msg.yes
	}
	local res = skynet.call("db", "lua", "gm_setagent", newmsg)
	if msg.yes == true then
		log.log("gm_setagent id = %d, is_agent = true, res = %d", msg.id, res)
	else
		log.log("gm_setagent id = %d, is_agent = false, res = %d", msg.id, res)
	end

	local error_data = {
							[0] = "成功",
							[1] = "目标不存在",
							[2] = "没有权限",
							[3] = "总gm为nil"
						}

	local err = error_data[res]
	if err == nil then
		err = "非法操作"
	end	
	env.send_msg("Table.SysErrorRsp",  {err_info = err, err_type = 0})
	--if res == 0 then
	--	env.send_msg("Table.SysErrorRsp",  {err_info = "操作成功", err_type = 0})
	--else
	--	env.send_msg("Table.SysErrorRsp",  {err_info = "失败:err = "..res, err_type = 0})
	--end
end

function M.IOS_PurchaseReq(msg)
	if mjconfig.GUEST_MODE == false then
		return
	end

	local newmsg = {playerid = player.id, purchase_id = msg.purchase_id}
	local money = skynet.call("db", "lua", "ios_purchase", newmsg)
	if money > 0 then
		env.send_msg("Table.MoneyChange", {mymoney = money})
		env.send_msg("Table.SysErrorRsp",  {err_info = "充值成功", err_type = 0})	
	end
	
end

function M.IOS_RegistReq(msg)
	if mjconfig.GUEST_MODE == false then
		return
	end
	local newmsg = {username = msg.username, password = msg.password}
	local errno = skynet.call("db", "lua", "ios_regist", newmsg)
	if errno == 0 then
		env.send_msg("Table.SysErrorRsp",  {err_info = "注册成功", err_type = 0})
	else
		env.send_msg("Table.SysErrorRsp",  {err_info = "注册失败，用户已存在", err_type = 0})	
	end	
end


function M.register()
	env.dispatcher:register("Table.CreateReq", M.create)
	env.dispatcher:register("Table.JoinReq", M.join)
	env.dispatcher:register("Table.ReJoinReq", M.rejoin)
	env.dispatcher:register("Table.KickReq", M.kick)
	env.dispatcher:register("Table.ReadyReq", M.ready)
	env.dispatcher:register("Table.QuitReq", M.quit)
	env.dispatcher:register("Table.Play", M.play)
	env.dispatcher:register("Table.Pass", M.pass)
	env.dispatcher:register("Table.Peng", M.peng)
	env.dispatcher:register("Table.Gang", M.gang)
	env.dispatcher:register("Table.Hu", M.hu)
	env.dispatcher:register("Table.CanHu", M.canhu)
	env.dispatcher:register("Table.PassHu", M.passhu)
	env.dispatcher:register("Table.HeartbeatReq", M.heartbeat)
	env.dispatcher:register("Table.QuickMsg", M.quickmsg)
	env.dispatcher:register("Table.VoiceChat", M.voicechat)
	env.dispatcher:register("Table.HistoryTableReq", M.HistoryTableReq)
	env.dispatcher:register("Table.HistoryTableDetailReq", M.HistoryTableDetailReq)
	env.dispatcher:register("Table.CancelAutoReq", M.CancelAutoReq)
	env.dispatcher:register("Table.ReadyNextReq", M.ReadyNextReq)
	env.dispatcher:register("Table.ChangeAutoReq", M.ChangeAutoReq)

	env.dispatcher:register("Table.GMDestroyRoomReq", M.GMDestroyRoomReq)
	env.dispatcher:register("Table.GMStopServerReq", M.GMStopServerReq)
	env.dispatcher:register("Table.GMQuerySinglePlayerReq", M.GMQuerySinglePlayerReq)
	env.dispatcher:register("Table.GMQueryAgentListReq", M.GMQueryAgentListReq)
	env.dispatcher:register("Table.GMQueryPlayerListReq", M.GMQueryPlayerListReq)
	env.dispatcher:register("Table.GMQueryMoneyLogReq", M.GMQueryMoneyLogReq)
	env.dispatcher:register("Table.GMQuerySingleGMLogReq", M.GMQuerySingleGMLogReq)
	env.dispatcher:register("Table.GMAddMoneyReq", M.GMAddMoneyReq)
	env.dispatcher:register("Table.GMAddAgentReq", M.GMAddAgentReq)
	env.dispatcher:register("Table.IOS_PurchaseReq", M.IOS_PurchaseReq)
	env.dispatcher:register("Table.IOS_RegistReq", M.IOS_RegistReq)
	env.dispatcher:register("Table.VoteQuitReq", M.VoteQuitReq)
	env.dispatcher:register("Table.StartVoteReq", M.StartVoteReq)
	env.dispatcher:register("Table.GMQuerySysInfoReq", M.GMQuerySysInfoReq)
end

return M
