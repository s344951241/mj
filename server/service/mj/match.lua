local M = {
	list = {}
}

function M:add(info)

	for k, v in pairs(self.list) do 
		print ("ids:", v.id)
		if info.id == v.id then
			print("已经在桌上")
			return
		end
	end
	if  #self.list >= 4 then
		print("人满")
		return
	end
	table.insert(self.list, info)
end

function M:remove(id)
	for i,v in ipairs(self.list) do
		if v.id == id then
			table.remove(self.list,i)
			break
		end
	end
end

function M:peek()
	if #self.list < 4 then
		return
	end

	local p1 = table.remove(self.list,1)
	local p2 = table.remove(self.list,1)
	local p3 = table.remove(self.list,1)
	local p4 = table.remove(self.list,1)

	return p1, p2, p3, p4
end

return M 
