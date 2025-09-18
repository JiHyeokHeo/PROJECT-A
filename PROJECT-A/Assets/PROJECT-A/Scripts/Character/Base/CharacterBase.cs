using UnityEngine;
using System;
using System.Collections.Generic;

namespace Character
{
    [DefaultExecutionOrder(-50)]
    public class CharacterBase : MonoBehaviour, ICharacter
    {
        [SerializeField]
        string id = Guid.NewGuid().ToString();
        public Transform Transform => transform;
        public IStats Stats { get; private set; }
        public IHealth Health { get; private set; }
        public IStateMachine StateMachine { get; private set; }

        readonly Dictionary<Type, object> capacity = new();
       
        void Awake()
        {
            Stats = GetComponent<IStats>();
            Health = GetComponent<IHealth>();
            StateMachine = GetComponent<IStateMachine>();

            var monos = GetComponentsInChildren<MonoBehaviour>(true);
            foreach (var mb in monos)
            {
                if (mb is not ICapacity)
                    continue;

            }
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
