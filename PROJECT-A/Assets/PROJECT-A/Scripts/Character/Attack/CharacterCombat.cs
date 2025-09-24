using Character;
using System.Collections;
using UnityEngine;

public class CharacterCombat : MonoBehaviour, ICapability, ITickable
{
    [SerializeField] private LayerMask enemyMask;

    [Header("테스트용 하드코딩")]
    [SerializeField] 
    private float attackRange = 1.5f; // 평타 사거리
    [SerializeField] 
    private float attackWindup = 0.12f; // 평타 휘두르는 속도
    [SerializeField] 
    private float attackCooldown = 0.8f; // 평타 쿨타임
    [SerializeField] 
    private float baseDamage = 15f; // 기본 데미지
    
    [SerializeField]
    private bool autoAggro = true;
    [SerializeField]
    private float retargetCooldown = 0.2f;
    private float _nextRetargetTime = 0f;

    public ICharacter Self { get; private set; } // 시전자
    IMovable _movable;
    ActionLock _lock;
    CharacterAnimatorDriver _anim;
    SpineSideFlip2D _flip;
    AggroSensor _sensor;


    private ICharacter _currentTarget; // 타겟
    private bool _attackMoveActive; // A 무브 활성화 여부
    private Vector2 _attackMoveDest;

    //직전프레임에 타겟 추격중이었으면 true
    private bool _wasChasingTarget;
    private bool _isAttacking;
    private float _nextAttackReady;

    public void CancelAll()
    {
        StopAllCoroutines();
        _isAttacking = false;
        _attackMoveActive = false;
        _currentTarget = null;
        _wasChasingTarget = false;
        _movable.Stop();
    }

    private void Awake()
    {
        Self = GetComponentInParent<ICharacter>(true);
        _movable = Self.GetCapability<IMovable>();
        _lock = Self.GetCapability<ActionLock>();
        _anim = Self.GetCapability<CharacterAnimatorDriver>();
        _flip = Self.GetCapability<SpineSideFlip2D>();
        _sensor = Self.GetCapability<AggroSensor>();
    }

    public void Tick(float dt)
    {
        if (_currentTarget != null && _currentTarget.Health.IsDead)
            _currentTarget = null;

        if (_wasChasingTarget && _currentTarget == null)
        {
            _attackMoveActive = false;
            StopHere();
            _wasChasingTarget = false;
            return;
        }

        if (_currentTarget == null)
        {
            if (_sensor is not null)
            {
               if (Time.time >= _nextRetargetTime)
               {
                    _nextRetargetTime = Time.time + retargetCooldown;

                    _sensor.Prune();

                    ICharacter best = null;
                    float bestD = float.PositiveInfinity;
                    foreach (var c in _sensor.InRange)
                    {
                        if (c == null || c.Health ==null || c.Health.IsDead)
                            continue;
                        float dist = Vector2.Distance(transform.position, c.Transform.position);
                        if (dist < bestD)
                        {
                            bestD = dist;
                            best = c;
                        }
                    }
                    if (best != null)
                        SetTarget(best);
                }
            }

            if (_attackMoveActive && _currentTarget == null)
            {
                _movable.MoveTo(_attackMoveDest);
            }
            if (_currentTarget == null)
            {
                _wasChasingTarget = false;
                return;
            }
        }
        float r = attackRange * 0.98f;
        float r2 = r * r;
        float d2 = ((Vector2)transform.position - (Vector2)_currentTarget.Transform.position).sqrMagnitude;

        if (d2 > r2)
        {
            _movable.MoveTo(_currentTarget.Transform.position);
            _wasChasingTarget = true;
        }
        else
        {
            TryBasicAttack();
            _wasChasingTarget = true;
        }

    }
    public void IssueAttackMove(Vector2 dest)
    {
        _attackMoveActive = true;
        _attackMoveDest = dest;
        _currentTarget = null;
        _movable.MoveTo(dest);
    }
    public void IssueAttackTarget(ICharacter target)
    {
        _attackMoveActive = false;
        _currentTarget = target;
    }

    public void SetTarget(ICharacter t)
    {
        _attackMoveActive = false;
        _currentTarget = t;
    }
    private void StopHere()           
    {
        _movable.Stop();
        _isAttacking = false;        
    }

    private void TryBasicAttack()
    {
        if (_isAttacking) return;
        if (Time.time < _nextAttackReady)
            return;
        if (_lock is not null && _lock.IsLocked)
            return;
        StartCoroutine(CoBasicAttack());
    }

    IEnumerator CoBasicAttack()
    {
        _isAttacking = true;
        _movable.Stop();

        _anim.TriggerAction((int)ActionNumber.Attack);
        _lock.LockFor(0.25f);

        yield return new WaitForSeconds(attackWindup);

        if (_currentTarget != null && !_currentTarget.Health.IsDead)
        {
            _flip?.FaceByPoint(_currentTarget.Transform.position);
            var atk = Self.Stats.Atk;
            var aspd = Self?.Stats?.AtkSpeed;

            var dmg = new Damage
            {
                Amount = baseDamage + atk,
                Kind = DamageKind.Physical,
                Source = gameObject
            };
            CombatUtility.ApplyDamage(_currentTarget, dmg);
            _nextAttackReady = Time.time + attackCooldown / Mathf.Max(0.1f, (Self?.Stats?.AtkSpeed?? 1f));
        }
        
        _isAttacking = false;
    }
}
