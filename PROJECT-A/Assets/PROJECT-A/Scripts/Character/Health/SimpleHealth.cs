using Character;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace A
{
    public class SimpleHealth : MonoBehaviour, IHealth
    {
        [SerializeField] float maxHp = 100f;
        float cur;
        bool dead;
        public bool IsDead => dead;
        public event Action OnDead;

        void Awake() { cur = maxHp; }

        public void TakeDamage(float amount, object source = null)
        {
            if (dead) 
            {
                return;
            }
            cur = Mathf.Max(0, cur - MathF.Max(0, amount));

            Debug.Log($"{gameObject.name} took {amount} damage from {source}");

            if (cur <= 0)
            {
                dead = true;
                OnDead?.Invoke();
            }
        }
    }
}
