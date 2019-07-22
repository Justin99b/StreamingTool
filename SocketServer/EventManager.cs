using System;
using System.Collections.Generic;

namespace ConsoleApp1 {
	public class EventManager {

		static IDictionary<String, Event> events = new Dictionary<String, Event>();
		public static void registerEvent(String name, Event e) {
			events.Add(name, e);
		}

		public static Event getEvent(String name) {
			Event e;
			events.TryGetValue(name, out e);
			return e;
		}

		public static void execEvent(String name, params Object[] args) {
			getEvent(name).Invoke(args);
		}

		public static void registerEvent2(string name, Action action) {
			//events.Add(name, new Event(action));
		}
	}
}