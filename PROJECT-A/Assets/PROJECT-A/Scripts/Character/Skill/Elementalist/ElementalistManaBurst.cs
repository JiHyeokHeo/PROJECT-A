using Character;
using System.Collections;
using UnityEngine;


[CreateAssetMenu(menuName = "Skills/Elementalist/ElementalistR")]
public class ElementalistManaBurst : SkillBase
{
    [SerializeField] float holdTimeout = 12f;

    public override KeyCode HotKey => KeyCode.R;
    public override ActionNumber ActionNumber => ActionNumber.SkillR;
    public override SkillTargetType Type => SkillTargetType.None;
    public override float CoolDown => 45f;
    public override float Range => 0f;
    public override float ActionTime => 0f;

    public override void Cast(ICharacter caster, Vector2 point, ISelectable target)
    {
        base.Cast(caster, point, target);

        var go = caster.Transform.gameObject;
        var buff = go.GetComponent<ManaBurstBuff>();
        if (!buff)
            buff = go.AddComponent<ManaBurstBuff>();
        buff.Arm(holdTimeout);

        MarkCast();
    }

}
