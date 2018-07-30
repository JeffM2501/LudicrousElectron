using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace LudicrousElectron.Engine.IO
{
	public static class StreamUtils
	{
		public static int ReadInt(Stream s)
		{
			byte[] buffer = new byte[] { 0, 0, 0, 0 };

			s.Read(buffer, 0, 4);
			if (BitConverter.IsLittleEndian)
				Array.Reverse(buffer);

			return BitConverter.ToInt32(buffer, 0);
		}

		public static string ReadPString(Stream s)
		{
			byte size = (byte)s.ReadByte();
			byte[] buffer = new byte[size];

			s.Read(buffer, 0, size);
			return Encoding.UTF8.GetString(buffer);
		}

		public static string ReadIString(Stream s)
		{
			int size = ReadInt(s);
			byte[] buffer = new byte[size];

			s.Read(buffer, 0, size);
			return Encoding.UTF8.GetString(buffer);
		}
	}
}
