using System;

namespace A
{
    public enum EAIStateId { None, Idle, Patrol, Chase, Attack, Groggy, Dead }

    public abstract class AIState 
    {
        public AIState(MonsterBase monster) : base()
        {
            this.monster = monster;
        }
        protected MonsterBase monster;
        public abstract EAIStateId aiStateId { get; }
        public abstract void Enter();
        public abstract void Tick(float dt);
        public abstract void Exit();
        public abstract EAIStateId CheckTransition();
    }
}
