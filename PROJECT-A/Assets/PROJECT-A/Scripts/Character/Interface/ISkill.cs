using UnityEngine;
using Character;
public interface ISkill
{
    KeyCode HotKey { get; }
    SkillTargetType Type { get; }
    
    float Range { get; }
    float CoolDown { get; }
    bool CanCast(ICharacter caster);
    void Cast(ICharacter caster, Vector2 point, ISelectable target);
}
