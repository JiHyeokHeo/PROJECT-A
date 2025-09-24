using System;
using System.Collections;
using UnityEngine;

namespace Character
{
    public class ActionLock : MonoBehaviour
    {

        public bool IsLocked { get; private set; }
        public event Action<bool> OnLockChanged;

        private int _lockCount;
        private bool _timedActive;  
        private Coroutine _timedCo;

        public void Lock()
        {
            _lockCount++;
            UpdateState();
        }

        public void UnLock()
        {
            _lockCount = Mathf.Max(0, _lockCount - 1);
            UpdateState();
        }

        public void LockFor(float seconds)
        {
            if (!_timedActive)
            {
                _timedActive = true;
                _lockCount++;
                UpdateState();
            }

            if (_timedCo != null) StopCoroutine(_timedCo);
            _timedCo = StartCoroutine(CoTimed(seconds));
        }
        private IEnumerator CoTimed(float s)
        {
            yield return new WaitForSeconds(s);

            _timedActive = false;
            _lockCount = Mathf.Max(0, _lockCount - 1); 
            UpdateState();

            _timedCo = null;
        }
        private void UpdateState()
        {
            bool on = _lockCount > 0;
            if (on == IsLocked) return;
            IsLocked = on;
            OnLockChanged?.Invoke(IsLocked);
        }
    }
}