using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace A
{
    public interface IStateMachine
    {
        CharacterState State { get; }
        void ChangeState(CharacterState state);
        event Action<CharacterState> OnStateChanged;
    }
}
