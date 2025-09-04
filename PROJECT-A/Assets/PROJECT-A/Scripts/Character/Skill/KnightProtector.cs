using A;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Knight")]
public class KnightProtector : SkillBase
{
    public override KeyCode HotKey => KeyCode.W;
    public override SkillTargetType Type => SkillTargetType.None;
    public override float CoolDown => 30f;
    public override float Range => 4f;

    [SerializeField] float duration = 10f;
    [SerializeField] float resist = 20f;
    public override void Cast(ICharacter caster, Vector2 point, ISelectable target)
    {
        if (!CanCast(caster))
            return;

        var go = ((Component)caster.Transform).gameObject;

        var r = go.AddComponent<ResistBuff>();
        r.physBonus = resist;
        r.magicBonus = resist;
        r.duration = duration;

        var pb = go.AddComponent<ProjectileBreaker>();
        pb.duration = duration;

        var taunt = go.AddComponent<TauntAura>();
        taunt.radius = Range;
        taunt.duration = duration;
        taunt.enemyMask = LayerMask.GetMask("Enemy");
        MarkCast();
    }
}
