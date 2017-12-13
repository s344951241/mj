using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Threading;

public class IdTest : MonoBehaviour {

    public GameObject _obj;
	// Use this for initialization
	void Start () {
        //int num = 111222333;
        //int result = num ^ 100100;
        //Debug.LogError(num);
        //Debug.LogError(result);
        //Debug.LogError(result ^ 100100);

        //UIEventHandlerBase.AddListener(_obj, UIEventType.ON_DRAG, delegate (GameObject obj, BaseEventData evn)
        //{
        //    float startPos = ((PointerEventData)evn).pressPosition.y;
        //    float endPos = ((PointerEventData)evn).position.y;
        //    if (endPos - startPos >= 50)
        //    {
        //        Debug.LogError("拖拽");
        //    }
        //});

        //string str = "g111111111111111fm";
        //string strAfter = str.Substring(0, str.Length - 1) + "d";
        //Debug.LogError(strAfter);
        //TimeManager.
        Timer time = null;
        time = new Timer(delegate
        {
            Debug.LogError("11111");
            time.Dispose();
        }, this, 1000, 500);
    }
	
	// Update is called once per frame
	void Update () {
        //Debug.LogError(Time.time);
	}
}
