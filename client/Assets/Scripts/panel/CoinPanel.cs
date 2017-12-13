using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CoinPanel : BasePanel {

    private static CoinPanel instance;
    public static CoinPanel Instance
    {
        get
        {
            if (instance == null)
                instance = new CoinPanel();
            return instance;
        }
    }

    private CoinPanel()
    {
        base_name = PanelTag.COIN_PANEL;
        _isPop = true;
    }

    private Button _close;

    public override void InitPanel(Transform uiSprite)
    {
        base.InitPanel(uiSprite);
        _close = uiSprite.FindChild("close").GetComponent<Button>();

        _close.onClick.AddListener(delegate
        {
            ClosePanel();
        });
    }
}
