using System;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SocketClient {
	public class Packet {
		
		private Guid packetGuid = Guid.NewGuid();
		private byte[] data;
		private JObject obj = new JObject();
		private PacketType type;
		private Guid sender;
		private int timeout = 5000;
		private JObject message;
		public Packet(PacketType type, String s) {
			this.type = type;
			obj.Add("packetguid", packetGuid.ToString());
			obj.Add("type", type.ToString());
			obj.Add("message", s);
			data = Encoding.ASCII.GetBytes(obj.ToString());
		}
		
		public Packet(PacketType type, JObject obj) {
			this.type = type;
			if(!obj.ContainsKey("packetguid"))
				obj.Add("packetguid", packetGuid.ToString());
			if (obj.ContainsKey("timeout"))
				timeout = int.Parse(obj.GetValue("timeout").ToString());
			sender = Guid.Parse(obj.GetValue("clientguid").ToString());
			if(obj.ContainsKey("message"))
				message = JObject.Parse(obj.GetValue("message").ToString());
			data = Encoding.ASCII.GetBytes(obj.ToString());
		}

		public static Packet fromBytes(byte[] bytes) {
			JObject obj = JObject.Parse(Encoding.ASCII.GetString(bytes));
			//packetGuid = Guid.Parse(obj.GetValue("packetguid").ToString());
			PacketType type;
			PacketType.TryParse(obj.GetValue("type").ToString(), out type);
			return new Packet(type, obj);
		}

		public enum PacketType {
			KEEP_ALIVE, EVENT, MESSAGE_CALLBACK
		}
		
		public Guid PacketGuid {
			get => packetGuid;
		}
		public JObject Obj {
			get => obj;
			set => obj = value;
		}
		public PacketType Type => type;

		public byte[] Data {
			get => data;
			set => data = value;
		}
/*
		public Packet(Guid packetGuid, byte[] data) {
			this.data = Combine(packetGuid.ToByteArray(), data);
		}
*/
		public override string ToString() {
			return Encoding.ASCII.GetString(data);
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