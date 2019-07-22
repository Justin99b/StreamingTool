using System;

namespace SocketClient {
	public delegate void Event(params Object[] args);
	public delegate void PacketCallback(Packet packet);
	public delegate void Failed(params Object[] args);
}