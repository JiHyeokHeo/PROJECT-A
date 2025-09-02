using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace A
{
    [CreateAssetMenu(menuName = "PROJECT.A/Monster/MonsterConfig", fileName = "MonsterConfig")]
    public class MonsterConfig : ScriptableObject
    {
        [Header("기본 스탯")]
        public float maxHp = 100f;
        public float MoveSpeed = 3.0f;
        public float damage = 1.0f;

        [Header("시야/사거리")]
        public float DetectRange = 12f;
        public float AttackRange = 3.5f;
        public float ChaseStopRange = 1.5f;
    }
}
