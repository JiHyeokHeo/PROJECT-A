using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace A
{
    public class MonsterBase : MonoBehaviour, IDamage
    {
        public MonsterConfig monsterConfig;
        public SpineAnimationSetSO animationSetSO;
        public SpineAnimationDriver animationDriver;
        MonsterContext monsterContext;

        public void ApplyDamage(float damage)
        {
            CombatSystem.Singleton.ApplyDamage(damage);
        }

        private void Awake()
        {
            monsterContext = new MonsterContext();
            monsterContext.Owner = transform;
            monsterContext.RigidBody2D = GetComponent<Rigidbody2D>();
            animationDriver = GetComponent<SpineAnimationDriver>();
            animationDriver.animSetSO = animationSetSO;
            monsterContext.AnimationDriver = GetComponent<SpineAnimationDriver>();
            monsterContext.AnimationDriver.animSetSO = animationSetSO;
            monsterContext.MonsterConfig = monsterConfig;
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.transform.root.TryGetComponent(out IDamage damageInterface))
            {
                damageInterface.ApplyDamage(monsterConfig.damage);
            }
        }
    }
}
