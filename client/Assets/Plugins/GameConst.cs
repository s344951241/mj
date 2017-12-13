using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameConst {

    public static MonoBehaviour driver;
    public static GameObject auto;
    public static int tableId;
    public static int timeCount = 15;

    public static int selfPos = 0;

    public static Dictionary<string, AudioClip> _bigAudio = new Dictionary<string, AudioClip>();

    public static string Language = "s";
    public static string BGM = "1";
    public static float musicVol;
    public static float soundVol;
    public static int ower = 0;
    public static Vector3 selfPosition = new Vector3(0, -160, 0);
    public static Vector3 rightPosition = new Vector3(200,0,0);
    public static Vector3 topPosition = new Vector3(0,178,0);
    public static Vector3 leftPosition = new Vector3(-300,0,0);

    public static float upDis = 50;

    public static bool isGM = false;
    public static bool isDaili = false;
    public static bool isTurn = false;
    public static bool isGuest = false;

    public static uint tanpai = 1000;//摊牌
    public static uint dipai = 3000;//底牌显示
    public static uint jiesuan = 8000;//总结算

    public static int talkLength = 15000;
    public static int talkCD = 20000;

    public static bool isPlay = false;

    public static bool isSetDanjuHead = true;

    public static GameObject _room;

    public static decimal RecBuffLeng = 0;
    public static decimal SendBuffLeng = 0;

    public static float VoteTimer = 0;
    public static float VoteTime = 120f;

    public static int curId = -1;
    public static int curCard = -1;
    public static int startStatus = 0;
}
