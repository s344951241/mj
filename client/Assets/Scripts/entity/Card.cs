using UnityEngine;
using System.Collections;
using System;
public class Card : IComparable<Card>
{
    private int _cardType;
    private int _cardNum;
    private bool _isOut;
    private int _bigNum;
    public Card()
    {

    }
    public Card(int type, int num)
    {
        _cardType = type;
        _cardNum = num;
        _bigNum = CardConst.GetCardBigNum(type, num);
    }

    public Card(int bigNum)
    {
        _bigNum = bigNum;
        _cardType = CardConst.getCardInfo(bigNum).type;
        _cardNum = CardConst.getCardInfo(bigNum).value;
    }

    public Card(int type,int value,int bigNum)
    {

        _cardType = type;
        _cardNum = value;
        _bigNum = bigNum;
    }
    public int CompareTo(Card card)
    {
        int result;
        if (_cardType == card.CardType && _cardNum == card.CardNum)
        {
            result = 0;
        }
        else
        {
            if (_cardType > card.CardType)
            {
                result = 1;
            }
            else if (_cardType == card._cardType && _cardNum > card._cardNum)
            {
                result = 1;
            }
            else
            {
                result = -1;
            }
        }
        return result;
    }
    public int CardType
    {
        get
        {
            return _cardType;
        }

        set
        {
            _cardType = value;
        }
    }

    public int CardNum
    {
        get
        {
            return _cardNum;
        }

        set
        {
            _cardNum = value;
        }
    }

    public bool IsOut
    {
        get
        {
            return _isOut;
        }

        set
        {
            _isOut = value;
        }
    }

    public int BigNum
    {
        get
        {
            return _bigNum;
        }

        set
        {
            _bigNum = value;
        }
    }
}
