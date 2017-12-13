using UnityEngine;

public class Updater:MonoBehaviour
{
    public delegate void ON_UPDATE(float dt);
    public ON_UPDATE onUpdate = null;
    public ON_UPDATE onLateUpdate = null;
    public ON_UPDATE onFixedUpdate = null;
    public ON_UPDATE onGUI = null;

    void Update()
    {
        if(onUpdate!=null)
        onUpdate.Invoke(Time.deltaTime);
    }

    void LateUpdate()
    {
        if(onLateUpdate!=null)
            onLateUpdate.Invoke(Time.deltaTime);
    }

    void FixedUpdate()
    {
        if(onFixedUpdate!=null)
            onFixedUpdate.Invoke(Time.deltaTime);
    }
    void OnGUI()
    {
        if(onGUI!=null)
            onGUI.Invoke(Time.deltaTime);
    }
}