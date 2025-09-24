using System.Collections.Generic;
using UnityEngine;

namespace Character
{
    public static class SkillSetExtensions
    {
        public static bool TryGetByKey(this ISkillSet set, KeyCode key, ICharacter caster, out ISkill skill)
        {
            skill = null;
            if (set == null) return false;

            foreach (var s in set.Skills)
            {
                if (s.HotKey == key && s.CanCast(caster, default))
                {
                    skill = s;
                    return true;
                }
            }
            return false;
        }

        public static IEnumerable<ISkill> GetByKind(this ISkillSet set, SkillKind kind)
        {
            if (set == null) yield break;
            foreach (var s in set.Skills)
                if (s.Kind == kind) yield return s;
        }
    }
}