using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class handCardProxy : MonoBehaviour {

    public GameObject[] handCards;
    public GameObject pengCard0;
    public GameObject pengCard1;
    public GameObject pengCard2;
    public GameObject pengCard3;

    public GameObject gangCard0;
    public GameObject gangCard1;
    public GameObject gangCard2;
    public GameObject gangCard3;
    public GameObject buGang;
    public GameObject card;
    public GameObject cardStart;
    private float posY = 0;
    public bool isHu = false;
    private List<GameObject> huCards = new List<GameObject>();
	// Use this for initialization
	void Start () {
        isHu = false;
        posY = handCards[0].transform.localPosition.y;
	}
    public void reset()
    {
        isHu = false;
        posY = handCards[0].transform.localPosition.y;
        cleanHuCards();
    }
	// Update is called once per frame
	void Update () {
      
    }
    public void setPos()
    {
        for (int i = 0; i < handCards.Length; i++)
        {
            handCards[i].transform.localPosition = new Vector3(handCards[i].transform.localPosition.x, posY, 0);
        }
    }
    public void setFalse()
    {
        for (int i = 0; i < handCards.Length; i++)
        {
            handCards[i].SetActive(false);
        }
    }
    public void setTrue()
    {
        for (int i = 0; i < handCards.Length; i++)
        {
            handCards[i].SetActive(true);
        }
    }
    public void setHandCards(int num)
    {
        for (int i = 0; i < handCards.Length; i++)
        {
            if (i < num)
                handCards[i].SetActive(true);
            else
                handCards[i].SetActive(false);
        }
    }
    public void setCard(List<int> list,string str)
    {
        for (int i = 0; i < list.Count; i++)
        {
            GameObject obj = null;
            if (i == 0 && cardStart != null)
            {
                obj = GameObject.Instantiate(cardStart, transform, true) as GameObject;
            }
            else
            {
                obj = GameObject.Instantiate(card, transform, true) as GameObject;
            }
            huCards.Add(obj);
            obj.SetActive(true);
            IconMgr.Instance.SetImage(obj.transform.FindChild("value").GetComponent<Image>(), str+list[i]);
        }
    }

    public void cleanHuCards()
    {
        for (int i = 0; i < huCards.Count; i++)
        {
            GameObject.Destroy(huCards[i]);
        }
        huCards.Clear();
    }
    public GameObject peng(int pos,int from, int card)
    {
        GameObject obj = null;
        switch (from)
        {
            case 0:
                obj = GameObject.Instantiate(pengCard0,transform,true) as GameObject;
                break;
            case 1:
                obj = GameObject.Instantiate(pengCard1,transform,true) as GameObject;
                break;
            case 2:
                obj = GameObject.Instantiate(pengCard2, transform, true) as GameObject;
                break;
            case 3:
                obj = GameObject.Instantiate(pengCard3, transform, true) as GameObject;
                break;
        }
        obj.transform.localScale = Vector3.one;
       
        obj.transform.parent = transform;
        obj.transform.SetAsFirstSibling();
        for (int i = 0; i < obj.transform.GetComponent<PengAndGangProxy>().startStrs.Length; i++)
        {
            IconMgr.Instance.SetImage(obj.GetComponent<PengAndGangProxy>().images[i], obj.GetComponent<PengAndGangProxy>().startStrs[i]+card);
        }
        obj.SetActive(true);
        if (DataMgr.Instance._everyPeng[pos].ContainsKey(card))
        {
            GameObject o = DataMgr.Instance._everyPeng[pos][card].obj;
            if (o != null)
            {
                GameObject.Destroy(o);
            }
            DataMgr.Instance._everyPeng[pos][card].obj = obj;
        }
        else
        {
            DataMgr.Instance._everyPeng[pos].Add(card, new PengData(pos, from, card, obj));
        }
       
        return obj;
    }

    public GameObject gang(int pos,int from, int card,bool flag)
    {
        GameObject obj = null;
        if (flag)
            obj = GameObject.Instantiate(buGang, transform, true) as GameObject;
        else
        {
            switch (from)
            {
                case 0:
                    obj = GameObject.Instantiate(gangCard0, transform, true) as GameObject;
                    break;
                case 1:
                    obj = GameObject.Instantiate(gangCard1, transform, true) as GameObject;
                    break;
                case 2:
                    obj = GameObject.Instantiate(gangCard2, transform, true) as GameObject;
                    break;
                case 3:
                    obj = GameObject.Instantiate(gangCard3, transform, true) as GameObject;
                    break;
            }

        }
        
        obj.transform.localScale = Vector3.one;
        obj.transform.parent = transform;
        obj.transform.SetAsFirstSibling();
        for (int i = 0; i < obj.transform.GetComponent<PengAndGangProxy>().startStrs.Length; i++)
        {
            IconMgr.Instance.SetImage(obj.GetComponent<PengAndGangProxy>().images[i], obj.GetComponent<PengAndGangProxy>().startStrs[i]+card);
        }
        obj.SetActive(true);
        if (DataMgr.Instance._everyGang[pos].ContainsKey(card))
        {
            GameObject o = DataMgr.Instance._everyGang[pos][card].obj;
            if (o != null)
                GameObject.Destroy(o);
            DataMgr.Instance._everyGang[pos][card].obj = obj;
        }
        else
        {
            DataMgr.Instance._everyGang[pos].Add(card, new GangData(pos, from, card,flag, obj));
        }
       
        return obj;
    }
}
