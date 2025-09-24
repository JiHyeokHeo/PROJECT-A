using UnityEngine;

namespace Character
{


    public interface ISkill
    {
        KeyCode HotKey { get; }
        SkillKind Kind { get; }
        SkillTargetType TargetType { get; }
        
        bool IsOn { get; }
        bool IsCoolingDown { get; }
        float RemainingCooldown { get; }

        void Initialize(ICharacter owner);
        bool CanCast(ICharacter caster, SkillCastContext ctx);
        void Cast(ICharacter caster, SkillCastContext ctx);

        void Tick(ICharacter caster, float dt);
    }
}