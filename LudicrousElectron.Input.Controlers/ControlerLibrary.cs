using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudicrousElectron.Input.Controlers
{
	public static class ControlerLibrary
	{
		public static bool UseDInput = true;
        public static bool UseXInput = true;

        public static bool FallbackToExternal = true;

		internal static List<Joystick> StickList = null;
        internal static List<Keyboard> KeyboardList = null;

        internal static List<Device> DeviceList = null;

        public delegate List<Device> GetExternalDevicesCallback();

        public static GetExternalDevicesCallback GetExternalDevices;


        internal static void SetupDevices()
        {
            StickList = new List<Joystick>();
            KeyboardList = new List<Keyboard>();
            DeviceList = new List<Device>();

            if (UseDInput)
            {
                StickList.AddRange(DInputJoystick.GetDevices());
                KeyboardList.AddRange(DInputKeyboard.GetDevices());
            }

            if (UseXInput)
                StickList.AddRange(XInputJoystick.GetDevices());
      
            DeviceList.AddRange(StickList.ToArray());
            DeviceList.AddRange(KeyboardList.ToArray());

            if (GetExternalDevices != null && (!FallbackToExternal || DeviceList.Count == 0) )
            {
                try
                {
                    List<Device> externalDevices = GetExternalDevices();
                    foreach (var device in externalDevices)
                    {
                        if (device as Joystick != null)
                            StickList.Add(device as Joystick);
                        else if (device as Keyboard != null)
                            KeyboardList.Add(device as Keyboard);

                        DeviceList.Add(device);
                    }
                }
                catch (Exception)
                {

                }
            }
            
        }

        public static IEnumerable<Joystick> Sticks
		{
			get
			{
                if (DeviceList == null)
                    SetupDevices();

				return StickList;
			}
		}

        public static Keyboard DefaultKeyboard
        {
            get
            {
                if (DeviceList == null)
                    SetupDevices();

                return KeyboardList.Count == 0 ? null : KeyboardList[0];
            }
        }

        public static IEnumerable<Keyboard> Kebyboards
        {
            get
            {
                if (DeviceList == null)
                    SetupDevices();

                return KeyboardList;
            }
        }

        public static IEnumerable<Device> Devices
        {
            get
            {
                if (DeviceList == null)
                    SetupDevices();

                return DeviceList;
            }
        }
    }
}
