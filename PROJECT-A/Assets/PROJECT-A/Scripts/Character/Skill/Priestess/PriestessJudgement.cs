using Character;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Priestess/PriestessQ")]
public class PriestessJudgement : SkillBase
{
    [SerializeField]
    float damage = 80f;
    [SerializeField]
    float preDelay = 0.35f;
    [SerializeField]
    float searchRadius = 6f;
    [SerializeField]
    LayerMask enemyMask;
    public override KeyCode HotKey => KeyCode.Q;
    public override ActionNumber ActionNumber => ActionNumber.SkillQ;
    public override SkillTargetType Type => SkillTargetType.None;
    public override float CoolDown => 15f;
    public override float Range => searchRadius;
    public override float ActionTime => preDelay;

    public override void Cast(ICharacter caster, Vector2 point, ISelectable target)
    {
        base.Cast(caster, point, target);

        var go = caster.Transform.gameObject;
        var runner = go.GetComponent<JudgementRunner>();
        if (runner == null)
            runner = go.AddComponent<JudgementRunner>();
        runner.Run(caster, damage, preDelay, searchRadius, enemyMask);

        MarkCast();
    }
}
