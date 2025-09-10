using Character;
using System.Collections;
using UnityEditor;
using UnityEngine;


public class CharacterCombat : MonoBehaviour
{
    [SerializeField] private LayerMask enemyMask;

    [Header("테스트용 하드코딩")]
    [SerializeField] private float attackRange = 1.5f; // 평타 사거리
    [SerializeField] private float attackWindup = 0.12f; // 평타 휘두르는 속도
    [SerializeField] private float attackCooldown = 0.8f; // 평타 쿨타임
    [SerializeField] private float baseDamage = 15f; // 기본 데미지

    public ICharacter Self { get; private set; } // 시전자

    private ICharacter currentTarget; // 타겟

    private bool attackMoveActive; // A 무브 활성화 여부
    private Vector2 attackMoveDest;

    //직전프레임에 타겟 추격중이었으면 true
    private bool wasChasingTarget = false;
    private bool isAttacking = false;
    private float nextAttackReady;

    public void CancelAll()
    {
        StopAllCoroutines();
        isAttacking = false;
        //currentTarget.Movable?.Stop();
        currentTarget = null;
        attackMoveActive = false;
        Self = null;
    }

    private void Update()
    {
        if (Self == null || Self.Health == null)
            return;

        // 타깃이 죽었으면 참조 해제
        if (currentTarget != null && currentTarget.Health.IsDead)
            currentTarget = null;

        if (wasChasingTarget && currentTarget == null)
        {
            attackMoveActive = false;
            StopHere();
            wasChasingTarget=false;
            return;
        }

        if (attackMoveActive && currentTarget == null)
        {
            var t = FindNearestEnmy(transform.position, attackRange);
            if (t != null)
                SetTarget(t);
            else
                Self.Movable?.MoveTo(attackMoveDest);
        }
        if (currentTarget != null)
        {
            float d = Vector2.Distance(transform.position, currentTarget.Transform.position);
            if (d > attackRange * 0.98f)
            {
                Self.Movable?.MoveTo(currentTarget.Transform.position);
                wasChasingTarget = true;
            }
            else
            {
                TryBasicAttack();
                wasChasingTarget = true;
            }
        }
        else
        {
            if (!attackMoveActive) wasChasingTarget = false;
        }
    }

    public void IssueAttackMove(Vector2 dest, ICharacter caster)
    {
        Self = caster;
        attackMoveActive = true;
        attackMoveDest = dest;
        currentTarget = null;
        Self.Movable?.MoveTo(dest);
    }

    public void SetTarget(ICharacter t)
    {
        attackMoveActive = false;
        currentTarget = t;
    }

    public ICharacter FindNearestEnmy(Vector2 pos, float radius)
    {
        var hits = Physics2D.OverlapCircleAll(pos, radius, enemyMask);

        float best = float.PositiveInfinity;
        ICharacter cho = null;
        foreach (var h in hits)
        {
            var ch = h.GetComponent<ICharacter>();
            if (ch == null || ch.Health.IsDead) continue;
            float d = Vector2.Distance(pos, ch.Transform.position);
            if (d < best)
            {
                best = d;
                cho = ch;
            }
        }
        return cho;
    }
    private void StopHere()           
    {
        Self.Movable.Stop();
        isAttacking = false;        
    }
    public void IssueAttackTarget(ICharacter target, ICharacter caster)
    {
        Self = caster;
        attackMoveActive = false;
        currentTarget = target;
    }
    private void TryBasicAttack()
    {
        if (isAttacking) return;
        if (Time.time < nextAttackReady)
            return;
        if (Self.Lock != null && Self.Lock.IsLocked)
            return;
        StartCoroutine(CoBasicAttack());
    }

    IEnumerator CoBasicAttack()
    {
        isAttacking = true;
        Self.Movable?.Stop();

        Self.Driver?.TriggerAction((int)ActionNumber.Attack);
        Self.Lock?.LockFor(0.25f);

        yield return new WaitForSeconds(attackWindup);

        var flipper = Self.SpineSideFlip;
        if (currentTarget != null && !currentTarget.Health.IsDead)
        {
            flipper?.FaceByPoint(currentTarget.Transform.position);
            var dmg = new Damage { amount = baseDamage + (Self?.Stats?.Atk ?? 0f), kind = DamageKind.Physical, source = gameObject };
            CombatUtility.ApplyDamage(currentTarget, dmg);
        }
        nextAttackReady = Time.time + attackCooldown / Mathf.Max(0.1f, (Self?.Stats?.AtkSpeed?? 1f));
        isAttacking = false;
    }
}
