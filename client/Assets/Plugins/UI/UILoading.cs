using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UILoading : SingletonMonoBehaviour<UILoading> {

    private static Slider _slider;
    private static Image _bg;

    private static Text _subTitle;
    private static Text _percentText;
    private static GameObject _gameObject;

    void Awake()
    {
        _gameObject = gameObject;
        _slider = transform.FindChild("Slider").GetComponent<Slider>();
        _bg = transform.FindChild("bg").GetComponent<Image>();
        _subTitle = transform.FindChild("subTitle").GetComponent<Text>();
        _percentText = transform.FindChild("percentText").GetComponent<Text>();
        GameObject.DontDestroyOnLoad(this);
    }

    public static string subTitle
    {
        set {
            _subTitle.text = value;
        }
    }
    public static float percent
    {
        set {
            var p = Mathf.Clamp(value,0f, 1f);
            _slider.value = p;
            _percentText.text = (p * 100).ToString("0.00") + "%";
        }
    }

    public static void SetSubTitle(string title, float kPercent)
    {
        subTitle = title;
        percent = kPercent;

    }

    public static void ShowLoading(string kSubTitle = "", float kPercent = 0)
    {
        if (!string.IsNullOrEmpty(kSubTitle))
            subTitle = kSubTitle;
        percent = kPercent;
        _gameObject.SetActive(true);
    }

    public static void CloseLoading()
    {
        _gameObject.SetActive(false);
    }
}
