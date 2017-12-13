using UnityEngine;
using System.Collections;

public class TimeCount : MonoBehaviour {

    [SerializeField]
    private GameObject[] objCount;
    public float time;

    void Start()
    {
        time = GameConst.timeCount;
    }

    void Update()
    {
        time = time - Time.deltaTime;
        if (time < 0)
        {
            gameObject.SetActive(false);
            time = 15;
            EventDispatcher.Instance.Dispatch(GameEventConst.TIME_COUNT_END);
        }
            
        for (int i = objCount.Length-1; i >= 0; i--)
        {
            if (i == Mathf.FloorToInt(time))
            {
                objCount[i].SetActive(true);
            }
            else
                objCount[i].SetActive(false);
        }
    }

}
