using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Table;
using System.Threading;

public class TableController :Singleton<TableController> {


    public void getCards(Table.Cards cards)
    {
        Debug.Log("收到牌");
        CardController.Instance.cleanUp();
        for (int i = 0; i < cards.cards.Count; i++)
        {
            CardController.Instance.addCard(CardConst.getCardInfo(cards.cards[i]).type, CardConst.getCardInfo(cards.cards[i]).value);
        }
        EventDispatcher.Instance.Dispatch(GameEventConst.CARD_TO_HAND);
    }

    public void createdTable(Table.CreateRsp create)
    {
        Debug.Log("桌子号" + create.tab_id);
        GameConst.tableId = create.tab_id;
        // RoomPanel.Instance.load(); 
        ProtoReq.JoinTable(create.tab_id);
    }

    public void joinedTable(Table.JoinRsp join)
    {
        Debug.Log("JoinRsp");
        
        if (join.err_no == 0)
        {
            RoleController.Instance.clear();
            GameConst.ower = join.owner;
            DataMgr.Instance.sumRound = join.rule.totalround;
            DataMgr.Instance.maxbet = join.rule.maxbet;
            DataMgr.Instance.jiangma = join.rule.jiangma;
            DataMgr.Instance.maima = join.rule.maima;
            ProtoReq.Ready();
            for (int i = 0; i < join.roles.Count; i++)
            {
                RoleController.Instance.addPlayer(join.roles[i]);

                Debug.Log("role" + i + join.roles[i].name);
            }
            RoomPanel.Instance.load();
        }
        else if (join.err_no == 1)
        {
            QuickTips.ShowQuickTipsNor("房间不存在");
        }
        else if (join.err_no == 2)
        {
            QuickTips.ShowQuickTipsNor("人数已满");
        }
        else if (join.err_no == 3)
        {
            QuickTips.ShowQuickTipsNor("已经在座位上，可能别处登录");
        }
        else if (join.err_no == 4)
        {
            QuickTips.ShowQuickTipsNor("没有适合座位了");
        }
        
    }

    public void rejoinTable(Table.ReJoinRsp reJoin)
    {
        if (reJoin.err_no == 0)
        {
            RoleController.Instance.clear();
            GameConst.tableId = reJoin.tab_id;
            GameConst.ower = reJoin.owner;
            DataMgr.Instance.sumRound = reJoin.rule.totalround;
            DataMgr.Instance.maxbet = reJoin.rule.maxbet;
            DataMgr.Instance.jiangma = reJoin.rule.jiangma;
            DataMgr.Instance.maima = reJoin.rule.maima;
            if (reJoin.cur_id == MainRole.Instance.Id)
            {
                GameConst.isTurn = true;
            }
            else
            {
                GameConst.isTurn = false;
            }
             
            foreach (var item in reJoin.roles)
            {
                RoleController.Instance.addPlayer(item);
            }
            if (reJoin.is_playing==1)
            {
                GameConst.curId = reJoin.cur_id;
                GameConst.curCard = reJoin.cur_card;
                GameConst.isPlay = true;
                DataMgr.Instance.reSet();
                DataMgr.Instance.curRound = reJoin.cur_round;
                DataMgr.Instance.leftCardNum = reJoin.left_card;
               
                foreach (var item in reJoin.player_record)
                {
                    if (DataMgr.Instance._showScore.ContainsKey(item.id))
                        DataMgr.Instance._showScore[item.id] = item.total_score;
                    else
                        DataMgr.Instance._showScore.Add(item.id, item.total_score);
                }

                foreach (var item in reJoin.end_cards)
                {
                    if (item.id == MainRole.Instance.Id)
                    {
                        CardController.Instance.cleanUp();
                        bool flag = false;
                        for (int i = 0; i < item.cards.Count; i++)
                        {

                            if (reJoin.cur_id == MainRole.Instance.Id)
                            {
                                if (reJoin.cur_card == item.cards[i] && !flag)
                                {
                                    flag = true;
                                    continue;
                                }
                            }
                            Card card = new Card(item.cards[i]);
                            CardController.Instance.addCard(card.CardType, card.CardNum);
                        }
                    }
                    else
                    {
                        switch (item.id.idToPos())
                        {
                            case 1:
                                DataMgr.Instance.rightCardNum = item.cards.Count;
                                break;
                            case 2:
                                DataMgr.Instance.topCardNum = item.cards.Count;
                                break;
                            case 3:
                                DataMgr.Instance.leftCardNum = item.cards.Count;
                                break;
                        }
                    }

                    for (int i = 0; i < item.outcards.Count; i++)
                    {
                        DataMgr.Instance._heCards[item.id.idToPos()].Add(item.outcards[i]);
                    }
                    for (int i = 0; i < item.pgdata.Count; i++)
                    {
                        if (item.pgdata[i].ptype == 0)
                        {
                            DataMgr.Instance._everyPeng[item.id.idToPos()].Add(item.pgdata[i].card, new PengData(item.id.idToPos(), item.pgdata[i].from.idToPos(), item.pgdata[i].card, null));
                        }
                        else
                        {
                            if(item.pgdata[i].ptype == 3)
                                 DataMgr.Instance._everyGang[item.id.idToPos()].Add(item.pgdata[i].card, new GangData(item.id.idToPos(), item.id.idToPos(), item.pgdata[i].card,true, null));
                            else
                                DataMgr.Instance._everyGang[item.id.idToPos()].Add(item.pgdata[i].card, new GangData(item.id.idToPos(), item.pgdata[i].from.idToPos(), item.pgdata[i].card, false, null));
                        }

                    }
                }
                //EventDispatcher.Instance.Dispatch(GameEventConst.RECONNECT, true);
                RoomPanel.Instance.load(1);
            }
            else
            {
                if(reJoin.is_playing==0)
                    ProtoReq.Ready();
                //EventDispatcher.Instance.Dispatch(GameEventConst.RECONNECT, false);
                RoomPanel.Instance.load(reJoin.err_no);
            }
            SoundMgr._instance.bgmPlay("beijing-fangjian" + GameConst.BGM, GameConst.musicVol);
        }
        
        
    }
    public void joined(Table.Join join)
    {
        Debug.Log("Join");
        RoleController.Instance.addPlayer(join.role);
        EventDispatcher.Instance.Dispatch<Table.Role>(GameEventConst.ADD_PLAYER,join.role);
    }

    public void ready(Table.Ready ready)
    {
        RoleController.Instance.getPlayerById(ready.id).IsReady = true;
        EventDispatcher.Instance.Dispatch(GameEventConst.READY_TO_PALY,ready.id);
    }

    public void turn(Table.Turn turn)
    {
        Debug.Log("turn的回合");
        if (turn.id == MainRole.Instance.Id)
        {
            EventDispatcher.Instance.Dispatch(GameEventConst.TURN_TO, true,turn.id);
        }
        else
        {
            EventDispatcher.Instance.Dispatch(GameEventConst.TURN_TO, false,turn.id);
        }
    }
    public void start(Table.Start start)
    {
        Debug.Log("收到start协议");
        DataMgr.Instance.zhuangId = start.id;
        DataMgr.Instance.curRound = start.round;
        GameConst.startStatus = start.status;
        if (start.status == 1)
        {
            DataMgr.Instance.reSet();
            DataMgr.Instance.zhuangId = start.id;
            DataMgr.Instance.curRound = start.round;
            GameConst.isPlay = true;
            //QuickTips.ShowRedQuickTips("开始一局");
            foreach (var item in start.player_record)
            {
                if (DataMgr.Instance._showScore.ContainsKey(item.id))
                    DataMgr.Instance._showScore[item.id] = item.total_score;
                else
                    DataMgr.Instance._showScore.Add(item.id, item.total_score);
            }
            EventDispatcher.Instance.Dispatch(GameEventConst.START);
            
            if (start.round == 1)
            {
                SoundMgr._instance.bgmPlay("beijing-fangjian"+GameConst.BGM,GameConst.musicVol);
            }

        }
        if (start.status == 2)
        {
            //QuickTips.ShowRedQuickTips("结束一局");
            GameConst.isPlay = false;
            EventDispatcher.Instance.Dispatch(GameEventConst.END);
        }
        if (start.status == 3)
        {
            //QuickTips.ShowRedQuickTips("流局");
            GameConst.isPlay = false;
            EventDispatcher.Instance.Dispatch(GameEventConst.HUANGJU);
        }
    }

    public void nextReady(ReadyNextRsp body)
    {
        if (!DataMgr.Instance._readyNextIDs.Contains(body.id))
            DataMgr.Instance._readyNextIDs.Add(body.id);
        EventDispatcher.Instance.Dispatch(GameEventConst.READY_NEXT,body.id);
    }

    public void play(Table.Play play)
    {
        Debug.Log("收到play返回");
        if (play.err_no == 0)
        {
            switch (play.id.idToPos())
            {
                case 0:
                    GameConst.isTurn = false;
                    CardController.Instance.cleanUp();
                    for (int i = 0; i < play.leftcard.Count; i++)
                    {
                        CardController.Instance.addCard(CardConst.getCardInfo(play.leftcard[i]).type, CardConst.getCardInfo(play.leftcard[i]).value);
                    }
                    DataMgr.Instance._heCards[0] = play.outcards;
                    break;
                case 1:
                    DataMgr.Instance.rightCardNum = play.leftcard.Count;
                    DataMgr.Instance._heCards[1] = play.outcards;
                    break;
                case 2:
                    DataMgr.Instance.topCardNum = play.leftcard.Count;
                    DataMgr.Instance._heCards[2] = play.outcards;
                    break;
                case 3:
                    DataMgr.Instance.leftCardNum = play.leftcard.Count;
                    DataMgr.Instance._heCards[3] = play.outcards;
                    break;
            }
          
            SoundMgr._instance.soundPlay(GameConst.Language + ((RoleController.Instance._playerDic[play.id].Sex?0:1000) + play.card),GameConst.soundVol);
            DataMgr.Instance._curCard = play.card;
            Debug.Log("现在的cur" + play.card);
            EventDispatcher.Instance.Dispatch(GameEventConst.PUT_HE_CARD, play.id.idToPos(), play.card);
        }
        else
        {
            //QuickTips.ShowRedQuickTips("play___" + play.err_no);
        }
       
    }

    public void gang(Table.Gang gang)
    {
        if (gang.err_no == 0)
        {
            if (gang.id == MainRole.Instance.Id)
            {
                CardController.Instance.cleanUp();
                for (int i = 0; i < gang.leftcard.Count; i++)
                {
                    CardController.Instance.addCard(gang.leftcard[i]);
                }
            }
            DataMgr.Instance._curCard = gang.card;
            if (gang.from == 0)
            {
                SoundMgr._instance.soundPlay("bugang_" + (RoleController.Instance.getPlayerById(gang.id).Sex ? "0" : "1") + "_" + GameConst.Language, GameConst.soundVol);
                EventDispatcher.Instance.Dispatch(GameEventConst.GANG, gang.id.idToPos(), gang.id.idToPos(), gang.card,true);
            }
            else
            {
                
                EventDispatcher.Instance.Dispatch(GameEventConst.GANG, gang.id.idToPos(), gang.from.idToPos(), gang.card,false);
                if (gang.id == gang.from)
                {
                    SoundMgr._instance.soundPlay("angang_" + (RoleController.Instance.getPlayerById(gang.id).Sex ? "0" : "1") + "_" + GameConst.Language, GameConst.soundVol);
                }
                else
                {
                    SoundMgr._instance.soundPlay("minggang_" + (RoleController.Instance.getPlayerById(gang.id).Sex ? "0" : "1") + "_" + GameConst.Language, GameConst.soundVol);
                }
            }
           
        }
    }

    public void peng(Table.Peng peng)
    {
        if (peng.err_no == 0)
        {
            if (peng.id == MainRole.Instance.Id)
            {
                CardController.Instance.cleanUp();
                for (int i = 0; i < peng.leftcard.Count; i++)
                {
                    CardController.Instance.addCard(peng.leftcard[i]);
                }
                DataMgr.Instance._curCard = peng.nextcard;
            }
            EventDispatcher.Instance.Dispatch(GameEventConst.PENG, peng.id.idToPos(), peng.from.idToPos(), peng.card);
            SoundMgr._instance.soundPlay("peng_" + (RoleController.Instance.getPlayerById(peng.id).Sex ? "0" : "1") + "_" + GameConst.Language,GameConst.soundVol);
        }
    }
    public void pass(Table.Pass pass)
    {
        Debug.Log("收到pass");
    }

    public void newCard(Table.NewCard card)
    {
        Debug.Log("摸牌"+card.card);
        switch (card.id.idToPos())
        {
            case 0:
                if (card.myleftcard.Count != 0)
                {
                    CardController.Instance.cleanUp();
                    for (int i = 0; i < card.myleftcard.Count; i++)
                    {
                        CardController.Instance.addCard(card.myleftcard[i]);
                    }
                }
                break;
            case 1:
                //DataMgr.Instance.rightCardNum += 1;
                break;
            case 2:
                //DataMgr.Instance.topCardNum += 1;
                break;
            case 3:
               //DataMgr.Instance.leftCardNum += 1;
                break;
        }
        DataMgr.Instance.lassCardsNum = card.leftcard;
        EventDispatcher.Instance.Dispatch<int,int>(GameEventConst.GET_NEW_CARD,card.id,card.card);
    }

    public void anGang(Table.Angang gang)
    {
        Debug.Log("暗杠");
        EventDispatcher.Instance.Dispatch<int,int>(GameEventConst.AN_GANG, gang.id.idToPos(), gang.card);
    }

    public void quitTable(Table.QuitRsp quit)
    {

        Debug.Log("退出");
        if (quit.quit_type == 3)
        {
            //QuickTips.ShowRedQuickTips("3---房间解散");
            //RoomPanel.Instance.DestoryPanel();
            AlertMgr.Instance.close();
            HallPanel.Instance.load();
            SoundMgr._instance.bgmPlay("beijing_dating" + GameConst.BGM, GameConst.musicVol);
            RoleController.Instance._playerDic.Clear();
            return;
        }
           

        if (quit.id == MainRole.Instance.Id)
        {
            //RoomPanel.Instance.DestoryPanel();
            AlertMgr.Instance.close();
            HallPanel.Instance.load();
            SoundMgr._instance.bgmPlay("beijing_dating" + GameConst.BGM, GameConst.musicVol);
            RoleController.Instance._playerDic.Clear();
        }
        else
        {
            EventDispatcher.Instance.Dispatch<int>(GameEventConst.SOMEOME_QUIT,quit.id);
        }
       
    }
    public void hu(Table.Hu hu)
    {
        if (hu.err_no == 0)
        {
            //QuickTips.ShowRedQuickTips(hu.id+"胡了");
            EventDispatcher.Instance.Dispatch<int,int,List<int>,int,int>(GameEventConst.HU,hu.id,hu.from,hu.cards,hu.card,hu.exttype);
            if (hu.id == hu.from)
            {
                SoundMgr._instance.soundPlay("zimo_" + (RoleController.Instance.getPlayerById(hu.id).Sex ? "0" : "1") + "_" + GameConst.Language,GameConst.soundVol);

            }
            else
            {
                SoundMgr._instance.soundPlay("chihu_" + (RoleController.Instance.getPlayerById(hu.id).Sex ? "0" : "1") + "_" + GameConst.Language,GameConst.soundVol);
            }
        }
           
        //else
            //QuickTips.ShowRedQuickTips("hu___" + hu.err_no);
    }

    public void roundScore(Table.RoundScore score)
    {
        DanjuController.Instance.clear();
        DanjuController.Instance.clearPgData();
        DataMgr.Instance.clearLeftCards();
        DataMgr.Instance.clearPutCards();
        for (int i = 0; i < score.end_cards.Count; i++)
        {
            DataMgr.Instance._leftCardsDic[score.end_cards[i].id.idToPos()].AddRange(score.end_cards[i].cards);
            DataMgr.Instance._leftCardsDic[score.end_cards[i].id.idToPos()].Sort();

            DataMgr.Instance._putCardsDic[RoleController.Instance.getPlayerById(score.end_cards[i].id).Pos].AddRange(score.end_cards[i].outcards);
        }

        foreach (Table.Score item in score.scores)
        {
            DanjuController.Instance._scoreDic.Add(item.id.idToPos(), item);

            //处理一堆显示
            if (item.hu == 3)
            {
                if (score.exttype == 1)
                {
                    DanjuController.Instance._scoreType.Add(item.id.idToPos(), DanjuController.Instance._scoreIcon[11]);
                }
                else if (score.exttype == 2)
                {
                    DanjuController.Instance._scoreType.Add(item.id.idToPos(), DanjuController.Instance._scoreIcon[12]);
                }
                else
                {
                    DanjuController.Instance._scoreType.Add(item.id.idToPos(), DanjuController.Instance._scoreIcon[item.hu]);
                }
            }
            else if (item.hu == 2)
            {
                if (score.exttype == 3)
                {
                    DanjuController.Instance._scoreType.Add(item.id.idToPos(), DanjuController.Instance._scoreIcon[13]);
                }
                else
                {
                    DanjuController.Instance._scoreType.Add(item.id.idToPos(), DanjuController.Instance._scoreIcon[item.hu]);
                }
            }
            else if (item.hu == 1)
            {
                if (score.exttype == 4)
                {
                    DanjuController.Instance._scoreType.Add(item.id.idToPos(), DanjuController.Instance._scoreIcon[14]);
                }
                else
                {
                    DanjuController.Instance._scoreType.Add(item.id.idToPos(), DanjuController.Instance._scoreIcon[item.hu]);
                }
            }
            else
            {
                DanjuController.Instance._scoreType.Add(item.id.idToPos(), DanjuController.Instance._scoreIcon[item.hu]);
            }
            


        }

        foreach (Table.EndCards item in score.end_cards)
        {
            foreach (Table.PGData data in item.pgdata)
            {

                DanjuController.Instance._pgDataDic[item.id.idToPos()].Add(data);
            }
           
        }
        DanjuController.Instance.setRoundScore(score);
        EventDispatcher.Instance.Dispatch(GameEventConst.ROUND_SCORE);
    }

    public void jiangMa(Table.JiangMa jiangma)
    {
        DanjuController.Instance._jiangma.Clear();
        DanjuController.Instance._lastCard.Clear();
        DanjuController.Instance._lastCard.AddRange(jiangma.cardsAll);
        DanjuController.Instance._jiangma.AddRange(jiangma.cards);
    }
    public void mainMa(Table.MaiMa mm)
    {
        DanjuController.Instance.clearMainMa();
        DanjuController.Instance._lastCard.Clear();
        DanjuController.Instance._jiangma.Clear();
        DanjuController.Instance._lastCard.AddRange(mm.cardsAll);
        foreach (var item in mm.maima_list)
        {
            DanjuController.Instance._maima[item.id.idToPos()].Add(item.card1);
            DanjuController.Instance._maima[item.id.idToPos()].Add(item.card2);
        }
        //买马分数的合并
        DanjuController.Instance.clearMaiMaScore();
        foreach (var item in mm.maima_score1)
        {
            DanjuController.Instance._maimaScore[item.id.idToPos()].Add(item);
        }
        foreach (var item in mm.maima_score2)
        {
            DanjuController.Instance._maimaScore[item.id.idToPos()].Add(item);
        }


        EventDispatcher.Instance.Dispatch(GameEventConst.MAINMA_DATA);
         
    }
    public void gameEnd(Table.GameEnd end)
    {
        //QuickTips.ShowRedQuickTips("gameEnd");
        //HallPanel.Instance.load();
        //RoomPanel.Instance.DestoryPanel();

        for (int i = 0; i < end.player_records.Count; i++)
        {
            Table.PlayerRecords record = end.player_records[i];
            JiesuanController.Instance._info[record.id.idToPos()].Clear();
            JiesuanController.Instance._info[record.id.idToPos()].Add("自摸次数:" + record.zm_count);
            JiesuanController.Instance._info[record.id.idToPos()].Add("吃胡次数:" + record.hp_count);
            JiesuanController.Instance._info[record.id.idToPos()].Add("点炮次数:" + record.dp_count);
            JiesuanController.Instance._info[record.id.idToPos()].Add("暗杠次数:" + record.ag_count);
            JiesuanController.Instance._info[record.id.idToPos()].Add("明杠次数:" + record.mg_count);
            JiesuanController.Instance._info[record.id.idToPos()].Add("中马次数:" + record.mm_count);
            JiesuanController.Instance._info[record.id.idToPos()].Add("总成绩:" + record.total_score);

        }
        //GlobleTimer.Instance.SetTimer(GameConst.jiesuan,1, delegate
        //{
        //    DanjuPanel.Instance.ClosePanel();
        //    JiesuanPanel.Instance.load(end.index);
        //});
        Timer time = new Timer(delegate
        {
            Loom.QueueOnMainThread(() => {
                DanjuPanel.Instance.ClosePanel();
                JiesuanPanel.Instance.load(end.index);
            });
               
        }, this, GameConst.jiesuan, 0);
    }

    public void record(Table.PlayerRecords record)
    {
        //QuickTips.ShowRedQuickTips("收到总结算");
    }

    public void switchAuto(Table.SwitchAuto auto)
    {
        if (auto.cur_id == MainRole.Instance.Id)
        {
            GameConst.isTurn = true;
        }
        else
        {
            GameConst.isTurn = false;
        }
        
        EventDispatcher.Instance.Dispatch<int,bool>(GameEventConst.AUTO, auto.id, auto.isauto);
       
    }
}


public class PengData {
    public int pos;
    public int fromPos;
    public int card;
    public GameObject obj;

    public PengData(int pos, int fromPos, int card, GameObject obj)
    {
        this.pos = pos;
        this.fromPos = fromPos;
        this.card = card;
        this.obj = obj;
    }
}

public class GangData
{
    public int pos;
    public int fromPos;
    public int card;
    public bool isBu;
    public GameObject obj;
    public GangData(int pos, int fromPos, int card,bool isBu, GameObject obj)
    {
        this.pos = pos;
        this.fromPos = fromPos;
        this.card = card;
        this.isBu = isBu;
        this.obj = obj;
    }
}
