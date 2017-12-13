using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ZhanjiPanel : BasePanel
{

    private static ZhanjiPanel instance;
    public static ZhanjiPanel Instance
    {
        get {
            if (instance == null)
                instance = new ZhanjiPanel();
            return instance;
        }
    }

    private ZhanjiPanel()
    {
        base_name = PanelTag.ZHANJI_PANEL;
        _isPop = true;

    }

    private int _curPage = 1;
    private int _sumPage = 1;

    private int _curEndCard;

    private Button _zjClose;
    private Transform _zhanji;
    private Button _before;
    private Button _after;
    private Text _page;
    private Transform _zjGrid;
    private GameObject _zjItem;
    private GameObject _zjInfo;
    private GameObject _zjNone;

    private Transform _jilu;
    private Button _jlClose;
    private Transform _jlHead;
    private Transform _jlGrid;
    private GameObject _jlItem;

    private Transform _xiangqing;
    private GameObject _xqGrid;
    private Button _xqClose;
    private GameObject _xqCard;
    private GameObject _xqPeng;
    private GameObject _xqGang;

    public override void InitPanel(Transform uiSprite)
    {
        base.InitPanel(uiSprite);
        _zhanji = uiSprite.FindChild("zhanji");
        _zjClose = _zhanji.FindChild("close").GetComponent<Button>();
        _before = _zhanji.FindChild("info/before").GetComponent<Button>();
        _after = _zhanji.FindChild("info/after").GetComponent<Button>();
        _page = _zhanji.FindChild("info/page").GetComponent<Text>();
        _zjGrid = _zhanji.FindChild("info/list/grid");
        _zjItem = _zjGrid.FindChild("item").gameObject;
        _zjItem.SetActive(false);
        _zjInfo = _zhanji.FindChild("info").gameObject;
        _zjNone = _zhanji.FindChild("none").gameObject;

        _jilu = uiSprite.FindChild("jilu");
        _jlClose = _jilu.FindChild("close").GetComponent<Button>();
        _jlHead = _jilu.FindChild("head");
        _jlGrid = _jilu.FindChild("list/grid");
        _jlItem = _jlGrid.FindChild("item").gameObject;
        _jlItem.SetActive(false);

        _xiangqing = uiSprite.FindChild("xiangqing");
        _xqGrid = _xiangqing.FindChild("list/grid").gameObject;
        _xqClose = _xiangqing.FindChild("close").GetComponent<Button>();
        _xqCard = _xiangqing.FindChild("card").gameObject;
        _xqPeng = _xiangqing.FindChild("peng").gameObject;
        _xqGang = _xiangqing.FindChild("gang").gameObject;

        

        addClick();
    }

    public override void startUp(object obj = null)
    {
        base.startUp(obj);
        _zhanji.gameObject.SetActive(true);
        _jilu.gameObject.SetActive(false);
        _xiangqing.gameObject.SetActive(false);

        _zjInfo.gameObject.SetActive(false);
        _zjNone.gameObject.SetActive(true);
        ProtoReq.historyTableReq(_curPage);
    }

    public override void AddListener()
    {
        base.AddListener();
        EventDispatcher.Instance.AddEventListener(GameEventConst.HISTORY,onHistory);
        EventDispatcher.Instance.AddEventListener(GameEventConst.HISTORY_DETAIL, onHistoryDetail);
    }

    public override void RemoveListener()
    {
        base.RemoveListener();
        EventDispatcher.Instance.RemoveEventListener(GameEventConst.HISTORY, onHistory);
        EventDispatcher.Instance.RemoveEventListener(GameEventConst.HISTORY_DETAIL, onHistoryDetail);
    }
    private void addClick()
    {
        _zjClose.onClick.AddListener(delegate
        {
            //DestoryPanel();
            ClosePanel();
        });
        _jlClose.onClick.AddListener(delegate
        {
            _zhanji.gameObject.SetActive(true);
            _jilu.gameObject.SetActive(false);
        });
        _xqClose.onClick.AddListener(delegate
        {
            _xiangqing.gameObject.SetActive(false);
            _jilu.gameObject.SetActive(true);
        });
        _before.onClick.AddListener(delegate
        {
            if (_curPage > 1)
            {
                _curPage -= 1;
                ProtoReq.historyTableReq(_curPage);
            }
        });

        _after.onClick.AddListener(delegate
        {
            if (_curPage < _sumPage)
            {
                _curPage += 1;
                ProtoReq.historyTableReq(_curPage);
            }
        });
    }

    private void onHistory()
    {
        if (HistoryController.Instance.tableList == null || HistoryController.Instance.tableList.Count == 0)
        {
            _zjInfo.SetActive(false);
            _zjNone.SetActive(true);
        }
        else
        {
            _zjInfo.SetActive(true);
            _zjNone.SetActive(false);
            _curPage = HistoryController.Instance.page;
            _sumPage = HistoryController.Instance.sumPage;
            _page.text = _curPage + "/" + _sumPage;
            _zjGrid.destoryChild(true);
            for (int i = 0; i < HistoryController.Instance.tableList.Count; i++)
            {
                GameObject obj = GameObject.Instantiate(_zjItem, _zjItem.transform.parent,true) as GameObject;
                obj.transform.localScale = Vector3.one;
                obj.transform.parent = _zjItem.transform.parent;
                obj.name = HistoryController.Instance.tableList[i].index.ToString();
                obj.SetActive(true);
                obj.transform.FindChild("button").GetComponent<Button>().onClick.AddListener(delegate
                {
                    ProtoReq.historyDetailReq(int.Parse(obj.name));
                });
                obj.transform.FindChild("roomNum").GetComponent<Text>().text = "房间号：" + HistoryController.Instance.tableList[i].table_id;
                obj.transform.FindChild("date").GetComponent<Text>().text = "日期:" + HistoryController.Instance.tableList[i].start_time;
                IconMgr.Instance.SetImage(obj.transform.FindChild("index").GetComponent<Image>(), "index" + (i+1));
                GameObject objInfo = obj.transform.FindChild("info").gameObject;
                for (int j = 0; j < HistoryController.Instance.tableList[i].playerinfo_list.Count; j++)
                {
                    objInfo.transform.FindChild(j + "/name").GetComponent<Text>().text = HistoryController.Instance.tableList[i].playerinfo_list[j].name;
                    objInfo.transform.FindChild(j + "/id").GetComponent<Text>().text = "【"+HistoryController.Instance.tableList[i].playerinfo_list[j].id.IdEx()+"】";
                    int score = HistoryController.Instance.tableList[i].playerinfo_list[j].total_score;
                    if (score >= 0)
                    {
                        objInfo.transform.FindChild(j + "/score").GetComponent<Text>().text = "<color='#e83a1b'>+" + score + "</color>";

                    }
                    else
                    {
                        objInfo.transform.FindChild(j + "/score").GetComponent<Text>().text = "<color='#825a48'>" + score + "</color>";
                    }
                }
            }
        }
    }

    private void onHistoryDetail()
    {
        _zhanji.gameObject.SetActive(false);
        _jilu.gameObject.SetActive(true);

        for (int i = 0; i < HistoryController.Instance.curHistory.playerinfo_list.Count; i++)
        {
            _jlHead.FindChild("name" + i).GetComponent<Text>().text = HistoryController.Instance.curHistory.playerinfo_list[i].name;
            _jlHead.FindChild("id"+i).GetComponent<Text>().text = "ID:"+ HistoryController.Instance.curHistory.playerinfo_list[i].id.IdEx();
        }
        _jlGrid.destoryChild(true);
        for (int i = 0; i < HistoryController.Instance.roundScoreList.Count; i++)
        {
            GameObject obj = GameObject.Instantiate(_jlItem, _jlGrid,true)as GameObject;
            obj.transform.localScale = Vector3.one;
            int num = i+1;
            obj.name = num.ToString();
            obj.transform.parent = _jlGrid;
            obj.SetActive(true);
            setJlIndex(obj.transform.FindChild("index"), num);
            //IconMgr.Instance.SetImage(obj.transform.FindChild("index").GetComponent<Image>(), "index" + (num + 1));
            obj.transform.FindChild("time").GetComponent<Text>().text = HistoryController.Instance.roundScoreList[i].starttime;

            obj.transform.FindChild("ss").GetComponent<Button>().onClick.AddListener(delegate
            {
                _curEndCard = num - 1;
                _jilu.gameObject.SetActive(false);
                _xiangqing.gameObject.SetActive(true);
                showXiangQing();
            });

            for (int j = 0; j < HistoryController.Instance.roundScoreList[i].scores.Count; j++)
            {
                string str = "";
                if (HistoryController.Instance.roundScoreList[i].scores[j].hu == 1 || HistoryController.Instance.roundScoreList[i].scores[j].hu == 3)
                    str = "胡牌";
                else
                    str = "未胡牌";
                string score = "";
                if (HistoryController.Instance.roundScoreList[i].scores[j].score > 0)
                    score = "+" + HistoryController.Instance.roundScoreList[i].scores[j].score;
                else
                    score = HistoryController.Instance.roundScoreList[i].scores[j].score.ToString();
                obj.transform.FindChild("info" + j).GetComponent<Text>().text =
                    str + "\n" + score;
            }

        }
    }

    private void showXiangQing()
    {
        setHead();
        xiangqing();
        setMa();
    }

    private void xiangqing()
    {
        for (int k = 0; k<HistoryController.Instance.roundScoreList[_curEndCard].end_cards.Count; k++)
        {
            //put设置
            GameObject outObj = _xqGrid.transform.FindChild(k + "/out").gameObject;
            GameObject outItem = outObj.transform.FindChild("item").gameObject;
            outObj.transform.destoryChild(true);
            outItem.SetActive(false);

            for (int i = 0; i < HistoryController.Instance.roundScoreList[_curEndCard].end_cards[k].outcards.Count; i++)
            {
                GameObject copyItem = GameObject.Instantiate(outItem, outObj.transform,true) as GameObject;
                copyItem.transform.localScale = Vector3.one;
                copyItem.transform.parent = outObj.transform;
                IconMgr.Instance.SetImage(copyItem.transform.FindChild("value").GetComponent<Image>(), "shangmian1_" + HistoryController.Instance.roundScoreList[_curEndCard].end_cards[k].outcards[i]);
                copyItem.SetActive(true);
            }

            //in的碰杠设置

            GameObject inObj = _xqGrid.transform.FindChild(k + "/in").gameObject;
            inObj.transform.destoryChild();
            for (int i = 0; i < HistoryController.Instance.roundScoreList[_curEndCard].end_cards[k].pgdata.Count; i++)
            {
                GameObject copyItem = null;
                if (HistoryController.Instance.roundScoreList[_curEndCard].end_cards[k].pgdata[i].ptype == 0)
                {
                    copyItem = GameObject.Instantiate(_xqPeng, inObj.transform,true) as GameObject;
                    copyItem.transform.localScale = Vector3.one;
                    copyItem.transform.parent = inObj.transform;
                    copyItem.transform.localPosition = Vector3.zero;
                    for (int j = 1; j <= 3; j++)
                    {
                        IconMgr.Instance.SetImage(copyItem.transform.FindChild("value" + j).GetComponent<Image>(), "shangmian1_" + HistoryController.Instance.roundScoreList[_curEndCard].end_cards[k].pgdata[i].card);
                    }
                    copyItem.SetActive(true);
                }
                else
                {
                    copyItem = GameObject.Instantiate(_xqGang, inObj.transform,true)as GameObject;
                    copyItem.transform.localScale = Vector3.one;
                    copyItem.transform.parent = inObj.transform;
                    copyItem.transform.localPosition = Vector3.zero;
                    for (int j = 1; j <= 3; j++)
                    {
                        IconMgr.Instance.SetImage(copyItem.transform.FindChild("value" + j).GetComponent<Image>(), "shangmian1_" + HistoryController.Instance.roundScoreList[_curEndCard].end_cards[k].pgdata[i].card);
                    }
                    copyItem.SetActive(true);
                }
            }
            //in 的手牌设置
            for (int i = 0; i < HistoryController.Instance.roundScoreList[_curEndCard].end_cards[k].cards.Count; i++)
            {
                GameObject copyItem = GameObject.Instantiate(_xqCard, inObj.transform,true) as GameObject;
                copyItem.transform.localScale = Vector3.one;
                copyItem.transform.parent = inObj.transform;
                copyItem.transform.localPosition = Vector3.zero;
                IconMgr.Instance.SetImage(copyItem.transform.FindChild("value").GetComponent<Image>(), "shangmian1_" + HistoryController.Instance.roundScoreList[_curEndCard].end_cards[k].cards[i]);
                copyItem.SetActive(true);
            }

        }
    }

    private void setHead()
    {
        for (int i = 0; i < HistoryController.Instance.curHistory.playerinfo_list.Count; i++)
        {
            _xqGrid.transform.FindChild(i + "/name").GetComponent<Text>().text = HistoryController.Instance.curHistory.playerinfo_list[i].name;
            if (_xqGrid.transform.FindChild(i + "/head").GetComponent<Image>().sprite == null)
                SDKMgr.Instance.SetAsyncImage(HistoryController.Instance.curHistory.playerinfo_list[i].url, _xqGrid.transform.FindChild(i + "/head").GetComponent<Image>());
            else if (!_xqGrid.transform.FindChild(i + "/head").GetComponent<Image>().sprite.name.Equals(HistoryController.Instance.curHistory.playerinfo_list[i].url))
            {
                SDKMgr.Instance.SetAsyncImage(HistoryController.Instance.curHistory.playerinfo_list[i].url, _xqGrid.transform.FindChild(i + "/head").GetComponent<Image>());
            }
        }
    }

    private void setMa()
    {
        for (int i = 0; i < 4; i++)
        {
            _xqGrid.transform.FindChild(i + "/ma").transform.destoryChild(true);
        }
        if (HistoryController.Instance.jiangmaList==null|| HistoryController.Instance.jiangmaList.Count==0)
        {
            //买马
            if (HistoryController.Instance.maimaList == null || HistoryController.Instance.maimaList.Count == 0)
                return;
            if (HistoryController.Instance.maimaList[_curEndCard].maima_list == null || HistoryController.Instance.maimaList[_curEndCard].maima_list.Count == 0)
                return;
            for (int i = 0; i < HistoryController.Instance.maimaList[_curEndCard].maima_list.Count; i++)
            {
                GameObject obj = _xqGrid.transform.FindChild(i + "/ma/item").gameObject;
                if(HistoryController.Instance.maimaList[_curEndCard].maima_list[i].card1!=0)
                {
                    GameObject copyItem = GameObject.Instantiate(obj, obj.transform.parent,true) as GameObject;
                    copyItem.transform.localScale = Vector3.one;
                    copyItem.transform.parent = obj.transform.parent;
                    copyItem.SetActive(true);
                    IconMgr.Instance.SetImage(copyItem.transform.FindChild("value").GetComponent<Image>(), "shangmian1_" + HistoryController.Instance.maimaList[_curEndCard].maima_list[i].card1);
                    if (HistoryController.Instance.maimaList[_curEndCard].maima_list[i].zhong1==0)
                        copyItem.transform.FindChild("del").gameObject.SetActive(false);
                    else
                        copyItem.transform.FindChild("del").gameObject.SetActive(true);
                    copyItem.transform.FindChild("add").gameObject.SetActive(false);
                }
                if (HistoryController.Instance.maimaList[_curEndCard].maima_list[i].card2 != 0)
                {
                    GameObject copyItem = GameObject.Instantiate(obj, obj.transform.parent,true) as GameObject;
                    copyItem.transform.localScale = Vector3.one;
                    copyItem.transform.parent = obj.transform.parent;
                    copyItem.SetActive(true);
                    IconMgr.Instance.SetImage(copyItem.transform.FindChild("value").GetComponent<Image>(), "shangmian1_" + HistoryController.Instance.maimaList[_curEndCard].maima_list[i].card2);
                    if(HistoryController.Instance.maimaList[_curEndCard].maima_list[i].zhong2==0)
                        copyItem.transform.FindChild("del").gameObject.SetActive(false);
                    else
                        copyItem.transform.FindChild("del").gameObject.SetActive(true);
                    copyItem.transform.FindChild("add").gameObject.SetActive(false);
                }
            }
        }
        else
        {
            //奖马
            for (int i = 0; i < HistoryController.Instance.roundScoreList[_curEndCard].scores.Count; i++)
            {
                GameObject obj = _xqGrid.transform.FindChild(i + "/ma/item").gameObject;

                if (HistoryController.Instance.roundScoreList[_curEndCard].scores[i].hu == 1 || HistoryController.Instance.roundScoreList[_curEndCard].scores[i].hu == 3)
                {
                    foreach (var item in HistoryController.Instance.jiangmaList[_curEndCard].jiangma_list)
                    {
                        GameObject copyItem = GameObject.Instantiate(obj, obj.transform.parent,true) as GameObject;
                        copyItem.transform.localScale = Vector3.one;
                        copyItem.transform.parent = obj.transform.parent;
                        copyItem.SetActive(true);
                        IconMgr.Instance.SetImage(copyItem.transform.FindChild("value").GetComponent<Image>(), "shangmian1_" + item);
                        bool isFlag = false;
                        foreach (var score in HistoryController.Instance.roundScoreList[_curEndCard].scores[i].jiangma)
                        {
                            if (score == item)
                            {
                                isFlag = true;
                                break;
                            }
                           
                        }
                        if(isFlag)
                             copyItem.transform.FindChild("add").gameObject.SetActive(true);
                        else
                            copyItem.transform.FindChild("add").gameObject.SetActive(false);
                        copyItem.transform.FindChild("del").gameObject.SetActive(false);
                        
                    }
                }
                
            }
           
            //DanjuController.Instance._maima;
        }
    }

    private void setJlIndex(Transform tran, int num)
    {
        int num1 = num % 10;
        int num10 = num / 10;
        if (num10 == 0)
        {
            tran.FindChild("10").gameObject.SetActive(false);
        }
        else
        {
            tran.FindChild("10").gameObject.SetActive(true);
            IconMgr.Instance.SetImage(tran.FindChild("10").GetComponent<Image>(), "index" + num10);

        }
        tran.FindChild("1").gameObject.SetActive(true);
        IconMgr.Instance.SetImage(tran.FindChild("1").GetComponent<Image>(), "index" + num1);
    }
}
