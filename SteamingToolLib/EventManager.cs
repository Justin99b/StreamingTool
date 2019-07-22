using System;
using System.Collections.Generic;

namespace SocketClient {
	public class EventManager {

		static IDictionary<string, Event> events = new Dictionary<string, Event>();
		public static void registerEvent(string name, PacketCallback callback) {
			events.Add(name, new Event(name, callback));
		}

		public static Event getEvent(string name) {
			Event e;
			events.TryGetValue(name, out e);
			return e;
		}

		public static void unregisterEvent(string name) {
			events.Remove(name);
		}

		public static void execEvent(String name, Packet packet) {
			getEvent(name).Callback.Invoke(packet);
		}

		public static Boolean hasEvent(String name) {
			return events.ContainsKey(name);
		}
	}
}