using System;
using UnityEngine;

public enum PatternID
{
    Rush,

    None,
}

namespace A
{
    // 패턴에 관련된 에셋
    [CreateAssetMenu(menuName = "PROJECT.A/Monster/PatternSetSO", fileName = "PatternSetSO")]
    public class MonsterPatternSetSO : ScriptableObject
    {
        public PatternID PatternID; // 추후 뭐 인트로 바꿉시다
        public float CoolDown;
        [Range(0, 1)] public float Weight = 0.3f; // 가중도 
    }
}
