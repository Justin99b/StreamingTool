using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using SocketClient;
using Newtonsoft.Json.Linq;

namespace SocketClient {
	class Program {
		public static UdpClient udpClient;
		public static IPEndPoint remoteIpEndPoint;
		private static Dictionary<Guid, Packet> packetQueue = new Dictionary<Guid, Packet>();

		static void Main(string[] args) {
			remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
			udpClient = new UdpClient();
			try {
				udpClient.Connect("127.0.0.1", 1337);

				PacketEventManager.registerEvent("init", packet => {
					Console.WriteLine("INIT!!!");
					Console.WriteLine(packet.verbose());
				});
				keepAlive();
				new Thread(() => {
					while (true) {
						String input = Console.ReadLine();
						Byte[] sendBytes = Encoding.ASCII.GetBytes(input);
						udpClient.Send(sendBytes, sendBytes.Length);
					}
				}).Start();

				new Thread(() => {
					while (true) {
						byte[] receive = udpClient.Receive(ref remoteIpEndPoint);
						Packet packet = Packet.fromBytes(receive);
						JObject obj = packet.Obj;
						if (packet.Type.Equals(Packet.PacketType.EVENT)) {
							if (PacketEventManager.hasEvent(obj.GetValue("event").ToString()))
								PacketEventManager.execEvent(obj.GetValue("event").ToString(), packet);
						} else if (packet.Type.Equals(Packet.PacketType.KEEP_ALIVE)) {
							//sendPacket(new Packet(Packet.PacketType.KEEP_ALIVE, Guid));
						}

						Console.WriteLine(Encoding.ASCII.GetString(receive, 0, receive.Length));
					}
				}).Start();
			} catch (Exception e) {
				Console.WriteLine(e.ToString());
			}
		}

		public static void sendPacket(Packet packet) {
			byte[] data = packet.Data;
			udpClient.Send(data, data.Length);
		}
		
		public static String sendPacket(Packet packet, PacketCallback success, Failed failed, int timeout) {
			byte[] data = packet.Data;
			udpClient.Send(data, data.Length);
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

		public static void keepAlive() {
			new Thread(() => {
				Boolean connected = true;
				while (connected) {
					JObject obj = new JObject();
					obj.Add("clientguid", new Guid());
					obj.Add("special", "pong!");
					sendPacket(new Packet(Packet.PacketType.KEEP_ALIVE, obj), packet => {
						Console.WriteLine("got bacK!");
					}, args => {
						connected = false;
						Console.WriteLine("Connection lost to: " + remoteIpEndPoint);
					}, 5000);
				}
			}).Start();
		}
	}
}