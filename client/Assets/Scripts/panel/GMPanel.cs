using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class GMPanel : BasePanel {

    private static GMPanel instance;

    public static GMPanel Instance
    {
        get {
            if (instance == null)
                instance = new GMPanel();
            return instance;
        }
    }
    private GMPanel()
    {
        base_name = PanelTag.GM_PANEL;
        _isPop = true;
    }

    private Button _stopServer;
    private Button _openServer;
    private Button _button;
    private Button _button1;
    private Button _button2;
    private Button _button3;
    private Button _button4;
    private Button _button5;
    private Button _button6;
    private Button _button7;

    private GameObject _panel;
    private GameObject _panel1;
    private GameObject _panel2;
    private GameObject _panel3;
    private GameObject _panel4;
    private GameObject _panel5;
    private GameObject _panel6;
    private GameObject _panel7;

    private InputField _curText;

    private Button _up;
    private Button _down;
    private Text _page;
    private int index = 0;
    private int page = 0;


    private Text numText;

    public override void InitPanel(Transform uiSprite)
    {
        base.InitPanel(uiSprite);
        _stopServer = uiSprite.FindChild("stopServer").GetComponent<Button>();
        _openServer = uiSprite.FindChild("openServer").GetComponent<Button>();

        _button = uiSprite.FindChild("Button").GetComponent<Button>();
        _button1 = uiSprite.FindChild("Button1").GetComponent<Button>();
        _button2 = uiSprite.FindChild("Button2").GetComponent<Button>();
        _button3 = uiSprite.FindChild("Button3").GetComponent<Button>();
        _button4 = uiSprite.FindChild("Button4").GetComponent<Button>();
        _button5 = uiSprite.FindChild("Button5").GetComponent<Button>();
        _button6 = uiSprite.FindChild("Button6").GetComponent<Button>();
        _button7 = uiSprite.FindChild("Button7").GetComponent<Button>();

        _panel = uiSprite.FindChild("Panel").gameObject;
        _panel.SetActive(false);
        _panel1 = uiSprite.FindChild("Panel1").gameObject;
        _panel1.SetActive(false);
        _panel2 = uiSprite.FindChild("Panel2").gameObject;
        _panel2.SetActive(false);
        _panel3 = uiSprite.FindChild("Panel3").gameObject;
        _panel3.SetActive(false);

        _up = _panel3.transform.FindChild("up").GetComponent<Button>();
        _down = _panel3.transform.FindChild("down").GetComponent<Button>();
        _page = _panel3.transform.FindChild("page").GetComponent<Text>();

        _panel4 = uiSprite.FindChild("Panel4").gameObject;
        _panel4.SetActive(false);
        _panel5 = uiSprite.FindChild("Panel5").gameObject;
        _panel5.SetActive(false);
        _panel6 = uiSprite.FindChild("Panel6").gameObject;
        _panel6.SetActive(false);
        _panel7 = uiSprite.FindChild("Panel7").gameObject;
        _panel7.SetActive(false);

        numText = uiSprite.FindChild("num").GetComponent<Text>();

        uiSprite.FindChild("close").GetComponent<Button>().onClick.AddListener(delegate
        {
            //this.DestoryPanel();
            ClosePanel();
        });
        addClick();
        setPanel();
        setPanel2();
        setPanel3();
        setPanel4();
        setPanel5();
        setPanel6();
        setPanel7();
    }

    public override void startUp(object obj = null)
    {
        base.startUp(obj);
        index = 0;
        page = 0;
    }

    private void addClick()
    {
        _stopServer.onClick.AddListener(delegate
        {
            ProtoReq.GMStopServer(true);
        });

        _openServer.onClick.AddListener(delegate
        {
            ProtoReq.GMStopServer(false);
        });

        _button.onClick.AddListener(delegate
        {
            _panel.SetActive(true);
            _panel1.SetActive(false);
            _panel2.SetActive(false);
            _panel3.SetActive(false);
            _panel4.SetActive(false);
            _panel5.SetActive(false);
            _panel6.SetActive(false);
            _panel7.SetActive(false);
            _curText = _panel.transform.FindChild("info").GetComponent<InputField>();
        });
        _button1.onClick.AddListener(delegate
        {
            _panel.SetActive(false);
            _panel1.SetActive(true);
            _panel2.SetActive(false);
            _panel3.SetActive(false);
            _panel4.SetActive(false);
            _panel5.SetActive(false);
            _panel6.SetActive(false);
            _panel7.SetActive(false);
            ProtoReq.GMQueryAgentList();
            _curText = _panel1.transform.FindChild("info").GetComponent<InputField>();
        });
        _button2.onClick.AddListener(delegate
        {
            _panel.SetActive(false);
            _panel1.SetActive(false);
            _panel2.SetActive(true);
            _panel3.SetActive(false);
            _panel4.SetActive(false);
            _panel5.SetActive(false);
            _panel6.SetActive(false);
            _panel7.SetActive(false);
            _curText = _panel2.transform.FindChild("info").GetComponent<InputField>();
        });
        _button3.onClick.AddListener(delegate
        {
            _panel.SetActive(false);
            _panel1.SetActive(false);
            _panel2.SetActive(false);
            _panel3.SetActive(true);
            _panel4.SetActive(false);
            _panel5.SetActive(false);
            _panel6.SetActive(false);
            _panel7.SetActive(false);
            _curText = _panel3.transform.FindChild("info").GetComponent<InputField>();
        });
        _button4.onClick.AddListener(delegate
        {
            _panel.SetActive(false);
            _panel1.SetActive(false);
            _panel2.SetActive(false);
            _panel3.SetActive(false);
            _panel4.SetActive(true);
            _panel5.SetActive(false);
            _panel6.SetActive(false);
            _panel7.SetActive(false);
            _curText = _panel4.transform.FindChild("info").GetComponent<InputField>();
        });
        _button5.onClick.AddListener(delegate
        {
            _panel.SetActive(false);
            _panel1.SetActive(false);
            _panel2.SetActive(false);
            _panel3.SetActive(false);
            _panel4.SetActive(false);
            _panel5.SetActive(true);
            _panel6.SetActive(false);
            _panel7.SetActive(false);
            _curText = _panel5.transform.FindChild("info").GetComponent<InputField>();
        });
        _button6.onClick.AddListener(delegate
        {
            _panel.SetActive(false);
            _panel1.SetActive(false);
            _panel2.SetActive(false);
            _panel3.SetActive(false);
            _panel4.SetActive(false);
            _panel5.SetActive(false);
            _panel6.SetActive(true);
            _panel7.SetActive(false);
            _curText = _panel6.transform.FindChild("info").GetComponent<InputField>();
        });
        _button7.onClick.AddListener(delegate
        {
            _panel.SetActive(false);
            _panel1.SetActive(false);
            _panel2.SetActive(false);
            _panel3.SetActive(false);
            _panel4.SetActive(false);
            _panel5.SetActive(false);
            _panel6.SetActive(false);
            _panel7.SetActive(true);
            _curText = _panel7.transform.FindChild("info").GetComponent<InputField>();
            ProtoReq.GMQuerySysInfo("");
        });

        
    }

    private void setPanel()
    {
        _panel.transform.FindChild("Button").GetComponent<Button>().onClick.AddListener(delegate
        {
            ProtoReq.GMQuerySinglePlayer(int.Parse(_panel.transform.FindChild("id").GetComponent<InputField>().text).IdEx());
        });
    }

    private void setPanel2()
    {
        _panel2.transform.FindChild("Button").GetComponent<Button>().onClick.AddListener(delegate
        {
            ProtoReq.GMQueryCoinPlayer(int.Parse(_panel2.transform.FindChild("coin").GetComponent<InputField>().text));
        });
    }
    private void setPanel3()
    {
        _panel3.transform.FindChild("Button").GetComponent<Button>().onClick.AddListener(delegate
        {
            index = 0;
            ProtoReq.GMQueryMoneyLog(int.Parse(_panel3.transform.FindChild("id").GetComponent<InputField>().text).IdEx(),_panel3.transform.FindChild("Toggle").GetComponent<Toggle>().isOn);
        });

        _up.onClick.AddListener(delegate
        {
            if (index >= 6)
                ProtoReq.GMQueryMoneyLog(MainRole.Instance.Id, true, index - 6);
            page = -1;
        });

        _down.onClick.AddListener(delegate
        {
            ProtoReq.GMQueryMoneyLog(MainRole.Instance.Id, true, index + 6);
            page = 1;
        });
    }
    private void setPanel4()
    {
        _panel4.transform.FindChild("Button").GetComponent<Button>().onClick.AddListener(delegate
        {
            ProtoReq.GMQuerySingleGMLog(int.Parse(_panel4.transform.FindChild("index").GetComponent<InputField>().text));
        });
    }
    private void setPanel5()
    {
        _panel5.transform.FindChild("Button").GetComponent<Button>().onClick.AddListener(delegate
        {
            ProtoReq.addCoin(int.Parse(_panel5.transform.FindChild("id").GetComponent<InputField>().text).IdEx(), int.Parse(_panel5.transform.FindChild("coin").GetComponent<InputField>().text));
        });
    }
    private void setPanel6()
    {
        _panel6.transform.FindChild("Button").GetComponent<Button>().onClick.AddListener(delegate
        {
            ProtoReq.addAgent(int.Parse(_panel6.transform.FindChild("id").GetComponent<InputField>().text).IdEx(), _panel6.transform.FindChild("Toggle").GetComponent<Toggle>().isOn);
        });
    }
    private void setPanel7()
    {
        _panel7.transform.FindChild("Button").GetComponent<Button>().onClick.AddListener(delegate
        {
            ProtoReq.GMDesRoom(int.Parse(_panel7.transform.FindChild("id").GetComponent<InputField>().text));
        });
    }
    public override void AddListener()
    {
        base.AddListener();
        EventDispatcher.Instance.AddEventListener<int,List<string>>(GameEventConst.GMPANEL, onShowPanelText);
        EventDispatcher.Instance.AddEventListener<int,List<string>>(GameEventConst.GM_MONEYLOG, onShowPage);
    }

    public override void RemoveListener()
    {
        base.RemoveListener();
        EventDispatcher.Instance.RemoveEventListener<int,List<string>>(GameEventConst.GMPANEL, onShowPanelText);
        EventDispatcher.Instance.RemoveEventListener<int,List<string>>(GameEventConst.GM_MONEYLOG, onShowPage);
    }
    private void ShowNum(int num)
    {
        if (num == 0)
            numText.text = "";
        else
            numText.text = "总数"+num;
    }
    private void onShowPanelText(int num,List<string> list)
    {
        _curText.text = "";
        for (int i = 0; i < list.Count; i++)
        {
            _curText.text += (list[i] + "\n");
        }
        ShowNum(num);
    }
    private void onShowPage(int num,List<string> list)
    {
        _curText.text = "";
        if (list != null && list.Count > 0)
        {
            if (page == 1)
            {
                index += 6;
            }
            else if (page == -1)
            {
                index -= 6;
            }
            for (int i = 0; i < list.Count; i++)
            {
                _curText.text += (list[i] + "\n");
            }
            _page.text = ((index / 6) + 1).ToString();
        }
        ShowNum(num);
    }
    public override void OnClose()
    {
        base.OnClose();
    }
}
