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

                if (hp <= 0)
                    OnDead();
            }
        }
        public float hp;

        //public event Action<float, Vector2> OnDamagedEvent;
        //public event Action OnDeadEvent;

        float facingSing = 1; // 왼쪽 1 오른쪽 -1
        #endregion
        public AI_Controller aI_Controller;
        public PatternScheduler patternScheduler;

        // TODO : 추후 클래스 분할 필요
        public SerializableWrapDictionary<string, MonsterWarningSign> warningSign = new SerializableWrapDictionary<string, MonsterWarningSign>();

        public void ApplyDamage(float damage)
        {
            CombatSystem.Singleton.ApplyDamage(damage);
        }

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
        //    // 데미지를 준 주체자를 찾는 방식이 좋을까 
        //}
        
        void OnDead()
        {
            
            
        }

        private void OnGUI()
        {
            
        }
    }
}
