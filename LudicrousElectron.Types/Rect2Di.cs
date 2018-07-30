using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudicrousElectron.Types
{
	public class Rect2Di
	{
		public Vector2i LowerBound = new Vector2i();
		public Vector2i UpperBound = new Vector2i();

		public Rect2Di() { }

		public Rect2Di(int x, int y, int width, int height)
		{
			LowerBound = new Vector2i(x, y);
			UpperBound = new Vector2i(x + width, y + height);
		}

		public Rect2Di(Vector2i origin, Vector2i size)
		{
			LowerBound = new Vector2i(origin.x, origin.x);
			UpperBound = new Vector2i(origin.x + size.x, origin.x + size.y);
		}
	}
}
