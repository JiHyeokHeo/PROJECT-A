using Character;
using System.Collections;
using UnityEngine;

namespace Character
{
    public class CharacterAnimatorDriver : MonoBehaviour
    {
        [SerializeField]
        private Animator animator;
        [SerializeField]
        private Rigidbody2D rb;

        private IStats stats;
        private ActionLock actionLock;

        static readonly int HASH_Speed = Animator.StringToHash("Speed");
        static readonly int HASH_ActionTrigger = Animator.StringToHash("ActionTrigger");
        static readonly int HASH_ActionID = Animator.StringToHash("ActionID");
        static readonly int HASH_RollTrigger = Animator.StringToHash("RollTrigger");
        static readonly int HASH_DieTrigger = Animator.StringToHash("DieTrigger");
        static readonly int HASH_StunTrigger = Animator.StringToHash("StunTrigger");
        static readonly int HASH_StunEnd = Animator.StringToHash("StunEnd");
        static readonly int HASH_isActionLocked = Animator.StringToHash("isActionLocked");

        private void Awake()
        {
            if (!animator)
                animator = GetComponent<Animator>();
            if (!rb)
                rb = GetComponent<Rigidbody2D>();
            stats = GetComponent<IStats>();
            actionLock = GetComponent<ActionLock>();
            if (actionLock)
                actionLock.OnLockChanged += OnLockChanged;
        }

        private void OnDestroy()
        {
            if (actionLock)
                actionLock.OnLockChanged -= OnLockChanged;
        }

        void OnLockChanged(bool on)
        {
            animator.SetBool(HASH_isActionLocked, on);
        }

        private void FixedUpdate()
        {
            float max = (stats != null ? Mathf.Max(0.01f, stats.Speed) : 5f);
            float normalized = Mathf.Clamp01(rb.velocity.magnitude / max);
            animator.SetFloat(HASH_Speed, normalized);
        }

        public void TriggerAction(int actionId)
        {
            animator.SetInteger(HASH_ActionID, actionId);
            animator.ResetTrigger(HASH_ActionTrigger);
            animator.SetTrigger(HASH_ActionTrigger);
        }

        public void TriggerRoll() => animator.SetTrigger(HASH_RollTrigger);
        public void TriggerStun() => animator.SetTrigger(HASH_StunTrigger);
        public void TriggerStunEnd() => animator.SetTrigger(HASH_StunEnd);
        public void TriggerDie() => animator.SetTrigger(HASH_DieTrigger);
    }
}
