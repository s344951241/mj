using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Threading;

public class DanjuPanel : BasePanel
{
    private static DanjuPanel instance;
    public static DanjuPanel Instance
    {
        get
        {
            if (instance == null)
                instance = new DanjuPanel();
            return instance;
        }
    }

    private Transform _jiesuan;
    private Transform _headGrid;
    private Button _jixuBtn;
    private Button _dipaiBtn;

    private Transform _dipai;
    private Button _dipaiClose;
    private GameObject _grid;
    private GameObject _itemGrid;
    private GameObject _item;
    private GameObject _maima;
    private Transform _xiangqing;
    private GameObject _xqGrid;
    private Button _xqClose;
    private GameObject _xqCard;
    private GameObject _xqPeng;
    private GameObject _xqGang;
    private int curIndex = 0;
    private DanjuPanel()
    {
        base_name = PanelTag.DANJU_PANEL;
        _isPop = true;
    }

    public override void InitPanel(Transform uiSprite)
    {
        base.InitPanel(uiSprite);
        _jiesuan = uiSprite.FindChild("jiesuan").transform;
        _dipai = uiSprite.FindChild("dipai").transform;
        _jixuBtn = _jiesuan.FindChild("jixu").GetComponent<Button>();
        _dipaiBtn = _jiesuan.FindChild("dipai").GetComponent<Button>();

        _dipaiClose = _dipai.FindChild("close").GetComponent<Button>();
        _grid = _dipai.FindChild("list/grid").gameObject;
        _maima = _dipai.FindChild("maima").gameObject;
        _headGrid = _jiesuan.FindChild("headList/grid");
        _itemGrid = _jiesuan.FindChild("list/grid").gameObject;
        _item = _itemGrid.transform.FindChild("item").gameObject;

        _xiangqing = uiSprite.FindChild("xiangqing");
        _xqGrid = _xiangqing.FindChild("list/grid").gameObject;
        _xqClose = _xiangqing.FindChild("close").GetComponent<Button>();
        _xqCard = _xiangqing.FindChild("card").gameObject;
        _xqPeng = _xiangqing.FindChild("peng").gameObject;
        _xqGang = _xiangqing.FindChild("gang").gameObject;

        addClick();
      
    }
    private void addClick()
    {
        _xqClose.onClick.AddListener(delegate
        {
            _dipai.gameObject.SetActive(false);
            _xiangqing.gameObject.SetActive(false);
            _jiesuan.gameObject.SetActive(true);
        });
        _jixuBtn.onClick.AddListener(delegate
        {
            //DestoryPanel();
            if (DataMgr.Instance.curRound != DataMgr.Instance.sumRound)
            {
                ClosePanel();
                EventDispatcher.Instance.Dispatch(GameEventConst.SELF_NEXT);
            }
          
            //ProtoReq.readyNext();
        });
        _dipaiBtn.onClick.AddListener(delegate
        {
            _dipai.gameObject.SetActive(false);
            _jiesuan.gameObject.SetActive(false);
            _xiangqing.gameObject.SetActive(true);
        });

        //_dipaiClose.onClick.AddListener(delegate
        //{
        //    _dipai.gameObject.SetActive(false);
        //    _jiesuan.gameObject.SetActive(true);
        //});

        for (int i = 0; i < 4; i++)
        {
            int index = i;
            _headGrid.FindChild(i + "/button").GetComponent<Button>().onClick.AddListener(delegate {
                curIndex = index;
                _headGrid.FindChild(index + "/button").gameObject.SetActive(false);
                _headGrid.FindChild(index + "/down").gameObject.SetActive(true);
                setButtonGrid();
                setList(RoleController.Instance.getPlayerByPos(index).Id);
            });
        }
    }

    public override void startUp(object obj = null)
    {
        base.startUp(obj);
        if(GameConst.isSetDanjuHead)
            SetHeadList();

        if (DanjuController.Instance._lastCard.Count != 0)
        {
            setDiPai();
            addTimeToOpenJieSuan();
            _dipai.gameObject.SetActive(true);
            _jiesuan.gameObject.SetActive(false);
            _xiangqing.gameObject.SetActive(false);
        }
        else
        {
            _dipai.gameObject.SetActive(false);
            _jiesuan.gameObject.SetActive(true);
            _xiangqing.gameObject.SetActive(false);
            setJiesuan();
        }
        if(GameConst.isSetDanjuHead)
            setHead();
        GameConst.isSetDanjuHead = false;
        xiangqing();
        setMa();
    }
    private void SetHeadList()
    {
        for (int i= 0;i < 4;i++)
        {
            Player player = RoleController.Instance.getPlayerByPos(i);
            if (player != null)
            {
                SDKMgr.Instance.SetAsyncImage(player.Url,_headGrid.FindChild(player.Pos + "/button/head").GetComponent<Image>());
                _headGrid.FindChild(player.Pos + "/button/name").GetComponent<Text>().text = player.Name;
                int score = DanjuController.Instance._scoreDic[player.Id.idToPos()].score;
                _headGrid.FindChild(player.Pos + "/button/info").GetComponent<Text>().text = "本局"+ (score>0?("+"+score):score.ToString());

                if (string.IsNullOrEmpty(DanjuController.Instance._scoreType[player.Id.idToPos()]))
                {
                    _headGrid.FindChild(player.Pos + "/button/icon").gameObject.SetActive(false);
                    _headGrid.FindChild(player.Pos + "/down/icon").gameObject.SetActive(false);
                }
                else
                {
                    _headGrid.FindChild(player.Pos + "/button/icon").gameObject.SetActive(true);
                    IconMgr.Instance.SetImage(_headGrid.FindChild(player.Pos + "/button/icon").GetComponent<Image>(), DanjuController.Instance._scoreType[player.Id.idToPos()],true);

                    _headGrid.FindChild(player.Pos + "/down/icon").gameObject.SetActive(true);
                    IconMgr.Instance.SetImage(_headGrid.FindChild(player.Pos + "/down/icon").GetComponent<Image>(), DanjuController.Instance._scoreType[player.Id.idToPos()],true);
                }

                SDKMgr.Instance.SetAsyncImage(player.Url,_headGrid.FindChild(player.Pos + "/down/head").GetComponent<Image>());
                _headGrid.FindChild(player.Pos + "/down/name").GetComponent<Text>().text = player.Name;
                _headGrid.FindChild(player.Pos + "/down/info").GetComponent<Text>().text = "本局" + (score > 0 ? ("+" + score) : score.ToString());
            }
        }
    }
    private void addTimeToOpenJieSuan()
    {

        //GlobleTimer.Instance.SetTimer(GameConst.dipai,1, delegate
        //{
        //    Debug.Log("时间到");
        //    if (_dipai != null)
        //    {
        //        _dipai.gameObject.SetActive(false);
        //        _jiesuan.gameObject.SetActive(true);
        //        _xiangqing.gameObject.SetActive(false);
        //        setJiesuan();
        //    }
           
        //});

        Timer time = new Timer(delegate
        {

            Loom.QueueOnMainThread(() => {
                if (_dipai != null)
                {
                    _dipai.gameObject.SetActive(false);
                    _jiesuan.gameObject.SetActive(true);
                    _xiangqing.gameObject.SetActive(false);
                    setJiesuan();
                }
            });
        }, this, GameConst.dipai, 0);
    }

    private void setDiPai()
    {
        for (int i = 0; i < DanjuController.Instance._lastCard.Count; i++)
        {
            IconMgr.Instance.SetImage(_grid.transform.GetChild(i).FindChild("value").GetComponent<Image>(), "zm1_" + DanjuController.Instance._lastCard[i]);
            _grid.transform.GetChild(i).gameObject.SetActive(true);
            if (DanjuController.Instance._jiangma.Contains(DanjuController.Instance._lastCard[i]))
            {
                _grid.transform.GetChild(i).FindChild("down").gameObject.SetActive(true);
            }
            else
            {
                _grid.transform.GetChild(i).FindChild("down").gameObject.SetActive(false);
            }
        }

        //买马
        for (int i = 0; i < DanjuController.Instance._maima.Count; i++)
        {
            if (DanjuController.Instance._maima[i].Count == 0)
                _maima.transform.FindChild(i.ToString()).gameObject.SetActive(false);
            else
            {
                _maima.transform.FindChild(i.ToString()).gameObject.SetActive(true);
                if (DanjuController.Instance._maima[i][0] == 0)
                {
                    _maima.transform.FindChild(i + "/ma1").gameObject.SetActive(false);
                }
                else
                {
                    _maima.transform.FindChild(i + "/ma1").gameObject.SetActive(true);
                    IconMgr.Instance.SetImage(_maima.transform.FindChild(i + "/ma1/value").GetComponent<Image>(), "zm1_" + DanjuController.Instance._maima[i][0]);
                }

                if (DanjuController.Instance._maima[i][1] == 0)
                {
                    _maima.transform.FindChild(i + "/ma2").gameObject.SetActive(false);
                }
                else
                {
                    _maima.transform.FindChild(i + "/ma2").gameObject.SetActive(true);
                    IconMgr.Instance.SetImage(_maima.transform.FindChild(i + "/ma2/value").GetComponent<Image>(), "zm1_" + DanjuController.Instance._maima[i][1]);
                }
            }

        }
    }

    private void setList(int id)
    {
        for (int i = 1; i < _itemGrid.transform.childCount; i++)
        {
            GameObject.Destroy(_itemGrid.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < DanjuController.Instance._infoDic[id.idToPos()].Count; i++)
        {
            GameObject obj = GameObject.Instantiate(_item, _itemGrid.transform,true) as GameObject;
            obj.name = "111111";
            obj.SetActive(true);
            obj.transform.FindChild("info1").GetComponent<Text>().text = DanjuController.Instance._infoDic[id.idToPos()][i][0];
            obj.transform.FindChild("info2").GetComponent<Text>().text = DanjuController.Instance._infoDic[id.idToPos()][i][1];
            obj.transform.FindChild("info3").GetComponent<Text>().text = DanjuController.Instance._infoDic[id.idToPos()][i][2];
            obj.transform.FindChild("info4").GetComponent<Text>().text = DanjuController.Instance._infoDic[id.idToPos()][i][3];
            obj.transform.FindChild("info5").GetComponent<Text>().text = DanjuController.Instance._infoDic[id.idToPos()][i][4];
        }
       

    }
    private void setJiesuan()
    {
        curIndex = MainRole.Instance.Pos;
        _headGrid.FindChild(curIndex + "/button").gameObject.SetActive(false);
        _headGrid.FindChild(curIndex + "/down").gameObject.SetActive(true);
        setButtonGrid();
        setList(MainRole.Instance.Id);
    }
    private void setButtonGrid()
    {
        for (int i = 0; i < 4; i++)
        {
            if (curIndex != i)
            {
                _headGrid.FindChild(i + "/button").gameObject.SetActive(true);
                _headGrid.FindChild(i + "/down").gameObject.SetActive(false);
            }
        }
    }

    private void xiangqing()
    {
        foreach (var item in DataMgr.Instance._putCardsDic)
        {
           

        }

        for (int k = 0; k < 4; k++)
        {

            //put设置
            GameObject outObj = _xqGrid.transform.FindChild( k+"/out").gameObject;
            GameObject outItem = outObj.transform.FindChild("item").gameObject;
            //outItem.SetActive(false);
            outObj.transform.setChildrenActive(false);

            for (int i = 0; i < DataMgr.Instance._putCardsDic[k].Count; i++)
            {
                GameObject copyItem = null;
                if (i < outObj.transform.childCount - 1)
                {
                    copyItem = outObj.transform.GetChild(i + 1).gameObject;
                }
                else
                {
                    copyItem = GameObject.Instantiate(outItem, outObj.transform, true) as GameObject;
                }
                copyItem.transform.localScale = Vector3.one;
                IconMgr.Instance.SetImage(copyItem.transform.FindChild("value").GetComponent<Image>(), "shangmian1_" + DataMgr.Instance._putCardsDic[k][i]);
                copyItem.SetActive(true);
            }

            //in的碰杠设置

            GameObject inObj = _xqGrid.transform.FindChild(k + "/in").gameObject;
            inObj.transform.destoryChild();
            if (RoleController.Instance.getPlayerByPos(k) == null)
                continue;
            for (int i = 0; i < DanjuController.Instance._pgDataDic[RoleController.Instance.getPlayerByPos(k).Id.idToPos()].Count; i++)
            {
                GameObject copyItem = null;
                if (DanjuController.Instance._pgDataDic[RoleController.Instance.getPlayerByPos(k).Id.idToPos()][i].ptype == 0)
                {
                    copyItem = GameObject.Instantiate(_xqPeng, inObj.transform,true) as GameObject;
                    copyItem.transform.localScale = Vector3.one;
                    copyItem.transform.localPosition = Vector3.zero;
                    for (int j = 1; j <= 3; j++)
                    {
                        IconMgr.Instance.SetImage(copyItem.transform.FindChild("value" + j).GetComponent<Image>(), "shangmian1_" + DanjuController.Instance._pgDataDic[RoleController.Instance.getPlayerByPos(k).Id.idToPos()][i].card);
                    }
                    copyItem.SetActive(true);
                }
                else
                {
                    copyItem = GameObject.Instantiate(_xqGang, inObj.transform,true) as GameObject;
                    copyItem.transform.localScale = Vector3.one;
                    copyItem.transform.localPosition = Vector3.zero;
                    for (int j = 1; j <= 3; j++)
                    {
                        IconMgr.Instance.SetImage(copyItem.transform.FindChild("value" + j).GetComponent<Image>(), "shangmian1_" + DanjuController.Instance._pgDataDic[RoleController.Instance.getPlayerByPos(k).Id.idToPos()][i].card);
                    }
                    copyItem.SetActive(true);
                }
            }
            //in 的手牌设置
            for (int i = 0; i < DataMgr.Instance._leftCardsDic[RoleController.Instance.getPlayerByPos(k).Id.idToPos()].Count; i++)
            {
                GameObject copyItem = GameObject.Instantiate(_xqCard, inObj.transform,true) as GameObject;
                copyItem.transform.localScale = Vector3.one;
                copyItem.transform.localPosition = Vector3.zero;
                IconMgr.Instance.SetImage(copyItem.transform.FindChild("value").GetComponent<Image>(), "shangmian1_" + DataMgr.Instance._leftCardsDic[RoleController.Instance.getPlayerByPos(k).Id.idToPos()][i]);
                copyItem.SetActive(true);
            }
            
        }
    }

    private void setHead()
    {
        for (int i = 0; i < 4; i++)
        {
            if (RoleController.Instance.getPlayerByPos(i) == null)
                continue;
            _xqGrid.transform.FindChild(i + "/name").GetComponent<Text>().text = RoleController.Instance.getPlayerByPos(i).Name;
            SDKMgr.Instance.SetAsyncImage(RoleController.Instance.getPlayerByPos(i).Url,_xqGrid.transform.FindChild(i + "/head").GetComponent<Image>());
        }
    }

    private void setMa()
    {
        if (DanjuController.Instance.isJiangma)
        {
            //奖马
            for (int i = 0; i < 4; i++)
            {
                if (RoleController.Instance.getPlayerByPos(i) == null)
                    continue;

                _xqGrid.transform.FindChild(i + "/ma").setChildrenActive(false);
                GameObject obj = _xqGrid.transform.FindChild(i + "/ma/item").gameObject;

                if(DanjuController.Instance._scoreDic[RoleController.Instance.getPlayerByPos(i).Id.idToPos()].hu==1|| DanjuController.Instance._scoreDic[RoleController.Instance.getPlayerByPos(i).Id.idToPos()].hu == 3)
                {
                    for (int j = 0; j < DanjuController.Instance._jiangma.Count; j++)
                    {
                        GameObject copyItem = null;
                        if (j < _xqGrid.transform.FindChild(i + "/ma").childCount - 1)
                        {
                            copyItem = _xqGrid.transform.FindChild(i + "/ma").GetChild(j + 1).gameObject;
                        }
                        else
                        {
                            copyItem = GameObject.Instantiate(obj, obj.transform.parent, true) as GameObject;
                        }
                        copyItem.transform.localScale = Vector3.one;
                        copyItem.SetActive(true);
                        IconMgr.Instance.SetImage(copyItem.transform.FindChild("value").GetComponent<Image>(), "shangmian1_" + DanjuController.Instance._jiangma[j]);
                        bool isadd = false;
                        foreach (var score in DanjuController.Instance._scoreDic[RoleController.Instance.getPlayerByPos(i).Id.idToPos()].jiangma)
                        {
                            if (score == DanjuController.Instance._jiangma[j])
                            {
                                isadd = true;
                                break;
                            }
                        }
                        copyItem.transform.FindChild("del").gameObject.SetActive(false);
                        if (isadd)
                            copyItem.transform.FindChild("add").gameObject.SetActive(true);
                        else
                            copyItem.transform.FindChild("add").gameObject.SetActive(false);
                    }

                }
            }
        }
        else
        {
            //买马
            for (int i = 0; i < 4; i++)
            {
                if (RoleController.Instance.getPlayerByPos(i) == null)
                    continue;
                _xqGrid.transform.FindChild(i + "/ma").setChildrenActive(false);
                GameObject obj = _xqGrid.transform.FindChild(i + "/ma/item").gameObject;
                int index = 0;
                foreach (var item in DanjuController.Instance._maima[RoleController.Instance.getPlayerByPos(i).Id.idToPos()])
                {
                    GameObject copyItem = null;
                    if (index < _xqGrid.transform.FindChild(i + "/ma").childCount - 1)
                    {
                        copyItem = _xqGrid.transform.FindChild(i + "/ma").GetChild(index + 1).gameObject;
                        index++;
                    }
                    else
                    {
                        copyItem = GameObject.Instantiate(obj, obj.transform.parent, true) as GameObject;
                    }
                    copyItem.transform.localScale = Vector3.one;
                    copyItem.SetActive(true);
                    IconMgr.Instance.SetImage(copyItem.transform.FindChild("value").GetComponent<Image>(), "shangmian1_" + item);
                    bool isadd = false;
                    foreach (var score in DanjuController.Instance._maimaScore[RoleController.Instance.getPlayerByPos(i).Id.idToPos()])
                    {
                        if (score.card == item)
                        {
                            isadd = true;
                            break;
                        }
                    }
                    if (isadd)
                        copyItem.transform.FindChild("del").gameObject.SetActive(true);
                    else
                        copyItem.transform.FindChild("del").gameObject.SetActive(false);
                    copyItem.transform.FindChild("add").gameObject.SetActive(false);
                }
            }
            //DanjuController.Instance._maima;
        }
    }
}
