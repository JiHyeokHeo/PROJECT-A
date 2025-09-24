using System.Collections;
using UnityEngine;

namespace Character
{
    public class RollAbility : MonoBehaviour, ICapability
    {
        [SerializeField] 
        private float rollSpeed = 8f;
        [SerializeField] 
        private float rollTime = 0.25f;

        private Rigidbody2D _rb;
        private IMovable _movable;
        private CharacterAnimatorDriver _driver;
        private ActionLock _actionLock;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _movable = GetComponent<IMovable>();
            _driver = GetComponent<CharacterAnimatorDriver>();
            _actionLock = GetComponent<ActionLock>();
        }

        public void Roll(Vector2 dir)
        {
            if (_actionLock.IsLocked)
                return;
            StartCoroutine(CoRoll(dir.normalized));
        }

        IEnumerator CoRoll(Vector2 dir)
        {
            _movable?.Stop();
            _actionLock.LockFor(rollTime);
            _driver.TriggerRoll();

            float t = 0f;
            Vector2 v = dir * rollSpeed;
            while (t < rollTime)
            {
                _rb.velocity = v;
                t += Time.deltaTime;
                yield return null;
            }
        }
    }
}