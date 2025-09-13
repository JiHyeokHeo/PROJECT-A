using Character;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Priestess/PriestessW")]
public class PriestessResurrection : SkillBase
{
    [SerializeField]
    float searchRadius = 10f;
    [SerializeField]
    LayerMask allyMask;
    [SerializeField]
    float reviveHPPercent = 0.30f;
    [SerializeField]
    float tauntRadius = 8f; // 어그로 범위
    [SerializeField]
    LayerMask enemyMask;
    [SerializeField]
    float preDelay = 0.25f;

    public override KeyCode HotKey => KeyCode.W;
    public override ActionNumber ActionNumber => ActionNumber.SkillW;
    public override SkillTargetType Type => SkillTargetType.None;
    public override float CoolDown => 60f;
    public override float Range => searchRadius;
    public override float ActionTime => 0.55f;

    public override void Cast(ICharacter caster, Vector2 point, ISelectable target)
    {
        base.Cast(caster, point, target);

        var go = caster.Transform.gameObject;
        var runner = go.GetComponent<ResurrectionRunner>();
        if (!runner)
            runner = go.AddComponent<ResurrectionRunner>();

        runner.Run(
            caster,
            preDelay,
            searchRadius, allyMask, Mathf.Clamp01(reviveHPPercent),
            tauntRadius, enemyMask);
        MarkCast();

    }

}
