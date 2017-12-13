using UnityEngine;
using System.Collections;

public class TalkShow : MonoBehaviour {

    public GameObject[] objs;
    public float time = 2f;
    private float curTime = 0;
	// Use this for initialization
	void Start () {
        for (int i = 0; i < objs.Length; i++)
        {
            objs[i].SetActive(false);
        }
        curTime = 0;

    }
	
	// Update is called once per frame
	void Update () {
        if (curTime <= 2)
        {
            curTime=curTime+Time.deltaTime;
            float mul = time / objs.Length;
            int num = (int)(curTime / mul);
            for (int i = 0; i < objs.Length; i++)
            {
                if (i <= num)
                {
                    objs[i].SetActive(true);
                }
                else
                    objs[i].SetActive(false);
            }
           

        }
        else
        {
            curTime = 0;
        }
	}
}
