using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class ChongzhiPanel : BasePanel
{

    private static ChongzhiPanel instance;
    public static ChongzhiPanel Instance
    {
        get
        {
            if (instance == null)
                instance = new ChongzhiPanel();
            return instance;
        }
    }

    private ChongzhiPanel()
    {
        base_name = PanelTag.CHONGZHI_PANEL;
        _isPop = true;
    }

    private Button _close;
    private Button _confirm1;
    private Button _confirm2;
    private Button _confirm3;

    private float _curTime;

    //用来存放商品列表
    public List<string> productInfo = new List<string>();
    private int buyType = 0;
    public override void InitPanel(Transform uiSprite)
    {
        base.InitPanel(uiSprite);
        PluginTool.Instance.initIAP();
        if (!PluginTool.Instance.isProductAvailable()) throw new System.Exception("IAP not enabled");
        productInfo = new List<string>();
        PluginTool.Instance.reqProductInfo("money1|money2|money3");
        _close = uiSprite.FindChild("close").GetComponent<Button>();

        _close.onClick.AddListener(delegate
        {
            ClosePanel();
        });

        _confirm1 = uiSprite.FindChild("info/confirm1").GetComponent<Button>();
        _confirm1.onClick.AddListener(delegate
        {
            if (_curTime==0||Time.time - _curTime > 5)
            {
                _curTime = Time.time;
                PluginTool.Instance.buyProduct("money1");
                buyType = 1;
            }
           
        });

        _confirm2 = uiSprite.FindChild("info/confirm2").GetComponent<Button>();
        _confirm2.onClick.AddListener(delegate
        {

            if (_curTime == 0||Time.time - _curTime > 5)
            {
                _curTime = Time.time;
                PluginTool.Instance.buyProduct("money2");
                buyType = 2;
            }
        });

        _confirm3 = uiSprite.FindChild("info/confirm3").GetComponent<Button>();
        _confirm3.onClick.AddListener(delegate
        {
            if (_curTime == 0||Time.time - _curTime > 5)
            {
                _curTime = Time.time;
                PluginTool.Instance.buyProduct("money3");
                buyType = 3;
            }
        });
    }

    public override void startUp(object obj = null)
    {
        base.startUp(obj);
        _curTime = 0;
        buyType = 0;
        
    }

    public override void AddListener()
    {
        base.AddListener();
        EventDispatcher.Instance.AddEventListener<string>(GameEventConst.BUY_OVER, buyOver);
    }
    public override void RemoveListener()
    {
        base.RemoveListener();
        EventDispatcher.Instance.RemoveEventListener<string>(GameEventConst.BUY_OVER, buyOver);
    }

    private void buyOver(string result)
    {
        if (result.Equals("1"))
        {
            ProtoReq.purchaseReq(buyType);
        }
    }
}
