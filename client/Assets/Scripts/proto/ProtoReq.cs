using UnityEngine;
using System.Collections;
using NetWork;
using Table;
using System.Collections.Generic;

public class ProtoReq{
    NetClient network;

    public static void CheckVersion(string version,string os)
    {
        Login.CheckVerReq ver = new Login.CheckVerReq();
        ver.version = version;
        ver.client_os = os;
        NetClient.Instance().WriteMsg("Login.CheckVerReq", ver);
    }

    public static void zc(string name, string password)
    {
        Login.IOS_RegistReq regist = new Login.IOS_RegistReq();
        regist.username = name;
        regist.password = password;
        NetClient.Instance().WriteMsg("Login.IOS_RegistReq", regist);
    }
    public static void LoginReq(bool flag,string account,string name,int sex,string url,string password = "")
    {
        Login.LoginReq login = new Login.LoginReq();
        login.version = GameConfig.programVersion;
        login.guest_mode = flag;
        login.account = account;
        login.password = password;
        login.name = name;
        login.url = url;
        login.sex = sex;
        login.client_os = GameConfig.OsName;
        MainRole.Instance.Url = url;
        MainRole.Instance.Name = name;
        MainRole.Instance.Sex = sex==1?true:false;

        NetClient.Instance().WriteMsg("Login.LoginReq", login);
    }
    public static void EnterRoom()
    {
        Room.EnterReq room = new Room.EnterReq();
        room.room_id = 1;
        NetClient.Instance().WriteMsg("Room.EnterReq", room);
    }
    public static void CreateTable(int playoffs,int times,int jiangma,int maima,string token)
    {
        Table.CreateReq table = new CreateReq();
        table.token = token;
        table.playoffs = playoffs;
        table.times = times;
        table.jiangma = jiangma;
        table.maima = maima;
        NetClient.Instance().WriteMsg("Table.CreateReq", table);
    }

    public static void JoinTable(int id)
    {
        Table.JoinReq table = new JoinReq();
        table.tab_id = id;
        NetClient.Instance().WriteMsg("Table.JoinReq", table);
    }

    public static void ReJoinTable(int id)
    {
        Table.ReJoinReq table = new ReJoinReq();
        table.tab_id = id;
        NetClient.Instance().WriteMsg("Table.ReJoinReq", table);
    }
    public static void Ready()
    {
        Table.ReadyReq ready = new Table.ReadyReq();
        NetClient.Instance().WriteMsg("Table.ReadyReq", ready);
    }

    public static void Quit()
    {
        Table.QuitReq quit = new QuitReq();
        NetClient.Instance().WriteMsg("Table.QuitReq", quit);
    }
    public static void Dissolve()
    {
            
    }
    public static void Kick(int roleId)
    {
        Table.KickReq kick = new KickReq();
        kick.id = roleId;
        NetClient.Instance().WriteMsg("Table.KickReq", kick);
    }
    public static void Turn()
    {
        Table.Turn turn = new Table.Turn();
        NetClient.Instance().WriteMsg("Table.Turn", turn);
    }
    public static void Play(int num)
    {
        Table.Play play = new Table.Play();
        play.card = num;
        NetClient.Instance().WriteMsg("Table.Play", play);
    }
    public static void Gang(int num)
    {
        Debug.Log("杠了个：" + num);
        Table.Gang gang = new Table.Gang();
        gang.card = num;
        NetClient.Instance().WriteMsg("Table.Gang", gang);
    }

    //public static void AnGang(int num)
    //{
    //    Table.Angang gang = new Table.Angang();
    //    gang.card = num;
    //    NetClient.Instance().WriteMsg("Table.Angang", gang);
    //}
    public static void Pass(int card)
    {
        Debug.LogError("发送pass"+card);
        Table.Pass pass = new Table.Pass();
        pass.passcard = card;
        NetClient.Instance().WriteMsg("Table.Pass", pass);    
    }

    public static void Peng(int num)
    {
        Debug.Log("碰了:" + num);
        Table.Peng peng = new Table.Peng();
        peng.card = num;
        NetClient.Instance().WriteMsg("Table.Peng", peng);
    }

    public static void NetCard()
    {
        Table.NewCard card = new NewCard();
        NetClient.Instance().WriteMsg("Table.NewCard", card);
    }

    public static void Hu(int type, int card, List<int> cards)
    {
        Table.Hu hu = new Table.Hu();
        hu.hutype = type;
        hu.card = card;
        hu.cards.AddRange(cards);
        NetClient.Instance().WriteMsg("Table.Hu", hu);
    }

    public static void PassHu()
    {
        NetClient.Instance().WriteMsg("Table.PassHu", new Table.PassHu());
    }
    public static void VoiceChat(byte[] data)
    {
        Table.VoiceChat voice = new Table.VoiceChat();
        if (data.Length > 16384)
        {
            byte[] b = new byte[16384];
            for (int i = 0; i < 16384; i++)
            {
                b[i] = data[i];
            }
            voice.data = b;
        }
        else
        {
            voice.data = data;
        }
        NetClient.Instance().WriteMsg("Table.VoiceChat", voice);
    }


  
    public static void findUser(int id)
    {
        //Table.GMQueryInfoReq query = new GMQueryInfoReq();
        //query.id = id;
        //NetClient.Instance().WriteMsg("Table.GMQueryInfoReq", query);
    }

    public static void findPlayer()
    {
        //Table.GMQueryInfoReq player = new GMQueryInfoReq();
        //NetClient.Instance().WriteMsg("Table.GMQuerySysInfoReq", player);
    }

    public static void heartReq()
    {
        NetClient.Instance().WriteMsg("Table.HeartbeatReq", new Table.HeartbeatReq());
    }

    public static void historyTableReq(int page)
    {
        Table.HistoryTableReq history = new HistoryTableReq();
        history.page = page;
        NetClient.Instance().WriteMsg("Table.HistoryTableReq", history);
    }

    public static void historyDetailReq(int id)
    {
        Table.HistoryTableDetailReq history = new HistoryTableDetailReq();
        history.index = id;
        NetClient.Instance().WriteMsg("Table.HistoryTableDetailReq", history);
    }

    public static void cancelAuto()
    {
        Table.CancelAutoReq auto = new CancelAutoReq();
        NetClient.Instance().WriteMsg("Table.CancelAutoReq", auto);
    }
    public static void readyNext()
    {
        NetClient.Instance().WriteMsg("Table.ReadyNextReq", new Table.ReadyNextReq());
    
    }

    public static void changeAuto()
    {
        NetClient.Instance().WriteMsg("Table.ChangeAutoReq", new Table.ChangeAutoReq());
    }

    public static void purchaseReq(int num)
    {
        Table.IOS_PurchaseReq req = new IOS_PurchaseReq();
        req.purchase_id = num;
        NetClient.Instance().WriteMsg("Table.IOS_PurchaseReq", req);
    }

    public static void voteQuitReq(bool boo)
    {
        Table.VoteQuitReq req = new VoteQuitReq();
        req.yes = boo;
        NetClient.Instance().WriteMsg("Table.VoteQuitReq", req);
    }

    public static void startVoteReq()
    {
        Table.StartVoteReq req = new StartVoteReq();
        NetClient.Instance().WriteMsg("Table.StartVoteReq", req);
    }

    public static void canHu()
    {
        Table.CanHu req = new CanHu();
        NetClient.Instance().WriteMsg("Table.CanHu", req);
    }
    #region GM相关
    /// <summary>
    /// 停开服
    /// </summary>
    /// <param name="boo"></param>
    public static void GMStopServer(bool boo)
    {
        Table.GMStopServerReq server = new GMStopServerReq();
        server.stop = boo;
        NetClient.Instance().WriteMsg("Table.GMStopServerReq", server);
    }
    /// <summary>
    /// 查看单个用户
    /// </summary>
    /// <param name="id"></param>
    public static void GMQuerySinglePlayer(int id)
    {
        GMQuerySinglePlayerReq player = new GMQuerySinglePlayerReq();
        player.id = id;
        NetClient.Instance().WriteMsg("Table.GMQuerySinglePlayerReq", player);
    }
    /// <summary>
    /// 查看所有代理人
    /// </summary>
    public static void GMQueryAgentList()
    {
        NetClient.Instance().WriteMsg("Table.GMQueryAgentListReq", new GMQueryAgentListReq());
    }
    /// <summary>
    /// 查看金币大于多少的玩家
    /// </summary>
    /// <param name="money"></param>
    public static void GMQueryCoinPlayer(int money)
    {
        GMQueryPlayerListReq player = new GMQueryPlayerListReq();
        player.money = money;
        NetClient.Instance().WriteMsg("Table.GMQueryPlayerListReq", player);
    }
    /// <summary>
    /// 查询某人id的加钱操作日志
    /// </summary>
    /// <param name="id"></param>
    /// <param name="boo">主动还是被动加钱</param>
    public static void GMQueryMoneyLog(int id, bool boo, int index = 0)
    {
        GMQueryMoneyLogReq money = new GMQueryMoneyLogReq();
        money.id = id;
        money.is_active = boo;
        money.startindex = index;
        NetClient.Instance().WriteMsg("Table.GMQueryMoneyLogReq", money);
    }
    /// <summary>
    /// 根据流水号，查看单条流水日志
    /// </summary>
    /// <param name="index"></param>
    public static void GMQuerySingleGMLog(int index)
    {
        GMQuerySingleGMLogReq log = new GMQuerySingleGMLogReq();
        log.index = index;
        NetClient.Instance().WriteMsg("Table.GMQuerySingleGMLogReq",log);
    }
    /// <summary>
    /// 给玩家增加金币
    /// </summary>
    /// <param name="id"></param>
    /// <param name="coin"></param>
    public static void addCoin(int id, int coin)
    {
        Table.GMAddMoneyReq mon = new GMAddMoneyReq();
        mon.id = id;
        mon.value = coin;
        NetClient.Instance().WriteMsg("Table.GMAddMoneyReq", mon);
    }
    /// <summary>
    /// 添加代理
    /// </summary>
    /// <param name="id"></param>
    /// <param name="boo"></param>
    public static void addAgent(int id, bool boo)
    {
        GMAddAgentReq agent = new GMAddAgentReq();
        agent.id = id;
        agent.yes = boo;
        NetClient.Instance().WriteMsg("Table.GMAddAgentReq", agent);
    }
    public static void GMQuerySysInfo(string password)
    {
        GMQuerySysInfoReq info = new GMQuerySysInfoReq();
        info.password = password;
        NetClient.Instance().WriteMsg("Table.GMQuerySysInfoReq", info);
    }
    public static void GMDesRoom(int roomId)
    {
        GMDestroyRoomReq room = new GMDestroyRoomReq();
        room.tabid = roomId;
        NetClient.Instance().WriteMsg("Table.GMDestroyRoomReq", room);
    }
    #endregion

   
}
