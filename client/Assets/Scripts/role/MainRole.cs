using UnityEngine;
using System.Collections;

public class MainRole : Player
{
    private static MainRole instance;
    public static MainRole Instance
    {
        get {
            if (instance == null)
                instance = new MainRole();
            return instance;
        }
    }

    private MainRole()
    {

    }


}
