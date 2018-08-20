using System;
using System.Collections.Generic;
using System.Drawing;

using LudicrousElectron.GUI.Drawing;
using LudicrousElectron.GUI.Geometry;

using OpenTK;

namespace LudicrousElectron.GUI.Elements
{
	public class UIBlank : SingleDrawGUIItem
	{
		public UIBlank(RelativeRect rect) : base(rect, Color.Transparent)
		{
			IgnoreMouse = false;
		}

		public override bool Draw()
		{
			return true;
		}

		public override void Resize(int x, int y)
		{
			base.Resize(x, y);

			CurrentMaterial = null;
		}
	}
}
