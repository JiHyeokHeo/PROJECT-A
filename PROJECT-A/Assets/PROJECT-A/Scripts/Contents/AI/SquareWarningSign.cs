using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace A
{
    public class SquareWarningSign : MonsterWarningSign
    {
        public override void SetData(MonsterContext context, MonsterPatternSetSO data)
        {
            Vector2 start = context.RigidBody2D.position;
            Vector2 end = context.Target.position;
            Vector2 dir = end - start;

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            outer.transform.localRotation = Quaternion.Euler(0, 0, angle);
            float distance = (start - end).magnitude;
            outer.transform.localScale = new Vector3(distance, 1, 1);
        }
    }
}
