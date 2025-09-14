using Character;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(menuName= "Skills/Elementalist/ElementalistQ")]
public class ElementalistIceSpear : SkillBase
{
    [SerializeField]
    float searchRadius = 8f;
    [SerializeField]
    LayerMask enemyMask;
    [SerializeField]
    float preDelay = 0.4f;
    [SerializeField]
    float speed = 7f;
    [SerializeField]
    float turnRate = 540f;
    [SerializeField]
    float hitRadius = 0.2f;
    [SerializeField]
    float maxLifetime = 4f;
    [SerializeField]
    float coolDown = 1f;

    [SerializeField]
    float damage = 50f;
    [SerializeField]
    float fanAngleDeg = 30f;
    [SerializeField]
    GameObject projectileVisualPrefab;

    public override KeyCode HotKey => KeyCode.Q;
    public override ActionNumber ActionNumber => ActionNumber.SkillQ;
    public override SkillTargetType Type => SkillTargetType.Point;
    public override float CoolDown => coolDown;
    public override float Range => searchRadius;
    public override float ActionTime => preDelay + 0.1f;

    static readonly Collider2D[] _hits = new Collider2D[32];

    public override void Cast(ICharacter caster, Vector2 point, ISelectable target)
    {
        if (!CanCast(caster)) return;
        base.Cast(caster, point, target);

        var tgt = CombatSearch.NearestCharacter(point, searchRadius, enemyMask, _hits, filter: CombatSearch.Alive());
        if (tgt == null) { MarkCast(); return; }

        bool empowered = caster.Transform.GetComponent<ManaBurstBuff>()?.ConsumeIfArmed() == true;
        var runner = caster.Transform.GetComponent<IceSpearRunner>();

        if (!runner)
            runner = caster.Transform.AddComponent<IceSpearRunner>();
        runner.Run(caster, tgt, preDelay, speed, turnRate, hitRadius, maxLifetime, damage, fanAngleDeg, projectileVisualPrefab, enemyMask, empowered);
        MarkCast();
    }
}
