using Character;
using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class IceSpearRunner : MonoBehaviour
{
    Coroutine co;
    float forwardOffset = 1.6f;  
    float lateralOffset = 0.6f;

    public void Run(ICharacter caster,
        ICharacter target,
        float preDelay,
        float speed,
        float turnRateDeg,
        float hitRadius,
        float maxLifetime,
        float baseDmg,
        float fanAngleDeg,
        GameObject vfxPrefab,
        LayerMask enemyMask,
        bool empowered)
    {
        if (co != null) StopCoroutine(co);
        co = StartCoroutine(Co(caster, target, preDelay, speed, turnRateDeg, hitRadius, maxLifetime, baseDmg, fanAngleDeg, vfxPrefab, enemyMask, empowered));
    }

    IEnumerator Co(ICharacter caster,
        ICharacter target,
        float preDelay,
        float speed,
        float turnRateDeg,
        float hitRadius,
        float maxLifetime,
        float baseDmg,
        float fanAngleDeg,
        GameObject vfxPrefab,
        LayerMask enemyMask,
        bool empowered)
    {
        if (preDelay > 0f) 
            yield return new WaitForSeconds(preDelay);

        if (caster == null || caster.Health == null || caster.Health.IsDead) 
        {
            co = null;
            yield break;
        }

        Vector2 cpos = caster.Transform.position;
        Vector2 mainDir = ((Vector2)target.Transform.position - cpos).normalized;
        Vector2 perp = new Vector2(-mainDir.y, mainDir.x);

        void Spawn(Vector2 dir, Vector2 projpos)
        {
            GameObject go = Instantiate(vfxPrefab, projpos, Quaternion.identity);
            var proj = go.GetComponent<HomingProjectile2D>();
            if (!proj) 
                proj = go.AddComponent<HomingProjectile2D>();
            var dmg = new Damage 
            {
                Amount = baseDmg + (caster.Stats?.Atk ?? 0f),
                Kind = DamageKind.Magical,
                Source =caster.Transform.gameObject 
            };
            proj.Init(caster, target, dir, dmg, speed, turnRateDeg, hitRadius, maxLifetime, enemyMask);
        }

        if (!empowered)
        {
            Vector2 centerPos = cpos + mainDir * forwardOffset;
            Spawn(mainDir, centerPos);
        }
        else
        {
            float a = fanAngleDeg * Mathf.Deg2Rad;
            Vector2 dirL = new Vector2(mainDir.x * Mathf.Cos(a) - mainDir.y * Mathf.Sin(a), mainDir.x * Mathf.Sin(a) + mainDir.y * Mathf.Cos(a));
            Vector2 dirR = new Vector2(mainDir.x * Mathf.Cos(-a) - mainDir.y * Mathf.Sin(-a), mainDir.x * Mathf.Sin(-a) + mainDir.y * Mathf.Cos(-a));
            Vector2 centerPos = cpos + mainDir * forwardOffset;
            Vector2 leftPos = cpos + mainDir * forwardOffset + perp * lateralOffset;
            Vector2 rightPos = cpos + mainDir * forwardOffset - perp * lateralOffset;

            Spawn(mainDir, centerPos);
            Spawn(dirL, leftPos);
            Spawn(dirR, rightPos);
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

    
