using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Reflection; 
using UnityEngine;

namespace NetWork{
	
	public class Msg {
		public string name;
		public object body;
	}

	public class RecvBuff {
		public const int size = 16384;
		public int cur = 0;
		public byte[] buff = new Byte[size];
	}

	public class NetClient{
		private Socket socket;
		public bool connected = false;
		private RecvBuff recvBuff = new RecvBuff();
		private static Dictionary<String, Type> mProtoTbl = new Dictionary<String, Type>();
		private List<Msg> msgList = new List<Msg>();

        private bool IsReconnect = false;

		private static NetClient _netClient;

		private NetClient(){

		}

		public static NetClient Instance(){
			if (_netClient == null) {
				_netClient = new NetClient ();
			}

			return _netClient;
		}

		private UInt16 ReadUInt16(byte[] buff, int offset){
			UInt16 i = (ushort)(buff[offset]*256 + buff[offset + 1]);
			return i;
		}

		private void WriteUInt16(UInt16 i, byte[] buff, int offset){
			byte[] b = BitConverter.GetBytes (i);
			buff[offset] = b[1];
			buff [offset + 1] = b [0];
		}

		public bool Connect( string ip, int port){
			try {
				Debug.Log ("begin connect " + ip);
#if UNITY_IPHONE
                socket = new Socket(SocketHelper.addressFamily, SocketType.Stream, ProtocolType.Tcp);
#else

                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
#endif

                socket.BeginConnect(ip, port, new AsyncCallback(ConnectCallBack), null);
				return true ;
			}
			catch {
                Debug.LogError("Connect Catch");
				return false ;
			}
		}

		public void ConnectCallBack(IAsyncResult ar){
			try {
				socket.EndConnect(ar);  
				connected = true; 
				BeginReceive ();
                EventDispatcher.Instance.Dispatch(GameEventConst.CONNECTED);
            }
            catch(Exception e){
				Debug.Log (e.ToString());
               // Debug.LogError("ConnectCallBack Catch");
			}
		}

		private void SendCallback(IAsyncResult ar)  
		{
            //Debug.LogError("SendCallback");
        }  

		public void Disconnect(){
			socket.Close();
		}

		public void BeginReceive(){
			try{
				socket.BeginReceive(recvBuff.buff, recvBuff.cur, RecvBuff.size - recvBuff.cur, 0, new AsyncCallback(ReceiveCallback), socket);
			}
			catch(Exception e) {
				Debug.Log (e.ToString ());
                //Debug.LogError("BeginReceive Catch");
			}
		}

		private void ReceiveCallback(IAsyncResult ar){
            try
            {
                //if (socket.Poll(-1, SelectMode.SelectRead))
                //{
                //    if (socket.Receive(new byte[1024]) == 0)
                //    {
                //        Debug.LogError("断开");
                //    }
                //}
            }
            catch (Exception e)
            {
                Debug.LogError("Poll Catch");
            }

            try
            {
				int bytesRead = socket.EndReceive(ar);
				if (bytesRead <= 0) {
					BeginReceive ();
					return;
				}
                GameConst.RecBuffLeng += bytesRead;

                Debug.Log(String.Format("recv {0}",bytesRead));
					
				recvBuff.cur += bytesRead;
				int cur = 0;

				while (cur + 2 < recvBuff.cur) {
					int len = 0;
					Msg msg = ReadMsg(recvBuff.buff, cur, ref len);
					if (msg == null)
						break;
                    msgList.Add (msg);
                    // if (mainFun != null)
                    // mainFun(msg);
                    //ProtoRes.Instance.invokeFun(msg.name, msg);
                    //EventDispatcher.Instance.Dispatch<Msg>(GameEventConst.SOCKET_MSG, msg);
                    //Loom.QueueOnMainThread(() => {
                    //    ProtoRes.Instance.invokeFun(msg);
                    //});
                    cur = cur + len;
				}
				Debug.Log(string.Format("{0},{1}",recvBuff.cur, cur));
				if(cur > 0){
					Buffer.BlockCopy(recvBuff.buff, cur, recvBuff.buff, 0, recvBuff.cur - cur);
					recvBuff.cur = recvBuff.cur - cur;
				}
               
            }
			catch(Exception e){
				Debug.Log (e.ToString ());
                //Debug.LogError("ReceiveCallback Catch");
			}
            if (socket.Connected)
            {
                BeginReceive();
            }
           
        }

		private Msg ReadMsg(byte[] buff, int offset, ref int len){
			try{
				len = ReadUInt16(buff, offset) + 2;
				if (offset + len > recvBuff.cur) {
					return null;
				}
				UInt16 nlen = ReadUInt16(buff, offset + 2);
				string name = System.Text.Encoding.ASCII.GetString (buff, offset + 2 + 2, nlen);
				Debug.Log (String.Format("received msg {0} offset={1}, len={2}", name, offset, len-2));

				UInt16 dlen = ReadUInt16(buff, offset + 2 + 2 + nlen);
				Type type = mProtoTbl [name];
				if (type == null) {
					return new Msg(); 
				}
				MemoryStream stream = new MemoryStream ();
				stream.Write (buff, offset + 2 + 2 + nlen + 2, dlen);
				stream.Position = 0;
				var body = ProtoBuf.Meta.RuntimeTypeModel.Default.Deserialize(stream, null, type);
				Msg msg = new Msg();
				msg.name = name;
				msg.body = body;
				return msg;
			}catch(Exception e){
				Debug.Log (e.ToString ());
                //Debug.LogError("ReadMsg Catch");
				return null;
			}
		}

		public Msg PeekMsg(){
			if (msgList.Count == 0) {
				return null;
			}

			Msg msg = msgList [0];
			msgList.RemoveAt (0);
			return msg;
		}

		public void WriteMsg<MsgType>(string name, MsgType msg){
			Debug.Log(string.Format("send msg {0}", name));
			try{
				MemoryStream ms = new MemoryStream ();
				ProtoBuf.Serializer.Serialize(ms, msg);
				byte[] protoByte = ms.ToArray();
				UInt16 protolen = (UInt16)protoByte.Length;

				byte[] nameByte = System.Text.Encoding.ASCII.GetBytes (name);
				UInt16 nlen = (UInt16)nameByte.Length;

				UInt16 len = (UInt16)(2 + nlen + 2 + protolen);
                GameConst.SendBuffLeng += len;
                //Debug.Log("长度" + GameConst.buffLeng);

				byte[] packBuff = new byte[len + 2];



				WriteUInt16 (len, packBuff, 0);
				WriteUInt16 (nlen, packBuff, 2);
				Buffer.BlockCopy(nameByte, 0, packBuff, 2 + 2, nlen);
				WriteUInt16 (protolen, packBuff, 2 + 2 + nlen);
				Buffer.BlockCopy(protoByte, 0, packBuff, 2 + 2 + nlen + 2, protolen);
				Write (0, packBuff);
			}catch(Exception e) {
				Debug.Log (e.ToString());
                //Debug.LogError("WriteMsg Catch");
			}
		}

		private void Write( int msgType, byte [] msgContent){
            try
            {
                socket.BeginSend(msgContent, 0, msgContent.Length, 0, new AsyncCallback(SendCallback), msgContent);
            }
            catch (Exception ex)
            {
                Debug.LogError("断线重连");
                if (!IsReconnect)
                {
                    IsReconnect = true;
                    AlertMgr.Instance.showAlert(ALERT_TYPE.type1, "是否重新连接服务器", delegate
                    {
                        Disconnect();
                        IsReconnect = false;
                        LoginPanel.Instance.load();
                    },delegate {
                        IsReconnect = false;
                    });
                }
             
                //Debug.LogError("Write Catch");
            }
			
		}

		public static void Register()
		{
			//通过GetAssemblies 调用appDomain的所有程序集
			System.Reflection.Assembly assembly = Assembly.GetExecutingAssembly();
			{
				//反射当前程序集的信息
				foreach(Type type in assembly.GetTypes())
				{
					if (!type.IsAbstract && !type.IsInterface && type.GetCustomAttributes (typeof(ProtoBuf.ProtoContractAttribute), false).Length > 0) {
						mProtoTbl [type.FullName] = type;
					}
				}
			}


		}

		public void ToHexString(byte[] bytes)
		{
			try{
				string byteStr = string.Empty;
				if (bytes != null || bytes.Length > 0)
				{
					foreach (var item in bytes)
					{
						byteStr += string.Format("{0:X2} ", item);
					}
				}
				Debug.Log(byteStr);
			}catch(Exception e){
				Debug.Log (e.ToString ());
			}
		}

        public bool isConnect()
        {
            if (socket != null)
            {
                if (!socket.Connected)
                {
                    return false;
                }
                   
                if (socket.Poll(1, SelectMode.SelectRead))
                {
                    try

                    {
                        byte[] temp = new byte[1024];
                        int nRead = socket.Receive(temp);
                        if (nRead == 0)
                        {
                            Debug.LogError("连接已断开");
                        }
                    }
                    catch
                    {
                        Debug.LogError("连接已断开");
                    }
                    try
                    {
                        int ii = socket.Send(new byte[1]);
                    }
                    catch (SocketException se)
                    {
                        if (se.ErrorCode == 10054)
                        {
                            Debug.LogError("连接已断开");
                        }
                    }
                }
            }
            return true;
        }
    }
} 
