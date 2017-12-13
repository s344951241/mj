//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18408
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Collections;
namespace cn.sharesdk.unity3d{
	public abstract class ShareSDKImpl{

		/// <summary>
		/// Init the ShareSDK.
		/// </summary>
		public abstract void InitSDK (String appKey);

		/// <summary>
		/// Sets the platform config.
		/// </summary>
		public abstract void SetPlatformConfig (Hashtable configs);
		
		/// <summary>
		/// Authorize the specified platform.
		/// </summary>
		public abstract void Authorize (int reqID, PlatformType platform);
		
		/// <summary>
		/// Removes the account of the specified platform.
		/// </summary>
		public abstract void CancelAuthorize (PlatformType platform);
		
		/// <summary>
		/// Determine weather the account of the specified platform is valid.
		/// </summary>
		public abstract bool IsAuthorized (PlatformType platform);

		/// <summary>
		/// Determine weather the APP-Client of platform is valid.
		/// </summary>
		public abstract bool IsClientValid (PlatformType platform);
		/// <summary>
		/// Request the user info of the specified platform.
		/// </summary>
		public abstract void GetUserInfo (int reqID, PlatformType platform);

		/// <summary>
		/// Share the content to the specified platform with api.
		/// </summary>
		public abstract void ShareContent (int reqID, PlatformType platform, ShareContent content);

		/// <summary>
		/// Share the content to the specified platform with api.
		/// </summary>
		public abstract void ShareContent (int reqID, PlatformType[] platforms, ShareContent content);
		
		/// <summary>
		/// Show the platform list to share.
		/// </summary>
		public abstract void ShowPlatformList (int reqID, PlatformType[] platforms, ShareContent content, int x, int y);
		
		/// <summary>
		/// OGUI share to the specified platform. 
		/// </summary>
		public abstract void ShowShareContentEditor (int reqID, PlatformType platform, ShareContent content);

		/// <summary>
		/// share according to the name of node<Content> in ShareContent.xml(in ShareSDKConfigFile.bunle,you can find it in xcode - ShareSDK folider) [only valid in iOS temporarily)]
		/// </summary>
		public abstract void ShareWithContentName (int reqId, PlatformType platform, string contentName, Hashtable customFields);

		/// <summary>
		/// show share platform list according to the name of node<Content> in ShareContent.xml file(in ShareSDKConfigFile.bunle,you can find it in xcode - ShareSDK folider) [only valid in iOS temporarily)] 
		/// </summary>
		public abstract void ShowPlatformListWithContentName (int reqId, string contentName, Hashtable customFields, PlatformType[] platforms, int x, int y);

		/// <summary>
		/// show share content editor according to the name of node<Content> in ShareContent.xml file(in ShareSDKConfigFile.bunle,you can find it in xcode - ShareSDK folider) [only valid in iOS temporarily)] 
		/// </summary>
		public abstract void ShowShareContentEditorWithContentName (int reqId, PlatformType platform, string contentName, Hashtable customFields);

		/// <summary>
		/// Gets the friend list.
		/// </summary>
		public abstract void GetFriendList (int reqID, PlatformType platform, int count, int page);
		
		
		/// <summary>
		/// Follows the friend.
		/// </summary>
		public abstract void AddFriend (int reqID, PlatformType platform, String account);

		/// <summary>
		/// Gets the auth info.
		/// </summary>
		public abstract Hashtable GetAuthInfo (PlatformType platform);

		/// <summary>
		/// the setting of SSO
		/// </summary>
		public abstract void DisableSSO (Boolean disable);

	}
}

