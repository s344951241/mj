using UnityEngine;
using System.Collections;

public class CardConst {

    public static int GetCardBigNum(int type, int num)
    {
        int bigNum = 0;
        switch (type)
        {
            case CARD_TYPE.万:
                bigNum = num;
                break;
            case CARD_TYPE.条:
                bigNum = num + 9;
                break;
            case CARD_TYPE.饼:
                bigNum = num + 18;
                break;
            case CARD_TYPE.风:
                bigNum = (int)Mathf.Sqrt(num) + 27;
                break;
            case CARD_TYPE.ZFB:
                bigNum =(int)Mathf.Sqrt(num) + 31;
                break;
        }

        return bigNum;
    }
    public static CardInfo getCardInfo(int num)
    {
        CardInfo info = new CardInfo();
        if (num >= 1 && num <= 9)
        {
            info.type = CARD_TYPE.万;
            info.value = num;
        }
        else if (num >= 10 && num <= 18)
        {
            info.type = CARD_TYPE.条;
            info.value = num - 9;
        }
        else if (num >= 19 && num <= 27)
        {
            info.type = CARD_TYPE.饼;
            info.value = num - 18;
        }
        else if (num >= 28 && num <= 31)
        {
            info.type = CARD_TYPE.风;
            info.value = (num - 27)*(num-27);
        }
        else if (num >= 32 && num <= 34)
        {
            info.type = CARD_TYPE.ZFB;
            info.value = (num - 31)*(num-31);
        }
        return info;
    }
}

public class CARD_TYPE
{
    public const int ZFB = 4;//1-zhong, 4--fa, 9---bai
    public const int 风 = 3;//1-dong, 4-- nan, 9-xi, 16-bei
    public const int 万 = 0;
    public const int 条 = 1;
    public const int 饼 = 2;
}

public class CARD_STATE
{
    public const bool CARD_GET = true;
    public const bool CARD_PUT = false;
}
public class CardInfo
{
    public int type;
    public int value;
}

