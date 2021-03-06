﻿using System;
using System.Collections.Generic;
using System.Drawing;

using LudicrousElectron.GUI.Drawing;
using LudicrousElectron.GUI.Geometry;

using OpenTK;

namespace LudicrousElectron.GUI.Elements
{
	public class UIPanel : SingleDrawGUIItem
	{
		public bool NoDraw = false;

        public UIPanel(RelativeRect rect, Color color) : base(rect,color)
		{
			IgnoreMouse = true;
		}

		public UIPanel(RelativeRect rect, Color color, string texture) : base(rect, color, texture)
		{
			IgnoreMouse = true;
		}

		public UIPanel(RelativeRect rect, string texture) : base(rect, Color.White, texture)
		{
			IgnoreMouse = true;
		}

		public override bool Draw()
		{
			if (NoDraw)
				return true;

			return base.Draw();
		}

		public override void Resize(int x, int y)
        {
            base.Resize(x, y);

            CheckMaterial();

            if (CurrentMaterial.DiffuseTexture == null)
                ShapeBuffer.FilledRect(this, Rect);
            else
                HandleTexturedRect();
           
        }
    }
}
