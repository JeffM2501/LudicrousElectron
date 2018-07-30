using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Input;

namespace LudicrousElectron.Engine.Input
{
    public class KeyMap : Dictionary<string, Key>
    {
        public static KeyMap Names = new KeyMap();

        public KeyMap()
        {
            Add("A", Key.A);
            Add("B", Key.B);
            Add("C", Key.C);
            Add("D", Key.D);
            Add("E", Key.E);
            Add("F", Key.F);
            Add("G", Key.G);
            Add("H", Key.H);
            Add("I", Key.I);
            Add("J", Key.J);
            Add("K", Key.K);
            Add("L", Key.L);
            Add("M", Key.M);
            Add("N", Key.N);
            Add("O", Key.O);
            Add("P", Key.P);
            Add("Q", Key.Q);
            Add("R", Key.R);
            Add("S", Key.S);
            Add("T", Key.T);
            Add("U", Key.U);
            Add("V", Key.V);
            Add("W", Key.W);
            Add("X", Key.X);
            Add("Y", Key.Y);
            Add("Z", Key.Z);
            Add("Num0", Key.Number0);
            Add("Num1", Key.Number1);
            Add("Num2", Key.Number2);
            Add("Num3", Key.Number3);
            Add("Num4", Key.Number4);
            Add("Num5", Key.Number5);
            Add("Num6", Key.Number6);
            Add("Num7", Key.Number7);
            Add("Num8", Key.Number8);
            Add("Num9", Key.Number9);
            Add("Escape", Key.Escape);
            Add("LControl", Key.LControl);
            Add("LShift", Key.LShift);
            Add("LAlt", Key.LAlt);
            Add("LSystem", Key.WinLeft);
            Add("RControl", Key.RControl);
            Add("RShift", Key.RShift);
            Add("RAlt", Key.RAlt);
            Add("RSystem", Key.WinRight);
            Add("Menu", Key.Menu);
            Add("LBracket", Key.LBracket);
            Add("RBracket", Key.RBracket);
            Add("SemiColon", Key.Semicolon);
            Add("Comma", Key.Comma);
            Add("Period", Key.Period);
            Add("Quote", Key.Quote);
            Add("Slash", Key.Slash);
            Add("BackSlash", Key.BackSlash);
            Add("Tilde", Key.Tilde);
            Add("Space", Key.Space);
            Add("Return", Key.Enter);
            Add("BackSpace", Key.BackSpace);
            Add("Tab", Key.Tab);
            Add("PageUp", Key.PageUp);
            Add("PageDown", Key.PageDown);
            Add("End", Key.End);
            Add("Home", Key.Home);
            Add("Insert", Key.Insert);
            Add("Delete", Key.Delete);
            Add("Add", Key.Plus);
            Add("Subtract", Key.Minus);
            Add("Multiply", Key.KeypadMultiply);
            Add("Divide", Key.KeypadDivide);
            Add("Left", Key.Left);
            Add("Right", Key.Right);
            Add("Up", Key.Up);
            Add("Down", Key.Down);
            Add("Numpad0", Key.Keypad0);
            Add("Numpad1", Key.Keypad1);
            Add("Numpad2", Key.Keypad2);
            Add("Numpad3", Key.Keypad3);
            Add("Numpad4", Key.Keypad4);
            Add("Numpad5", Key.Keypad5);
            Add("Numpad6", Key.Keypad6);
            Add("Numpad7", Key.Keypad7);
            Add("Numpad8", Key.Keypad8);
            Add("Numpad9", Key.Keypad9);
            Add("F1", Key.F1);
            Add("F2", Key.F2);
            Add("F3", Key.F3);
            Add("F4", Key.F4);
            Add("F5", Key.F5);
            Add("F6", Key.F6);
            Add("F7", Key.F7);
            Add("F8", Key.F8);
            Add("F9", Key.F9);
            Add("F10", Key.F10);
            Add("F11", Key.F11);
            Add("F12", Key.F12);
            Add("F13", Key.F13);
            Add("F14", Key.F14);
            Add("F15", Key.F15);
            Add("Pause", Key.Pause);
        }
    }
}
