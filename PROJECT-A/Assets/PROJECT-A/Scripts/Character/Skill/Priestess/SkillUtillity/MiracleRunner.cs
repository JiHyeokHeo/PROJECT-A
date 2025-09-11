using Character;
using System.Collections;
using UnityEngine;


public class MiracleRunner : MonoBehaviour
{
    ICharacter caster;

    float searchRadius;
    LayerMask allyMask;

    float invulnDuration;
    float atkMultiplier;

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
        float searchRadius,
        LayerMask allyMask,
        float invulnDuration,
        float atkMultiplier)
    {
        this.caster = caster;
        this.searchRadius = searchRadius;
        this.allyMask = allyMask;
        this.invulnDuration = invulnDuration;
        this.atkMultiplier = atkMultiplier;

        if (_allyHits == null ||  _allyHits.Length != maxAllyScan) 
            _allyHits = new Collider2D[maxAllyScan];

        if (co != null)
            StopCoroutine(co);
        co = StartCoroutine(Co());
    }

    IEnumerator Co()
    {
        if (caster == null || caster.Health == null)
        {
            co = null;
            yield break;
        }
        Vector2 pos = caster.Transform.position;
        var Target = CombatSearch.NearestCharacter(pos, searchRadius, allyMask, _allyHits, filter: ch => CombatSearch.IsAlive(ch));

        if (Target == null)
            Target = caster;

        var heal = Target.Health;
        if (heal != null)
            heal.HealPercent(1f);

        var go = caster.Transform.gameObject;
        var inv = go.AddComponent<InvulnerableBuff>();
        inv.Refresh(invulnDuration);

        var boost = go.AddComponent<OutgoingDamageBoost>();
        boost.Refresh(invulnDuration, atkMultiplier);

        co = null;
        yield break;
    
    }

    void OnDisable()
    {
        if (co != null) { StopCoroutine(co); co = null; }
    }
}
