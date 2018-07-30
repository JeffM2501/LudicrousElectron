using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LudicrousElectron.Engine.Window;

namespace LudicrousElectron.Engine.RenderChain
{
	public interface IRenderable
	{
		void Render(WindowManager.Window target);
	}
}
