using Character;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Priestess/PriestessE")]
public class PriestessPray : SkillBase
{
    [SerializeField]
    float searchRadius = 8f;
    [SerializeField]
    LayerMask allyMask;

    [SerializeField]
    float duration = 10f;
    [SerializeField]
    float tickInterval = 1f;
    [SerializeField]
    float percentPerTick = 0.04f;
    [SerializeField]
    private float coolDown = 10f;

    public override KeyCode HotKey => KeyCode.E;
    public override ActionNumber ActionNumber => ActionNumber.SkillE;
    public override SkillTargetType Type => SkillTargetType.None;
    public override float CoolDown => coolDown;
    public override float Range => searchRadius;
    public override float ActionTime => 0.3f;

    public override void Cast(ICharacter caster, Vector2 point, ISelectable target)
    {
        base.Cast(caster, point, target);

        var go = caster.Transform.gameObject;
        var runner = go.GetComponent<PrayRunner>();
        if (!runner)
            runner = go.AddComponent<PrayRunner>();
        MarkCast();
    }
}
