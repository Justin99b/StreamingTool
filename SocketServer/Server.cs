using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SocketClient {
    public class Server {
        public static Socket server;
        public static Dictionary<Socket, Client> clients = new Dictionary<Socket, Client>();

        public static void startServer() {
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, 1337);
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(ip);
            server.Listen(100); //todo screw down please
            while (true) {
                try {
                    Socket socket = server.Accept();
                    if (!clients.ContainsKey(socket)) {
                        Guid guid = Guid.NewGuid();
                        Client client = new Client(guid, socket);
                        clients.Add(socket, client);
                    }
                }
                catch (Exception e) {
                    Console.WriteLine("disconnected");
                }
            }
        }
    }
}