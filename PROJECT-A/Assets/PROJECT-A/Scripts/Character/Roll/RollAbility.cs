using System.Collections;
using UnityEngine;

namespace Character
{
    public class RollAbility : MonoBehaviour
    {
        [SerializeField] float rollSpeed = 8f;
        [SerializeField] float rollTime = 0.25f;

        Rigidbody2D rb;
        IMovable movable;
        CharacterAnimatorDriver driver;
        ActionLock actionLock;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            movable = GetComponent<IMovable>();
            driver = GetComponent<CharacterAnimatorDriver>();
            actionLock = GetComponent<ActionLock>();
        }

        public void Roll(Vector2 dir)
        {
            if (actionLock.IsLocked)
                return;
            StartCoroutine(CoRoll(dir.normalized));
        }

        IEnumerator CoRoll(Vector2 dir)
        {
            movable?.Stop();
            actionLock.LockFor(rollTime);
            driver.TriggerRoll();

            float t = 0f;
            Vector2 v = dir * rollSpeed;
            while (t < rollTime)
            {
                rb.velocity = v;
                t += Time.deltaTime;
                yield return null;
            }
        }
    }
}