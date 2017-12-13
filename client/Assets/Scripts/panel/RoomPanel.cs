using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Threading;

public class RoomPanel : BasePanel {

    private static RoomPanel instance;
    public static RoomPanel Instance
    {
        get
        {
            if (instance == null)
                instance = new RoomPanel();
            return instance;
        }

    }
    private RoomPanel()
    {
        base_name = PanelTag.ROOM_PANEL;
        _isPop = false;
    }


    private Transform root;

    private int _timeCount;
    private int _curCard;
    private int _curHeCard;
   


    private Button _get;
    private GameObject _list;
    private GameObject _grid;
    private GameObject _last;
    private Button _otherCardClick;
    private GameObject _otherCard;
    private Text _tableNum;
    private Text _tableTime;
    private GameObject _always;
    private GameObject _before;
    private GameObject _after;
    private GameObject _card;
    private GameObject _count;
    private GameObject _center;
    private GameObject _leftCard;
    private GameObject _selfCard;
    private GameObject _topCard;
    private GameObject _rightCard;
    private GameObject _fun;
    private Button _hu;
    private Button _gang;
    private GameObject _gangs;
    private Button _peng;
    private Button _pass;
    private GameObject _zimo;
    private GameObject _bzimo;
    private GameObject _bzimo1;
    private GameObject _bzimo2;
    private GameObject _dianpao;
    private GameObject _huangju;
    private GameObject _huIcon;
    private GameObject _huIcon1;
    private GameObject _huIcon2;
    private GameObject _huIcon3;
    private GameObject _point;
    private GameObject _maima;
    private GameObject _handCard;

    private GameObject _talkShow;

    private GameObject _rightHand;
    private GameObject _topHand;
    private GameObject _leftHand;

    private Text _state;

    private delegate void ON_GANG();
    private ON_GANG gangBack = null;

    private GameObject _upCard = null;
    private float _handPosY = 0;
    private bool _isTalk = false;
    private bool _isTalkCD = false;
    private int talkTimer;

    private float talkTime;
    public override void InitPanel(Transform uiSprite)
    {
        base.InitPanel(uiSprite);
        root = uiSprite;
        _isTalk = false;
        _isTalkCD = false;
        _always = uiSprite.FindChild("always").gameObject;
        _before = uiSprite.FindChild("before").gameObject;
        _after = uiSprite.FindChild("after").gameObject;
        _tableNum = _always.transform.FindChild("number").GetComponent<Text>();
        _count = _always.transform.FindChild("count").gameObject;
        _center = _always.transform.FindChild("center").gameObject;
        _tableTime = _always.transform.FindChild("time").GetComponent<Text>();
        //GlobleTimer.Instance.update += timeUp;

        _fun = uiSprite.transform.FindChild("fun").gameObject;
        _hu = _fun.transform.FindChild("hu").GetComponent<Button>();
        _gang = _fun.transform.FindChild("gang").GetComponent<Button>();
        _gangs = _fun.transform.FindChild("gangs").gameObject;
        _peng = _fun.transform.FindChild("peng").GetComponent<Button>();
        _pass = _fun.transform.FindChild("pass").GetComponent<Button>();
        _pass.gameObject.SetActive(true);
        _talkShow = _always.transform.FindChild("talkShow").gameObject;
        _state = _always.transform.FindChild("state").GetComponent<Text>();
        _talkShow.SetActive(false);
        addClick();
        setAfter();
    }

    private void timeUp(float dt)
    {
        if(_tableTime!=null)
         _tableTime.text = System.DateTime.Now.Hour + ":" + (System.DateTime.Now.Minute>=10? System.DateTime.Now.Minute.ToString():("0"+ System.DateTime.Now.Minute));
    }
    public override void OnClose()
    {
        GlobleTimer.Instance.removeUpdate(timeUp);
        _before.transform.FindChild("invite").gameObject.SetActive(true);
        _before.transform.FindChild("quit").gameObject.SetActive(true);
        base.OnClose();
        //GlobleTimer.Instance.update -= timeUp;
       
    }
    public override void startUp(object obj = null)
    {
        base.startUp(obj);
        talkTime = 0;
        _tableNum.text = GameConst.tableId.ToString();
        _state.text = "等待中";
        _count.SetActive(false);
        _center.SetActive(false);
        _before.transform.FindChild("quit").gameObject.SetActive(true);
        if (GameConst.ower == MainRole.Instance.Id)
        {
            _before.transform.FindChild("quit/quit").gameObject.SetActive(false);
            _before.transform.FindChild("quit/dissolve").gameObject.SetActive(true);

        }
        else
        {
            _before.transform.FindChild("quit/quit").gameObject.SetActive(true);
            _before.transform.FindChild("quit/dissolve").gameObject.SetActive(false);

        }

        if (GameConst.isGuest)
        {
            _before.transform.FindChild("invite").gameObject.SetActive(false);
        }
        else
        {
            _before.transform.FindChild("invite").gameObject.SetActive(true);
        }

        GlobleTimer.Instance.addUpdate(timeUp);

        if (obj == null || (int)obj == 0)
        {
            _always.SetActive(true);
            _before.SetActive(true);
            _after.SetActive(false);
            _fun.SetActive(false);
            //_handCard.SetActive(false);
            //initCard();
            initHead();
            addPlayer(null);
        }
        else if ((int)obj == 1)
        {
            _always.SetActive(true);
            _before.SetActive(false);
            _after.SetActive(false);
            _fun.SetActive(false);
            addPlayer(null);
            onReconnect();
        }
        else if ((int)obj == 2)
        {
            jixu();
        }
        else
        {

        }
    }
    private void setAfter()
    {
        _card = _after.transform.FindChild("card").gameObject;

        _selfCard = _after.transform.FindChild("selfCard").gameObject;
        _leftCard = _after.transform.FindChild("leftCard").gameObject;
        _rightCard = _after.transform.FindChild("rightCard").gameObject;
        _topCard = _after.transform.FindChild("topCard").gameObject;

        _handCard = _after.transform.FindChild("handCard").gameObject;
        _handPosY = _handCard.transform.localPosition.y;

        _zimo = _after.transform.FindChild("zimo").gameObject;
        _bzimo = _after.transform.FindChild("bzimo").gameObject;
        _bzimo1 = _after.transform.FindChild("bzimo1").gameObject;
        _bzimo2 = _after.transform.FindChild("bzimo2").gameObject;
        _dianpao = _after.transform.FindChild("dianpao").gameObject;
        _huIcon = _after.transform.FindChild("hu").gameObject;
        _huIcon1 = _after.transform.FindChild("hu1").gameObject;
        _huIcon2 = _after.transform.FindChild("hu2").gameObject;
        _huIcon3 = _after.transform.FindChild("hu3").gameObject;
        _huangju = _after.transform.FindChild("huangju").gameObject;
        _point = _after.transform.FindChild("point").gameObject;
        _maima = _after.transform.FindChild("maima").gameObject;

        _rightHand = _after.transform.FindChild("rightHand").gameObject;
        _topHand = _after.transform.FindChild("topHand").gameObject;
        _leftHand = _after.transform.FindChild("leftHand").gameObject;
        setOtherHand(0);
        for (int i = 0; i < 13; i++)
        {
            int index = i;
            GameObject obj = _selfCard.transform.FindChild(i.ToString()).gameObject;

            obj.GetComponent<Button>().onClick.AddListener(delegate
            {
                if (!GameConst.isTurn)
                    return;
                if (obj == _upCard)
                {
                    _upCard = null;
                    string name = obj.transform.FindChild("value").GetComponent<Image>().sprite.name;
                    int num = GameTools.getCardNumByName(name);
                    obj.SetActive(false);
                    _selfCard.GetComponent<handCardProxy>().setPos();
                    obj.SetActive(true);
                    _handCard.transform.localPosition = new Vector3(_handCard.transform.localPosition.x, _handPosY, 0);
                    ProtoReq.Play(num);
                }
                else
                {
                    _upCard = obj;
                    _selfCard.GetComponent<handCardProxy>().setPos();
                    obj.transform.localPosition = new Vector3(obj.transform.localPosition.x, obj.transform.localPosition.y + GameConst.upDis, 0);
                    _handCard.transform.localPosition = new Vector3(_handCard.transform.localPosition.x, _handPosY, 0);
                }
            });
            UIEventHandlerBase.AddListener(obj, UIEventType.ON_DRAG, delegate (GameObject o, BaseEventData evn)
            {
                float startPos = ((PointerEventData)evn).pressPosition.y;
                float endPos = ((PointerEventData)evn).position.y;
                if (endPos - startPos >= 50 && GameConst.isTurn)
                {
                    GameConst.isTurn = false;
                    _upCard = null;
                    string name = obj.transform.FindChild("value").GetComponent<Image>().sprite.name;
                    int num = GameTools.getCardNumByName(name);
                    obj.SetActive(false);
                    _selfCard.GetComponent<handCardProxy>().setPos();
                    obj.SetActive(true);
                    _handCard.transform.localPosition = new Vector3(_handCard.transform.localPosition.x, _handPosY, 0);
                    ProtoReq.Play(num);
                }
            });


        }

        _handCard.GetComponent<Button>().onClick.AddListener(delegate
        {
            if (!GameConst.isTurn)
                return;
            if (_upCard == _handCard)
            {
                _upCard = null;
                string name = _handCard.transform.FindChild("value").GetComponent<Image>().sprite.name;
                int num = GameTools.getCardNumByName(name);
                _handCard.transform.localPosition = new Vector3(_handCard.transform.localPosition.x, _handPosY, 0);
                ProtoReq.Play(num);
            }
            else
            {
                _upCard = _handCard;
                _selfCard.GetComponent<handCardProxy>().setPos();
                _handCard.transform.localPosition = new Vector3(_handCard.transform.localPosition.x, _handPosY + GameConst.upDis, 0);
            }
        });
        UIEventHandlerBase.AddListener(_handCard, UIEventType.ON_DRAG, delegate (GameObject o, BaseEventData evn)
        {
            float startPos = ((PointerEventData)evn).pressPosition.y;
            float endPos = ((PointerEventData)evn).position.y;
            if (endPos - startPos >= 50 && GameConst.isTurn)
            {
                GameConst.isTurn = false;
                _upCard = null;
                string name = _handCard.transform.FindChild("value").GetComponent<Image>().sprite.name;
                int num = GameTools.getCardNumByName(name);
                _handCard.transform.localPosition = new Vector3(_handCard.transform.localPosition.x, _handPosY, 0);
                ProtoReq.Play(num);
            }
        });

    }
    private void initIcon()
    {
        IconMgr.Instance.SetImage(_zimo.GetComponent<Image>(), "zimo1",true);
        IconMgr.Instance.SetImage(_dianpao.GetComponent<Image>(), "pao", true);
        IconMgr.Instance.SetImage(_huIcon.GetComponent<Image>(), "hu1", true);
        IconMgr.Instance.SetImage(_huIcon1.GetComponent<Image>(), "hu1", true);
        IconMgr.Instance.SetImage(_huIcon2.GetComponent<Image>(), "hu1", true);
        IconMgr.Instance.SetImage(_huIcon3.GetComponent<Image>(), "hu1", true);
        IconMgr.Instance.SetImage(_bzimo.GetComponent<Image>(), "beizimo1", true);
        IconMgr.Instance.SetImage(_bzimo1.GetComponent<Image>(), "beizimo1", true);
        IconMgr.Instance.SetImage(_bzimo2.GetComponent<Image>(), "beizimo1", true);
    }
    private void initAfter()
    {
        _center.SetActive(true);
        //_count.SetActive(true);

        for (int i = 0; i < 4; i++)
        {
            GameObject o = _card.transform.FindChild(i.ToString()).gameObject;
            for(int j=0;j<o.transform.childCount;j++)
            {

                o.transform.GetChild(j).gameObject.SetActive(false);
            }
        }

        foreach (var item in RoleController.Instance._playerDic)
        {
            if (item.Value.Id == DataMgr.Instance.zhuangId)
                _after.transform.FindChild("role" + item.Key.idToPos() + "/zhuang").gameObject.SetActive(true);
            else
                _after.transform.FindChild("role" + item.Key.idToPos() + "/zhuang").gameObject.SetActive(false);
        }
        foreach (var item in DataMgr.Instance._showScore)
        {
            string str = "0";
            if (item.Value >= 0)
            {
                str = "<color='ff8e00ff'>" + item.Value + "</color>";
            }
            else
            {
                str = "<color='acaaa8ff'>" + item.Value + "</color>";
            }
            _after.transform.FindChild("role" + item.Key.idToPos() + "/score").GetComponent<Text>().text = str;
        }
        _leftCard.GetComponent<handCardProxy>().setTrue();
        _rightCard.GetComponent<handCardProxy>().setTrue();
        _topCard.GetComponent<handCardProxy>().setTrue();
        _selfCard.GetComponent<handCardProxy>().setTrue();

        _leftCard.GetComponent<handCardProxy>().reset();
        _rightCard.GetComponent<handCardProxy>().reset();
        _topCard.GetComponent<handCardProxy>().reset();
        _selfCard.GetComponent<handCardProxy>().reset();




        _rightHand.SetActive(false);
        _topHand.SetActive(false);
        _leftHand.SetActive(false);

        _zimo.SetActive(false);
        _huangju.SetActive(false);
        _dianpao.SetActive(false);
        _huIcon.SetActive(false);
        _huIcon1.SetActive(false);
        _huIcon2.SetActive(false);
        _huIcon3.SetActive(false);
        _maima.SetActive(false);
        _bzimo.SetActive(false);
        _bzimo1.SetActive(false);
        _bzimo2.SetActive(false);

        initIcon();

        _after.SetActive(true);
    }
    private void initHead()
    {
        for(int i=0;i<4;i++)
        {
            //IconMgr.Instance.SetHeadRawImage(_before.transform.FindChild("role" + i + "/head").GetComponent<RawImage>(), "111");
            _before.transform.FindChild("role" + i).gameObject.SetActive(false);

        }
    }
    private void initLeftCard(int num)
    {
        _leftCard.GetComponent<handCardProxy>().setHandCards(num);
    }
    private void initRightCard(int num)
    {
        _rightCard.GetComponent<handCardProxy>().setHandCards(num);
    }
    private void initTopCard(int num)
    {
        _topCard.GetComponent<handCardProxy>().setHandCards(num);
    }
    private void initSelfCard(int num)
    {
        _topCard.GetComponent<handCardProxy>().setHandCards(num);
    }
    private void initCard(bool flag = false)
    {
        if (flag)
        {
            if (GameConst.curId == MainRole.Instance.Id)
            {
                _handCard.SetActive(true);
                IconMgr.Instance.SetImage(_handCard.transform.FindChild("value").GetComponent<Image>(), "zm1_" + GameConst.curCard);
            }
            else
            {
                _handCard.SetActive(false);
            }
        }
        
        _selfCard.GetComponent<handCardProxy>().setHandCards(CardController.Instance.getCardsCount());
        int index = 0;
        for (int i =0 ; i < CardController.Instance._myCardList.Length; i++)
        {
            for (int j =0; j < CardController.Instance._myCardList[i].Count; j++)
            {
                if (_selfCard.transform.FindChild(index.ToString()) == null)
                    return;
                IconMgr.Instance.SetImage(_selfCard.transform.FindChild(index.ToString()).transform.FindChild("value").GetComponent<Image>(), "zm1_" + CardConst.GetCardBigNum(i, CardController.Instance._myCardList[i][j]));
                index++;
            }
        }
        
        //庄家判断
        initRightCard(DataMgr.Instance.rightCardNum);
        initTopCard(DataMgr.Instance.topCardNum);
        initLeftCard(DataMgr.Instance.leftCardNum);
    }
    private void addClick()
    {
        _always.transform.FindChild("set").GetComponent<Button>().onClick.AddListener(delegate {
            SetPanel.Instance.load(2);
        });

        _always.transform.FindChild("rule").GetComponent<Button>().onClick.AddListener(delegate
        {
            //InfoPanel.Instance.load();
            //TODO
            if (GameConst.VoteTimer==0||Time.time - GameConst.VoteTimer > GameConst.VoteTime)
            {
                AlertMgr.Instance.showAlert(ALERT_TYPE.type1, "是否解散房间", delegate
                {
                    ProtoReq.startVoteReq();
                    GameConst.VoteTimer = Time.time;
                });
            }
            else
            {
                QuickTips.ShowQuickTipsNor("解散房间操作太频繁，请稍后再试");
            }
           
        });
        UIEventHandlerBase.AddListener(_always.transform.FindChild("talk").gameObject, UIEventType.ON_POINTER_DOWN,onTalkDown);
        UIEventHandlerBase.AddListener(_always.transform.FindChild("talk").gameObject, UIEventType.ON_POINTER_UP, onTalkUp);
        _before.transform.FindChild("quit/dissolve").GetComponent<Button>().onClick.AddListener(delegate
        {
            AlertMgr.Instance.showAlert(ALERT_TYPE.type1, "是否解散房间", delegate ()
            {
                //DestoryPanel();
                ClosePanel();
                HallPanel.Instance.load();
                SoundMgr._instance.bgmPlay("beijing_dating" + GameConst.BGM, GameConst.musicVol);
                RoleController.Instance._playerDic.Clear();
                ProtoReq.Quit();
            });


        });

        _before.transform.FindChild("quit/quit").GetComponent<Button>().onClick.AddListener(delegate
        {
            AlertMgr.Instance.showAlert(ALERT_TYPE.type1, "是否退出房间", delegate ()
            {
                //DestoryPanel();
                ClosePanel();
                HallPanel.Instance.load();
                SoundMgr._instance.bgmPlay("beijing_dating" + GameConst.BGM, GameConst.musicVol);
                RoleController.Instance._playerDic.Clear();
                ProtoReq.Quit();
            });
        });
        _hu.onClick.AddListener(delegate
        {
            List<int> cards = new List<int>();
            List<int> cardsForType = new List<int>();
            for (int i = 0; i < CardController.Instance._myCardList.Length; i++)
            {
                for (int j = 0; j < CardController.Instance._myCardList[i].Count; j++)
                {
                    cards.Add(CardConst.GetCardBigNum(i, CardController.Instance._myCardList[i][j]));
                    cardsForType.Add(CardConst.GetCardBigNum(i, CardController.Instance._myCardList[i][j]));
                }
            }
            ///添加扛碰的牌给胡类型判断
            foreach (var item in DataMgr.Instance._everyPeng[0])
            {
                cardsForType.Add(item.Key);
                cardsForType.Add(item.Key);
                cardsForType.Add(item.Key);
            }
            foreach (var item in DataMgr.Instance._everyGang[0])
            {
                cardsForType.Add(item.Key);
                cardsForType.Add(item.Key);
                cardsForType.Add(item.Key);
            }
            Debug.Log("本地校验胡牌张数：" + cardsForType.Count);
            CardController.Instance.cleanUp();
            for (int i = 0; i < cardsForType.Count; i++)
            {
                CardController.Instance.addCard(cardsForType[i]);
            }
            if (cardsForType.Count == 14)
            {
                ProtoReq.Hu(CardController.Instance.getHuType(CardController.Instance._myCardList), DataMgr.Instance._curCard, cards);
            }
            else if (cardsForType.Count == 13)
            {
                CardController.Instance.addCard(CardConst.getCardInfo(DataMgr.Instance._curCard).type, CardConst.getCardInfo(DataMgr.Instance._curCard).value);
                ProtoReq.Hu(CardController.Instance.getHuType(CardController.Instance._myCardList), DataMgr.Instance._curCard, cards);
            }
            else
            {
               
            }
            _fun.SetActive(false);
            _hu.gameObject.SetActive(false);
            _peng.gameObject.SetActive(false);
            _gang.gameObject.SetActive(false);
            _gangs.SetActive(false);
        });
        _gang.onClick.AddListener(delegate {
           
            gangBack.Invoke();
            _fun.SetActive(false);
            _hu.gameObject.SetActive(false);
            _peng.gameObject.SetActive(false);
            _gang.gameObject.SetActive(false);
            _gangs.gameObject.SetActive(false);
        });
        _peng.onClick.AddListener(delegate {
            ProtoReq.Peng(DataMgr.Instance._curCard);
            GameConst.isTurn = true;
            _fun.SetActive(false);
            _hu.gameObject.SetActive(false);
            _peng.gameObject.SetActive(false);
            _gang.gameObject.SetActive(false);
            _gangs.SetActive(false);
        });

        _pass.onClick.AddListener(delegate {
            ProtoReq.Pass(-1);
            _fun.SetActive(false);
            _hu.gameObject.SetActive(false);
            _peng.gameObject.SetActive(false);
            _gang.gameObject.SetActive(false);
            _gangs.SetActive(false);
            if (huCard != 0)
            {
                huCard = 0;
            }
        });
        _before.transform.FindChild("invite").GetComponent<Button>().onClick.AddListener(delegate
        {
            weixinInvite();
        });
    }
    private void selfGang(int card,int pos,bool flag)
    {
        GameObject obj = _selfCard.GetComponent<handCardProxy>().gang(0,pos, card,flag);
        CardController.Instance.gang(CardConst.getCardInfo(card).type, CardConst.getCardInfo(card).value);
        initCard();
        Debug.Log("重新整牌");
       
    }

    private void topGang(int card, int pos,bool flag)
    {
        if(!flag)
            DataMgr.Instance.topCardNum -= 3;
        initTopCard(DataMgr.Instance.topCardNum);
        GameObject obj = _topCard.GetComponent<handCardProxy>().gang(2,pos, card,flag);
        
    }

    private void leftGang(int card, int pos,bool flag)
    {
        if(!flag)
            DataMgr.Instance.leftCardNum -= 3;
        initLeftCard(DataMgr.Instance.leftCardNum);
        GameObject obj = _leftCard.GetComponent<handCardProxy>().gang(3,pos, card,flag);
       
    }
    private void rightGang(int card, int pos,bool flag)
    {
        if(!flag)
            DataMgr.Instance.rightCardNum -= 3;
        initRightCard(DataMgr.Instance.rightCardNum);
        GameObject obj = _rightCard.GetComponent<handCardProxy>().gang(1,pos, card,flag);
        
    }

    private void selfPeng(int card,int pos)
    {
        GameObject obj =_selfCard.GetComponent<handCardProxy>().peng(0, pos, card);
        //CardController.Instance.peng(CardConst.getCardInfo(card).type, CardConst.getCardInfo(card).value);
        IconMgr.Instance.SetImage(_handCard.transform.FindChild("value").GetComponent<Image>(), "zm1_" + DataMgr.Instance._curCard);
        CardController.Instance.delCard(CardConst.getCardInfo(DataMgr.Instance._curCard).type, CardConst.getCardInfo(DataMgr.Instance._curCard).value);
        initCard();
        _handCard.SetActive(true);
    }

    private void topPeng(int card,int pos)
    {
        DataMgr.Instance.topCardNum -= 2;
        initTopCard(DataMgr.Instance.topCardNum);
        GameObject obj = _topCard.GetComponent<handCardProxy>().peng(2, pos, card);
        Debug.Log("pengPos" + pos);
       
    }

    private void leftPeng(int card,int pos)
    {
        DataMgr.Instance.leftCardNum -= 2;
        initLeftCard(DataMgr.Instance.leftCardNum);
        GameObject obj = _leftCard.GetComponent<handCardProxy>().peng(3, pos, card);
        Debug.Log("pengPos" + pos);
       
    }

    private void rightPeng(int card,int pos)
    {
        DataMgr.Instance.rightCardNum -= 2;
        initRightCard(DataMgr.Instance.rightCardNum);
        GameObject obj  = _rightCard.GetComponent<handCardProxy>().peng(1, pos, card);
        Debug.Log("pengPos" + pos);
       
    }
    public override void AddListener()
    {
        base.AddListener();
        EventDispatcher.Instance.AddEventListener<int>(GameEventConst.READY_TO_PALY, onReady);
        EventDispatcher.Instance.AddEventListener(GameEventConst.CARD_TO_HAND, getCard);
        EventDispatcher.Instance.AddEventListener<int,int>(GameEventConst.GET_NEW_CARD, getCurCard);
        EventDispatcher.Instance.AddEventListener<int, int>(GameEventConst.PUT_HE_CARD, putHeCard);
        EventDispatcher.Instance.AddEventListener<Table.Role>(GameEventConst.ADD_PLAYER, addPlayer);
        EventDispatcher.Instance.AddEventListener(GameEventConst.START, onStart);
        EventDispatcher.Instance.AddEventListener<int, int, int>(GameEventConst.PENG, onPeng);
        EventDispatcher.Instance.AddEventListener<int, int, int,bool>(GameEventConst.GANG, onGang);
        EventDispatcher.Instance.AddEventListener(GameEventConst.TIME_COUNT_END, onTimeEnd);
        EventDispatcher.Instance.AddEventListener(GameEventConst.ROUND_SCORE, onRoundScore);
        EventDispatcher.Instance.AddEventListener<int,int,List<int>,int,int>(GameEventConst.HU, onHu);
        EventDispatcher.Instance.AddEventListener(GameEventConst.HUANGJU, onHuang);
        EventDispatcher.Instance.AddEventListener<int>(GameEventConst.SOMEOME_QUIT, someQuit);
        EventDispatcher.Instance.AddEventListener(GameEventConst.MAINMA_DATA, setMaima);
        EventDispatcher.Instance.AddEventListener(GameEventConst.JIXU, jixu);
        EventDispatcher.Instance.AddEventListener<int,bool>(GameEventConst.AUTO, setAuto);
        EventDispatcher.Instance.AddEventListener<int>(GameEventConst.READY_NEXT, onNextReady);
        //EventDispatcher.Instance.AddEventListener<bool>(GameEventConst.RECONNECT, onReconnect);
        EventDispatcher.Instance.AddEventListener<string>(GameEventConst.RSP_COUNT, rspCount);
        EventDispatcher.Instance.AddEventListener(GameEventConst.SELF_NEXT, readyNext);
        EventDispatcher.Instance.AddEventListener<int, bool>(GameEventConst.PLAYER_OUT, onPlayerOut);
    }

    public override void RemoveListener()
    {
        base.RemoveListener();
        EventDispatcher.Instance.RemoveEventListener<int>(GameEventConst.READY_TO_PALY, onReady);
        EventDispatcher.Instance.RemoveEventListener(GameEventConst.CARD_TO_HAND, getCard);
        EventDispatcher.Instance.RemoveEventListener<int,int>(GameEventConst.GET_NEW_CARD, getCurCard);
        EventDispatcher.Instance.RemoveEventListener<int, int>(GameEventConst.PUT_HE_CARD, putHeCard);
        EventDispatcher.Instance.RemoveEventListener<Table.Role>(GameEventConst.ADD_PLAYER, addPlayer);
        EventDispatcher.Instance.RemoveEventListener(GameEventConst.START, onStart);
        EventDispatcher.Instance.RemoveEventListener<int, int, int>(GameEventConst.PENG, onPeng);
        EventDispatcher.Instance.RemoveEventListener<int, int, int,bool>(GameEventConst.GANG, onGang);
        EventDispatcher.Instance.RemoveEventListener(GameEventConst.TIME_COUNT_END, onTimeEnd);
        EventDispatcher.Instance.RemoveEventListener(GameEventConst.ROUND_SCORE, onRoundScore);
        EventDispatcher.Instance.RemoveEventListener<int,int, List<int>,int,int>(GameEventConst.HU, onHu);
        EventDispatcher.Instance.RemoveEventListener(GameEventConst.HUANGJU, onHuang);
        EventDispatcher.Instance.RemoveEventListener<int>(GameEventConst.SOMEOME_QUIT, someQuit);
        EventDispatcher.Instance.RemoveEventListener(GameEventConst.MAINMA_DATA, setMaima);
        EventDispatcher.Instance.RemoveEventListener(GameEventConst.JIXU, jixu);
        EventDispatcher.Instance.RemoveEventListener<int,bool>(GameEventConst.AUTO, setAuto);
        EventDispatcher.Instance.RemoveEventListener<int>(GameEventConst.READY_NEXT, onNextReady);
        //EventDispatcher.Instance.RemoveEventListener<bool>(GameEventConst.RECONNECT, onReconnect);
        EventDispatcher.Instance.RemoveEventListener<string>(GameEventConst.RSP_COUNT, rspCount);
        EventDispatcher.Instance.RemoveEventListener(GameEventConst.SELF_NEXT, readyNext);
        EventDispatcher.Instance.RemoveEventListener<int, bool>(GameEventConst.PLAYER_OUT, onPlayerOut);
    }
    private void onPlayerOut(int id, bool boo)
    {
        if (boo)
        {
            _after.transform.FindChild("role" + id.idToPos() + "/state").GetComponent<Text>().text = "";
        }
        else
        {
            _after.transform.FindChild("role" +id.idToPos() + "/state").GetComponent<Text>().text = "离线";
        }
    }
    private void rspCount(string str)
    {
        _always.transform.FindChild("rspCount").GetComponent<Text>().text = str;
    }

    private void readyNext()
    {
        jixu();
        if (GameConst.startStatus == 1)
        {
            _before.SetActive(false);
            _after.SetActive(true);
            _center.SetActive(true);
        }
        ProtoReq.readyNext();
    }

    private void onNextReady(int id)
    {
        if (id == MainRole.Instance.Id)
        {
           
            foreach (var player in RoleController.Instance._playerDic)
            {
                _before.transform.FindChild("role" + player.Value.Pos).gameObject.SetActive(true);
                //SDKMgr.Instance.SetAsyncImage(player.Value.Url, _before.transform.FindChild("role" + player.Value.Pos + "/head").GetComponent<Image>());
                if (DataMgr.Instance._readyNextIDs.Contains(player.Value.Id))
                {
                    _before.transform.FindChild("role" + player.Value.Pos + "/ready").gameObject.SetActive(true);
                }
                else
                {
                    _before.transform.FindChild("role" + player.Value.Pos + "/ready").gameObject.SetActive(false);
                }
            }
        }
        else
        {
            _before.transform.FindChild("role" + RoleController.Instance.getPlayerById(id).Pos + "/ready").gameObject.SetActive(true);
        }
    }
    private void setAuto(int id,bool flag)
    {
        if (id == MainRole.Instance.Id)
        {
            if (flag)
            {
                GameConst.auto.SetActive(true);
            }
            else
            {
                GameConst.auto.SetActive(false);
            }
            //关掉所有显示
            _hu.gameObject.SetActive(false);
            _peng.gameObject.SetActive(false);
            _gang.gameObject.SetActive(false);
            _gangs.SetActive(false);
            _fun.gameObject.SetActive(false);
        }
    }

    private void onReconnect()
    {
        Debug.Log("断线重连");
        initFeng();
        _state.text = DataMgr.Instance.curRound == 0 ? "等待中" :"第"+ DataMgr.Instance.curRound+"局";
        initAfter();
        //打牌
        _after.SetActive(true);
        //头像
        foreach (var item in RoleController.Instance._playerDic)
        {
            Transform obj = _after.transform.FindChild("role" + item.Value.Id.idToPos());
            if (obj != null)
            {
                SDKMgr.Instance.SetAsyncImage(item.Value.Url, obj.FindChild("head").GetComponent<Image>());
            }
            SDKMgr.Instance.SetAsyncImage(item.Value.Url, _before.transform.FindChild("role" + item.Value.Pos + "/head").GetComponent<Image>());
        }
            //分数
            //桌子上的牌
            _hu.gameObject.SetActive(false);
            _peng.gameObject.SetActive(false);
            _gang.gameObject.SetActive(false);
            _gangs.SetActive(false);
            _fun.gameObject.SetActive(false);
        for (int k = 0; k < 4; k++)
            {
                CardProxy proxy = _card.transform.FindChild(k.ToString()).GetComponent<CardProxy>();

                for (int i = 0; i < proxy.cards.Length; i++)
                {
                    if (i < DataMgr.Instance._heCards[k].Count - 1)
                    {
                        proxy.cards[i].SetActive(true);
                        IconMgr.Instance.SetImage(proxy.cards[i].transform.FindChild("value").GetComponent<Image>(), proxy.headStr + DataMgr.Instance._heCards[k][i]);
                    }
                    else if (i == DataMgr.Instance._heCards[k].Count - 1)
                    {
                        proxy.cards[i].SetActive(true);
                        IconMgr.Instance.SetImage(proxy.cards[i].transform.FindChild("value").GetComponent<Image>(), proxy.headStr + DataMgr.Instance._heCards[k][i]);
                        //设置点;
                        _point.SetActive(true);
                        _point.transform.position = proxy.cards[i].transform.position;
                    }
                    else
                    {
                        proxy.cards[i].SetActive(false);
                    }
                }
            }

            //手里的牌
            initCard(true);
            //碰杠
            for (int j= 0; j < 4;j++)
            {
                foreach (var item in DataMgr.Instance._everyPeng[j])
                {
                    switch (j)
                    {
                        case 0:
                            _selfCard.GetComponent<handCardProxy>().peng(0, item.Value.fromPos, item.Value.card);
                            break;
                        case 1:
                            _rightCard.GetComponent<handCardProxy>().peng(1, item.Value.fromPos, item.Value.card);
                            break;
                        case 2:
                            _topCard.GetComponent<handCardProxy>().peng(2, item.Value.fromPos, item.Value.card);
                            break;
                        case 3:
                            _leftCard.GetComponent<handCardProxy>().peng(3, item.Value.fromPos, item.Value.card);
                            break;
                            
                    }
                   
                }

                foreach (var item in DataMgr.Instance._everyGang[j])
                {

                    switch (j)
                    {
                        case 0:
                            _selfCard.GetComponent<handCardProxy>().gang(0, item.Value.fromPos, item.Value.card,item.Value.isBu);
                            break;
                        case 1:
                            _rightCard.GetComponent<handCardProxy>().gang(1, item.Value.fromPos, item.Value.card, item.Value.isBu);
                            break;
                        case 2:
                            _topCard.GetComponent<handCardProxy>().gang(2, item.Value.fromPos, item.Value.card, item.Value.isBu);
                            break;
                        case 3:
                            _leftCard.GetComponent<handCardProxy>().gang(3, item.Value.fromPos, item.Value.card, item.Value.isBu);
                            break;

                    }

                }
            }

            //中间字:
            _after.transform.FindChild("text").GetComponent<Text>().text = "剩余<color='#ffde00'>" + DataMgr.Instance.lassCardsNum + "</color>张  第<color='#ffde00'>" + DataMgr.Instance.curRound + "/" + DataMgr.Instance.sumRound + "</color>局";
        DataMgr.Instance._curCard = GameConst.curCard;
        if (GameConst.curId == MainRole.Instance.Id)
        {
            //TODO
            getCurCard(GameConst.curId, GameConst.curCard);
        }
        else
        {
            //putHeCard(GameConst.curId, GameConst.curCard);
            if (GameConst.curCard != 0 || GameConst.curCard != -1)
            {
                
                if (_card == null)
                    return;
                _hu.gameObject.SetActive(false);
                _peng.gameObject.SetActive(false);
                _gang.gameObject.SetActive(false);
                _gangs.SetActive(false);
                _fun.gameObject.SetActive(false);
                Debug.Log("检测00000");
                bool isPass = true;
                Card card1 = new Card(DataMgr.Instance._curCard);
                //一系列判断
                if (CardController.Instance.checkPeng(card1.CardType, card1.CardNum))
                {
                    _fun.SetActive(true);
                    _peng.gameObject.SetActive(true);
                    isPass = false;
                }
                List<int>[] cards = CardController.Instance.getCopyCard(CardController.Instance._myCardList);
                cards.addCard(card1.CardType, card1.CardNum);
                if (CardController.Instance.checkCard(cards))
                {
                    DataMgr.Instance.isHu = true;
                    _fun.SetActive(true);
                    _hu.gameObject.SetActive(true);
                    ProtoReq.canHu();
                    isPass = false;
                    huCard = GameConst.curCard;
                }
                else
                {
                    DataMgr.Instance.isHu = false;

                    _hu.gameObject.SetActive(false);
                }
                if (CardController.Instance.checkMingGang(card1.CardType, card1.CardNum) != 0)
                {
                    _fun.SetActive(true);
                    _gang.gameObject.SetActive(true);
                    gangBack = delegate ()
                    {
                        ProtoReq.Gang(CardConst.GetCardBigNum(card1.CardType, card1.CardNum));
                    };
                    isPass = false;
                }
                if (isPass)
                {
                    ProtoReq.Pass(DataMgr.Instance._curCard);
                }
                else
                {

                }

                cards = null;
            }
        }


    }
    private void jixu()
    {
        _hu.gameObject.SetActive(false);
        _peng.gameObject.SetActive(false);
        _gang.gameObject.SetActive(false);
        _gangs.SetActive(false);
        _fun.SetActive(false);
        _after.SetActive(false);
        _before.SetActive(true);
        _before.transform.FindChild("invite").gameObject.SetActive(false);
        _before.transform.FindChild("quit").gameObject.SetActive(false);
        for (int i = 0; i < 4; i++)
        {
            if (i == MainRole.Instance.Pos)
            {
                _before.transform.FindChild("role" + i).gameObject.SetActive(true);
                _before.transform.FindChild("role" + i + "/ready").gameObject.SetActive(true);
            }
               
            else
                _before.transform.FindChild("role" + i).gameObject.SetActive(false);
        }
        _center.SetActive(false);
        _count.SetActive(false);
    }

    private void someQuit(int id)
    {
        _before.transform.FindChild("role" + RoleController.Instance._playerDic[id].Pos).gameObject.SetActive(false);
        
        RoleController.Instance._playerDic.Remove(id);
    }
    private void onHuang()
    {
        _huangju.SetActive(true);
    }

    //胡以后的摊牌
    private void onHu(int id,int from, List<int> cards,int card,int exType)
    {
        _topHand.SetActive(false);
        _leftHand.SetActive(false);
        _rightHand.SetActive(false);
        if (id!=MainRole.Instance.Id&&from!=MainRole.Instance.Id)
        {
            if (!DataMgr.Instance.isHu)
            {
                ProtoReq.PassHu();
            }
        }
       
        switch (from.idToPos())
        {
            case 0:
                if (id == from)
                {
                    _zimo.SetActive(true);
                    IconMgr.Instance.SetImage(_zimo.transform.FindChild("value").GetComponent<Image>(), "zm2_" + card);
                    _zimo.transform.localPosition = GameConst.selfPosition;
                    if (exType == 1)
                    {
                        IconMgr.Instance.SetImage(_zimo.GetComponent<Image>(), "hdlyRoom", true);
                    }
                    if (exType == 2)
                    {
                        IconMgr.Instance.SetImage(_zimo.GetComponent<Image>(), "bgRoom", true);
                    }
                    _bzimo.SetActive(true);
                    _bzimo1.SetActive(true);
                    _bzimo2.SetActive(true);
                    _bzimo.transform.localPosition = GameConst.rightPosition;
                    _bzimo1.transform.localPosition = GameConst.topPosition;
                    _bzimo2.transform.localPosition = GameConst.leftPosition;
                }
                else
                {
                    _dianpao.SetActive(true);
                    _dianpao.transform.localPosition = GameConst.selfPosition;
                    if (exType == 3)
                    {
                        IconMgr.Instance.SetImage(_dianpao.GetComponent<Image>(), "gspRoom", true);
                    }
                    Vector3 endPos = Vector3.zero;
                    switch (id.idToPos())
                    {
                        case 1:
                            _huIcon1.SetActive(true);
                            _huIcon1.transform.localPosition = GameConst.rightPosition;
                            if (exType == 4)
                            {
                                IconMgr.Instance.SetImage(_huIcon1.GetComponent<Image>(), "gshRoom", true);
                            }
                            endPos = _huIcon1.transform.FindChild("pos").transform.position;
                            break;
                        case 2:
                            _huIcon2.SetActive(true);
                            _huIcon2.transform.localPosition = GameConst.topPosition;
                            if (exType == 4)
                            {
                                IconMgr.Instance.SetImage(_huIcon2.GetComponent<Image>(), "gshRoom", true);
                            }
                            endPos = _huIcon2.transform.FindChild("pos").transform.position;
                            break;
                        case 3:
                            _huIcon3.SetActive(true);
                            _huIcon3.transform.localPosition = GameConst.leftPosition;
                            if (exType == 4)
                            {
                                IconMgr.Instance.SetImage(_huIcon3.GetComponent<Image>(), "gshRoom", true);
                            }
                            endPos = _huIcon3.transform.FindChild("pos").transform.position;
                            break;
                    }
                    GameObject cardObj = _dianpao.transform.FindChild("card").gameObject;
                    IconMgr.Instance.SetImage(cardObj.transform.FindChild("value").GetComponent<Image>(), "zm2_" + card);
                    GameObject cardCopy = GameObject.Instantiate(cardObj, cardObj.transform.parent, true) as GameObject;
                    cardCopy.transform.localScale = Vector3.zero;
                    cardCopy.SetActive(true);
                    DoTweenFun.FlyCard(cardCopy.transform, endPos, 1.5f);
                }
                break;
            case 1:
                if (id == from)
                {
                    _zimo.SetActive(true);
                    IconMgr.Instance.SetImage(_zimo.transform.FindChild("value").GetComponent<Image>(), "zm2_" + card);
                    _zimo.transform.localPosition = GameConst.rightPosition;
                    if (exType == 1)
                    {
                        IconMgr.Instance.SetImage(_zimo.GetComponent<Image>(), "hdlyRoom", true);
                    }
                    if (exType == 2)
                    {
                        IconMgr.Instance.SetImage(_zimo.GetComponent<Image>(), "bgRoom", true);
                    }
                    _bzimo.SetActive(true);
                    _bzimo1.SetActive(true);
                    _bzimo2.SetActive(true);
                    _bzimo.transform.localPosition = GameConst.selfPosition;
                    _bzimo1.transform.localPosition = GameConst.topPosition;
                    _bzimo2.transform.localPosition = GameConst.leftPosition;
                }
                else
                {
                    _dianpao.SetActive(true);
                    _dianpao.transform.localPosition = GameConst.rightPosition;
                    if (exType == 3)
                    {
                        IconMgr.Instance.SetImage(_dianpao.GetComponent<Image>(), "gspRoom", true);
                    }
                    Vector3 endPos = Vector3.zero;
                    switch (id.idToPos())
                    {
                        case 0:
                            _huIcon.SetActive(true);
                            _huIcon.transform.localPosition = GameConst.selfPosition;
                            if (exType == 4)
                            {
                                IconMgr.Instance.SetImage(_huIcon.GetComponent<Image>(), "gshRoom", true);
                            }
                            endPos = _huIcon.transform.FindChild("pos").transform.position;
                            break;
                        case 2:
                            _huIcon2.SetActive(true);
                            _huIcon2.transform.localPosition = GameConst.topPosition;
                            if (exType == 4)
                            {
                                IconMgr.Instance.SetImage(_huIcon2.GetComponent<Image>(), "gshRoom", true);
                            }
                            endPos = _huIcon2.transform.FindChild("pos").transform.position;
                            break;
                        case 3:
                            _huIcon3.SetActive(true);
                            _huIcon3.transform.localPosition = GameConst.leftPosition;
                            if (exType == 4)
                            {
                                IconMgr.Instance.SetImage(_huIcon3.GetComponent<Image>(), "gshRoom", true);
                            }
                            endPos = _huIcon3.transform.FindChild("pos").transform.position;
                            break;
                    }
                    GameObject cardObj = _dianpao.transform.FindChild("card").gameObject;
                    IconMgr.Instance.SetImage(cardObj.transform.FindChild("value").GetComponent<Image>(), "zm2_" + card);
                    GameObject cardCopy = GameObject.Instantiate(cardObj, cardObj.transform.parent, true) as GameObject;
                    cardCopy.transform.localScale = Vector3.zero;
                    cardCopy.SetActive(true);
                    DoTweenFun.FlyCard(cardCopy.transform, endPos, 1.5f);
                }
                break;
            case 2:
                if (id == from)
                {
                    _zimo.SetActive(true);
                    IconMgr.Instance.SetImage(_zimo.transform.FindChild("value").GetComponent<Image>(), "zm2_" + card);
                    _zimo.transform.localPosition = GameConst.topPosition;
                    if (exType == 1)
                    {
                        IconMgr.Instance.SetImage(_zimo.GetComponent<Image>(), "hdlyRoom", true);
                    }
                    if (exType == 2)
                    {
                        IconMgr.Instance.SetImage(_zimo.GetComponent<Image>(), "bgRoom", true);
                    }
                    _bzimo.SetActive(true);
                    _bzimo1.SetActive(true);
                    _bzimo2.SetActive(true);
                    _bzimo.transform.localPosition = GameConst.rightPosition;
                    _bzimo1.transform.localPosition = GameConst.selfPosition;
                    _bzimo2.transform.localPosition = GameConst.leftPosition;
                }
                else
                {
                    _dianpao.SetActive(true);
                    _dianpao.transform.localPosition = GameConst.topPosition;
                    if (exType == 3)
                    {
                        IconMgr.Instance.SetImage(_dianpao.GetComponent<Image>(), "gspRoom", true);
                    }
                    Vector3 endPos = Vector3.zero;
                    switch (id.idToPos())
                    {
                        case 1:
                            _huIcon1.SetActive(true);
                            _huIcon1.transform.localPosition = GameConst.rightPosition;
                            if (exType == 4)
                            {
                                IconMgr.Instance.SetImage(_huIcon1.GetComponent<Image>(), "gshRoom", true);
                            }
                            endPos = _huIcon1.transform.FindChild("pos").transform.position;
                            break;
                        case 0:
                            _huIcon.SetActive(true);
                            _huIcon.transform.localPosition = GameConst.selfPosition;
                            if (exType == 4)
                            {
                                IconMgr.Instance.SetImage(_huIcon.GetComponent<Image>(), "gshRoom", true);
                            }
                            endPos = _huIcon.transform.FindChild("pos").transform.position;
                            break;
                        case 3:
                            _huIcon3.SetActive(true);
                            _huIcon3.transform.localPosition = GameConst.leftPosition;
                            if (exType == 4)
                            {
                                IconMgr.Instance.SetImage(_huIcon3.GetComponent<Image>(), "gshRoom", true);
                            }
                            endPos = _huIcon3.transform.FindChild("pos").transform.position;
                            break;
                    }
                    GameObject cardObj = _dianpao.transform.FindChild("card").gameObject;
                    IconMgr.Instance.SetImage(cardObj.transform.FindChild("value").GetComponent<Image>(), "zm2_" + card);
                    GameObject cardCopy = GameObject.Instantiate(cardObj, cardObj.transform.parent, true) as GameObject;
                    cardCopy.transform.localScale = Vector3.zero;
                    cardCopy.SetActive(true);
                    DoTweenFun.FlyCard(cardCopy.transform, endPos, 1.5f);
                }
                break;
            case 3:
                if (id == from)
                {
                    _zimo.SetActive(true);
                    IconMgr.Instance.SetImage(_zimo.transform.FindChild("value").GetComponent<Image>(), "zm2_" + card);
                    _zimo.transform.localPosition = GameConst.leftPosition;
                    if (exType == 1)
                    {
                        IconMgr.Instance.SetImage(_zimo.GetComponent<Image>(), "hdlyRoom", true);
                    }
                    if (exType == 2)
                    {
                        IconMgr.Instance.SetImage(_zimo.GetComponent<Image>(), "bgRoom", true);
                    }
                    _bzimo.SetActive(true);
                    _bzimo1.SetActive(true);
                    _bzimo2.SetActive(true);
                    _bzimo.transform.localPosition = GameConst.rightPosition;
                    _bzimo1.transform.localPosition = GameConst.topPosition;
                    _bzimo2.transform.localPosition = GameConst.selfPosition;
                }
                else
                {
                    _dianpao.SetActive(true);
                    _dianpao.transform.localPosition = GameConst.leftPosition;
                    if (exType == 3)
                    {
                        IconMgr.Instance.SetImage(_dianpao.GetComponent<Image>(), "gspRoom", true);
                    }
                    Vector3 endPos = Vector3.zero;
                    switch (id.idToPos())
                    {
                        case 1:
                            _huIcon1.SetActive(true);
                            _huIcon1.transform.localPosition = GameConst.rightPosition;
                            if (exType == 4)
                            {
                                IconMgr.Instance.SetImage(_huIcon1.GetComponent<Image>(), "gshRoom", true);
                            }
                            endPos = _huIcon1.transform.FindChild("pos").transform.position;
                            break;
                        case 2:
                            _huIcon2.SetActive(true);
                            _huIcon2.transform.localPosition = GameConst.topPosition;
                            if (exType == 4)
                            {
                                IconMgr.Instance.SetImage(_huIcon2.GetComponent<Image>(), "gshRoom", true);
                            }
                            endPos = _huIcon2.transform.FindChild("pos").transform.position;
                            break;
                        case 0:
                            _huIcon.SetActive(true);
                            _huIcon.transform.localPosition = GameConst.selfPosition;
                            if (exType == 4)
                            {
                                IconMgr.Instance.SetImage(_huIcon.GetComponent<Image>(), "gshRoom", true);
                            }
                            endPos = _huIcon.transform.FindChild("pos").transform.position;
                            break;
                    }
                    GameObject cardObj = _dianpao.transform.FindChild("card").gameObject;
                    IconMgr.Instance.SetImage(cardObj.transform.FindChild("value").GetComponent<Image>(), "zm2_" + card);
                    GameObject cardCopy = GameObject.Instantiate(cardObj, cardObj.transform.parent, true) as GameObject;
                    cardCopy.transform.localScale = Vector3.zero;
                    cardCopy.SetActive(true);
                    DoTweenFun.FlyCard(cardCopy.transform, endPos, 1.5f);
                }
                break;

        }
        cards.Sort();
        //cards.Reverse();
        _handCard.SetActive(false);
        switch (id.idToPos())
        {
            // _after.transform.FindChild()
            case 0:
                _selfCard.GetComponent<handCardProxy>().setFalse();
                _selfCard.GetComponent<handCardProxy>().isHu = true;
                _selfCard.GetComponent<handCardProxy>().setCard(cards,"zm2_");
                break;
            case 1:
                _rightCard.GetComponent<handCardProxy>().setFalse();
                _rightCard.GetComponent<handCardProxy>().isHu = true;
                _rightCard.GetComponent<handCardProxy>().setCard(cards,"youmian2_");
                break;
            case 2:
                _topCard.GetComponent<handCardProxy>().setFalse();
                _topCard.GetComponent<handCardProxy>().isHu = true;
                _topCard.GetComponent<handCardProxy>().setCard(cards,"shangmian1_");
                break;
            case 3:
                _leftCard.GetComponent<handCardProxy>().setFalse();
                _leftCard.GetComponent<handCardProxy>().isHu = true;
                _leftCard.GetComponent<handCardProxy>().setCard(cards,"left2_");
                break;
        }

       
    }
    private void onRoundScore()
    {
        _fun.SetActive(false);
        _hu.gameObject.SetActive(false);
        _peng.gameObject.SetActive(false);
        _gang.gameObject.SetActive(false);
        _gangs.SetActive(false);
        //隐藏手牌

        _selfCard.GetComponent<handCardProxy>().setFalse();
        _rightCard.GetComponent<handCardProxy>().setFalse();
        _topCard.GetComponent<handCardProxy>().setFalse();
        _leftCard.GetComponent<handCardProxy>().setFalse();
        _handCard.SetActive(false);

        //显示摊牌
        //pos0
        if (!_selfCard.GetComponent<handCardProxy>().isHu)
        {
            _selfCard.GetComponent<handCardProxy>().isHu = true;
            _selfCard.GetComponent<handCardProxy>().setFalse();
            _selfCard.GetComponent<handCardProxy>().setCard(DataMgr.Instance._leftCardsDic[0],"zm2_");
        }
        //pos1
        if (!_rightCard.GetComponent<handCardProxy>().isHu)
        {
            _rightCard.GetComponent<handCardProxy>().isHu = true;
            _rightCard.GetComponent<handCardProxy>().setFalse();
            _rightCard.GetComponent<handCardProxy>().setCard(DataMgr.Instance._leftCardsDic[1], "youmian2_");
        }
        //pos2
        if (!_topCard.GetComponent<handCardProxy>().isHu)
        {
            _topCard.GetComponent<handCardProxy>().isHu = true;
            _topCard.GetComponent<handCardProxy>().setFalse();
            _topCard.GetComponent<handCardProxy>().setCard(DataMgr.Instance._leftCardsDic[2], "shangmian1_");
        }
        //pos3
        if (!_leftCard.GetComponent<handCardProxy>().isHu)
        {
            _leftCard.GetComponent<handCardProxy>().isHu = true;
            _leftCard.GetComponent<handCardProxy>().setFalse();
            _leftCard.GetComponent<handCardProxy>().setCard(DataMgr.Instance._leftCardsDic[3], "left2_");
        }

        //GlobleTimer.Instance.SetTimer(GameConst.tanpai,1, delegate {
        //    _huangju.SetActive(false);
        //    if (_dianpao != null)
        //        _dianpao.transform.destoryChild(true);
        //    DanjuPanel.Instance.load();
        //    //copyAfter();
        //});
        Timer time = new Timer(delegate
        {
           
            Loom.QueueOnMainThread(() => {
                _huangju.SetActive(false);
                if (_dianpao != null)
                    _dianpao.transform.destoryChild(true);
                DanjuPanel.Instance.load();
            });
        }, this, GameConst.tanpai, 0);


    }
    private void onTimeEnd()
    {
        //if (isTurn)
        //{
        //    isTurn = false;
          
        //    if (_handCard.activeSelf)
        //    {
        //        string name = _handCard.transform.FindChild("value").GetComponent<Image>().sprite.name;
        //        int num = GameTools.getCardNumByName(name);
        //        Debug.Log("牌号:" + GameTools.getCardNumByName(name));
        //        CardController.Instance.delCard(CardConst.getCardInfo(num).type, CardConst.getCardInfo(num).value);
        //        ProtoReq.Play(num);
        //        //setSelfHe(num);
        //        initCard();
        //    }
        //    else
        //    {
        //        ProtoReq.Pass();
        //    }
        //    endTimeCount();
        //}
    }

    private void onPeng(int pos,int fromPos,int card)
    {
        Debug.Log("转化为pos" + pos);
        Debug.Log("来自pos" + fromPos);
        beginTimeCount();
        foreach (var item in RoleController.Instance._playerDic)
        {
            if (item.Value.Id.idToPos() == pos)
                setCenterPos(item.Value.Id);
        }
        
        _card.transform.FindChild(fromPos.ToString()).GetComponent<CardProxy>().cards[DataMgr.Instance._heCards[fromPos].Count - 1].SetActive(false);
        switch (pos)
        {
            case 0:
                selfPeng(card, fromPos);
                GameConst.isTurn = true;
                break;
            case 1:
                rightPeng(card, fromPos);
                break;
            case 2:
                topPeng(card, fromPos);
                break;
            case 3:
                leftPeng(card, fromPos);
                break;
        }
    }

    private void onGang(int pos, int fromPos, int card,bool flag)
    {
        beginTimeCount();
        if (DataMgr.Instance._everyPeng[pos].ContainsKey(card))
        {
            GameObject.Destroy(DataMgr.Instance._everyPeng[pos][card].obj);
            DataMgr.Instance._everyPeng[pos].Remove(card);
        }
        if (pos != fromPos)
        {
            _card.transform.FindChild(fromPos.ToString()).GetComponent<CardProxy>().cards[DataMgr.Instance._heCards[fromPos].Count - 1].SetActive(false);
        }
       
        switch (pos)
        {
            case 0:
                selfGang(card, fromPos,flag);
                GameConst.isTurn = true;
                break;
            case 1:
                rightGang(card, fromPos,flag);
                break;
            case 2:
                topGang(card, fromPos,flag);
                break;
            case 3:
                leftGang(card, fromPos,flag);
                break;
        }

        if (flag && pos != 0)
        {
            List<int>[] cards = CardController.Instance.getCopyCard(CardController.Instance._myCardList);
            Card ca = new Card(card);
            cards.addCard(ca.CardType, ca.CardNum);
            if (CardController.Instance.checkCard(cards))
            {
                DataMgr.Instance.isHu = true;
                _fun.SetActive(true);
                _hu.gameObject.SetActive(true);
                ProtoReq.canHu();
                huCard = card;
            }
            else
            {
                DataMgr.Instance.isHu = false;
                _hu.gameObject.SetActive(false);
                _fun.gameObject.SetActive(false);
                ProtoReq.Pass(DataMgr.Instance._curCard);
            }

        }
    }
    private void addPlayer(Table.Role role)
    {
       
        if (role == null)
        {
            foreach (var item in RoleController.Instance._playerDic)
            {
                Transform obj = _before.transform.FindChild("role" + item.Value.Pos);
                if (obj != null)
                {
                    obj.gameObject.SetActive(true);
                    SDKMgr.Instance.SetAsyncImage(item.Value.Url, obj.FindChild("head").GetComponent<Image>());
                    obj.FindChild("head").gameObject.SetActive(true);
                    _before.transform.FindChild("role" + item.Value.Pos + "/ready").gameObject.SetActive(item.Value.IsReady);
                    SDKMgr.Instance.SetAsyncImage(item.Value.Url, _after.transform.FindChild("role" + item.Key.idToPos() + "/head").GetComponent<Image>());
                    _after.transform.FindChild("role" + item.Key.idToPos() + "/state").GetComponent<Text>().text = "";
                    _after.transform.FindChild("role" + item.Key.idToPos() + "/name").GetComponent<Text>().text = item.Value.Name.ToString();
                }
            }
        }
        else
        {
            Transform obj = _before.transform.FindChild("role" + RoleController.Instance.getPlayerPos(role.id));
            obj.gameObject.SetActive(true);
            SDKMgr.Instance.SetAsyncImage(role.url, obj.FindChild("head").GetComponent<Image>());
            obj.FindChild("head").gameObject.SetActive(true);
            //_before.transform.FindChild("role" + RoleController.Instance.getPlayerPos(role.id) + "/ready").gameObject.SetActive(true);
            obj.FindChild("ready").gameObject.SetActive(true);
            SDKMgr.Instance.SetAsyncImage(role.url, _after.transform.FindChild("role" + role.id.idToPos() + "/head").GetComponent<Image>());
            _after.transform.FindChild("role" + role.id.idToPos() + "/state").GetComponent<Text>().text = "";
            _after.transform.FindChild("role" + role.id.idToPos() + "/name").GetComponent<Text>().text = role.name.ToString();
        }
      
    }
    private void putHeCard(int pos, int card)
    {
        if (_card == null)
            return;
        _hu.gameObject.SetActive(false);
        _peng.gameObject.SetActive(false);
        _gang.gameObject.SetActive(false);
        _gangs.SetActive(false);
        _fun.gameObject.SetActive(false);
        CardProxy proxy = _card.transform.FindChild(pos.ToString()).GetComponent<CardProxy>();

        for (int i = 0; i < proxy.cards.Length; i++)
        {
            if (i < DataMgr.Instance._heCards[pos].Count - 1)
            {
                proxy.cards[i].SetActive(true);
                IconMgr.Instance.SetImage(proxy.cards[i].transform.FindChild("value").GetComponent<Image>(), proxy.headStr + DataMgr.Instance._heCards[pos][i]);
            }
            else if (i == DataMgr.Instance._heCards[pos].Count - 1)
            {
                proxy.cards[i].SetActive(true);
                IconMgr.Instance.SetImage(proxy.cards[i].transform.FindChild("value").GetComponent<Image>(), proxy.headStr + DataMgr.Instance._heCards[pos][i]);
                //设置点;
                _point.SetActive(true);
                _point.transform.position = proxy.cards[i].transform.position;
            }
            else
            {
                proxy.cards[i].SetActive(false);
            }
        }
        if (pos !=0)
        {
            Debug.Log("检测00000");
            bool isPass = true;
            Card card1 = new Card(DataMgr.Instance._curCard);
            //一系列判断
            if (CardController.Instance.checkPeng(card1.CardType, card1.CardNum))
            {
                _fun.SetActive(true);
                _peng.gameObject.SetActive(true);
                isPass = false;
            }
            List<int>[] cards = CardController.Instance.getCopyCard(CardController.Instance._myCardList);
            cards.addCard(card1.CardType, card1.CardNum);
            if (CardController.Instance.checkCard(cards))
            {
                DataMgr.Instance.isHu = true;
                _fun.SetActive(true);
                _hu.gameObject.SetActive(true);
                ProtoReq.canHu();
                isPass = false;
                huCard = card;
            }
            else
            {
                DataMgr.Instance.isHu = false;
               
                _hu.gameObject.SetActive(false);
            }
            if (CardController.Instance.checkMingGang(card1.CardType, card1.CardNum)!=0)
            {
                _fun.SetActive(true);
                _gang.gameObject.SetActive(true);
                gangBack = delegate ()
                {
                    ProtoReq.Gang(CardConst.GetCardBigNum(card1.CardType, card1.CardNum));
                };
                isPass = false;
            }
            if (isPass)
            {
                ProtoReq.Pass(DataMgr.Instance._curCard);
            }
            else
            {
                
            }

            cards = null;
        }
        else
        {
            Debug.Log("牌号:" + card);
            _handCard.SetActive(false);
            _selfCard.GetComponent<handCardProxy>().setPos();
            _handCard.transform.localPosition = new Vector3(_handCard.transform.localPosition.x, _handPosY, 0);
            _upCard = null;
            initCard();
            //beginTimeCount();
        }
    }

    private void onReady(int id)
    {
        _before.transform.FindChild("role" + RoleController.Instance.getPlayerPos(id) + "/ready").gameObject.SetActive(true);
    }
    private void weixinInvite()
    {
        Debug.Log("微信邀请");


#if !UNITY_EDITOR
         string str = "";
        if (DataMgr.Instance.maxbet == 100)
        {
            str += "不设封顶";
        }
        else
        {
            str += "封顶" + DataMgr.Instance.maxbet + "分";
        }

        if (DataMgr.Instance.jiangma == 0)
        {
            str += "、无奖马";
        }
        else
        {
            str += "、奖" + DataMgr.Instance.jiangma + "马";
        }

        if (DataMgr.Instance.maima == 0)
        {
            str += "、无买马";
        }
        else
        {
            str += "买" + DataMgr.Instance.maima + "马";
        }
        SDKMgr.Instance.DoShareTable(GameConst.tableId, DataMgr.Instance.sumRound, str);
#endif

    }
    private void onStart()
    {
        DanjuPanel.Instance.ClosePanel();
        GameConst.isSetDanjuHead = true;
        _state.text = DataMgr.Instance.curRound == 0 ? "等待中" :"第"+DataMgr.Instance.curRound+"局";
    }
    private void getCard()
    {
        DanjuPanel.Instance.ClosePanel();
        initAfter();
        _after.SetActive(true);
        _before.SetActive(false);
        _handCard.SetActive(false);
        _selfCard.SetActive(false);
        _hu.gameObject.SetActive(false);
        _peng.gameObject.SetActive(false);
        _gang.gameObject.SetActive(false);
        _gangs.SetActive(false);
        _pass.gameObject.SetActive(true);
        initFeng();

        initCard();
        _selfCard.SetActive(true);
        //foreach (var item in RoleController.Instance._playerDic)
        //{
        //    Transform obj = _after.transform.FindChild("role" + item.Value.Id.idToPos());
        //    if (obj != null)
        //    {
        //        SDKMgr.Instance.SetAsyncImage(item.Value.Url,obj.FindChild("head").GetComponent<Image>());
        //    }
        //}
    }

    private void initFeng()
    {
        //Debug.LogError("风" + MainRole.Instance.Pos);
        _center.transform.localRotation = Quaternion.Euler(0, 0, -(90 * (MainRole.Instance.Pos-1)));
    }
    private void getCurCard(int roleId,int num)
    {
        if (_after == null)
            return;
        _after.SetActive(true);
        _center.SetActive(true);
        _before.SetActive(false);
        initCard();
        beginTimeCount();
        setOtherHand(roleId.idToPos());
        _hu.gameObject.SetActive(false);
        _peng.gameObject.SetActive(false);
        _gang.gameObject.SetActive(false);
        _gangs.SetActive(false);
        _fun.gameObject.SetActive(false);
        setCenterPos(roleId);
        if (_after != null)
        {
            _after.transform.FindChild("text").GetComponent<Text>().text = "剩余<color='#ffde00'>" + DataMgr.Instance.lassCardsNum + "</color>张  第<color='#ffde00'>" + DataMgr.Instance.curRound + "/" + DataMgr.Instance.sumRound + "</color>局";
        }
       
        if (num!=0)
        {
            IconMgr.Instance.SetImage(_handCard.transform.FindChild("value").GetComponent<Image>(), "zm1_" + num);
            //beginTimeCount();
            GameConst.isTurn = true;
            DataMgr.Instance._curCard = num;
            Card card = new Card(DataMgr.Instance._curCard);
            //一系列判断
            CardController.Instance.addCard(card.CardType, card.CardNum);
            _handCard.SetActive(true);
            List<int>[] cards = CardController.Instance.getCopyCard(CardController.Instance._myCardList);
            if (CardController.Instance.checkCard(cards))
            {
                _fun.SetActive(true);
                _hu.gameObject.SetActive(true);
                ProtoReq.canHu();
            }
            List<int> gangTemp = CardController.Instance.checkAnGang();
            if (CardController.Instance.checkBuGang(num))
                gangTemp.Add(num);
            //多个扛的判断
            if (gangTemp.Count!= 0)
            {
                _fun.SetActive(true);
                if (gangTemp.Count == 1)
                {
                    _gang.gameObject.SetActive(true);
                    gangBack = delegate ()
                    {
                        ProtoReq.Gang(gangTemp[0]);
                    };
                }
                else
                {
                    _gangs.gameObject.SetActive(true);
                    GameObject item = _gangs.transform.FindChild("info/item").gameObject;
                    item.SetActive(false);
                    _gangs.transform.FindChild("info").destoryChild(true);
                    for (int k = 0; k < gangTemp.Count; k++)
                    {
                        GameObject obj = GameObject.Instantiate(item);
                        obj.transform.localScale = Vector3.one;
                        obj.transform.parent = item.transform.parent;
                        
                        obj.name = gangTemp[k].ToString();
                        int cardNum = gangTemp[k];
                        IconMgr.Instance.SetImage(obj.transform.FindChild("value").GetComponent<Image>(), "zm1_" + cardNum);
                        obj.SetActive(true);
                        obj.GetComponent<Button>().onClick.AddListener(delegate
                        {
                            ProtoReq.Gang(cardNum);
                        });
                    }
                }
              
            }
            cards = null;
        }
        else
        {
            
        }
    }

    private void beginTimeCount()
    {
        _count.GetComponent<TimeCount>().time = GameConst.timeCount;
        _count.SetActive(true);
    }

    private void endTimeCount()
    {
        _count.SetActive(false);
        //_fun.SetActive(false);
        //_hu.gameObject.SetActive(false);
        //_peng.gameObject.SetActive(false);
        //_gang.gameObject.SetActive(false);
    }

    private void setMaima()
    {
        _maima.SetActive(true);
        foreach (var item in DanjuController.Instance._maima)
        {
            if (DanjuController.Instance._maima[0][0] == 0)
            {
                _maima.transform.FindChild(item.Key+"/1").gameObject.SetActive(false);
            }
            else
            {
                _maima.transform.FindChild(item.Key+"/1").gameObject.SetActive(true);
                if(item.Value.Count!=0)
                {
                    IconMgr.Instance.SetImage(_maima.transform.FindChild(item.Key + "/1/value").GetComponent<Image>(), getMaimaImageStr(item.Key) + item.Value[0]);
                }
                
            }

            if (DanjuController.Instance._maima[0][1] == 0)
            {
                _maima.transform.FindChild(item.Key+"/2").gameObject.SetActive(false);
            }
            else
            {
                _maima.transform.FindChild(item.Key+"/2").gameObject.SetActive(true);
                if (item.Value.Count != 0)
                {
                    IconMgr.Instance.SetImage(_maima.transform.FindChild(item.Key + "/2/value").GetComponent<Image>(), getMaimaImageStr(item.Key) + item.Value[1]);
                }
                    
            }
        }
        
    }

    private string getMaimaImageStr(int pos)
    {
        string str = "xia_1_";
        switch (pos)
        {
            case 0:
                str = "xia_1_";
                break;
            case 1:
                str = "you1_";
                break;
            case 2:
                str = "shang1_";
                break;
            case 3:
                str = "zuo1_";
                break;
        }
        return str;
    }

    private void setCenterPos(int id)
    {
        for (int i = 0; i < 4; i++)
        {
            if (i == RoleController.Instance._playerDic[id].Pos)
            {
                _center.transform.FindChild(i.ToString()).gameObject.SetActive(true);
            }
            else
            {
                _center.transform.FindChild(i.ToString()).gameObject.SetActive(false);
            }
        }

    }

    private int huCard = 0;//记录别人打出的能胡的牌

    private void onTalkDown(GameObject obj,BaseEventData even)
    {

        if (PluginTool.Instance.isPlaying)
        {
            QuickTips.ShowQuickTipsNor("有人在说话，稍后再说");
            return;
        }
        if (!_isTalkCD)
        {
            //talkTime = Time.time;

            _isTalk = true;
            new Timer(delegate
            {
                onTalkUp(null, null);
            }, this, GameConst.talkLength, 0);

           // talkTimer = GlobleTimer.Instance.SetTimer((uint)GameConst.talkLength, 1, delegate
            //{
            //    onTalkUp(null, null);
           // });

            _talkShow.SetActive(true);
            SoundMgr._instance.bgmSetVolume(0);
            PluginTool.Instance.stopTalk = delegate (byte[] b)
            {
                ProtoReq.VoiceChat(b);
            };
            PluginTool.Instance.StartRecord(URL.localCachePath);
        }
        else
        {
            QuickTips.ShowQuickTipsNor("说话时间间隔太短，稍后再说");
        }
    }

    private void onTalkUp(GameObject obj, BaseEventData even)
    {
        
        if (_isTalk)
        {
            _isTalk = false;
           // SoundMgr._instance.bgmSetVolume(GameConst.musicVol);
           // _talkShow.SetActive(false);
           // if (Time.time - talkTime < 1)
           // {
           //     _isTalkCD = false;
          //      GlobleTimer.Instance.ClearTimer(talkTimer);
           //     return;
          //  }
            _isTalkCD = true;
            new Timer(delegate
            {
                _isTalkCD = false;
            }, this, GameConst.talkCD, 0);
           _talkShow.SetActive(false);
            SoundMgr._instance.bgmSetVolume(GameConst.musicVol);
            PluginTool.Instance.StopRecord();
        }
    }

    private void setOtherHand(int num)
    {
        switch (num)
        {
            case 0:
                if(_rightHand!=null)
                    _rightHand.SetActive(false);
                if(_leftHand!=null)
                    _leftHand.SetActive(false);
                if(_topHand!=null)
                    _topHand.SetActive(false);
                break;
            case 1:
                if (_rightHand != null)
                    _rightHand.SetActive(true);
                if (_leftHand != null)
                    _leftHand.SetActive(false);
                if (_topHand != null)
                    _topHand.SetActive(false);
                break;
            case 2:
                if (_rightHand != null)
                    _rightHand.SetActive(false);
                if (_leftHand != null)
                    _leftHand.SetActive(false);
                if (_topHand != null)
                    _topHand.SetActive(true);
                break;
            case 3:
                if (_rightHand != null)
                    _rightHand.SetActive(false);
                if (_leftHand != null)
                    _leftHand.SetActive(true);
                if (_topHand != null)
                    _topHand.SetActive(false);
                break;
        }
    }
}
