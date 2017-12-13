using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
public class IconMgr:Singleton<IconMgr>{

	private Dictionary<string,UnityEngine.Object> configSprites;
	private Material _material;
	private Texture2D _texture;
	private Texture2D _textureAlpha;

	private Dictionary<string,Texture2D> _dicImageTex = new Dictionary<string, Texture2D>();
	private Dictionary<string,List<RawImage>> _dicImage = new Dictionary<string, List<RawImage>>();

	public void SetRawImage(RawImage kImage,string path,bool isNativeSize = true)
	{
		if (kImage == null)
			return;
		if (_dicImageTex.ContainsKey (path)) {
			kImage.texture = _dicImageTex [path];
			if (isNativeSize)
				kImage.SetNativeSize ();
			return;
		}

		List<RawImage> list;
		bool repeat = false;
		if (!_dicImage.ContainsKey (path)) {
			_dicImage [path] = new List<RawImage> ();
		} else
			repeat = true;
		list = _dicImage [path];
		list.Add (kImage);
		if (repeat)
			return;
        SimpleLoader.Instance.Load(path, delegate (WWW www)
        {
            if (_dicImage.ContainsKey(path))
            {
                list = _dicImage[path];
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].texture = www.texture;
                }
                list.Clear();
                _dicImage.Remove(path);
            }
            _dicImageTex[path] = www.texture;
            www.Dispose();
            www = null;
        });
	}

	public bool SetImage(Image kImage,string spriteName,bool isNativeSize = false)
	{
		if (kImage == null || string.IsNullOrEmpty (spriteName))
			return false;
		var sprite = GetSprite (spriteName);
		if (sprite) {
			kImage.sprite = sprite;
			if (isNativeSize) {
				kImage.SetNativeSize ();
			}
			return true;
		}
		return false;
	}

	private Sprite GetSprite(string fileName)
	{
		if (configSprites.ContainsKey (fileName))
			return configSprites [fileName] as Sprite;
		return null;
	}

	public IconMgr()
	{

		GetObject ();
	}

	private void GetObject()
	{
#if _DEBUG
		if(configSprites==null)
			configSprites = new Dictionary<string,UnityEngine.Object>();
		Sprite[] assets = Resources.LoadAll<Sprite>("GameAsset/Assetbundles/UI/Icon/");
		for(int i=0;i<assets.Length;i++)
		{
			var textAsset = assets[i];
			if(textAsset is Sprite)
				configSprites[textAsset.name] = textAsset;
		}

#else
		configSprites = new Dictionary<string,UnityEngine.Object>();
		CreateIcon(ResourceManager.Instance.GetResource(URLConst.ICON_ATLAS_PATH));
#endif
	}

	private void CreateIcon(Resource res)
	{
		foreach (var item in res.dicObject) {
			if (configSprites.ContainsKey (item.Key) || !item.Value is Sprite)
				continue;
			configSprites.Add (item.Key, item.Value);
		}
		ResourceManager.Instance.DestoryResource (res.BundlePath, false, true);
	}

	public void SetImageWithSize(Image kImage,string fileName,int iconSize)
	{
		if (kImage == null)
			return;
		Sprite spt = GetSpriteWithSize (fileName, iconSize);
		if (spt != null)
			kImage.sprite = spt;
	}

	private Sprite GetSpriteWithSize(string fileName,int iconSize)
	{
		string iconName = fileName + "_" + iconSize;
		if (configSprites.ContainsKey (iconName))
			return configSprites [iconName] as Sprite;
		return null;
	}

	public bool IsExistsRawImage(string imageId)
	{
		#if UNITY_EDITOR
		string path = Application.dataPath+"/"+imageId+".png";
		#else
		string path = URL.localCachePath+"Photos/"+imageId+".png";
		#endif
		return FileTools.IsExistFile (path);
	}
	public bool SetHeadRawImage(RawImage kImage,string imageId)
	{
		if (kImage == null)
			return false;
        kImage.texture = TextureMgr.Instance.GetTextureByName(TextureMgr.TEXT_ICONLOAD);

#if UNITY_EDITOR
        string path = Application.dataPath + "/" + imageId + ".png";
#else
		string path = URL.localCachePath+"Photos/"+imageId+".png";
#endif

        if (FileTools.IsExistFile (path)) {
#if UNITY_EDITOR
			IconMgr.Instance.SetRawImage (kImage, Application.dataPath + "/" + imageId + ".png", false);
#else
			IconMgr.Instance.SetRawImage(kImage,"file://"+URL.localCachePath+"Photos/"+imageId+".png",false);
#endif
			return true;
		} else
			return false;
	}

	public void Destroy()
	{
		if (configSprites != null)
			configSprites.Clear ();
		configSprites = null;
	}



}
