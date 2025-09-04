using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace A
{
    // 패턴에 관련된 에셋
    [CreateAssetMenu(menuName = "PROJECT.A/Monster/PatternSetSO", fileName = "PatternSetSO")]
    public class PatternSetSO : ScriptableObject
    {
        public WeightedPattern[] Patterns;
    }

    [Serializable]
    public class WeightedPattern
    {
        [SerializeReference] public MonsterPattern Pattern;
        [Range(0, 1)] public float Weight = 0.3f; // 가중도 
    }
}
