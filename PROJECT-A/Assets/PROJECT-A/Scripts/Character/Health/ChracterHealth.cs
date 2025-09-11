using Character;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character
{
    public class ChracterHealth : MonoBehaviour, IHealth
    {
        [SerializeField] private float maxHp = 100f;
        [SerializeField] private float cur;
        [SerializeField] bool dead;


        public bool IsDead => dead;
        public float CurrentHP => cur;
        public float MaxHP => maxHp;    

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

        public void Heal(float amount)
        {
            if (dead)
                return;
            float h = Mathf.Max(0f, amount);
            if (h <= 0f)
                return;

            cur = Mathf.Min(maxHp, cur + h);
        }

        public void HealPercent(float percent)
        {
            if (dead)
                return;
            float p = Mathf.Clamp01(percent);
            if (p <= 0f)
                return;
            Heal(maxHp *  p);
        }

        public bool TryResurrect(float hpPercent)
        {
            if (!dead)
                return false;
            float p = Mathf.Clamp01(hpPercent);
            cur = Mathf.Clamp(MaxHP * p, 1f, maxHp);
            dead = false;

            return true;
        }
    }
}
