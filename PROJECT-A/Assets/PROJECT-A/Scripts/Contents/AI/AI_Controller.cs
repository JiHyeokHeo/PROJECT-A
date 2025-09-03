using Unity.VisualScripting;
using UnityEngine;

namespace A
{
    public class AI_Controller : MonoBehaviour
    {
        public IAIState CurrentState { get { return currentState; } private set { } }
        public AIStateId CurrentId => CurrentState != null ? CurrentState.aiStateId : AIStateId.None;

        [SerializeReference] private IAIState currentState;
        [SerializeReference] public IAIState[] aiStates = new IAIState[6];

        private void Awake()
        {
            int idleIndex = (int)AIStateId.Idle;
            currentState = aiStates[idleIndex];
        }

        private void Start()
        {
            CustomEvent.Trigger(gameObject, "GoTo", this);
        }

        public void GoTo(AIStateId target)
        {
            CustomEvent.Trigger(gameObject, "GoTo", target);
        }

        public void AIStateChange(AIStateId next)
        {
            if (next == AIStateId.None || CurrentId == next)
                return;
            currentState.Exit(); // ¾À ³ª°£ ÈÄ
            currentState = aiStates[(int)next];
            GoTo(currentState.aiStateId); // Visual Scripting Trigger Event Send
            currentState.Enter();
        }

        void Update()
        {
            currentState?.Tick(Time.deltaTime);

            if (Input.GetKeyDown(KeyCode.K))
            {
                AIStateChange(AIStateId.Attack);
            }
        }
    }
}
