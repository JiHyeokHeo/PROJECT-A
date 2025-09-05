

using UnityEngine;

namespace A
{
    public class IdleState : AIState
    {
        public IdleState(MonsterBase monster) : base(monster)
        {
            this.monster = monster;
        }

        public override EAIStateId aiStateId => EAIStateId.Idle;

        public override void Enter()
        {
            
        }

        public override void Exit()
        {
        }

        public override void Tick(float dt)
        {
            // 특정 구간에 들어가면 State 변경



        }
    }
}
