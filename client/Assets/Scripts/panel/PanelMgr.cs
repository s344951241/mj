using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PanelMgr : Singleton<PanelMgr> {

    public Dictionary<string, BasePanel> _panelDic;

    public PanelMgr()
    {
        _panelDic = new Dictionary<string, BasePanel>();
    }
}
