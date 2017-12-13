using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TextureMgr : Singleton<TextureMgr> {

    public const string TEX_00001 = "Tex_00001";
    public const string TEX_00002 = "Tex_00002";
    public const string TEX_CODE = "code";
    public const string TEXT_ICONLOAD = "iconLoad";

    private Dictionary<string, Texture> _dicRunTimeTexture;
    private string[] _strTexName = {
        //TEX_00001,
        //TEX_00002,
        //TEX_CODE,
        TEXT_ICONLOAD
    };
    private const string _strPathPrefix = "";

    public TextureMgr()
    {
        _dicRunTimeTexture = new Dictionary<string, Texture>();
    }

    public void StartLoading()
    {
        string[] realPaths = new string[_strTexName.Length];
        for (int i = 0; i < _strTexName.Length; i++)
        {
            realPaths[i] = URLConst.GetRunTimeTexture(_strTexName[i]);
        }
        Action<object> func = delegate (object data)
        {
            for (int i = 0; i < realPaths.Length; i++)
            {
                Resource res = ResourceManager.Instance.GetResource(realPaths[i]);
                Texture tex = res.MainAsset as Texture;
                _dicRunTimeTexture.Add(_strTexName[i], tex);
                ResourceManager.Instance.DestoryResource(res.BundlePath, false, true);
            }
        };
        ResourceManager.Instance.DownLoadBundles(realPaths, func, ResourceManager.DEFAULT_PRIORITY);
    }

    public Texture GetTextureByName(string name)
    {
        Texture text;
        _dicRunTimeTexture.TryGetValue(name, out text);
        return text;
    }

    public void Release(string name)
    {
        Texture kTexture;
        if (_dicRunTimeTexture.TryGetValue(name, out kTexture))
        {
            _dicRunTimeTexture.Remove(name);
            GameObject.Destroy(kTexture);
            kTexture = null;
        }
    }

}
