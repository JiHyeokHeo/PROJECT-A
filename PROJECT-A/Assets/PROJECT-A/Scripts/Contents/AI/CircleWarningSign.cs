using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace A
{
    public class CircleWarningSign : MonsterWarningSign
    {
        public override void SetData(MonsterContext context)
        {
            Vector2 start = context.RigidBody2D.position;
            Vector2 end = context.Target.position;
            Vector2 dir = end - start;

            
            float distance = (start - end).magnitude;
            distance *= 3;     // grid ¸ÂÃã
            distance += 2;     // grid ¸ÂÃã
            outer.transform.localScale = new Vector3(distance, distance, distance);
        }
    }
}
