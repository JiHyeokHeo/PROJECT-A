using Character;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Knight/KnightW")]
public class KnightProtector : SkillBase
{
    public override KeyCode HotKey => KeyCode.W;
    public override SkillTargetType Type => SkillTargetType.None;
    public override float CoolDown => 30f;
    public override ActionNumber ActionNumber => ActionNumber.SkillW;
    public override float ActionTime => 0.35f;
    public override float Range => 4f;

    [SerializeField] float duration = 10f;
    [SerializeField] float resist = 20f;
    public override void Cast(ICharacter caster, Vector2 point, ISelectable target)
    {
        base.Cast(caster, point, target);

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
