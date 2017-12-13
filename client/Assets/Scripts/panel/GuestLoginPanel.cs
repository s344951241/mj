using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GuestLoginPanel : BasePanel
{
    private static GuestLoginPanel instance;
    public static GuestLoginPanel Instance
    {
        get
        {
            if (instance == null)
                instance = new GuestLoginPanel();
            return instance;
        }
    }

    private GuestLoginPanel()
    {
        base_name = PanelTag.GUEST_PANEL;
        _isPop = true;
    }

    private Button _login;
    private Button _zc;
    private Button _close;
    private InputField _name;
    private InputField _password;


    public override void InitPanel(Transform uiSprite)
    {
        base.InitPanel(uiSprite);
        _login = uiSprite.FindChild("login").GetComponent<Button>();
        _zc = uiSprite.FindChild("zc").GetComponent<Button>();
        _close = uiSprite.FindChild("CloseBtn").GetComponent<Button>();
        _name = uiSprite.FindChild("name").GetComponent<InputField>();
        _password = uiSprite.FindChild("password").GetComponent<InputField>();

        _login.onClick.AddListener(delegate
        {
            if (string.IsNullOrEmpty(_name.text) || string.IsNullOrEmpty(_password.text))
            {
                QuickTips.ShowQuickTipsNor("用户名或密码不能为空");
                return;
            }
                
            ProtoReq.LoginReq(GameConst.isGuest, _name.text.ToZhuan(), _name.text.ToZhuan(),0, "htttp://test", _password.text);
            //DestoryPanel();
            ClosePanel();
        });

        _zc.onClick.AddListener(delegate
        {
            if (string.IsNullOrEmpty(_name.text) || string.IsNullOrEmpty(_password.text))
            {
                QuickTips.ShowQuickTipsNor("用户名或密码不能为空");
                return;
            }
              
            ProtoReq.zc(_name.text, _password.text);
        });

        _close.onClick.AddListener(delegate
        {
            ClosePanel();
        });
    }
    public override void startUp(object obj = null)
    {
        base.startUp(obj);
        if ((int)obj == 1)
        {
            _login.gameObject.SetActive(false);
            _zc.gameObject.SetActive(true);
        }
        else
        {
            _login.gameObject.SetActive(true);
            _zc.gameObject.SetActive(false);
        }
        _name.text = "";
        _password.text = "";
    }
    public override void AddListener()
    {
        base.AddListener();
        EventDispatcher.Instance.AddEventListener(GameEventConst.REGISTED, onRegisted);
    }

    public override void RemoveListener()
    {
        base.RemoveListener();
        EventDispatcher.Instance.RemoveEventListener(GameEventConst.REGISTED, onRegisted);
    }

    private void onRegisted()
    {
        ClosePanel();
    }
}
