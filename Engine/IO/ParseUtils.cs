﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudicrousElectron.Engine.IO
{
	public static class ParseUtils
	{
		public static byte[] HexStringToBytes(string hex)
		{
			if (hex.Length % 2 != 0)
				return new byte[0];

			byte[] result = new byte[hex.Length >> 1];

			for (int i = 0; i < hex.Length >> 1; ++i)
				result[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));

			return result;
		}

		public static int GetHexVal(char hex)
		{
			int val = (int)hex;
			return val - (val < 58 ? 48 : 55);
		}
	}
}
