using System.Collections;
using UnityEngine;

namespace Character
{
    public class StunStatus : MonoBehaviour
    {
        private Rigidbody2D rb;
        private CharacterAnimatorDriver driver;
        private ActionLock actionLock;

        RigidbodyConstraints2D _origConstraints;
        bool _stunning;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            driver = GetComponent<CharacterAnimatorDriver>();
            actionLock = GetComponent<ActionLock>();
            _origConstraints = rb.constraints;
        }

        public void ApplyStun(float seconds)
        {
            if (!_stunning)
                StartCoroutine(CoStun(seconds));
            else
                actionLock.LockFor(seconds);
        }

        IEnumerator CoStun(float seconds)
        {
            _stunning = true;
            rb.velocity = Vector3.zero;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;

            actionLock.LockFor(seconds);
            driver.TriggerStun();

            yield return new WaitForSeconds(seconds);

            driver.TriggerStunEnd();
            rb.constraints = _origConstraints;
            _stunning = false;
        }
    }
}