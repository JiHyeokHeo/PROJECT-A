

namespace A
{
    public class DeadState : AIState
    {
        public DeadState(MonsterBase monster) : base(monster)
        {
            this.monster = monster;
        }

        public override EAIStateId aiStateId => EAIStateId.Dead;

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
