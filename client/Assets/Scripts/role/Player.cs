using UnityEngine;
using System.Collections;

public class Player{

    private int _id;
    private string _name;
    private int _pos;
    private string _url;
    private bool _isReady = false;
    private bool _sex = true;

    public Player(int id,string name,int pos,int sex)
    {
        Id = id;
        Name = name;
        Pos = pos;
        Sex = (sex==1?true:false);  
    }
    public Player()
    {

    }
    public int Id
    {
        get
        {
            return _id;
        }

        set
        {
            _id = value;
        }
    }

    public string Name
    {
        get
        {
            return _name;
        }

        set
        {
            _name = value;
        }
    }

    public int Pos
    {
        get
        {
            return _pos;
        }

        set
        {
            _pos = value;
        }
    }

    public bool IsReady
    {
        get
        {
            return _isReady;
        }

        set
        {
            _isReady = value;
        }
    }

    public string Url
    {
        get
        {
            return _url;
        }

        set
        {
            _url = value;
        }
    }

    public bool Sex
    {
        get
        {
            return _sex;
        }

        set
        {
            _sex = value;
        }
    }
}
