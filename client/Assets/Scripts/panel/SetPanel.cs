using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SetPanel : BasePanel{

    private static SetPanel instance;
    public static SetPanel Instance
    {
        get {
            if (instance == null)
                instance = new SetPanel();
            return instance;
        }
    }

    private SetPanel()
    {
        base_name = PanelTag.SET_PANEL;
        _isPop = true;
    }
    private GameObject _type1;
    private GameObject _type2;
    private Button _close;
    private Toggle _puTong;
    private Toggle _chaoShan;

    private Slider _musicSlider;
    private Slider _soundSlider;

    private Toggle _music;
    private Toggle _sound;



    public override void InitPanel(Transform uiSprite)
    {
        base.InitPanel(uiSprite);
        _type1 = uiSprite.FindChild("type1").gameObject;
        _type2 = uiSprite.FindChild("type2").gameObject;
        _close = uiSprite.FindChild("close").GetComponent<Button>();
        _puTong = uiSprite.FindChild("language/puTong").GetComponent<Toggle>();
        _chaoShan = uiSprite.FindChild("language/chaoShan").GetComponent<Toggle>();

        _music = uiSprite.FindChild("music").GetComponent<Toggle>();
        _sound = uiSprite.FindChild("sound").GetComponent<Toggle>();
        _musicSlider = uiSprite.FindChild("musicSlider").GetComponent<Slider>();
        _soundSlider = uiSprite.FindChild("soundSlider").GetComponent<Slider>();

        addClick();
    }

    public override void startUp(object obj = null)
    {
        base.startUp(obj);
        if ((int)obj == 1)
        {
            _type1.SetActive(true);
            _type2.SetActive(false);
        }
        else
        {
            _type1.SetActive(false);
            _type2.SetActive(true);
        }
        if (GameConst.Language.Equals(""))
        {
            _puTong.isOn = true;
            _chaoShan.isOn = false;
        }
        else
        {
            _puTong.isOn = false;
            _chaoShan.isOn = true;
        }

        _musicSlider.value = GameConst.musicVol;
        if (GameConst.musicVol == 0)
        {
            _music.isOn = false;
        }
        else
        {
            _music.isOn = true;
        }

        _soundSlider.value = GameConst.soundVol;
        if (GameConst.soundVol == 0)
        {
            _sound.isOn = false;
        }
        else
        {
            _sound.isOn = true;
        }
    }

    private void addClick()
    {
        _close.onClick.AddListener(delegate
        {
            ClosePanel();
        });

        _puTong.onValueChanged.AddListener(delegate
        {
            if (_puTong.isOn)
            {
                PlayerPrefs.SetString("language", "");
                GameConst.Language = "";
            }
           
        });
        _chaoShan.onValueChanged.AddListener(delegate
        {
            if (_chaoShan.isOn)
            {
                PlayerPrefs.SetString("language", "s");
                GameConst.Language = "s";
            }
            
        });

        _type1.transform.FindChild("change").GetComponent<Button>().onClick.AddListener(delegate
        {
            if (string.IsNullOrEmpty(PlayerPrefs.GetString("BGM")))
            {
                PlayerPrefs.SetString("BGM", "1");
                GameConst.BGM = "1";
            }
            else
            {
                PlayerPrefs.SetString("BGM", "");
                GameConst.BGM = "";
            }
            SoundMgr._instance.switchBGM();

        });
        _type1.transform.FindChild("quit").GetComponent<Button>().onClick.AddListener(delegate
        {
            //退出
            PlayerPrefs.DeleteKey("openId");
            LoginPanel.Instance.load();
            //DestoryPanel();
            ClosePanel();
        });


        _type2.transform.FindChild("change").GetComponent<Button>().onClick.AddListener(delegate
        {
            if (string.IsNullOrEmpty(PlayerPrefs.GetString("BGM")))
            {
                PlayerPrefs.SetString("BGM", "1");
                GameConst.BGM = "1";
            }
            else
            {
                PlayerPrefs.SetString("BGM", "");
                GameConst.BGM = "";
            }
            // SoundMgr._instance.bgmPlay("beijing-fangjian" + GameConst.BGM, GameConst.musicVol);
            SoundMgr._instance.switchBGM();
        });

        _music.onValueChanged.AddListener(delegate
        {
            if (!_music.isOn)
            {
                GameConst.musicVol = 0;
                PlayerPrefs.SetFloat("MUSIC", 0);
                _musicSlider.value = 0;
                SoundMgr._instance.bgmSetVolume(0);
            }
        });
        _musicSlider.onValueChanged.AddListener(delegate
        {
            GameConst.musicVol = _musicSlider.value;
            PlayerPrefs.SetFloat("MUSIC", GameConst.musicVol);
            SoundMgr._instance.bgmSetVolume(GameConst.musicVol);

        });

        _sound.onValueChanged.AddListener(delegate
        {
            if (!_sound.isOn)
            {
                GameConst.soundVol = 0;
                PlayerPrefs.SetFloat("SOUND", 0);
                _soundSlider.value = 0;
            }
        });
        _soundSlider.onValueChanged.AddListener(delegate
        {
            GameConst.soundVol = _soundSlider.value;
            PlayerPrefs.SetFloat("SOUND", GameConst.soundVol);
        });

    }
}
