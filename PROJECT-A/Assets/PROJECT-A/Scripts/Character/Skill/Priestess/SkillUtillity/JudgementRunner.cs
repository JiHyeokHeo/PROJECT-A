using Character;
using System.Collections;
using UnityEngine;


public class JudgementRunner : MonoBehaviour
{
    ICharacter caster;
    float dmg, delay, radius;
    LayerMask enemyMask;
    Coroutine co;

    [SerializeField] int maxScan = 32;
    Collider2D[] _hits;

    private void Awake()
    {
        if (_hits == null || _hits.Length != maxScan)
            _hits = new Collider2D[maxScan];
    }

    public void Run(ICharacter caster, float damage, float preDelay, float searchRadius, LayerMask mask)
    {
        this.caster = caster;
        dmg = damage;
        delay = preDelay;
        radius = searchRadius;
        enemyMask = mask;

        if (_hits == null || _hits.Length != maxScan)
            _hits = new Collider2D[maxScan];

        if (co != null)
            StopCoroutine(co);
        co = StartCoroutine(Co());
    }

    private IEnumerator Co()
    {
        yield return new WaitForSeconds(delay);

        if (caster == null || caster.Health == null || caster.Health.IsDead)
            yield break;

        var pos = (Vector2)caster.Transform.position;
        var target = CombatSearch.NearestCharacter(pos, radius, enemyMask, _hits, filter: ch => CombatSearch.IsAlive(ch));

        if (target != null)
        {
            var packet = new Damage
            {
                amount = dmg + (caster.Stats?.Atk ?? 0f),
                kind = DamageKind.Magical,
                source = caster.Transform.gameObject,
                hitPoint = (Vector2)target.Transform.position
            };
            CombatUtility.ApplyDamage(target, packet);
        }

        co = null;
    }
    void OnDisable()
    {
        if (co != null) { StopCoroutine(co); co = null; }
    }
}
