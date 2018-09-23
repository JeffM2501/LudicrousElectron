using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudicrousElectron.Input.Controlers
{
	public class Joystick : Device
    {
		public enum StickTypes
		{
			Generic,
			Joystick,
			Throttle,
			Gamepad,
		}

		public virtual StickTypes StickType { get; } = StickTypes.Generic;

		public class Axis : Control
		{
			public static double DefaultGranularity = 0.001;

			public enum NomialAxes
			{
				X,
				Y,
				Z,
				Other,
			}

			public bool IsRotary = false;
			public NomialAxes NominalAxis = NomialAxes.X;

			public double Value = 0;
			public double DeadZone = 0;

			public enum MotionModes
			{
				Centered,
				Absolute,
				AbsoluteParametric,
			}

            public override double GetValue(int index)
            {
                return Value;
            }

            public MotionModes MotionMode = MotionModes.Centered;

			public event EventHandler<Axis> Moved = null;

			protected void CallMoved()
			{
				Moved?.Invoke(this, this);
			}
		}

		public virtual IEnumerable<Axis> Axes { get; }

		public class Button : Control
		{
			public int Index = 0;

			public bool IsAnalog = false;

			public double Value = 0;
			public bool IsPressed ()
			{
				if (IsAnalog)
					return Value > 0.5;
				else
					return Value > 0;
			}

            public override double GetValue(int index)
            {
                return IsPressed() ? 1 : 0;
            }

            public event EventHandler<Button> Pressed;

			protected void CallPressed()
			{
				Pressed?.Invoke(this, this);
			}
		}
		public virtual IEnumerable<Button> Buttons { get; }

		public class Hat : Control
		{
            public double Angle = 0;
			public bool Engaged = false;

			public enum Ordinals
			{
				North,
				South,
				East,
				West,
				NorthEast,
				NorthWest,
				SouthEast,
				SouthWest,
			}

			protected bool AngleNear(double value)
			{
				double delta = Angle - value;
				return Math.Abs(delta) < 1; 
			}

			public bool OrdinalIsPressed(Ordinals direction)
			{
				if (!Engaged)
					return false;

				switch (direction)
				{
					case Ordinals.North:
						return AngleNear(0);
					case Ordinals.South:
						return AngleNear(180);
					case Ordinals.East:
						return AngleNear(90);
					case Ordinals.West:
						return AngleNear(270);
					case Ordinals.NorthEast:
						return AngleNear(45);
					case Ordinals.NorthWest:
						return AngleNear(240+45);
					case Ordinals.SouthEast:
						return AngleNear(90+45);
					case Ordinals.SouthWest:
						return AngleNear(180+45);
				}

				return false;
			}

			public event EventHandler<Hat> Moved = null;

			protected void CallMoved()
			{
				Moved?.Invoke(this, this);
			}

            public override double GetValue(int index)
            {
                return Engaged ? Angle : double.MinValue;
            }
        }

		public virtual IEnumerable<Hat> Hats { get; }

		public event EventHandler<Control> ControllChanged = null;

		protected void CallCTLChange(Control ctl)
		{
			if (ctl == null)
				return;

			ControllChanged?.Invoke(this, ctl);
		}

		public virtual Axis FindAxisByName(string name)
		{
			foreach (var item in Axes)
			{
				if (item.Name == name)
					return item;
			}

			foreach (var item in Axes)
			{
				if (item.Name.Contains(name))
					return item;
			}

			return null;
		}
		
		public virtual Button FindButtonByName(string name)
		{
			foreach (var item in Buttons)
			{
				if (item.Name == name)
					return item;
			}

			foreach (var item in Buttons)
			{
				if (item.Name.Contains(name))
					return item;
			}

			return null;
		}

		public virtual Hat FindHatByName(string name)
		{
			foreach (var item in Hats)
			{
				if (item.Name == name)
					return item;
			}

			foreach (var item in Hats)
			{
				if (item.Name.Contains(name))
					return item;
			}

			return null;
		}
	}
}
