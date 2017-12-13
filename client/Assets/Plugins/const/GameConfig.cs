using UnityEngine;
using System.Collections;

public class GameConfig  {
    public static readonly string GAME_NAME = "mj";

    public static bool isStart = false;

    public static int WAIT_TIME = 5;

    public static int punish = 0;

    public static uint serverID = 0;

    public static string portID = string.Empty;

    public static string serverIP = string.Empty;

    public static int gid = 1;

    public static string serverName = string.Empty;

    public static bool isGameInit = false;

    public static string deviceName = string.Empty;

    public static string deviceUniqueIdentifier = string.Empty;


    public static string androidProgramVersion = "2.2";
    public static string iosProgramVersion = "1.1";

    public static string programVersion {
        get {
#if UNITY_EDITOR||UNITY_ANDROID
            return androidProgramVersion;
#else
            return iosProgramVersion;
#endif
        }
    }

    public static string platformName = string.Empty;

    public static string OsName
    {
        get
        {
            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
            {
                return "EDITOR";
            }
            if (Application.platform == RuntimePlatform.Android)
            {
                return "ANDROID";
            }
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                return "IOS";
            }
            if (Application.platform == RuntimePlatform.WindowsPlayer)
            {
                return "WINDOW";
            }
            return string.Empty;
        }
    }
    public static string platformId;

    public static string unityVersion = string.Empty;
    public static void Init()
    {
        GameConfig.deviceUniqueIdentifier = SystemInfo.deviceUniqueIdentifier;
        GameConfig.deviceName = SystemInfo.deviceName;
        GameConfig.isGameInit = false;
        GameConfig.unityVersion = "Unity " + Application.unityVersion;
        GameConfig.serverName = WWW.UnEscapeURL(PlayerPrefs.GetString("name"));
        GameConfig.serverIP = WWW.UnEscapeURL(PlayerPrefs.GetString("ip"));
        GameConfig.portID = WWW.UnEscapeURL(PlayerPrefs.GetString("port"));
        GameConfig.serverID = (uint)PlayerPrefs.GetInt("id");
    }
}
