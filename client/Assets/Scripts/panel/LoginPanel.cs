using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using NetWork;
using cn.sharesdk.unity3d;

public class LoginPanel : BasePanel {

    private static LoginPanel instance;
    public static LoginPanel Instance
    {
        get
        {
            if (instance == null)
                instance = new LoginPanel();
            return instance;
        }

    }
    private LoginPanel()
    {
        base_name = PanelTag.LOGIN_PANEL;
        PlayerPrefs.SetInt("check", 1);
        _isPop = false;
    }

    private Button _login;
    private Button _guestLogin;
    private Button _guestZc;
    private Image _head;
    private Text _name;

    private Toggle _toggle;


    private InputField _accInput;
    private InputField _nameInput;
    private Text _version;

    private bool _isWx = false;
    public override void InitPanel(Transform uiSprite)
    {
        base.InitPanel(uiSprite);
        _login = uiSprite.Find("Button").GetComponent<Button>();
        _guestLogin = uiSprite.Find("Button1").GetComponent<Button>();
       
        _guestZc = uiSprite.Find("zcBtn").GetComponent<Button>();
        _toggle = uiSprite.Find("info/Toggle").GetComponent<Toggle>();
        _toggle.isOn = PlayerPrefs.HasKey("check") ? true : false;


        _guestLogin.gameObject.SetActive(false);
        _login.gameObject.SetActive(false);
        _guestZc.gameObject.SetActive(false);

        _login.onClick.AddListener(delegate
        {
            login();
        });
        _guestLogin.onClick.AddListener(delegate
        {
            if (!_toggle.isOn)
            {
                QuickTips.ShowQuickTipsNor("请同意用户协议");
                return;
            }

            if (GameConst.isGuest)
            {
                GuestLoginPanel.Instance.load(2);
            }
           
        });

        _guestZc.onClick.AddListener(delegate
        {
            if (!_toggle.isOn)
            {
                QuickTips.ShowQuickTipsNor("请同意用户协议");
                return;
            }

            if (GameConst.isGuest)
            {
                GuestLoginPanel.Instance.load(1);
            }
        });
        _toggle.onValueChanged.AddListener(delegate
        {
            PlayerPrefs.SetInt("check", 1);
            //_toggle.isOn = !_toggle.isOn;
        });
        _version = uiSprite.Find("version").GetComponent<Text>();
       _accInput = uiSprite.Find("acc").GetComponent<InputField>();
        _nameInput = uiSprite.Find("name").GetComponent<InputField>();
#if UNITY_EDITOR
        _accInput.gameObject.SetActive(true);
        _nameInput.gameObject.SetActive(true);
#else
         _accInput.gameObject.SetActive(false);
        _nameInput.gameObject.SetActive(false);
#endif

    }

    public override void startUp(object obj = null)
    {
        base.startUp();
        _isWx = PlayerPrefs.HasKey("openId");
        _version.text = "版本号："+GameConfig.programVersion;
        SDKMgr.Instance.updateUserInfo = updateUserInfo;
        //s_canLogin = false;
        GameConst.isGuest = false;
        connect();
    }

    public override void AddListener()
    {
        base.AddListener();
        EventDispatcher.Instance.AddEventListener(GameEventConst.CONNECTED, logining);
        EventDispatcher.Instance.AddEventListener<bool,bool>(GameEventConst.VERSION, versionChecked);
    }
    public override void RemoveListener()
    {
        base.RemoveListener();
        EventDispatcher.Instance.RemoveEventListener(GameEventConst.CONNECTED, logining);
        EventDispatcher.Instance.RemoveEventListener<bool,bool>(GameEventConst.VERSION, versionChecked);
    }

    private void versionChecked(bool boo,bool guest)
    {
        Debug.LogError("版本号check返回");
        if (!boo)
        {
           
            AlertMgr.Instance.showAlert(ALERT_TYPE.type3, "请到官网 http://27toy.com/ 下载最新版本", delegate ()
            {
                Application.OpenURL("http://27toy.com/");
                //Application.Quit();
            });
        }
        GameConst.isGuest = guest;
        if (guest)
        {
            //_accInput.gameObject.SetActive(true);
            //_nameInput.gameObject.SetActive(true);
            _login.gameObject.SetActive(false);
            _guestLogin.gameObject.SetActive(true);
            _guestZc.gameObject.SetActive(true);
        }
        else
        {
            _login.gameObject.SetActive(true);
            _guestLogin.gameObject.SetActive(false);
            _guestZc.gameObject.SetActive(false);
        }
    }
    private void logining()
    {
        ProtoReq.CheckVersion(GameConfig.programVersion, GameConfig.OsName);
        Debug.LogError("收到链接返回");
        if (_isWx)
        {
            //SDKMgr.Instance.GetUserInfo();
            //updateUserInfo(PlayerPrefs.GetString("openId"), "", PlayerPrefs.GetString("nickName"), PlayerPrefs.GetInt("sex"), PlayerPrefs.GetString("url"));
        }
        
    }
    private void login()
    {
        //QuickTips.ShowRedQuickTips("消息");
        //HallPanel.Instance.load();
        if (!_toggle.isOn)
        {
            QuickTips.ShowQuickTipsNor("请同意用户协议");
            return;
        }
#if !UNITY_EDITOR
       
        
        if (GameConst.isGuest)
        {
            ProtoReq.LoginReq(GameConst.isGuest,_accInput.text.ToZhuan(),_accInput.text.ToZhuan(), 0, "htttp://test", _nameInput.text);
        }
        else
        {
		//SDKMgr.Instance.GetUserInfo();Authorize
			SDKMgr.Instance.GetUserInfo();
        }
#else
        if (string.IsNullOrEmpty(_accInput.text) || string.IsNullOrEmpty(_nameInput.text))
            QuickTips.ShowQuickTipsNor("输入有误");
        else
            ProtoReq.LoginReq(GameConst.isGuest,_accInput.text.ToZhuan(), _nameInput.text,0, "htttp://test");
#endif
    }
    private void connect()
    {
        Debug.Log("Start Loading...");
        NetClient network = NetClient.Instance();
        //network.Connect("123.207.241.224",8888);
        // network.Connect("127.0.0.1", 8888);
        //network.Connect("192.168.0.2", 8888);
        //network.Connect("123.207.107.11", 8888);
        // network.Connect("192.168.0.106", 8888);
        //network.Connect("mja.ledounet.com", 8888);
#if UNITY_ANDROID
        //network.Connect("mjcsa.ledounet.com",8888);
        network.Connect("120.24.170.229",23526);
#elif UNITY_IPHONE
        network.Connect("mjios.ledounet.com", 8888);
#endif
        NetClient.Register();
        //SDKMgr.Instance.DoShareTestFriend();
    }

    private void updateUserInfo(string openId, string unionId, string nickName,int sex, string imageUrl)
    {
        Debug.LogError("收到微信，准备登录服务器");
        PlayerPrefs.SetString("openId", openId);
        MainRole.Instance.Name = nickName;
        string imageUrlAfter = "";
        if (!string.IsNullOrEmpty(imageUrl))
        {
            imageUrlAfter = imageUrl.Substring(0, imageUrl.Length - 1) + "46";
        }
        ProtoReq.LoginReq(GameConst.isGuest,openId, nickName,sex, imageUrlAfter);
    }
}
