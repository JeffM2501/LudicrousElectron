using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LudicrousElectron.Engine.Window;

namespace LudicrousElectron.Engine.RenderChain.Effects
{
    public class PostProcessor : IRenderable
    {
        private static bool GlobalEffectEnabled = true;
        public static void EnableEffects(bool enabled) { GlobalEffectEnabled = enabled; }
       
        protected bool LocalEnabled = false;

        public void Enable(bool value) { LocalEnabled = value; }
        public bool Enabled() { return GlobalEffectEnabled && LocalEnabled; }

        public IRenderable Parrent = null;

        public PostProcessor(string name, IRenderable _parrent)
        {
            Parrent = _parrent;
        }

        public virtual void Render(WindowManager.Window target)
        {
           
        }
    }
}
