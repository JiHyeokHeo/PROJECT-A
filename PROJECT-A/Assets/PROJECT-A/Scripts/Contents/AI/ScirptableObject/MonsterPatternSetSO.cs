using System;
using UnityEngine;

public enum PatternID
{
    Rush,

    None,
}

namespace A
{
    // ���Ͽ� ���õ� ����
    [CreateAssetMenu(menuName = "PROJECT.A/Monster/PatternSetSO", fileName = "PatternSetSO")]
    public class MonsterPatternSetSO : ScriptableObject
    {
        public PatternID PatternID; // ���� �� ��Ʈ�� �ٲ߽ô�
        public float CoolDown;
        [Range(0, 1)] public float Weight = 0.3f; // ���ߵ� 
    }
}
