using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace DirectInputWrapper
{
    /// <summary>
    /// Send direct input keys and key combinations globally with ease
    /// </summary>
    public class DirectInput
    {
        #region NativeCode
        [Flags]
        private enum InputType
        {
            Mouse = 0,
            Keyboard = 1,
            Hardware = 2
        }

        [Flags]
        private enum KeyEventF
        {
            KeyDown = 0x0000,
            ExtendedKey = 0x0001,
            KeyUp = 0x0002,
            Unicode = 0x0004,
            Scancode = 0x0008,
        }

        private struct Input
        {
            public int type;
            public InputUnion u;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct InputUnion
        {
            [FieldOffset(0)] public readonly MouseInput mi;
            [FieldOffset(0)] public KeyboardInput ki;
            [FieldOffset(0)] public readonly HardwareInput hi;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MouseInput
        {
            public readonly int dx;
            public readonly int dy;
            public readonly uint mouseData;
            public readonly uint dwFlags;
            public readonly uint time;
            public readonly IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KeyboardInput
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public readonly uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct HardwareInput
        {
            public readonly uint uMsg;
            public readonly ushort wParamL;
            public readonly ushort wParamH;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, Input[] pInputs, int cbSize);

        [DllImport("user32.dll")]
        private static extern IntPtr GetMessageExtraInfo();

        private static void RawKeyDown(ushort key)
        {
            Input[] inputs =
            {
                new Input
                {
                    type = (int) InputType.Keyboard,
                    u = new InputUnion
                    {
                        ki = new KeyboardInput
                        {
                            wVk = 0,
                            wScan = key,
                            dwFlags = (uint) (KeyEventF.KeyDown | KeyEventF.Scancode),
                            dwExtraInfo = GetMessageExtraInfo()
                        }
                    }
                }
            };

            SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(Input)));
        }

        private static void RawKeyUp(ushort key)
        {
            Input[] inputs =
            {
                new Input
                {
                    type = (int) InputType.Keyboard,
                    u = new InputUnion
                    {
                        ki = new KeyboardInput
                        {
                            wVk = 0,
                            wScan = key,
                            dwFlags = (uint) (KeyEventF.KeyUp | KeyEventF.Scancode),
                            dwExtraInfo = GetMessageExtraInfo()
                        }
                    }
                }
            };

            SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(Input)));
        }
        #endregion

        public static void KeyDown(DirectKeycode key) => RawKeyDown((ushort)key);
        
        public static void KeyUp(DirectKeycode key) => RawKeyUp((ushort)key);

        public static void KeyPress(DirectKeycode key, uint duration)
        {
            KeyDown(key);
            Thread.Sleep((int)duration);
            KeyUp(key);
        }

        public static void ComboDown(DirectKeyCombo combo)
        {
            var kc = new List<DirectKeycode>();
            if (combo.CtrlKey && combo.Left)
                kc.Add(DirectKeycode.DkcLcontrol);
            else
                kc.Add(DirectKeycode.DkcRcontrol);
            
            if (combo.ShiftKey && combo.Left)
                kc.Add(DirectKeycode.DkcLshift);
            else
                kc.Add(DirectKeycode.DkcRshift);

            if (combo.AltKey && combo.Left)
                kc.Add(DirectKeycode.DkcLmenu);
            else
                kc.Add(DirectKeycode.DkcRmenu);

            kc.AddRange(combo.DirectKeycodes);

            foreach(var key in kc)
            {
                KeyDown(key);
            }
        }

        public static void ComboUp(DirectKeyCombo combo)
        {
            var kc = new List<DirectKeycode>();
            if (combo.CtrlKey && combo.Left)
                kc.Add(DirectKeycode.DkcLcontrol);
            else
                kc.Add(DirectKeycode.DkcRcontrol);

            if (combo.ShiftKey && combo.Left)
                kc.Add(DirectKeycode.DkcLshift);
            else
                kc.Add(DirectKeycode.DkcRshift);

            if (combo.AltKey && combo.Left)
                kc.Add(DirectKeycode.DkcLmenu);
            else
                kc.Add(DirectKeycode.DkcRmenu);

            kc.AddRange(combo.DirectKeycodes);

            foreach (var key in kc)
            {
                KeyUp(key);
            }
        }

        public static void ComboPress(DirectKeyCombo combo, uint duration)
        {
            ComboDown(combo);
            Thread.Sleep((int)duration); 
            ComboDown(combo);
        }
    }
}