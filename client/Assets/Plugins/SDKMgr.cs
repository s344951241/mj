using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using cn.sharesdk.unity3d;
public class SDKMgr : SingletonMonoBehaviour<SDKMgr> {
	private ShareSDK ssdk;

	public OnUserInfo updateUserInfo;
	public delegate void OnUserInfo(string openId, string unionId, string nickName,int sex, string imageUrl);
    public delegate bool SetImage(Image kImage, string spriteName, bool isNativeSize = false);
    public SetImage setImage;
    // Uprivate ShareSDK ssdk;se this for initialization
    void Start () {
		ssdk = gameObject.GetComponent<ShareSDK> ();
		ssdk.authHandler = AuthResultHandler;
		ssdk.showUserHandler = GetUserInfoResultHandler;
		ssdk.shareHandler = ShareResultHandler;  
	}

	//public void GetUserInfo(OnUserInfo userInfoCb)
	public void GetUserInfo()
	{
		/*GameObject driverGo = GameObject.Find ("Driver");
		if (driverGo != null) 
		{
			SDKMgr sdkMgr = driverGo.GetComponent<SDKMgr>();
			sdkMgr.updateUserInfo = userInfoCb;//改为eventdispatch也可以，这里测试
			sdkMgr.DoGetUserInfo();
		}*/
		//this.updateUserInfo = userInfoCb;
		DoGetUserInfo();
	}

	public void AuthUser()
	{
		ssdk.Authorize(PlatformType.WeChat);
	}

	void AddLog(string msg)
	{
		Debug.LogError(msg);
	}

	void AuthResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable result)
	{
		if (state == ResponseState.Success) {
			AddLog ("authoirize success");
			GetUserInfo();
		} else if (state == ResponseState.Fail) {
			AddLog ("authoirize falied " + result ["error_code"] + ";error_msg" + result ["error_msg"]);
		} else if (state == ResponseState.Cancel) {
			AddLog ("authoirize cancel");
		}
	}

	void GetUserInfoResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable result)
	{
		if (state == ResponseState.Success) {
			AddLog ("getuserinfo success");
			AddLog (MiniJSON.jsonEncode (result));
			string name = (string)result ["nickname"];
			string openid = (string)result ["openid"];
			string unionid = (string)result ["unionid"];
			string headimgurl = (string)result ["headimgurl"];
            double sex = (double)result["sex"];
			AddLog ("name = " + name);
			AddLog ("openid = " + openid);
			AddLog ("uionid = " + unionid);
			AddLog ("headimgurl = " + headimgurl);
            AddLog("sex" + sex);
            if (updateUserInfo != null) {
				updateUserInfo (openid, unionid, name, (int)sex, headimgurl);
			}
		} else if (state == ResponseState.Fail) {
			ssdk.Authorize(PlatformType.WeChat);
			AddLog ("getuserinfo falied " + result ["error_code"] + ";error_msg" + result ["error_msg"]);
		} else if (state == ResponseState.Cancel) {
			AddLog ("getuserinfo cancel");
		}		
	}

	void ShareResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable result)
	{
		if (state == ResponseState.Success) {
			AddLog ("share success");
		} else if (state == ResponseState.Fail) {
			AddLog ("share falied " + result ["error_code"] + ";error_msg" + result ["error_msg"]);
		} else if (state == ResponseState.Cancel) {
			AddLog ("share cancel");
		}		
	}

	public void DoGetUserInfo()
	{
		ssdk.GetUserInfo(PlatformType.WeChat);
	}

	public void DoShareTest()
	{
		ShareContent content = new ShareContent ();
		content.SetText("胶己人雀友会");
		content.SetImageUrl ("http://www.27toy.com/images/108.png");
        content.SetTitle("潮汕麻将四人玩法\n邀请胶己人一起来麻将，三缺一就等你了");
        content.SetUrl("http://27toy.com/");
        content.SetShareType (ContentType.Webpage);

		ssdk.ShareContent (PlatformType.WeChatMoments, content);

      
	}

	public void DoShareTestFriend()
	{
        ShareContent content = new ShareContent();
        content.SetText("潮汕麻将四人玩法\n邀请胶己人一起来麻将，三缺一就等你了");
        content.SetImageUrl("http://www.27toy.com/images/108.png");
        content.SetTitle("胶己人雀友会");
        content.SetUrl("http://27toy.com/");
        content.SetShareType(ContentType.Webpage);
        ssdk.ShareContent (PlatformType.WeChat, content);
    }
    public void DoShareTable(int tableId,int round,string info)
    {
        ShareContent content = new ShareContent();
        content.SetText(round + "局潮汕麻将四人玩法\n"
            + info + "\n"
           + "邀请胶己人一起来麻将，三缺一就等你了");
        content.SetImageUrl("http://www.27toy.com/images/108.png");
        content.SetTitle(
            "房间号：" + tableId);
        content.SetUrl("http://27toy.com/");
        content.SetShareType(ContentType.Webpage);
        ssdk.ShareContent(PlatformType.WeChat, content);
    }
    public void Capture(string path)
    {
        
        ShareContent content = new ShareContent();
        content.SetText("结算");
       // content.SetImageUrl("http://www.27toy.com/images/108.png");
        content.SetTitle("胶己人雀友会");
        content.SetUrl("http://27toy.com/");
       // content.SetComment("分享结算");
        content.SetShareType(ContentType.Image);

        //Application.CaptureScreenshot("shot4Share.png");
        content.SetImagePath(path);
        ssdk.ShareContent(PlatformType.WeChat, content);
    }

	public Sprite myWXPic;
	public void InitImagePath()
	{
		if (!Directory.Exists (imagePath)) 
		{
			Directory.CreateDirectory (imagePath);
		}
		myWXPic = null;
	}
	public string imagePath
	{
		get{
			//return Application.persistentDataPath + "/ImageCache/";
			return Application.persistentDataPath + "/";
		}
	}
	public string _outputLog;
	void OnGUI()
	{
		//_outputLog = GUI.TextField (new Rect (Screen.width/2, 450, 790, 100), _outputLog);
	}
	public void NewAddLog(string log)
	{
		_outputLog = _outputLog + log + "\n";
	}

	public void SetAsyncImage(string url, Image image)
	{
        //if (myWXPic != null)
        //{
        //	image.sprite = myWXPic;
        //	return;
        //}
        if (GameConst.isGuest)
        {
            if (setImage != null)
            {
                setImage(image, "head");
            }
        }
        else
        {
            if (url.Equals("htttp://test"))
            {
                if (setImage != null)
                {
                    setImage(image, "head");
                }
            }
            else
            {
                Debug.Log("hashcode = " + MyGetHashCode(url).ToString());
                if (!File.Exists(imagePath + MyGetHashCode(url)))
                {
                    StartCoroutine(DownLoadImage(url, image));
                }
                else
                {
                    StartCoroutine(LoadLocalImage(url, image));
                }
            }
          
        }
	
	}

	IEnumerator DownLoadImage(string url, Image image)
	{
		Debug.Log ("Download image" + MyGetHashCode(url).ToString());
		NewAddLog("download image");
		WWW www = new WWW (url);
		yield return www;

		Texture2D tex2d = www.texture;
		Sprite m_sprite = Sprite.Create (tex2d, new Rect (0, 0, tex2d.width, tex2d.height), new Vector2 (0, 0));
		image.sprite = m_sprite;
		myWXPic = m_sprite;
		byte[] pngData = tex2d.EncodeToPNG();
		File.WriteAllBytes(imagePath + MyGetHashCode (url), pngData);


	}

	IEnumerator LoadLocalImage(string url, Image image)
	{
		//NewAddLog ("local image");

		string filePath = "file:///" + imagePath +  MyGetHashCode (url);
		WWW www = new WWW (filePath);
		yield return www;

		Texture2D texture = www.texture;
		Sprite m_sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),new Vector2(0, 0));
		Debug.Log ("Load local image size = " + texture.width.ToString() + " " + texture.height.ToString());
		image.sprite = m_sprite;
		myWXPic = m_sprite;
	}

	public int MyGetHashCode(string url)
	{
		
		int value = url.GetHashCode ();
		if (value < 0)
			value = -value;
		return value;
	}

}
