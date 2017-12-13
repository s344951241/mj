using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Threading;

public class AlertMgr : Singleton<AlertMgr> {

    public AlertMgr()
    {

    }
    public void Init(GameObject obj)
    {
        _alert = obj;
        _type1 = _alert.transform.FindChild("type1").gameObject;
        _type2 = _alert.transform.FindChild("type2").gameObject;
        _type3 = _alert.transform.FindChild("type3").gameObject;
        _info = _alert.transform.FindChild("info").GetComponent<Text>();
    }

    public GameObject _alert;
    public delegate void ALERT_FUN();
    private GameObject _type1;
    private GameObject _type2;
    private GameObject _type3;
    private Text _info;

    public void showAlert(ALERT_TYPE type,string info, ALERT_FUN fun1 = null, ALERT_FUN fun2 = null,int count = 0)
    {
        _info.text = info;
        GameObject obj = null;
        switch ((int)type)
        {
            case 1:
                _type1.SetActive(true);
                obj = _type1;
                _type2.SetActive(false);
                _type3.SetActive(false);
                break;
            case 2:
                _type1.SetActive(false);
                _type2.SetActive(true);
                obj = _type2;
                _type3.SetActive(false);
                break;
            case 3:
                _type1.SetActive(false);
                _type2.SetActive(false);
                _type3.SetActive(true);
                obj = _type3;
                break;
        }
        _alert.SetActive(true);
        if (obj != null)
        {
            if (obj.transform.GetChild(0).GetComponent<Button>()!=null)
            {
                obj.transform.GetChild(0).GetComponent<Button>().onClick.RemoveAllListeners();
                obj.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate
                {
                    if (fun1 != null)
                    {
                        fun1.Invoke();
                       
                    }
                    close();
                });
            }
            if(obj.transform.GetChild(1).GetComponent<Button>()!=null)
            {
                obj.transform.GetChild(1).GetComponent<Button>().onClick.RemoveAllListeners();
                obj.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate
                {
                    if (fun2 != null)
                    {
                        fun2.Invoke();
                       
                    }
                    close();
                });
            }
        }

        if (count != 0)
        {
            Text countText = obj.transform.GetChild(0).transform.FindChild("count").GetComponent<Text>();
            countText.text = count + "s";
            Timer timerCount = null;
            timerCount = new Timer(delegate
            {
                Loom.QueueOnMainThread(() => {
                    count -= 1;
                    countText.text = count + "s";
                    if (count == 0)
                    {
                        close();
                        timerCount.Dispose();
                    }
                });
               
            }, this, 1000, 1000);
        }
        else
        {
            Text countText = obj.transform.GetChild(0).transform.FindChild("count").GetComponent<Text>();
            countText.text = "";
        }
    }

    public void close()
    {
        _alert.SetActive(false);
    }
}

public enum ALERT_TYPE
{
    /// <summary>
    /// 确定,取消
    /// </summary>
    type1 = 1,
    /// <summary>
    /// 确定
    /// </summary>
    type2 = 2,
    /// <summary>
    /// 跟新
    /// </summary>
    type3 = 3,
}
