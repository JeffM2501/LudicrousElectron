using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudicrousElectron.Input.Controlers
{
    public static class ControlMapSetter
    {
        internal static ControlMapping.Command CommandToTest = null;
		internal static ControlMapping.Command.Input InputToUpdate = null;
		internal static bool SetPositiveKeyAxis = false;

        internal static bool Done = false;
		internal static bool AddInput = false;

        public static void BeginSetControl(ControlMapping.Command cmd, bool add = false, bool setPositive = true, ControlMapping.Command.Input inputToUpdate = null)
        {
            Done = false;
			AddInput = add && inputToUpdate == null;
			CommandToTest = cmd;
			InputToUpdate = inputToUpdate;
			SetPositiveKeyAxis = setPositive && (CommandToTest as ControlMapping.Axis) != null;

            foreach (var device in ControlerLibrary.Devices)
            {
                if (device as Keyboard != null)
                    (device as Keyboard).KeyPressed += ControlMapSetter_KeyPressed;
                else if (device as Joystick != null)
                    (device as Joystick).ControllChanged += ControlMapSetter_ControllChanged;
            }
        }

        private static void ControlMapSetter_ControllChanged(object sender, Device.Control e)
        {
            if (Done || CommandToTest == null)
                return;

            Joystick stick = sender as Joystick;
            if (stick == null)
                return;

			ControlMapping.Command.Input iInput = null;

			if (CommandToTest as ControlMapping.Axis != null)
            {
				ControlMapping.Axis.AxisInput aInput = new ControlMapping.Axis.AxisInput();

				if (InputToUpdate != null && InputToUpdate as ControlMapping.Axis.AxisInput != null)
					aInput = InputToUpdate as ControlMapping.Axis.AxisInput;

				if (e as Joystick.Axis != null)
                {
					aInput.DeviceName = stick.DeviceName;
                    aInput.DeviceGUID = stick.GUID;
                    aInput.BoundInputName = e.Name;

                }
                else if (e as Joystick.Button != null)
                {
					aInput.DeviceName = stick.DeviceName;
                    aInput.DeviceGUID = stick.GUID;

                    if (SetPositiveKeyAxis)
                    {
                        if (aInput.BoundInputName.Contains(":"))
							aInput.BoundInputName = e.Name + ":" + aInput.BoundInputName.Substring(aInput.BoundInputName.IndexOf(':') + 1);
                        else
							aInput.BoundInputName = e.Name;
                    }
                    else
                    {
                        if (aInput.BoundInputName.Contains(":"))
							aInput.BoundInputName = aInput.BoundInputName.Substring(0, aInput.BoundInputName.IndexOf(':') - 1);
						aInput.BoundInputName = aInput.BoundInputName + ":" + e.Name;
                    }
                }
                else if (e as Joystick.Hat != null)
                {
					aInput.DeviceName = stick.DeviceName;
                    aInput.DeviceGUID = stick.GUID;
                    aInput.BoundInputName = e.Name;
                    Joystick.Hat hat = e as Joystick.Hat;

                    if (hat.OrdinalIsPressed(Joystick.Hat.Ordinals.North) || hat.OrdinalIsPressed(Joystick.Hat.Ordinals.South))
						aInput.BoundInputName += ":V";
                    else
						aInput.BoundInputName += ":H";
                }

				iInput = aInput;
			}
            else if (CommandToTest as ControlMapping.Button != null)
            {
				ControlMapping.Button.ButtonInput bInput = new ControlMapping.Button.ButtonInput();

				if (InputToUpdate != null && InputToUpdate as ControlMapping.Button.ButtonInput != null)
					bInput = InputToUpdate as ControlMapping.Button.ButtonInput;

				bInput.DeviceName = stick.DeviceName;
                bInput.DeviceGUID = stick.GUID;
                bInput.BoundInputName = e.Name;
                bInput.TriggerValue = e.GetValue(0) > 0 ? 1 : -1;

                if (e as Joystick.Hat != null)
					bInput.TriggerValue = e.GetValue(0);

				iInput = bInput;
			}

			if (AddInput && iInput != null)
				CommandToTest.Inputs.Add(iInput);

            Done = true;
        }

        private static void ControlMapSetter_KeyPressed(object sender, Keyboard.KeyEventArgs e)
        {
            if (Done || CommandToTest == null)
                return;

            Keyboard board = sender as Keyboard;
            if (board == null)
                return;

			ControlMapping.Command.Input iInput = null;

			if (CommandToTest as ControlMapping.Axis != null)
            {
				ControlMapping.Axis.AxisInput aInput = new ControlMapping.Axis.AxisInput();

				if (InputToUpdate != null && InputToUpdate as ControlMapping.Axis.AxisInput != null)
					aInput = InputToUpdate as ControlMapping.Axis.AxisInput;

				aInput.BoundInput = ControlMapping.Command.Input.BoundInputTypes.Key;
                if (!SetPositiveKeyAxis && aInput.DeviceName != string.Empty && aInput.DeviceName != board.DeviceName)
					return;

				aInput.DeviceName = board.DeviceName;
				aInput.DeviceGUID = board.GUID;

                if (SetPositiveKeyAxis)
                {
                    if (aInput.BoundInputName.Contains(":"))
						aInput.BoundInputName = e.KeyCode.ToString() + ":" + aInput.BoundInputName.Substring(aInput.BoundInputName.IndexOf(':') + 1);
                    else
						aInput.BoundInputName = e.KeyCode.ToString();
                }
                else
                {
                    if (aInput.BoundInputName.Contains(":"))
						aInput.BoundInputName = aInput.BoundInputName.Substring(0, aInput.BoundInputName.IndexOf(':') - 1);
					aInput.BoundInputName = aInput.BoundInputName + ":" + e.KeyCode.ToString();
                }
            }
            else if (CommandToTest as ControlMapping.Button != null)
            {
				ControlMapping.Button.ButtonInput bInput = new ControlMapping.Button.ButtonInput();

				if (InputToUpdate != null && InputToUpdate as ControlMapping.Button.ButtonInput != null)
					bInput = InputToUpdate as ControlMapping.Button.ButtonInput;

				bInput.DeviceName = board.DeviceName;
				bInput.DeviceGUID = board.GUID;
				bInput.BoundInputName = e.KeyCode.ToString();
            }
            else
                return;

			if (AddInput && iInput != null)
				CommandToTest.Inputs.Add(iInput);

			Done = true;
        }

        public static bool UpdateSetControl()
        {
            if (CommandToTest == null)
                return true;

            foreach (var device in ControlerLibrary.Devices)
                device.UpdateState();

            if (Done)
                CancelSetControl();

            return Done;
        }

        public static void CancelSetControl()
        {
			CommandToTest = null;
            foreach (var device in ControlerLibrary.Devices)
            {
                if (device as Keyboard != null)
                    (device as Keyboard).KeyPressed -= ControlMapSetter_KeyPressed;
                else if (device as Joystick != null)
                    (device as Joystick).ControllChanged -= ControlMapSetter_ControllChanged;
            }
        }
    }
}
