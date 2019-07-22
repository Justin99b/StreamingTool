using System;
using Newtonsoft.Json.Linq;

namespace SocketClient {
    public class PacketKeepAlive : Packet {
        public PacketKeepAlive(Guid guid) {
            Type = PacketType.KEEP_ALIVE;
            JObject obj = new JObject();
            Message = obj;
            addToObj("message", "ping");
            Sender = guid;
        }
        public PacketKeepAlive() {
            Type = PacketType.KEEP_ALIVE;
            JObject obj = new JObject();
            Message = obj;
            addToObj("message", "pong");
        }
    }
}