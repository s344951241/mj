using UnityEngine;
using System.Collections;

public class DoTweenFun {

    public static void FlyCard(Transform tran,Vector3 endPos,float time)
    {
        DoTweenUtils.DoMove(tran, endPos, time, null);
        DoTweenUtils.DoScale(tran, Vector3.one, time, null);
    }
}
