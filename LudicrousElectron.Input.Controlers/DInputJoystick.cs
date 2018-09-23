using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX.DirectInput;

namespace LudicrousElectron.Input.Controlers
{
	internal class DInputJoystick : Joystick
	{
		internal static DirectInput DInput = null;

		public override string DeviceName { get => Device.InstanceName; }
		public override string GUID { get => Device.InstanceGuid.ToString(); protected set { return; } }

		protected DeviceInstance Device = null;

		protected SharpDX.DirectInput.Joystick DIStick = null;

		protected bool Aquired = false;

		public override StickTypes StickType => Device.Type.HasFlag(DeviceType.FirstPerson)  ? StickTypes.Throttle : StickTypes.Joystick;

		protected enum ShiftedAxisType
		{
			X = 0,
			Y = 4,
			RotationZ = 8,
			Sliders0 = 12,
			Sliders1 = 16,
			RotationX = 20,
			RotationY = 24,
			Unknown1 = 28,
			PointOfViewControllers0 = 32,
			PointOfViewControllers1 = 36,
			PointOfViewControllers2 = 40,
			PointOfViewControllers3 = 44,
		}

		public interface IDControl
		{
			JoystickOffset Offset { get; set; }
			void PollUpdate(JoystickState state);
			void BufferUpdate(JoystickUpdate update);
		}

		public class DInputAxis : Axis, IDControl
		{
			public JoystickOffset Offset { get; set; } = JoystickOffset.X;

			protected double LastValue = 0;

			public DInputAxis(JoystickOffset offset)
			{
				Name = offset.ToString();
				Offset = offset;

				switch (Offset)
				{
					case JoystickOffset.X:
						NominalAxis = NomialAxes.X;
						break;
					case JoystickOffset.Y:
						NominalAxis = NomialAxes.Y;
						break;
					case JoystickOffset.Z:
						NominalAxis = NomialAxes.Z;
						break;
					case JoystickOffset.RotationX:
						IsRotary = true;
						NominalAxis = NomialAxes.X;
						break;
					case JoystickOffset.RotationY:
						IsRotary = true;
						NominalAxis = NomialAxes.Y;
						break;
					case JoystickOffset.RotationZ:
						IsRotary = true;
						NominalAxis = NomialAxes.Z;
						break;
					case JoystickOffset.Sliders0:
						NominalAxis = NomialAxes.Other;
						MotionMode = MotionModes.AbsoluteParametric;
						break;
					case JoystickOffset.Sliders1:
						NominalAxis = NomialAxes.Other;
						MotionMode = MotionModes.AbsoluteParametric;
						break;
				}
			}

			public void PollUpdate(JoystickState state)
			{
				switch (Offset)
				{
					case JoystickOffset.X:
						SetValue(state.X);
						break;
					case JoystickOffset.Y:
						SetValue(state.Y);
						break;
					case JoystickOffset.Z:
						SetValue(state.Z);
						break;
					case JoystickOffset.RotationX:
						SetValue(state.RotationX);
						break;
					case JoystickOffset.RotationY:
						SetValue(state.RotationY);
						break;
					case JoystickOffset.RotationZ:
						SetValue(state.RotationZ);
						break;
					case JoystickOffset.Sliders0:
						SetValue(state.Sliders[0]);
						break;
					case JoystickOffset.Sliders1:
						SetValue(state.Sliders[1]);
						break;
				}
			}

			protected void SetValue(int value)
			{
				if (MotionMode == MotionModes.Absolute)
					Value = value;
				else if (MotionMode == MotionModes.AbsoluteParametric)
					Value = value/ (double)UInt16.MaxValue;
				else
					Value = (value - Int16.MaxValue) / (double)Int16.MaxValue;

				if (DefaultGranularity > 0)
					Value = Math.Round((Value * (1.0f / DefaultGranularity)) + 0.5f) * DefaultGranularity;

				if (LastValue != Value)
					CallMoved();

				LastValue = value;
			}

			public void BufferUpdate(JoystickUpdate update)
			{
				SetValue(update.Value);
			}
		}

		protected List<DInputAxis> DAxes = new List<DInputAxis>();
		public override IEnumerable<Axis> Axes => DAxes;


		public class DButton : Button, IDControl
		{
			public JoystickOffset Offset { get; set; } = JoystickOffset.Buttons0;

			protected double LastValue = 0;

			public DButton(JoystickOffset offset)
			{
				Offset = offset;
				IsAnalog = false;
				Index = (offset - JoystickOffset.Buttons0);
				Name = NameUtils.GetButtonLabel(Index);
			}
			public void PollUpdate(JoystickState state)
			{
				SetValue(state.Buttons[Index]);
			}

			protected void SetValue(bool value)
			{
				Value = value ? 1 : 0;

				if (LastValue != Value)
					CallPressed();

				LastValue = Value;
			}

			public void BufferUpdate(JoystickUpdate update)
			{
				SetValue(update.Value > 0);
			}
		}

		protected List<DButton> DButtons = new List<DButton>();
		public override IEnumerable<Button> Buttons => DButtons;

		public class DHat : Hat, IDControl
		{
			public JoystickOffset Offset { get; set; } = JoystickOffset.PointOfViewControllers0;

			protected double LastAngle = 0;
			protected bool LastEngaged = false;

			protected int Index = 0;

			public DHat(JoystickOffset offset)
			{
				Offset = offset;
				switch(offset)
				{
					case JoystickOffset.PointOfViewControllers0:
						Index = 0;
						break;

					case JoystickOffset.PointOfViewControllers1:
						Index = 1;
						break;

					case JoystickOffset.PointOfViewControllers2:
						Index = 2;
						break;

					case JoystickOffset.PointOfViewControllers3:
						Index = 3;
						break;
				}
	
				Name = "POV" + (Index + 1).ToString();
			}

			public void PollUpdate(JoystickState state)
			{
				SetValue(state.PointOfViewControllers[Index]);
			}

			protected void SetValue(int value)
			{
				Angle = 0;
				if (value == 0)
					Engaged = false;
				else
				{
					Engaged = true;
					Angle = (value / 100.0);
				}

				if (Engaged != LastEngaged || (Engaged && LastAngle != Angle))
					CallMoved();

				LastAngle = Angle;
				LastEngaged = Engaged;
			}

			public void BufferUpdate(JoystickUpdate update)
			{
				SetValue(update.Value);
			}
		}

		protected List<DHat> DHats = new List<DHat>();
		public override IEnumerable<Hat> Hats => DHats;

		protected Dictionary<JoystickOffset, IDControl> Controls = new Dictionary<JoystickOffset, IDControl>();

		public static int RemapAxisOffset(string name, int offset)
		{
			string nameLower = name.ToLowerInvariant();

			if (nameLower == "z rotation" && offset == (int)JoystickOffset.Z)
				return (int)JoystickOffset.RotationX;

			return offset;
		}

		public DInputJoystick(DirectInput di, DeviceInstance instance)
		{
			Device = instance;
			GUID = instance.InstanceGuid.ToString();

			DIStick = new SharpDX.DirectInput.Joystick(di, instance.InstanceGuid);
			DIStick.Properties.BufferSize = 2048;

			List<DeviceObjectInstance> axes = new List<DeviceObjectInstance>();

			foreach (var item in DIStick.GetObjects(DeviceObjectTypeFlags.Axis | DeviceObjectTypeFlags.Button | DeviceObjectTypeFlags.PointOfViewController))
			{
				try
				{
					var info = DIStick.GetObjectPropertiesById(item.ObjectId);

					item.Offset = RemapAxisOffset(item.Name, item.Offset);

					if (item.ObjectId.Flags.HasFlag(DeviceObjectTypeFlags.Axis) || item.ObjectId.Flags.HasFlag(DeviceObjectTypeFlags.RelativeAxis) || item.ObjectId.Flags.HasFlag(DeviceObjectTypeFlags.AbsoluteAxis))
					{
						axes.Add(item);
						var axis = new DInputAxis(JoystickOffset.X + item.Offset);// + (item.ObjectId.InstanceNumber * 4));
						axis.Name = /*item.Name + "(" + */ axis.Offset.ToString()/* + ")"*/;
						DAxes.Add(axis);
					}
					else if (item.ObjectId.Flags.HasFlag(DeviceObjectTypeFlags.PushButton) || item.ObjectId.Flags.HasFlag(DeviceObjectTypeFlags.Button) || item.ObjectId.Flags.HasFlag(DeviceObjectTypeFlags.ToggleButton))
					{
						DButtons.Add(new DButton(JoystickOffset.Buttons0 + item.ObjectId.InstanceNumber));
					}
					else if (item.ObjectId.Flags.HasFlag(DeviceObjectTypeFlags.PointOfViewController))
					{
						DHats.Add(new DHat(JoystickOffset.PointOfViewControllers0 + (item.ObjectId.InstanceNumber * 4)));
					}
					else
					{

					}
				}
				catch (Exception)
				{

				}
			}

			foreach (var axis in DAxes)
				Controls.Add(axis.Offset,axis);
			foreach (var button in DButtons)
				Controls.Add(button.Offset, button);
			foreach (var hat in DHats)
				Controls.Add(hat.Offset, hat);
		}

		public override void StartPoll()
		{
 			DIStick.Acquire();
 			Aquired = true;
		}

		public override void EndPoll()
		{
 			Aquired = false;
 			DIStick.Unacquire();
 		}

		public override void UpdateState()
		{
			List<double> axes = new List<double>();
			if (Aquired)
			{
				JoystickState state = new JoystickState();
				DIStick.GetCurrentState(ref state);

				foreach (var axis in DAxes)
					axis.PollUpdate(state);
				foreach (var button in DButtons)
					button.PollUpdate(state);
				foreach (var hat in DHats)
					hat.PollUpdate(state);
			}
			else
			{
				DIStick.Poll();
				foreach (JoystickUpdate item in DIStick.GetBufferedData())
				{
					if (Controls.ContainsKey(item.Offset))
					{
						Controls[item.Offset].BufferUpdate(item);
						CallCTLChange(Controls[item.Offset] as Control);
					}
					else
					{
						int i = (int)item.Offset;
					}
				}
			}
		}


		internal static Dictionary<string, DInputJoystick> StickList = null;

		internal static DInputJoystick[] GetDevices()
		{
			if (DInput == null)
				DInput = new DirectInput();

			if (StickList == null)
			{
				StickList = new Dictionary<string, DInputJoystick>();

				foreach (var dStick in DInput.GetDevices(DeviceType.Joystick, DeviceEnumerationFlags.ForceFeedback))
				{
					DInputJoystick stick = new DInputJoystick(DInput, dStick);
				
					StickList.Add(stick.GUID, stick);
				}

				foreach (var dStick in DInput.GetDevices(DeviceType.Flight, DeviceEnumerationFlags.AllDevices))
				{
					if (StickList.ContainsKey(dStick.InstanceGuid.ToString()))
						continue;

					DInputJoystick stick = new DInputJoystick(DInput, dStick);

					StickList.Add(stick.GUID, stick);
				}

				foreach (var dStick in DInput.GetDevices(DeviceType.FirstPerson, DeviceEnumerationFlags.AllDevices))
				{
					if (StickList.ContainsKey(dStick.InstanceGuid.ToString()))
						continue;
				
				       DInputJoystick stick = new DInputJoystick(DInput, dStick);
				
					StickList.Add(stick.GUID, stick);
				}

				foreach (var dStick in DInput.GetDevices(DeviceType.Joystick, DeviceEnumerationFlags.AllDevices))
				{
					if (StickList.ContainsKey(dStick.InstanceGuid.ToString()))
						continue;

					DInputJoystick stick = new DInputJoystick(DInput, dStick);

					StickList.Add(stick.GUID, stick);
				}
			}

			return StickList.Values.ToArray();
		}
	}
}
