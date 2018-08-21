using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Input;

namespace LudicrousElectron.Engine.Input
{
    public class KeyEvent : IEquatable<KeyEvent>
    {
        public Key Code = Key.Unknown;  //< Code of the key that has been pressed
        public bool Alt = false;        //< Is the Alt key pressed?
        public bool Control = false;    //< Is the Control key pressed?
        public bool Shift = false;      //< Is the Shift key pressed?
        public bool Meta = false;       //< Is the System key pressed?

        public static bool operator == (KeyEvent lhs, KeyEvent rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(KeyEvent lhs, KeyEvent rhs)
        {
            return !lhs.Equals(rhs);
        }

        public override bool Equals(object obj)
        {
            if (obj as KeyEvent == null)
                return false;

            KeyEvent other = obj as KeyEvent;

            return (Code == other.Code && Alt == other.Alt && Control == other.Control && Shift == other.Shift && Meta == other.Meta);
        }

        public override int GetHashCode()
        {
            return Code.GetHashCode();
        }

        public bool Equals(KeyEvent other)
        {
            return this == other;
        }
    }
}
