using A;
using UnityEngine;

public abstract class SkillBase : ScriptableObject, ISkill
{
    public abstract KeyCode HotKey { get; }
    public abstract SkillTargetType Type { get; }
    public abstract float CoolDown { get; }
    public abstract float Range { get; }

    private float lastCastTime = -999f;
    public virtual bool CanCast(ICharacter caster)
    {
        if (caster == null || caster.Health.IsDead)
            return false;
        return Time.time >= lastCastTime + CoolDown;
    }

    public abstract void Cast(ICharacter caster, Vector2 point, ISelectable target);
    protected void MarkCast()
    {
        lastCastTime= Time.time;
    }
}