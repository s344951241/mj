using UnityEngine;
using System.Collections;
using NetWork;

public class FPS : MonoBehaviour {
    private float updateInterval = 1.0f;
    private float lastInterval;
    private int frames = 0;
    public static float fps;
    private float ms;
    private bool isVisible;


    public static int senPackageNum = 0;
    public static int recPackageNum = 0;
    private int packageNumBySecond = 0;

    // Use this for initialization
    void Start() {
        lastInterval = Time.realtimeSinceStartup;
        frames = 0;
        GameObject.DontDestroyOnLoad(this);
        isVisible = true;
    }

    // Update is called once per frame
    void Update() {
        ++frames;
        var timeNow = Time.realtimeSinceStartup;
        if (timeNow > lastInterval + updateInterval)
        {
            packageNumBySecond = senPackageNum + recPackageNum;
            senPackageNum = recPackageNum = 0;
            fps = frames / (timeNow - lastInterval);
            ms = 1000.0f / Mathf.Max(fps, 0.00001f);
            frames = 0;
            lastInterval = timeNow;
        }
    }

    public static bool isLowFps
    {
        get { return true; }
    }

    void OnGUI()
    {
        if (!isVisible)
        {
            return;
        }
        if (fps < 15)
        {
            GUI.color = Color.red;
        }
        else if (fps < 25)
        {
            GUI.color = Color.yellow;
        }
        else
        {
            GUI.color = Color.white;
        }

        int a = 1024 * 1024;
        string info = "";
        if (NetClient.Instance().connected)
        {
            info += "发送流量:";
            if (GameConst.SendBuffLeng / (1024 * 1024) < 1)
            {
                info += (GameConst.SendBuffLeng / 1024) + "KB\n";
            }
            else
            {
                info += (GameConst.SendBuffLeng / (1024 * 1024)) + "MB\n";
            }
            info += "接受流量";
            if (GameConst.RecBuffLeng / (1024 * 1024) < 1)
            {
                info += (GameConst.RecBuffLeng / 1024) + "KB\n";
            }
            else
            {
                info += (GameConst.RecBuffLeng / (1024 * 1024)) + "MB\n";
            }
        }
        GUIStyle bb = new GUIStyle();
        bb.fontSize = 40;
        GUI.Label(new Rect(20, 20, 500, 500),
             fps.ToString("f2") + "FPS\n" + info
            + "<color=yellow>发包数:" + senPackageNum + "条\n"
            + "收包数:" + recPackageNum + "条/秒"
            + "总包数:" + packageNumBySecond + "条/秒"
            + "</color>",bb);
        GUI.color = Color.white;
    }
}
