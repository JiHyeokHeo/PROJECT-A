using Character;
using Sirenix.OdinInspector.Editor.TypeSearch;
using System;
using TST;
using Unity.VisualScripting;
using UnityEngine;

public interface IDamage
{
    public void ApplyDamage(float damage);
    public bool IsDead { get; }
}

namespace A
{
    public class MonsterBase : MonoBehaviour, IDamage
    {
        public MonsterConfigSO monsterConfig;
        public SpineAnimationSetSO animationSetSO;
        public SpineAnimationDriver animationDriver;
        public MonsterContext monsterContext;

        public bool isFriendly = false; // 밀리 보스몹이 존재할 경우
        public GameObject Target
        {
            get
            {
                if (target != null)
                    return target;

                return null;
            }

            set
            {
                target = value;
                if (target == null || monsterContext == null)
                    return;
                
                monsterContext.Target = target.transform;
            }
        }

        private GameObject target;
        // 일단은 MonsterBase에서 실시간 데이터 관리
        #region Temp Data 
        public float CurrentHP
        {
            get { return hp; }
            set
            {
                if (hp != value)
                    hp = value;
            }
        }

        public bool IsDead { get; private set; }

        public float hp;

        float facingSing = 1; // 왼쪽 1 오른쪽 -1
        #endregion
        public AI_Controller aI_Controller;
        public PatternScheduler patternScheduler;

        // TODO : 추후 클래스 분할 필요
        public SerializableWrapDictionary<string, MonsterWarningSign> warningSign = new SerializableWrapDictionary<string, MonsterWarningSign>();

 

        public void SetInfo(int monsterId)
        {
            // 여기다가 SO 데이터 연동하면 될듯?
        }

        private void Awake()
        {
            aI_Controller = GetComponent<AI_Controller>();
            monsterContext = new MonsterContext();
            monsterContext.Owner = this;
            monsterContext.RigidBody2D = GetComponent<Rigidbody2D>();
            animationDriver = GetComponent<SpineAnimationDriver>();
            animationDriver.animSetSO = animationSetSO;
            monsterContext.AnimationDriver = GetComponent<SpineAnimationDriver>();
            monsterContext.AnimationDriver.animSetSO = animationSetSO;
            monsterContext.Config = monsterConfig;

            patternScheduler = new PatternScheduler();
            patternScheduler.SetUp(monsterContext);
        }

        private void Start()
        {
      
        }

        void FixedUpdate()
        {
            
        }

        public void Move(Vector2 dir, float speed)
        {
            Vector2 nextPosition = monsterContext.RigidBody2D.position + dir.normalized * speed * Time.deltaTime;
            monsterContext.RigidBody2D.MovePosition(Vector2.Lerp(nextPosition, transform.position, Time.deltaTime * 10f)) ;

            int sign = dir.x <= 0 ? 1 : -1;
            if (sign != facingSing)
            {
                facingSing = sign;
                animationDriver.Flip();
            }
        }

        private void Dead()
        {
   
        }

        public void ApplyDamage(float damage)
        {
            if (IsDead)
                return;

            CurrentHP -= damage;

            GameManager.Instance.NotifyMonsterHpChanged(this, CurrentHP, monsterConfig.MaxHp);
        }
    }
}
