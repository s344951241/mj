using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class JiesuanPanel : BasePanel
{

    private static JiesuanPanel instance;
    public static JiesuanPanel Instance
    {
        get
        {
            if (instance == null)
                instance = new JiesuanPanel();
            return instance;
        }
    }

    private JiesuanPanel()
    {
        base_name = PanelTag.JIESUAN_PANEL;
        _isPop = true;
    }

    private GameObject _jiesuan;
    private Button _jsclose;
    private Button _jsxiangqing;
    private Button _jsfenxiang;
    private Button _jsjixu;
    private GameObject _jsgrid;

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

    private int index = 0;
    private int _curEndCard;

    private bool _isShare;

    public override void InitPanel(Transform uiSprite)
    {
        base.InitPanel(uiSprite);
        _jiesuan = uiSprite.FindChild("jiesuan").gameObject;
        
        _jsclose = _jiesuan.transform.FindChild("close").GetComponent<Button>();
        _jsjixu = _jiesuan.transform.FindChild("jixu").GetComponent<Button>();
        _jsfenxiang = _jiesuan.transform.FindChild("share").GetComponent<Button>();
        _jsxiangqing = _jiesuan.transform.FindChild("xiangqing").GetComponent<Button>();
        _jsgrid = _jiesuan.transform.FindChild("list/grid").gameObject;

        _jilu = uiSprite.FindChild("jilu");
       
        _jlClose = _jilu.FindChild("close").GetComponent<Button>();
        _jlHead = _jilu.FindChild("head");
        _jlGrid = _jilu.FindChild("list/grid");
        _jlItem = _jlGrid.FindChild("item").gameObject;
       

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
        _jiesuan.SetActive(true);
        _jilu.gameObject.SetActive(false);
        _jlItem.SetActive(false);
        _xiangqing.gameObject.SetActive(false);
        initData();
        if (obj != null)
        {
            index = (int)obj;
        }
        _isShare = false;
        if (GameConst.isGuest)
        {
            _jsfenxiang.gameObject.SetActive(false);
        }

       
    }

    public override void AddListener()
    {
        base.AddListener();
        EventDispatcher.Instance.AddEventListener(GameEventConst.JIESUAN_DETAIL,onDetail);
    }
    public override void RemoveListener()
    {
        base.RemoveListener();
        EventDispatcher.Instance.RemoveEventListener(GameEventConst.JIESUAN_DETAIL,onDetail);
    }
    private void onDetail()
    {
        _jiesuan.SetActive(false);
        _jilu.gameObject.SetActive(true);

        for (int i = 0; i < HistoryController.Instance._hisPlayerDic.Count; i++)
        {
            _jlHead.FindChild("name" + i).GetComponent<Text>().text = HistoryController.Instance._hisPlayerDic[i].Name;
            _jlHead.FindChild("id" + i).GetComponent<Text>().text = "ID:" + HistoryController.Instance._hisPlayerDic[i].Id.IdEx();
        }
        _jlGrid.destoryChild(true);
        for (int i = 0; i < HistoryController.Instance.roundScoreList.Count; i++)
        {
            GameObject obj = GameObject.Instantiate(_jlItem, _jlGrid, true) as GameObject;
            obj.transform.localScale = Vector3.one;
            int num = i + 1;
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
                    score = HistoryController.Instance.roundScoreList[i].scores[j].score.ToString().Replace('-','—');
                obj.transform.FindChild("info" + j).GetComponent<Text>().text =
                    str + "\n" + score;
            }
        }
    }
    private void addClick()
    {
        _jsclose.onClick.AddListener(delegate
        {
            //DestoryPanel();
            ClosePanel();
            RoleController.Instance.clear();
            HallPanel.Instance.load();
            SoundMgr._instance.bgmPlay("beijing_dating" + GameConst.BGM, GameConst.musicVol);
        });
        _jsjixu.onClick.AddListener(delegate
        {
            //DestoryPanel();
            ClosePanel();
            RoleController.Instance.clear();
            HallPanel.Instance.load();
            SoundMgr._instance.bgmPlay("beijing_dating" + GameConst.BGM, GameConst.musicVol);
        });
        _jsfenxiang.onClick.AddListener(delegate
        {
            if (!_isShare)
            {
                _isShare = true;

                GameConst.driver.StartCoroutine(CaptureScreen());
            }
            
        });
        _jsxiangqing.onClick.AddListener(delegate
        {
            ProtoReq.historyDetailReq(index);
        });
        _jlClose.onClick.AddListener(delegate
        {
            _jiesuan.SetActive(true);
            _jilu.gameObject.SetActive(false);
        });
        _xqClose.onClick.AddListener(delegate
        {
            _xiangqing.gameObject.SetActive(false);
            _jilu.gameObject.SetActive(true);
        });
    }

    private void initData()
    {
        foreach (var item in RoleController.Instance._playerDic)
        {

            _jsgrid.transform.FindChild(item.Value.Pos + "/name").GetComponent<Text>().text = item.Value.Name;
            _jsgrid.transform.FindChild(item.Value.Pos + "/id").GetComponent<Text>().text = "ID:" + item.Value.Id.IdEx();
            SDKMgr.Instance.SetAsyncImage(item.Value.Url,_jsgrid.transform.FindChild(item.Value.Pos + "/head").GetComponent<Image>());


            for (int j = 1; j <= 7; j++)
            {
                if (JiesuanController.Instance._info[item.Value.Id.idToPos()].Count == 0)
                    _jsgrid.transform.FindChild(item.Value.Pos + "/info" + j).GetComponent<Text>().text = "无数据";
                else
                    _jsgrid.transform.FindChild(item.Value.Pos + "/info" + j).GetComponent<Text>().text = JiesuanController.Instance._info[item.Value.Id.idToPos()][j - 1];

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
        for (int k = 0; k < HistoryController.Instance.roundScoreList[_curEndCard].end_cards.Count; k++)
        {
            //put设置
            GameObject outObj = _xqGrid.transform.FindChild(k + "/out").gameObject;
            GameObject outItem = outObj.transform.FindChild("item").gameObject;
            outObj.transform.destoryChild(true);
            outItem.SetActive(false);

            for (int i = 0; i < HistoryController.Instance.roundScoreList[_curEndCard].end_cards[k].outcards.Count; i++)
            {
                GameObject copyItem = GameObject.Instantiate(outItem, outObj.transform, true) as GameObject;
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
                    copyItem = GameObject.Instantiate(_xqPeng, inObj.transform, true) as GameObject;
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
                    copyItem = GameObject.Instantiate(_xqGang, inObj.transform, true) as GameObject;
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
                GameObject copyItem = GameObject.Instantiate(_xqCard, inObj.transform, true) as GameObject;
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
        for (int i = 0; i < HistoryController.Instance._hisPlayerDic.Count; i++)
        {
            _xqGrid.transform.FindChild(i + "/name").GetComponent<Text>().text = HistoryController.Instance._hisPlayerDic[i].Name;
            if (_xqGrid.transform.FindChild(i + "/head").GetComponent<Image>().sprite == null)
                SDKMgr.Instance.SetAsyncImage(HistoryController.Instance._hisPlayerDic[i].Url, _xqGrid.transform.FindChild(i + "/head").GetComponent<Image>());
            else if (!_xqGrid.transform.FindChild(i + "/head").GetComponent<Image>().sprite.name.Equals(HistoryController.Instance._hisPlayerDic[i].Url))
            {
                SDKMgr.Instance.SetAsyncImage(HistoryController.Instance._hisPlayerDic[i].Url, _xqGrid.transform.FindChild(i + "/head").GetComponent<Image>());
            }
        }
    }

    private void setMa()
    {
        for (int i = 0; i < 4; i++)
        {
            _xqGrid.transform.FindChild(i + "/ma").transform.destoryChild(true);
        }
        if (HistoryController.Instance.jiangmaList == null || HistoryController.Instance.jiangmaList.Count == 0)
        {
            //买马
            if (HistoryController.Instance.maimaList == null || HistoryController.Instance.maimaList.Count == 0)
                return;
            if (HistoryController.Instance.maimaList[_curEndCard].maima_list == null || HistoryController.Instance.maimaList[_curEndCard].maima_list.Count == 0)
                return;
           
            for (int i = 0; i < HistoryController.Instance.maimaList[_curEndCard].maima_list.Count; i++)
            {
                GameObject obj = _xqGrid.transform.FindChild(i + "/ma/item").gameObject;
                if (HistoryController.Instance.maimaList[_curEndCard].maima_list[i].card1 != 0)
                {
                    GameObject copyItem = GameObject.Instantiate(obj, obj.transform.parent, true) as GameObject;
                    copyItem.transform.localScale = Vector3.one;
                    copyItem.transform.parent = obj.transform.parent;
                    copyItem.SetActive(true);
                    IconMgr.Instance.SetImage(copyItem.transform.FindChild("value").GetComponent<Image>(), "shangmian1_" + HistoryController.Instance.maimaList[_curEndCard].maima_list[i].card1);
                    if (HistoryController.Instance.maimaList[_curEndCard].maima_list[i].zhong1 == 0)
                        copyItem.transform.FindChild("del").gameObject.SetActive(false);
                    else
                        copyItem.transform.FindChild("del").gameObject.SetActive(true);
                    copyItem.transform.FindChild("add").gameObject.SetActive(false);
                }
                if (HistoryController.Instance.maimaList[_curEndCard].maima_list[i].card2 != 0)
                {
                    GameObject copyItem = GameObject.Instantiate(obj, obj.transform.parent, true) as GameObject;
                    copyItem.transform.localScale = Vector3.one;
                    copyItem.transform.parent = obj.transform.parent;
                    copyItem.SetActive(true);
                    IconMgr.Instance.SetImage(copyItem.transform.FindChild("value").GetComponent<Image>(), "shangmian1_" + HistoryController.Instance.maimaList[_curEndCard].maima_list[i].card2);
                    if (HistoryController.Instance.maimaList[_curEndCard].maima_list[i].zhong2 == 0)
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
                        GameObject copyItem = GameObject.Instantiate(obj, obj.transform.parent, true) as GameObject;
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
                        if (isFlag)
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


    private IEnumerator CaptureScreen()
    {
        //string ssname = "share.png";
        string sspath = Application.persistentDataPath + "/share.png";
        if (FileTools.IsExistFile(sspath))
        {
            FileTools.DeleteFile(sspath);
        }
        Texture2D textrue1;
        byte[] bytes;
        yield return new WaitForEndOfFrame();
        textrue1 = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        textrue1.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        textrue1.Apply();
        yield return 0;
        bytes = textrue1.EncodeToPNG();
        FileTools.WriteBytesToFile(bytes, sspath);
        SDKMgr.Instance.Capture(sspath);
    }
}
