using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml;
using System.Xml.Serialization;

namespace LudicrousElectron.Input.Controlers
{
	public enum InverseTypes
	{
		Normal,
		Inverted,
	}

	public enum NegationTypes
	{
		Normal,
		Negated,
	}

	public enum AxisSignTypes
	{
		Normal,
		Unsigned,
	}

	public class ControlMapping
	{
		public class Command : EventArgs
		{
			public string FunctionName = string.Empty;

			protected bool LastValB = false;
			protected double LastValD = 0.0;

			public virtual double GetValueD() { return LastValD; }
			public virtual bool GetValueB() { return LastValB; }

			public event EventHandler<Command> Changed = null;

			public List<Input> Inputs = new List<Input>();

			protected void CallChanged()
			{
				Changed?.Invoke(this, this);
			}

			internal virtual void Update()
			{
				foreach (var ctl in Inputs)
				{
					if (ctl != null && ctl.OnUpdate())
					{
						LastValB = ctl.GetValueB();
						LastValD = ctl.GetValueD();
						Changed?.Invoke(this, this);
						return;
					}
				}
			}

			internal virtual void Setup()
			{
				foreach (Input ctl in Inputs)
				{
					ctl.Setup();
				}
			}

			[XmlInclude(typeof(Axis))]
			[XmlInclude(typeof(Button))]
			public class Input
			{
				public string DeviceGUID = string.Empty;
				public string DeviceName = string.Empty;

				public Command Command = null;

				public enum BoundInputTypes
				{
					Axis,
					Button,
					Hat,
					Key,
				}
				public BoundInputTypes BoundInput = BoundInputTypes.Axis;
				public string BoundInputName = string.Empty;

				internal virtual void Setup()
				{

				}

				internal virtual bool OnUpdate()
				{
					return false;
				}

				public virtual double GetValueD() { return 0; }
				public virtual bool GetValueB() { return false; }
				internal Device BoundDevice = null;
			}
		}

		public class Axis : Command
		{
			internal override void Update()
			{
				List<Tuple<double, AxisInput>> checkedInputs = new List<Tuple<double, AxisInput>>();
				double max = double.MinValue;

				if (Inputs.Count == 1)
				{
					if (!Inputs[0].OnUpdate())
						return;

					LastValD = Inputs[0].GetValueD();
					LastValB = Inputs[0].GetValueB();
					CallChanged();
					return;
				}
				foreach (var ctl in Inputs)
				{
					if (ctl as AxisInput != null && ctl.OnUpdate())
					{
						double val = ctl.GetValueD();
						if (Math.Abs(val) > max)
						{
							max = Math.Abs(val);
							checkedInputs.Add(new Tuple<double, AxisInput>(val, ctl as AxisInput));
						}
					}
				}

				if (checkedInputs.Count != 0)
				{
					double total = 0;
					foreach (var i in checkedInputs)
						total += i.Item1;

					LastValD = total / checkedInputs.Count;
					LastValB = LastValD != 0;
					CallChanged();
				}
			}

			public class AxisInput : Command.Input
			{
				internal Joystick.Axis BoundAxis = null;

				internal Joystick.Hat BoundHat = null;
				internal bool HatVertAxis = true;

				internal Device.Control PositiveButton = null;
				internal Device.Control NegativeButton = null;

				public InverseTypes Inversion = InverseTypes.Normal;
				public NegationTypes Negation = NegationTypes.Normal;
				public AxisSignTypes Sign = AxisSignTypes.Normal;

				protected int PositiveIndex = 0;
				protected int NegativeIndex = 0;

				protected double LastValue = 0;

				internal override void Setup()
				{
					if (BoundDevice == null)
						return;

					if (BoundInput == BoundInputTypes.Axis)
					{
						Joystick stick = BoundDevice as Joystick;
						if (stick == null)
							return;

						foreach (var axis in stick.Axes)
						{
							if (axis.Name.ToLowerInvariant() == BoundInputName.ToLowerInvariant())
							{
								BoundAxis = axis;
								break;
							}
						}
					}
					else if (BoundInput == BoundInputTypes.Button || BoundInput == BoundInputTypes.Key)
					{
						string[] parts = BoundInputName.Split(":".ToCharArray());
						if (BoundInput == BoundInputTypes.Key)
						{
							Keyboard board = BoundDevice as Keyboard;
							if (board == null)
								return;

							PositiveButton = board.Keys;
							PositiveIndex = 0;
							if (parts[0] != string.Empty)
							{
								Keyboard.KeyCodes code = Keyboard.KeyCodes.Unknown;
								if (Enum.TryParse(parts[0], out code))
									PositiveIndex = (int)code;
							}

							if (parts.Length > 1 || parts[1] != string.Empty)
							{
								NegativeButton = board.Keys;
								Keyboard.KeyCodes code = Keyboard.KeyCodes.Unknown;
								if (Enum.TryParse(parts[0], out code))
									NegativeIndex = (int)code;
							}
						}
						else
						{
							Joystick stick = BoundDevice as Joystick;
							if (stick == null)
								return;

							foreach (var button in stick.Buttons)
							{
								if (button.Name.ToLowerInvariant() == parts[0].ToLowerInvariant())
									PositiveButton = button;
								else if (parts.Length > 1 && button.Name.ToLowerInvariant() == parts[1].ToLowerInvariant())
									NegativeButton = button;
							}
						}
					}
					else if (BoundInput == BoundInputTypes.Hat)
					{
						Joystick stick = BoundDevice as Joystick;
						if (stick == null)
							return;

						string[] parts = BoundInputName.Split(":".ToCharArray());
						HatVertAxis = parts.Length > 1 && parts[1].ToUpper() == "V";
						string name = parts[0].ToLowerInvariant();

						foreach (var hat in stick.Hats)
						{
							if (hat.Name.ToLowerInvariant() == name)
							{
								PositiveButton = hat;
								break;
							}
						}
					}
				}

				internal override bool OnUpdate()
				{
					double value = 0;

					if (BoundAxis != null)
						value = BoundAxis.GetValue(0);
					else if (PositiveButton != null)
					{
						if (BoundInput == BoundInputTypes.Hat)
						{
							Joystick.Hat hat = PositiveButton as Joystick.Hat;
							if (HatVertAxis)
							{
								if (hat.OrdinalIsPressed(Joystick.Hat.Ordinals.North) || hat.OrdinalIsPressed(Joystick.Hat.Ordinals.NorthEast) || hat.OrdinalIsPressed(Joystick.Hat.Ordinals.NorthWest))
									value = 1;
								else if (hat.OrdinalIsPressed(Joystick.Hat.Ordinals.South) || hat.OrdinalIsPressed(Joystick.Hat.Ordinals.SouthEast) || hat.OrdinalIsPressed(Joystick.Hat.Ordinals.SouthWest))
									value = -1;
							}
							else
							{
								if (hat.OrdinalIsPressed(Joystick.Hat.Ordinals.East) || hat.OrdinalIsPressed(Joystick.Hat.Ordinals.NorthEast) || hat.OrdinalIsPressed(Joystick.Hat.Ordinals.SouthEast))
									value = 1;
								else if (hat.OrdinalIsPressed(Joystick.Hat.Ordinals.West) || hat.OrdinalIsPressed(Joystick.Hat.Ordinals.SouthWest) || hat.OrdinalIsPressed(Joystick.Hat.Ordinals.NorthWest))
									value = -1;
							}
						}
						else
						{
							value = PositiveButton.GetValue(PositiveIndex);
							if (NegativeButton != null)
								value = PositiveButton.GetValue(NegativeIndex) > 0 ? (value > 0 ? 0 : -1) : value;
						}
					}

					if (Sign == AxisSignTypes.Unsigned)
					{
						value = (value + 1.0) * 0.5;

						if (Inversion == InverseTypes.Inverted)
							value = 1.0 - value;

						if (value < 0)
							value = 0;
						if (value > 1)
							value = 1;
					}

					if (Negation == NegationTypes.Negated)
						value = -1 * value;

					bool changed = value != LastValue;
					LastValue = value;
					return changed;
				}

				public override double GetValueD() { return LastValue; }
				public override bool GetValueB() { return LastValue != 0; }
			}
		}

		public class Button : Command
		{
			public class ButtonInput : Command.Input
			{
				internal Device.Control ActualControl = null;
				internal int BoundIndex = 0;
				public double TriggerValue = 0;

				protected bool LastValue = false;

				internal override void Setup()
				{
					if (BoundDevice == null)
						return;

					if (BoundInput == BoundInputTypes.Button)
					{
						Joystick stick = BoundDevice as Joystick;
						if (stick == null)
							return;

						foreach (var button in stick.Buttons)
						{
							if (button.Name.ToLowerInvariant() == BoundInputName.ToLowerInvariant())
							{
								ActualControl = button;
								break;
							}
						}
					}
					else if (BoundInput == BoundInputTypes.Hat)
					{
						Joystick stick = BoundDevice as Joystick;
						if (stick == null)
							return;

						string[] parts = BoundInputName.ToLowerInvariant().Split(":".ToCharArray());
						string name = parts[0];
						TriggerValue = 0.5;
						if (parts.Length > 1)
							double.TryParse(parts[1], out TriggerValue);

						foreach (var hat in stick.Hats)
						{
							if (hat.Name.ToLowerInvariant() == name)
							{
								ActualControl = hat;
								break;
							}
						}
					}
					else if (BoundInput == BoundInputTypes.Axis)
					{
						Joystick stick = BoundDevice as Joystick;
						if (stick == null)
							return;

						string[] parts = BoundInputName.ToLowerInvariant().Split(":".ToCharArray());
						string name = parts[0];
						TriggerValue = 1;
						if (parts.Length > 1)
							double.TryParse(parts[1], out TriggerValue);

						foreach (var axis in stick.Axes)
						{
							if (axis.Name.ToLowerInvariant() == name)
							{
								ActualControl = axis;
								break;
							}
						}
					}
					else if (BoundInput == BoundInputTypes.Key)
					{
						Keyboard board = BoundDevice as Keyboard;
						if (board == null)
							return;

						Keyboard.KeyCodes code;
						if (Enum.TryParse(BoundInputName, out code))
							BoundIndex = (int)code;
					}
				}

				internal override bool OnUpdate()
				{
					if (ActualControl == null)
						return false;

					bool value = ActualControl.GetValue(BoundIndex) > TriggerValue;
					bool changed = value != LastValue;
					LastValue = value;
					return changed;
				}

				public override double GetValueD() { return LastValue ? 1 : 0; }
				public override bool GetValueB() { return LastValue; }
			}
		}

		public List<Command> Commands = new List<Command>();

		public void Aquire()
		{
			foreach (var device in UsedDevices)
				device.StartPoll();
		}

		public void Release()
		{
			foreach (var device in UsedDevices)
				device.EndPoll();
		}

		public void Update()
		{
			foreach (var device in UsedDevices)
				device.UpdateState();

			foreach (var input in Commands)
				input.Update();
		}

		protected List<Device> UsedDevices = new List<Device>();

		public class DeviceNotFoundEventArguments : EventArgs
		{
			public string DeviceName = string.Empty;
			public string DeviceGUID = string.Empty;

			public Joystick DeviceToUse = null;

			public DeviceNotFoundEventArguments(string n, string g)
			{
				DeviceName = n;
				DeviceGUID = g;
			}
		}

		public event EventHandler<DeviceNotFoundEventArguments> DeviceNotFound = null;

		protected void BindInput(Command cmd, Command.Input input, Device device = null, Device.Control control = null)
		{
			cmd.Inputs.Add(input);

			if (device != null && control != null)
			{
				input.DeviceGUID = device.GUID;
				input.DeviceName = device.DeviceName;

				input.BoundInputName = control.Name;

				if (control as Joystick.Axis != null)
					input.BoundInput = Command.Input.BoundInputTypes.Axis;
				else if (control as Joystick.Button != null)
					input.BoundInput = Command.Input.BoundInputTypes.Button;
				else if (control as Joystick.Hat != null)
					input.BoundInput = Command.Input.BoundInputTypes.Hat;
				else if (device as Keyboard != null)
					input.BoundInput = Command.Input.BoundInputTypes.Key;
			}

			input.Command = cmd;
		}

		public Axis AddAxisCommand(string name, Device device = null, Device.Control control = null, AxisSignTypes sign = AxisSignTypes.Normal, InverseTypes invert = InverseTypes.Normal, NegationTypes negate = NegationTypes.Normal)
		{
			Command cmd = Commands.Find(x => x.FunctionName == name);
			if (cmd == null)
			{
				cmd = new Axis();
				cmd.FunctionName = name;

				Commands.Add(cmd);
			}

			if (cmd as Axis != null)
			{
				Axis.AxisInput axis = new Axis.AxisInput();
				BindInput(cmd, axis, device, control);

				axis.Sign = sign;
				axis.Negation = negate;
				axis.Inversion = (sign == AxisSignTypes.Unsigned) ? invert : InverseTypes.Normal;
			}

			return cmd as Axis;
		}

		public Axis AddAxisCommand(string name, Device device, Device.Control posControl, Device.Control negControl)
		{
			Command cmd = Commands.Find(x => x.FunctionName == name);
			if (cmd == null)
			{
				cmd = new Axis();
				cmd.FunctionName = name;

				Commands.Add(cmd);
			}

			if (cmd as Axis != null)
			{
				Axis.AxisInput axis = new Axis.AxisInput();
				axis.NegativeButton = negControl;
				BindInput(cmd, axis, device, posControl);
			}

			return cmd as Axis;
		}

        public Axis AddAxisKeyboardCommand(string name, Keyboard device, Keyboard.KeyCodes posKey, Keyboard.KeyCodes negKey = Keyboard.KeyCodes.Unknown)
        {
            Command cmd = Commands.Find(x => x.FunctionName == name);
            if (cmd == null)
            {
                cmd = new Axis();
                cmd.FunctionName = name;

                Commands.Add(cmd);
            }


            if (device == null)
                device = ControlerLibrary.DefaultKeyboard;

            if (device != null && cmd as Axis != null)
            {
                Axis.AxisInput axis = new Axis.AxisInput();
                axis.NegativeButton = device.Keys;
                BindInput(cmd, axis, device, device.Keys);

                if (negKey == Keyboard.KeyCodes.Unknown)
                    axis.BoundInputName = posKey.ToString();
                else
                    axis.BoundInputName = posKey.ToString() + ":" + negKey.ToString();
            }

            return cmd as Axis;
        }

        public Button AddButtonCommand(string name, Device device = null, Device.Control control = null)
		{
			Command cmd = Commands.Find(x => x.FunctionName == name);
			if (cmd == null)
			{
				cmd = new Button();
				cmd.FunctionName = name;

				Commands.Add(cmd);
			}

			if (cmd as Button != null)
			{
				Button.ButtonInput button = new Button.ButtonInput();
				BindInput(cmd, button, device, control);
			}

			return cmd as Button;
		}

		public void RemoveCommand(string name)
		{
			Command cmd = Commands.Find(x => x.FunctionName == name);
			if (cmd != null)
				Commands.Remove(cmd);
		}

		public void Setup()
		{
			foreach (var device in UsedDevices)
				device.EndPoll();

			UsedDevices.Clear();

			List<Device> devices = new List<Device>(ControlerLibrary.Devices.ToArray());

			Dictionary<string, Device> deviceRemap = new Dictionary<string, Device>();

			foreach (var cmd in Commands)
			{
				foreach (var input in cmd.Inputs)
				{
					Device device = null;

					foreach (var d in devices)
					{
						if (d.GUID == input.DeviceGUID && input.DeviceName == d.DeviceName)
						{
							device = d;
							break;
						}
					}

					if (device == null)
					{
						if (deviceRemap.ContainsKey(input.DeviceGUID))
							device = deviceRemap[input.DeviceGUID];
						else
						{
							DeviceNotFoundEventArguments args = new DeviceNotFoundEventArguments(input.DeviceName, input.DeviceGUID);
							DeviceNotFound?.Invoke(this, args);
							if (args.DeviceToUse != null)
							{
								device = args.DeviceToUse;
								deviceRemap.Add(input.DeviceGUID, device);
							}
							else
								continue;
						}
					}

					if (device != null && !UsedDevices.Contains(device))
						UsedDevices.Add(device);

					input.BoundDevice = device;
				}
				cmd.Setup();
			}
		}
	}
}
