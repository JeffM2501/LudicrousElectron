using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudicrousElectron.Types
{
	public class Rect2Df
	{
		public Vector2f LowerBound = new Vector2f();
		public Vector2f UpperBound = new Vector2f();

		public Rect2Df() { }

		public Rect2Df(float x, float y, float width, float height)
		{
			LowerBound = new Vector2f(x, y);
			UpperBound = new Vector2f(x + width, y + height);
		}

		public Rect2Df(Vector2f origin, Vector2f size)
		{
			LowerBound = new Vector2f(origin.x, origin.x);
			UpperBound = new Vector2f(origin.x + size.x, origin.x + size.y);
		}
	}
}
