using System.Collections;
using UnityEngine;

namespace Character
{
    public enum InputMode
    {
        Normal,
        AttackPrimed,
        BoxSelecting,
        Casting
    }
    public sealed class InputModeFSM
    {
        public InputMode Mode { get; private set; } = InputMode.Normal;
        public bool IsPrimed => Mode == InputMode.AttackPrimed;

        public void ToggleAttackPrimed()
        {
            Mode = (Mode == InputMode.AttackPrimed) ? InputMode.Normal : InputMode.AttackPrimed;
        }

        public void BeginBoxSelect()
        {
            Mode = InputMode.BoxSelecting;
        }

        public void EndBoxSelect()
        {
            if (Mode == InputMode.BoxSelecting)
                Mode = InputMode.Normal;
        }
        
        public void BeginCasting()
        {
            Mode = InputMode.Casting;
        }

        public void EndCasting()
        {
            Mode = InputMode.Normal;
        }

        public override string ToString()
        {
            return Mode.ToString();
        }
    }
}