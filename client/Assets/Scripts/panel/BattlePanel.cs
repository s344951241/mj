using UnityEngine;
using System.Collections;

public class BattlePanel : BasePanel{

    private static BattlePanel instance;
    public static BattlePanel Instance
    {
        get
        {
            if (instance == null)
                instance = new BattlePanel();
            return instance;
        }

    }
    private BattlePanel()
    {
        base_name = PanelTag.BATTLE_PANEL;
        _isPop = false;
    }

    public override void InitPanel(Transform uiSprite)
    {
        base.InitPanel(uiSprite);
    }
}
