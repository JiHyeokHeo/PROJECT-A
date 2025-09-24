using Character;
using System.Collections;
using UnityEngine;

namespace Character
{
    public class CharacterAnimatorDriver : MonoBehaviour , ICapability
    {
        [SerializeField]
        private Animator animator;
        [SerializeField]
        private Rigidbody2D rb;

        private IStats _stats;
        private ActionLock _actionLock;

        private static readonly int HashSpeed = Animator.StringToHash("Speed");
        private static readonly int HashActionTrigger = Animator.StringToHash("ActionTrigger");
        private static readonly int HashActionID = Animator.StringToHash("ActionID");
        private static readonly int HashRollTrigger = Animator.StringToHash("RollTrigger");
        private static readonly int HashDieTrigger = Animator.StringToHash("DieTrigger");
        private static readonly int HashStunTrigger = Animator.StringToHash("StunTrigger");
        private static readonly int HashStunEnd = Animator.StringToHash("StunEnd");
        private static readonly int HashIsActionLocked = Animator.StringToHash("isActionLocked");

        private void Awake()
        {
            if (!animator)
                animator = GetComponent<Animator>();
            if (!rb)
                rb = GetComponent<Rigidbody2D>();
            _stats = GetComponent<IStats>();
            _actionLock = GetComponent<ActionLock>();
            if (_actionLock)
                _actionLock.OnLockChanged += OnLockChanged;
        }

        private void OnDestroy()
        {
            if (_actionLock)
                _actionLock.OnLockChanged -= OnLockChanged;
        }

        void OnLockChanged(bool on)
        {
            animator.SetBool(HashIsActionLocked, on);
        }

        private void FixedUpdate()
        {
            float max = (_stats != null ? Mathf.Max(0.01f, _stats.Speed) : 5f);
            float normalized = Mathf.Clamp01(rb.velocity.magnitude / max);
            animator.SetFloat(HashSpeed, normalized);
        }

        public void TriggerAction(int actionId)
        {
            animator.SetInteger(HashActionID, actionId);
            animator.ResetTrigger(HashActionTrigger);
            animator.SetTrigger(HashActionTrigger);
        }

        public void TriggerRoll() => animator.SetTrigger(HashRollTrigger);
        public void TriggerStun() => animator.SetTrigger(HashStunTrigger);
        public void TriggerStunEnd() => animator.SetTrigger(HashStunEnd);
        public void TriggerDie() => animator.SetTrigger(HashDieTrigger);
    }
}
