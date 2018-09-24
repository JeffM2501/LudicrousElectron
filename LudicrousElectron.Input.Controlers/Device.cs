using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudicrousElectron.Input.Controlers
{
    public class Device
    {
        public virtual string DeviceName { get; } = string.Empty;
        public virtual string GUID { get; } = string.Empty;

        public class Control : EventArgs
        {
            public string Name = string.Empty;

            public virtual double GetValue(int index) { return 0; }
        }

        public virtual void UpdateState() { }

        public virtual void StartPoll() { }

        public virtual void EndPoll() { }
    }
}
