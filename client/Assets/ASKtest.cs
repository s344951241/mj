using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Threading;

public class ASKtest : MonoBehaviour {


	// Use this for initialization
	void Start () {
        //transform.GetComponent<Button>().onClick.AddListener(delegate
        //{
        //    byte [] b = new byte[9000];
        //    for (int i = 0; i < 9000; i++)
        //    {
        //        b[i] = (byte)i;
        //    }
        //    ProtoReq.VoiceChat(b);
        //});
        new Timer(delegate
        {
            Debug.LogError(11111);
        }, this, 5000, 0);
        new Timer(delegate
        {
            Debug.LogError(22222);
        }, this, 6000, 0);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
