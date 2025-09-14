namespace A
{
    public class IdleState : AIState
    {
        public IdleState(MonsterBase monster) : base(monster)
        {
            this.monster = monster;
        }

        public override EAIStateId aiStateId => EAIStateId.Idle;

        public override EAIStateId CheckTransition()
        {
            if (FindTarget())
                return EAIStateId.Attack;

            return EAIStateId.None;
        }

        public override void Enter()
        {
            
        }

        public override void Exit()
        {
        }

        public override void Tick(float dt)
        {
            
        }

        private bool FindTarget()
        {
            if (monster.Target == null)
                return false;


            return true;
        }
    }
}
