using System;

namespace Character
{
    public interface IHealth
    {
        bool IsDead { get; }
        void TakeDamage ( float amount, object source = null );
        event Action OnDead;
    }
}
