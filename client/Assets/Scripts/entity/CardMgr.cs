using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardMgr : Singleton<CardMgr> {

    private List<Card> _list = new List<Card>();
    private Card _card;
    private Card _giveCard;

    public List<Card> List
    {
        get
        {
            return _list;
        }

        set
        {
            _list = value;
        }
    }

    public Card Card
    {
        get
        {
            return _card;
        }

        set
        {
            _card = value;
        }
    }

    public Card GiveCard
    {
        get
        {
            return _giveCard;
        }

        set
        {
            _giveCard = value;
        }
    }

    public CardMgr()
    {
        

    }

    public void addList(Card card)
    {
        List.Add(card);
    }
    public Card removeList(Card card)
    {
        if (List.Contains(card))
            List.Remove(card);
        return card;
    }

    public void sort()
    {
        List.Sort();
    }

    public bool isPeng(Card card)
    {
        int num = 0;
        foreach (var item in _list)
        {
            if (item.CardType == card.CardType && item.CardNum == item.CardNum&&!item.IsOut)
            {
                num++;
            }
        }
        if (num >= 2)
            return true;
        else
            return false;
    }
    //先不考虑明扛暗扛 
    public bool isGang(Card card)
    {
        int num = 0;
        foreach(var item in _list)
        {
            if (item.CardType == card.CardType && item.CardNum == item.CardNum)
            {
                num++;
            }
        }
        if (num == 3)
            return true;
        else
            return false;
    }


}
