using Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StripDoT2D : MonoBehaviour
{
    public float length = 6f;
    public float width = 1f;
    public float duration = 5f;
    public LayerMask enemyMask;

    public float dps = 10f;
    public float tickInterval = 0.25f;

    ICharacter caster;
    float endAt;
    float angleDeg;
    Vector2 center;

    static readonly Collider2D[] _hits = new Collider2D[32];
    readonly HashSet<ICharacter> _tickGate = new();

    public void Arm(ICharacter caster, Vector2 origin, Vector2 dir, float length, float width, float duration,
        float dps, float tickInterval, LayerMask enemyMask)
    {
        this.caster = caster;
        this.center = origin + dir.normalized * (length * 0.5f);
        this.length = length;
        this.width = width;
        this.duration = duration;
        this.dps = dps;
        this.tickInterval = tickInterval;
        this.enemyMask = enemyMask;

        angleDeg = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        endAt = Time.time + duration;

        StartCoroutine(Co());
    }

    IEnumerator Co()
    {
        var wait = new WaitForSeconds(tickInterval);
        float dmgPerTick = dps * tickInterval;

        while (Time.time < endAt)
        {
            int n = Physics2D.OverlapBoxNonAlloc(center, new Vector2(length, width), angleDeg, _hits, enemyMask);
            for (int i = 0; i <n; i++)
            {
                var ch = _hits[i].GetComponent<ICharacter>();
                if (ch == null || ch.Health.IsDead)
                    continue;
                if (!_tickGate.Add(ch))
                    continue;

                var packet = new Damage
                {
                    Amount = dmgPerTick,
                    Kind = DamageKind.Magical,
                    Source = caster.Transform.gameObject,
                    HitPoint = _hits[i].bounds.ClosestPoint(center),
                };
                CombatUtility.ApplyDamage(ch, packet);
            }

            _tickGate.Clear();
            yield return wait;
        }
        Destroy(gameObject);
    }
}
