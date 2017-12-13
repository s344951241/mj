using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DanjuController : Singleton<DanjuController> {

    public bool isJiangma;
    public Dictionary<int, List<int>> _maima;
    public Dictionary<int, List<Table.MaiMaScore>> _maimaScore;
    public List<int> _jiangma;
    public List<int> _lastCard;
    public int _maiMaNum;
    public Dictionary<int, List<int>> _jiangmaDic;//自己的pos
    //public Dictionary<int,huType> _huT

    public Dictionary<int, Table.Score> _scoreDic;
    public Dictionary<int, string> _scoreType;

    

    public Dictionary<int, Dictionary<int, List<string>>> _infoDic;

    public Dictionary<int, string> _scoreIcon;
    public Dictionary<int, string> _huInfo;
    public Dictionary<int, string> _huType;

    public Dictionary<int, List<Table.PGData>> _pgDataDic;//自己的pos
    public DanjuController()
    {
        _lastCard = new List<int>();
        _jiangma = new List<int>();
        _maima = new Dictionary<int, List<int>>();
        _maimaScore = new Dictionary<int, List<Table.MaiMaScore>>();
        List<int> list0 = new List<int>();
        List<int> list1 = new List<int>();
        List<int> list2 = new List<int>();
        List<int> list3 = new List<int>();



        _maima.Add(0, list0);
        _maima.Add(1, list1);
        _maima.Add(2, list2);
        _maima.Add(3, list3);

        List<Table.MaiMaScore> list0Scroe = new List<Table.MaiMaScore>();
        List<Table.MaiMaScore> list1Scroe = new List<Table.MaiMaScore>();
        List<Table.MaiMaScore> list2Scroe = new List<Table.MaiMaScore>();
        List<Table.MaiMaScore> list3Scroe = new List<Table.MaiMaScore>();

        _maimaScore.Add(0, list0Scroe);
        _maimaScore.Add(1, list1Scroe);
        _maimaScore.Add(2, list2Scroe);
        _maimaScore.Add(3, list3Scroe);


        _scoreDic = new Dictionary<int, Table.Score>();
        _scoreType = new Dictionary<int, string>();
        _scoreIcon = new Dictionary<int, string>();
        _huInfo = new Dictionary<int, string>();
        _huType = new Dictionary<int, string>();

        _infoDic = new Dictionary<int, Dictionary<int, List<string>>>();

        Dictionary<int, List<string>> dic0 = new Dictionary<int, List<string>>();
        Dictionary<int, List<string>> dic1 = new Dictionary<int, List<string>>();
        Dictionary<int, List<string>> dic2 = new Dictionary<int, List<string>>();
        Dictionary<int, List<string>> dic3 = new Dictionary<int, List<string>>();


        _infoDic.Add(0, dic0);
        _infoDic.Add(1, dic1);
        _infoDic.Add(2, dic2);
        _infoDic.Add(3, dic3);
        setDicForShow();


        _pgDataDic = new Dictionary<int, List<Table.PGData>>();

        List<Table.PGData> pgList0 = new List<Table.PGData>();
        List<Table.PGData> pgList1 = new List<Table.PGData>();
        List<Table.PGData> pgList2 = new List<Table.PGData>();
        List<Table.PGData> pgList3 = new List<Table.PGData>();

        _pgDataDic.Add(0,pgList0);
        _pgDataDic.Add(1, pgList1);
        _pgDataDic.Add(2, pgList2);
        _pgDataDic.Add(3, pgList3);


        _jiangmaDic = new Dictionary<int, List<int>>();

        List<int> listJm0 = new List<int>();
        List<int> listJm1 = new List<int>();
        List<int> listJm2 = new List<int>();
        List<int> listJm3 = new List<int>();

        _jiangmaDic.Add(0, listJm0);
        _jiangmaDic.Add(1, listJm1);
        _jiangmaDic.Add(2, listJm2);
        _jiangmaDic.Add(3, listJm3);

    }
    public void clearJmDic()
    {
        foreach (var item in _jiangmaDic)
        {
            item.Value.Clear();
        }
    }
    public void clearPgData()
    {
        foreach (var item in _pgDataDic)
        {
            item.Value.Clear();
        }
    }
    public void setDicForShow()
    {
        _scoreIcon.Add(0, "");
        _scoreIcon.Add(1, "chihu");
        _scoreIcon.Add(2, "fangpao");
        _scoreIcon.Add(3, "zimo");

        _scoreIcon.Add(11, "hdly");
        _scoreIcon.Add(12, "baogang");
        _scoreIcon.Add(13, "gsp");
        _scoreIcon.Add(14, "qgh");
        _huInfo.Add(1, "吃胡");
        _huInfo.Add(2, "放炮");
        _huInfo.Add(3, "自摸");
        //胡类型，先不加
        //_huType.Add()
    }
    public void clearMainMa()
    {
        foreach (var item in _maima)
        {
            item.Value.Clear();
        }
    }

    public void clearMaiMaScore()
    {
        foreach (var item in _maimaScore)
        {
            item.Value.Clear();
        }
    }

    public void clear()
    {
        foreach (var item in _infoDic)
        {
            item.Value.Clear();
        }
       _scoreDic.Clear();
       _scoreType.Clear();
    }

    public void setRoundScore(Table.RoundScore score)
    {
        clearJmDic();
        string[] lianzhuang = score.lianzhuang.Split(',');
        Debug.Log("连庄："+ lianzhuang);
        int exttype = 0;
        string extStr = "";
        switch (score.exttype)
        {
            case 1:
                exttype = 1;
                extStr = "(海底捞月)";
                break;
            case 2:
                exttype = 2;
                extStr = "(杠上爆)";
                break;
            case 3:
                extStr = "(杠上花)";
                exttype = 3;
                break;
            case 4:
                extStr = "(抢杠胡)";
                exttype = 4;
                break;
        }
        foreach (Table.Score item in score.scores)
        {
            //_scoreDic.Add(item.id.idToPos(), item);
            //_scoreType.Add(item.id.idToPos(), DanjuController.Instance._scoreIcon[item.hu]);

            int index = 0;
            string str = "";
           
            if (item.id == int.Parse(lianzhuang[0]))
            {
                List<string> list = new List<string>();
                list.Add("连庄");
                list.Add("连庄");
                list.Add("+" + (int.Parse(lianzhuang[1]) * 3));
                list.Add("下家对家上家");
                list.Add("+" + (int.Parse(lianzhuang[1]) * 3));
                _infoDic[item.id.idToPos()].Add(index, list);
                index++;
            }
            //胡
            if (item.hu != 0)
            {
                if (item.hu == 2)
                {
                    for (int i = 0; i < score.scores.Count; i++)
                    {
                        if (score.scores[i].hu ==1)
                        {
                            List<string> list = new List<string>();
                            list.Add(_huInfo[item.hu]);
                            list.Add(GameTools.huTypeName(score.scores[i].hu_type));

                            list.Add((-score.scores[i].basescore).ToString());
                            list.Add(getHuFrom(score.scores[i].id.idToPos()));
                            //str = getHuFrom(item.id.idToPos());
                            list.Add(score.scores[i].basescore.ToString());
                            _infoDic[item.id.idToPos()].Add(index, list);
                            index++;
                        }
                    }
                }
                else
                {
                    List<string> list = new List<string>();
                    list.Add(_huInfo[item.hu]);
                
                    if (exttype == 1 || exttype == 2|| exttype==3||exttype==4)
                    {
                        list.Add(GameTools.huTypeName(item.hu_type) + extStr);
                    }
                    else
                        list.Add(GameTools.huTypeName(item.hu_type));
                    list.Add(item.basescore.ToString());
                    list.Add(getHuFrom(item.id.idToPos()));
                    str = getHuFrom(item.id.idToPos());
                    list.Add((-item.basescore).ToString());
                    _infoDic[item.id.idToPos()].Add(index, list);
                    index++;
                }
               
            }

            //扛
            for (int i = 0; i < _pgDataDic[item.id.idToPos()].Count; i++)
            {
                
                Table.PGData data = _pgDataDic[item.id.idToPos()][i];
                if (data.ptype != 0)
                {
                    List<string> listGang = new List<string>();
                    if (data.ptype == 1)
                    {
                        listGang.Add("明扛");
                        listGang.Add(GameTools.cardStr(data.card));
                        listGang.Add("" + 3);
                        listGang.Add(GameTools.playerPosStr(item.id.idToPos(), data.from.idToPos()));
                        listGang.Add("-"+3);
                    }
                    if (data.ptype == 2)
                    {
                        listGang.Add("暗扛");
                        listGang.Add(GameTools.cardStr(data.card));
                        listGang.Add("" + 6);
                        listGang.Add(GameTools.playerPosStr(item.id.idToPos(),data.from.idToPos()));
                        listGang.Add("-"+6);
                    }
                    if (data.ptype == 3)
                    {
                        listGang.Add("补扛");
                        listGang.Add(GameTools.cardStr(data.card));
                        listGang.Add(""+3);
                        listGang.Add(GameTools.playerPosStr(item.id.idToPos(),data.from.idToPos()));
                        listGang.Add("-"+3);
                    }
                    _infoDic[item.id.idToPos()].Add(index, listGang);
                    index++;
                }
               
            }

            if (item.jiangma == null||item.jiangma.Count==0)
            {
                //买马
                isJiangma = false;
                foreach (var data in _maimaScore[item.id.idToPos()])
                {
                    List<string> listMa = new List<string>();
                    listMa.Add("买马");
                    listMa.Add(GameTools.cardStr(data.card));
                    listMa.Add(data.score.ToString());
                    listMa.Add(GameTools.playerPosStr(data.id.idToPos(),data.targetid.idToPos()));
                    listMa.Add((-data.score).ToString());
                    _infoDic[item.id.idToPos()].Add(index, listMa);
                    index++;
                }

            }
            else
            {
                isJiangma = true;
                //奖马
                foreach(var data in item.jiangma)
                {
                    //_jiangmaDic[item.id.idToPos()].Add(data);

                    List<string> listJiangma = new List<string>();
                    listJiangma.Add("奖马");
                    listJiangma.Add(GameTools.cardStr(data));
                    listJiangma.Add(item.basescore.ToString());
                    listJiangma.Add(str);
                    listJiangma.Add((-item.basescore).ToString());
                    _infoDic[item.id.idToPos()].Add(index, listJiangma);
                    index++;
                }
               
            }
        }
    }


    private string getHuFrom(int pos)
    {
        string str = "";
        foreach (var score in _scoreDic)
        {
            if (pos != score.Value.id.idToPos())
            {
                if (_scoreDic[pos].hu == 1)
                {
                    if (score.Value.hu == 2)
                    {
                        str += GameTools.playerPosStr(pos, score.Value.id.idToPos());
                    }
                    
                }
                if (_scoreDic[pos].hu == 2)
                {
                    if (score.Value.hu == 1)
                    {
                        str += GameTools.playerPosStr(pos, score.Value.id.idToPos());
                    }
                }
                if (_scoreDic[pos].hu == 0)
                {
                    if (score.Value.hu == 3)
                    {
                        str += GameTools.playerPosStr(pos,score.Value.id.idToPos());
                    }
                }
                if(_scoreDic[pos].hu==3)
                {
                    str = GameTools.playerPosStr(pos, pos);
                }
            }
        }
        return str;
    }
}

