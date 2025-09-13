using Character;
using System.Collections;
using UnityEngine;


[CreateAssetMenu(menuName = "Skills/Elementalist/ElementalistW")]
public class ElementalistFlameRoad : SkillBase
{
    [SerializeField]
    private float range = 8f;
    [SerializeField]
    private LayerMask enemyMask;

    [SerializeField]
    private float preDelay = 0.2f;
    [SerializeField]
    private float duration = 5f;
    [SerializeField]
    private float tickInterval = 0.25f;

    [SerializeField]
    private float length = 6f;
    [SerializeField]
    private float width = 1f;
    [SerializeField]
    private float dps = 40f;

    [SerializeField]
    private float empowerDurationMul = 2f;
    [SerializeField]
    private float empowerWidthMul = 2f;

    [SerializeField]
    private GameObject stripPrefab;

    public override KeyCode HotKey => KeyCode.W;
    public override ActionNumber ActionNumber => ActionNumber.SkillW;
    public override SkillTargetType Type => SkillTargetType.Point;
    public override float CoolDown => 10f;
    public override float Range => range;
    public override float ActionTime => preDelay + 0.1f;


    public override void Cast(ICharacter caster, Vector2 point, ISelectable target)
    {
        if (!CanCast(caster)) 
            return; 
        base.Cast(caster, point, target);
        bool empowered = ((Component)caster.Transform).GetComponent<ManaBurstBuff>()?.ConsumeIfArmed() == true;
        var runner = ((Component)caster.Transform).GetComponent<FlameRoadRunner>() ?? ((Component)caster.Transform).gameObject.AddComponent<FlameRoadRunner>();
        runner.Run(caster, point, preDelay, duration, length, width, tickInterval, dps, enemyMask, empowerDurationMul, empowerWidthMul, stripPrefab, empowered);
        MarkCast();
    }


}
