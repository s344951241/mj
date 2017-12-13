using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HistoryController:Singleton<HistoryController>{

    public int page;
    public int sumPage;
    public Table.HistoryTable curHistory;
    public List<Player> _hisPlayerDic;
    public Dictionary<int, int> _idPos;
    public List<Table.HistoryTable> tableList;

    public int index;
    public List<Table.RoundScore> roundScoreList;
    public List<Table.HistoryMaiMa> maimaList;
    public List<Table.HistroyJiangMa> jiangmaList;
    public HistoryController()
    {
        _hisPlayerDic = new List<Player>();
        tableList = new List<Table.HistoryTable>();
        roundScoreList = new List<Table.RoundScore>();
        maimaList = new List<Table.HistoryMaiMa>();
        jiangmaList = new List<Table.HistroyJiangMa>();
        _idPos = new Dictionary<int, int>();
    }
    public void history(Table.HistoryTableRsp rsp)
    {
        clearHistory();
        page = rsp.page;
        sumPage = rsp.maxpage;
        tableList = rsp.table_list;
        EventDispatcher.Instance.Dispatch(GameEventConst.HISTORY);
    }

    public void historyDetail(Table.HistoryTableDetailRsp rsp)
    {
        clearDetail();
        index = rsp.index;
        curHistory = getHistoryByIndex(index);
        if (curHistory == null)
        {
            foreach (var pl in RoleController.Instance._playerDic)
            {
                _hisPlayerDic.Add(pl.Value);
                _idPos.Add(pl.Value.Id, pl.Value.Pos);
            }
            _hisPlayerDic.Sort(delegate (Player p1, Player p2)
            {
                return p1.Pos.CompareTo(p2.Pos);
            });
        }
        else
        {
            curHistory.playerinfo_list.Sort(delegate (Table.HistoryTablePlayerinfo a, Table.HistoryTablePlayerinfo b)
            {
                return a.pos.CompareTo(b.pos);
            });

            foreach (var item in curHistory.playerinfo_list)
            {
                _idPos.Add(item.id, item.pos);
            }
        }
       
        roundScoreList = rsp.round_score_list;
        roundScoreList.Sort(delegate (Table.RoundScore a, Table.RoundScore b)
        {
            return a.round_id.CompareTo(b.round_id);
        });
        if (roundScoreList != null && roundScoreList.Count > 0)
        {
            foreach (var item in roundScoreList)
            {
                item.scores.Sort(delegate (Table.Score a, Table.Score b) {
                    return idToPos(a.id).CompareTo(idToPos(b.id));
                });

                item.end_cards.Sort(delegate (Table.EndCards a, Table.EndCards b) {
                    return idToPos(a.id).CompareTo(idToPos(b.id));
                });

                foreach (var cards in item.end_cards)
                {
                    cards.cards.Sort(delegate (int a, int b)
                    {
                        return a.CompareTo(b);
                    });
                }
            }
        }
        //买马
        maimaList = rsp.maima_list;
        maimaList.Sort(delegate (Table.HistoryMaiMa a, Table.HistoryMaiMa b)
        {
            return a.round_id.CompareTo(b.round_id);
        });

        if (maimaList != null && maimaList.Count != 0)
        {
            foreach (var item in maimaList)
            {
                item.maima_list.Sort(delegate (Table.MaiMaDataNew a, Table.MaiMaDataNew b) {
                    return idToPos(a.id).CompareTo(idToPos(b.id));
                });
            }
        }
        //奖马
        jiangmaList = rsp.jiangma_list;
        jiangmaList.Sort(delegate (Table.HistroyJiangMa a, Table.HistroyJiangMa b)
        {
            return a.round_id.CompareTo(b.round_id);
        });
        
        EventDispatcher.Instance.Dispatch(GameEventConst.HISTORY_DETAIL);
        EventDispatcher.Instance.Dispatch(GameEventConst.JIESUAN_DETAIL);
    }

    public Table.HistoryTable getHistoryByIndex(int index)
    {
        Table.HistoryTable history = null;
        for (int i = 0; i < tableList.Count; i++)
        {
            if (tableList[i].index == index)
            {
                history = tableList[i];
            }
        }
        return history;
    }
    private void clearHistory()
    {
        tableList.Clear();
        
    }
    private void clearDetail()
    {
        curHistory = null;
        roundScoreList.Clear();
        maimaList.Clear();
        jiangmaList.Clear();
        _idPos.Clear();
        _hisPlayerDic.Clear();
    }

    public int idToPos(int id)
    {
        if (!_idPos.ContainsKey(id))
            return -1;
        return _idPos[id];
    }
}
