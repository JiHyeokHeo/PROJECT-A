using System.Collections.Generic;
using UnityEngine;

namespace Character
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class AggroSensor : MonoBehaviour, ICapability
    {
        [SerializeField]
        private LayerMask enemyMask;

        private ICharacter Self { get; set; }

        static readonly List<ICharacter> TMPList = new(16);
        private readonly HashSet<ICharacter> _inRange = new();
        public IReadOnlyCollection<ICharacter> InRange => _inRange;

        CircleCollider2D _col;

        private void Awake()
        {
            _col = GetComponent<CircleCollider2D>();
            _col.isTrigger = true;

            Self = GetComponentInParent<ICharacter>(true);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!enabled)
                return;
            if ((enemyMask.value & (1 << other.gameObject.layer)) == 0)
                return;

            var ch = other.GetComponent<ICharacter>();
            if (ch == null)
                return;
            if (ch.Health == null || ch.Health.IsDead)
                return;

            if (ReferenceEquals(ch, Self))
                return;

            _inRange.Add(ch);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            var ch = other.GetComponent<ICharacter>();
            if (ch == null)
                return;
            _inRange.Remove(ch);
        }

        public void Prune()
        {
            if (_inRange.Count == 0)
                return;
            TMPList.Clear();
            foreach (var c in _inRange)
            {
                if (c == null || c.Health == null || c.Health.IsDead)
                    TMPList.Add(c);
            }
            if (TMPList.Count > 0)
            {
                foreach (var d in TMPList)
                    _inRange.Remove(d);
            }
        }
        public float GetRadius() => (_col != null ? _col.radius : 0f);
    }
}