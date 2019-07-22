using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace ConsoleApp1 {
	public class Client {
		private Guid guid = Guid.NewGuid();
		private EndPoint clientEndPoint;
		private Dictionary<Guid, Packet> packetQueue = new Dictionary<Guid, Packet>();

		public Client(EndPoint clientEndPoint) {
			this.clientEndPoint = clientEndPoint;
			new Thread(() => {
				while (true) {
					byte[] buffer = new byte[1024];
					//Todo ask J if work when message is over 1024 bytes
					int receivedSize = Server.server.ReceiveFrom(buffer, ref clientEndPoint);
					string received = Encoding.ASCII.GetString(buffer);
					Packet packet = new Packet(received);
					//TODO maybe add automatic buffer size
					packetQueue.Add(packet.PacketGuid, packet);
				}
			}).Start();
			sendMessage("test", () => { Console.WriteLine("test callback"); });
		}

		public Guid Guid => guid;

		public void sendMessage(String s) {
			byte[] bytes = Encoding.ASCII.GetBytes(s);
			Server.server.SendTo(bytes, clientEndPoint);
		}

		public String sendMessage(String s, Action callback) {
			JObject obj = new JObject();
			obj.Add("clientguid", guid);
			obj.Add("message", s);

			Packet packet = new Packet(obj);

			Server.server.SendTo(packet.Data, clientEndPoint);

			while (!packetQueue.ContainsKey(packet.PacketGuid)) ;

			Packet resultPacket;
			packetQueue.TryGetValue(packet.PacketGuid, out resultPacket);

			packetQueue.Remove(packet.PacketGuid);

			return resultPacket.ToString();
		}
	}
}