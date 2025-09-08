using A;
using Character;
using UnityEngine;

public abstract class SkillBase : ScriptableObject, ISkill
{
    public abstract KeyCode HotKey { get; }
    public abstract SkillTargetType Type { get; }

    public abstract ActionNumber ActionNumber { get; }
    public abstract float ActionTime { get; }
    public abstract float CoolDown { get; }
    public abstract float Range { get; }

    private ActionLock actionLock;
    [System.NonSerialized] private float lastCastTime = -999f;

    public virtual bool CanCast(ICharacter caster)
    {
        if (caster == null || caster.Health.IsDead)
            return false;
        return Time.time >= lastCastTime + CoolDown;
    }

    public virtual void Cast(ICharacter caster, Vector2 point, ISelectable target)
    {
        if (!CanCast(caster)) return;

        var driver = caster.Driver;
        driver?.TriggerAction((int)ActionNumber);

        var lockC = caster.Lock;
        lockC.LockFor(ActionTime);

        var Moveable = caster.Movable;
        Moveable?.Stop();
    }

    protected void MarkCast()
    {
        lastCastTime= Time.time;
    }
}