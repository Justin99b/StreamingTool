﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SocketClient {
	class Program {
		static void Main(string[] args) {
			Server.startServer();
		}
	}
}