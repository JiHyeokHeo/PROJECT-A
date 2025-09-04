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
        public float MaxHp;
        public float MoveSpeed;
        public float Damage;

        [Header("�þ�/��Ÿ�")]
        public float DetectRange;
        public float AttackRange;
        public float ChaseStopRange;

        // ���� ����
        public PatternSetSO PatternSO;
    }
}
