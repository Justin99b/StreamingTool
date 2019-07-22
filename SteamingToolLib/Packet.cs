using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SocketClient {
	public class Packet {
		
		private Guid packetGuid = Guid.NewGuid();
		private JObject obj = new JObject();
		private PacketType type;
		private Guid sender;
		private Guid callbackGuid;
		private int timeout = 5000;
		private JObject message = new JObject();
		
		public Packet() {
			
		}
		
		public Packet(PacketType type, JObject obj) {
			this.type = type;
			this.obj = obj;

			if (obj.ContainsKey("timeout"))
				timeout = int.Parse(obj.GetValue("timeout").ToString());
			
			if(obj.ContainsKey("sender"))
				sender = Guid.Parse(obj.GetValue("sender").ToString());
			
			if(obj.ContainsKey("message"))
				message = JObject.Parse(obj.GetValue("message").ToString());
			
			if(obj.ContainsKey("callbackguid"))
				callbackGuid = Guid.Parse(obj.GetValue("callbackguid").ToString());
		}

		public static Packet fromBytes(byte[] bytes) {
			JObject obj = JObject.Parse(Encoding.ASCII.GetString(bytes));
			//packetGuid = Guid.Parse(obj.GetValue("packetguid").ToString());
			PacketType type;
			PacketType.TryParse(obj.GetValue("type").ToString(), out type);
			return new Packet(type, obj);
		}

		public byte[] getData() {
			return Encoding.ASCII.GetBytes(getObj().ToString());
		}
		public enum PacketType {
			INIT, KEEP_ALIVE, EVENT, MESSAGE_CALLBACK
		}
		
		public Guid PacketGuid {
			get => packetGuid;
		}

		public JObject getObj() {
			obj.TryAdd("packetguid", packetGuid.ToString());
			obj.TryAdd("type", type.ToString());
			obj.TryAdd("sender", sender.ToString());
			obj.TryAdd("message", message);
			obj.TryAdd("callbackguid", CallbackGuid.ToString());
			return obj;
		}

		public Boolean addToObj(String key, string value) {
			return message.TryAdd(key, value);
		}

		public Object getObj(String key) {
			return message.GetValue(key);
		}
		
		
		public PacketType Type {
			get => type;
			set => type = value;
		}

		public Guid CallbackGuid {
			get => callbackGuid;
			set => callbackGuid = value;
		}

		public Guid Sender {
			get => sender;
			set => sender = value;
		}

/*
		public Packet(Guid packetGuid, byte[] data) {
			this.data = Combine(packetGuid.ToByteArray(), data);
		}
*/
		public override string ToString() {
			return Encoding.ASCII.GetString(getData());
		}

		public String verbose() {

			return $"PacketType: {type}\nSender: {sender}\nObj: {obj.ToString()}";
		}

		public JObject Message {
			get => message;
			set => message = value;
		}

		public int Timeout {
			get => timeout;
			set => timeout = value;
		}
	}
}