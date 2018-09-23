using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using SharpDX.DirectInput;

namespace LudicrousElectron.Input.Controlers
{
    public class DInputKeyboard : Keyboard
    {
        public override string DeviceName { get => Device.InstanceName; }
        public override string GUID { get => Device.InstanceGuid.ToString(); protected set { return; } }

        protected DeviceInstance Device = null;

        public class DIKeys : Control
        {
            public SharpDX.DirectInput.Keyboard DIBoard = null;
            public KeyboardState State = null;

            public DIKeys(DirectInput di)
            {
                DIBoard = new SharpDX.DirectInput.Keyboard(di); ;
            }

            public override double GetValue(int index)
            {
                if (State == null)
                    return 0;

                if (State.IsPressed((Key)index))
                    return 1;

                return 0;
            }

            public virtual void Update()
            {
                State = DIBoard.GetCurrentState();
            }
        }

        public DInputKeyboard(DirectInput di, DeviceInstance instance)
        {
            Device = instance;
            Keys = new DIKeys(di);
            Keys.Name = instance.ProductName;
        }

        public override void UpdateState()
        {
            DIKeys k = Keys as DIKeys;
            k.Update();

            if (!HaveEvent())
                return;

            foreach (var key in k.State.PressedKeys)
                CallKeyPressed((int)key);
        }


        internal static Dictionary<string, DInputKeyboard> BoardList = null;

        internal static DInputKeyboard[] GetDevices()
        {
            if (DInputJoystick.DInput == null)
                DInputJoystick.DInput = new DirectInput();

            if (BoardList == null)
            {
                BoardList = new Dictionary<string, DInputKeyboard>();

                foreach (var dBoard in DInputJoystick.DInput.GetDevices(DeviceType.Keyboard, DeviceEnumerationFlags.AttachedOnly))
                {
                    DInputKeyboard board = new DInputKeyboard(DInputJoystick.DInput,dBoard);

                    BoardList.Add(board.GUID, board);
                }

            }

            return BoardList.Values.ToArray();
        
}
    }
}
