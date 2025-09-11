using System;

namespace Character
{
    public interface IHealth
    {
        bool IsDead { get; }
        public void TakeDamage ( float amount, object source = null );
        public bool TryResurrect(float hpPercent);
        public void Heal(float amount);
        public void HealPercent(float percent);
        event Action OnDead;
    }
}
