using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class IAPExample : MonoBehaviour {
	
	//用来存放商品列表
	public List<string> productInfo = new List<string>();
	
	//请在inspestor面板中的CommodityID输入你在iTunes Connect后台设定的商品ID
	public string[] CommodityID=new string[3];

	[DllImport("__Internal")]
	private static extern void TestMsg();//测试信息发送
	
	[DllImport("__Internal")]
	private static extern void TestSendString(string s);//测试发送字符串
	
	[DllImport("__Internal")]
	private static extern void TestGetString();//测试接收字符串
	
	[DllImport("__Internal")]
	private static extern void InitIAPManager();//初始化
	
	[DllImport("__Internal")]
	private static extern bool IsProductAvailable();//判断是否可以购买
	
	[DllImport("__Internal")]
	private static extern void RequstProductInfo(string s);//获取商品信息
	
	[DllImport("__Internal")]
	private static extern void BuyProduct(string s);//购买商品
	
	//测试从xcode接收到的字符串
	void IOSToU(string s){
		Debug.Log ("[MsgFrom ios]"+s);
	}
	
	//获取product列表
	void ShowProductList(string s){
		productInfo.Add (s);
	}

	void BuyReturn(string s)
	{
		Debug.Log ("BuyReturn = " + s);
	}

	//获取商品回执
	void ProvideContent(string s){
		Debug.Log ("[MsgFrom ios]proivideContent : "+s);
	}
	
	
	void Start () {
		//初始化
		InitIAPManager();
	}
	
	void Update () {
	
	}
	
	void OnGUI(){
		/*if(GUILayout.Button("Test 1",GUILayout.Width(200), GUILayout.Height(100)))
			TestMsg();
		
		GUILayout.Space (200);
		if(GUILayout.Button("Test 1",GUILayout.Width(200), GUILayout.Height(100)))
			TestSendString("This is a msg form unity3d\tt1\tt2\tt3\tt4");
		
		GUILayout.Space (200);
		if(GUILayout.Button("Test 1",GUILayout.Width(200), GUILayout.Height(100)))
			TestGetString();
		/*通信测试*/
		
		if(Btn ("GetProducts")){
			//如果未能启动IAP则抛出异常
			if(!IsProductAvailable())throw new System.Exception("IAP not enabled");
						
			//每次点击商店按钮则初始化商品列表
			productInfo = new List<string>();

			//RequstProductInfo(string)是获取你做iTunes Connect后台设定的商品信息
			//也可以直接这样写Package_0/tPackage_1,注意每个商品ID之间用空格符号/t分开
			//RequstProductInfo(CommodityID[0]+"/t"+CommodityID[1]+"/t"+CommodityID[2]+"/t"+CommodityID[3]+"/t"+CommodityID[4]+"/t"+CommodityID[5]);
			//RequstProductInfo(CommodityID[0]+"/t"+CommodityID[1]+"/t"+CommodityID[2]);
			RequstProductInfo("money1|money2|money3");
			//productInfo.Add (CommodityID [0] + "/t" + CommodityID [1] + "/t" + CommodityID [2]);
			//RequstProductInfo(CommodityID[0]);
		}
		
		GUILayout.Space(40);
		//productInfo.Add (CommodityID [0] + "/t" + CommodityID [1] + "/t" + CommodityID [2]);
		//循环把商品显示到屏幕中
		for(int i=0; i<productInfo.Count; i++){
			if(GUILayout.Button (productInfo[i],GUILayout.Height (100), GUILayout.MinWidth (200))){
				string[] cell = productInfo[i].Split('\t');
				Debug.Log ("Value = " + cell [cell.Length - 1]);
				BuyProduct(cell[cell.Length-1]);
			}
		}
	}
	//界面上的商店按钮
	bool Btn(string msg){
		GUILayout.Space (100);
		return 	GUILayout.Button (msg,GUILayout.Width (200),GUILayout.Height(100));
	}
}
