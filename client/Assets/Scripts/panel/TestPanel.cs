using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TestPanel : BasePanel {

    private static TestPanel instance;

    private Button _play;
    private Button _sound;
    private Button _testAndroid;
    public static TestPanel Instance {
        get {
            if (instance == null)
                instance = new TestPanel();
            return instance;
        }
        
    }

    private TestPanel()
    {
        base_name = PanelTag.PANEL_TEST;
    }

    public override void InitPanel(Transform uiSprite)
    {
        base.InitPanel(uiSprite);
        _play = uiSprite.FindChild("Button").GetComponent<Button>();
        _sound = uiSprite.FindChild("Button2").GetComponent<Button>();
        _testAndroid = uiSprite.FindChild("android").GetComponent<Button>();
        _play.onClick.AddListener(delegate {
            SoundMgr._instance.bgmPlay("3");
        });
        _sound.onClick.AddListener(delegate {
            SoundMgr._instance.soundPlay("playbutton");
        });
        _testAndroid.onClick.AddListener(delegate {
            //PluginTool.Instance.testFun();
        });
    }
    private void test(string str)
    {
        _testAndroid.transform.FindChild("Text").GetComponent<Text>().text = str;
    }
}
