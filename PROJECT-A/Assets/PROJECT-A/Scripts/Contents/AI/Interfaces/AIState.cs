using System;

namespace A
{
    public enum EAIStateId { None, Idle, Patrol, Chase, Attack, Groggy, Dead }

    [Serializable]
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
    }
}
