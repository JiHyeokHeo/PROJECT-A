using Character;
using System.Collections;
using UnityEngine;


public class HomingProjectile2D : MonoBehaviour
{
    public float speed = 12f;
    public float turnRateDeg = 360f;
    public float maxLifetime = 5f;

    public float hitRadius = 0.2f;
    public LayerMask hitMask;

    ICharacter caster;
    ICharacter target;
    Damage dmg;
    float dieAt;
    Vector2 velDir = Vector2.right;

    static readonly Collider2D[] _hits = new Collider2D[8];

    public void Init(ICharacter caster, ICharacter target, Vector2 launchDir, Damage damage,
        float speed, float turnRateDeg, float hitRadius, float maxLifetime, LayerMask hitMask)
    {
        this.caster = caster;
        this.target = target;
        this.velDir = launchDir.sqrMagnitude > 1e-6f ? launchDir.normalized : Vector2.right;
        this.dmg = damage;
        this.speed = speed;
        this.turnRateDeg = turnRateDeg;
        this.hitRadius = hitRadius;
        this.maxLifetime = maxLifetime;
        this.hitMask = hitMask;
        dieAt = Time.time + maxLifetime;
        Debug.Log($"{dieAt}은 dieAt입니다. {Time.time}은 time입니다.");
    }

    private void FixedUpdate()
    {
        if (Time.time >= dieAt)
        {
            Destroy(gameObject);
            return;
        }
        // Health로 되어 있지만 추후에 지혁님 에너미로 바꿈
        if (target != null && target.Health != null && !target.Health.IsDead)
        {
            Vector2 to = (Vector2)target.Transform.position - (Vector2)transform.position;
            if (to.sqrMagnitude > 1e-6f)
            {
                float maxRad = turnRateDeg * Mathf.Deg2Rad * Time.fixedDeltaTime;
                var cur3 = new Vector3(velDir.x, velDir.y, 0f);
                var target3 = new Vector3(to.x, to.y, 0f);
                var next3 = Vector3.RotateTowards(cur3, target3, maxRad, 0f);
                velDir = new Vector2(next3.x, next3.y).normalized;
            }
        }
        Vector2 next = (Vector2)transform.position + velDir * (speed * Time.fixedDeltaTime);
        transform.position = next;

        int n = Physics2D.OverlapCircleNonAlloc(next ,hitRadius, _hits, hitMask);
        for (int i = 0; i < n; i++)
        {
            var ch = _hits[i]?.GetComponent<ICharacter>();
            if (ch == null || ch.Health.IsDead)
                continue;
            if (target != null && ch != target)
                continue;
            CombatUtility.ApplyDamage(ch, dmg);
            Destroy(gameObject);
            return;
        }
    }
}
