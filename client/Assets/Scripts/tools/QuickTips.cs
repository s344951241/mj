using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class QuickTips
{
    private static GameObject _quickTipsLayers;
    private static IzObjectPool<TipsElement> _tipsPool;
    private static float _fOriginPosY;
    private static float _fOriginPosX;
    private static float _fTargetPosY;
    private static float _fTargetPosX;

    private static GameObject _grid;
    private static GameObject _template;
    private static int _lineHeight = 40;
    private static TipsElement[] _elements = new TipsElement[3];
    private static float _speed = 0;
    private static float _speedTime = 0;
    private static float _comeTime = 0;
    private static float _moveDistance = 0;
    private static float _haveMove = 0;
    private static Vector3[] _vect3 = new Vector3[2];
    private static readonly float _fMoveUp = 60.0f;
    private static Queue<IzPoolElement> _kNewTipsQueue = new Queue<IzPoolElement>();
    private static Queue<IzPoolElement> _kTipsQueue = new Queue<IzPoolElement>();

    private static float _fTimeSinceLastPop = 0.0f;
    private static readonly float _fDefautDelayTime = 0.25f;
    private static readonly float _fDefautPopNextDelay = 0.15f;
    private static readonly float _fDefautFlyTime = 4.0f;
    private static float _fDelayTime;
    private static float _fPopNextDelay;
    private static float _fFlyTime = 4.0f;
    private static int TIP_LIMIT = 10;
    private static float SpeedRate = 200.0f;
    private static Updater _kUpdater;
    private static bool _isShow = true;

    private static GameObject _iconTemplate;

    private static bool _isMoving = true;
    private static bool _startMoving = false;
    private static Vector3 _endPos;
    private static float _speedX = 0.0f;
    private static float _speedY = 0.0f;
    private static float _speedScale = 0.0f;
    private static Vector3 _nextShowPos;

    public static void InitQuickTipsLayer(GameObject layer,int poolSize)
    {
        _fDelayTime = _fDefautDelayTime;
        _fPopNextDelay = _fDefautPopNextDelay;
        _fFlyTime = _fDefautFlyTime;

        _quickTipsLayers = layer;
        _kUpdater = _quickTipsLayers.AddComponent<Updater>();
        _template = _quickTipsLayers.transform.GetChild(0).gameObject;
        RectTransform rect = _template.GetComponent<RectTransform>();
        _fOriginPosY = rect.anchoredPosition.y;
        _fOriginPosX = rect.anchoredPosition.x;

        _grid = _quickTipsLayers.transform.FindChild("TipList/Grid").gameObject;
        _grid.SetActive(true);
        TIP_LIMIT = poolSize-1;
        _template.SetActive(false);
        _fTargetPosY = Screen.width /2.0f-(-_fOriginPosY-_fMoveUp)+10;
        _fTargetPosX = _fOriginPosX;
        _kUpdater.onUpdate+=PopTips;
    }

    public static void ShowRedQuickTips(string message)
    {   
        ShowQuickTips(message,Color.red);
    }

    public static void ShowGreenQuickTips(string message)
    {
        ShowQuickTips(message,Color.green);
    }
    public static void ShowBlueQuickTips(string message)
    {   
        ShowQuickTips(message,Color.blue);
    }
    public static void ShowQuickTipsNor(string message)
    {
        _quickTipsLayers.SetActive(false);
        _quickTipsLayers.SetActive(true);
        ShowQuickTips(message, new Color(0.75f, 0.776f, 0.776f));
    }

    public static void ShowQuickTips(string message,Color color)
    {
        if(_kNewTipsQueue.Count<10&&!string.IsNullOrEmpty(message))
        {
            TipsElement element = new TipsElement();
            element.InitData(_template);
            element.TextContent.text = "请选择要";
            int minHeight = (int)element.TextContent.preferredHeight;
            element.TextContent.text = message;
            element.TextContent.color = color;
            float txtWidth = element.TextContent.preferredWidth;
            int lines = (int)element.TextContent.preferredHeight/minHeight;
            if(txtWidth>580)
            {
                txtWidth = 600;
            }
            else
            {
                txtWidth +=20;
            }
            element._lifeTime = 2.5f;
            element._txtLines = lines;

            element.ImgBackGround.GetComponent<RectTransform>().sizeDelta = new Vector2(txtWidth,_lineHeight*lines);
            element.TextContent.GetComponent<RectTransform>().sizeDelta = new Vector2(txtWidth-18,_lineHeight*lines);
            element.Alpha = 0.0f;
            element.TextContent.enabled = true;
            _kNewTipsQueue.Enqueue(element);
        }
        else
        {
            return;
        }
    }

    public static void ShowQuickTipsWithOutColor(string message)
    {
        if(_kNewTipsQueue.Count<10)
        {
            TipsElement element = new TipsElement();
            element.InitData(_template);
            element.TextContent.text = "请选择要";
            int minHeight = (int)element.TextContent.preferredHeight;
            element.TextContent.text = message;
            element.TextContent.enabled = true;
            float txtWidth = element.TextContent.preferredWidth;
            int lines = (int)element.TextContent.preferredHeight/minHeight;
            if(txtWidth>580)
            {
                txtWidth = 600;
            }
            else
            {
                txtWidth +=20;
            }
            element._lifeTime = 2.5f;
            element._txtLines = lines;

            element.ImgBackGround.GetComponent<RectTransform>().sizeDelta = new Vector2(txtWidth,_lineHeight*lines);
            element.TextContent.GetComponent<RectTransform>().sizeDelta = new Vector2(txtWidth-18,_lineHeight*lines);
            element.Alpha = 0.0f;
            element.TextContent.enabled = true;
            _kNewTipsQueue.Enqueue(element);
        }
         else
        {
            return;
        }
    }

    public static void ShowQuickTipsByPos(string message,Color color,Vector3 vect31,Vector3 vect32)
    {
        //

    }

    private static void PopTips(float dt)
    {
        _fTimeSinceLastPop+=dt;
        if(_kTipsQueue.Count>0)
        {
            float factor = 1;
            _fDelayTime = _fDefautDelayTime*factor;
            _fPopNextDelay = _fDefautPopNextDelay*factor;
            _fFlyTime = _fDefautFlyTime*factor;
        }
        for(int i=0;i<3;i++)
        {
            if(_elements[i]!=null)
            {

                _elements[i]._lifeTime  -=0.03f;
                if(_elements[i]._lifeTime<=0)
                {
                    GameObject.DestroyImmediate(_elements[i].GetTarget());
                    _elements[i] = null;
                }
            }
        }
        if(_kNewTipsQueue.Count>0)
        {

            if(_grid.transform.childCount>0)
            {
                if(_speedTime>=1.0f)
                {

                    for(int k=0;k<3;k++)
                    {
                        if(_elements[k]==null)
                        {

                            _elements[k] = _kNewTipsQueue.Dequeue() as TipsElement;
                            _elements[k].GetTarget().transform.parent = _grid.transform;
                            _elements[k].SetAlpha(1.0f);
                            _elements[k].GetTarget().transform.GetChild(0).gameObject.SetActive(true);
                            _elements[k].GetTarget().SetActive(true);
                            if(_elements[k]._txtLines>1)
                            {

                                RectTransform tran = _elements[k].GetTargetTrans();
                                tran.localPosition = new Vector3(tran.localPosition.x,tran.localPosition.y-(_elements[k]._txtLines-1)*_lineHeight/2,tran.localPosition.z);

                            }
                            else
                            {
                                _elements[k].GetTargetTrans().localPosition  = new Vector3(0,0,0);
                            }
                            _speed = _elements[k]._txtLines*_lineHeight*0.03f;
                            _elements[k]._moveDistance = _elements[k]._txtLines*_lineHeight;
                            _moveDistance = _elements[k]._txtLines*_lineHeight;
                            _speedTime = 0;
                            break;
                        }
                    }
                }
            }
            else
            {
                _elements[0] = _kNewTipsQueue.Dequeue() as TipsElement;
                //_elements[0].GetTarget().transform.parent = _grid.transform;
                _elements[0].GetTarget().transform.SetParent(_grid.transform);
                _elements[0].SetAlpha(1.0f);
                _elements[0].GetTarget().transform.GetChild(0).gameObject.SetActive(true);
                _elements[0].GetTarget().SetActive(true);

                if(_elements[0]._txtLines>1)
                {
                    RectTransform tran = _elements[0].GetTargetTrans();
                    tran.localPosition = new Vector3(tran.localPosition.x,tran.localPosition.y-(_elements[0]._txtLines-1)*_lineHeight/2,tran.localPosition.z);
                }
                else
                {
                     _elements[0].GetTargetTrans().localPosition  = new Vector3(0,0,0);
                }
                _speed = _elements[0]._txtLines*_lineHeight*0.03f;
                _elements[0]._moveDistance = _elements[0]._txtLines*_lineHeight;
                _moveDistance = _elements[0]._txtLines*_lineHeight;
                _speedTime = 0;

            }
        }
        if(_speedTime<1.0f&&_grid.transform.childCount>0)
        {
            _speedTime+=dt;
            for(int j=0;j<_grid.transform.childCount;j++)
            {
                Transform child = _grid.transform.GetChild(j);
                child.localPosition = new Vector3(child.localPosition.x,child.localPosition.y+_speed,child.localPosition.z);
            }
            _haveMove += _speed;
            if(_haveMove>=_moveDistance||_speedTime>=1.0f)
            {
                if(_haveMove<_moveDistance)
                {
                    for(int j=0;j<_grid.transform.childCount;j++)
                    {
                        Transform child = _grid.transform.GetChild(j);
                        child.localPosition = new Vector3(child.localPosition.x,child.localPosition.y+(_moveDistance-_haveMove),child.localPosition.z);
                    }
                }
                _haveMove = 0;
                _speedTime = 1.0f;
            }
        
        }


        if(_fTimeSinceLastPop>(_fDelayTime+_fPopNextDelay)&&_kNewTipsQueue.Count>0&&!_isShow)
        {
            _isShow = true;
            _fTimeSinceLastPop = 0.0f;
            TipsElementInterface element = _kNewTipsQueue.Dequeue() as TipsElementInterface;
            element.GetTargetTrans().SetAsLastSibling();
            float elementLastTime = 0.0f;
            float alphaChangeRate = 1.0f/_fFlyTime;

            Updater.ON_UPDATE elementUpdate = delegate(float delta)
            {
                elementLastTime+=delta;
            };

            TweenCallback onComplete = delegate()
            {
                element.GetTarget().SetActive(false);
                element.GetTargetTrans().anchoredPosition = _vect3[0];
                _kUpdater.onUpdate-=elementUpdate;
                if(element as TipsElement!=null)
                    _tipsPool.ReturnObject(element as TipsElement);
                _isShow = false;
            };

            GameConst.driver.StartCoroutine(func(delegate ()
            {
                TipsElement tempElement = element as TipsElement;
                element.SetAlpha(1.0f);
                element.GetTarget().transform.GetChild(0).gameObject.SetActive(true);
                element.GetTarget().SetActive(true);
                Tween tween = element.GetTargetTrans().DOLocalMove(tempElement.EndPos, _fFlyTime);
                tween.SetUpdate(true);
                tween.OnComplete(onComplete);
                _kUpdater.onUpdate += elementUpdate;
            },_fDelayTime/(float)1000));
        }
    }

    private static IEnumerator func(Action action,float time)
    {

        yield return new WaitForSeconds(time);
        action.Invoke();
    }
}

interface TipsElementInterface
{
    GameObject GetTarget();
    RectTransform GetTargetTrans();
    void SetAlpha(float value);
}

[Serializable]
class TipsElement:IzPoolElement,TipsElementInterface
{
    private GameObject _kTarget;
    private Text _txtTextContent;
    private RectTransform _kTargetTrans;
    private Image _imgBackGround;
    public Vector2 _beginPos;
    public Vector2 _endPos;
    public int _txtLines;
    public float _lifeTime = 0;
    public float _moveDistance = 0;
    public long id = 0;

    public void InitData(object param=null)
    {
        GameObject template = param as GameObject;
        _kTarget = GameObject.Instantiate(template) as GameObject;
        _kTarget.name = UtilsExtends.GetTimeStamp().ToString();
        id = UtilsExtends.GetTimeStamp();
        _kTargetTrans = _kTarget.GetComponent<RectTransform>();
        _kTarget.name = "TipsText";
        _kTargetTrans.SetParent(template.GetComponent<RectTransform>().parent,false);
        _txtTextContent = _kTargetTrans.GetChild(0).gameObject.GetComponent<Text>();
        _imgBackGround = _kTarget.GetComponent<Image>();
        _txtTextContent.text = "";
        _kTarget.SetActive(false);
    }

    public void InitForUse(object param = null)
    {
        _beginPos = ((Vector3[])param)[0];
        _endPos = ((Vector3[])param)[1];
        _kTargetTrans.anchoredPosition = _beginPos;
        _kTargetTrans.gameObject.SetActive(true);
    }
    public void Clear()
    {

        _txtTextContent.text = "";
        _kTarget.SetActive(false);
    }

    public void Recycle()
    {

    }

    public void Destroy()
    {

    }

    public Text TextContent
    {

        get{return _txtTextContent;}
    }
    public Image ImgBackGround
    {
        get{return _imgBackGround;}
    }

    public Vector3 BeginPos{
        get{return _beginPos;}
    }

    public Vector3 EndPos{
        get{return _endPos;}
    }
    public float Alpha{
        set{
            Color bg = _imgBackGround.color;
            Color txt = _txtTextContent.color;
            float alpha = Mathf.Clamp01(value);
            _imgBackGround.color = new Color(bg.r,bg.g,bg.b,alpha);
            _txtTextContent.color = new Color(txt.r,txt.g,txt.b,alpha);
        }
    }

    public GameObject GetTarget()
    {
        return _kTarget;
    }

    public RectTransform GetTargetTrans()
    {

        return _kTargetTrans;
    }
    public void SetAlpha(float value)
    {
        Alpha = value;
    }

}


