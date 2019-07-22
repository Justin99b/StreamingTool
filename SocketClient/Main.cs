using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using SocketClient;
using Newtonsoft.Json.Linq;

namespace SocketClient {
    class Program {
        private static TcpClient client;
        private static PacketManager packetManager;
        private static Guid guid;
        static void Main(string[] args) {
            new Login();

            client = new TcpClient("127.0.0.1", 1337);
            packetManager = new PacketManager(client.Client);
            guid = packetManager.waitForInit();
            Console.WriteLine("INIT FINISHED GID = " + guid);
            while (client.Connected) {
                Thread.Sleep(1000);
            }

        }
    }
}