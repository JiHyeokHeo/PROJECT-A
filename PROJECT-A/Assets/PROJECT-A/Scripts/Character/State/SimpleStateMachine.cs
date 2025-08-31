using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace A
{
    public class SimpleStateMachine : MonoBehaviour, IStateMachine
    {
        [SerializeField]
        CharacterState _state = CharacterState.Idle;
        public CharacterState State => _state;
        public event Action<CharacterState> OnStateChanged;

        public void ChangeState(CharacterState state)
        {
            if (_state == state)
                return;
            OnStateChanged?.Invoke(state);
        }
    }
}
