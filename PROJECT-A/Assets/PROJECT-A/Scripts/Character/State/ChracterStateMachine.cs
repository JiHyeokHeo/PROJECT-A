using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character
{
    public class ChracterStateMachine : MonoBehaviour, IStateMachine
    {
        [SerializeField] private CharacterState state = CharacterState.Idle;
        public CharacterState State => state;
        public event Action<CharacterState> OnStateChanged;

        public void ChangeState(CharacterState currentState)
        {
            if (state == currentState)
                return;
            state = currentState;
            OnStateChanged?.Invoke(currentState);
        }
    }
}
