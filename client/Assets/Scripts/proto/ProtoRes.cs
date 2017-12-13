using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NetWork;
using System;

public class ProtoRes:Singleton<ProtoRes>{
	    float time = 0;
		
		
    public delegate void ON_RES(NetWork.Msg msg);
    public ON_RES onRes;
    public Dictionary<string, ON_RES> dic;
    public ProtoRes()
    {
        dic = new Dictionary<string, ON_RES>();
        addRes();
        GlobleTimer.Instance.addUpdate(delegate (float dt)
          {
              onUpdate();
          });
    }

    private void addRes()
    {
        dic.Add("Login.CheckVerRsp", versionChecked);
        dic.Add("Login.IOS_RegistRsp", registRsp);
        dic.Add("Login.LoginRsp", login_rsp);
       
        dic.Add("Room.RoomListRsp", roomList_rsp);
        dic.Add("Room.EnterRsp", enterRoom_rsp);
        dic.Add("Room.leaveRsp", leaveRoom_rsp);

        dic.Add("Table.CreateRsp", createTable_rsp);
        dic.Add("Table.Join", joined);
        dic.Add("Table.JoinRsp", joinTable_rsp);
        dic.Add("Table.ReJoinRsp", tableRejoin);
        dic.Add("Table.QuitRsp", quit_rsp);
        dic.Add("Table.Ready", ready_rsp);
        dic.Add("Table.Start", start);
        dic.Add("Table.Cards", get_cards);
        dic.Add("Table.Turn", turn);
        dic.Add("Table.Play", play);
        dic.Add("Table.Gang", gang);
        dic.Add("Table.Peng", peng);
        dic.Add("Table.Pass", pass);
        dic.Add("Table.NewCard", newCard);
        dic.Add("Table.Angang",anGang);
        dic.Add("Table.Hu", hu);

        dic.Add("Table.RoundScore", roundScore);

        dic.Add("Table.VoiceChat", voiceChat);
        dic.Add("Table.QuickMsg", quickMsg);
        dic.Add("Table.JiangMa", jiangMa);
        dic.Add("Table.GameEnd", gameEnd);

        dic.Add("Table.PlayerRecords", record);

        dic.Add("Table.SysErrorRsp", SysErrorRsp);
        dic.Add("Table.MaiMa", MaiMaRes);
        dic.Add("Table.MoneyChange", MoneyChanged);

        //dic.Add("Table.GMMsgRsp", GMRsp);

        dic.Add("Table.HistoryTableRsp", HistoryTableRsp);
        dic.Add("Table.HistoryTableDetailRsp", HistoryDetailRsp);

        dic.Add("Table.SwitchAuto", SwitchAuto);
        dic.Add("Table.ReadyNextRsp", readyNext);

        dic.Add("Table.GMAddMoneyRsp", GMAddMoney);
        dic.Add("Table.GMMoneyLogRsp", GMMoneyRsp);
        dic.Add("Table.GMPlayerInfoRsp", GMPlayerRsp);
        dic.Add("Table.GMQuerySysInfoRsp", GMQuerySysInfoRsp);

        dic.Add("Table.PassCountRsp", passCount);

        dic.Add("Table.VoteQuitRsp", voteQuitRsp);
        dic.Add("Table.StartVoteRsp",startVoteRsp);
        dic.Add("Table.ShowOfflineRsp", showOfflineRsp);
    }

    public void onUpdate()
    {
        NetWork.Msg msg = NetClient.Instance().PeekMsg();
        if (msg == null)
            return;
        Debug.Log("收到协议" + msg.name);
        if (dic.ContainsKey(msg.name))
            dic[msg.name].Invoke(msg);
    }

    public void invokeFun(Msg msg)
    {
        if (dic.ContainsKey(msg.name))
            dic[msg.name].Invoke(msg);
    }

    #region login

    private void versionChecked(Msg msg)
    {
        EventDispatcher.Instance.Dispatch<bool,bool>(GameEventConst.VERSION, ((Login.CheckVerRsp)(msg.body)).version_match, ((Login.CheckVerRsp)(msg.body)).guest_mode);
    }

    private void registRsp(Msg msg)
    {
        Login.IOS_RegistRsp rsp = (Login.IOS_RegistRsp)msg.body;
        if (rsp.errno == 0)
        {
            EventDispatcher.Instance.Dispatch(GameEventConst.REGISTED);
            QuickTips.ShowQuickTipsNor("注册成功");
        }
        else
        {
            QuickTips.ShowQuickTipsNor("注册失败,用户已存在");
        }
    }
    private void login_rsp(Msg msg)
    {
        //登录返回
        Debug.LogError("登录返回");
        Login.LoginRsp rsp = (Login.LoginRsp)msg.body;
		if (rsp.err_no == 0)
			LoginController.Instance.LoginBack ((Login.LoginRsp)msg.body);
		else if (rsp.err_no == 2) {
			AlertMgr.Instance.showAlert (ALERT_TYPE.type3, "请到官网 http://27toy.com/ 下载最新版本", delegate () {
				Application.OpenURL ("http://27toy.com/");
			});
		} else if(rsp.err_no == 1) {
			//QuickTips.ShowQuickTipsNor ("用户名密码错误，请重新输入");
			AlertMgr.Instance.showAlert(ALERT_TYPE.type2, "用户密码错误", delegate ()
				{
					
				});
		}
    }
    #endregion

    #region room
    private void roomList_rsp(Msg msg)
    {
        //RoomController.Instance.InitRoomList((Room.RoomListRsp)msg.body);
    }
    private void enterRoom_rsp(Msg msg)
    {
		GlobleTimer.Instance.removeUpdate(addTimer);
		GlobleTimer.Instance.addUpdate(addTimer);
        if (((Room.EnterRsp)msg.body).err_no == 0)
        {
            int tableId = ((Room.EnterRsp)msg.body).tab_id;
            if (tableId > 0)
            {
                //AlertMgr.Instance.showAlert(ALERT_TYPE.type1,"是否重连到桌子："+ tableId+"的对局", delegate{
                //    ProtoReq.ReJoinTable(tableId);
                //});
                QuickTips.ShowQuickTipsNor("重连到桌子：" + tableId + "的对局");
                ProtoReq.ReJoinTable(tableId);
            }
            else
            {
                //HallPanel.Instance.load();
            }
        }
        //RoomController.Instance.enteredRoom((Room.EnterRsp)msg.body);
        else
        {
            //HallPanel.Instance.load();
        }
    }

    private void leaveRoom_rsp(Msg msg)
    {
        Debug.Log("收到Room.leaveRsp");
    }
    #endregion

    #region table
    private void createTable_rsp(Msg msg)
    {
        TableController.Instance.createdTable((Table.CreateRsp)msg.body);
    }
    private void joinTable_rsp(Msg msg)
    {
        TableController.Instance.joinedTable((Table.JoinRsp)msg.body);
    }
    private void tableRejoin(Msg msg)
    {
        TableController.Instance.rejoinTable((Table.ReJoinRsp)msg.body);
    }
    private void joined(Msg msg)
    {
        TableController.Instance.joined((Table.Join)msg.body);
    }
    private void ready_rsp(Msg msg)
    {
        TableController.Instance.ready((Table.Ready)msg.body);
    }
    private void start(Msg msg)
    {
        TableController.Instance.start((Table.Start)msg.body);
        DataMgr.Instance._readyNextIDs.Clear();
        GameConst.VoteTimer = 0;
    }
    private void get_cards(Msg msg)
    {
        TableController.Instance.getCards((Table.Cards)msg.body);
    }

    private void turn(Msg msg)
    {
        TableController.Instance.turn((Table.Turn)msg.body);
    }

    private void play(Msg msg)
    {
        TableController.Instance.play((Table.Play)msg.body);

    }
    private void peng(Msg msg)
    {
        TableController.Instance.peng((Table.Peng)msg.body);
    }
    private void gang(Msg msg)
    {
        TableController.Instance.gang((Table.Gang)msg.body);

    }

    private void pass(Msg msg)
    {
        TableController.Instance.pass((Table.Pass)msg.body);

    }
    private void newCard(Msg msg)
    {
        TableController.Instance.newCard((Table.NewCard)msg.body);

    }

    private void anGang(Msg msg)
    {
        //TableController.Instance.anGang((Table.Angang)msg.body);
    }

    private void quit_rsp(Msg msg)
    {
        TableController.Instance.quitTable((Table.QuitRsp)msg.body);
    }

    private void hu(Msg msg)
    {
        TableController.Instance.hu((Table.Hu)msg.body);
    }

    private void roundScore(Msg msg)
    {
        TableController.Instance.roundScore((Table.RoundScore)msg.body);
    }
    private void voiceChat(Msg msg)
    {
        VoiceMsgController.Instance.voiceChat((Table.VoiceChat)msg.body);
    }
    private void quickMsg(Msg msg)
    {
        VoiceMsgController.Instance.quickMsg((Table.QuickMsg)msg.body);
    }
    private void jiangMa(Msg msg)
    {
        TableController.Instance.jiangMa((Table.JiangMa)msg.body);
    }

    private void gameEnd(Msg msg)
    {
        GameConst.auto.SetActive(false);
        TableController.Instance.gameEnd((Table.GameEnd)msg.body);
    }
    private void record(Msg msg)
    {
        TableController.Instance.record((Table.PlayerRecords)msg.body);
    }

    private void SysErrorRsp(Msg msg)
    {
        QuickTips.ShowQuickTipsNor(((Table.SysErrorRsp)msg.body).err_info);
    }
    
    private void MaiMaRes(Msg msg)
    {
        TableController.Instance.mainMa((Table.MaiMa)msg.body);
    }

    private void MoneyChanged(Msg msg)
    {
        DataMgr.Instance.moneyChanged((Table.MoneyChange)msg.body);
        EventDispatcher.Instance.Dispatch(GameEventConst.COIN_CHANGED);
    }

    private void HistoryTableRsp(Msg msg)
    {
        HistoryController.Instance.history((Table.HistoryTableRsp)msg.body);
    }
    private void HistoryDetailRsp(Msg msg)
    {
        HistoryController.Instance.historyDetail((Table.HistoryTableDetailRsp)msg.body);
    }

    private void SwitchAuto(Msg msg)
    {
        TableController.Instance.switchAuto((Table.SwitchAuto)msg.body);
    }
    private void readyNext(Msg msg)
    {
        TableController.Instance.nextReady((Table.ReadyNextRsp)msg.body);
    }
    #endregion

    #region GM
    private void  GMAddMoney(Msg msg)
    {
        Table.GMAddMoneyRsp rsp = (Table.GMAddMoneyRsp)msg.body;

        List<string> list = new List<string>();
        list.Add(rsp.err_no==0?"成功":"失败");

    }
     private void GMMoneyRsp(Msg msg)
     {
        Table.GMMoneyLogRsp rsp = (Table.GMMoneyLogRsp)msg.body;
        List<string> list = new List<string>();
        for (int i = 0; i < rsp.money_log_list.Count; i++)
        {
            list.Add("流水号：" + rsp.money_log_list[i].index + ";"
                + "id:" + rsp.money_log_list[i].fromid.IdEx() + "名字:" + rsp.money_log_list[i].fromname + "给"
                + "id:" + rsp.money_log_list[i].toid.IdEx() + "名字:" + rsp.money_log_list[i].toname + "加了"
                + rsp.money_log_list[i].value + "个钻石" + "时间"+rsp.money_log_list[i].optime);
        }
        EventDispatcher.Instance.Dispatch<int,List<string>>(GameEventConst.GM_MONEYLOG,list.Count,list);
        EventDispatcher.Instance.Dispatch<List<Table.GMMoneyLog>>(GameEventConst.DAILI_MONEY, rsp.money_log_list);
    }
     private void GMPlayerRsp(Msg msg)
     {
        Table.GMPlayerInfoRsp rsp = (Table.GMPlayerInfoRsp)msg.body;
        List<string> list = new List<string>();
        for (int i = 0; i < rsp.player_info_list.Count; i++)
        {
            list.Add("id:" + rsp.player_info_list[i].id.IdEx() + "名字:" + rsp.player_info_list[i].name
                + "钻石:" + rsp.player_info_list[i].total_money + "是否代理：" + rsp.player_info_list[i].is_agent);
        }
        EventDispatcher.Instance.Dispatch<int, List<string>>(GameEventConst.GMPANEL, list.Count, list);
        EventDispatcher.Instance.Dispatch<List<Table.GMPlayerInfo>>(GameEventConst.DAILI_PLAYER, ((Table.GMPlayerInfoRsp)msg.body).player_info_list);
    }

    private void GMQuerySysInfoRsp(Msg msg)
    {
        Table.GMQuerySysInfoRsp rsp = (Table.GMQuerySysInfoRsp)msg.body;
        List<string> list = new List<string>();
        for (int i = 0; i < rsp.tablelist.Count; i++)
        {
            list.Add("桌子号:" + rsp.tablelist[i]);
        }
        EventDispatcher.Instance.Dispatch<int,List<string>>(GameEventConst.GMPANEL,list.Count, list);
    }


    #endregion

    private void  addTimer(float dt)
	{
		  time += dt;
		  if (time > 5)
		  {
			  ProtoReq.heartReq();
              time = 0;
		  }
    }

    private void passCount(Msg msg)
    {
        Table.PassCountRsp rsp = (Table.PassCountRsp)(msg.body);
        EventDispatcher.Instance.Dispatch<string>(GameEventConst.RSP_COUNT, rsp.value);
    }

    private void voteQuitRsp(Msg msg)
    {
        Table.VoteQuitRsp rsp = (Table.VoteQuitRsp)(msg.body);
        string str = rsp.yes ? "同意解散房间" : "不同意解散房间";
        QuickTips.ShowQuickTipsNor(RoleController.Instance.getPlayerById(rsp.id).Name + str);
    }

    private void startVoteRsp(Msg msg)
    {
        Table.StartVoteRsp rsp = (Table.StartVoteRsp)(msg.body);
        AlertMgr.Instance.showAlert(ALERT_TYPE.type1, RoleController.Instance.getPlayerById(rsp.id).Name + "发起解散房间，是否同意", delegate
        {
            GameConst.VoteTimer = Time.time;
            ProtoReq.voteQuitReq(true);

        },delegate {
            GameConst.VoteTimer = Time.time;
            ProtoReq.voteQuitReq(false);
        },15);
    }

    private void showOfflineRsp(Msg msg)
    {
        Table.ShowOfflineRsp rsp = (Table.ShowOfflineRsp)(msg.body);
        EventDispatcher.Instance.Dispatch<int, bool>(GameEventConst.PLAYER_OUT, rsp.id, rsp.yes);
    }
}
