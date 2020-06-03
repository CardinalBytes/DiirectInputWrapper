namespace DirectInputWrapper
{
    public readonly struct DirectKeyCombo
    {
        internal bool ShiftKey { get; }
        internal bool CtrlKey { get; }
        internal bool AltKey { get; }
        internal bool Left { get; }
        internal DirectKeycode[] DirectKeycodes { get; }

        public DirectKeyCombo(bool shift, bool ctrl, bool alt, bool left, DirectKeycode[] keycodes)
        {
            ShiftKey = shift;
            CtrlKey = ctrl;
            AltKey = alt;
            Left = left;
            DirectKeycodes = keycodes;
        }
    }
}