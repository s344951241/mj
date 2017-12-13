using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class HuTest : MonoBehaviour {



    private int[] cards = new int[34];
    string str = "";
    // Use this for initialization
    List<int> list1 = new List<int>();
	void Start () {


        UIEventHandlerBase.AddListener(gameObject, UIEventType.ON_POINTER_CLICK, delegate
        {
            Debug.Log("单击");
        });
        UIEventHandlerBase.AddListener(gameObject, UIEventType.ON_POINTER_DOUBLE_CLICK, delegate
        {
            Debug.Log("双击");
        });

        //GlobleTimer.Instance.SetTimer(1000, delegate
        //{
        //    Debug.Log("11111");
        //});

        list1.Add(1);
        list1.Add(4);
        list1.Add(2);
        list1.Add(3);

        list1.Sort(delegate (int a, int b)
        {
            return toInt(a).CompareTo(toInt(b));
        });
        for (int j = 0; j < list1.Count; j++)
        {
            Debug.LogError(list1[j]);
        }

    }

    private int toInt(int num)
    {
        return num + 5;
    }
    // Update is called once per frame
    void Update()
    {

    }

    void OnGUI()
    {
        //if (GUILayout.Button("测试", GUILayout.Width(50), GUILayout.Height(20)))
        //{
        //    List<int> cards = new List<int>();
        //    cards.Add(2);
        //    cards.Add(2);
        //    cards.Add(2);

        //    cards.Add(5);
        //    cards.Add(5);
        //    cards.Add(5);

        //    cards.Add(6);
        //    cards.Add(6);
        //    cards.Add(6);

        //    cards.Add(9);
        //    cards.Add(9);

        //    cards.Add(21);
        //    cards.Add(21);
        //    cards.Add(21);


        //    if (CardController.Instance.IsCanHU(cards) == true)
        //    {
        //        Debug.Log(" can hu");
        //    }
        //    else
        //    {
        //        Debug.Log("can not hu");
        //    }
        //}
        //return;
        GUILayout.BeginHorizontal();

        for (int i = 0; i < 9; i++)
        {
            GUILayout.Label((i + 1) + "万");
            cards[i] = int.Parse(GUILayout.TextField(cards[i].ToString(), GUILayout.Width(30)));
        }


        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        for (int i = 9; i < 18; i++)
        {
            GUILayout.Label((i - 8) + "条");
            cards[i] = int.Parse(GUILayout.TextField(cards[i].ToString(), GUILayout.Width(30)));
        }

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        for (int i = 18; i < 27; i++)
        {
            GUILayout.Label((i - 17) + "饼");
            cards[i] = int.Parse(GUILayout.TextField(cards[i].ToString(), GUILayout.Width(30)));
        }

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("东");
        cards[27] = int.Parse(GUILayout.TextField(cards[27].ToString(), GUILayout.Width(30)));
        GUILayout.Label("南");
        cards[28] = int.Parse(GUILayout.TextField(cards[28].ToString(), GUILayout.Width(30)));
        GUILayout.Label("西");
        cards[29] = int.Parse(GUILayout.TextField(cards[29].ToString(), GUILayout.Width(30)));
        GUILayout.Label("北");
        cards[30] = int.Parse(GUILayout.TextField(cards[30].ToString(), GUILayout.Width(30)));

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("中");
        cards[31] = int.Parse(GUILayout.TextField(cards[31].ToString(), GUILayout.Width(30)));
        GUILayout.Label("發");
        cards[32] = int.Parse(GUILayout.TextField(cards[32].ToString(), GUILayout.Width(30)));
        GUILayout.Label("白");
        cards[33] = int.Parse(GUILayout.TextField(cards[33].ToString(), GUILayout.Width(30)));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("只测是否能胡（任意牌数）", GUILayout.Width(200), GUILayout.Height(40)))
        {
            CardController.Instance.cleanUp();
            for (int i = 0; i < cards.Length; i++)
            {
                for (int j = 0; j < cards[i]; j++)
                {
                    CardController.Instance.addCard(CardConst.getCardInfo(i + 1).type, CardConst.getCardInfo(i + 1).value);
                }
            }
            bool bo = CardController.Instance.checkCard(CardController.Instance._myCardList);
            if (bo)
                str = "能胡";
            else
                str = "不能胡";

        }
		if (GUILayout.Button ("测试胡牌类型（14张牌）", GUILayout.Width (200), GUILayout.Height (40))) 
		{
			CardController.Instance.cleanUp();
			for (int i = 0; i < cards.Length; i++)
			{
				for (int j = 0; j < cards[i]; j++)
				{
					CardController.Instance.addCard(CardConst.getCardInfo(i + 1).type, CardConst.getCardInfo(i + 1).value);
				}
			}
			bool bo = CardController.Instance.checkCard(CardController.Instance._myCardList);
			if (bo)
 			{				//str = "能胡";
				int hutype = CardController.Instance.getHuType (CardController.Instance._myCardList);
				HuScore hscore = GetHuType.Instance.gethuScore (hutype);
				str = hscore.name + "  " + hscore.score;
			}
			else
				str = "不能胡";			
		}
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("<color='#ff0000'>" + str + "</color>");
        GUILayout.EndHorizontal();
    }

}
