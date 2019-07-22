using System;
using Newtonsoft.Json.Linq;

namespace SocketClient {
    public class PacketInit : Packet {
        public PacketInit() {
            Type = PacketType.INIT;
        }

        public PacketInit(Guid callbackGuid, Guid guid) {
            Type = PacketType.INIT;
            CallbackGuid = callbackGuid;
            addToObj("guid", guid.ToString());
        }
    }
}