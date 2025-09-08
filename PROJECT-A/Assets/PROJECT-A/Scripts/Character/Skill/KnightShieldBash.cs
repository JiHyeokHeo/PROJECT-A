//using A;
//using Character;
//using System.Collections;
//using UnityEngine;

//[CreateAssetMenu(menuName = "Skills/Knight")]
//public class KnightShieldBash : SkillBase
//{
//    public override KeyCode HotKey => KeyCode.E;
//    public override SkillTargetType Type => SkillTargetType.Point;
//    public override ActionNumber ActionNumber => ActionNumber.SkillQ;
//    public override float ActionTime => 0.35f;
//    public override float CoolDown => 15f;
//    public override float Range => 3f;
//    [SerializeField] float dashTime = 0.25f;
//    bool isDamage = false;

//    [SerializeField] float hitRadius = 0.7f;

//    public override void Cast(ICharacter caster, Vector2 point, ISelectable target)
//    {
//        if (!CanCast(caster)) return;
//        var mb = ((Component)caster.Transform).GetComponent<MonoBehaviour>();
//    }

//    IEnumerator Co(ICharacter caster, Vector2 point)
//    {
//        var go = ((Component)caster.Transform).gameObject;
//        var move = go.GetComponent<IMovable>();
//        var rb = go.GetComponent<Rigidbody2D>();

//        Vector2 dir = (point - (Vector2)caster.Transform.position).normalized;
//        float speed = Range / dashTime;

//        var parry = go.AddComponent<ParryBuff>();
//    }

//}
