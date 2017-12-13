using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameTools {

    private static Dictionary<int, Player> _playerDic = new Dictionary<int, Player>();

    public static int getCardNumByName(string name)
    {
        string str = name.Substring(name.Length - 2);
        if (str.StartsWith("_"))
            str = str.Substring(1);
        return int.Parse(str);
    }

    public static Player getPlayerByPos(int pos)
    {
        if (_playerDic.Count == 0)
        {
            foreach (var player in RoleController.Instance._playerDic)
            {
                _playerDic.Add(player.Value.Id.idToPos(), player.Value);
            }
        }

        if (_playerDic.ContainsKey(pos))
            return _playerDic[pos];
        return null;
    }

    public static string playerPosStr(int pos,int from)
    {
        string str = "上家对家下家";
        switch (pos)
        {
            case 0:
                str = "上家对家下家";
                break;
            case 1:
                str = "下家";
                break;
            case 2:
                str = "对家";
                break;
            case 3:
                str = "上家";
                break;

        }
        if(pos==from)
            str = "上家对家下家";
        if (Mathf.Abs(pos - from) == 2)
            str = "对家";
        if ((pos - from) == 1 || (pos - from) == -3)
            str = "上家";
        if ((from - pos) == 1 || (from - pos) == -3)
            str = "下家";
        return str;
    }

    public static string cardStr(int card)
    {
        string str = "";
        switch (card)
        {
            case 1:
                str = "一万";
                break;
            case 2:
                str = "二万";
                break;
            case 3:
                str = "三万";
                break;
            case 4:
                str = "四万";
                break;
            case 5:
                str = "五万";
                break;
            case 6:
                str = "六万";
                break;
            case 7:
                str = "七万";
                break;
            case 8:
                str = "八万";
                break;
            case 9:
                str = "九万";
                break;
            case 10:
                str = "一条";
                break;
            case 11:
                str = "二条";
                break;
            case 12:
                str = "三条";
                break;
            case 13:
                str = "四条";
                break;
            case 14:
                str = "五条";
                break;
            case 15:
                str = "六条";
                break;
            case 16:
                str = "七条";
                break;
            case 17:
                str = "八条";
                break;
            case 18:
                str = "九条";
                break;
            case 19:
                str = "一筒";
                break;
            case 20:
                str = "二筒";
                break;
            case 21:
                str = "三筒";
                break;
            case 22:
                str = "四筒";
                break;
            case 23:
                str = "五筒";
                break;
            case 24:
                str = "六筒";
                break;
            case 25:
                str = "七筒";
                break;
            case 26:
                str = "八筒";
                break;
            case 27:
                str = "九筒";
                break;
            case 28:
                str = "東风";
                break;
            case 29:
                str = "南风";
                break;
            case 30:
                str = "西风";
                break;
            case 31:
                str = "北风";
                break;
            case 32:
                str = "红中";
                break;
            case 33:
                str = "發财";
                break;
            case 34:
                str = "白板";
                break;
        }
        return str;
    }

    public static string huTypeName(int type)
    {
        HuScore hscore = GetHuType.Instance.gethuScore(type);
        return hscore.name;
    }
}
