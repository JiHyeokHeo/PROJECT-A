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

        // �ϴ��� MonsterBase���� �ǽð� ������ ����
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


        #endregion
        AI_Controller aI_Controller;
        public PatternScheduler patternScheduler;

        public void ApplyDamage(float damage)
        {
            CombatSystem.Singleton.ApplyDamage(damage);
        }

        private void Awake()
        {
            aI_Controller = GetComponent<AI_Controller>();
            monsterContext = new MonsterContext();
            monsterContext.Owner = transform;
            monsterContext.RigidBody2D = GetComponent<Rigidbody2D>();
            animationDriver = GetComponent<SpineAnimationDriver>();
            animationDriver.animSetSO = animationSetSO;
            monsterContext.AnimationDriver = GetComponent<SpineAnimationDriver>();
            monsterContext.AnimationDriver.animSetSO = animationSetSO;
            monsterContext.MonsterConfig = monsterConfig;
            monsterContext.Target = target.transform;

            patternScheduler = new PatternScheduler();
            patternScheduler.SetUp(monsterContext);
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            //if (other.transform.root.TryGetComponent(out IDamage damageInterface))
            //{
            //    damageInterface.ApplyDamage(monsterConfig.damage);
            //}
        }

        void OnDamaged(float dmg, Vector2 hitDir)
        {
            // �������� �� ��ü�ڸ� ã�� ����� ������ 
        }
        
        void OnDead()
        {
            // ������ fsm ü����
            aI_Controller.AIStateChange(EAIStateId.Dead);
        }
    }
}
