local skynet = require "skynet"
require "skynet.manager"
local mysql = require "mysql"
local utils = require "utils"
local mjconfig = require "mjconfig"
local log = require "log"
local CMD = {}
local mysqldb = {}
local account_cache = {}
local id2account = {}
local score_id = 1
local score_list = {} --[id] = {info = {tableinfo}, round_list = {{round1}, {round2}}}
local player_table_list = {} --[playerid] = {tableid, tableid, tableid}
--local EXT_MONEY = 2
--local FIRST_MONEY = 20
local ios_test_user = {} --ios注册用的
--openid就是account
local function dump(obj)
    local getIndent, quoteStr, wrapKey, wrapVal, dumpObj
    getIndent = function(level)
        return string.rep("\t", level)
    end
    quoteStr = function(str)
        return '"' .. string.gsub(str, '"', '\\"') .. '"'
    end
    wrapKey = function(val)
        if type(val) == "number" then
            return "[" .. val .. "]"
        elseif type(val) == "string" then
            return "[" .. quoteStr(val) .. "]"
        else
            return "[" .. tostring(val) .. "]"
        end
    end
    wrapVal = function(val, level)
        if type(val) == "table" then
            return dumpObj(val, level)
        elseif type(val) == "number" then
            return val
        elseif type(val) == "string" then
            return quoteStr(val)
        else
            return tostring(val)
        end
    end
    dumpObj = function(obj, level)
        if type(obj) ~= "table" then
            return wrapVal(obj)
        end
        level = level + 1
        local tokens = {}
        tokens[#tokens + 1] = "{"
        for k, v in pairs(obj) do
            tokens[#tokens + 1] = getIndent(level) .. wrapKey(k) .. " = " .. wrapVal(v, level) .. ","
        end
        tokens[#tokens + 1] = getIndent(level - 1) .. "}"
        return table.concat(tokens, "\n")
    end
    return dumpObj(obj, 0)
end


--[[
score_list = 
{
    [1] = 
    { --game_score
        tableid = 1000,
        starttime =  "2000",
        endtime = "3000",
        ownerid = "abc",
        playerlist = 
        {
            {id = 1, name = "abc", total_score = 100}, 
            { name, total_score}, 
            { name, total_score}, 
            { name, total_score}
        },

        roundlist = 
        {
            RoundScore1,RoundScore2,RoundScore3
        }
        maimalist = 
        {
            HistoryMaiMa,HistoryMaiMa,HistoryMaiMa
        }
        jiangmalist = 
        {
            HistroyJiangMa, HistroyJiangMa
        }
    }
}]]
--返回HistoryTableDetailRsp
function CMD.get_table_score(score_id)
    local score = score_list[score_id]

    if score == nil then
        print("get_table_score 数据不存在", score_id)
        return nil
    end
    --utils.print(score_list)
    --print("查询 get_table_score",score_id)
    log.log("get_table_score score_id = %d", score_id)
    local ret = {}   
    ret.index = score_id
    ret.round_score_list = score.roundlist
    ret.maima_list = score.maimalist
    ret.jiangma_list = score.jiangmalist

    return ret
end

function CMD.get_history_table_list(msg)
    local id = msg.id
    local page = msg.page

    local PAGE_COUNT = 5
    local ret = {}
    local t = player_table_list[id]
    if t == nil then
        log.log("get_history_table_list 该玩家尚无记录 = %d", id)
        return nil
    end

    local maxpage = math.ceil(#t/PAGE_COUNT)
    if page > maxpage then
        --print("超过最大页面:", maxpage, page)
        log.log("get_history_table_list 超过最大页面 %d, %d:", maxpage, page)
        return nil
    end

    log.log("get_history_table_list count = %d, id = %d, page = %d", #t, id, page)
    --print("get_history_table_list  记录个数 =", #t, id, page)
    --utils.print(t)
    --print('111111111111111')
    for i = #t - page * PAGE_COUNT + PAGE_COUNT, #t - page * PAGE_COUNT + 1,-1 do
        local score_id = t[i]
        if score_id ~= nil then
            local score = score_list[score_id]
            if score ~= nil then
                local table_info = {} --桌子信息简要，对应HistoryTable
                table_info.table_id = score.tableid
                table_info.index = score_id
                table_info.start_time = score.starttime
                table_info.end_time = score.endtime
                table_info.owner_id = score.ownerid
                table_info.playerinfo_list = score.playerlist
                table.insert(ret, table_info)
            else
                print("score is nil")
            end
        else
            print("scoreid is nil", score_id)
        end
    end
    return {ret,maxpage}
end

function CMD.add_history_score(game_score) --整局数据，8/16 round
    score_list[score_id] = game_score
    local playerid_list = game_score.playerlist
    for _,v in pairs(playerid_list) do
        if player_table_list[v.id] == nil then
            player_table_list[v.id] = {}
        end
        table.insert(player_table_list[v.id], score_id)
    end
    score_id = score_id + 1
    --print("db add_history_score = ")
    --utils.print(playerid_list)
    --print("111111111111111")
    utils.print(player_table_list)
    return score_id - 1
end



function CMD.start()
    CMD.connect_db()
end

function CMD.testcall(value)
    print("db testcall = ", value)
end

function CMD.register(account, nickname) 
    local now_timestr = utils.get_time_str()
    local account_data = {openid = account, money = mjconfig.FIRST_MONEY, logouttime = now_timestr, usertype = 0, nickname = nickname, createtime = now_timestr, logintime = now_timestr}--还缺个id
   
    --local sql_str = string.format("insert into user SET openid=%s,money=%d,logouttime=%s,nickname=%s,create_time=%s,login_time=%s"
    local sql_str = utils.build_sql("insert into user SET ", account_data)
    --print("sql_str = ", sql_str)
    local rs = mysqldb:query(sql_str)
    --local rs = mysqldb:query("insert into user SET openid='"..account.."', money=100,logouttime=0,nickname='"..nickname.."'")
    --print("新用户 = ", now_timestr, dump(rs))
    account_data.id = rs.insert_id
    account_data.extmoney = mjconfig.FIRST_MONEY
    log.log("CMD.register account_data = %s rs = %s", dump(account_data), dump(rs))

    if rs.insert_id == nil then --第一次插入错误，可能是火星文问题，再插一次
        nickname = "FakeUserName"
        account_data = {openid = account, money = mjconfig.FIRST_MONEY, logouttime = now_timestr, usertype = 0, nickname = nickname, createtime = now_timestr, logintime = now_timestr}
        sql_str = utils.build_sql("insert into user SET ", account_data)
        rs = mysqldb:query(sql_str)
        account_data.id = rs.insert_id
        account_data.extmoney = mjconfig.FIRST_MONEY
        log.log("CMD.register insert_id = nil account_data = %s rs = %s", dump(account_data), dump(rs))
    end
    return account_data
end

function CMD.updateMoney(param)
    local id = param.id
    local newmoney = param.money
    local account = id2account[id]
    if account ~= nil then
        local account_data = account_cache[account]
        account_data.money = newmoney
    end
    mysqldb:query("update user SET money="..newmoney.." where id ="..id)
end

function CMD.updateUserType(id, value)
    local account = id2account[id]
    if account ~= nil then
        local account_data = account_cache[account]
        account_data.usertype = value
    end
    mysqldb:query("update user SET usertype="..value.." where id ="..id)
end

function CMD.logout(msg)
end


function CMD.login(msg)
    local function is_login_otherday(now_time_str, last_login_time_str) --检测隔日登陆
        --local now_time_str = utils.get_time_str()
        local now_daystr = utils.get_date_str(now_time_str) --日期
        local last_daystr = utils.get_date_str(last_login_time_str)
        --print("is_login_otherday = ", now_daystr, last_daystr, (now_daystr ~= last_daystr))
        return (now_daystr ~= last_daystr)
    end

    local account = msg.account
    local nickname = msg.name
    local account_data = account_cache[account]
    local now_time_str = utils.get_time_str()
    local isold_user = false
    local login_other_day = false
    local last_login_time_str = now_time_str
    local account_data_type = 0
    if account_data == nil then
        local rs = mysqldb:query("select * from user where openid='"..account.."'")
        --print(dump(rs), " rs= ", rs)
        if next(rs) == nil then --新用户注册
            account_data = CMD.register(account, nickname)
            account_data.fd = msg.fd
            account_cache[account] = account_data
            account_data_type = 1
           -- print("ddddddddddddddddddddddddddddddd")
        else
            account_data = rs[1]
            last_login_time_str = account_data.logintime
            isold_user = true
            --print("login --------------------", now_time_str, last_login_time_str)
            login_other_day = is_login_otherday(now_time_str, last_login_time_str)
            account_data.extmoney = 0
            if login_other_day == true then
                account_data.money = account_data.money + mjconfig.EXT_MONEY --隔日登陆加1块
                account_data.extmoney = mjconfig.EXT_MONEY
            end            
            account_data.logintime = now_time_str
            account_data.fd = msg.fd
            account_cache[account] = account_data
            account_data_type = 2
            log.log("next(rs) is nil account_data_type = %d account_data = %s", account_data_type, dump(account_data))
        end
    else

        last_login_time_str = account_data.logintime
        isold_user = true
        login_other_day = is_login_otherday(now_time_str, last_login_time_str)
        account_data.extmoney = 0
        if login_other_day == true then
            account_data.money = account_data.money + mjconfig.EXT_MONEY --隔日登陆加1块
            account_data.extmoney = mjconfig.EXT_MONEY
        end
        --print("login_other_day = ", login_other_day)
        account_data.fd = msg.fd
        account_data.logintime = now_time_str
        account_data_type = 3
       -- print("sssssssssssssssssssssssssssssssssssssss")
    end
    --print("login = "..account.." nickname = ", nickname, now_time_str, last_login_time_str)
    if isold_user == true then
        if login_other_day == true then            
            local sql = "update user SET logintime='"..now_time_str.."', money="..account_data.money.." where id ="..account_data.id
            mysqldb:query(sql)     
            --print("sql1 = ", sql)      
            --mysqldb:query("update user SET logintime='"..now_timestr.."' where id ="..account_data.id)
        else
            if account_data.id ~= nil then
                mysqldb:query("update user SET logintime='"..now_time_str.."' where id ="..account_data.id)
            else
                log.log("account_data.id is nil account_data_type = %d account_data = %s", account_data_type, dump(account_data))
                account_data = CMD.register(account, "fakeuser")
                account_data.fd = msg.fd
                account_cache[account] = account_data       
            end
            --mysqldb:query("update user SET logintime='"..now_time_str.."' where id ="..account_data.id)
            --print("sql2 = ", sql)
        end        
    end

    --[[if account_data == nil then
        account_data = CMD.register(account, nickname)
        account_cache[account] = account_data
    end]]
    if account_data.id == nil then
        return nil
    end
    id2account[account_data.id] = account
    --mysqldb:query("update user SET logintime='"..now_timestr.."' where id ="..account_data.id)
    if account == mjconfig.GM_NAME then
        account_data.usertype = 2
    end

    return account_data
end


local function load_all()
    local rs = mysqldb:query("select * from user")
    local count = 0
    for _,data in pairs(rs) do
        account_cache[data.openid] = data
        id2account[data.id] = data.openid
        --if data.id == 2 then
        --    print("222222222222", data.openid)
        --    utils.print(data)
        --end
        count = count + 1
    end

    log.log("数据库玩家总数 = %d", count)
end


function CMD.connect_db()
    local function on_connect(db)
        db:query("set charset utf8")
        log.log("db connect ok")
        mysqldb = db
        load_all()
        --CMD.gm_query_money_log_by_index({index = 33})
       --CMD.gm_query_money_log({id=101,startindex=0,is_active=false})
        --CMD.gm_query_agentlist()
        --CMD.gm_query_player_bymoney({money = 15})

        --for i=1,100 do
        --    CMD.gm_log(100, 100+i, "testabc", "test"..i, 999) 
        --end
        --CMD.gm_log(11, 101, "testaaa11", "test101", 222) 
        --CMD.gm_log(2, 101, "testaaa2", "test101", 222) 
        --CMD.gm_log(3, 101, "testaaa3", "test101", 222) 
        
    end
    local db = mysql.connect({
        host="127.0.0.1",
        port=3306,
        database="csmj",
        user="root",
        password=mjconfig.DB_PASSWORD,
        max_packet_size = 1024 * 1024,
        on_connect = on_connect
    })
    if not db then
        log.log("db connect failed")
    end    
end


--0 -成功， 1-非大gm, 2-代理，操作失败，3目标玩家不存在, 4-非Gm,非代理，5非法用户， 6-代理不够钱转出，7代理不能转负数， 8不能自己转给自己
function CMD.gm_add_money(msg)
    local fromid = msg.fromid
    local toid = msg.toid
    local money = msg.money
    if fromid == toid then
        return 8
    end

    local from_account = id2account[fromid]    
    if from_account == nil then
        return 5
    end

    local from_data = account_cache[from_account]
    if from_data == nil then
        return 5
    end
    local usertype = from_data.usertype
    if from_account == mjconfig.GM_NAME then
        usertype = 2
    end

    if usertype ~= 1 and usertype ~= 2 then --无权限
        return 4
    end

    local to_account = id2account[toid]
    local to_data = nil
    if to_account ~= nil then
        to_data = account_cache[to_account]
    end

    if to_data == nil then
        local rs = mysqldb:query("select * from user where id="..toid)        
        if next(rs) == nil then --用户不存在

        else
            to_data = rs[1]
        end     
    end
    if to_data == nil then
        return 3
    end

    local fromname = from_data.nickname
    local toname = to_data.nickname


    if usertype == 2 then --总gm
        local newmoney = to_data.money + money
        local param = {id = toid, money = newmoney}
        CMD.updateMoney(param)
        --print('to_data = ')
        --utils.print(to_data)        
        if to_data.fd ~= nil then            
            skynet.call("watchdog", "lua", "MoneyChange", {fd = to_data.fd, newmoney = newmoney})
        end    

        CMD.gm_log(fromid, toid, fromname, toname, money)
        return 0
    end

    if usertype == 1 then --代理
        if money <= 0 then --不能扣对方钱
            return 7
        end

        if from_data.money < money then --够不够钱
            return 6
        end
        local newmoney_from = from_data.money - money
        local newmoney_to = to_data.money + money
        local param_from = {id = fromid, money = newmoney_from}
        local param_to = {id = toid, money = newmoney_to}
        CMD.updateMoney(param_from)
        CMD.updateMoney(param_to)
        if to_data.fd ~= nil then            
            skynet.call("watchdog", "lua", "MoneyChange", {fd = to_data.fd, newmoney = newmoney_to})
        end    

        if from_data.fd ~= nil then            
            skynet.call("watchdog", "lua", "MoneyChange", {fd = from_data.fd, newmoney = newmoney_from})
        end  
        CMD.gm_log(fromid, toid, fromname, toname, money)        
        return 0
    end
    return 0
end

function CMD.gm_setagent(msg)
    local id = msg.id
    local from_id = msg.fromid
    local is_agent = msg.yes

    local account = id2account[id]    
    if account == nil then
        log.log("account = nil")
        return 1 --不存在
    end

    local from_account = id2account[from_id]
    if from_account == nil then
        return 3
    end
    
    if from_account ~= mjconfig.GM_NAME then
        return 2
    end
       
    if is_agent == true then
        CMD.updateUserType(id, 1)
    else
        CMD.updateUserType(id, 0)
    end
    return 0
end

function CMD.gm_query_money_log_by_index(msg)
    local index = msg.index
    local money_log_list = {}
    local sql_str = "select * from gmlog where id="..index
    local rs = mysqldb:query(sql_str)
    log.log("gm_query_money_log_by_index = %d", index)
    for _,log_data in pairs(rs) do
        print("log_data = ", log_data)
        local money_log = {}
        money_log.index = log_data.id
        money_log.fromid = log_data.fromid
        money_log.fromname = log_data.fromnickname
        money_log.toid = log_data.toid
        money_log.toname = log_data.tonickname
        money_log.value = log_data.money
        money_log.optime = log_data.optime
        table.insert(money_log_list, money_log)
    end
    utils.print(money_log_list)
    return money_log_list    
end

function CMD.gm_query_money_log(msg)
    local id = msg.id
    local is_active = msg.is_active
    local start = msg.startindex
    local money_log_list = {}
    local sql_str = "select * from gmlog where fromid="..id.." order by id desc LIMIT "..start..",6"
    if is_active == false then
        sql_str = "select * from gmlog where toid="..id.." order by id desc LIMIT "..start..",6"
    end
   -- print("1111111111111111 = ", start)
    local rs = mysqldb:query(sql_str)
   -- print(dump(rs))
   -- print("1111111111111111")
    for _,log_data in pairs(rs) do
        local money_log = {}
        money_log.index = log_data.id
        money_log.fromid = log_data.fromid
        money_log.fromname = log_data.fromnickname
        money_log.toid = log_data.toid
        money_log.toname = log_data.tonickname
        money_log.value = log_data.money
        money_log.optime = log_data.optime
        table.insert(money_log_list, money_log)
    end
    utils.print(money_log_list)
    return money_log_list
end


function CMD.gm_query_player_bymoney(msg)
    local money = msg.money
    local player_info_list = {}
    local sql_str = "select * from user where money>="..money
    local rs = mysqldb:query(sql_str)
    log.log("gm_query_player_bymoney = %d", money)
    for _,account_data in pairs(rs) do
        local player_info = {}
        player_info.id = account_data.id
        player_info.name = account_data.nickname
        player_info.total_money = account_data.money
        if account_data.usertype == 1 then
            player_info.is_agent = true
        else
            player_info.is_agent =  false
        end
        table.insert(player_info_list, player_info)
    end
   --utils.print(player_info_list)

    return player_info_list    
end

--返回GMPlayerInfo列表
function CMD.gm_query_agentlist(msg)
    local player_info_list = {}
    local sql_str = "select * from user where usertype=1"
    local rs = mysqldb:query(sql_str)

    for _,account_data in pairs(rs) do
        local player_info = {}
        player_info.id = account_data.id
        player_info.name = account_data.nickname
        player_info.total_money = account_data.money
        if account_data.usertype == 1 then
            player_info.is_agent = true
        else
            player_info.is_agent =  false
        end
        table.insert(player_info_list, player_info)
    end
    --utils.print(player_info_list)
    return player_info_list
end


--返回GMPlayerInfo
function CMD.gm_query_player(msg)
    local player_info_list = {}
    local id = msg.id
    local account_data = nil
    local sql_str = "select * from user where id="..id
    local rs = mysqldb:query(sql_str)

    if next(rs) == nil then --用户不存在
        return player_info_list
    else
        account_data = rs[1]
    end

    local player_info = {}
    player_info.id = account_data.id
    player_info.name = account_data.nickname
    player_info.total_money = account_data.money
    if account_data.usertype == 1 then
        player_info.is_agent = true
    else
        player_info.is_agent =  false
    end
    table.insert(player_info_list, player_info)
    utils.print(player_info_list)
    return player_info_list
end


function CMD.gm_log(fromid, toid, fromname, toname, money)
    local now_timestr = utils.get_time_str()
    local data = {fromnickname = fromname, tonickname = toname, fromid = fromid, toid = toid, money = money, optime = now_timestr}
    local sql_str = utils.build_sql("insert into gmlog SET ", data)
    local rs = mysqldb:query(sql_str)
    --local rs = mysqldb:query("insert into gmlog SET fromnickname='"..fromname.."', tonickname='"..toname.."',fromid="..fromid..",toid="..toid..",money="..money..) 
end


function CMD.ios_purchase(msg)
    local money_data = {[1] = 2, [2] = 4, [3] = 10}

    local purchase_id = msg.purchase_id
    local toid = msg.playerid
    local money = money_data[purchase_id]
    if money == nil then
        money = 3
    end

    local to_account = id2account[playerid]
    local to_data = nil
    if to_account ~= nil then
        to_data = account_cache[to_account]
    end

    if to_data == nil then
        local rs = mysqldb:query("select * from user where id="..toid)        
        if next(rs) == nil then --用户不存在

        else
            to_data = rs[1]
        end     
    end

    if to_data == nil then
        return 0
    end

    local newmoney = to_data.money + money
    local param = {id = toid, money = newmoney}
    CMD.updateMoney(param)
    --print('to_data = ')
    --utils.print(to_data)
    if to_data.fd ~= nil then            
        skynet.call("watchdog", "lua", "MoneyChange", {fd = to_data.fd, newmoney = newmoney})
    end     


   return newmoney
end

--ios_test_user
function CMD.ios_regist(msg)
    local username = msg.username
    local password = msg.password
    --用户已存在

    if mjconfig.IOS_TEST_GUEST[username] ~= nil then
        return 1
    end

    if ios_test_user[username] ~= nil then
        return 1
    end

    ios_test_user[username] = password
    log.log("ios_regist username = %s, password = %s", username, password)
    return 0
end


--1登录成功， 0登录失败
function CMD.ios_check_pass(msg)
    local username = msg.username
    local password = msg.password
    if ios_test_user[username] == password then
        return 1
    end
    return 0
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
    skynet.register("db")
end)