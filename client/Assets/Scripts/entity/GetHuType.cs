using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class HuScore{
	public int huid;
	public string name;
	public int score;
	public HuScore (string name, int score)
	{
		this.name = name;
		this.score = score;
	}
}

public class GetHuType:Singleton<GetHuType>{
	private List<int>[] _myAllCardList; 
	private Dictionary<int, HuScore> _huTable;
	public GetHuType ()
	{
		init ();
	}

	public void init()
	{
		_myAllCardList = new List<int>[5];
		initData();
		for (int i = 0; i < 5; i++)
		{
			List<int> list = new List<int>();
			_myAllCardList[i] = list;
		}
	}

	public void clear()
	{
		for (int i = 0; i < 5; i++)
		{
			_myAllCardList[i].Clear();
		}		
	}

	//小四喜 10
	private bool checkXSX()
	{
		return (_myAllCardList[CARD_TYPE.风].Count == 11);
	}


	//大四喜 20
	private bool checkDSX()
	{
		return (_myAllCardList[CARD_TYPE.风].Count == 12);
	}

	//小三元 10
	private bool checkXSY()
	{
		return (_myAllCardList[CARD_TYPE.ZFB].Count == 8);
	}

	//大三元 20
	private bool checkDSY()
	{
		return (_myAllCardList[CARD_TYPE.ZFB].Count == 9);
	}

	//字一色 20
	private bool checkZYS()
	{
		//int total_count = _myAllCardList[CARD_TYPE.风].Count + _myAllCardList[CARD_TYPE.ZFB].Count;
		return (getZICount() == 14);
	}


	//混幺9 10
	private bool checkHYJ()
	{
		int tong1_count = getCardCountByType(CARD_TYPE.饼, 1);
		int tong9_count = getCardCountByType(CARD_TYPE.饼, 9);
		int wan1_count = getCardCountByType(CARD_TYPE.万, 1);
		int wan9_count = getCardCountByType(CARD_TYPE.万, 9);
		int tiao1_count = getCardCountByType(CARD_TYPE.条, 1);
		int tiao9_count = getCardCountByType(CARD_TYPE.条, 9);
		int zi_count = getZICount();
		int totalcount = tong1_count + tong9_count + wan1_count + wan9_count + tiao1_count + tiao9_count + zi_count;
		return (totalcount == 14);
	}

	//检测混一色 4
	private bool checkHYS()
	{
		return ((_myAllCardList[CARD_TYPE.万].Count + getZICount () == 14) ||
				(_myAllCardList[CARD_TYPE.条].Count + getZICount () == 14) ||
				(_myAllCardList[CARD_TYPE.饼].Count + getZICount () == 14));
	}


	//检测对对胡 4
	private bool checkDDH()
	{
		return ((checkListPeng(_myAllCardList [CARD_TYPE.万]) == true) &&
				(checkListPeng(_myAllCardList [CARD_TYPE.条]) == true) &&
				(checkListPeng(_myAllCardList [CARD_TYPE.饼]) == true) &&
				(checkListPeng(_myAllCardList [CARD_TYPE.风]) == true) &&
				(checkListPeng(_myAllCardList [CARD_TYPE.ZFB]) == true));
	}

	//检测混碰 6
	private bool checkHP()
	{
		
		//if (_myAllCardList [CARD_TYPE.万].Count > 0) 
		if(_myAllCardList[CARD_TYPE.万].Count + getZICount () == 14)
		{
			bool wan = ((checkListPeng (_myAllCardList [CARD_TYPE.万]) == true) &&
			           (checkListPeng (_myAllCardList [CARD_TYPE.风]) == true) &&
			           (checkListPeng (_myAllCardList [CARD_TYPE.ZFB]) == true));
			if (wan == true) 
			{
				return true;
			}
		}

		//if (_myAllCardList [CARD_TYPE.条].Count > 0) 
		if(_myAllCardList[CARD_TYPE.条].Count + getZICount () == 14)
		{
			bool tiao = ((checkListPeng (_myAllCardList [CARD_TYPE.条]) == true) &&
			            (checkListPeng (_myAllCardList [CARD_TYPE.风]) == true) &&
			            (checkListPeng (_myAllCardList [CARD_TYPE.ZFB]) == true));
			if (tiao == true)
				return true;
		}	

		//if (_myAllCardList [CARD_TYPE.饼].Count > 0) 
		if(_myAllCardList[CARD_TYPE.饼].Count + getZICount () == 14)
		{
			bool bing = ((checkListPeng (_myAllCardList [CARD_TYPE.饼]) == true) &&
			            (checkListPeng (_myAllCardList [CARD_TYPE.风]) == true) &&
			            (checkListPeng (_myAllCardList [CARD_TYPE.ZFB]) == true));
			if (bing == true)
				return true;
		}
		return false;
	}

	//检测清碰 6
	private bool checkQP()
	{
		bool wan = ((_myAllCardList [CARD_TYPE.万].Count == 14) && (checkListPeng (_myAllCardList [CARD_TYPE.万]) == true));
		if (wan == true)
			return true;
		
		bool tiao = ((_myAllCardList [CARD_TYPE.条].Count == 14) && (checkListPeng (_myAllCardList [CARD_TYPE.条]) == true));
		if (tiao == true)
			return true;
		
		bool bing = ((_myAllCardList [CARD_TYPE.饼].Count == 14) && (checkListPeng (_myAllCardList [CARD_TYPE.饼]) == true));
		if (bing == true)
			return true;
		
		return false;
	}

	//一条龙， 6
	private bool checkYTL()
	{
		if (getZICount () > 0)
		{
			return false;
		}
		Dictionary<int, int> map = new Dictionary<int, int>();
		List<int> list1 = _myAllCardList [CARD_TYPE.万];
		List<int> list2 = _myAllCardList [CARD_TYPE.条];
		List<int> list3 = _myAllCardList [CARD_TYPE.饼];

		for (int i = 0; i <= list1.Count - 1; i++) 
		{
			int value = list1[i];
			if (map.ContainsKey(value) == false) 
			{
				map[value] = 0;
			}
			map[value] = map[value] + 1;
		}

		for (int i = 0; i <= list2.Count - 1; i++) 
		{
			int value = list2[i];
			if (map.ContainsKey(value) == false) 
			{
				map[value] = 0;
			}
			map[value] = map[value] + 1;
		}


		for (int i = 0; i <= list3.Count - 1; i++) 
		{
			int value = list3[i];
			if (map.ContainsKey(value) == false) 
			{
				map[value] = 0;
			}
			map[value] = map[value] + 1;
		}

		if (map.Count != 9)
			return false;
		
		int count = 0;
		foreach (KeyValuePair<int, int>kv in map)
		{
			if (kv.Value == 1) 
			{
				count++;
			}
		}

		if ((count == 7) || (count == 8))
		{
			return true;
		}

		return false;
	}

	//检测清一色 4
	private bool checkQYS()
	{
		return ((_myAllCardList[CARD_TYPE.万].Count == 14) || 
				(_myAllCardList[CARD_TYPE.条].Count == 14) || 
				(_myAllCardList[CARD_TYPE.饼].Count == 14));
	}

	private int getZICount()//获取东南西北中发白的，总数
	{
		return _myAllCardList[CARD_TYPE.风].Count + _myAllCardList [CARD_TYPE.ZFB].Count;
	}

	//根据card_type获取该牌的个数，比如cardtype =  万， value 1,统计1万个数
	private int getCardCountByType(int card_type, int value)
	{
		int count = 0;
		List<int> tmpList = _myAllCardList[card_type];
		for (int i = 0; i <= tmpList.Count - 1; i++) 
		{
			if (value == tmpList [i]) 
			{
				count++;
			}
		}

		return count;
	}

	//输入5,6返回2，判断可以最多的碰子是多少，输入情况其实只有3，5，6，8，9，11，12，14
	private int getPengCount(int value)
	{
		int tmpValue = value;
		if (tmpValue % 3 == 2)
		{
			tmpValue = tmpValue + 1;
		}
		return tmpValue / 3;
	}


	//检查一个List是不是都是对子（1，1，1，2，2，3，3，3）== true
	bool checkListPeng(List<int> list)
	{
		
		Dictionary<int, int> d = new Dictionary<int, int> ();
		for (int i = 0; i <= list.Count - 1; i++)
		{
			int value = list[i];
			if (d.ContainsKey (value) == false) 
			{
				d[value] = 0;
			}
			d[value] = d[value] + 1;
		}

		foreach (KeyValuePair<int, int> kv in d) 
		{
			if (kv.Value >= 4) 
			{
				Debug.LogError ("over count = " + kv.Value.ToString ());
				return false;
			}
		}

		return (getPengCount(list.Count) == d.Count);
	}
		

	public string getHuStr(int huType)
	{
		return "";
	}

	//考虑是否可以传对将进来
	public int getHuType(List<int>[] cardList)
	{
		int num = 0;
		clear();
		for (int i = 0; i < 5; i++) 
		{
			List<int> tmpList = cardList[i];
			for (int j = 0; j <= tmpList.Count - 1; j++) 
			{
				_myAllCardList[i].Add(tmpList[j]);
				num++;
			}
		}
		if (num != 14) {
			Debug.LogError("长度不是14");
			return 100;
		}
        

		if (checkDSX() == true)
			return 5; //大四喜 

		if (checkDSY () == true)
			return 6; //大三元

		if (checkZYS () == true)
			return 7;  //字一色


		if (checkXSX () == true)
			return 8;//小四喜

		if (checkXSY () == true)
			return 9;//小三元

		if (checkHYJ () == true)
			return 10; //混幺九

		//if (checkYTLNew () == true)
		//	return 20; //新的一条龙

		if (checkQP () == true)
			return 12; //清碰

		if (checkHP () == true)
			return 13;//混碰

		if (checkYTL () == true)
			return 14;//混龙

		if (checkQYS () == true)
			return 15; //清一色

		if (checkHYS () == true)
			return 16;//混一色

		if (checkDDH () == true)
			return 17;//对对胡


		return 18;
	}


	public HuScore gethuScore(int hutype)
	{
		//return _huTable[hutype];
		if (_huTable.ContainsKey (hutype) == true) {
			return _huTable[hutype];
		}

		return new HuScore("非法胡牌类型"+hutype.ToString(), 0);
	}

	public void initData()
	{
		_huTable = new Dictionary<int, HuScore>();
		_huTable.Add (1, new HuScore("天胡", 40));
		_huTable.Add (2, new HuScore("地胡", 20));
		_huTable.Add (3, new HuScore("十八罗汉", 36));
		_huTable.Add (4, new HuScore("十三幺", 26));
		_huTable.Add (5, new HuScore("大四喜", 20));
		_huTable.Add (6, new HuScore("大三元", 20));
		_huTable.Add (7, new HuScore("字一色", 20));
		_huTable.Add (8, new HuScore("小四喜", 10));
		_huTable.Add (9, new HuScore("小三元", 10));
		_huTable.Add (10, new HuScore("混幺九", 10));
		_huTable.Add (11, new HuScore("七对子", 6));
		_huTable.Add (12, new HuScore("清碰", 8));
		_huTable.Add (13, new HuScore("混碰", 6));
		_huTable.Add (14, new HuScore("混龙", 6));
		_huTable.Add (15, new HuScore("清一色", 6));
		_huTable.Add (16, new HuScore("混一色", 4));
		_huTable.Add (17, new HuScore("对对胡", 4));
		_huTable.Add (18, new HuScore("鸡平", 2));
		_huTable.Add (19, new HuScore("鸡平", 2));
		_huTable.Add (20, new HuScore("一条龙", 10));
	}
}

