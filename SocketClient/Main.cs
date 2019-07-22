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
        private static UdpClient udpClient;
        private static IPEndPoint remoteIpEndPoint;
        private static PacketManager packetManager;
        public static Guid guid;
        static void Main(string[] args) {
            remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
            udpClient = new UdpClient();
            packetManager = new PacketManager(udpClient, remoteIpEndPoint);
            try {
                udpClient.Connect("127.0.0.1", 1337);

                new Thread(() => {
                    while (true) {
                        byte[] receive = udpClient.Receive(ref remoteIpEndPoint);
                        Packet packet = Packet.fromBytes(receive);
                        packetManager.handlePacket(packet);
                    }
                }).Start();
                init();
            }
            catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }
        
        static Boolean connected = true;

        public static void init() {
            packetManager.sendPacket(new PacketInit(), packet => {
                guid = Guid.Parse(packet.getObj("guid").ToString());
                keepAlive();
            }, failed => { Console.WriteLine("NAY"); }, 1000);
        }

        public static void keepAlive() {
            new Thread(() => {
                while (connected) {
                    packetManager.sendPacket(new PacketKeepAlive(guid), packet => { Console.WriteLine("got Answere!: \n" + packet.getObj()); }, packet => {
                        connected = false;
                        Console.WriteLine("Connection lost to: " + remoteIpEndPoint);
                    }, 5000);
                }
            }).Start();
        }
    }
}