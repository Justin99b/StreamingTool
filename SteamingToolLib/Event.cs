namespace SocketClient {
    public delegate void PacketCallback(Packet packet);

    public delegate void Success(Packet packet);

    public delegate void Failed(Packet packet);

    public delegate void EventCallback(params object[] args);

    public class Event {
        private string name;
        private PacketCallback callback;

        public Event(string name, PacketCallback callback) {
            this.name = name;
            this.callback = callback;
        }

        public string Name {
            get => name;
            set => name = value;
        }

        public PacketCallback Callback => callback;
    }
}