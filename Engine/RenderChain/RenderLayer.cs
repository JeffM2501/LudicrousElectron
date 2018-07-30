using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LudicrousElectron.Engine.Window;

namespace LudicrousElectron.Engine.RenderChain
{
	public class RenderLayerItem : IRenderable, IDisposable
	{
		private RenderLayer LinkedLayer = null;

		public RenderLayerItem(RenderLayer _layer = null)
		{
			LinkedLayer = _layer;
			if (LinkedLayer == null)
				LinkedLayer = RenderLayer.DefaultLayer;

			if (LinkedLayer != null)
				LinkedLayer.AddItem(this);
		}

	
		public virtual void Render(WindowManager.Window target) { }

		public virtual void MoveToRenderLayer(RenderLayer newLayer)
		{
			if (LinkedLayer != null)
				LinkedLayer.RemoveItem(this);

			LinkedLayer = newLayer;
			if (LinkedLayer != null)
				LinkedLayer.AddItem(this);
		}

		public void Dispose()
		{
			if (LinkedLayer != null)
				LinkedLayer.RemoveItem(this);

			LinkedLayer = null;
		}

		~RenderLayerItem()
		{
			Dispose();
		}
	}


	public class RenderLayer : IRenderable
	{
		public static RenderLayer DefaultLayer = new RenderLayer();

		private List<RenderLayerItem> RenderItemList = new List<RenderLayerItem>();
		private List<IRenderable> ChildLayers = new List<IRenderable>();

		public void AddChildLayer(IRenderable child)
		{
			ChildLayers.Add(child);
		}

		public void RemoveItem(RenderLayerItem item)
		{
			if (RenderItemList.Contains(item))
				RenderItemList.Remove(item);
		}

		public void AddItem(RenderLayerItem item)
		{
			if (!RenderItemList.Contains(item))
				RenderItemList.Add(item);
		}

		public void Render(WindowManager.Window target)
		{
			foreach (var child in ChildLayers)
				child.Render(target);

			foreach (var item in RenderItemList)
				item.Render(target);
		}
	}
}
