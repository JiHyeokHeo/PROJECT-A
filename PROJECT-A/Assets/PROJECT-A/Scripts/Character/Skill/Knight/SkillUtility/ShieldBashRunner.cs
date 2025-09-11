using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character
{
    public class ShieldBashRunner : MonoBehaviour
    {
        ICharacter caster;
        Rigidbody2D rb;
        Moveable move;
        Vector2 dir;

        float dmg, dist, speed, radius,preDelay;
        LayerMask enemyMask;
        bool stopOnFirstHit;

        readonly HashSet<ICharacter> hitOnce = new();
        static readonly Collider2D[] _hits = new Collider2D[16];

        Coroutine co;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            move = GetComponent<Moveable>();
        }

        public void Run(ICharacter caster, Vector2 point, float damage, float dashDistance, float dashSpeed, float hitRadius, LayerMask enemyMask, bool stopOnFirstHit, float slamHold)
        {
            this.caster = caster;
            this.dmg = damage;
            this.dist = dashDistance;
            this.speed = dashSpeed;
            this.radius = hitRadius;
            this.enemyMask = enemyMask;
            this.stopOnFirstHit = stopOnFirstHit;
            this.preDelay = slamHold;

            Vector2 to = point - (Vector2)rb.position;
            dir = to.sqrMagnitude > 1e-6f ? to.normalized : Vector2.zero;
            hitOnce.Clear();

            if (move)
            {
                move.Stop();
                move.enabled = false;
            }

            co = StartCoroutine(CoDash());
        }

        IEnumerator CoDash()
        {
            float remain = dist;
            var wait = new WaitForFixedUpdate();
            float t = preDelay;

            while (t > 0f)
            {
                if (caster == null || caster.Health == null || caster.Health.IsDead) { Finish(); yield break; }
                rb.velocity = Vector2.zero;
                t -= Time.fixedDeltaTime;
                yield return wait;
            }

            while (remain > 0 )
            {
                if (caster == null || caster.Health == null || caster.Health.IsDead)
                    break;

                float step = Mathf.Min(speed * Time.fixedDeltaTime, remain);
                Vector2 next = rb.position + dir * step;

                int n = Physics2D.OverlapCircleNonAlloc(next, radius, _hits, enemyMask);
                bool hitNow = false;

                for (int i = 0; i < n; i++)
                {
                    var ch = _hits[i]?.GetComponent<ICharacter>();
                    if (ch == null || ch.Health.IsDead)
                        continue;
                    if (!hitOnce.Add(ch))
                        continue;

                    var dmgPacket = new Damage
                    {
                        amount = dmg + (caster.Stats?.Atk ?? 0f),
                        kind = DamageKind.Physical,
                        source = caster.Transform.gameObject,
                        hitPoint = _hits[i].bounds.ClosestPoint(next),
                    };
                    CombatUtility.ApplyDamage(ch, dmgPacket);
                    hitNow = true;
                    
                    if (stopOnFirstHit)
                    {
                        remain = 0f;
                        break;
                    }
                }
                rb.MovePosition(next);
                remain -= step;

                if (stopOnFirstHit && hitNow)
                    break;
                yield return wait;
            }
            Finish();
            co = null;
        }
        void Finish()
        {
            if (move)
            {
                move.enabled = true;
                move.Stop();
                rb.velocity = Vector2.zero;
            }
        }

        private void OnDisable()
        {
            if (co != null)
            {
                StopCoroutine(co);
                co = null;
            }
            Finish();
        }
    }
}