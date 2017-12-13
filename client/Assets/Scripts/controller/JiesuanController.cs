using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JiesuanController:Singleton<JiesuanController> {

    public Dictionary<int,List<string>> _info;

    public JiesuanController()
    {


        _info = new Dictionary<int, List<string>>();
        List<string> list0 = new List<string>();
        List<string> list1 = new List<string>();
        List<string> list2 = new List<string>();
        List<string> list3 = new List<string>();
        _info.Add(0,list0);
        _info.Add(1,list1);
        _info.Add(2, list2);
        _info.Add(3, list3);
    }

    public void clear()
    {
        for (int i = 0; i < 4; i++)
        {
            _info[i].Clear();
        }
    }
}
