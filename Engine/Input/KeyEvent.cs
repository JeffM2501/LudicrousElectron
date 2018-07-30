using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Input;

namespace LudicrousElectron.Engine.Input
{
    public class KeyEvent
    {
        public Key Code = Key.Unknown;  //< Code of the key that has been pressed
        public bool Alt = false;        //< Is the Alt key pressed?
        public bool Control = false;    //< Is the Control key pressed?
        public bool Shift = false;      //< Is the Shift key pressed?
        public bool Meta = false;       //< Is the System key pressed?
    }
}
