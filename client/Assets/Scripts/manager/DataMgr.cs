using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DataMgr : Singleton<DataMgr> {

    public int money;

    public Dictionary<int, List<int>> _leftCardsDic;//转换后的pos为序
    public Dictionary<int, List<int>> _putCardsDic;//打出去的牌以东南西北为顺序



    public Dictionary<int, Dictionary<int,PengData>> _everyPeng;//转换pos排序
    public Dictionary<int, Dictionary<int, GangData>> _everyGang;//转换pos排序

    public int _curCard;

    public int leftCardNum = 13;
    public int rightCardNum = 13;
    public int topCardNum = 13;

    public int curHeIndex = 0;

    public int curRound = 0;
    public int zhuangId = 0;

    public bool isHu = false;

    //新加
    public Dictionary<int, List<int>> _heCards;


    public int lassCardsNum = 0;
    public Dictionary<int, int> _showScore;
    public List<int> _readyNextIDs;

    public int sumRound;
    public int maxbet;
    public int jiangma;
    public int maima;
    public void reSet()
    {
        lassCardsNum = 0;
        zhuangId = 0;
        curRound = 0;
        curHeIndex = 0;
        leftCardNum = 13;
        rightCardNum = 13;
        topCardNum = 13;

        for (int i = 0; i < _everyPeng.Count; i++)
        {
            foreach (var item in _everyPeng[i])
            {
                GameObject.Destroy(item.Value.obj);
            }
            _everyPeng[i].Clear();
        }

        for (int i = 0; i < _everyGang.Count; i++)
        {
            foreach (var item in _everyGang[i])
            {
                GameObject.Destroy(item.Value.obj);
            }
            _everyGang[i].Clear();
        }
        clearHeCards();
        clearLeftCards();
        _showScore.Clear();
}

    public DataMgr()
    {
        _showScore = new Dictionary<int, int>();

        _readyNextIDs = new List<int>();

        _everyPeng = new Dictionary<int, Dictionary<int, PengData>>();
        _everyGang = new Dictionary<int, Dictionary<int, GangData>>();

        Dictionary<int, PengData> dic0 = new Dictionary<int, PengData>();
        Dictionary<int, PengData> dic1 = new Dictionary<int, PengData>();
        Dictionary<int, PengData> dic2 = new Dictionary<int, PengData>();
        Dictionary<int, PengData> dic3 = new Dictionary<int, PengData>();

        _everyPeng.Add(0, dic0);
        _everyPeng.Add(1, dic1);
        _everyPeng.Add(2, dic2);
        _everyPeng.Add(3, dic3);

        Dictionary<int, GangData> di0 = new Dictionary<int, GangData>();
        Dictionary<int, GangData> di1 = new Dictionary<int, GangData>();
        Dictionary<int, GangData> di2 = new Dictionary<int, GangData>();
        Dictionary<int, GangData> di3 = new Dictionary<int, GangData>();

        _everyGang.Add(0, di0);
        _everyGang.Add(1, di1);
        _everyGang.Add(2, di2);
        _everyGang.Add(3, di3);

        _leftCardsDic = new Dictionary<int, List<int>>();
        for (int i = 0; i < 4; i++)
        {
            List<int> list = new List<int>();
            _leftCardsDic.Add(i, list);
        }

        _putCardsDic = new Dictionary<int, List<int>>();
        for (int i = 0; i < 4; i++)
        {
            List<int> list = new List<int>();
            _putCardsDic.Add(i, list);
        }
        //创建hecards
        _heCards = new Dictionary<int, List<int>>();
        List<int> heList0 = new List<int>();
        List<int> heList1 = new List<int>();
        List<int> heList2 = new List<int>();
        List<int> heList3 = new List<int>();
        _heCards.Add(0, heList0);
        _heCards.Add(1, heList1);
        _heCards.Add(2, heList2);
        _heCards.Add(3, heList3);

    }
    public void clearLeftCards()
    {
        for (int i = 0; i < 4; i++)
        {
            _leftCardsDic[i].Clear();
        }
    }

    public void clearHeCards()
    {
        for (int i = 0; i < 4; i++)
        {
            _heCards[i].Clear();
        }
    }

    public void clearPutCards()
    {
        for (int i = 0; i < 4; i++)
        {
            _putCardsDic[i].Clear();
        }
    }
    public void moneyChanged(Table.MoneyChange value)
    {
        money = value.mymoney;
    }
}
