using Character;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Knight/KnightR")]
public class KnightPride : SkillBase
{
    public override KeyCode HotKey => KeyCode.R;
    public override SkillTargetType Type => SkillTargetType.None;
    public override ActionNumber ActionNumber => ActionNumber.SkillR;
    public override float ActionTime => 0.35f;
    public override float CoolDown => 90f;
    public override float Range => 5f;

    [SerializeField] float duration = 10f;
    [SerializeField] float floorPercent = 0.1f;

    public override void Cast(ICharacter caster, Vector2 point, ISelectable target)
    {
        base.Cast(caster, point, target);   

        var hits = Physics2D.OverlapCircleAll(caster.Transform.position, Range, LayerMask.GetMask("Ally"));
        foreach (var h in hits)
        {
            var ch = h.GetComponent<ICharacter>();
            if (ch == null)
                continue;
            var buff = ((Component)ch.Transform).gameObject.AddComponent<HPFloorBuff>();
            buff.duration = duration;
            buff.floor = floorPercent;
        }
        MarkCast();
    }
}
