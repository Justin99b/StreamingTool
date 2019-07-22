using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SocketClient {
	public class Server {
		public static Socket server;
		public static Dictionary<Guid, Client> clients = new Dictionary<Guid, Client>();
		public static void startServer() {
			
			IPEndPoint ip = new IPEndPoint(IPAddress.Any, 1337);
			server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			server.Bind(ip);

			while (true) {
				IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0); 
				byte[] data = new byte[1024];
				EndPoint remote = (EndPoint) (sender);
				server.ReceiveFrom(data, ref remote);
				Console.WriteLine(Encoding.ASCII.GetString(data));
				Packet packet = Packet.fromBytes(data);
				if (packet.getObj("sender") != null) {
					String senderGuid = packet.getObj("sender").ToString();
					if (clients.ContainsKey(Guid.Parse(senderGuid))) {
						//todo uhm... yeah
					}
				}
				Guid guid = Guid.NewGuid();
				
				PacketInit packetInit = new PacketInit(packet.CallbackGuid, guid);
				Client client = new Client(guid, remote, packetInit);
				clients.Add(client.Guid, client);
			}
		}
	}
}