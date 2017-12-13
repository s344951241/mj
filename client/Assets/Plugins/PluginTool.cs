using UnityEngine;
using System.Collections;
using System;
using System.IO;
using cn.sharesdk.unity3d;
using System.Collections.Generic;

public class PluginTool:MonoBehaviour {

#if UNITY_ANDROID && !UNITY_EDITOR
    public static PluginAndroidTool Instance {
        get {
            return GameConst.driver.GetComponent<PluginAndroidTool>();
        }
    }
#elif UNITY_IPHONE&& !UNITY_EDITOR
    public static PluginIOSTool Instance {
        get {
           return GameConst.driver.GetComponent<PluginIOSTool>();
        }
    }
#else
    public static PluginTool Instance {
        get {
           return GameConst.driver.GetComponent<PluginTool>();
        }
    }
#endif
    public Action<string> testBack;

    public delegate void STOP_TALK(byte[] b);
    public STOP_TALK stopTalk;
    public Queue<ChatDate> queue = new Queue<ChatDate>();
    public bool isPlaying = false;
    public virtual void SendMsgTest(string msg)
    {
        
    }
    public virtual void StartRecord(string path)
    {

        
    }
    public virtual void StopRecord()
    {
        
    }
    public virtual void PlaySound(string path)
    {
        Debug.LogError("ToolPlaySound");
    }

    public virtual void WriteVoiceToFile(byte[] bytes, int voiceId)
    {
        
    }

    public virtual void reqProductInfo(string s)
    {
       
    }
    public virtual void buyProduct(string s)
    {
        
    }
    public virtual bool isProductAvailable()
    {
        return false;
    }

    public virtual void initIAP()
    {

    }
    public virtual void reStart()
    {

    }
}

public class ChatDate
{
    public int index;
    public int playerId;

    public ChatDate(int id, int playerId)
    {
        this.index = id;
        this.playerId = playerId;
    }
}
