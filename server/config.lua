root="."
thread = 8
logger = nil
logpath = "."
harbor = 1
address = "127.0.0.1:2526"
master = "127.0.0.1:2013"
start = "main"	-- main script
bootstrap = "snlua bootstrap"	-- The service for bootstrap
standalone = "0.0.0.0:2013"
luaservice = "./service/?.lua;./service/?/main.lua;./skynet/service/?.lua"
lualoader = "./skynet/lualib/loader.lua"
lua_path = "./lualib/?.lua;./skynet/lualib/?.lua;./skynet/lualib/?/init.lua"
lua_cpath = "./luaclib/?.so;./skynet/luaclib/?.so"
cpath = "./skynet/cservice/?.so"
-- daemon = "./skynet.pid"