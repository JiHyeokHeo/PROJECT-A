using System.Collections.Generic;
using UnityEngine;

namespace Character
{
    public struct SkillCastContext
    {
        public Vector2 Point;
        public ISelectable Selectable;
        private List<ICharacter> _targets;
        public bool HasAnyTarget => _targets != null && _targets.Count > 0;
        public IReadOnlyList<ICharacter> Targets => _targets;

        public void AddTarget(ICharacter target)
        {
            if (_targets == null)
            {
                _targets = new();
            }
            _targets.Add(target);
        }

        public static SkillCastContext Auto(ICharacter caster, SkillDefinition def)
        {
            var ctx = new SkillCastContext();
            TargetResolver.FillTargets(caster, def, ctx);
            return ctx;
        }
    }
}