using System;
using System.Collections.Generic;

namespace SocketClient {
	public class PacketEventManager {

		static IDictionary<String, PacketCallback> events = new Dictionary<String, PacketCallback>();
		public static void registerEvent(String name, PacketCallback cb) {
			events.Add(name, cb);
		}

		public static PacketCallback getEvent(String name) {
			PacketCallback e;
			events.TryGetValue(name, out e);
			return e;
		}

		public static void execEvent(String name, Packet packet) {
			getEvent(name).Invoke(packet);
		}

		public static Boolean hasEvent(String name) {
			return events.ContainsKey(name);
		}
	}
}