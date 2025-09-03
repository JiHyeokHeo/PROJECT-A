using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace A
{
    public class IdleState : IAIState
    {
        public override AIStateId aiStateId => AIStateId.Idle;

        public override void Enter()
        {
            
        }

        public override void Exit()
        {
        }

        public override void Tick(float dt)
        {
        }
    }
}
