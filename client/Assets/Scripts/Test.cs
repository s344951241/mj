using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Test : MonoBehaviour {

	// Use this for initialization
	void Start () {
        //for (int i = 0; i < 13; i++)
        //{
        //    Card card = new Card(Random.Range(1, 4), Random.Range(1, 9));
        //    CardMgr.Instance.addList(card);
        //}
        //Debug.Log(getString(CardMgr.Instance.List));
        //CardMgr.Instance.sort();
        //Debug.Log(getString(CardMgr.Instance.List));
        TextureMgr.Instance.StartLoading();
        IconMgr.Instance.SetHeadRawImage(this.GetComponent<RawImage>(), "111");
        
	}
	
	// Update is called once per frame
	void Update () {
	    
	}
    //private string getString(List<Card> list)
    //{
    //    string str = "";
    //    foreach (var item in CardMgr.Instance.List)
    //    {
    //        str += "(" + item.CardType + "," + item.CardNum + ")";
    //    }
    //    return str;
    //}
}
