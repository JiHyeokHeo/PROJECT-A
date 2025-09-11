using Character;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Knight/knightE" +
    "")]
public class KnightShieldBash : SkillBase
{
    [SerializeField]
    float damage = 60f;
    [SerializeField]
    float dashDistance = 1.5f;
    [SerializeField]
    float dashSpeed = 12f;
    [SerializeField]
    float hitRadius = 0.5f;
    [SerializeField]
    LayerMask enemyMask = LayerMask.GetMask("Enemy");
    [SerializeField]
    bool stopOnFirstHit = true;

    [SerializeField]
    float slamHold = 0.6f;
    public override KeyCode HotKey => KeyCode.E;
    public override ActionNumber ActionNumber => ActionNumber.SkillE;
    public override SkillTargetType Type => SkillTargetType.Point;

    public override float CoolDown => 3f;
    public override float Range => dashDistance;
    public override float ActionTime => slamHold + Mathf.Max(0.05f, dashDistance / Mathf.Max(0.01f, dashSpeed)) + 0.1f;
    
  

    public override void Cast(ICharacter caster, Vector2 point, ISelectable target)
    {
        base.Cast(caster, point, target);

        caster.SpineSideFlip.FaceByPoint(point);

        var go = caster.Transform.gameObject;
        var runner = go.GetComponent<ShieldBashRunner>();
        if (!runner)
            runner = go.AddComponent<ShieldBashRunner>();
        runner.Run(caster, point, damage, dashDistance, dashSpeed, hitRadius, enemyMask, stopOnFirstHit,slamHold);

        MarkCast();
    }
}
