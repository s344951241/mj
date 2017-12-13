using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections.Generic;
public class PluginIOSTool:MonoBehaviour
{

    public string _outputLog;
    private int count = 0;

    public delegate void STOP_TALK(byte[] b);
    public STOP_TALK stopTalk = null;
	public Queue<ChatDate> queue = new Queue<ChatDate>();
	public bool isPlaying = false;
    void Start()
    {
        CallIOS("OCinitsySDK", new string[] { "Driver", "1" });
    }

    public void CallIOS(string funName, string[] args = null)
    {
#if UNITY_IPHONE
		OC_CallIOS(funName, args);
#endif
    }

	public void PlaySound(string path)
	{
		EventDispatcher.Instance.Dispatch(GameEventConst.BGM_DOWN);
		CallIOS ("OCplaySound", new string[] {path});
	}


	public void StartRecord(string path)
	{
		CallIOS("OCstartRecord", new string[] {path});
	}

	public void StopRecord()
	{
		CallIOS("OCstopRecord");
	}

	public void SendMsgTest(string msg)
	{
		CallIOS("OCsendMsg", new string[] { msg });
	}

    public void Log(string msg)
    {
        CallIOS("OCDebugLog", new string[] { msg });
    }

    [DllImport("__Internal")]
   	private static extern void OC_CallIOS(string funName, string[] args);

    void AddLog(string log)
    {
        Debug.Log("AddLog = " + log);
        _outputLog = _outputLog + log + "\n";
    }

    public void IOS_RecordEnd(string armPath)
    {
        byte[] bytes = File.ReadAllBytes(armPath);
        if (stopTalk != null)
            stopTalk(bytes);
        count++;

        AddLog("IOS_RecordEnd = " + count.ToString() + " len = " + bytes.Length);

        Debug.Log("armPath = " + armPath + " len = " + bytes.Length);
        //WriteVoiceToFile(bytes, count);
    }

    public void IOS_PlaySound(string retcode)
    {
        //retcode=0代表成功，其他错误吗
         AddLog("IOS_PlaySound = " + retcode.ToString());
		if (retcode != "0") {
			Debug.LogError ("IOS_PlaySound error " + retcode.ToString ());
		}
        //回复音量
		isPlaying = false;
		EventDispatcher.Instance.Dispatch(GameEventConst.BGM_UP);
		if (queue.Count != 0)
		{
			PluginTool.Instance.isPlaying = true;
			ChatDate ch = queue.Dequeue();
			PluginTool.Instance.PlaySound(URL.localCachePath + ch.index + ".amr");
		}
    }

    public void WriteVoiceToFile(byte[] bytes, int voiceId)
    {
        FileTools.WriteBytesToFile(bytes, URL.localCachePath  + voiceId + ".amr");
    }


    public void recvVoice(byte[] bytes)
    {
        //降低音量
        WriteVoiceToFile(bytes, 1);
        //PluginIOSTool.Instance.PlaySound(URL.localCachePath + "UnityVoice/" + count + ".amr");
    }
    public bool VoiceFileExists(uint voiceId)
    {
        return File.Exists(URL.localCachePath + "UnityVoice/" + voiceId + ".amr");
    }


    /*void OnGUI()
    {
        _outputLog = GUI.TextField(new Rect(Screen.width / 2 - 50, 50, 300, 400), _outputLog);
        if (GUI.Button(new Rect(40, 50, 100, 100), "TestLog"))
        {
            Debug.Log("Cccccccc");
            count++;
            //AddLog ("count = " + count.ToString ());
            SendMsgTest("count = " + count.ToString());
        }

        if (GUI.Button(new Rect(40, 170, 100, 100), "Start"))
        {
            StartRecord(URL.localCachePath);
        }

        if (GUI.Button(new Rect(40, 290, 100, 100), "Stop"))
        {
            StopRecord();
        }

        if (GUI.Button(new Rect(40, 410, 100, 100), "Replay"))
        {
            PlaySound(URL.localCachePath + "UnityVoice/" + count + ".amr");
        }
    }*/

    [DllImport("__Internal")]
    private static extern void RequstProductInfo(string s);//获取商品信息

    public void reqProductInfo(string s)
    {
        RequstProductInfo(s);
    }

    [DllImport("__Internal")]
    private static extern void BuyProduct(string s);//购买商品

    public void buyProduct(string s)
    {
        BuyProduct(s);
    }
    [DllImport("__Internal")]
    private static extern bool IsProductAvailable();//判断是否可以购买

    public bool isProductAvailable()
    {
        return IsProductAvailable();
    }

    [DllImport("__Internal")]
    private static extern void InitIAPManager();//初始化
    public void initIAP()
    {
        InitIAPManager();
    }
    public void reStart()
    {

    }


    void IOSToU(string s)
    {
        Debug.Log("[MsgFrom ios]" + s);
    }

    //获取product列表
    void ShowProductList(string s)
    {
       
    }

    void BuyReturn(string s)
    {
        //1成功
        Debug.Log("BuyReturn = " + s);
        EventDispatcher.Instance.Dispatch(GameEventConst.BUY_OVER, s);
    }

}
