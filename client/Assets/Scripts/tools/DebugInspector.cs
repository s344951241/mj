using UnityEngine;
using System.Collections;

#if UNITY_EDITOR

using UnityEditor;

public class DebugInspector : MonoBehaviour {
    [HideInInspector]
    public int cxID;
    [HideInInspector]
    public int coinID;
    [HideInInspector]
    public int coinAdd;
    [HideInInspector]
    public string info;
    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    private void gmRes(string value)
    {
        info = value;
    }
}
[CustomEditor(typeof(DebugInspector))]
public class DebugInspectorFor : Editor {

    private DebugInspector debug;

    public override void OnInspectorGUI()
    {
        debug = (DebugInspector)target;

        base.OnInspectorGUI();
        GUILayout.BeginHorizontal();
        GUILayout.Label("查询ID", GUILayout.Width(80));
        debug.cxID = int.Parse(GUILayout.TextField(debug.cxID.ToString()));
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("查询"))
        {
            //debug.info = "查询";
            ProtoReq.findUser(debug.cxID);
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(20);

        GUILayout.BeginHorizontal();
        GUILayout.Label("ID", GUILayout.Width(40));
        debug.coinID = int.Parse(GUILayout.TextField(debug.coinID.ToString(), GUILayout.Width(100)));
        GUILayout.Label("金币数：", GUILayout.Width(40));
        debug.coinAdd = int.Parse(GUILayout.TextField(debug.coinAdd.ToString(),GUILayout.Width(100)));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("添加"))
        {
            ProtoReq.addCoin(debug.coinID, debug.coinAdd);
            //debug.info = "添加";
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(20);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("查看当前玩家数"))
        {
            ProtoReq.findPlayer();
            //debug.info = "查看当前玩家数";
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(20);

        GUILayout.BeginHorizontal();
        GUILayout.TextArea(debug.info, GUILayout.Height(100));
        GUILayout.EndHorizontal();
    }
}
#endif
