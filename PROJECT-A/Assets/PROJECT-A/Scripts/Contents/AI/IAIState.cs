using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace A
{
    public enum AIStateId { None, Idle, Patrol, Chase, Attack, Groggy }

    [Serializable]
    public abstract class IAIState 
    {
        public abstract AIStateId aiStateId { get; }
        public abstract void Enter();
        public abstract void Tick(float dt);
        public abstract void Exit();
    }

    public interface IDamage
    {
        void ApplyDamage(float damage);
    }
}
