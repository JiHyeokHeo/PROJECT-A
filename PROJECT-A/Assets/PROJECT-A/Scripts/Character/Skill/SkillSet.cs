using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character
{
    public class SkillSet : MonoBehaviour, ISkillSet
    {
        [SerializeField]
        SkillBase skillQ;
        [SerializeField]
        SkillBase skillW;
        [SerializeField]
        SkillBase skillE;
        [SerializeField]
        SkillBase skillR;

        public IEnumerable<ISkill> Skills
        {
            get
            {
                if (skillQ) yield return skillQ;
                if (skillW) yield return skillW;
                if (skillE) yield return skillE;
                if (skillR) yield return skillR;
            }
        }
        public ISkill Get(SkillSlot slot) => slot switch
        {
            SkillSlot.Q => skillQ,
            SkillSlot.W => skillW,
            SkillSlot.E => skillE,
            SkillSlot.R => skillR,
            _ => null
        };
        // 런타임에 슬롯을 바꿀경우
        public void Bind(SkillSlot slot, ISkill skill)
        {
            var def = skill as SkillBase;
            switch (slot)
            {
                case SkillSlot.Q: skillQ = def; break;
                case SkillSlot.W: skillW = def; break;
                case SkillSlot.E: skillE = def; break;
                case SkillSlot.R: skillR = def; break;
            }
        }
        public bool TryCast(SkillSlot slot, ICharacter caster)
        {
            var s = Get(slot);
            if (s == null)
                return false;
            if (!s.CanCast(caster))
                return false;
            s.Cast(caster, caster.Transform.position, null);
            return true;
        }
    }
}
