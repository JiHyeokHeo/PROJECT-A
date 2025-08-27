using A;
using UnityEngine;

public interface ISkill
{
    KeyCode HotKey { get; }
    SkillTargetType Type { get; }
    float CoolDown { get; }
    bool CanCast(ISkillUser caster);
    void Cast(ISkillUser caster, Vector2 point, ISelectable target);

}
