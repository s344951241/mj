local skynet = require "skynet"
local utils = require "utils"
local log = {}

function log.log(format, ...)
	local format_new = utils.get_time_str_sec().."="..format
	skynet.error(string.format(format_new, ...))
	--skynet.error(string.format(format, ...))
end

return log


