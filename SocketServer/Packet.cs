using System;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ConsoleApp1 {
	public class Packet {
		private Guid packetGuid = Guid.NewGuid();
		private byte[] data;
		private JObject obj = new JObject();
		/*
		public Packet(byte[] data) {
			this.data = data;
		}
		
		public Packet(JObject obj) {
			obj.Add("packetguid", packetGuid.ToString());
			data = Encoding.ASCII.GetBytes(obj.ToString());
		}
		*/
		public Packet(String s) {
			obj.Add("packetguid", packetGuid.ToString());
			obj.Add("message", s);
			data = Encoding.ASCII.GetBytes(obj.ToString());
		}
		
		public Packet(JObject obj) {
			obj.Add("packetguid", packetGuid.ToString());
			data = Encoding.ASCII.GetBytes(obj.ToString());
		}
		
		public Guid PacketGuid {
			get => packetGuid;
		}

		public byte[] Data {
			get => data;
			set => data = value;
		}
/*
		public Packet(Guid packetGuid, byte[] data) {
			this.data = Combine(packetGuid.ToByteArray(), data);
		}
*/
		public override string ToString() {
			return Encoding.ASCII.GetString(data);
		}

		private byte[] Combine(params byte[][] arrays) {
			byte[] combined = new byte[arrays.Sum(a => a.Length)];
			int offset = 0;
			foreach (byte[] array in arrays) {
				System.Buffer.BlockCopy(array, 0, combined, offset, array.Length);
				offset += array.Length;
			}
			return combined;
		}
	}
}