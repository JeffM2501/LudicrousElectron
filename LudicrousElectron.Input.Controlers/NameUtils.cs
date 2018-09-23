using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudicrousElectron.Input.Controlers
{
	public static class NameUtils
	{
		public static string GetAxisLabel(int index)
		{
			switch (index)
			{
				case 0:
					return "X";
				case 1:
					return "Y";
				case 2:
					return "R";
				case 3:
					return "Z";
				case 4:
					return "U";
				case 5:
					return "V";
			}

			return "A" + index.ToString();
		}

		public static string GetButtonLabel(int index)
		{
			return "Button" + (index + 1).ToString();
		}
	}
}
