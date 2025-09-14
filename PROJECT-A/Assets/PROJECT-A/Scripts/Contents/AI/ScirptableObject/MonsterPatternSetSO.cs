using System;
using UnityEngine;

public enum EPatternID
{
    Rush,
    Smash,
    ColdBeam,
    None,
}

public enum EMonsterID
{
    CopyBara = 10000000,
}

namespace A
{
    // ���Ͽ� ���õ� ����
    [CreateAssetMenu(menuName = "PROJECT.A/Monster/PatternSetSO", fileName = "PatternSetSO")]
    public class MonsterPatternSetSO : ScriptableObject
    {
        [Header("���� �⺻ ����")]
        public int monsterID;
        public EPatternID PatternID; // ���� �� ��Ʈ�� �ٲ߽ô�
        public float CoolDown;
        [Tooltip("0 = ����, 1 �̻� = ���� �׷� ID")]
        public int CooldownGroupId; 
        public float AttackRange;
        [Range(0, 1)] public float Weight = 0.3f; // ���ߵ� 
        public GameObject ProjectilePrefab;
    }
}
