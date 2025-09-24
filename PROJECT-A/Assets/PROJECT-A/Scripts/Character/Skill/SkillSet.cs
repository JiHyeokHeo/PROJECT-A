using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character
{
    [DefaultExecutionOrder(-5)]
    public class SkillSet : MonoBehaviour, ISkillSet, ICapability, ITickable
    {
        [SerializeField] private List<MonoBehaviour> _skillBehaviours = new();
        private readonly List<ISkill> _skills = new();
        private ICharacter _owner;

        [SerializeField] private int _maxTotalSlots = 2;
        public IEnumerable<ISkill> Skills => _skills;

        private void Awake()
        {
            _owner = GetComponentInParent<ICharacter>(true);
            _skills.Clear();
            foreach (var mb in _skillBehaviours)
            {
                if (mb is ISkill s)
                {
                    s.Initialize(_owner);
                    _skills.Add(s);
                }
            }
            EnforceSlotRules();
        }

        private void EnforceSlotRules()
        {
            int macroCount = 0;
            int activeCount = 0;
            foreach (var s in _skills)
            {
                if (s.Kind == SkillKind.Macro) macroCount++;
                else if (s.Kind == SkillKind.ActiveImmediate || s.Kind == SkillKind.ActiveToggle) activeCount++;
            }
            if (activeCount > 2 || (activeCount == 2 && macroCount > 0) || 
                (activeCount + macroCount) > _maxTotalSlots)
            {
                Debug.LogWarning($"[SkillSet] Slot rule violation on {_owner}. Active:{activeCount}, Macro:{macroCount}");
            }
        }

        public void Tick(float dt)
        {
            foreach (var s in _skills)
                s.Tick(_owner, dt);
        }
    }
}
