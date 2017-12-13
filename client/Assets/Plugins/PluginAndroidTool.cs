using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;

public class PluginAndroidTool:MonoBehaviour
{
    public string _outputLog = "";
    private int count = 0;

    private AndroidJavaObject activity;

    public delegate void STOP_TALK(byte[] b);
    public STOP_TALK stopTalk;

    public Queue<ChatDate> queue = new Queue<ChatDate>();
    public bool isPlaying = false;
    void Start()
    {
        if (activity == null)
        {
            try
            {
                var jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                activity = jc.GetStatic<AndroidJavaObject>("currentActivity");
                if (activity != null)
                {

                }
            }
            catch (Exception e)
            {
                Debug.Log("初始化android插件失败" + e);
            }

        }
    }
    public void setFun(STOP_TALK fun)
    {
        stopTalk = fun;
    }
    private void CallAndroid(string methodName, params object[] args)
    {
        if (activity != null)
        {
            activity.Call(methodName, args);
        }
    }

    private T CallAndroid<T>(string methodName, params object[] args)
    {
        if (activity != null)
        {
            activity.Call(methodName, args);
        }
        return default(T);
    }

    public void StartRecord(string path)
    {
        CallAndroid("startRecord", path);
    }

    public  void StopRecord()
    {
        CallAndroid("stopRecord", "");
    }
    public void PlaySound(string path)
    {
        EventDispatcher.Instance.Dispatch(GameEventConst.BGM_DOWN);
        CallAndroid("playSound", path);
    }
    public void SendMsgTest(string msg)
    {

    }
    public void AND_RecordEnd(string param)
    {
        byte[] bytes = File.ReadAllBytes(param);
        if (stopTalk != null)
            stopTalk(bytes);
        count++;
        AddLog("IOS_RecordEnd = " + count.ToString() + " len = " + bytes.Length);

        Debug.Log("armPath = " + param + " len = " + bytes.Length);
        //this.WriteVoiceToFile(bytes, count);
    }

    public void AND_PlaySound(string param)
    {
        //retcode=0代表成功，其他错误吗
        AddLog("IOS_PlaySound = " + param.ToString());
        //回复音量
    }
    public void AddLog(string log)
    {
        Debug.Log(log);
        _outputLog = _outputLog + log + "\n";
    }

    public void WriteVoiceToFile(byte[] bytes, int voiceId)
    {
        FileTools.WriteBytesToFile(bytes, URL.localCachePath + voiceId + ".amr");
    }


    //void OnGUI()
    //{
    //    _outputLog = GUI.TextField(new Rect(Screen.width / 2 - 50, 50, 300, 400), _outputLog);
    //    if (GUI.Button(new Rect(40, 50, 100, 100), "TestLog"))
    //    {
    //        Debug.Log("Cccccccc");
    //        count++;
    //        //AddLog ("count = " + count.ToString ());
    //        SendMsgTest("count = " + count.ToString());
    //    }

    //    if (GUI.Button(new Rect(40, 170, 100, 100), "Start"))
    //    {
    //        StartRecord(URL.localCachePath);
    //    }

    //    if (GUI.Button(new Rect(40, 290, 100, 100), "Stop"))
    //    {
    //        StopRecord();
    //    }

    //    if (GUI.Button(new Rect(40, 410, 100, 100), "Replay"))
    //    {
    //        PlaySound(URL.localCachePath + count + ".amr");
    //    }


    //}
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

    public void reStart()
    {
        CallAndroid("doRest", "");
    }

    public void StopPlay(string param)
    {
        isPlaying = false;
        EventDispatcher.Instance.Dispatch(GameEventConst.BGM_UP);
        if (queue.Count != 0)
        {
            PluginTool.Instance.isPlaying = true;
            ChatDate ch = queue.Dequeue();
            PluginTool.Instance.PlaySound(URL.localCachePath + ch.index + ".amr");
        }
    }
}
