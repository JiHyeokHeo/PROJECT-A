using UnityEngine;
using System;
using System.Collections.Generic;

namespace Character
{
    [DefaultExecutionOrder(-50)]
    public class CharacterBase : MonoBehaviour, ICharacter
    {
        [SerializeField]
        string id;
        [SerializeField]
        Faction faction;

        public string Id => id;
        public Faction Faction => faction;
        public Transform Transform => transform;

        public IStats Stats { get; private set; }
        public IHealth Health { get; private set; }
        public IStateMachine StateMachine { get; private set; }

        readonly Dictionary<Type, object> _capacity = new();

        public event Action OnCapabilitiesReady;

        void Reset()
        {
            if (string.IsNullOrEmpty(id)) 
                id = Guid.NewGuid().ToString("N");
        }
       
        void Awake()
        {
            Stats = GetComponentInChildren<IStats>();
            Health = GetComponentInChildren<IHealth>();
            StateMachine = GetComponentInChildren<IStateMachine>();

            OnCapabilitiesReady?.Invoke();
        }
        
        public T GetCapability<T>() where T : class
        {
            if (_capacity.TryGetValue(typeof(T), out var t))
            {
                return (T)t;
            }
            return null;
        }

        public void RegisterCapability(Type t, object inst)
        {
            if (t == null || inst == null)
                return;
            _capacity[t] = inst;
        }

        public void RegisterCapability<T>(T inst) where T : class
        {
            RegisterCapability(typeof(T), inst);
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
