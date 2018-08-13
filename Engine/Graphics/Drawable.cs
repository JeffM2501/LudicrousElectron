using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudicrousElectron.Engine.Graphics
{
	public class Drawable
	{
		public Material Mat = null;

		public virtual bool Draw() { return false; }
	}
}
