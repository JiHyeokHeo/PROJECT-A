using Character;
using System.Collections;
using UnityEngine;



public class ResurrectionRunner : MonoBehaviour
{
    ICharacter caster;

    float delay;
    float allySearchRadius;
    LayerMask allyMask;
    float revivePercent;

    [Header("Timing")]
    [SerializeField] float preDelay = 0.35f;

    float tauntRadius;
    LayerMask enemyMask;

    [SerializeField] int maxAllyScan = 32;
    [SerializeField] int maxEnemyScan = 32;

    Collider2D[] _allyHits;
    Collider2D[] _enemyHits;

    Coroutine co;

    private void Awake()
    {
        _allyHits = new Collider2D[maxAllyScan];
        _enemyHits = new Collider2D[maxEnemyScan];
    }

    public void Run(
        ICharacter caster,
        float preDelay,
        float searchRadius, LayerMask allyMask, float revivePercent,
        float tauntRadius, LayerMask enemyMask)
    {
        this.caster = caster;
        this.delay = preDelay;
        this.allySearchRadius = searchRadius;
        this.allyMask = allyMask;
        this.revivePercent = revivePercent;
        this.tauntRadius = tauntRadius;
        this.enemyMask = enemyMask;


        if (_allyHits == null || _allyHits.Length != maxAllyScan) _allyHits = new Collider2D[maxAllyScan];
        if (_enemyHits == null || _enemyHits.Length != maxEnemyScan) _enemyHits = new Collider2D[maxEnemyScan];

        if (co != null)
            StopCoroutine(co);
        co = StartCoroutine(Co());
    }

    IEnumerator Co()
    {
        if (delay > 0f) yield return new WaitForSeconds(delay);

        if (caster == null || caster.Health == null)
        {
            co = null;
            yield break;
        }

        Vector2 cpos = caster.Transform.position;

        ICharacter deadTarget = CombatSearch.NearestCharacter(
            cpos,
            allySearchRadius,
            allyMask,
            _allyHits,
            CombatSearch.Dead());

        if (deadTarget != null)
        {
            var health = deadTarget.Health;
            if (health != null && health.TryResurrect(revivePercent))
            {
                int ne = Physics2D.OverlapCircleNonAlloc(cpos, tauntRadius, _enemyHits, enemyMask);
                for (int i = 0; i < ne; i++)
                {
                    var enemy = _enemyHits[i];
                    if (!enemy)
                        continue;
                    // 여기에 모든 보스가 나를 공격 대상으로 지정하는 알고리즘 추가 (전투 시스템 개발 시)
                }
            }
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
