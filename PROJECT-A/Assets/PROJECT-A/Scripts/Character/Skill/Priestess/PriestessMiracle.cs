using Character;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Priestess/PriestessR")]
public class PriestessMiracle : SkillBase
{
    [SerializeField] float searchRadius = 10f;
    [SerializeField] LayerMask allyMask;

    [SerializeField] float invulnDuration = 5f;
    [SerializeField] float atkBoostPercent = 0.5f;

    public override KeyCode HotKey => KeyCode.R;
    public override ActionNumber ActionNumber => ActionNumber.SkillR;
    public override SkillTargetType Type => SkillTargetType.None;
    public override float CoolDown => 60f;
    public override float Range => searchRadius;
    public override float ActionTime => 0.4f;

    public override void Cast(ICharacter caster, Vector2 point, ISelectable target)
    {
        base.Cast(caster, point, target);

        var go = caster.Transform.gameObject;
        var runner = go.GetComponent<MiracleRunner>();
        if (!runner) 
            runner = go.AddComponent<MiracleRunner>();


        runner.Run(
            caster,
            searchRadius,
            allyMask,
            invulnDuration, 1f + atkBoostPercent);

        MarkCast();
    }

}
