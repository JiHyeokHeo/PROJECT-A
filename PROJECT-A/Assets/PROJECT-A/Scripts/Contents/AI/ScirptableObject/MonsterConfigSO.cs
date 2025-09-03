using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace A
{
    [CreateAssetMenu(menuName = "PROJECT.A/Monster/MonsterConfigSO", fileName = "MonsterConfigSO")]
    public class MonsterConfigSO : ScriptableObject
    {
        [Header("�⺻ ����")]
        public float maxHp = 100f;
        public float MoveSpeed = 3.0f;
        public float damage = 1.0f;

        [Header("�þ�/��Ÿ�")]
        public float DetectRange = 12f;
        public float AttackRange = 3.5f;
        public float ChaseStopRange = 1.5f;

        // ���� ����
        public PatternSetSO patternSetSO;
    }
}
