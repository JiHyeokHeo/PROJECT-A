using Character;
using System.Collections;
using UnityEngine;


public class PrayRunner : MonoBehaviour
{
    ICharacter caster;

    float searchRadius;
    LayerMask allyMask;

    float duration;
    float tickInterval;
    float percentPerTick;

    [SerializeField]
    int maxAllyScan = 32;
    Collider2D[] _allyHits;

    Coroutine co;

    private void Awake()
    {
        _allyHits = new Collider2D[maxAllyScan];
    }

    public void Run(
        ICharacter caster,
        float searchRadius, LayerMask allyMask,
        float duration, float tickInterval, float percentPerTick)
    {
        this.caster = caster;
        this.searchRadius = searchRadius;
        this.allyMask = allyMask;
        this.duration = duration;
        this.tickInterval = tickInterval;
        this.percentPerTick = percentPerTick;

        if (_allyHits == null || _allyHits.Length == maxAllyScan)
            _allyHits = new Collider2D[maxAllyScan];

        if (co != null)
            StopCoroutine(co);
        co = StartCoroutine(Co());
    }

    IEnumerator Co()
    {
        if (caster == null || caster.Health == null || caster.Health.IsDead)
        {
            co = null;
            yield break;
        }
        Vector2 pos = caster.Transform.position;
        var target = CombatSearch.NearestCharacter(
            pos, searchRadius, allyMask, _allyHits,
            filter: ch => CombatSearch.IsAlive(ch));

        if (target != null)
        {
            var go = target.Transform.gameObject;
            var hot = go.GetComponent<PeriodicHealBuff>();
            if (!hot)
                hot = go.AddComponent<PeriodicHealBuff>();

            hot.StartOrRefresh(target, duration, tickInterval, percentPerTick);
                
        }

        co = null;
    }
    void OnDisable()
    {
        if (co != null) { StopCoroutine(co); co = null; }
    }

}
