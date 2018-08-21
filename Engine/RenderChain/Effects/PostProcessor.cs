using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LudicrousElectron.Engine.Window;

namespace LudicrousElectron.Engine.RenderChain.Effects
{
    public class PostProcessor : RenderLayer
    {
        public string Name { get; protected set; } = string.Empty;

        private static bool GlobalEffectEnabled = true;
        public static void EnableEffects(bool enabled) { GlobalEffectEnabled = enabled; }
       
        protected bool LocalEnabled = false;

        public void Enable(bool value) { LocalEnabled = value; }
        public bool Enabled() { return GlobalEffectEnabled && LocalEnabled; }

        public PostProcessor(string name)
        {
            Name = name;
        }
    }
}
