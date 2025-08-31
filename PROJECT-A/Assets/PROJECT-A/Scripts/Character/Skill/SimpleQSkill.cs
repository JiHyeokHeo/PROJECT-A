using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace A
{
    [CreateAssetMenu(menuName = "Skills/SimpleQSkill")]
    public class SimpleQSkill : SkillBase
    {
        [SerializeField] KeyCode key = KeyCode.Q;
        [SerializeField] float coolDown = 1f;

        public override KeyCode HotKey => key;
        public override SkillTargetType Type => SkillTargetType.None;
        public override float CoolDown => coolDown;

        public override void Cast(ICharacter caster, Vector2 point, ISelectable target)
        {
            if (!CanCast(caster)) return;
            Debug.Log("Do SimpleQSkill");
            MarkCast();
        }

    }
}
