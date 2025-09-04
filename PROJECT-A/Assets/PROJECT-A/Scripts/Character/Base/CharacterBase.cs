using Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace A
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public abstract class CharacterBase : MonoBehaviour, ICharacter
    {
        public Transform Transform => transform;
        public IStats Stats { get; private set; }
        public IHealth Health { get; private set; }
        public IMovable Movable { get; private set; }
        public IStateMachine StateMachine { get; private set; }
        public ISkillSet SkillSet { get; private set; }
        public CharacterCombat CharacterCombat { get; private set; }
       

        void Awake()
        {
            Stats = GetComponent<IStats>();
            Health = GetComponent<IHealth>();
            Movable = GetComponent<IMovable>();
            StateMachine = GetComponent<IStateMachine>();
            SkillSet = GetComponent<ISkillSet>();
            CharacterCombat = GetComponent<CharacterCombat>();
        }

        protected virtual void OnEnable()
        {
            if (Health != null)
            {
                Health.OnDead += HandleDeath;
            }
        }
        protected virtual void OnDisable()
        {
            if (Health != null)
            {
                Health.OnDead -= HandleDeath;
            }
        }
        protected virtual void HandleDeath()
        {
            StateMachine.ChangeState(CharacterState.Dead);
        }
    }
}
