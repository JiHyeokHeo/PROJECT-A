using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace A
{
    public class CircleWarningSign : MonsterWarningSign
    {
        public override void SetData(MonsterContext context, MonsterPatternSetSO data)
        {
            Vector2 start = context.RigidBody2D.position;
            Vector2 end = context.Target.position;
            Vector2 dir = end - start;
            float attackRange = data.AttackRange;
            
            float distance = (start - end).magnitude;
            attackRange *= 3;     // grid ¸ÂÃã
            attackRange += 2;     // grid ¸ÂÃã
            outer.transform.localScale = new Vector3(attackRange, attackRange, attackRange);
        }
    }
}
