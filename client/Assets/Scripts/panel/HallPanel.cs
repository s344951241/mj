using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HallPanel : BasePanel
{
    private static HallPanel instance;
    public static HallPanel Instance
    {
        get
        {
            if (instance == null)
                instance = new HallPanel();
            return instance;
        }

    }
    private HallPanel()
    {
        base_name = PanelTag.HALL_PANEL;
        _isPop = false;
    }

    private Button _create;
    private Button _join;
    private Button _add;
    private Image _head;
    private Text _name;
    private Text _id;
    private Text _coin;
    private Button _share;
    private Button _zhanji;
    private Button _wanfa;
    private Button _set;

    private GameObject _sharePanel;
    private Button _weixin;
    private Button _pengyou;
    private Button _shareClose;

    private GameObject _createRoom;

    private int _playoffs = 0;
    private int _times = 0;
    private int _jiangma = 0;
    private int _maima = 0;

    private Toggle _times0;
    private Toggle _times1;

    private Toggle _playoffs0;
    private Toggle _playoffs1;
    private Toggle _playoffs2;

    private Toggle _jiangma0;
    private Toggle _jiangma1;
    private Toggle _jiangma2;
    private Toggle _jiangma3;

    private Toggle _maima0;
    private Toggle _maima1;
    private Toggle _maima2;

    private GameObject _input;
    private Button _button;
    private Button _close;
    private Button _closeCreate;
    private int _roomNum;
    private Button _button0;
    private Button _button1;
    private Button _button2;
    private Button _button3;
    private Button _button4;
    private Button _button5;
    private Button _button6;
    private Button _button7;
    private Button _button8;
    private Button _button9;
    private Button _buttonDel;
    private Button _buttonRes;
    private GameObject _shuruGrid;
    private GameObject _shuruItem;

    private Button _createButton;

    private Button _GM;
    private Button _daili;

    private GameObject _roleInfo;

    private int indexCount;
    public override void InitPanel(Transform uiSprite)
    {
        base.InitPanel(uiSprite);
        _GM = uiSprite.transform.FindChild("GM").GetComponent<Button>();
        _daili = uiSprite.transform.FindChild("daili").GetComponent<Button>();
        _GM.onClick.AddListener(delegate
        {
            GMPanel.Instance.load();
        });
        _daili.onClick.AddListener(delegate
        {
            DailiPanel.Instance.load();
        });

        if (GameConst.isGM)
        {
#if UNITY_EDITOR
            _GM.gameObject.SetActive(true);
#endif
        }
        else
        {
            _GM.gameObject.SetActive(false);
        }

        if (GameConst.isDaili)
        {
            _daili.gameObject.SetActive(true);
        }
        else
        {
            _daili.gameObject.SetActive(false);
        }
        _create = uiSprite.transform.FindChild("create").GetComponent<Button>();
        _join = uiSprite.transform.FindChild("join").GetComponent<Button>();
        _input = uiSprite.transform.FindChild("input").gameObject;

        _button = _input.transform.FindChild("confirm").GetComponent<Button>();
        _close = _input.transform.FindChild("close").GetComponent<Button>();

        _button0 = _input.transform.FindChild("anniu/button0").GetComponent<Button>();
        _button1 = _input.transform.FindChild("anniu/button1").GetComponent<Button>();
        _button2 = _input.transform.FindChild("anniu/button2").GetComponent<Button>();
        _button3 = _input.transform.FindChild("anniu/button3").GetComponent<Button>();
        _button4 = _input.transform.FindChild("anniu/button4").GetComponent<Button>();
        _button5 = _input.transform.FindChild("anniu/button5").GetComponent<Button>();
        _button6 = _input.transform.FindChild("anniu/button6").GetComponent<Button>();
        _button7 = _input.transform.FindChild("anniu/button7").GetComponent<Button>();
        _button8 = _input.transform.FindChild("anniu/button8").GetComponent<Button>();
        _button9 = _input.transform.FindChild("anniu/button9").GetComponent<Button>();
        _buttonDel = _input.transform.FindChild("anniu/buttonDel").GetComponent<Button>();
        _buttonRes = _input.transform.FindChild("anniu/buttonRes").GetComponent<Button>();
        _shuruGrid = _input.transform.FindChild("shuru/grid").gameObject;
        _shuruItem = _shuruGrid.transform.FindChild("item").gameObject;

        _sharePanel = uiSprite.transform.FindChild("sharePanel").gameObject;
        _weixin = _sharePanel.transform.FindChild("weixin").GetComponent<Button>();
        _pengyou = _sharePanel.transform.FindChild("pengyouquan").GetComponent<Button>();
        _shareClose = _sharePanel.transform.FindChild("close").GetComponent<Button>();
        
        _createRoom = uiSprite.transform.FindChild("createRoom").gameObject;
        _closeCreate = _createRoom.transform.FindChild("close").GetComponent<Button>();
        _times0 = _createRoom.transform.FindChild("jsGroup/10").GetComponent<Toggle>();
        _times1 = _createRoom.transform.FindChild("jsGroup/11").GetComponent<Toggle>();

        _playoffs0 = _createRoom.transform.FindChild("fdGroup/20").GetComponent<Toggle>();
        _playoffs1 = _createRoom.transform.FindChild("fdGroup/21").GetComponent<Toggle>();
        _playoffs2 = _createRoom.transform.FindChild("fdGroup/22").GetComponent<Toggle>();

        _jiangma0 = _createRoom.transform.FindChild("jmGroup/30").GetComponent<Toggle>();
        _jiangma1 = _createRoom.transform.FindChild("jmGroup/31").GetComponent<Toggle>();
        _jiangma2 = _createRoom.transform.FindChild("jmGroup/32").GetComponent<Toggle>();
        _jiangma3 = _createRoom.transform.FindChild("jmGroup/33").GetComponent<Toggle>();

        _maima0 = _createRoom.transform.FindChild("wmGroup/40").GetComponent<Toggle>();
        _maima1 = _createRoom.transform.FindChild("wmGroup/41").GetComponent<Toggle>();
        _maima2 = _createRoom.transform.FindChild("wmGroup/42").GetComponent<Toggle>();

        _roleInfo = uiSprite.FindChild("roleInfo").gameObject;

        _createButton = _createRoom.transform.FindChild("Button").GetComponent<Button>();
        _createButton.onClick.AddListener(delegate
        {
            createTable();
        });

        _times0.isOn = true;
        _times1.isOn = false;
        _playoffs0.isOn = true;
        _playoffs1.isOn = false;
        _playoffs2.isOn = false;
        _jiangma0.isOn = true;
        _jiangma1.isOn = false;
        _jiangma2.isOn = false;
        _jiangma3.isOn = false;
        _maima0.isOn = true;
        _maima1.isOn = false;
        _maima2.isOn = false;

        _times0.onValueChanged.AddListener(delegate
        {
            //_times0.isOn = true;
            //_times1.isOn = false;
            _times = 0;
        });
        _times1.onValueChanged.AddListener(delegate
        {
            //_times1.isOn = true;
            //_times0.isOn = false;
            _times = 1;
        });
        _playoffs0.onValueChanged.AddListener(delegate
        {
            //_playoffs0.isOn = true;
            //_playoffs1.isOn = false;
            //_playoffs2.isOn = false;
            _playoffs = 0;
        });
        _playoffs1.onValueChanged.AddListener(delegate
        {
            //_playoffs0.isOn = false;
            //_playoffs1.isOn = true;
            //_playoffs2.isOn = false;
            _playoffs = 1;

        });
        _playoffs2.onValueChanged.AddListener(delegate
        {
            //_playoffs0.isOn = false;
            //_playoffs1.isOn = false;
            //_playoffs2.isOn = true;
            _playoffs = 2;
        });

        UIEventHandlerBase.AddListener(_jiangma0.gameObject, UIEventType.ON_POINTER_CLICK,delegate (GameObject obj,BaseEventData data)
        {
            _jiangma0.isOn = true;
            _jiangma1.isOn = false;
            _jiangma2.isOn = false;
            _jiangma3.isOn = false;
            _jiangma = 0;
        });

        UIEventHandlerBase.AddListener(_jiangma1.gameObject, UIEventType.ON_POINTER_CLICK, delegate (GameObject obj, BaseEventData data)
        {
            _jiangma0.isOn = false;
            _jiangma1.isOn = true;
            _jiangma2.isOn = false;
            _jiangma3.isOn = false;
            _jiangma = 1;
            _maima0.isOn = true;
            _maima1.isOn = false;
            _maima2.isOn = false;
            _maima = 0;
        });

        UIEventHandlerBase.AddListener(_jiangma2.gameObject, UIEventType.ON_POINTER_CLICK, delegate (GameObject obj, BaseEventData data)
        {
            _jiangma0.isOn = false;
            _jiangma1.isOn = false;
            _jiangma2.isOn = true;
            _jiangma3.isOn = false;
            _jiangma = 2;
            _maima0.isOn = true;
            _maima1.isOn = false;
            _maima2.isOn = false;
            _maima = 0;
        });
        UIEventHandlerBase.AddListener(_jiangma3.gameObject, UIEventType.ON_POINTER_CLICK, delegate (GameObject obj, BaseEventData data)
        {
            _jiangma0.isOn = false;
            _jiangma1.isOn = false;
            _jiangma2.isOn = false;
            _jiangma3.isOn = true;
            _jiangma = 3;
            _maima0.isOn = true;
            _maima1.isOn = false;
            _maima2.isOn = false;
            _maima = 0;
        });

        UIEventHandlerBase.AddListener(_maima0.gameObject, UIEventType.ON_POINTER_CLICK, delegate (GameObject obj, BaseEventData data)
        {
            _maima0.isOn = true;
            _maima1.isOn = false;
            _maima2.isOn = false;
            _maima = 0;
        });

        UIEventHandlerBase.AddListener(_maima1.gameObject, UIEventType.ON_POINTER_CLICK, delegate (GameObject obj, BaseEventData data)
        {
            _maima0.isOn = false;
            _maima1.isOn = true;
            _maima2.isOn = false;
            _jiangma0.isOn = true;
            _jiangma1.isOn = false;
            _jiangma2.isOn = false;
            _jiangma3.isOn = false;
            _maima = 1;
            _jiangma = 0;
        });
        UIEventHandlerBase.AddListener(_maima2.gameObject, UIEventType.ON_POINTER_CLICK, delegate (GameObject obj, BaseEventData data)
        {
            _maima0.isOn = false;
            _maima1.isOn = false;
            _maima2.isOn = true;
            _jiangma0.isOn = true;
            _jiangma1.isOn = false;
            _jiangma2.isOn = false;
            _jiangma3.isOn = false;
            _maima = 2;
            _jiangma = 0;
        });
      

        _create.onClick.AddListener(delegate
        {
            // createTable(1,2,3,4,"123");
            createRoom();

        });

        _join.onClick.AddListener(delegate
        {
            _input.SetActive(true);
        });

        _button.onClick.AddListener(delegate
        {
            if (_roomNum == 0)
            {
                Debug.Log("请输入正确房间号");
            }
            else
            {
                GameConst.tableId = _roomNum;
                joinTable(_roomNum);
                Debug.Log("num" + _roomNum);
            }
        });

        _head = uiSprite.FindChild("info/head").GetComponent<Image>();

        SDKMgr.Instance.SetAsyncImage(MainRole.Instance.Url, _head);
        SDKMgr.Instance.SetAsyncImage(MainRole.Instance.Url, _roleInfo.transform.FindChild("head").GetComponent<Image>());

        _name = uiSprite.FindChild("info/name").GetComponent<Text>();
        _name.text = MainRole.Instance.Name;
        _id = uiSprite.FindChild("info/id").GetComponent<Text>();
        _id.text = "ID:"+MainRole.Instance.Id.IdEx();

        _roleInfo.transform.FindChild("name").GetComponent<Text>().text = MainRole.Instance.Name;
        _roleInfo.transform.FindChild("id").GetComponent<Text>().text = "ID:" + MainRole.Instance.Id.IdEx();
        _roleInfo.transform.FindChild("GameObject").GetComponent<Button>().onClick.AddListener(delegate
        {
            _roleInfo.SetActive(false);
        });
        _roleInfo.SetActive(false);
        uiSprite.FindChild("info").GetComponent<Button>().onClick.AddListener(delegate
        {
            indexCount++;
            if (indexCount == 3)
            {
                indexCount = 0;
                GameConst.driver.gameObject.AddComponent<FPS>();
            }
            _roleInfo.SetActive(true);
        });
        _coin = uiSprite.FindChild("coin/num").GetComponent<Text>();
        _add = uiSprite.FindChild("coin/add").GetComponent<Button>();
        _add.onClick.AddListener(delegate {
            if (GameConst.isGuest)
            {
                ChongzhiPanel.Instance.load();
            }
            else
            {
                CoinPanel.Instance.load();
            }
        });

        _share = uiSprite.FindChild("share").GetComponent<Button>();
        _share.onClick.AddListener(delegate
        {
            _sharePanel.SetActive(true);
        });

        _zhanji = uiSprite.FindChild("zhanji").GetComponent<Button>();
        _zhanji.onClick.AddListener(delegate
        {
            ZhanjiPanel.Instance.load();
        });

        _wanfa = uiSprite.FindChild("wanfa").GetComponent<Button>();
        _wanfa.onClick.AddListener(delegate
        {
            InfoPanel.Instance.load();
        });

        _set = uiSprite.FindChild("set").GetComponent<Button>();
        _set.onClick.AddListener(delegate
        {
            SetPanel.Instance.load(1);
        });

        _input.SetActive(false);
        _createRoom.SetActive(false);

        _close.onClick.AddListener(delegate
        {
            _input.SetActive(false);
            if (_shuruGrid != null)
            {
                for (int i = 1; i < _shuruGrid.transform.childCount; i++)
                {
                    GameObject.Destroy(_shuruGrid.transform.GetChild(i).gameObject);
                }
            }
            _roomNum = 0;
        });
        _closeCreate.onClick.AddListener(delegate
        {
            _createRoom.SetActive(false);
        });
        _button0.onClick.AddListener(delegate
        {
            shuru(0);
        });
        _button1.onClick.AddListener(delegate
        {
            shuru(1);
        });
        _button2.onClick.AddListener(delegate
        {
            shuru(2);
        });
        _button3.onClick.AddListener(delegate
        {
            shuru(3);
        });
        _button4.onClick.AddListener(delegate
        {
            shuru(4);
        });
        _button5.onClick.AddListener(delegate
        {
            shuru(5);
        });
        _button6.onClick.AddListener(delegate
        {
            shuru(6);
        });
        _button7.onClick.AddListener(delegate
        {
            shuru(7);
        });
        _button8.onClick.AddListener(delegate
        {
            shuru(8);
        });
        _button9.onClick.AddListener(delegate
        {
            shuru(9);
        });
        _buttonDel.onClick.AddListener(delegate {
            if (_roomNum != 0)
            {
                GameObject.Destroy(_shuruGrid.transform.GetChild(_shuruGrid.transform.childCount - 1).gameObject);
                _roomNum = _roomNum / 10;
            }
        });
        _buttonRes.onClick.AddListener(delegate
        {
            for (int i = 1; i < _shuruGrid.transform.childCount; i++)
            {
                GameObject.Destroy(_shuruGrid.transform.GetChild(i).gameObject);
            }
            _roomNum = 0;
        });

        _weixin.onClick.AddListener(delegate
        {
            SDKMgr.Instance.DoShareTestFriend();
        });

        _pengyou.onClick.AddListener(delegate
        {
            SDKMgr.Instance.DoShareTest();
        });
        _shareClose.onClick.AddListener(delegate
        {
            _sharePanel.SetActive(false);
        });
    }
    public override void startUp(object obj = null)
    {
        base.startUp(obj);
        indexCount = 0;
        _coin.text = DataMgr.Instance.money.ToString();
        _input.SetActive(false);
        _createRoom.SetActive(false);
        _sharePanel.SetActive(false);
        if (_shuruGrid != null)
        {
            for (int i = 1; i < _shuruGrid.transform.childCount; i++)
            {
                GameObject.Destroy(_shuruGrid.transform.GetChild(i).gameObject);
            }
        }
        _roomNum = 0;

        if (GameConst.isGuest)
        {
            _share.gameObject.SetActive(false);
        }
            
    }
    public override void AddListener()
    {
        base.AddListener();
        EventDispatcher.Instance.AddEventListener(GameEventConst.COIN_CHANGED, coinChanged);
    }

    public override void RemoveListener()
    {
        base.RemoveListener();
        EventDispatcher.Instance.RemoveEventListener(GameEventConst.COIN_CHANGED, coinChanged);
    }
    private void coinChanged()
    {
        if(_coin!=null)
             _coin.text = DataMgr.Instance.money.ToString();
    }
    private void shuru(int num)
    {
        if (_roomNum >= 100000)
        {
            return;
        }
        GameObject item = GameObject.Instantiate(_shuruItem, _shuruItem.transform.parent,true) as GameObject;
        IconMgr.Instance.SetImage(item.transform.FindChild("num").GetComponent<Image>(), num.ToString());
        item.SetActive(true);
        _roomNum = _roomNum * 10 + num;
    }

    private void createRoom()
    {
        _createRoom.SetActive(true);
    }


    private void createTable()
    {
        ProtoReq.CreateTable(_playoffs, _times, _jiangma, _maima, "123");
    }
    private void joinTable(int id)
    {
        ProtoReq.JoinTable(id);
    }
}
