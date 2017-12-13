using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class DailiPanel : BasePanel
{

    private static DailiPanel instance;
    public static DailiPanel Instance
    {
        get
        {
            if (instance == null)
                instance = new DailiPanel();
            return instance;
        }
    }

    private DailiPanel()
    {
        base_name = PanelTag.DAILI_PANEL;
        _isPop = true;
    }

    private Button _close;
    private Transform _panel;
    private Transform _right;
    private InputField _idInput;
    private Button _confirm;
    private GameObject _roleNone;
    private GameObject _roleInfo;
    private Text _num;
    private InputField _coinInput;
    private Button _set;
    private GameObject _warning;

    private GameObject _grid;
    private GameObject _item;
    private Button _up;
    private Button _down;
    private Text _page;

    private int index = 0;
    private int page = 0;
    public override void InitPanel(Transform uiSprite)
    {
        base.InitPanel(uiSprite);
        _close = uiSprite.FindChild("close").GetComponent<Button>();

        _panel = uiSprite.FindChild("panel");
        _right = uiSprite.FindChild("right");
        _idInput = _panel.FindChild("idInput").GetComponent<InputField>();
        _confirm = _panel.FindChild("confirm").GetComponent<Button>();
        _roleNone = _panel.FindChild("role/none").gameObject;
        _roleInfo = _panel.FindChild("role/info").gameObject;
        _num = _panel.FindChild("num").GetComponent<Text>();
        _coinInput = _panel.FindChild("coinInput").GetComponent<InputField>();
        _set = _panel.FindChild("set").GetComponent<Button>();
        _warning = _panel.FindChild("warning").gameObject;
        _warning.SetActive(false);
        _grid = _right.FindChild("grid").gameObject;
        _item = _grid.transform.FindChild("item").gameObject;
        _item.SetActive(false);
        _up = _right.FindChild("up").GetComponent<Button>();
        _down = _right.FindChild("down").GetComponent<Button>();
        _page = _right.FindChild("page/num").GetComponent<Text>();

        addClick();
    }

    public override void startUp(object obj = null)
    {
        base.startUp(obj);
        _num.text = "代理员当前钻石总数量为：" + DataMgr.Instance.money;
        index = 0;
        page = 0;
        ProtoReq.GMQueryMoneyLog(MainRole.Instance.Id, true);
    }
    private void addClick()
    {
        _close.onClick.AddListener(delegate
        {
            ClosePanel();
        });

        _confirm.onClick.AddListener(delegate
        {
            if (_idInput.text.Equals(""))
            {
                QuickTips.ShowRedQuickTips("请输入玩家id");
                return;
            }
            ProtoReq.GMQuerySinglePlayer(int.Parse(_idInput.text).IdEx());
        });

        _set.onClick.AddListener(delegate
        {
            if (_idInput.text.Equals(""))
            {
                QuickTips.ShowRedQuickTips("请输入玩家id");
                return;
            }

            if (int.Parse(_coinInput.text) > DataMgr.Instance.money)
            {
                _warning.SetActive(true);
                return;
            }
            _warning.SetActive(false);
            ProtoReq.addCoin(int.Parse(_idInput.text).IdEx(), int.Parse(_coinInput.text));
            _idInput.text = "";
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


    public override void AddListener()
    {
        base.AddListener();
        EventDispatcher.Instance.AddEventListener<List<Table.GMPlayerInfo>>(GameEventConst.DAILI_PLAYER, onPlayer);
        EventDispatcher.Instance.AddEventListener<List<Table.GMMoneyLog>>(GameEventConst.DAILI_MONEY, onMoneyLog);
        EventDispatcher.Instance.AddEventListener(GameEventConst.COIN_CHANGED, onCoin);
    }
    public override void RemoveListener()
    {
        base.RemoveListener();
        EventDispatcher.Instance.RemoveEventListener<List<Table.GMPlayerInfo>>(GameEventConst.DAILI_PLAYER, onPlayer);
        EventDispatcher.Instance.RemoveEventListener<List<Table.GMMoneyLog>>(GameEventConst.DAILI_MONEY, onMoneyLog);
        EventDispatcher.Instance.RemoveEventListener(GameEventConst.COIN_CHANGED, onCoin);
    }
    private void onCoin()
    {
        _num.text = "代理员当前钻石总数量为：" + DataMgr.Instance.money;
    }
    private void onPlayer(List<Table.GMPlayerInfo> list)
    {
        if (list == null || list.Count == 0)
        {
            _roleNone.SetActive(true);
            _roleInfo.SetActive(false);
        }
        else if (list.Count == 1)
        {
            _roleInfo.SetActive(true);
            _roleNone.SetActive(false);

            _roleInfo.transform.FindChild("name").GetComponent<Text>().text = list[0].name;
            _roleInfo.transform.FindChild("id").GetComponent<Text>().text = "ID:"+list[0].id.IdEx();
        }
    }

    private void onMoneyLog(List<Table.GMMoneyLog> log)
    {
        if (log != null && log.Count > 0)
        {
            if (page == 1)
            {
                index += 6;
            }
            else if(page==-1)
            {
                index -= 6;
            }
            _item.SetActive(false);
            _grid.transform.destoryChild(true);
            for (int i = 0; i < log.Count; i++)
            {
                GameObject obj = GameObject.Instantiate(_item, _grid.transform, true) as GameObject;
                obj.transform.FindChild("name").GetComponent<Text>().text = "时间"+log[i].optime+"  流水号："+ log[i].index;
                obj.transform.FindChild("id").GetComponent<Text>().text = log[i].toid.IdEx().ToString();
                obj.transform.FindChild("coin").GetComponent<Text>().text = log[i].value + "钻石";
                obj.SetActive(true);
            }
            _page.text = ((index / 6) + 1).ToString();
        }
    }
}
