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
        private Guid guid;
        private EndPoint clientEndPoint;
        private Dictionary<Guid, Packet> packetQueue = new Dictionary<Guid, Packet>();
        private PacketManager packetManager;

        public Client(Guid guid, EndPoint clientEndPoint, PacketInit packetInit) {
            this.guid = guid;
            this.clientEndPoint = clientEndPoint;
            
            packetManager = new PacketManager(Server.server, (IPEndPoint) clientEndPoint);
            packetManager.handlePacket(packetInit);
            
            packetManager.sendPacket(packetInit, packet => { Console.WriteLine("YAAAAAY"); }, packet => { Console.WriteLine("WTF"); }, 1000);

            new Thread(() => {
                while (true) {
                    byte[] data = new byte[1024];
                    Server.server.ReceiveFrom(data, ref clientEndPoint);

                    Packet packet = Packet.fromBytes(data);
                    packetManager.handlePacket(packet);
                    //Todo ask J if work when message is over 1024 bytes
                    /*
                     *                     Console.WriteLine("gonna receive");

                    Console.WriteLine("received!");
                    //TODO Cut packet at receivedSize;
                    Console.WriteLine(Encoding.ASCII.GetString(buffer));
                    Packet packet = Packet.fromBytes(buffer);
                    //TODO maybe add automatic buffer size
                    packetQueue.Add(packet.PacketGuid, packet);
                    Console.WriteLine($"added \"{packet.PacketGuid}\" to queue");
                    if (packet.Type.Equals(Packet.PacketType.EVENT)) {
                        string eventName = packet.getObj().GetValue("event").ToString();
                        if (EventManager.hasEvent(eventName))
                            EventManager.execEvent(eventName, packet);
                    } else {
                        
                    }
                     */
                }
            }).Start();
        }

        public void init() {
            EventManager.registerEvent("init", packet => {
                Console.WriteLine(packet.verbose());
                EventManager.unregisterEvent("init"); 
            });
            
        }

        public Guid Guid => guid;

        Boolean connected = true;

        public void keepAlive() {
            new Thread(() => {
                while (connected) {
                    packetManager.sendPacket(new PacketKeepAlive(guid), packet => { Console.WriteLine("got bacK!"); }, packet => {
                        connected = false;
                        Console.WriteLine("Connection lost to: " + clientEndPoint);
                    }, 5000);
                }
            }).Start();
        }
    }
}