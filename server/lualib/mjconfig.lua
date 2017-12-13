local M = {}
M.EXT_MONEY = 1      ---每日奖励钻石
M.FIRST_MONEY = 20   ---新玩家默认钻石
M.MONEY_ROUND8 = 3  --玩8局需要钻石
M.MONEY_ROUND16 = 6  --玩16局需要钻石
M.ROUND8 = 8
M.ROUND16 = 16
M.GM_NAME = "testgm" --总gm账号
M.VERSION_AND = "2.1"
M.VERSION_IOS = "1.0"
M.PLAYER_COUNT = 4 --玩家个数
M.AUTO_PLAY_TIMEOUT = 15  --自动打牌超时
M.AUTO_PASS_TIMEOUT = 12  --自动pass的超时
M.HU_TIMEOUT = 10 --胡了之后倒计时，到达后结算
M.PREPARE_TIMEOUT = 12 --结算后等待进入下一局的时间
M.REMAIN_CARD = 8 --剩余卡牌结束
M.TIME_OUT_TIMES = 2 -- 超时次数，超时那么多次，则委托自动de
M.GANG_SCORE = 3
M.ANGANG_SCORE = 2
M.BUGANG_SCORE = 1
M.GAME_TIMEOUT =  60 * 500 --桌子时间2小时自动解散 
M.CAN_LIANZHUANG = false --是否计算连庄分数

M.TEST_MODE = false --测试模式 
M.TEST_MA = false --开马测试模式，可强制结束时候开的码
M.TEST_GANG = false --测试杠上爆用，强制杠后发固定牌
M.TEST_DEAL = false --测试碰杠胡用，每次摸固定牌
M.TEST_DEBUG = false --开暗坑，测试3人不能pass的问题


M.GUEST_MODE = false --IOS游客模式
M.IOS_TEST_GUEST = 
{
    ["testios1"] = "111111",
    ["testios2"] = "222222",
    ["testios3"] = "333333",
    ["testios4"] = "444444",
    ["testios5"] = "555555",
    ["testios6"] = "666666",
    ["testios7"] = "777777",
    ["testios8"] = "888888",
    ["testios9"] = "999999",
    ["testios10"] = "000000",
    ["testios11"] = "aaaaaa",
    ["testios12"] = "bbbbbb",

} --ios游客测试

--M.DB_PASSWORD = "qwer1234"
M.DB_PASSWORD = "123456"
return M
