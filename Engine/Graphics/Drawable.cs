using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudicrousElectron.Engine.Graphics
{
	public class Drawable : EventArgs
	{
		public Material CurrentMaterial = null;

		public virtual bool Draw() { return false; }
	}
}
