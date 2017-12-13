local skynet = require "skynet"
local env = require "env"
local utils = require "utils"
local mjlib = require "mjlib"
local mjconfig = require "mjconfig"
local log = require "log"
local M = {}
local PLAYER_COUNT = mjconfig.PLAYER_COUNT --玩家个数
local AUTO_PLAY_TIMEOUT = mjconfig.AUTO_PLAY_TIMEOUT  --自动打牌超时
local AUTO_PASS_TIMEOUT = 15--mjconfig.AUTO_PASS_TIMEOUT  --自动pass的超时
local HU_TIMEOUT = mjconfig.HU_TIMEOUT --胡了之后倒计时，到达后结算
local PREPARE_TIMEOUT = mjconfig.PREPARE_TIMEOUT --结算后等待进入下一局的时间
local REMAIN_CARD = mjconfig.REMAIN_CARD --剩余卡牌结束
local TIME_OUT_TIMES = mjconfig.TIME_OUT_TIMES -- 超时次数，超时那么多次，则委托自动de
local GANG_SCORE = mjconfig.GANG_SCORE
local ANGANG_SCORE = mjconfig.ANGANG_SCORE
local BUGANG_SCORE = mjconfig.BUGANG_SCORE
local GAME_TIMEOUT =  mjconfig.GAME_TIMEOUT --桌子时间2小时自动解散 
local VOTE_TIMEOUT = 15	--投票解散房间超时 

local TEST_MODE = mjconfig.TEST_MODE --测试模式 
local TEST_MA = mjconfig.TEST_MA --开马测试模式，可强制结束时候开的码
local TEST_GANG = mjconfig.TEST_GANG --测试杠上爆用，强制杠后发固定牌
local TEST_DEAL = mjconfig.TEST_DEAL
local JIANGMA_TEST = {1,2,3,4,5,6,7,8}
local GANG_TEST = 1
local DEAL_TEST = 1
local TEST_DEBUG = mjconfig.TEST_DEBUG
local DONT_AUTO_PLAY = true
--以上内容测试用，如果发布正式版，请回复原始数据, 默认 false
M.__index = M

function M.new(info)
	local o = {
		has_init = false,
		token = info.token,
		is_playing = 0,
		poslist = {},--{[1] = 0, [2] = 0,[3] = 0,[4] = 0}--1永远为房主
		players = {}, --格式 {[id] = {}}
		cards = {},
		owner = info.id, --房主
		id = env.room:get_tab_id(),
		cur_zhuang = 0, --当前庄家id
		cur_step = 0, --当前打出了几张牌，没play一张+1
		cur_round = 0, --第几轮，从东开算，4次循环，比如1，5，9就是1号位作庄，2，6，10，就是2号位做庄，类推
		cur_pos = 0, --当前摸牌者的pos，也是最后的打牌者，非ID，作用在于，要发牌时候调用get_next_pos获得将要发牌的人
		last_playid = 0, --刚出牌的人的id，一个人无法连续出牌，防止小相公
		--cur_host = 0, --当前庄家
		passlist = {}, --已经pass的人id，每次打出一张牌后会清空，等待副职，判断有个人都pass的话就进行下一次发牌，超时判断也用这个,timer时间到后，判断谁没pass则认为其断线，格式{[id] = {}}
		timeoutlist = {},-- 没做pass或出牌行为的timeoutid次数计数器， >=3则标志其委托，格式{[id] = times}，每手动打牌的话，则晴空
		cur_card = 0,--当前最后打出去的牌，碰杠胡根据这张牌
		auto_play_counter = -1000,
		is_auto_play_mode = false,
		auto_pass_counter = -1000,
		is_auto_pass_mode = false,
		hu_counter = 0, --胡牌计数器，有人胡开始计数，10秒后结算
		hu_from = 0, --胡了谁，如果自摸，则对应hu_list的唯一key
		pass_hu_count = {}, --pass_hu计数器，有人胡了
		has_hued = false, --胡了的标志
		round_is_end = false,--本轮结束，有人胡了，不可碰杠，但还可以胡
		score_is_end = false,--结算结束，胡了10秒结算，不可碰杠胡
		hu_list = {},
		setting = {totalround = 8, maxbet = 10, jiangma = 0, maima = 0},
		is_prepare_mode= false,
		prepare_counter = -1000,
		player_record = {},
		game_score = {},
		zhuang_list = {}, --{[1] = id,[2] = id} 每局的庄
		zimo_id = 0, --如果这盘自摸的话，对应的id
		is_haidi = false,  --海底
		is_gangshangbao = false, --杠上爆
		is_gangshanghua = false, --杠上花
		is_qiangganghu = false, --抢杠胡
		last_gang_id = 0, --最后一个杠的人id，当不是自己Play时候，清空，自己play的时候，认为还是他的
		cur_round_start_time = "", --本局开始的时间
		moneyneed = 3, --每局花的钱
		game_time = 0,
		voice_index = 0,
		last_deal_card = 0, --自动出牌用
		delay_peng = {}, --延迟碰
		delay_gang = {},  --延迟杠
		bugang_id = 0, --补杠者id，做延迟发牌用
		ready_next_count = {}, --进入下一局计数
		last_heartbeat_time = {}, --最后指令的时间，用来判断用户是否在线[id] = time
		vote_list = {}, --投票解散
		vote_counter = 0, --投票计算器，15秒自动同意
		is_vote_mode = false,
		can_hu_list = {}

	} --这些默认参数等重开一局时候需要重赋值

	setmetatable(o, M)
	return o
end

function M:init()
	if self.has_init == false then
		for i = 1,PLAYER_COUNT do --赋闲值
			self.poslist[i] = 0
		end
		self.has_init = true
		local now = utils.get_time_str()
		self.game_score = {tableid = self.id, ownerid = owner, starttime = now, endtime = now, playerlist = {}, roundlist = {}, maimalist = {}, jiangmalist = {}}
	end
end

function M:save_game_score()
	local now = utils.get_time_str()
	local playerlist = {}
	for _,record in pairs(self.player_record) do
		local tmp = {} -- 对应playerlist
		local player = self.players[record.id]
		tmp.total_score = record.total_score
		if player ~= nil then
			tmp.name = player.name
			tmp.url = player.headurl
			tmp.pos = player.pos
		else
			tmp.name = "ID"..tostring(record.id)
			tmp.url = ""
			tmp.pos = 0
		end

		tmp.id = record.id
		table.insert(playerlist, tmp)
	end
	self.game_score.playerlist = playerlist
	self.game_score.endtime = now
	return skynet.call("db", "lua", "add_history_score", self.game_score)
end

function M:init_rules(msg)
	self:init()
	if msg.times == 0 then 
		self.setting.totalround = mjconfig.ROUND8
		self.moneyneed = mjconfig.MONEY_ROUND8
	else 
		self.setting.totalround = mjconfig.ROUND16
		self.moneyneed = mjconfig.MONEY_ROUND16
	end
	local maxbet = {[0] = 100, [1] = 2, [2] = 10} --倍率
	self.setting.maxbet =  maxbet[msg.playoffs] or 100 --msg.playoffs
	local jiangma = {[0] = 0, [1] = 2, [2] = 5, [3] = 8} -- 奖码数
	self.setting.jiangma = jiangma[msg.jiangma] or 0
	local maima = {[0] = 0, [1] = 1, [2] = 2} --买马
	self.setting.maima = maima[msg.maima] or 0
	--print("房间设置")
	--utils.print(self.setting)
	--[[print('111111111')

	local test = {1,2,3,4,5,6,6,6,7,8,9,6}
	--utils.print(test)
	for i,v in ipairs(test) do
		print(i,v)
	end
	print("----------")
	mjlib:remove_t_value(test, 2, 6)
	for i,v in ipairs(test) do
		print(i,v)
	end	
	--utils.print(test)
	print("222222222")]]
	-- body
end

function M:broadcast(name, msg)	
	for k, v in pairs(self.players) do
		--if name == "Table.JiangMa" then
		--print(name, " is called", v.agent, k)
		--end
		skynet.send(v.agent, "lua", "send", name, msg)
	end
end

function M:broadcast_others(name, msg, myid)
	for k, v in pairs(self.players) do
		if  myid ~= v.id then --不发给ziji
			skynet.send(v.agent, "lua", "send", name, msg)
		end
	end	
end

function M:readyNextReq(info, msg)
	--self.ready_next_count = self.ready_next_count + 1
	--if self.ready_next_count >= PLAYER_COUNT then
	--	self.prepare_counter = PREPARE_TIMEOUT - 1
	--	return
	--end
	--table.insert(self.ready_next_count, info.id)

	self.ready_next_count[info.id] = 1
	local count = 0
	for k,v in pairs(self.ready_next_count) do
		count = count + 1
	end

	self:broadcast("Table.ReadyNextRsp", {id = info.id})
	if count >= PLAYER_COUNT then
		self.prepare_counter = PREPARE_TIMEOUT - 1
		return
	end	
end


function M:ChangeAutoReq(info, msg)
	if DONT_AUTO_PLAY == true then
		return
	end
	self.timeoutlist[info.id] = TIME_OUT_TIMES + 1
	local player = self.players[info.id]
	if player == nil then
		return
	end		
	local lastplay_id = self.poslist[self.cur_pos] --当前出牌者
	if lastplay_id == nil then
		lastplay_id = 0
	end	
	self:broadcast("Table.SwitchAuto", {id = id, isauto = true, cur_id = lastplay_id})
	player.online = false	
end

function M:cancelAutoReq(info, msg)
	if DONT_AUTO_PLAY == true then
		return
	end
	self.timeoutlist[info.id] = 0
	--self:set_player_automode(id, false)
	local player = self.players[info.id]
	if player == nil then
		return
	end	
	local lastplay_id = self.poslist[self.cur_pos] --当前出牌者
	if lastplay_id == nil then
		lastplay_id = 0
	end	
	self:broadcast("Table.SwitchAuto", {id = info.id, isauto = false, cur_id = lastplay_id})
	player.online = true	
end

--获得当前出牌者超时时间，没委托的10秒，超时不出委托1秒  
function M:get_cur_timeout()
	local id = self.poslist[self.cur_pos]
	local time_out = self.timeoutlist[id]
	--print("----------------------------")
	--utils.print(self.timeoutlist)
	--print("id = ", id)
	--print("----------------------------")
	if time_out == nil or time_out < TIME_OUT_TIMES then
		self:set_player_automode(id, false)
		return AUTO_PLAY_TIMEOUT
	end
	self:set_player_automode(id, true)
	return 2
end

function M:set_player_automode(id, auto)
	local player = self.players[id]
	if player == nil then
		return
	end

	if auto == true then
		if player.online == true then --有发生变化
			local lastplay_id = self.poslist[self.cur_pos] --当前出牌者
			if lastplay_id == nil then
				lastplay_id = 0
			end	
			self:broadcast("Table.SwitchAuto", {id = id, isauto = auto, cur_id = lastplay_id})
			player.online = false
		end		
	else
		if player.online == false then --有发生变化
			local lastplay_id = self.poslist[self.cur_pos] --当前出牌者
			if lastplay_id == nil then
				lastplay_id = 0
			end	
			self:broadcast("Table.SwitchAuto", {id = id, isauto = auto, cur_id = lastplay_id})
			player.online = true
		end			
	end

end

function M:removeTableWithMsg()
	self:broadcast("Table.QuitRsp", {id = 0, quit_type = 3})
	self:removeTable()	
end

function M:on_timer()
	--print("timer called.. tableid="..self.id)

	--有人胡了，倒数10秒结算
	self.game_time = self.game_time + 1
	if self.game_time >= GAME_TIMEOUT then
		self:broadcast("Table.QuitRsp", {id = 0, quit_type = 3})
		log.log("超时解散房间 %d", self.id)
		self:removeTable()
		return
	end

	if self.is_vote_mode == true then
		self.vote_counter = self.vote_counter + 1
		--print("vote_counter = ", self.vote_counter)
		if self.vote_counter >= VOTE_TIMEOUT then
			self:broadcast("Table.QuitRsp", {id = 0, quit_type = 3})
			self:removeTable()
			--local vote_id = ""
			for id,v in pairs(self.vote_list) do
			 	--vote_id = vote_id + tostring(id) + "|"
			 	log.log("VoteTimeout =%d", id)
			end 
			return
		end
	end

	if self.round_is_end == true then
		if self.has_hued == true then
			self.hu_counter = self.hu_counter + 1
			if self.hu_counter >= HU_TIMEOUT then
				self:handle_hu_score()
			end
		end
		return
	end


	if self.is_prepare_mode == true then
		self.prepare_counter = self.prepare_counter + 1 
		--print("is_prepare_mode = true"..self.prepare_counter)
		if self.prepare_counter >= PREPARE_TIMEOUT then
			self:stop_prepare_counter()
			self:start_round()
		end
	end

	self:check_heart_beat()

	if self.is_auto_pass_mode == true then
		self.auto_pass_counter = self.auto_pass_counter + 1
		if self.auto_pass_counter >= AUTO_PASS_TIMEOUT then --self:get_cur_timeout() then
			self:auto_pass()
		end		
		--print("is_auto_pass_mode = true  "..self.auto_pass_counter.."  "..self:get_cur_timeout())
	end	

	if DONT_AUTO_PLAY == true then return end

	if self.is_auto_play_mode == true then
		self.auto_play_counter = self.auto_play_counter + 1
		if self.auto_play_counter >= self:get_cur_timeout() then
			self:auto_play()
		end
		--print("is_auto_play_mode = true  "..self.auto_play_counter.."  "..self:get_cur_timeout())
	end




end



function M:check_heart_beat()
	local HEART_BEAT_TIMEOUT = 15
	local now = skynet.time()
	for id,data in pairs(self.last_heartbeat_time) do			
		local time = data[1]
		local status = data[2]
		if time ~= nil and status ~= nil then
			local delta_time = now - time
			if delta_time > HEART_BEAT_TIMEOUT then
				if status == true then --原来在线的，发生超时后，通知客户端改变状态为不在线
					--
					self:broadcast("Table.ShowOfflineRsp", {id = id, yes = false})
					self.last_heartbeat_time[id] = {time, false}

				end
			else
				if status == false then--原来不在线，发生改变后，通知客户端改变状态为在线
					self:broadcast("Table.ShowOfflineRsp", {id = id, yes = true})
					self.last_heartbeat_time[id] = {time, true}

				end
			end	
		end
	end	
end

function M:get_zhuang_index(round)
	local r = round%PLAYER_COUNT
	if r == 0 then
		r = PLAYER_COUNT
	end
	return r	
end

--获取庄家id
function M:get_zhuang_id(round)
	--local r = self:get_zhuang_index(round)
	--return  self.poslist[r]
	return self.cur_zhuang
end

function M:card_count()
	return #self.cards
end
function M:player_count()
	local count = 0
	for k,v in pairs(self.players) do
		count = count + 1
	end
	return count
end

function M:get_next_pos(npos)--按1234循环下一个pos，将要给她发牌
	if npos >= PLAYER_COUNT then
		return 1
	else 
		return npos + 1
	end
end

function M:find_free_pos()
	local pos = 0
	for i = 1,PLAYER_COUNT do
		if self.poslist[i] == 0 then
			pos = i
			break
		end
	end
	return pos
end


--删除某玩家最后一张牌，碰和明杠
function M:remove_last_outcard(id)
	local  player  = self.players[id]
	if player ~= nil then
		local len = #player.outcards
		if len > 0 then
			table.remove(player.outcards, len)
		end
	end
end

--重置玩家所有卡牌
function M:reset_player_card(id)
	if self.players[id] ~= nil then
		self.players[id]["cards"] = {}    --手里的牌
		self.players[id]["outcards"] = {}  --打出去的牌
		self.players[id]["plist"] = {} --peng和gang的牌，格式：{{type = 1, fromid = 1000, card = 1} } type ==0, peng, 1,gang, 2,angang, 3,bugang from为执行目标，暗杠为自己id， 有目标则是杠的对象，card是哪张牌
	end	
end

function M:get_player_by_id(id)
	return self.players[id]
end


--踢人 
function M:kick(info, msg)

	local resp
	if info.id ~= self.owner then
		log.log("你不是房主不能踢人")
		resp.name = "Table.KickRsp"
		resp.msg = {err_no = 1, id = msg.id}
		return resp		
	end

	if info.id == msg.id then
		log.log("你不能踢自己房主")
		resp.name = "Table.KickRsp"
		resp.msg = {err_no = 2, id = msg.id}
		return resp			
	end

	local playerinfo = self:get_player_by_id(info.id)
	if playerinfo ~= nil then
		self.poslist[playerinfo.pos] = 0 
		self.players[info.id] = nil
	end
	--resp.name = "Table.QuitRsp"
	--resp.msg = {quit_type = 2 ,id = msg.id}
	--return resp	
	self:broadcast("Table.QuitRsp", {id = msg.id, quit_type = 2})
end


--结束，或房主没开始就离开，删除座子
function M:removeTable()
	for id,player in pairs(self.players) do
		env.room:remove_player_id_tab(id, self.id)
	end
	env.room:remove_table(self.id)
	self.players = {}
	self.poslist = {}
end


	--for id,player in pairs(self.players) do
	--	env.room:remove_player_id_tab(id, self.id)
	--end
	--env.room:remove_table(self.id)
function M:quit(info)
	if self.is_playing ~= 0 then -- 牌局
		return
	end
	
	--print("Tab.quit = ", info.id, self.owner)
	local resp = {}
	local playerinfo = self:get_player_by_id(info.id)


	if info.id == self.owner then --解散
		self:broadcast("Table.QuitRsp", {id = info.id, quit_type = 3})
		--self:broadcast("Table.Ready", {id = info.id})
		log.log("解散房间 user=%d tableid=%d", info.id, self.id)

		self:removeTable()
	else
		log.log("离开房间 user=%d", info.id)
		self:broadcast("Table.QuitRsp", {id = info.id, quit_type = 1})
		if playerinfo ~= nil then
			self.poslist[playerinfo.pos] = 0 
			self.players[info.id] = nil
			env.room:remove_player_id_tab(info.id, self.id)
		end		
	end
	--resp.name = "Table.QuitRsp"
	--resp.msg = {id = info.id}
	--return resp
end

function M:rejoin(info)
	local rs = {}
	local end_cards = {}
	local resp = {}
	local player = self.players[info.id]
	if player == nil then --不在桌子上，非法重进入
		resp.name = "Table.ReJoinRsp"
		resp.msg = {err_no = 1, owner = 0, tab_id = 0, is_playing = 0, roles = {}, end_cards = {}, rule = {}}
		return resp
	end
	log.log("rejoin:%d ", info.id)
	self:heartbeat(info)
	--self.players[info.id] = info
	self.players[info.id].agent = info.agent
	self.players[info.id].fd = info.fd

	for k, v in pairs(self.players) do
		table.insert(rs, {id = v.id, name = v.name, pos = v.pos, ready = v.ready, online = v.online, url = v.headurl, sex = v.sex})
	end	

	local player_record = {}
	for id,record in pairs(self.player_record) do
		table.insert(player_record, record)
	end	
	local lastplay_id = self.poslist[self.cur_pos] --当前出牌者
	
	if lastplay_id == nil then
		log.log("rejoin last_playid == nil =%d", info.id)
		lastplay_id = 0
	end
	
	end_cards = self:get_all_player_cards()
	local cur_card = self.cur_card
	if lastplay_id == info.id then --如果轮到自己打牌，cur_card就是最后摸的牌
		log.log("rejoin lastplay_id == info.id =%d", info.id)
		cur_card = self.last_deal_card
	end
	local cardnum = #self.cards
	log.log("rejoin lastplay_id=%d cur_card = %d tabid = %d left_card=%d", lastplay_id, cur_card, self.id, cardnum)

	resp.name = "Table.ReJoinRsp"
	resp.msg = {err_no = 0, owner = self.owner, tab_id = self.id, is_playing = self.is_playing, roles = rs, end_cards = end_cards, rule = self.setting, player_record = player_record, cur_id = lastplay_id, cur_round = self.cur_round, cur_card = cur_card, left_card = cardnum}
	self:cancelAutoReq(info)
	self:set_player_automode(info.id, false)

	return resp	
end

function M:join(info)
	local rs = {}
	local resp = {}
	local rule = {}
	--self:removeTable() --test
	if self:player_count() >= PLAYER_COUNT then
		--print("人满")
		resp.name = "Table.JoinRsp"
		resp.msg = {err_no = 2, roles = rs, owner = 0, rule = {}}
		return resp
	end

	local player = self.players[info.id]
	if player ~= nil then
		--print("已经在桌上")
		resp.name = "Table.JoinRsp"
		resp.msg = {err_no = 3, roles = rs, owner = 0, rule = {}}
		return resp		
	end
	--self:init()
	local freepos = self:find_free_pos()
	if freepos == 0 then
		--print("没有适合座位了")
		resp.name = "Table.JoinRsp"
		resp.msg = {err_no = 4, roles = rs, owner = 0,rule = {}}
		return resp
	end
	--print("freepos = "..freepos)
	--print("加入座子 = No."..self.id.." Player id = "..info.id.."  Pos ="..freepos)
	log.log("join called id = %d tabid=%d Pos = %d fd = %d", info.id, self.id, freepos, info.fd)
	info.pos = freepos
	self.players[info.id] = info
	self.poslist[freepos] = info.id
	self:broadcast_others("Table.Join", {id = info.tab_id, role = {id = info.id, name = info.name, pos = info.pos, ready = false, online = true, url = info.headurl, sex = info.sex}}, info.id)
	--self:broadcast_others("Table.Join", {id = info.tab_id, role = {id = info.id, name = "test_"..info.id, pos = info.pos, ready = false, url = info.headurl}}, info.id)
	
	for k, v in pairs(self.players) do
		table.insert(rs, {id = v.id, name = v.name, pos = v.pos, ready = v.ready, online = v.online, url = v.headurl, sex = v.sex})
		log.log("join:%d , url = %s, sex=%d, tabid=%d", v.id, v.headurl, v.sex, self.id)
		--table.insert(rs, {id = v.id, name = "123", pos = v.pos, ready = v.ready, url = v.headurl})
	end

	resp.name = "Table.JoinRsp"
	resp.msg = {err_no = 0, roles = rs, owner = self.owner, rule = self.setting}
	skynet.send(info.agent, "lua", "send", resp.name, resp.msg)
	self:readyold(info)
	--return resp
	--return resp--融合ready一起

end


function M:init_records()
	for id,player in pairs(self.players) do
		self.player_record[id] = {id = id, zm_count = 0, dp_count = 0, ag_count = 0, hp_count = 0, mg_count = 0, mm_count = 0, total_score = 0}
	end
end

--开局扣钱
function M:cost_money(id, money)
	local player = self.players[id]
	if player == nil then
		return
	end

	skynet.send(player.agent, "lua", "cost_money", money)
end

function M:ready(info)
	--print("ready called = ", info.id)
end

function M:readyold(info)
	if self.is_playing ~= 0 then -- 牌局
		return
	end

	if self.players[info.id] ~= nil then
		self.players[info.id].ready = true
		self.players[info.id].online = true --是否在线
		self:reset_player_card(info.id)
	end

	local ready_num = 0

	for k, v in pairs(self.players) do 
		if v.ready == true then
			ready_num = ready_num + 1
		end
	end
	self:broadcast("Table.Ready", {id = info.id})
	--print("准备：id="..info.id.." count = "..ready_num.."  房主 = "..self.owner.."  桌号 = "..self.id)
	log.log("ready：id=%d count = %d  房主 = %d  桌号 = %d", info.id, ready_num, self.owner, self.id)
	--print("is all ready ? :", ready_num, ready_num == PLAYER_COUNT)
	if ready_num == PLAYER_COUNT then
		self.is_playing = 1
		self.cur_zhuang = self.owner --所有人准备好，房主起装
		self:cost_money(self.owner, self.moneyneed)
		self:init_records()
		self:start_round()
	end
	
end



--Table.Start, status = 1代表开始 ，=2，代表有人和牌结束，=3代表刘局
function M:end_round(status)
	local player_record = {}
	for id,record in pairs(self.player_record) do
		table.insert(player_record, record)
	end
	self:broadcast("Table.Start", {status = status, id = self:get_zhuang_id(self.cur_round), round = self.cur_round, player_record = player_record})
end

function M:reset_round()
	--self.round_is_end = false
	--self.score_is_end = false
	--self.cur_round = 0, --第几轮，从东开算，4次循环，比如1，5，9就是1号位作庄，2，6，10，就是2号位做庄，类推
	self.cur_step = 0;
	self.cur_pos = 0 --当前摸牌者的pos，也是最后的打牌者，非ID，作用在于，要发牌时候调用get_next_pos获得将要发牌的人
		--cur_host = 0, --当前庄家
	self.passlist = {} --已经pass的人id，每次打出一张牌后会清空，等待副职，判断有个人都pass的话就进行下一次发牌，超时判断也用这个,timer时间到后，判断谁没pass则认为其断线，格式{[id] = {}}
	self.timeoutlist = {}-- 没做pass或出牌行为的timeoutid次数计数器， >=3则标志其委托，格式{[id] = times}，每手动打牌的话，则晴空
	self.cur_card = 0--当前最后打出去的牌，碰杠胡根据这张牌
	self.auto_play_counter = -1000
	self.is_auto_play_mode = false
	self.auto_pass_counter = -1000
	self.is_auto_pass_mode = false
	self.hu_counter = 0--胡牌计数器，有人胡开始计数，10秒后结算
	self.hu_from = 0 --胡了谁，如果自摸，则对应hu_list的唯一key
	self.has_hued = false--胡了的标志
	self.round_is_end = false--本轮结束，有人胡了，不可碰杠，但还可以胡
	self.score_is_end = false--结算结束，胡了10秒结算，不可碰杠胡
	self.hu_list = {}
	self.is_preparing = false
	self.prepare_counter = -1000
	self.pass_hu_count = {}
	self.zimo_id = 0
	self.is_haidi = false
	self.is_gangshangbao = false
	self.is_gangshanghua = false
	self.is_qiangganghu = false
	self.last_gang_id = 0 
	self.last_playid = 0
	self.last_deal_card = 0
	self.ready_next_count = {}
	self.delay_peng = {}
	self.delay_gang = {}
	self.bugang_id = 0
	self.is_vote_mode = false
	self.vote_counter = 0
	self.can_hu_list = {}
end

--开始一局
function M:start_round()
	self.is_playing = 1
	self:reset_round()
	self.cur_round = self.cur_round + 1
	self:end_round(1)
	self.cur_round_start_time = utils.get_time_str()
	for id,v in pairs(self.players) do ---重设所有玩家的牌
		self:reset_player_card(id)	
	end	
	self.cards = self:create(true)
		-- 洗牌
	self:shuffle(self.cards)
		-- 发牌
	if TEST_MODE == true then
		self:deal_test()
	else
		self:deal()
	end
	log.log("Startround="..self.cur_round.." tabid= "..self.id)
	self.zhuang_list[self.cur_round] = self:get_zhuang_id(self.cur_round)
	--for id,v in pairs(self.players) do
	--	self:get_hu_dir(id)
	--end
	
	--self:broadcast("Table.Start", {status = 1, id = self:get_zhuang_id(self.cur_round), round = self.cur_round})
	--self:broadcast("Table.Turn", {id = self.turn}, 0)
end


-- 创建一幅牌,牌里存的不是牌本身，而是牌的序号
function M:create(zi)
    local t = {}
    local num = 3*9
    if zi then
        num = num + 7
    end
    for i=1,num do
        for _=1,4 do
            table.insert(t, i)
        end
    end
    return t
end

-- 洗牌
function M:shuffle(t)
    for i=#t,2,-1 do
        local tmp = t[i]
        local index = math.random(1, i - 1)
        t[i] = t[index]
        t[index] = tmp
    end
end

function M:get_pos_by_id(id)
	local player = self.players[id]
	if player == nil then
		log.log("get_pos_by_id player not found = %d", id)
		return 0
	end

	return player.pos
end

function M:pass(info, msg)
	local function make_string()

		if TEST_DEBUG == false then
			return 
		end
		local ret = "=C"..tostring(self.cur_pos)
		for id,v in pairs(self.passlist) do
			if v == true then
				local pos = self:get_pos_by_id(id)
				ret = ret.."=D"..tostring(pos)
			end
		end
		self:broadcast("Table.PassCountRsp", {value = ret})
		return
	end
	log.log("Pass：= %d tab_id = %d step = %d passcard=%s", info.id, self.id, self.cur_step, self:get_card_str(msg.passcard))

	if self.round_is_end == true or self.score_is_end == true then
		--print("Try Pass本局已结束")		
		return
	end
	if self.cur_card == 0 then
		--print("deal card pass error")
		log.log("deal card pass error")
		return
	end

	if msg.passcard ~= -1 then
		if msg.passcard ~= self.cur_card then
			log.log("passcard card pass error, passcard = "..self:get_card_str(msg.passcard).." cur_card = "..self:get_card_str(self.cur_card))
			return
		end
	end

	local lastplay_id = self.poslist[self.cur_pos] --刚摸牌的人的id
	if lastplay_id == info.id then --这种情况一般不会发生，自动pass会发生
		--print("自动发过 = "..lastplay_id)
		log.log("AutoPass = %d", lastplay_id)
		return
	end
	--print("guo:"..info.id)
	if self.passlist[info.id] ~= true then	
		local name = "Table.Pass"
		local msg = {id = info.id, auto = msg.auto}
		local player = self.players[info.id]
		if player ~= nil then
			skynet.send(player.agent, "lua", "send", name, msg)
		end
	end
	self.passlist[info.id] = true
	--print("$#########################")
	--utils.print(self.passlist)
	--print("$#########################")
	make_string()
	
	local pass_count = 0
	for k,v in pairs(self.passlist) do
		pass_count = pass_count + 1
	end
	--print("pass_count = ", pass_count, info.id)
	if pass_count == PLAYER_COUNT - 2 then
		local peng_info = self.delay_peng[1]
		local peng_msg = self.delay_peng[2]
		if peng_info ~= nil and peng_msg ~= nil then 
			self:peng(peng_info, peng_msg)
			return
		end
	end

	if pass_count == PLAYER_COUNT - 2 then
		local gang_info = self.delay_gang[1]
		local gang_msg = self.delay_gang[2]
		if gang_info ~= nil or gang_msg ~= nil then 
			self:gang(gang_info, gang_msg)
			return			
		end
	end

	log.log("PassOK：= %d tab_id = %d step = %d passcard=%d", info.id, self.id, self.cur_step, msg.passcard)
	if pass_count >= PLAYER_COUNT - 1 then
		local nextpos = self:get_next_pos(self.cur_pos)
		local id = self.poslist[nextpos]
		if self.bugang_id > 0 then
			id = self.bugang_id
		end
		self:deal_one(id)
	end	

end

function M:newgang(info, msg)
	local gang_from = self.poslist[self.cur_pos]
	if gang_from ~= info.id then --杠别人的
		local pass_count = 0
		for id,v in pairs(self.passlist) do
			pass_count = pass_count + 1
		end
		
		if pass_count >= (PLAYER_COUNT - 2) then
			return self:gang(info, msg)
		end

		self.delay_gang = {info, msg}
		log.log("dealy gang = %d, last = %d", msg.card, self.cur_card)
	else
		self:gang(info, msg)
	end
end


--尚未对已碰列表进行杠
function M:gang(info, msg)

	local function broadcast_new(player, gang_from)
		local myCard = {}
		local heCard = {}	
		for i,card in ipairs(player.cards) do
			table.insert(myCard, card)
			table.insert(heCard, 0)
		end
		--self:print_cards(myCard)
		skynet.send(player.agent, "lua", "send", "Table.Gang", {id = info.id, from = gang_from, card = msg.card, err_no = 0, pgdata = player.plist, leftcard = myCard})
		for playerid, v in pairs(self.players) do
			if  playerid ~= info.id then --不发给ziji
				local name = "Table.Gang"
				local msg = {id = info.id, from = gang_from, card = msg.card, err_no = 0, pgdata = player.plist, leftcard = heCard}
				skynet.send(v.agent, "lua", "send", name, msg)
			end
		end	
		log.log("Gang tabid = %d, playerid = %d, card = %s, from = %d", self.id, info.id, self:get_card_str(msg.card), gang_from )	
	end

	if self.round_is_end == true or self.score_is_end == true then
		--print("Try Gang本局已经结束")
		resp.name = "Table.Gang"
		resp.msg = {id = info.id, from = 0, card = msg.card, err_no = 5, pgdata = {}, leftcard = {}}		
		return resp	
	end

	local resp = {}
	local gang_from = self.poslist[self.cur_pos]
	log.log("gang："..self:get_card_str(msg.card).." "..self:get_card_str(self.cur_card))
	local player = self.players[info.id]
	if player == nil then
		log.log("gang牌 非法用户")
		resp.name = "Table.Gang"
		resp.msg = {id = info.id, from = gang_from, card = msg.card, err_no = 1, pgdata = {}, leftcard = {}}
		return resp
	end

	local can_gang = false
	if gang_from ~= info.id then --杠别人的
		if msg.card ~= self.cur_card then
			log.log("gang和打出来的不一致")
			resp.name = "Table.Gang"
			resp.msg = {id = info.id, from = gang_from, card = msg.card, err_no = 2, pgdata = {}, leftcard = {}}
			return resp
		end
		can_gang = mjlib:can_gang(player.cards, msg.card)
		if can_gang == false then
			log.log("用户手中没有三张牌可gang的")
			resp.name = "Table.Gang"
			resp.msg = {id = info.id, from = gang_from, card = msg.card, errb_no = 3, pgdata = {}, leftcard = {}}
			return resp
		end

		--print("杠前：")
		--self:print_cards(player.cards)

		mjlib:remove_t_value(player.cards, 3, msg.card)
		--print("杠后：")
		--self:print_cards(player.cards)		

		self:remove_last_outcard(gang_from)
		local pdata = {ptype = 1, from = gang_from, card = msg.card}
		table.insert(player.plist, pdata)		
		--self:broadcast("Table.Gang", {id = info.id, from = gang_from, card = msg.card, err_no = 0, pgdata = player.plist, leftcard = {}})
		broadcast_new(player, gang_from)
		self.cur_card = 0--被碰杠的话，变为0，目的是处理抢胡的情况，就是说我碰杠了，你胡不了
		self:deal_one(info.id, true)
		self.last_gang_id = info.id
		log.log("tagang："..self:get_card_str(msg.card).." id="..info.id)
		return
	else
		--暗杠
		--("aaaaaa gang = ", player.cards, "  ssss ", msg.card)
		local can_angang = mjlib:can_angang(player.cards, msg.card)
		if can_angang == true then
			local num = 0

			--print("暗杠前：")
			--self:print_cards(player.cards)

			mjlib:remove_t_value(player.cards, 4, msg.card)
			--print("暗杠后：")
			--self:print_cards(player.cards)			
			local pdata = {ptype = 2, from = info.id, card = msg.card}
			table.insert(player.plist, pdata)			
			--self:broadcast("Table.Gang", {id = info.id, from = info.id, card = msg.card, err_no = 0, pgdata = player.plist, leftcard = {}})
			broadcast_new(player, info.id)
			self:deal_one(info.id, true)
			self.last_gang_id = info.id
			log.log("angang："..self:get_card_str(msg.card).." id="..info.id)
			return
		else
			local can_bugang = false
			for _,pdata in ipairs(player.plist) do --检测已碰的牌是否可以补杠
				if pdata.ptype == 0 and pdata.card == msg.card then
					pdata.ptype = 3
					can_bugang = true
					break
				end	
			end
			if can_bugang == true then
				mjlib:remove_t_value(player.cards, 1, msg.card)
				--self:broadcast("Table.Gang", {id = info.id, from = info.id, card = msg.card, err_no = 0, pgdata = player.plist, leftcard = {}})
				broadcast_new(player, 0)
				log.log("bugang："..self:get_card_str(msg.card).." id="..info.id)
				self.last_gang_id = info.id
				self:delay_bugang(info, msg)
				--self:deal_one(info.id, true)
				return
			end
		end
		--杠已碰的
		log.log("err gong tabid=%d", self.id) --一般不会发生
		resp.name = "Table.Gang"
		resp.msg = {id = info.id, from = gang_from, card = msg.card, err_no = 4, pgdata = {}, leftcard = {}}
		return resp		
	end
end

function M:delay_bugang(info, msg)
	self.bugang_id = info.id
	self.cur_card = msg.card
	self:stop_auto_play_counter()
	self:start_auto_pass_counter()
	self:try_auto_pass(info.id)

end

function M:passhu(info, msg)
	self.pass_hu_count[info.id] = true
	local count = 0
	for _,v in pairs(self.hu_list) do
		count = count + 1
	end

	--self.pass_hu_count = self.pass_hu_count + 1
	
	for _,v in pairs(self.pass_hu_count) do
		count = count + 1
	end
	if count == PLAYER_COUNT - 1 then
		self.hu_counter = HU_TIMEOUT - 2
	end
	--print("PassHu called = "..info.id)
	--print("1111111111111111111111  cur_round= ", self.cur_round)
	--utils.print(self.pass_hu_count)
	--print("1111111111111111111111")		
	--if self.pass_hu_count ==  2 then
		--self.hu_counter = HU_TIMEOUT - 2
	--end
end


--检测海底捞
function M:check_haidilao(id)
	if #self.cards <= REMAIN_CARD then
		--print("海底捞月...")
		self.is_haidi = true
	else
		self.is_haidi = false
	end
end

--检测杠上爆
function M:check_gangshangbao(id)
	if self.last_gang_id == id then
		--print("杠上爆")
		self.is_gangshangbao = true
	else
		self.is_gangshangbao = false
	end
end

function M:check_gangshanghua(hu_from)
	if self.last_gang_id == hu_from then
		--print("杠上花")
		self.is_gangshanghua = true
	else
		self.is_gangshanghua = false
	end
end

function M:check_qiangganghu(hu_from)
	self.is_qiangganghu = false
	if self.bugang_id > 0 then
		if self.bugang_id == hu_from then
			self.is_qiangganghu = true
		end
	end
end

function M:canhu(info, msg)
	log.log("canhu called = %d" , info.id)
	--self:stop_auto_pass_counter()
	self.can_hu_list[info.id] = true
end

function M:hu(info, msg)
	if self.score_is_end == true then
		--print("TryHu本局已经结束")
		resp.name = "Table.Hu"
		resp.msg = {id = info.id, from = 0, hutype = 0, cards = {}, card = msg.card, err_no = 6, exttype = 0}	
		return resp	
	end

	local resp = {}
	local hu_from = self.poslist[self.cur_pos]
	--log.log("hupai："..self:get_card_str(msg.card).." "..self:get_card_str(self.cur_card))

	local player = self.players[info.id]
	if player == nil then
		log.log("hu:err player")
		resp.name = "Table.Hu"
		resp.msg = {id = info.id, from = info.id, hutype = 0, cards = {}, card = msg.card, err_no = 1, exttype = 0}
		return resp
	end	
	log.log("hu called = %d tabid = %d", info.id, self.id)
	if hu_from == info.id then --自摸
		local can_hu = mjlib:can_hu(player.cards, msg.cards)
		--print("自摸")
		--print("----------server cardlist--------------")
		--utils.print(player.cards)
		--print("-----------client cardlist--------------")
		--utils.print(msg.cards)
		--print("-------------------------")
		if can_hu == true then
			self:add_huer(info.id, msg.hutype, info.id)
			self:check_gangshangbao(info.id)
			self:check_haidilao(info.id)
			self:broadcast("Table.Hu", {id = info.id, from = info.id, hutype = msg.hutype, cards = player.cards, card = msg.card, err_no = 0, exttype = self:get_round_score_exttype()})
			return			
		end
		log.log("hu: err zimo")
		resp.name = "Table.Hu"
		resp.msg = {id = info.id, from = info.id, hutype = 0, cards = {}, card = msg.card, err_no = 2, exttype = 0}
		return resp		
	else
		if self.cur_card == 0 then
			log.log("hu: target no exist 目标牌不存在，被碰了或杠了")
			resp.name = "Table.Hu"
			resp.msg = {id = info.id, from = info.id, hutype = 0, cards = {}, card = msg.card, err_no = 5, exttype = 0}
			return resp			
		end

		if msg.card ~= self.cur_card then
			log.log("hu not match card = %d  cur_card = %d", msg.card, self.cur_card)
			resp.name = "Table.Hu"
			resp.msg = {id = info.id, from = info.id, hutype = 0, cards = {}, card = msg.card, err_no = 3, exttype = 0}
			return resp
		end		
		local can_hu
		local cards1 = {}
		local cards2 = {}
		for _,v in ipairs(player.cards) do
			table.insert(cards1, v)
		end
		table.insert(cards1, self.cur_card)

		for _,v in ipairs(msg.cards) do
			table.insert(cards2, v)
		end
		table.insert(cards2, msg.card)
		--print("胡牌："..hu_from.."  "..msg.hutype)
		--print("----------server cardlist--------------")
		--utils.print(cards1)
		--self:print_cards(cards1)
		--print("-----------client cardlist--------------")
		--utils.print(cards2)
		--self:print_cards(cards2)
		--print("-------------------------")		
		can_hu = mjlib:can_hu(cards1, cards2)
		if can_hu == true then
			self:add_huer(info.id, msg.hutype, hu_from)
			self:check_gangshanghua(hu_from)
			self:check_qiangganghu(hu_from)
			self:broadcast("Table.Hu", {id = info.id, from = hu_from, hutype = msg.hutype, cards = player.cards, card = msg.card, err_no = 0, exttype = self:get_round_score_exttype()})
			return
		end
		log.log("hu error hu")
		resp.name = "Table.Hu"
		resp.msg = {id = info.id, from = hu_from, hutype = 0, cards = {}, card = msg.card, err_no = 4, exttype = 0}
		return resp		
	end

end

function M:handle_delaypeng()
	local info = self.delay_peng[1]
	local msg = self.delay_peng[2]
	if info == nil or msg == nil then 
		return false
	end
	self:peng(info, msg)
end




function M:newpeng(info, msg)
	local pass_count = 0
	for id,v in pairs(self.passlist) do
		pass_count = pass_count + 1
	end
	
	if pass_count >= (PLAYER_COUNT - 2) then
		return self:peng(info, msg)
	end

	self.delay_peng = {info, msg}
end

function M:peng(info, msg)
	if self.round_is_end == true or self.score_is_end == true then
		--print("本局已经结束")
		resp.name = "Table.Peng"
		resp.msg = {id = info.id, from = 0, card = msg.card, err_no = 5, pgdata = {}, leftcard = {}, nextcard = 0}	
		return resp	
	end

	local resp = {}
	local peng_from = self.poslist[self.cur_pos]
	--print("碰牌："..self:get_card_str(msg.card).." "..self:get_card_str(self.cur_card))
	if msg.card ~= self.cur_card then
		log.log("碰的牌和打出来的不一致 peng = %d, cur_card = %d", msg.card, self.cur_card)
		resp.name = "Table.Peng"
		resp.msg = {id = info.id, from = peng_from, card = msg.card, err_no = 1, pgdata = {}, leftcard = {}, nextcard = 0}
		return resp
	end

	local player = self.players[info.id]
	if player == nil then
		log.log("碰牌 非法用户")
		resp.name = "Table.Peng"
		resp.msg = {id = info.id, from = peng_from, card = msg.card, err_no = 2, pgdata = {}, leftcard = {}, nextcard = 0}
		return resp
	end

	if info.id == peng_from then
		log.log("不能自己碰自己")
		resp.name = "Table.Peng"
		resp.msg = {id = info.id, from = peng_from, card = msg.card, err_no = 4, pgdata = {}, leftcard = {}, nextcard = 0}		
		return resp
	end

	local can_peng = mjlib:can_peng(player.cards, self.cur_card)
	if can_peng == false then
		log.log("用户手中没有两张用户可碰的")
		resp.name = "Table.Peng"
		resp.msg = {id = info.id, from = peng_from, card = msg.card, err_no = 3, pgdata = {}, leftcard = {}, nextcard = 0}
		return resp
	end

	--print("碰前：")
	--self:print_cards(player.cards)
	mjlib:remove_t_value(player.cards, 2, self.cur_card)
	--print("碰后：")
	--self:print_cards(player.cards)
	self.cur_pos = player.pos
	self.cur_card = 0--被碰杠的话，变为0，目的是处理抢胡的情况，就是说我碰杠了，你胡不了

	self:remove_last_outcard(peng_from)
	local pdata = {ptype = 0, from = peng_from, card = msg.card}
	table.insert(player.plist, pdata)

--[[
	for i,card in ipairs(player.cards) do
		table.insert(myCard, card)
		table.insert(heCard, 0)
	end
	skynet.send(player.agent, "lua", "send", "Table.Play", {id = id, card = card, leftcard = myCard, outcards = player.outcards, pgdata = player.plist, err_no = 0})
	--self:broadcast_others("Table.Play", {id = id, card = card, leftcard = heCard, err_no = 0}, id)	


	for playerid, v in pairs(self.players) do
		if  playerid ~= id then --不发给ziji
			local name = "Table.Play"
			local msg = {id = id, card = card, leftcard = heCard, outcards = player.outcards, pgdata = player.plist, err_no = 0}
			skynet.send(v.agent, "lua", "send", name, msg)
		end
	end	]]
	local nextcard = player.cards[1]
	--self.last_deal_card = nextcard--自动打牌用的，现在取消， 因为取消自动打牌了，不需要记录最后一张牌

	local myCard = {}
	local heCard = {}	
	for i,card in ipairs(player.cards) do
		table.insert(myCard, card)
		table.insert(heCard, 0)
	end
	--self:print_cards(myCard)

	skynet.send(player.agent, "lua", "send", "Table.Peng", {id = info.id, from = peng_from, card = msg.card, err_no = 0, pgdata = player.plist, leftcard = myCard, nextcard = nextcard})
	for playerid, v in pairs(self.players) do
		if  playerid ~= info.id then --不发给ziji
			local name = "Table.Peng"
			local msg = {id = info.id, from = peng_from, card = msg.card, err_no = 0, pgdata = player.plist, leftcard = heCard, nextcard = nextcard}
			skynet.send(v.agent, "lua", "send", name, msg)
		end
	end
	log.log("Peng: tabid = %d, playerid = %d, card = %s, nextcard = %s", self.id, info.id, self:get_card_str(msg.card), self:get_card_str(nextcard))
	--self:broadcast("Table.Peng", {id = info.id, from = peng_from, card = msg.card, err_no = 0, pgdata = player.plist}) --碰了谁的牌，客户端可以处理删掉最后一个出牌的那张牌

	self:start_auto_play_counter()
	self:stop_auto_pass_counter()
	self.passlist = {}
end

function M:add_timeout_player(id)
	if self.timeoutlist[id] == nil then
		self.timeoutlist[id] = 0
	end
	self.timeoutlist[id] = self.timeoutlist[id] + 1	
end


--10秒强行自动pass，和出牌一起超过三次后，在play协议那里自动pass掉
function M:auto_pass()
	local function can_peng_gang(player) --能碰杠的不pass
		if player.cards == nil then
			log.log("Do auto_pass player card is nil tab_id = %d", self.id)
			return false
		end
		local can_peng = mjlib:can_peng(player.cards, self.cur_card)
		return can_peng
	end
	local function can_hu(id)
		local can_hu = self.can_hu_list[id]
		if can_hu == true then
			log.log("Do can_hu = %d step = %d tab_id = %d", id, self.cur_step, self.id)
			return true
		end
		return false
	end

	local lastplay_id = self.poslist[self.cur_pos] --刚打牌的人的id
	--print("auto_pass lastplay_id = "..lastplay_id)
	local copy_pass_list = {} --copy instance防止引用问题
	for _id,_v in pairs(self.passlist) do
		copy_pass_list[_id] = _v
	end

	for id,player in pairs(self.players) do

		--local has_passed = self.passlist[id]
		local has_passed = copy_pass_list[id]

		if has_passed == true then --已经pass的不处理
		else
			if id ~= lastplay_id then --刚打牌的人不参与pass
				--print("超时自动pass..id = "..id)
				if can_peng_gang(player) == false then
					if can_hu(id) == false then
						local info = {}
						local msg = {auto = false, passcard=-1}
						info.id = id
						self:pass(info, msg)
						self:add_timeout_player(id)
						log.log("Do auto_pass = %d step = %d tab_id = %d", id, self.cur_step, self.id)
					end
				end
			end
			--self:add_timeout_player(id)			
		end
	end
	
end

--收到play协议，尝试对timeout_player玩家进行自动pass
function M:try_auto_pass(ignore_id)
	--print("Try pass处理：")
	--utils.print(self.timeoutlist)
	for id,count in pairs(self.timeoutlist) do
		--print(k,v)
		if ignore_id ~= id then
			if count >= TIME_OUT_TIMES then
				local info = {}
				local msg = {auto = false,passcard=-1}	
				info.id = id	
				--print("委托自动Pass:"..info.id)	
				self:pass(info, msg)
				
			end
		end
	end
end


function M:stop_prepare_counter()
	self.is_prepare_mode = false
	self.prepare_counter = -1000
end

function M:start_prepare_counter()
	if self.cur_round >= self.setting.totalround then --判定所有局打完,发送所有结算
		local player_record = {}
		for id,record in pairs(self.player_record) do
			table.insert(player_record, record)
		end
		local index = self:save_game_score()
		--print("EndScore = ")
		--utils.print(player_record)
		self:broadcast("Table.GameEnd", {id = self.id, player_records = player_record, index = index})
		self:removeTable()
		return
	end
	self.is_prepare_mode = true
	self.prepare_counter = 0
	self.is_playing = 2
end

function M:start_auto_pass_counter()
	self.auto_pass_counter = 0
	self.is_auto_pass_mode = true
end

function M:stop_auto_pass_counter()
	self.auto_pass_counter = -1000
	self.is_auto_pass_mode = false
end

--出完牌，碰完之后，开始自动倒计时
function M:start_auto_play_counter()
	self.auto_play_counter = 0
	self.is_auto_play_mode = true	
end

function M:stop_auto_play_counter()
	self.is_auto_play_mode = false
	self.auto_play_counter = -1000
end

--[[
	self.cur_card = msg.card

	self:send_play_result(info.id, msg.card)
	self.last_playid = info.id
	self:stop_auto_play_counter()
	self:start_auto_pass_counter()
	self.timeoutlist[info.id] = 0 ---有手动打牌，则标志非超时状态
	self:try_auto_pass(id)
	self.cur_step = self.cur_step + 1
	if info.id ~= self.last_gang_id then
		self.last_gang_id = 0
	end]]
--[[
	for i,card in ipairs(player.cards) do
		if card == msg.card then
			card_found = true
			table.remove(player.cards, i)
			table.insert(player.outcards, card)--如果后面被碰要删除掉，客户端也要处理
			break
		end
	end]]

--超时，自动出牌
function M:auto_play()
	local id = self.poslist[self.cur_pos]
	local player = self.players[id]  --当前出牌者
	
	--self.cur_card
	local card_found = false
	local card = self.last_deal_card
	for i,mycard in ipairs(player.cards) do
		if mycard == card then
			card_found = true
			table.remove(player.cards, i)
			--table.insert(player.outcards, card)--如果后面被碰要删除掉，客户端也要处理
			break
		end
	end

	if card_found == false then
		card = table.remove(player.cards, 1) --打第一张
	end

	table.insert(player.outcards, card)
	self.cur_card = card
	self:send_play_result(id, card)
	--self:broadcast("Table.Play", {id = id, card = card, leftcard = #player.cards, err_no = 0})--客户端处理，其他人只显示在面前打出一张牌，ID是自己显示并删除对应card， yangxr
	self.last_playid = id
	self:stop_auto_play_counter()
	self:start_auto_pass_counter()
	
	self:add_timeout_player(id)
	self:try_auto_pass(id)
	self.cur_step = self.cur_step + 1
	local str = "超时自动出牌："..id.."  "..self:get_card_str(card).."  leftcard = "..#self.cards
	log.log(str)
	--if self.timeoutlist[id] >= TIME_OUT_TIMES then
	--end
	--[[for i,card in ipairs(player.cards) do
		if card == self.cur_card then
			card_found = true
			table.remove(player.cards, i)
			table.insert(player.outcards, card)
			break
		end
	end]]

	--if card_found == false then
	--	print("系统错误，自动打了一张不存在的牌")
	--	return
	--end
end

function M:send_play_result(id, card)

	local myCard = {}
	local heCard = {}
	local player = self.players[id]

	if player == nil then
		return
	end

	for i,card in ipairs(player.cards) do
		table.insert(myCard, card)
		table.insert(heCard, 0)
	end
	skynet.send(player.agent, "lua", "send", "Table.Play", {id = id, card = card, leftcard = myCard, outcards = player.outcards, pgdata = player.plist, err_no = 0})
	--self:broadcast_others("Table.Play", {id = id, card = card, leftcard = heCard, err_no = 0}, id)	


	for playerid, v in pairs(self.players) do
		if  playerid ~= id then --不发给ziji
			local name = "Table.Play"
			local msg = {id = id, card = card, leftcard = heCard, outcards = player.outcards, pgdata = player.plist, err_no = 0}
			skynet.send(v.agent, "lua", "send", name, msg)
		end
	end		

end


function M:play(info, msg)
	local resp = {}
	local card_found = false
	local player = self.players[info.id]
	--print("出牌："..self:get_card_str(msg.card).."    "..info.id)
	if player == nil then
		log.log("play card player is nil")
		resp.name = "Table.Play"
		resp.msg = {id = info.id, card = msg.card, leftcard = {}, err_no = 1, outcards = {}, pgdata = {}}
		return resp
	end

	if info.id == self.last_playid then
		log.log("连续打牌非法操作 %d", info.id)
		resp.name = "Table.Play"
		resp.msg = {id = info.id, card = msg.card, leftcard = {}, err_no = 6, outcards = {}, pgdata = {}}			
		return
	end

	if player.pos ~= self.cur_pos then
		log.log("本回合没轮到你出牌,当前出牌者是 %d %d", player.pos, self.cur_pos)
		resp.name = "Table.Play"
		resp.msg = {id = info.id, card = msg.card, leftcard = {}, err_no = 3, outcards = {}, pgdata = {}}	
		return
	end
	--print("mycardlist----------------")
	--utils.print(player.cards)
	--self:print_cards(player.cards)
	--print("--------------------------")

	for i,card in ipairs(player.cards) do
		if card == msg.card then
			card_found = true
			table.remove(player.cards, i)
			table.insert(player.outcards, card)--如果后面被碰要删除掉，客户端也要处理
			break
		end
	end

	if card_found == false then --非法打牌
		log.log("非法打牌，打了一张你没有的牌")
		resp.name = "Table.Play"
		resp.msg = {id = info.id, card = msg.card, leftcard = {}, err_no = 2,  outcards = {}, pgdata = {}}		
		return
	end

	self.cur_card = msg.card

	self:send_play_result(info.id, msg.card)
	--[[local myCard = {}
	local heCard = {}
	for i,card in ipairs(player.cards) do
		table.insert(myCard, card)
		table.insert(heCard, 0)
	end
	skynet.send(player.agent, "lua", "send", "Table.Play", {id = info.id, card = msg.card, leftcard = myCard, err_no = 0})
	self:broadcast_others(("Table.Play", {id = info.id, card = msg.card, leftcard = heCard, err_no = 0})
	--self:broadcast("Table.Play", {id = info.id, card = msg.card, leftcard = #player.cards, err_no = 0})--客户端处理，其他人只显示在面前打出一张牌，ID是自己显示并删除对应card， yangxr
	]]
	local cardnum = #self.cards
	log.log("playcard card = %s, id = %d, cur_round = %d, leftcard = %d tabid = %d", self:get_card_str(msg.card), info.id, self.cur_round, cardnum, self.id)
	self.last_playid = info.id
	self:stop_auto_play_counter()
	self:start_auto_pass_counter()
	self.timeoutlist[info.id] = 0 ---有手动打牌，则标志非超时状态
	self:try_auto_pass(id)
	self.cur_step = self.cur_step + 1
	if info.id ~= self.last_gang_id then
		self.last_gang_id = 0
	end
	self.can_hu_list[info.id] = false
end

-- 发牌
function M:deal()

		-- 先发13张牌
	log.log("deal playercount = %d tabid=%d is_playing=%d", self:player_count(), self.id, self.is_playing)
	for i = 1, 13 do
		for k, v in pairs(self.players) do 
			local card = table.remove(self.cards, 1)
			table.insert(v.cards, card)
		end
	end
	for k, v in pairs(self.players) do
		skynet.send(v.agent, "lua", "send", "Table.Cards", {cards = v.cards})
	end

	self:deal_one(self:get_zhuang_id(self.cur_round))--给庄家发多一张
end


--有人胡了加进来,hutype胡的类型，from胡的对象，from = id 代表自摸
function M:add_huer(id, hutype, from)
	--hutype = 15 -- for test
	
	if hutype == 1  or hutype == 2 then --客户端非法传参，不可能天胡或地胡
		hutype = 18
	end

	self.hu_from = from
	if self.has_hued == false then--第一个人胡了
		self:end_round(2)
		self.hu_counter = 0
		if id == from then --自摸，立刻显示
			self.hu_counter = HU_TIMEOUT - 2
		end
	end
	self.has_hued = true
	self.round_is_end = true


	--天地胡判断
	if self.cur_step == 0 then 
		if id == from then--天胡，自摸
			hutype = 1
		end
	end

	if hutype > 4 then --大于4的分数比地胡少才有判断地胡的需求
		--print("检测地胡 cur_step = "..self.cur_step.."  from = "..from.."  id = "..id)
		if self.cur_step == 1 then --庄家，打了第一张牌，有人胡了他，则为地胡
			if from == self:get_zhuang_id(self.cur_round)  and id ~= from then
				hutype = 2
			end
		else
			if self.cur_step < 4 then --第一轮
				if id ~= self:get_zhuang_id(self.cur_round) and from == id then --自摸者不是庄，则为地胡
					hutype = 2
				end
			end
		end
	end

	local str = "胡了: id = "..id.."  from = "..from.."  "..mjlib:get_hu_name(hutype).." tabid =  "..self.id
	log.log(str)

	self.hu_list[id] = {hutype , from}

	--参考passhu()
	local count = 0
	for _,v in pairs(self.hu_list) do
		count = count + 1
	end
	
	for _,v in pairs(self.pass_hu_count) do
		count = count + 1
	end

	if count == PLAYER_COUNT - 1 then
		self.hu_counter = HU_TIMEOUT - 2
	end	

end


function M:get_hu_dir_name(dir)
	local dir_str = {"东", "南", "西", "北"}
	return dir_str[dir]
end


--根据id所在本轮的位置，比如 A是庄家， B处于南风 1,2,3,4，不是桌面的pos
function M:get_hu_dir(hu_id)
	--[[local zhuang_pos = self:get_zhuang_index(self.cur_round) -- pos1，2，3，4谁是庄
	--local zhuang_pos = 
	local hu_player = self.players[hu_id]
	if hu_player == nil then
  		return
  	end
  	local hu_pos = hu_player.pos

  	local tmap = {
  		[1] = {[1] = 1, [2] = 2, [3] = 3, [4] = 4},
  		[2] = {[2] = 1, [3] = 2, [4] = 3, [1] = 4},
		[3] = {[3] = 1, [4] = 2, [1] = 3, [2] = 4},
		[4] = {[4] = 1, [1] = 2, [2] = 3, [3] = 4},
  	}

  	local hu_dir = tmap[zhuang_pos][hu_pos] --胡的风位
  	local dir_str = {"东", "南", "西", "北"}
  	print(hu_id.."  风位："..dir_str[hu_dir])

  	return hu_dir]]

 	local hu_player = self.players[hu_id]
	if hu_player == nil then
  		return
  	end
  	
  	return hu_player.pos 	

end

function M:handle_lianzhuang(player_score)
	local function get_lianzhuang_count(cur_zhuang_id) --判断连庄数
		--print("fffffffffff")
		--utils.print(self.zhuang_list)
		--print('fffffffffff')
		local count = 0
		
		for i = self.cur_round,1,-1 do
			if cur_zhuang_id == self.zhuang_list[i] then
				count = count + 1
			else
				break
			end
		end
		--print('count = ', count)
		return count
	end

	if mjconfig.CAN_LIANZHUANG == false then
		return "0,0"
	end

	local cur_zhuang_id = self:get_zhuang_id(self.cur_round)
	local count = get_lianzhuang_count(cur_zhuang_id)
	if count > 1 then
		for id,score in pairs(player_score) do
			if id == cur_zhuang_id then
				player_score[id] = player_score[id] + (PLAYER_COUNT-1) * (count - 1)
			else
				player_score[id] = player_score[id] - (count - 1)
			end
		end	

		return tostring(cur_zhuang_id)..","..tostring(count - 1)
	end
	return "0,0"
end


--			local pdata = {ptype = 2, from = info.id, card = msg.card}
--			table.insert(player.plist, pdata)
--1,gang, 2,angang, 3,bugang
function M:handle_gang(player_score)
	local tmpScore = {}
	for k,v in pairs(player_score) do
		tmpScore[k] = 0
	end
	for id,player in pairs(self.players) do
		local plist = player.plist
		for _,pdata in ipairs(plist) do
			local ptype = pdata.ptype
			local from = pdata.from
			if ptype == 1 then --杠
				--player_score[id] = player_score[id] + GANG_SCORE
				--player_score[from] = player_score[from] - GANG_SCORE	
				tmpScore[id] = tmpScore[id] + GANG_SCORE
				tmpScore[from] = tmpScore[from] - GANG_SCORE		
			elseif ptype == 2 then
				for pid,score in pairs(tmpScore) do
					if pid == id then --自摸者
						--player_score[pid] = player_score[pid] + 3 * ANGANG_SCORE
						tmpScore[pid] = tmpScore[pid] + (PLAYER_COUNT-1) * ANGANG_SCORE
					else
						--player_score[pid] = player_score[pid] - ANGANG_SCORE
						tmpScore[pid] = tmpScore[pid] - ANGANG_SCORE
					end	
				end		
			elseif ptype == 3 then
				for pid,score in pairs(tmpScore) do
					if pid == id then --自摸者
						--player_score[pid] = player_score[pid] + 3 * BUGANG_SCORE
						tmpScore[pid] = tmpScore[pid] + (PLAYER_COUNT-1) * BUGANG_SCORE
					else
						--player_score[pid] = player_score[pid] - BUGANG_SCORE
						tmpScore[pid] = tmpScore[pid] - BUGANG_SCORE
					end
				end
			end
		end
	end

	for k,v in pairs(player_score) do
		player_score[k] = player_score[k] + tmpScore[k]
	end

	return tmpScore
end

function M:get_round_score_exttype()
	local ret = 0
	if self.is_qiangganghu == true then
		return 4
	end	
	if self.is_haidi == true then
		return 1
	end

	if self.is_gangshanghua == true then
		return 3
	end

	if self.is_gangshangbao == true then
		return 2
	end


	return 0
end


function M:get_hu_score(hu_type)
	local score = mjlib:get_hu_score(hu_type)
	if score > self.setting.maxbet then
		score = self.setting.maxbet
	end
	if self.is_gangshangbao == true or self.is_gangshanghua == true or self.is_haidi == true or self.is_qiangganghu == true then
		score = score * 2
	end

	return score
end

function M:handle_maima(count, hu_list, player_score)
	local function print_ma_score(t, str)
		 --{id = id, card = card1, targetid = hu_id, score = score})
		--print("得分",str, " ID = ", t.id, "Card = ", self:get_card_str(t.card)," 作用者=", t.targetid, " 分数=", t.score)
	end
	---{[id] = {card1 = {0,0}, card2 = {0, 0}}}
	local function add_mazhong(maima_zhong, id, cardno, score)
		if cardno == 1 then
			local old_score = maima_zhong[id]["card1"][2]
			maima_zhong[id]["card1"] = {cardno, old_score + score}
		end
		if cardno == 2 then
			local old_score = maima_zhong[id]["card2"][2]
			maima_zhong[id]["card2"] = {cardno, old_score + score}
		end
	end

	--local malist = {}
	local malist = {} --统计中马个数，[id] = count
	local tmpScore = {}
	for k,v in pairs(player_score) do
		tmpScore[k] = 0
		malist[k] = 0
	end	
	local cardlist = {} --格式 [id] = {card1, card2} 
	local all_cardlist = {}
	local max_count = 8
	if max_count > #self.cards then
		max_count = #self.cards
	end
	for i = 1,max_count do
		table.insert(all_cardlist, self.cards[i])		
	end
	--count = count * PLAYER_COUNT
	if count ~= 2 then count = 1 end
	for i = 1,PLAYER_COUNT do
		local c = {}
		local id = self.poslist[i]
		for j = 1, count do
			local card = table.remove(self.cards, 1)			
			table.insert(c, card)
		end
		cardlist[id] = c
	end

	if TEST_MA == true then
		all_cardlist = {}
		cardlist = {}
		for i = 1,max_count do
			table.insert(all_cardlist, JIANGMA_TEST[i])
		end
		
		if count ~= 2 then count = 1 end
		local loop = 1
		for i = 1,PLAYER_COUNT do
			local c = {}
			local id = self.poslist[i]
			for j = 1, count do
				--local card = table.remove(self.cards, 1)
				local card = JIANGMA_TEST[loop]
				loop = loop + 1
				table.insert(c, card)
			end
			cardlist[id] = c
		end		
	end

	local maima_list = {}
	local maima_zhong = {}---{[id] = {card1 = {0,0}, card2 = {0, 0}}}
	for id,ma in pairs(cardlist) do
		local card1 = ma[1]
		local card2 = ma[2] 
		table.insert(maima_list, {id = id, card1 = card1, card2 = card2})
		maima_zhong[id] = {card1 = {card1, 0}, card2 = {card2, 0}}
	end

	
	--print("买马底牌：")
	--self:print_cards(all_cardlist)
	--print("买马对应表")
	--for k,v in pairs(cardlist) do	
	--	print("ID = ", k)
	--	self:print_cards(v)
	--end
	local maima_detail = {{}, {}} --买马中的详情,[id] = {target, score}

	for id,ma in pairs(cardlist) do
		for i = 1,count do
			local card1 = ma[i]
		--end
		--local card1 = ma[1]
		--local card2 = ma[2]
			for hu_id,hu_data in pairs(hu_list) do
				
				local score = self:get_hu_score(hu_data[1])
				local ma_dir = mjlib:get_maima_dir(card1) --该码的风位
				if ma_dir == 0 then
					print("handle_maima = 0")
				end
				local ma_id = self.poslist[ma_dir] -- 该码对应的player_id
				--print("信息id = ", id, "card = ", self:get_card_str(card1), " 该马对应者 = ", ma_id, " 风位= ", self:get_hu_dir_name(ma_dir))
				local from = hu_data[2]
				if hu_id == from then --自摸
					if ma_id == hu_id then --买码中了自摸的。如果买中了自摸者，自己多赢其他两个人钱，
						for _,posid in pairs(self.poslist) do 
							if posid == id or posid == hu_id then--自己和自摸家不处理
								print("不做")
							else
								tmpScore[id] = tmpScore[id] + score
								tmpScore[posid] = tmpScore[posid] - score
								table.insert(maima_detail[i], {id = id, card = card1, targetid = posid, score = score})
								print_ma_score({id = id, card = card1, targetid = posid, score = score}, "1")
								add_mazhong(maima_zhong, id, card1, score)
								malist[id] = malist[id] + 1
							end
						end
					else -- 如果买中了被自摸的，自己多扣一次分，给自摸者
						tmpScore[id] = tmpScore[id] - score
						tmpScore[hu_id] = tmpScore[hu_id] + score
						table.insert(maima_detail[i], {id = id, card = card1, targetid = hu_id, score = -score})
						print_ma_score({id = id, card = card1, targetid = hu_id, score = -score}, "2")
						add_mazhong(maima_zhong, id, card1, -score)
						malist[id] = malist[id] + 1
					end
				else
					if ma_id == hu_id then --我的码和庄家一致,庄家的分也给一份我，被胡者也扣
						tmpScore[id] = tmpScore[id] + score
						tmpScore[from] = tmpScore[from] - score
						table.insert(maima_detail[i], {id = id, card = card1, targetid = from, score = score})
						print_ma_score({id = id, card = card1, targetid = from, score = score}, "3")
						add_mazhong(maima_zhong, id, card1, score)
						malist[id] = malist[id] + 1
					end

					if ma_id == from then --我的码和被胡的一致，我的分扣给胡者
						tmpScore[id] = tmpScore[id] - score
						tmpScore[hu_id] = tmpScore[hu_id] + score
						table.insert(maima_detail[i], {id = id, card = card1, targetid = hu_id, score = -score})
						print_ma_score({id = id, card = card1, targetid = hu_id, score = -score}, "4")
						add_mazhong(maima_zhong, id, card1, -score)
						malist[id] = malist[id] + 1
					end
				end
			end
		end		
	end
	
	local maima_data = {maima_list = maima_list, cardsAll = all_cardlist, maima_score1 = maima_detail[1], maima_score2 = maima_detail[2]}
	self:broadcast("Table.MaiMa", maima_data)
	--table.insert(self.game_score.maimalist, {maima_list = maima_list, round_id = self.cur_round})

	--------------------买马history--------------------------------
	local maima_data_new = {}
	for id,v in pairs(maima_zhong) do
		local tmpdata = {id = id, card1 = 0, card2 =0, zhong1 = 0, zhong2 = 0}
		tmpdata.card1 = v.card1[1]
		tmpdata.card2 = v.card2[1]
		tmpdata.zhong1 = v.card1[2]
		tmpdata.zhong2 = v.card2[2]
		table.insert(maima_data_new, tmpdata)
		---{[id] = {card1 = {0,0}, card2 = {0, 0}}}
	end

	table.insert(self.game_score.maimalist, {maima_list = maima_data_new, round_id = self.cur_round})
	------------------------------------------------------


	--print("----------买马获得数begin----------")
	--utils.print(tmpScore)
	--print("----------买马获得分数end-----------")	

	for k,v in pairs(player_score) do
		player_score[k] = player_score[k] + tmpScore[k]
	end	
	return malist
end


function M:handle_noma()
	local all_cardlist = {}
	local count = 8
	if count > #self.cards then
		count = #self.cards
	end
	for i = 1,count do
		table.insert(all_cardlist, self.cards[i])		
	end
	--print("不设买马，底牌为：")
	--self:print_cards(all_cardlist)
	--print('--------------------------')
	--utils.print(all_cardlist)
	self:broadcast("Table.JiangMa", {cards = {}, cardsAll = all_cardlist})

end

function M:handle_jiangma(count, hu_list, player_score)
	local malist = {} --奖码返回倍数，格式{[id] = {倍率, malist}
	local hu_count = 0
	for id,hu_data in pairs(hu_list) do
		hu_count = hu_count + 1
	end


	
	local cardlist = {}
	local all_cardlist = {}
	--local count = self.setting.jiangma
	local max_count = 8
	if max_count > #self.cards then
		max_count = #self.cards
	end
	if count > #self.cards then
		count = #self.cards
	end
	for i = 1,max_count do --默认开8个，只取count个
		local card = table.remove(self.cards, 1)
		table.insert(all_cardlist, card)
		if i <= count then
			table.insert(cardlist, card)
		end
	end

	if TEST_MA == true then --测马
		cardlist = {}
		all_cardlist = {}
		for i = 1,max_count do --默认开8个，只取count个
			local card = JIANGMA_TEST[i]
			table.insert(all_cardlist, card)
			if i <= count then
				table.insert(cardlist, card)
			end
		end		
	end

	if hu_count ~= 1 then
		table.insert(self.game_score.jiangmalist, {jiangma_list = malist, round_id = self.cur_round})
		self:broadcast("Table.JiangMa", {cards = {}, cardsAll = all_cardlist})
		return malist
	end	

	--print("开码个数: "..count)
	--print("底牌：")
	--self:print_cards(all_cardlist)
	--[[local res = "所有码："
	for _,card in ipairs(all_cardlist) do
		res =  res + "  " + self:get_card_str(card)
	end
	print(res)]]
	--print("奖码:")
	--self:print_cards(cardlist)
	--[[res = "奖码："
	for _,card in ipairs(cardlist) do
		res =  res + "  " + self:get_card_str(card)
	end

	print(res)]]

	--self:broadcast("Table.JiangMa", {cards = cardlist, cardsAll = all_cardlist})

	local tmpScore = {}
	for k,v in pairs(player_score) do
		tmpScore[k] = 0
	end

	local on_card_get  = {}
	for id,hu_data in pairs(hu_list) do
		local score = self:get_hu_score(hu_data[1])
		local hu_dir = self:get_hu_dir(id)
		local from = hu_data[2]
		local power, card_get = mjlib:get_jiangma_count(hu_dir, cardlist)
		--if power > self.setting.maxbet then
		--	power = self.setting.maxbet
		--end
		--self:print_cards(cardlist)
		--print("中码者：ID = ", id, " 风位：" ,self:get_hu_dir_name(hu_dir), " 倍率=", power)

		--self:print_cards(card_get)
		--print("---------------------")
		log.log("jiangma ID = %d tab_id = %d feng=%s power=%d round_id=%d", id, self.id , self:get_hu_dir_name(hu_dir), power, self.cur_round)
		if from == id then --自摸 
			for pid,v in pairs(tmpScore) do
				if pid == id then
					tmpScore[pid] = tmpScore[pid] + (PLAYER_COUNT-1) * score * power
				else
					tmpScore[pid] = tmpScore[pid] - score * power
				end
			end
		else
			tmpScore[id] = tmpScore[id] + score * power
			tmpScore[from] = tmpScore[from] - score * power
		end
		malist[id] = {power, card_get}
		on_card_get = card_get
	end

	for k,v in pairs(player_score) do
		player_score[k] = tmpScore[k]
	end
	--print("----------奖码后分数begin----------")
	--utils.print(tmpScore)
	--print("----------奖码后分数end-----------")	

	-----------------奖码history---------------------------
	--table.insert(self.game_score.jiangmalist, {jiangma_list = cardlist, round_id = self.cur_round}) --old
	table.insert(self.game_score.jiangmalist, {jiangma_list = on_card_get, round_id = self.cur_round})
	self:broadcast("Table.JiangMa", {cards = on_card_get, cardsAll = all_cardlist})
	--print("11111111111111111")
	--utils.print(on_card_get)
	--print("222222222222222222")
	-------------------------------------------------------
	return malist
	
end


--self.hu_list[id] = {hutype , from},根据胡牌的人算各自的分
function M:handle_base_score(hu_list, player_score)
	for id,hu_data in pairs(hu_list) do
		--local hu_dir = self:get_hu_dir()
		local score = self:get_hu_score(hu_data[1])
		local from = hu_data[2]
		if from == id then --自摸 
			for pid,v in pairs(player_score) do
				if pid == id then
					player_score[pid] = player_score[pid] + (PLAYER_COUNT-1) * score
				else
					player_score[pid] = player_score[pid] - score
				end
			end
		else
			player_score[id] = player_score[id] + score
			player_score[from] = player_score[from] - score
		end
	end


end



--获取所有玩家牌结构，对应repeated EndCards end_cards = 2;一般，断线从连，游戏结算时候用
function M:get_all_player_cards()
	local end_cards = {}

	for id,player in pairs(self.players) do
		local player_cards = {} --对应proto的"end"
		local cards = {}
		local outcards = {}
		local player = self.players[id]
		--for i,card in ipairs(player.cards) do
		--	table.insert(cards, card)
		--end
		local plist = player.plist
		local plisttmp = {}
		for k,v in pairs(player.plist) do
			table.insert(plisttmp, v)
		end
		player_cards.cards = player.cards --cards
		player_cards.id = id
		player_cards.pgdata = plisttmp
		player_cards.outcards = player.outcards
		table.insert(end_cards, player_cards)		
	end

	return end_cards
end

--有人胡之后10秒后处理,海药检测杠分
function M:handle_hu_score()

	local player_score = {}
	local base_score = {}
	local jiangmalist = {} --[id] = {倍率，{列表}}
	local maimalist = {} --[id] = 个数

	for i,id in ipairs(self.poslist) do
		player_score[id] = 0
		base_score[id] = 0
	end

	self.has_hued = false
	self.score_is_end = true --
	self:handle_base_score(self.hu_list, player_score)
	for id,score in pairs(player_score) do
		base_score[id] = score
	end
	--print("---------基础胡分 begin------------")
	--utils.print(base_score)
	--print("---------基础胡分 end------------")

	if self.setting.jiangma > 0 then
		jiangmalist = self:handle_jiangma(self.setting.jiangma, self.hu_list, player_score)
	else
		if self.setting.maima > 0 then
			maimalist = self:handle_maima(self.setting.maima, self.hu_list, player_score)
		else
			self:handle_noma()
		end
	end

	local gang_score = self:handle_gang(player_score)
	local lianzhuang_id = self:handle_lianzhuang(player_score)
	--print("连庄检测："..lianzhuang_id)
	local round_score = {}

	local end_cards = {}
	for id,score in pairs(player_score) do
		local score_data = {id = id, hu_type = 0, hu = 0, score = score, basescore = base_score[id], jiangma = {}} -- 对应proto里面的Score
		local hu_er = self.hu_list[id]		
		if jiangmalist[id] ~= nil  then --中了几个码
			score_data.jiangma = jiangmalist[id][2]
		end
		
		if hu_er == nil then 
			if id == self.hu_from  then --响炮者
				score_data.hu = 2
				--self.cur_zhuang = id
			end
		else
			if id == self.hu_from then --自摸
				self.cur_zhuang = id
				score_data.hu_type = hu_er[1]
				score_data.hu = 3
			else -- 普通和牌
				score_data.hu_type = hu_er[1]
				score_data.hu = 1
			end
		end
		table.insert(round_score, score_data)

		-----记录总数据 ---------
		local player_record = self.player_record[id]
		if player_record ~= nil then
			player_record.total_score = player_record.total_score + score
			player_record.mm_count = player_record.mm_count + #score_data.jiangma --奖码个数中了
			if maimalist[id] ~= nil then
				player_record.mm_count = player_record.mm_count + maimalist[id] --买马个数
			end
			if score_data.hu == 2 then
				player_record.dp_count = player_record.dp_count + 1
			elseif score_data.hu == 1 then
				player_record.hp_count = player_record.hp_count + 1
			elseif score_data.hu == 3 then
				player_record.zm_count = player_record.zm_count + 1
			end

			local player = self.players[id]
			if player ~= nil then
				for k,pdata in pairs(player.plist) do
					if pdata.ptype == 1 or pdata.ptype == 3 then
						player_record.mg_count = player_record.mg_count + 1
					end

					if pdata.ptype == 2 then
						player_record.ag_count = player_record.ag_count + 1
					end
				end
			end
		end

	end

	end_cards = self:get_all_player_cards()
	local round_score_data = {scores = round_score, end_cards = end_cards, end_type = 1, round_id = self.cur_round, starttime = self.cur_round_start_time, lianzhuang = lianzhuang_id, exttype = self:get_round_score_exttype()}
	table.insert(self.game_score.roundlist, round_score_data)
	self:broadcast("Table.RoundScore", round_score_data)
	
	--print("-----------胡牌结算:begin------------")
	--utils.print(round_score_data)
	--print("----------------------------------")
	--utils.print(end_cards)
	--print("-----------胡牌结算:end------------")
	self:change_zhuang()
	self:reset_round()
 	self:start_prepare_counter()
end


function M:change_zhuang()
	local xiangpao_count = 0
	local next_zhuang = self.cur_zhuang

	for  id,hudata in pairs(self.hu_list) do
		local from = hudata[2]
		next_zhuang = id
		if from == self.hu_from then
			xiangpao_count = xiangpao_count + 1
		end
	end
	if xiangpao_count >= 2 then
		next_zhuang = self.hu_from
	end
	self.cur_zhuang = next_zhuang
	--return next_zhuang
end

--处理流局
function M:handle_fail_round()
	log.log("流局 tabid = %d", self.id)
	local player_score = {}
	local base_score = {}
	for i,id in ipairs(self.poslist) do
		player_score[id] = 0
		base_score[id]= 0
	end	
	self.has_hued = false
	self.round_is_end = true
	self.score_is_end = true 
	self:end_round(3)
	local gang_score = self:handle_gang(player_score)
	--self:handle_gang(player_score) --没人杠分是否考虑不发
	local lianzhuang_id = self:handle_lianzhuang(player_score)
	--self:handle_noma()

	local round_score = {}
	for id,score in pairs(player_score) do
		--local score_data = {id = id, hu_type = 0, hu = 0, score = score} -- 对应proto里面的Score
		local score_data = {id = id, hu_type = 0, hu = 0, score = score, basescore = 0, jiangma = {}}
		table.insert(round_score, score_data)
		local player_record = self.player_record[id]--加杠分，修复刘局杠没算分
		if player_record ~= nil then
			player_record.total_score = player_record.total_score + score
		end		
	end	
	local end_cards = self:get_all_player_cards()
	local round_score_data = {scores = round_score, end_cards = end_cards, end_type = 2, round_id = self.cur_round, starttime = self.cur_round_start_time, lianzhuang = lianzhuang_id, exttype = 0}
	table.insert(self.game_score.roundlist, round_score_data)
	--table.insert(self.game_score.jiangmalist, {jiangma_list = {}, round_id = self.cur_round}) --填坑
	--table.insert(self.game_score.maimalist, {maima_list = {}, round_id = self.cur_round})
	if self.setting.jiangma > 0 then --填坑
		table.insert(self.game_score.jiangmalist, {jiangma_list = {}, round_id = self.cur_round})
	else
		if self.setting.maima > 0 then
			table.insert(self.game_score.maimalist, {maima_list = {}, round_id = self.cur_round})
		end
	end	
	self:broadcast("Table.RoundScore", round_score_data)

	--print("-----------流局结算:begin------------")
	--utils.print(round_score)
	--print("-----------流局结算:end------------")
	self:reset_round()
	self:start_prepare_counter()
	--self:reset_round()考虑过一段时间再reset,否则客户端有pass过来会处理多一次pass行为
end


--小相公意外补牌
function M:deal_xiaoxianggong(id)

	local player = self.players[id]
	if player == nil then
		return false
	end	

	if self:is_xiao_xianggong(id) == false then
		return false
	end



	log.log("xiaoxianggong = %d cur_round = %d", id, self.cur_round)
	local card = table.remove(self.cards, 1)
	table.insert(player.cards, card)

	-- local myCard = {}	
	-- for _,_card in ipairs(player.cards) do
	-- 	table.insert(myCard, _card)
	-- end	
	-- skynet.send(player.agent, "lua", "send", "Table.FixCards", {id = id, leftcard = myCard})
	return true
end

function M:deal_daxianggong(id)
	local player = self.players[id]
	if player == nil then
		return false
	end	

	if self:is_da_xianggong(id) == false then
		return false
	end

	log.log("daxianggong = %d cur_round = %d", id, self.cur_round)
	table.remove(player.cards, 1)

	--local myCard = {}	
	--for _,_card in ipairs(player.cards) do
	--	table.insert(myCard, _card)
	--end		
	--skynet.send(player.agent, "lua", "send", "Table.FixCards", {id = id, leftcard = myCard})
	return true
end

function M:is_xiao_xianggong(id)
	local player = self.players[id]
	if player == nil then
		return false
	end
	local count = 0
	for i,card in ipairs(player.cards) do
		count = count + 1
	end
	if count == 3 or count == 6 or count == 9 or count == 12 then
		return true
	end

	return false
end

function M:is_da_xianggong(id)
	local player = self.players[id]
	if player == nil then
		return false
	end
	local count = 0
	for i,card in ipairs(player.cards) do
		count = count + 1
	end
	if count ==2 or count == 5 or count == 8 or count == 11 or count == 14 then
		return true
	end

	return false	
end


function M:voicechat(info, msg)
	self.voice_index = self.voice_index + 1
	log.log("voicechat = %d tabid = %d", self.voice_index, self.id)
	self:broadcast_others("Table.VoiceChat", {id = info.id, index = self.voice_index, data = msg.data}, info.id)
end

function M:quickmsg(info, msg)
	self:broadcast("Table.QuickMsg", {id = info.id, data = msg.msgid})	
end

function M:heartbeat(info, msg)

	if info ~= nil then
		--print("heartbeat called = ", info.id)
		if info.id ~= nil then
			local data = self.last_heartbeat_time[info.id]
			if data ~= nil then
				local time = data[1]
				local status = data[2]
				if status ~= true then
					self:broadcast("Table.ShowOfflineRsp", {id = info.id, yes = true})
				end
			end
			self.last_heartbeat_time[info.id] = {skynet.time(), true}

		end
	end	
end

function M:StartVoteReq(info, msg)
	self.vote_list = {}
	self.vote_list[info.id] = true	
	self:broadcast_others("Table.StartVoteRsp", {id = info.id}, info.id)
	--[[for id,data in pairs(self.last_heartbeat_time) do
		local time = data[1]
		local status = data[2]		
		if status == false then --离线玩家直接投票ok
			self.vote_list[id] = true
		end
	end	
	local newmsg = {yes = true}
	self.is_vote_mode = true
	self.vote_counter = 0
	self:VoteQuitReq(info, newmsg)	]]
	self.is_vote_mode = true
	self.vote_counter = 0
	log.log("StartVoteReq user=%d tableid=%d", info.id, self.id)
	--	vote_counter = 0, --投票计算器，15秒自动同意
	--	is_vote_mode = false
end

function M:VoteQuitReq(info, msg)
	if msg.yes == false then
		self.is_vote_mode = false
		self.vote_counter = 0
	end

	self.vote_list[info.id] = msg.yes
	local count = 0
	for k,v in pairs(self.vote_list) do
		if v == true then
			count = count + 1
		end
	end
	log.log("VoteQuitReq room id=%d tableid=%d", info.id, self.id)
	self:broadcast_others("Table.VoteQuitRsp", {id = info.id, yes = msg.yes}, info.id)
	if count >= PLAYER_COUNT then
		--dddd		
		self:broadcast("Table.QuitRsp", {id = 0, quit_type = 3})
		log.log("vote disband room user=%d tableid=%d", info.id, self.id)
		self:removeTable()
		
	end
end

--测试用，发送固定第一张牌
function M:deal_need(id, card)
	local cardnum = #self.cards - 1
	for k, v in pairs(self.players) do
		if v.id == id then
			table.remove(self.cards, 1)
			self.last_deal_card = card
			table.insert(v.cards, card)
			skynet.send(v.agent, "lua", "send", "Table.NewCard", {card = card,id = id,leftcard = cardnum, myleftcard = {}})
			self.cur_pos = v.pos
			--print("摸牌："..self:get_card_str(card).."  id="..id)
			--break
		else
			skynet.send(v.agent, "lua", "send", "Table.NewCard", {card = 0,id = id,leftcard = cardnum, myleftcard = {}})			
		end
	end	
	self:start_auto_play_counter()
	self:stop_auto_pass_counter()
	self.passlist = {}
	self.delay_peng = {}
	self.delay_gang = {}
	self.bugang_id = 0
	self.cur_card = 0	
end


function M:deal_one(id, gang_deal)
	local cardnum = #self.cards - 1
	if cardnum < REMAIN_CARD then
		self:handle_fail_round()
		return
	end
	local need_fix = false
	need_fix = self:deal_xiaoxianggong(id)
	if need_fix == false then
	 	need_fix = self:deal_daxianggong(id)
	end
	--need_fix = true
	local myleftcard = {}
	for k, v in pairs(self.players) do
		if v.id == id then
			local card = table.remove(self.cards, 1)
			if TEST_GANG == true and gang_deal == true then
				card = GANG_TEST
			end

			if TEST_DEAL == true then
				card = DEAL_TEST
			end

			if need_fix == true then
				for _,_card in ipairs(v.cards) do
					table.insert(myleftcard, _card)
				end	
			end

			self.last_deal_card = card
			table.insert(v.cards, card)

			skynet.send(v.agent, "lua", "send", "Table.NewCard", {card = card,id = id,leftcard = cardnum, myleftcard = myleftcard})
			self.cur_pos = v.pos
			local lastplay_id = self.poslist[self.cur_pos] 
			local str = "deal_one"..self:get_card_str(card).."  id="..id.."  card="..card.." self.cur_pos="..self.cur_pos.." lastplay_id = "..lastplay_id
			log.log(str)
			--break
		else
			skynet.send(v.agent, "lua", "send", "Table.NewCard", {card = 0,id = id,leftcard = cardnum, myleftcard = {}})
		end
	end
	self:start_auto_play_counter()
	self:stop_auto_pass_counter()
	self.passlist = {}
	self.delay_peng = {}
	self.delay_gang = {}
	self.bugang_id = 0
	self.cur_card = 0
end

function M:deal_test()

	local function W(i)
		return i
	end

	local function S(i)
		return i+9
	end

	local function T(i)
		return i+18
	end

	local function Z(i)
		return i+27
	end

	local p1 = {W(1), W(1), W(1), W(2), W(2), W(2), W(5), W(5), W(5), W(6), W(6), W(6), W(9)}
	local p2 = {W(2), W(3), W(4), W(5), W(6), W(7), W(8), W(9), T(5), T(5), T(5), T(6), T(6)}
	local p3 = {W(1), W(1), W(9), W(2), W(2), W(2), W(5), W(5), W(5), W(6), W(6), W(6), W(9)}
	local p4 = {Z(1), Z(1), Z(2), Z(2), Z(5), Z(5), Z(4), Z(4), Z(3), W(9), Z(7), Z(7), Z(3)}
	-- local p1 = {W(1), T(1), W(1), W(2), W(2), W(4), W(4), W(5), W(5), W(6), W(6), W(7), W(9)}
	-- local p2 = {W(9), W(9), W(9), T(1), T(1), T(9), Z(1), Z(6), Z(3), Z(4), W(1), W(1), W(1)}
	-- local p3 = {W(1), W(1), T(1), W(2), W(2), W(2), W(5), W(5), W(5), W(6), W(6), W(7), W(9)}
	-- local p4 = {W(9), T(9), W(8), W(2), W(2), W(4), W(4), W(5), W(5), W(6), W(6), W(7), W(1)}

	local first_card = Z(1)
	JIANGMA_TEST = {W(1), W(9), T(9), S(9), W(5), W(6), W(7), W(8)}
	GANG_TEST = W(1)
	DEAL_TEST = W(1) --每次摸固定牌
	local loop = 1
	for k, v in pairs(self.players) do
		if loop  == 1 then
			for i,card in pairs(p1) do
				table.insert(v.cards, card)
			end
		end

		if  loop == 2 then
			for i,card in pairs(p2) do
				table.insert(v.cards, card)
			end
		end

		if loop == 3 then
			for i,card in pairs(p3) do
				table.insert(v.cards, card)
			end
		end

		if loop == 4 then
			for i,card in pairs(p4) do
				table.insert(v.cards, card)
			end
		end

		loop = loop + 1
		
	end

	for k, v in pairs(self.players) do
		skynet.send(v.agent, "lua", "send", "Table.Cards", {cards = v.cards})
		--print("player id = "..k)
		--utils.print(v.cards)
	end	
	self:deal_need(self:get_zhuang_id(self.cur_round), first_card)
	return first_card
end


--1-9万字，10-18筒子，19-27条子,28东, 29南， 30西， 31北，32中，33发，34白
function M:get_card_str(index)

    if index >= 1 and index <= 9 then
        return tostring(index) .. "万"
    elseif index >= 10 and index <= 18 then
        return tostring(index - 9) .. "条"
    elseif index >= 19 and index <= 27 then
        return tostring(index - 18) .. "筒"
    end

    local t = {"东","南","西","北","中","发","白"}
    local s = t[index - 27]
    if s == nil then 
    	return "非法牌 "..tostring(index)
    end
    return s
end

function M:print_cards(cards)
	local str = "cardlist="
	for i,card in ipairs(cards) do
		--print(self:get_card_str(card))
		str =  str.."  "..self:get_card_str(card)
	end
	print(str)
end


function M:get_dir_str(dir)
	local t = {"东","南","西","北"}
	return t[dir]
end




return M
