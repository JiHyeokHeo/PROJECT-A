using Character;
using System.Collections;
using UnityEngine;


public class LightningRunner : MonoBehaviour
{

    Coroutine co;
    Collider2D[] _buf = new Collider2D[64];

    public void Run(ICharacter caster, float preDelay, float radius, LayerMask enemyMask, float baseDmg, float empowerMul, bool empowerCounter, bool empowered)
    {
        if (co != null) StopCoroutine(co);
        co = StartCoroutine(Co(caster, preDelay, radius, enemyMask, baseDmg, empowerMul, empowerCounter, empowered));
    }

    IEnumerator Co(ICharacter caster, float preDelay, float radius, LayerMask enemyMask, float baseDmg, float empowerMul, bool empowerCounter, bool empowered)
    {
        if (preDelay > 0f) yield return new WaitForSeconds(preDelay);
        if (caster == null || caster.Health == null || caster.Health.IsDead) { co = null; yield break; }

        Vector2 cpos = caster.Transform.position;
        int n = Physics2D.OverlapCircleNonAlloc(cpos, radius, _buf, enemyMask);

        ICharacter best = null; float bestHP = float.NegativeInfinity;
        for (int i = 0; i < n; i++)
        {
            var ch = _buf[i]?.GetComponent<ICharacter>();
            if (ch == null || ch.Health.IsDead) continue;
            float hp = ch.Stats?.HP ?? 0f; // Stats 기준
            if (hp > bestHP) { bestHP = hp; best = ch; }
        }

        if (best != null)
        {
            float dmgAmount = empowered ? baseDmg * empowerMul : baseDmg;
            var dmg = new Damage { Amount = dmgAmount + (caster.Stats?.Atk ?? 0f),
                Kind = DamageKind.Magical, Source = ((Component)caster.Transform).gameObject,
                HitPoint = (Vector2)best.Transform.position };
            CombatUtility.ApplyDamage(best, dmg);

        }
        co = null;
    }

    void OnDisable() 
    {
        if (co != null) 
        {
            StopCoroutine(co);
            co = null; 
        } 
    }
}
