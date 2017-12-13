using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardController:Singleton<CardController>{

    public List<int>[] _myCardList;//起的类型牌
	//private List<int>[] _myAllCardList; //所有的牌，桌面的和已经碰的
    public Card _lastCard;//最后起的牌

    private bool _9LBD;//是否听连宝灯牌型
    private bool _13Y;//是否听13幺
    private bool _4AK;//是否听4暗刻
    private int _MKNum;//明刻数
    private int _AKNum;//暗刻数

    public CardController()
    {
        _9LBD = false;
        _13Y = false;
        _4AK = false;
        _AKNum = 0;
        _MKNum = 0;
        _myCardList = new List<int>[5];
        for (int i = 0; i < 5; i++)
        {
            List<int> list1 = new List<int>();
            _myCardList[i] = list1;
        }
        _lastCard = new Card();
        init();
    }
    public List<int>[] getCopyCard(List<int>[] cards)
    {
        List<int>[] cardList = new List<int>[5];
        for (int i = 0; i<cards.Length; i++)
        {
            List<int> list1 = new List<int>();
            list1.AddRange(cards[i]);
            cardList[i] = list1;
        }
        return cardList;
    }
    public void init()
    {
        //for (int i = 0; i < 10; i++)
        //{
        //    int value = Random.Range(1, 34);
        //    addCard(CardConst.getCardInfo(value).type, CardConst.getCardInfo(value).value);
        //}
        //printAllCard();
    }
    public int getCardsCount(List<int>[] cards = null)
    {
        int result = 0;
        if (cards == null)
        {
            foreach (var item in _myCardList)
            {
                result += item.Count ;
            }
        }
        else
        {
           
            foreach (var item in cards)
            {
                result += item.Count;
            }
        }
        
        
        return result;

    }
    //起牌,加入新牌并排序
    public bool addCard(int type,int num)
    {
        int size = _myCardList[type].Count;
        bool _find = false;
        for (int i = 0; i < size; i++)
        {
            if (_myCardList[type][i] > num)
            {
                _myCardList[type].Insert(i, num);
                _find = true;
                break;
            }
        }
        if (_find == false)
        {
            _myCardList[type].Add(num);
        }
        _lastCard.CardType = type;
        _lastCard.CardNum = num;

        return true;
    }

    public bool addCard(int bigNum)
    {
        return addCard(CardConst.getCardInfo(bigNum).type, CardConst.getCardInfo(bigNum).value);
    }
    //打牌
    public bool delCard(int index)
    {
        int count = 0;
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < _myCardList[i].Count; j++)
            {
                if (count == index)
                {
                    _myCardList[i].RemoveAt(j);
                    return true;
                }
                count++;
            }
        }
        return false;
    }
    //删除牌
    public bool delCard(int type, int num)
    {
        for (int i = 0; i < _myCardList[type].Count; i++)
        {
            _myCardList[type].Remove(num);
            return true;
        }
        return false;
    }
    //清空牌
    public void cleanUp()
    {
        for (int i = 0; i < 5; i++)
        {
            _myCardList[i].Clear();
        }
    }
    //检测是否胡牌
    public bool checkCard(List<int>[] cardList)
    {
        if (Check7D_HU(cardList))
            return true;
        if (Check13Y_HU(cardList))
            return true;
		if (CheckYTL_New(cardList))
			return true;
        return  CheckHU(cardList);
    }


    //检测碰
    public bool checkPeng(int type, int num)
    {
        if (_myCardList[type].Count != 0)
        {
            int iSize = _myCardList[type].Count;
            if (iSize >= 2)
            {
                int value = 0;
                for (int i = 0; i < iSize; i++)
                {
                    if (_myCardList[type][i] == num)
                    {
                        value++;
                    }
                }
                if (value >= 2)
                {
                    return true;
                }
            }
        }
        return false;
    }
    //碰
    public bool peng(int type, int num)
    {
        delCard(type, num);
        delCard(type, num);

        //DataMgr.Instance._everyPeng[0].Add(new Card(type, num).BigNum, new PengData(0,from,new Card(type, num).BigNum,obj));
        return true;
    }
    //检测补扛
    public bool checkBuGang(int num)
    {
        if (DataMgr.Instance._everyPeng[0].ContainsKey(num))
            return true;
        return false;
    }
    ////检测明扛
    public int checkMingGang(int type, int num)
    {
        //_tempGangCardList.Clear();
        int temp = 0;
        if (_myCardList[type].Count != 0)
        {
            int iSize = _myCardList[type].Count;
            if (iSize >= 3)
            {
                for (int i = 0; i < iSize - 2; i++)
                {
                    if ((_myCardList[type][i] == num) && (_myCardList[type][i + 1] == num) && (_myCardList[type][i + 2] == num))
                    {
                        Card card = new Card(type, num);
                        temp = card.BigNum;
                    }
                }
            }
        }
        return temp;
    }
    //检测暗杠
    public List<int> checkAnGang()
    {
        List<int> list = new List<int>();
        for (int i = 0; i < 5; i++)
        {
            if (_myCardList[i].Count < 4)
                continue;
            for (int j = 0; j < _myCardList[i].Count; j++)
            {
                int temp = 0;
                for (int k = 0; k < _myCardList[i].Count; k++)
                {
                    
                    if (j != k)
                    {
                        if (_myCardList[i][j] == _myCardList[i][k])
                            temp++;
                    }
                }
                if (temp >= 3)
                {
                    int num = new Card(i, _myCardList[i][j]).BigNum;
                    if (!list.Contains(num))
                        list.Add(new Card(i, _myCardList[i][j]).BigNum);
                }
            }
        }
        return list;
    }
    //扛
    public bool gang(int type,int num)
    {
        //while(_myCardList[type].Contains(num))
        //{
        //    delCard(type, num);
        //}
        int gangNum = new Card(type, num).BigNum;
        if (DataMgr.Instance._everyPeng[0].ContainsKey(gangNum))
            DataMgr.Instance._everyPeng[0].Remove(gangNum);
        //DataMgr.Instance._everyGang[0].Add(gangNum,new GangData(0,from,gangNum,obj));
        return true;
    }
    //检测是否胡牌
    private bool checkAACard(int value1, int value2)
    {
        if (value1 == value2)
            return true;
        return false;
    }
    //检测是否三连张
    private bool checkABCCard(int value1, int value2,int value3)
    {
        if (value1 == (value2 - 1) && value2 == (value3 - 1))
            return true;
        return false;
    }
    //检测是否三重张
    private bool checkAAACard(int value1,int value2,int value3)
    {
        if(value1==value2&&value2==value3)
            return true;
        return false;
    }
    //检测是否四重张
    private bool checkAAAACard(int value1, int value2, int value3,int value4)
    {
        if (value1 == value2 && value2 == value3 && value3 == value4)
            return true;
        return false;
    }
    //检测是否三连对
    private bool checkAABBCCCard(int value1,int value2,int value3,int value4,int value5,int value6)

    {
        if (value1 == value2 && value3 == value4 && value5 == value6)
        {
            if ((value1 == value3 - 1) && (value3 == value5 - 1))
                return true;
        }
        return false;
    }

    private bool checkABBCCCDDF(int value1, int value2, int value3, int value4, int value5, int value6, int value7, int value8,int value9)
    {
        if (value2 == value3 && value4 == value5 && value5 == value6 && value7 == value8)
        {
            if ((value1 == value2 - 1) && (value2 == value4 - 1) && (value6 == value7 - 1) && (value8 == value9 - 1))
                return true;
           
        }
        return false;
    }

    private bool checkABBBCCCDD(int value1, int value2, int value3, int value4, int value5, int value6, int value7, int value8, int value9)
    {
        if (value2 == value3 && value3 == value4 && value5 == value6 && value6 == value7 && value8 == value9)
        {
            if ((value1 == value2 - 1) && (value4 == value5 - 1) && (value7 == value8 - 1))
                return true;
            if ((value1 == value2 + 1) && (value4 == value5 + 1) && (value7 == value8 + 1))
                return true;
        }
        return false;
    }

    private bool checkABBCCCDDDEEF(int value1, int value2, int value3, int value4, int value5, int value6, int value7, int value8, int value9,int value10,int value11,int value12)
    {
        if (value2 == value3 && value4 == value5 && value5 == value6 && value7 == value8 && value8 == value9 && value10 == value11)
        {
            if ((value1 == value2 - 1) && (value3 == value4 - 1) && (value6 == value7 - 1) && (value9 == value10 - 1) && (value11 == value12 - 1))
                return true;
        }
        return false;
    }

    private bool checkABBCCCCDDDEE(int value1, int value2, int value3, int value4, int value5, int value6, int value7, int value8, int value9, int value10, int value11, int value12)
    {
        if (value2 == value3 && value4 == value5 && value5 == value6 && value6 == value7 && value8 == value9 && value9 == value10 && value11 == value12)
        {
            if ((value1 == value2 - 1) && (value3 == value4 - 1) && (value7 == value8 - 1) && (value10 == value11 - 1))
            {
                return true;
            }
        }
        return false;
    }
    //检测是否三连三
    private bool checkAAABBBCCCCard(int value1, int value2, int value3, int value4, int value5, int value6,int value7,int value8,int value9)
    {
        if ((value1 == value2 && value2 == value3) && (value4 == value5 && value5 == value6) && value7 == value8 && value8 == value9)
        {
            if ((value1 == value4 - 1) && (value4 == value7 - 1))
                return true;
        }
        return false;
    }
    //检测是否三连刻
    private bool checkAAAABBBBCCCCCard(int value1, int value2, int value3, int value4, int value5, int value6, int value7, int value8, int value9,int value10,int value11,int value12)
    {
        if((value1==value2&&value2==value3&&value3==value4)&&(value5==value6&&value6==value7&&value7==value8)&&(value9==value10&&value10==value11&&value11==value12))
        {

            if ((value1 == value5 - 1) && (value5 == value9 - 1))
                return true;

        }
        return false;
    }
    //检测是否六连对
    private bool checkAABBCCDDEEFFCard(int value1, int value2, int value3, int value4, int value5, int value6, int value7, int value8, int value9, int value10, int value11, int value12)
    {
        if (value1 == value2 && value3 == value4 && value5 == value6 && value7 == value8 && value9 == value10 && value11 == value12)
        {
            if ((value1 == -value3 - 1) && (value3 == value5 - 1) && (value5 == value7 - 1) && (value7 == value9 - 1) && (value9 == value11 - 1))
            {
                return true;
            }
        }
        return false;
    }

    //带将牌检测------------------------------------------------------------------------------
    
    //检测是否胡牌
    private bool check5Card(int value1, int value2, int value3, int value4, int value5)
    {
        //如果1,2将 
        if (checkAACard(value1, value2))
        {
            if (check3Card(value3, value4, value5))
                return true;
        }
        //如果2，3将
        if (checkAACard(value2, value3))
        {
            if (check3Card(value1, value4, value5))
                return true;
        }
        //如果3，4将
        if (checkAACard(value3, value4))
        {
            if (check3Card(value1, value2, value5))
                return true;
        }
        //如果4,5将
        if (checkAACard(value4, value5))
        {
            if (check3Card(value1, value2, value3))
                return true;
        }
        return false;
    }
    //检测是否胡牌
    private bool check8Card(int value1, int value2, int value3, int value4, int value5, int value6, int value7, int value8)
    {
        //1,2将
        if (checkAACard(value1, value2))
        {
            if (check6Card(value3, value4, value5, value6, value7, value8))
                return true;
        }
        //2,3将
        if (checkAACard(value2, value3))
        {
            if (check6Card(value1,value4,value5,value6,value7,value8))
                return true;
        }
        //3,4将
        if (checkAACard(value3, value4))
        {
            if (check6Card(value1, value2, value5, value6, value7, value8))
                return true;
        }
        //4,5将
        if (checkAACard(value4, value5))
        {
            if (check6Card(value1, value2, value3, value6, value7, value8))
                return true;
        }
        //5,6将
        if (checkAACard(value5, value6))
        {
            if (check6Card(value1, value2, value3, value4, value7, value8))
                return true;
        }
        //6,7将
        if (checkAACard(value6, value7))
        {
            if (check6Card(value1, value2, value3, value4, value5, value8))
                return true;
        }
        //7,8将
        if (checkAACard(value7, value8))
        {
            if (check6Card(value1, value2, value3, value4, value5, value6))
                return true;
        }

        return false;
    }
    //检测是否胡牌
    private bool check11Card(int value1, int value2, int value3, int value4, int value5, int value6, int value7, int value8, int value9, int value10, int value11)
    {
        //1,2将
        if (checkAACard(value1, value2))
        {
            if (check9Card(value3, value4, value5, value6, value7, value8, value9, value10, value11))
                return true;
        }
        //2,3将
        if (checkAACard(value2, value3))
        {
            if (check9Card(value1, value4, value5, value6, value7, value8, value9, value10, value11))
                return true;
        }
        //3,4将
        if (checkAACard(value3, value4))
        {
            if (check9Card(value1, value2, value5, value6, value7, value8, value9, value10, value11))
                return true;
        }
        //4,5将
        if (checkAACard(value4, value5))
        {
            if (check9Card(value1, value2, value3, value6, value7, value8, value9, value10, value11))
                return true;
        }
        //5,6将
        if (checkAACard(value5, value6))
        {
            if (check9Card(value1, value2, value3, value4, value7, value8, value9, value10, value11))
                return true;
        }
        //7,8将
        if (checkAACard(value7, value8))
        {
            if (check9Card(value1, value2, value3, value4, value5, value6,value9, value10, value11))
                return true;
        }
        //8,9将
        if (checkAACard(value8, value9))
        {
            if (check9Card(value1, value2, value3, value4, value5, value6, value7, value10, value11))
                return true;
        }
        //9,10将
        if (checkAACard(value9, value10))
        {
            if (check9Card(value1, value2, value3, value4, value5, value6, value7, value8, value11))
                return true;
        }
        //10,11将
        if (checkAACard(value10, value11))
        {
            if (check9Card(value1, value2, value3, value4, value5, value6, value7, value8, value9))
                return true;
        }
           
        return false;
    }
    //检测是否胡牌
    private bool check14Card(int value1, int value2, int value3, int value4, int value5, int value6, int value7, int value8, int value9, int value10, int value11, int value12, int value13, int value14)
    {
        //1,2将
        if (checkAACard(value1, value2))
        {
            if (check12Card(value3, value4, value5, value6, value7, value8, value9, value10, value11, value12, value13, value14))
                return true;
        }
        //2,3将
        if (checkAACard(value2, value3))
        {
            if (check12Card(value1, value4, value5, value6, value7, value8, value9, value10, value11, value12, value13, value14))
                return true;
        }
        //3,4将
        if (checkAACard(value3, value4))
        {
            if (check12Card(value1, value2, value5, value6, value7, value8, value9, value10, value11, value12, value13, value14))
                return true;
        }
        //4,5将
        if (checkAACard(value4, value5))
        {
            if (check12Card(value1, value2, value3,value6, value7, value8, value9, value10, value11, value12, value13, value14))
                return true;
        }
        //5,6将
        if (checkAACard(value5, value6))
        {
            if (check12Card(value1, value2, value3,value4, value7, value8, value9, value10, value11, value12, value13, value14))
                return true;
        }
        //6,7将
        if (checkAACard(value6, value7))
        {
            if (check12Card(value1, value2, value3,value4, value5, value8, value9, value10, value11, value12, value13, value14))
                return true;
        }
        //7,8将
        if (checkAACard(value7, value8))
        {
            if (check12Card(value1, value2, value3, value4, value5, value6,value9, value10, value11, value12, value13, value14))
                return true;
        }
        //8,9将
        if (checkAACard(value8, value9))
        {
            if (check12Card(value1, value2, value3, value4, value5, value6, value7, value10, value11, value12, value13, value14))
                return true;
        }
        //9,10将
        if (checkAACard(value9, value10))
        {
            if (check12Card(value1, value2, value3, value4, value5, value6, value7, value8, value11, value12, value13, value14))
                return true;
        }
        //10,11将
        if (checkAACard(value10, value11))
        {
            if (check12Card(value1, value2, value3, value4, value5, value6, value7, value8, value9, value12, value13, value14))
                return true;
        }
        //11,12将
        if (checkAACard(value11, value12))
        {
            if (check12Card(value1, value2, value3, value4, value5, value6, value7, value8, value9, value10, value13, value14))
                return true;
        }
        //12,13将
        if (checkAACard(value12, value13))
        {
            if ( check12Card(value1, value2, value3, value4, value5, value6, value7, value8, value9, value10, value11, value14))
                return true;
        }
        //13,14将

        if (checkAACard(value13, value14))
        {
            if (check12Card(value1, value2, value3, value4, value5, value6, value7, value8, value9, value10, value11, value12))
                return true;
        }
        return false;
    }
    //不带将检测-----------------------------------------------------------------
    
    //检测是否胡牌
    private bool check3Card(int value1, int value2, int value3)
    {
        if (checkABCCard(value1, value2, value3))
            return true;
        if (checkAAACard(value1, value2, value3))
            return true;
        return false;
    }
    //检测是否胡牌
    private bool check6Card(int value1, int value2, int value3, int value4, int value5, int value6)
    {
        if (check3Card(value1, value2, value3) && check3Card(value4, value5, value6))
            return true;
        if (check3Card(value1, value3, value5) && check3Card(value2, value4, value6))
            return true;
        if (checkAABBCCCard(value1, value2, value3, value4, value5, value6))
            return true;
        if (checkAAAACard(value2, value3, value4, value5))
            if (checkABCCard(value1, value2, value6))
                return true;
        return false;
    }
    //检测是否胡牌
    private bool check9Card(int value1, int value2, int value3, int value4, int value5, int value6, int value7, int value8, int value9)
    {
        if (checkABCCard(value1, value2, value3) && check6Card(value4, value5, value6, value7, value8, value9))
            return true;
        if (checkAAACard(value1, value2, value3) && check6Card(value4, value5, value6, value7, value8, value9))
            return true;
        if (checkABCCard(value7, value8, value9) && check6Card(value1, value2, value3, value4, value5, value6))
            return true;
        if (checkAAACard(value7, value8, value9) && check6Card(value1, value2, value3, value4, value5, value6))
            return true;
        if (checkABBCCCDDF(value1, value2, value3, value4, value5, value6, value7, value8, value9))
            return true;
        if (checkABBBCCCDD(value1, value2, value3, value4, value5, value6, value7, value8, value9))
            return true;
        if (checkABBBCCCDD(value9, value8, value7, value6, value5, value4, value3, value2,value1))
            return true;
        return false;
    }
    //检测是否胡牌
    private bool check12Card(int value1, int value2, int value3, int value4, int value5, int value6, int value7, int value8, int value9, int value10, int value11, int value12)
    {
        if (checkABCCard(value1, value2, value3) && check9Card(value4, value5, value6, value7, value8, value9,value10,value11,value12))
            return true;
        if (checkAAACard(value1, value2, value3) && check9Card(value4, value5, value6, value7, value8, value9, value10, value11, value12))
            return true;
        if (checkABCCard(value10, value11, value12) && check9Card(value1, value2, value3, value4, value5, value6,value7,value8,value9))
            return true;
        if (checkAAACard(value10, value11, value12) && check9Card(value1, value2, value3, value4, value5, value6, value7, value8, value9))
            return true;
        if (check6Card(value1, value2, value3, value4, value5, value6) && check6Card(value7, value8, value9, value10, value11, value12))
            return true;
        if (checkABBCCCDDDEEF(value1, value2, value3, value4, value5, value6, value7, value8, value9, value10, value11, value12))
            return true;
        if (checkABBCCCCDDDEE(value1, value2, value3, value4, value5, value6, value7, value8, value9, value10, value11, value12))
            return true;
        return false;
    }
    private bool check18()
    {
        if (DataMgr.Instance._everyGang[0].Count == 4)
            return true;
        return false;
    }

	private bool docheckYTL(List<int>[] cardList, int cardtype)
	{
		HashSet<int> hs = new HashSet<int>();
		int count = 0;
		List<int> tmpList;
		tmpList = cardList[cardtype];
		if (tmpList.Count != 9 && tmpList.Count != 10)
			return false;
		for (int i = 0; i <= tmpList.Count - 1; i++) 
		{
			hs.Add(tmpList[i]);
			count ++;
		}

		tmpList = cardList[CARD_TYPE.风];
		if (tmpList.Count != 4 && tmpList.Count != 5)
			return false;
		for (int i = 0; i <= tmpList.Count - 1; i++)
		{
			hs.Add(100 + tmpList[i]);
			count++;
		}
		if (count == 14 && hs.Count == 13)
			return true;
		return false;		
	}

	private bool CheckYTL_New(List<int>[] cardList)
	{
		if (docheckYTL (cardList, CARD_TYPE.万) == true)
			return true;
		if (docheckYTL (cardList, CARD_TYPE.条) == true)
			return true;
		if (docheckYTL (cardList, CARD_TYPE.饼) == true)
			return true;
		return false;
	}

    //检测是否胡十三幺  
    private bool Check13Y_HU(List<int>[] cardList)
    {
        HashSet<int> hs = new HashSet<int>();
        int count = 0;
        List<int> tmpList;


        tmpList = cardList[CARD_TYPE.万];
        if (tmpList.Count != 2 && tmpList.Count != 3)
            return false;//减少后面代码执行
        for (int i = 0; i <= tmpList.Count - 1; i++)
        {
            if (tmpList[i] == 1)
            {
                hs.Add(1);
                count++;
            }
            if (tmpList[i] == 9)
            {
                hs.Add(9);
                count++;
            }
        }

        tmpList = cardList[CARD_TYPE.饼];
        if (tmpList.Count != 2 && tmpList.Count != 3)
            return false;//减少后面代码执行
        for (int i = 0; i <= tmpList.Count - 1; i++)
        {
            if (tmpList[i] == 1)
            {
                hs.Add(10);
                count++;
            }
            if (tmpList[i] == 9)
            {
                hs.Add(18);
                count++;
            }
        }

        tmpList = cardList[CARD_TYPE.条];
        if (tmpList.Count != 2 && tmpList.Count != 3)
            return false;//减少后面代码执行
        for (int i = 0; i <= tmpList.Count - 1; i++)
        {
            if (tmpList[i] == 1)
            {
                hs.Add(19);
                count++;
            }
            if (tmpList[i] == 9)
            {
                hs.Add(27);
                count++;
            }
        }

        tmpList = cardList[CARD_TYPE.风];
        if (tmpList.Count != 4 && tmpList.Count != 5)
            return false;
        for (int i = 0; i <= tmpList.Count - 1; i++)
        {
            hs.Add(100 + tmpList[i]);
            count++;
        }

        tmpList = cardList[CARD_TYPE.ZFB];
        if (tmpList.Count != 3 && tmpList.Count != 4)
            return false;
        for (int i = 0; i <= tmpList.Count - 1; i++)
        {
            hs.Add(200 + tmpList[i]);
            count++;
        }

        if (count == 14 && hs.Count == 13)
            return true;
        return false;
    }

    //检测是否七对  
    private bool Check7D_HU(List<int>[] cardList)
    {
        int iDoubleNum = 0;
        for (int i = 0; i < 5; i++)
        {
            int iSize = cardList[i].Count;
            if (iSize % 2 == 1)
                return false;
            if (iSize == 0)
                continue;
            for (int j = 0; j < iSize-1; j++)
            {
                if (cardList[i][j] == cardList[i][j + 1])
                {
                    iDoubleNum++;
                    j++;
                }
            }
        }
        if (iDoubleNum == 7)
            return true;
        return false;
    }



    //检测胡  
    private bool CheckHU(List<int>[] cardList)
    {
        bool t_OK = false;
        int iJiangNum = 0;
        int iSize = 0;// _myCardList[CARD_TYPE.ZFB].Count;

        for (int i = 0; i < 5; i++)
        {
            iSize = cardList[i].Count;
            if (iSize > 0)
            {
                if (iSize == 2)
                {
                    if (!checkAACard(cardList[i][0], cardList[i][1]))
                    {
                        return false;
                    }
                    else
                    {
                        iJiangNum++;
                    }
                }
                else if (iSize == 3)
                {
                    if (!checkAAACard(cardList[i][0], cardList[i][1], cardList[i][2]))
                    {
                        if (!checkABCCard(cardList[i][0], cardList[i][1], cardList[i][2]))
                        {
                            return false;
                        }
                    }
                }
                else if (iSize == 5)
                {
                    if (!check5Card(cardList[i][0], cardList[i][1], cardList[i][2], cardList[i][3], cardList[i][4]))
                    {
                        return false;
                    }
                    else
                    {
                        iJiangNum++;
                    }
                }
                else if (iSize == 6)
                {
                    if (!check6Card(cardList[i][0], cardList[i][1], cardList[i][2], cardList[i][3], cardList[i][4], cardList[i][5]))
                    {
                        return false;
                    }
                }
                else if (iSize == 8)
                {
                    if (!check8Card(cardList[i][0], cardList[i][1], cardList[i][2], cardList[i][3], cardList[i][4], cardList[i][5], cardList[i][6], cardList[i][7]))
                    {
                        return false;
                    }
                    else
                    {
                        iJiangNum++;
                    }
                }
                else if (iSize == 9)
                {
                    if (!check9Card(cardList[i][0], cardList[i][1], cardList[i][2], cardList[i][3], cardList[i][4], cardList[i][5], cardList[i][6], cardList[i][7], cardList[i][8]))
                    {
                        return false;
                    }
                }
                else if (iSize == 11)
                {
                    if (!check11Card(cardList[i][0], cardList[i][1], cardList[i][2], cardList[i][3], cardList[i][4], cardList[i][5], cardList[i][6], cardList[i][7], cardList[i][8], cardList[i][9], cardList[i][10]))
                    {
                        return false;
                    }
                    else
                    {
                        iJiangNum++;
                    }
                }
                else if (iSize == 12)
                {
                    if (!check12Card(cardList[i][0], cardList[i][1], cardList[i][2], cardList[i][3], cardList[i][4], cardList[i][5], cardList[i][6], cardList[i][7], cardList[i][8], cardList[i][9], cardList[i][10], cardList[i][11]))
                    {
                        return false;
                    }
                }
                else if (iSize == 14)
                {
                    if (!check14Card(cardList[i][0], cardList[i][1], cardList[i][2], cardList[i][3], cardList[i][4], cardList[i][5], cardList[i][6], cardList[i][7], cardList[i][8], cardList[i][9], cardList[i][10], cardList[i][11], cardList[i][12], cardList[i][13]))
                    {
                        return false;
                    }
                    else
                    {
                        iJiangNum++;
                    }
                }
                else
                {
                    return false;
                }
            }
        }
        if (iJiangNum == 1) return true;
        return false;
    }



    public  bool IsCanHU(List<int> mah)
    {
        List<int> pais = new List<int>(mah);

        //pais.Add(ID);
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
    }


    public int M(int a)
    {
        if ((a >= 1) && (a <= 9))
        {
            return a;
        }
        if ((a >= 10) && (a <= 18))
        {
            return a + 1;
        }

        if ((a >= 19) && (a <= 27))
        {
            return a + 2;
        }
        int num = 0;
        switch (a)
        {
            case 28:
                num = 31;
                break;
            case 29:
                num = 33;
                break;
            case 30:
                num = 35;
                break;
            case 31:
                num = 37;
                break;
            case 32:
                num = 41;
                break;
            case 33:
                num = 43;
                break;
            case 34:
                num = 45;
                break;
        }
        return num;
    }

	public int getHuType(List<int>[] cardList)
	{
		
        if (check18() == true)
            return 3;
        if (Check13Y_HU (cardList) == true) 
		{
			return 4;
		}

		if (Check7D_HU (cardList) == true) 
		{
			return 11;
		}

		if (CheckYTL_New(cardList) == true) 
		{
			return 20;
		}
		return GetHuType.Instance.getHuType(cardList); 
	}
}

