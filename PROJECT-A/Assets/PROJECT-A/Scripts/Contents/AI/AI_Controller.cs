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

        // Visual Graphc �ִϸ����� ������ ������ ���� Event �Լ�
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
            currentState.Exit(); // �� ���� ��
            currentState = aiStates[(int)next];
            GoTo(currentState.aiStateId); // Visual Scripting Trigger Event Send
            currentState.Enter();
        }

        void Update()
        {
            currentState?.Tick(Time.deltaTime);

            if (Input.GetKeyDown(KeyCode.K))
            {
                AIStateChange(EAIStateId.Attack);
            }
        }
    }
}
