using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ConsoleApp1 {
	public class Server {
		public static Socket server;
		public static Dictionary<Guid, Client> clients = new Dictionary<Guid, Client>();
		public static void startServer() {
			
			IPEndPoint ip = new IPEndPoint(IPAddress.Any, 1337);
			server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			server.Bind(ip);

			IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0); 
			while (true) {
				byte[] data = new byte[1024];
				EndPoint remote = (EndPoint) (sender);
				server.ReceiveFrom(data, ref remote);
				Client client = new Client(remote);
				clients.Add(client.Guid, client);
			}
		}
	}
}