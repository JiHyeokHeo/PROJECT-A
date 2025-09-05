using System;
using UnityEngine;

namespace A
{
    public class MonsterBase : MonoBehaviour/*, IDamage*/
    {
        public MonsterConfigSO monsterConfig;
        public SpineAnimationSetSO animationSetSO;
        public SpineAnimationDriver animationDriver;
        public MonsterContext monsterContext;

        public GameObject target;

        // 일단은 MonsterBase에서 실시간 데이터 관리
        #region Temp Data 
        public float CurrentHP
        {
            get { return hp; }
            set
            {
                if (hp != value)
                    hp = value;

                if (hp <= 0)
                    OnDead();
            }
        }
        public float hp;

        public event Action<float, Vector2> OnDamagedEvent;
        public event Action OnDeadEvent;

        float facingSing = 1; // 왼쪽 1 오른쪽 -1
        #endregion
        public AI_Controller aI_Controller;
        public PatternScheduler patternScheduler;

        public void ApplyDamage(float damage)
        {
            CombatSystem.Singleton.ApplyDamage(damage);
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
            monsterContext.MonsterConfig = monsterConfig;
            //monsterContext.Target = target.GetComponent<Rigidbody2D>();

            patternScheduler = new PatternScheduler();
            patternScheduler.SetUp(monsterContext);
        }


        void FixedUpdate()
        {
            
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            //if (other.transform.root.TryGetComponent(out IDamage damageInterface))
            //{
            //    damageInterface.ApplyDamage(monsterConfig.damage);
            //}
        }

        public void Move(Vector2 dir, float speed)
        {
            Vector2 nextPosition = monsterContext.RigidBody2D.position + dir.normalized * speed * Time.deltaTime;
            monsterContext.RigidBody2D.MovePosition(Vector2.Lerp(nextPosition, transform.position, Time.deltaTime * 10f)) ;

            int sign = dir.x <= 0 ? 1 : -1;
            if (sign != facingSing)
            {
                facingSing = sign;
                Vector3 s = transform.localScale;
                s.x *= -1;
                transform.localScale = s;
            }
        }


        void OnDamaged(float dmg, Vector2 hitDir)
        {
            // 데미지를 준 주체자를 찾는 방식이 좋을까 
        }
        
        void OnDead()
        {
            // 죽으면 fsm 체인지
            aI_Controller.AIStateChange(EAIStateId.Dead);
        }
    }
}
