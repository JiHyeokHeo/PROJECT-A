using TST;
using UnityEngine;

namespace A
{
    public class MonsterBase : MonoBehaviour
    {
        public MonsterConfigSO monsterConfig;
        public SpineAnimationSetSO animationSetSO;
        public SpineAnimationDriver animationDriver;
        public MonsterContext monsterContext;

        public bool isFriendly = false; // �и� �������� ������ ���
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

        //public event Action<float, Vector2> OnDamagedEvent;
        //public event Action OnDeadEvent;

        float facingSing = 1; // ���� 1 ������ -1
        #endregion
        public AI_Controller aI_Controller;
        public PatternScheduler patternScheduler;

        // TODO : ���� Ŭ���� ���� �ʿ�
        public SerializableWrapDictionary<string, MonsterWarningSign> warningSign = new SerializableWrapDictionary<string, MonsterWarningSign>();

        public void ApplyDamage(float damage)
        {
            CombatSystem.Singleton.ApplyDamage(damage);
        }

        public void SetInfo(int monsterId)
        {
            // ����ٰ� SO ������ �����ϸ� �ɵ�?
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

        //void OnDamaged(float dmg, Vector2 hitDir)
        //{
        //    // �������� �� ��ü�ڸ� ã�� ����� ������ 
        //}
        
        void OnDead()
        {
            
            
        }

        private void OnGUI()
        {
            
        }
    }
}
