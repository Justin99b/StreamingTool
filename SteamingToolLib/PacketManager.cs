using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace SocketClient {
    public class PacketManager {
        private static Dictionary<Guid, Packet> packetQueue = new Dictionary<Guid, Packet>();

        private Socket server;
        private IPEndPoint clientEndPoint;
        private UdpClient udpClient;
        private IPEndPoint remoteIpEndPoint;

        public PacketManager(Socket server, IPEndPoint clientEndPoint) {
            this.server = server;
            this.clientEndPoint = clientEndPoint;
        }

        public PacketManager(UdpClient udpClient, IPEndPoint remoteIpEndPoint) {
            this.udpClient = udpClient;
            this.remoteIpEndPoint = remoteIpEndPoint;
        }

        public void sendPacket(Packet packet) {
            byte[] data = packet.getData();
            if (server != null)
                server.SendTo(packet.getData(), clientEndPoint);
            else
                udpClient.Send(data, data.Length);
        }

        public void sendPacket(Packet packet, PacketCallback success, Failed failed, int timeout) {
            Guid callbackGuid = Guid.NewGuid();
            packet.CallbackGuid = callbackGuid;
            byte[] data = packet.getData();
            if (server != null)
                server.SendTo(packet.getData(), clientEndPoint);
            else
                udpClient.Send(data, data.Length);
            bool isSuccess = false;
            new Thread(() => {
                Thread.Sleep(timeout);
                if(!isSuccess)
                    failed.Invoke(packet);
            }).Start();

            Packet callbackPacket;

            while ((callbackPacket = getPacketByCallbackGuid(callbackGuid)) == null) ;
            packetQueue.Remove(packet.PacketGuid);
            isSuccess = true;
            success.Invoke(callbackPacket);
        }

        public static Packet getPacketByCallbackGuid(Guid callbackGuid) {
            foreach (Packet packet in packetQueue.Values.ToList())
                if (packet.CallbackGuid.Equals(callbackGuid))
                    return packet;
            return null;
        }

        public void handlePacket(Packet packet) {
            packetQueue.Add(packet.PacketGuid, packet);
            JObject obj = packet.getObj();
            Console.WriteLine($"added \"{packet.PacketGuid}\" to queue");

            if (packet.Type.Equals(Packet.PacketType.EVENT)) {
                String eventName = obj.GetValue("event").ToString();
                if (EventManager.hasEvent(eventName))
                    EventManager.execEvent(eventName, packet);
            } else if (packet.Type.Equals(Packet.PacketType.KEEP_ALIVE)) {
                //sendPacket(new Packet(Packet.PacketType.KEEP_ALIVE, Guid));
            }
        }
    }
}