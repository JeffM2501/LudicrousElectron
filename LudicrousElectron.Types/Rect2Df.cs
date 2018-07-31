using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;

namespace LudicrousElectron.Types
{
	public class Rect2Df
	{
		public Vector2 LowerBound = new Vector2();
		public Vector2 UpperBound = new Vector2();

		public Rect2Df() { }

		public Rect2Df(float x, float y, float width, float height)
		{
			LowerBound = new Vector2(x, y);
			UpperBound = new Vector2(x + width, y + height);
		}

		public Rect2Df(Vector2 origin, Vector2 size)
		{
			LowerBound = new Vector2(origin.X, origin.X);
			UpperBound = new Vector2(origin.X + size.X, origin.Y + size.Y);
		}
	}
}
