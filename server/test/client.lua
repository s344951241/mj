package.cpath = "./luaclib/?.so;./skynet/luaclib/?.so"
package.path = "./lualib/?.lua;./skynet/lualib/?.lua"

local socket = require "clientsocket"
local protobuf = require "protobuf"
local utils = require "utils"

function send(name, msg)
	-- print("---------------")
	-- print(name)
	-- utils.print(msg)
	-- print("---------------")
	local buf = protobuf.encode(name, msg)

	local len = 2 + #name + 2 + #buf
	local pack = string.pack(">Hs2s2", len, name, buf)
	socket.send(fd, pack)
end

function unpack_package(text)
	local size = #text
	if size < 2 then
		return nil, text
	end
	local s = text:byte(1) * 256 + text:byte(2)
	if size < s+2 then
		return nil, text
	end

	return text:sub(3,2+s), text:sub(3+s)
end

function recv_package(last)
	local result
	result, last = unpack_package(last)
	if result then
		return result, last
	end
	local r = socket.recv(fd)
	if not r then
		return nil, last
	end
	if r == "" then
		error "Server closed"
	end
	return unpack_package(last .. r)
end

function print_package(data)
	local name,buf = string.unpack(">s2s2", data)
	local msg = protobuf.decode(name, buf)
	utils.print(msg)
end

local last = ""
function dispatch_package()
	while true do
		local v
		v, last = recv_package(last)
		if not v then
			break
		end
		
		print_package(v)
	end
end

function parsecmd(str)
	local tokens = utils.split(str, " ")
	if not tokens then
		return
	end
	local cmd = tokens[1]
	local param1 = tokens[2]
	local param2 = tokens[3]

	-- print ("commands:" .. cmd ..",param1:" .. param1 .. ",param2:" .. param2)
	if cmd == "send" then
		param2 = load("return "..param2)()
	end

	-- print("cmd:" .. cmd, ",param1:" .. param1 .. ", param2:")
	utils.print(param2)

	return cmd, param1, param2
end

function help()
	print("commands:")
	print("\thelp: help info")
	print("\tquit: exit")
	print("\tsend: send name msg")
end

function enter( ... )
	send("Room.EnterReq", {room_id=1})
end

function create( ... )
	send("Table.CreateReq", {token="123"})
end

function join()
	send("Table.JoinReq", {tab_id=2})
end

function ready( ... )
	send("Table.ReadyReq", {})
end

function main(ip, port)
	protobuf.register_file("./proto/login.pb")
	protobuf.register_file("./proto/room.pb")
	protobuf.register_file("./proto/table.pb")
	fd = assert(socket.connect(ip, port))

	print("connect", ip, port, "success!")
	send("Login.LoginReq", {account="123",token="sss"})

	-- send("Room.EnterReq", {room_id=1})
	-- send("Table.MatchReq", {})

	while true do
		dispatch_package()
		local str = socket.readstdin()
		local cmd, param1, param2 = parsecmd(str)
		if cmd then
			if cmd == "quit" then
				break
			elseif cmd == "help" then
				help()
			elseif cmd == "enter" then
				enter()
			elseif cmd == "create" then
				create()
			elseif cmd == "join" then
				join()
			elseif cmd == "ready" then
				ready()
			elseif cmd == "send" then
				send(param1, param2)
			elseif cmd == "recv" then
				dispatch_package()
			end
		end
		socket.usleep(100)

	end

	socket.close(fd)
end

main("127.0.0.1", 8888)
