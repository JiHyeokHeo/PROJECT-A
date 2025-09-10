using System;
using UnityEngine;

public enum EPatternID
{
    Rush,

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
        public int monsterID;
        public EPatternID PatternID; // ���� �� ��Ʈ�� �ٲ߽ô�
        public float CoolDown;
        [Range(0, 1)] public float Weight = 0.3f; // ���ߵ� 
    }
}
