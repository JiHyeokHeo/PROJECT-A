using Character;
using System.Collections;
using UnityEngine;


[CreateAssetMenu(menuName = "Skills/Elementalist/ElementalistE")]
public class ElementalistLightning : SkillBase
{

    [SerializeField] 
    float preDelay = 0.2f; 
    [SerializeField] 
    float searchRadius = 8f; 
    [SerializeField] 
    LayerMask enemyMask; 
    [SerializeField] 
    float damage = 70f;
    [SerializeField]
    float empowerDamageMul = 1.5f; 
    [SerializeField] 
    bool empowerCounter = true;

    public override KeyCode HotKey => KeyCode.E;
    public override ActionNumber ActionNumber => ActionNumber.SkillE;
    public override SkillTargetType Type => SkillTargetType.None;
    public override float CoolDown => 10f;
    public override float Range => searchRadius;
    public override float ActionTime => preDelay + 0.1f;

    public override void Cast(ICharacter caster, Vector2 point, ISelectable target)
    {
        if (!CanCast(caster)) return; base.Cast(caster, point, target);
        bool empowered = ((Component)caster.Transform).GetComponent<ManaBurstBuff>()?.ConsumeIfArmed() == true;
        var runner = ((Component)caster.Transform).GetComponent<LightningRunner>() ?? ((Component)caster.Transform).gameObject.AddComponent<LightningRunner>();
        runner.Run(caster, preDelay, searchRadius, enemyMask, damage, empowerDamageMul, empowerCounter, empowered);
        MarkCast();
    }
}
