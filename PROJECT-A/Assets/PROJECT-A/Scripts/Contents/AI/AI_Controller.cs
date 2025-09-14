using System;
using Unity.VisualScripting;
using UnityEngine;

namespace A
{
    public class AI_Controller : MonoBehaviour
    {
        public MonsterBase monsterBase;

        public AIState CurrentState { get { return currentState; } private set { } }
        public EAIStateId CurrentId => CurrentState != null ? CurrentState.aiStateId : EAIStateId.None;

        [SerializeReference] private AIState currentState;
        [SerializeReference] public AIState[] aiStates;

        Collider2D[] hits = new Collider2D[4];
        private void Awake()
        {
            aiStates = new AIState[7];
            monsterBase = GetComponent<MonsterBase>();

            AttackState attackState = new AttackState(monsterBase);
            DeadState deadState = new DeadState(monsterBase);
            IdleState idleState = new IdleState(monsterBase);
            //AttackState attackState = new AttackState(monsterBase);
            //AttackState attackState = new AttackState(monsterBase);
            //AttackState attackState = new AttackState(monsterBase);

            aiStates[(int)EAIStateId.Attack] = attackState;
            aiStates[(int)EAIStateId.Dead] = deadState;
            aiStates[(int)EAIStateId.Idle] = idleState;

            currentState = idleState;
        }

        // Visual Graphc 애니메이터 딜레이 실행을 위한 Event 함수
        private void Start()
        {
            CustomEvent.Trigger(gameObject, "GoTo", this);
        }

        // Visual Scripting Trigger Event Send
        public void GoTo(EAIStateId target)
        {
            CustomEvent.Trigger(gameObject, "GoTo", target);
        }

        public void AIStateChange(EAIStateId next)
        {
            if (next == EAIStateId.None || CurrentId == next)
                return;
            currentState.Exit(); // 씬 나간 후
            currentState = aiStates[(int)next];
            GoTo(currentState.aiStateId); // Visual Scripting Trigger Event Send
            currentState.Enter();
        }

        void Update()
        {
            // 먼저 서칭 후 업데이트 
            var next = currentState.CheckTransition();
            AIStateChange(next); // 상태전환 검사

            SearchTarget();
            currentState?.Tick(Time.deltaTime);
        }

        private void SearchTarget()
        {
            int count = Physics2D.OverlapCircleNonAlloc(monsterBase.monsterContext.RigidBody2D.position, monsterBase.monsterContext.Config.DetectRange, hits);
            monsterBase.Target = count > 0 ? hits[0].gameObject : null;
        }
    }
}
