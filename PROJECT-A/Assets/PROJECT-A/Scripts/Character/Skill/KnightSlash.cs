using A;
using UnityEngine;

[CreateAssetMenu(menuName ="Skills/knight")]
public class KnightSlash : SkillBase
{
    [SerializeField] private float arcDeg = 90f;
    [SerializeField] private float damage = 40f;
    public override KeyCode HotKey => KeyCode.Q;
    public override SkillTargetType Type => SkillTargetType.None;
    public override float CoolDown => 3f;
    public override float Range => 1.6f;

    public override void Cast(ICharacter caster, Vector2 point, ISelectable target)
    {
        if (!CanCast(caster)) return;

        Vector2 dir = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - caster.Transform.position);
        dir.Normalize();

        var hits = Physics2D.OverlapCircleAll(caster.Transform.position, Range, LayerMask.GetMask("Enemy"));
        float cos = Mathf.Cos(arcDeg * 0.5f * Mathf.Deg2Rad);

        foreach (var h in hits)
        {
            var ch = h.GetComponent<ICharacter>();
            if (ch == null || ch.Health.IsDead) continue;

            Vector2 to = (Vector2)ch.Transform.position - (Vector2)caster.Transform.position;
            if (Vector2.Dot(dir, to.normalized) >= cos)
            {
                CombatUtility.ApplyDamage(ch, new Damage
                {
                    amount = damage + (caster.Stats?.Atk ?? 0f),
                    kind = DamageKind.Physical,
                    source = ((Component)caster.Transform).gameObject
                });
            }
        }
        MarkCast();
    }
}