using Character;
using System.Collections;
using UnityEngine;


public class FlameRoadRunner : MonoBehaviour
{
    Coroutine co;
    static readonly Collider2D[] _buf = new Collider2D[64];

    public void Run(ICharacter caster,
        Vector2 point,
        float preDelay,
        float duration,
        float length,
        float width,
        float tickInterval,
        float dps,
        LayerMask enemyMask,
        float empowerDurMul,
        float empowerWidthMul,
        GameObject areaPrefab,
        bool empowered)
    {
        if (co != null)
            StopCoroutine(co);
        co = StartCoroutine(Co(caster, point, preDelay, duration, length, width, tickInterval, dps, enemyMask, empowerDurMul, empowerWidthMul, areaPrefab, empowered));
    }

    private IEnumerator Co(ICharacter caster,
        Vector2 point,
        float preDelay,
        float duration,
        float length,
        float width,
        float tickInterval,
        float dps,
        LayerMask enemyMask,
        float empowerDurMul,
        float empowerWidthMul,
        GameObject areaPrefab,
        bool empowered)
    {
        if (preDelay > 0f) yield return new WaitForSeconds(preDelay);
        if (caster == null || caster.Health == null || caster.Health.IsDead) { co = null; yield break; }

        Vector2 start = caster.Transform.position;
        Vector2 dir = (point - start);
        if (dir.sqrMagnitude < 1e-6f)
            dir = Vector2.right;
        dir.Normalize();

        float dur = empowered ? duration * empowerDurMul : duration;
        float w = empowered ? width * empowerWidthMul : width;
        float halfW = w * 0.5f;

        // Optional visual prefab
        GameObject vis = null;
        if (areaPrefab)
        {
            vis = Instantiate(areaPrefab,
                start + dir * (length * 0.5f),
                Quaternion.FromToRotation(Vector2.right, dir));
            vis.transform.localScale = new Vector3(length, w, 1f);
            Destroy(vis, dur + 0.1f);
        }

        float endAt = Time.time + dur;
        float perTickDamage = dps * tickInterval;
        var wait = new WaitForSeconds(tickInterval);

        // OverlapBox center/size/angle
        Vector2 center = start + dir * (length * 0.5f);
        Vector2 size = new Vector2(length, w);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        while (Time.time < endAt)
        {
            int n = Physics2D.OverlapBoxNonAlloc(center, size, angle, _buf, enemyMask);
            for (int i = 0; i < n; i++)
            {
                var ch = _buf[i]?.GetComponent<ICharacter>();
                if (ch == null || ch.Health.IsDead) continue;
                var dmg = new Damage 
                {
                    Amount = perTickDamage + caster.Stats.Atk * 0.2f,
                    Kind = DamageKind.Magical,
                    Source = caster.Transform.gameObject
                };
                CombatUtility.ApplyDamage(ch, dmg);
            }
            yield return wait;
        }
        co = null;
    }

    private void OnDisable() 
    {
        if (co != null)
        {
            StopCoroutine(co);
            co = null; 
        } 
    }
}
