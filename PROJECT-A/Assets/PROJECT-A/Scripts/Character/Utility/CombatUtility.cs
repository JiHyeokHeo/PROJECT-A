using Character;
using Unity.Collections;
using UnityEditor;
using UnityEngine;

public static class CombatUtility
{
    public static void ApplyDamage(ICharacter target, Damage dmg)
    {
        if (target == null || target.Health == null) return;

        float final = dmg.Amount;

        /* 여기에 이제 Damage 관련된 식을 넣는다.
        

            ....

        */
        target.Health.TakeDamage(final, dmg.Source);
    }
}