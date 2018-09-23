using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using SharpDX.XInput;

namespace LudicrousElectron.Input.Controlers
{
    public class XInputJoystick : Joystick
    {
        protected Controller XIController = null;

        public override string DeviceName { get => "XBox Controller"; }
        public override string GUID { get => "XINPUT_CONTROLLER_" + XIController.UserIndex.ToString(); protected set { return; } }

		public override StickTypes StickType => StickTypes.Gamepad;

		public class XInputAxis : Axis
        {
            public bool Left = false;
            public bool X = false;

            protected double LastValue = 0;

            public XInputAxis(bool left, bool x, string name)
            {
                Left = left;
                X = x;

                Name = name;

                NominalAxis = X ? NomialAxes.X : NomialAxes.Y;
                MotionMode = MotionModes.Centered;
            }

            protected void SetValue(short val)
            {
                Value = val / (double)short.MaxValue;

                if (Value != LastValue)
                    CallMoved();

                LastValue = Value;
            }

            public virtual void Update(Gamepad pad)
            {
                if (Left)
                    SetValue(X ? pad.LeftThumbX : pad.LeftThumbY);
                else
                    SetValue(X ? pad.RightThumbX : pad.RightThumbY);
            }
        }

        public class XInputTrigger : XInputAxis
        {
            public XInputTrigger(bool left, string name) : base(left, false, name)
            {
                NominalAxis = NomialAxes.Other;
                MotionMode = MotionModes.Absolute;
            }

            public override void Update(Gamepad pad)
            {
                if (Left)
                    SetValue((short)(pad.LeftTrigger * byte.MaxValue));
                else
                    SetValue((short)(pad.RightTrigger * byte.MaxValue));
            }
        }

        public List<XInputAxis> AxisList = new List<XInputAxis>();
        public override IEnumerable<Axis> Axes => AxisList; 

        public class XInputButton : Button
        {
            public GamepadButtonFlags ButtonFlag = GamepadButtonFlags.A;

            protected double LastValue = 0;

            public XInputButton(GamepadButtonFlags f)
            {
                ButtonFlag = f;
                IsAnalog = false;
                Index = ButtonFlag == 0 ? 0 : (int)Math.Pow(2, 1.0 / (int)ButtonFlag);
                Name = f.ToString();
            }

            protected void SetValue(bool value)
            {
                Value = value ? 1 : 0;

                if (LastValue != Value)
                    CallPressed();

                LastValue = Value;
            }

            public virtual void Update(Gamepad pad)
            {
                SetValue(pad.Buttons.HasFlag(ButtonFlag));
            }
        }

        public List<XInputButton> ButtonList = new List<XInputButton>();
        public override IEnumerable<Button> Buttons => ButtonList;


        public override IEnumerable<Hat> Hats => new Hat[0];


        protected XInputJoystick(Controller ctl)
        {
            XIController = ctl;

            AxisList.Add(new XInputAxis(true, true, "Left X"));
            AxisList.Add(new XInputAxis(true, false, "Left Y"));

            AxisList.Add(new XInputAxis(false, true, "Right X"));
            AxisList.Add(new XInputAxis(false, false, "Right Y"));

            AxisList.Add(new XInputTrigger(true, "Left Trigger"));
            AxisList.Add(new XInputTrigger(false, "Right Trigger"));

            foreach (GamepadButtonFlags b in Enum.GetValues(typeof(GamepadButtonFlags)))
                ButtonList.Add(new XInputButton(b));
        }

        public override void UpdateState()
        {
            var state = XIController.GetState();

            foreach (var axis in AxisList)
                axis.Update(state.Gamepad);

            foreach (var button in ButtonList)
                button.Update(state.Gamepad);
        }

        internal static Dictionary<string, XInputJoystick> StickList = null;

        internal static void CheckControler(Controller controller)
        {
            if (!controller.IsConnected)
                return;

            XInputJoystick stick = new XInputJoystick(controller);
            StickList.Add(stick.GUID, stick);
        }

        internal static XInputJoystick[] GetDevices()
        {
            if (StickList == null)
            {
                StickList = new Dictionary<string, XInputJoystick>();

                CheckControler(new Controller(UserIndex.One));
                CheckControler(new Controller(UserIndex.Two));
                CheckControler(new Controller(UserIndex.Three));
                CheckControler(new Controller(UserIndex.Four));
            }

            return StickList.Values.ToArray();
        }
    }
}
