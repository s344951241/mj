package.path = "../../lualib/?.lua;"..package.path

local utils = require "utils"
local hu_table = require "hu_table"

local CardType = {
    [0x10] = {min = 1, max = 9, chi = true},
    [0x20] = {min = 10, max = 18, chi = true},
    [0x30] = {min = 19, max = 27, chi = true},
    [0x40] = {min = 28, max = 34, chi = false},
}


--1-9万字，10-18筒子，19-27条子,28东, 29南， 30西， 31北，32中，33发，34白
--[[function M:get_card_str(index)

    if index >= 1 and index <= 9 then
        return index .. "万"
    elseif index >= 10 and index <= 18 then
        return (index - 9) .. "筒"
    elseif index >= 19 and index <= 27 then
        return (index - 18) .. "条"
    end

    local t = {"东","南","西","北","中","发","白"}
    local s = t[index - 27]
    if s == nil then 
        return "非法牌 "..index
    end
    return s
end]]

local M = {}
--删除t表内count个元素 value 比如 t = {1,2,2,2,2,3,4,5}  :remove_t_value(t, 3, 2) ,, 将2删掉三个
function M:remove_t_value(t, count, value)
    local tmp_count = 0
    for i = #t,1,-1 do
        if t[i] == value then
            table.remove(t, i)
            tmp_count = tmp_count + 1
            if tmp_count >= count then
                break
            end
        end
    end
end

function M:an_gang_score()
    return 10
end

function M:gang_score()
    return 5
end

function M:get_hu_data(hu_type)
    local t =  {
        [1] = {"天胡", 40},
        [2] = {"地胡", 20},
        [3] = {"十八罗汉", 36},
        [4] = {"十三幺", 26},
        [5] = {"大四喜", 20},
        [6] = {"大三元", 20},
        [7] = {"字一色", 20},
        [8] = {"小四喜", 10},
        [9] = {"小三元", 10},
        [10] = {"混幺九", 10},
        [11] = {"七对子", 6},
        [12] = {"清碰", 8},
        [13] = {"混碰", 6},
        [14] = {"混龙", 6},
        [15] = {"清一色", 6},
        [16] = {"混一色", 4},
        [17] = {"对对胡", 4},
        [18] = {"鸡平", 2},
        [19] = {"鸡平", 2},
        [20] = {"一条龙", 10}
    }  
    local v =  t[hu_type] 
    if v == nil then
        v = t[19]
    end

    return v
end

function M:get_hu_name(hu_type)
    local v = self:get_hu_data(hu_type)
    return v[1]  
end

function M:get_hu_score(hu_type)
    local v = self:get_hu_data(hu_type)
    --print("hu_type = "..hu_type.." name = "..v[1])
    return v[2]
end

function M:get_maima_dir(maima_card)
    local p = {
        [1] = {1, 5, 9, 10, 14, 18, 19, 23, 27, 28},
        [2] = {2, 6, 11, 15, 20, 24, 29, 32},
        [3] = {3, 7, 12, 16, 21, 25, 30, 33},
        [4] = {4, 8, 13, 17, 22, 26, 31, 34}
    }
    
    for dir,v in pairs(p) do
        for _,d in ipairs(v) do
            if d == maima_card then
                return dir
            end
        end
    end  
    return 0 
end

--根据胡的风位获得码数,  cards开出来的8张牌，看一共中了几个？
function M:get_jiangma_count(hu_dir, jiangma_cards)
    local p = {
        [1] = {1, 5, 9, 10, 14, 18, 19, 23, 27, 28},
        [2] = {2, 6, 11, 15, 20, 24, 29, 32},
        [3] = {3, 7, 12, 16, 21, 25, 30, 33},
        [4] = {4, 8, 13, 17, 22, 26, 31, 34}
    }
    local pcard = p[hu_dir]
    local count = 1
    local t = {}
    if pcard == nil then return 1 end
    for _, card in ipairs(pcard) do
        for _,jiangma in ipairs(jiangma_cards) do
            if card == jiangma then
                count = count + 1
                table.insert(t, card)
            end
        end
    end
    return count, t
    --return p[pos]
end


function M:get_card_count(cards, card)
    local num = 0
    for k, v in pairs(cards) do
        if v == card then
            num = num + 1
        end
    end
    return num
end   

function M:can_gang(cards, card)
    return self:get_card_count(cards, card) >= 3
end 

function M:can_angang(cards, card)
    return self:get_card_count(cards, card) >= 4
end


function M:can_peng(cards, card)

    return self:get_card_count(cards, card) >= 2
end

function M:can_hu(cards1, cards2)
	local function dotrans(cards)
        local t = {}
        for _,card in ipairs(cards) do
            if t[card] == nil then
                t[card] = 1
            else
                t[card] = t[card] + 1
            end
        end
        return t
    end

    if #cards1 ~= #cards2 then
        return false
    end

    local equal = true
    local t1 = dotrans(cards1)
    local t2 = dotrans(cards2)
    for k,v in pairs(t1) do
        if t2[k] ~= v then
            equal = false
            break
        end
    end
    return equal
end


function M:is_can_hu(mah)
    local pais = {}
    for i,v in ipairs(mah) do
        table.insert(pais, v)
    end

    if (#pais == 2) then
        return pais[1] == pais[2]
    end

    table.sort(pais,function(a,b) return a<b end)

end
--[[
    public static bool IsCanHU(List<int> mah, int ID)
    {
        List<int> pais = new List<int>(mah);

        pais.Add(ID);
        //只有两张牌
        if (pais.Count == 2)
        {
            return pais[0] == pais[1];
        }

        //先排序
        pais.Sort();

        //依据牌的顺序从左到右依次分出将牌
        for (int i = 0; i < pais.Count; i++)
        {
            List<int> paiT = new List<int>(pais);
            List<int> ds = pais.FindAll(delegate (int d)
                {
                    return pais[i] == d;
                });

            //判断是否能做将牌
            if (ds.Count >= 2)
            {
                //移除两张将牌
                paiT.Remove(pais[i]);
                paiT.Remove(pais[i]);

                //避免重复运算 将光标移到其他牌上
                i += ds.Count;

                if (HuPaiPanDin(paiT))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private static bool HuPaiPanDin(List<int> mahs)
    {
        if (mahs.Count == 0)
        {
            return true;
        }

        List<int> fs = mahs.FindAll(delegate (int a)
            {
                return mahs[0] == a;
            });

        //组成克子
        if (fs.Count == 3)
        {
            mahs.Remove(mahs[0]);
            mahs.Remove(mahs[0]);
            mahs.Remove(mahs[0]);

            return HuPaiPanDin(mahs);
        }
        else
        { //组成顺子
            if (mahs.Contains(mahs[0] + 1) && mahs.Contains(mahs[0] + 2))
            {
                mahs.Remove(mahs[0] + 2);
                mahs.Remove(mahs[0] + 1);
                mahs.Remove(mahs[0]);

                return HuPaiPanDin(mahs);
            }
            return false;
        }
    }]]
return M