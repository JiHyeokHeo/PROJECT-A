using System.Collections;
using UnityEngine;

namespace Character
{
    public class StunStatus : MonoBehaviour
    {
        private Rigidbody2D _rb;
        private CharacterAnimatorDriver _driver;
        private ActionLock _actionLock;

        RigidbodyConstraints2D _origConstraints;
        bool _stunning;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _driver = GetComponent<CharacterAnimatorDriver>();
            _actionLock = GetComponent<ActionLock>();
            _origConstraints = _rb.constraints;
        }

        public void ApplyStun(float seconds)
        {
            if (!_stunning)
                StartCoroutine(CoStun(seconds));
            else
                _actionLock.LockFor(seconds);
        }

        IEnumerator CoStun(float seconds)
        {
            _stunning = true;
            _rb.velocity = Vector3.zero;
            _rb.constraints = RigidbodyConstraints2D.FreezeAll;

            _actionLock.LockFor(seconds);
            _driver.TriggerStun();

            yield return new WaitForSeconds(seconds);

            _driver.TriggerStunEnd();
            _rb.constraints = _origConstraints;
            _stunning = false;
        }
    }
}