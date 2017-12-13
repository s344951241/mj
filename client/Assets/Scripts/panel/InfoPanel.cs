using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InfoPanel : BasePanel
{
    private static InfoPanel instance;
    public static InfoPanel Instance
    {
        get {
            if (instance == null)
                instance = new InfoPanel();
            return instance;
        }
    }

    private InfoPanel()
    {
        base_name = PanelTag.INFO_PANEL;
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
