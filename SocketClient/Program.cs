using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace SocketClient {
	class Program {
		static void Main(string[] args) {
			IPEndPoint remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

			UdpClient udpClient = new UdpClient();
			try {
				udpClient.Connect("127.0.0.1", 1337);
				byte[] init = Encoding.ASCII.GetBytes("init!");
				udpClient.Send(init, init.Length);

				new Thread(() => {
					while (true) {
						String input = Console.ReadLine();
						Byte[] sendBytes = Encoding.ASCII.GetBytes(input);
						udpClient.Send(sendBytes, sendBytes.Length);
					}
				}).Start();
				new Thread(() => {
					while (true) {
						Byte[] receive = udpClient.Receive(ref remoteIpEndPoint);
						
						Console.WriteLine(Encoding.ASCII.GetString(receive, 0, receive.Length));
					}
				}).Start();
				//udpClient.Close();
			} catch (Exception e) {
				Console.WriteLine(e.ToString());
			}
		}
	}
}