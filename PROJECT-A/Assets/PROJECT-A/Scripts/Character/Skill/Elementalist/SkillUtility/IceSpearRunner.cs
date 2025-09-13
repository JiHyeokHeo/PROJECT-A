using Character;
using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class IceSpearRunner : MonoBehaviour
{
    Coroutine co;

    public void Run(ICharacter caster, ICharacter target, float preDelay, float speed, float turnRateDeg, float hitRadius, float maxLifetime, float baseDmg, float fanAngleDeg, GameObject vfxPrefab, LayerMask enemyMask, bool empowered)
    {
        if (co != null) StopCoroutine(co);
        co = StartCoroutine(Co(caster, target, preDelay, speed, turnRateDeg, hitRadius, maxLifetime, baseDmg, fanAngleDeg, vfxPrefab, enemyMask, empowered));
    }

    IEnumerator Co(ICharacter caster, ICharacter target, float preDelay, float speed, float turnRateDeg, float hitRadius, float maxLifetime, float baseDmg, float fanAngleDeg, GameObject vfxPrefab, LayerMask enemyMask, bool empowered)
    {
        if (preDelay > 0f) yield return new WaitForSeconds(preDelay);
        if (caster == null || caster.Health == null || caster.Health.IsDead) { co = null; yield break; }

        Vector2 cpos = caster.Transform.position;
        Vector2 mainDir = ((Vector2)target.Transform.position - cpos).normalized;

        void Spawn(Vector2 dir)
        {
            GameObject go = vfxPrefab ? Instantiate(vfxPrefab, cpos, Quaternion.identity) : new GameObject("IceSpear");
            var proj = go.GetComponent<HomingProjectile2D>();
            if (!proj) proj = go.AddComponent<HomingProjectile2D>();
            var dmg = new Damage { amount = baseDmg + (caster.Stats?.Atk ?? 0f), kind = DamageKind.Magical, source = ((Component)caster.Transform).gameObject };
            proj.Init(caster, target, dir, dmg, speed, turnRateDeg, hitRadius, maxLifetime, enemyMask);
        }

        if (!empowered)
        {
            Spawn(mainDir);
        }
        else
        {
            float a = fanAngleDeg * Mathf.Deg2Rad;
            Vector2 dirL = new Vector2(mainDir.x * Mathf.Cos(a) - mainDir.y * Mathf.Sin(a), mainDir.x * Mathf.Sin(a) + mainDir.y * Mathf.Cos(a));
            Vector2 dirR = new Vector2(mainDir.x * Mathf.Cos(-a) - mainDir.y * Mathf.Sin(-a), mainDir.x * Mathf.Sin(-a) + mainDir.y * Mathf.Cos(-a));
            Spawn(mainDir);
            Spawn(dirL.normalized);
            Spawn(dirR.normalized);
        }
        co = null;
    }

    void OnDisable() { if (co != null) { StopCoroutine(co); co = null; } }
}

    
