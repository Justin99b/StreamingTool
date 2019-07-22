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
        private Guid guid = Guid.NewGuid();
        private PacketManager packetManager;
        private Socket socket;
        public Client(Guid guid, Socket socket) {
            Console.WriteLine("CONECTEEEEEEEEEEEEEEED");
            this.guid = guid;
            this.socket = socket;
            packetManager = new PacketManager(socket);
            packetManager.sendPacket(new PacketInit(guid), packet => { Console.WriteLine("YAAAAAY"); }, packet => { Console.WriteLine("WTF"); }, 1000);
            
        }

        public void init() {
            EventManager.registerEvent("init", packet => {
                Console.WriteLine(packet.verbose());
                EventManager.unregisterEvent("init");
            });
        }

        public Guid Guid => guid;
    }
}