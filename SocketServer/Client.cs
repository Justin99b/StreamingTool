using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace SocketClient {
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
					//TODO Cut packet at receivedSize;
					Console.WriteLine(Encoding.ASCII.GetString(buffer));
					Packet packet = Packet.fromBytes(buffer);
					//TODO maybe add automatic buffer size
					packetQueue.Add(packet.PacketGuid, packet);
					if (packet.Type.Equals(Packet.PacketType.EVENT)) {
						if (EventManager.hasEvent(packet.Obj.GetValue("event").ToString()))

							EventManager.execEvent(packet.Obj.GetValue("event").ToString());
					} else {
						
					}
				}
			}).Start();
			keepAlive();
			init();
		}

		public void init() {
			JObject obj = new JObject();
			obj.Add("guid", guid);

			sendEvent("init", obj, packet => { Console.WriteLine(packet.verbose()); });
		}

		public Guid Guid => guid;

		public String sendEvent(String name, JObject request, PacketCallback cb) {
			JObject obj = new JObject();
			Packet.PacketType type = Packet.PacketType.MESSAGE_CALLBACK;
			obj.Add("clientguid", guid);
			obj.Add("type", type.ToString());
			obj.Add("event", name);
			obj.Add("message", request);

			Packet packet = new Packet(type, obj);

			Server.server.SendTo(packet.Data, clientEndPoint);

			while (!packetQueue.ContainsKey(packet.PacketGuid)) ;

			Packet resultPacket;
			packetQueue.TryGetValue(packet.PacketGuid, out resultPacket);

			packetQueue.Remove(packet.PacketGuid);
			cb.Invoke(packet);
			return resultPacket.ToString();
		}

		public void sendPacket(Packet packet) {
			Server.server.SendTo(packet.Data, clientEndPoint);
		}

		public String sendPacket(Packet packet, PacketCallback success, Failed failed, int timeout) {
			Server.server.SendTo(packet.Data, clientEndPoint);
			new Thread(() => {
				//TODO maybe add a timeout var in Packet
				Thread.Sleep(timeout);
				failed.Invoke();
			}).Start();

			while (!packetQueue.ContainsKey(packet.PacketGuid)) ;

			Packet resultPacket;
			packetQueue.TryGetValue(packet.PacketGuid, out resultPacket);

			packetQueue.Remove(packet.PacketGuid);

			success.Invoke(resultPacket);

			return resultPacket.ToString();
		}

		public void keepAlive() {
			new Thread(() => {
				Boolean connected = true;
				while (connected) {
					JObject obj = new JObject();
					obj.Add("clientguid", new Guid());
					obj.Add("special", "ping!");
					EventManager.registerEvent("KEEP_ALIVE");
					sendPacket(new Packet(Packet.PacketType.KEEP_ALIVE, obj), packet => { Console.WriteLine("got bacK!"); }, args => {
						connected = false;
						Console.WriteLine("Connection lost to: " + (IPEndPoint) clientEndPoint);
					}, 5000);
				}
			}).Start();
		}
	}
}