using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace A
{
    public interface IAIState 
    {
        public void Enter();
        public void Tick(float dt);
        public void Exit();
    }

    public interface IDamage
    {
        void ApplyDamage(float damage);
    }
}
