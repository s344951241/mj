local skynet = require "skynet"
local log = require "log"
local protopack = require "protopack"
local socket = require "socket"
local utils = require "utils"
local mjconfig = require "mjconfig"
require "skynet.manager"
--local db = require "db"
local gate
local SOCKET = {}
local agents = {}

---------------------------socket数据处理----------------------------
local sock_handler = {}


function checkpass(username, password)
	--log.log("verify failed account = %s password = %s", username, password)
	--print("checkpass username = ", username, password)
	if password == nil then
		return false
	end


	local test_pass = mjconfig.IOS_TEST_GUEST[username]
	--if test_pass == nil then
	--	return false
	--end
	if test_pass ~= nil then
		if test_pass == password then
			return true
		end
	end
	local msg = {}
	msg.username = username
	msg.password = password
	local ret = skynet.call("db", "lua", "ios_check_pass", msg)
	if ret == 1 then 
		return true
	end
	return false
end

sock_handler["Login.IOS_RegistReq"] = function(fd, msg)
	if mjconfig.GUEST_MODE == false then
		return
	end
	local newmsg = {username = msg.username, password = msg.password}
	local errno = skynet.call("db", "lua", "ios_regist", newmsg)
	if errno == 0 then
		--SOCKET.send(fd, "Table.SysErrorRsp",  {err_info = "注册成功", err_type = 0})
		SOCKET.send(fd, "Login.IOS_RegistRsp",  {errno = 0})
	else
		--SOCKET.send(fd, "Table.SysErrorRsp",  {err_info = "注册失败，用户已存在", err_type = 0})
		SOCKET.send(fd, "Login.IOS_RegistRsp",  {errno = 1})

	end	
end

sock_handler["Login.CheckVerReq"] = function(fd, msg)
	local guest_mode = false;
	local version_match = false
	--utils.print(msg)
	log.log("Login.CheckVerReq : version = %s, client_os = %s", msg.version, msg.client_os)
	local version_str = mjconfig.VERSION_AND

	if msg.client_os == "IOS" then
		guest_mode = mjconfig.GUEST_MODE
		version_str = mjconfig.VERSION_IOS
		--SOCKET.send(fd, "Login.CheckVerRsp", {guest_mode = true})
	end


	if msg.version == version_str then
		version_match = true
	end


	SOCKET.send(fd, "Login.CheckVerRsp", {guest_mode = guest_mode, version_match = version_match})
end 

sock_handler["Login.LoginReq"] = function (fd, msg)
	local version_str = mjconfig.VERSION_AND

	if msg.client_os == "IOS" then
		version_str = mjconfig.VERSION_IOS
	end

	if msg.version ~= version_str then
		SOCKET.send(fd, "Login.LoginRsp", {id = 0, money = 0, extmoney = 0, usertype = 0, err_no = 2})
		log.log("verify failed version not match account = %s version = %s client_os=%s", msg.account,version_str, msg.client_os)
		return
	end

	if msg.guest_mode == true then
		if checkpass(msg.account, msg.password) == false then	
			SOCKET.send(fd, "Table.SysErrorRsp",  {err_info = "用户密码错误", err_type = 0})		
			SOCKET.send(fd, "Login.LoginRsp", {id = 0, money = 0, extmoney = 0, usertype = 0, err_no = 1})
			log.log("verify failed password failed, account = %s password = %s", msg.account, msg.password)
			return
		end
	end

	agents[fd] = skynet.newservice("agent")
	msg.fd = fd
	log.log("login account = %s client_os = %s", msg.account, msg.client_os)
	local account_data = SOCKET.login(msg)
	if account_data == nil then
		log.log("verify failed account = %s", msg.account)
		SOCKET.send(fd, "Table.SysErrorRsp",  {err_info = "登录失败，请联系我们", err_type = 0})
		SOCKET.send(fd, "Login.LoginRsp", {id = 0, money = 0, extmoney = 0, usertype = 0, err_no = 1})
		--SOCKET.send(fd, "Login.LoginRsp", {id = 1, money = 0, extmoney = 0, usertype = 0, err_no = 0})
		return
	end
	--local account_data = SOCKET.login(msg.account)
	local id = skynet.call(agents[fd], "lua", "start", {gate = gate,
		fd = fd, watchdog = skynet.self(), account = msg.account, name = msg.name, url = msg.url, id = account_data.id, money = account_data.money, sex = msg.sex, usertype = account_data.usertype})
	
	SOCKET.send(fd, "Login.LoginRsp", {id = account_data.id, money = account_data.money, extmoney = account_data.extmoney, usertype = account_data.usertype, err_no = 0})
	--print("verify account success!", msg.account, msg.name, msg.url, account_data.money, account_data.extmoney)
	log.log("verify ok account = %s usertype = %d fd=%d id=%d", msg.account, account_data.usertype, fd, id)
end




function SOCKET.login(msg)
	--return db:login(account)
	return skynet.call("db", "lua", "login", msg)
end

------------------------ socket消息开始 -----------------------------
function SOCKET.open(fd, addr)
	log.log("New client from : %s  %d", addr, fd)
	skynet.call(gate, "lua", "accept", fd)
end


local function close_agent(fd)
	local a = agents[fd]
	agents[fd] = nil
	if a then
		skynet.call(gate, "lua", "kick", fd)
		-- disconnect never return
		skynet.send(a, "lua", "disconnect")
	end
end


function SOCKET.close_agents(fd)
	close_agent(fd)
end

function SOCKET.close(fd)
	log.log("socket close fd=%d", fd)
	close_agent(fd)
end

function SOCKET.error(fd, msg)
	log.log("socket error fd = %d msg=%s", fd, msg)
	close_agent(fd)
end

function SOCKET.warning(fd, size)
	-- size K bytes havn't send out in fd
	log.log("socket warning fd=%d size=%d", fd, size)
end

function SOCKET.data(fd, data)	
	local name, msg = protopack.unpack(data)
	if sock_handler[name] == nil then
		log.log("SOCKET.data = %s", name)
		return
	end
	sock_handler[name](fd, msg)
end

function SOCKET.send(fd, name, msg)
	local data = protopack.pack(name, msg)
	--utils.print(msg)
	socket.write(fd, data)
end

------------------------ socket消息结束-----------------------------

local CMD = {}
function CMD.start(conf)
	skynet.call(gate, "lua", "open" , conf)
end

function CMD.close(fd)
	close_agent(fd)
end

function CMD.test(fd)
	print("CMD.test = ", fd)
end

--gm指令修改金钱通知客户端刷新
function CMD.MoneyChange(msg)
	local fd = msg.fd
	local newmoney = msg.newmoney
	local a = agents[fd]
	log.log("MoneyChange fd = %d, money = %d", fd, newmoney )
	if a then
		skynet.call(a, "lua", "MoneyChange", newmoney)
	end
end

skynet.start(function()
	skynet.dispatch("lua", function(_, _, cmd, subcmd, ...)
		if cmd == "socket" then
			local f = SOCKET[subcmd]
			f(...)
			-- socket api don't need return
		else
			local f = assert(CMD[cmd])
			skynet.ret(skynet.pack(f(subcmd, ...)))
		end
	end)
	skynet.register("watchdog")
	gate = skynet.newservice("gate")
end)
