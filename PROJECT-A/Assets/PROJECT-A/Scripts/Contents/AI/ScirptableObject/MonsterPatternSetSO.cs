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
    // 패턴에 관련된 에셋
    [CreateAssetMenu(menuName = "PROJECT.A/Monster/PatternSetSO", fileName = "PatternSetSO")]
    public class MonsterPatternSetSO : ScriptableObject
    {
        [Header("패턴 기본 정보")]
        public int monsterID;
        public EPatternID PatternID; // 추후 뭐 인트로 바꿉시다
        public float CoolDown;
        [Tooltip("0 = 독립, 1 이상 = 공유 그룹 ID")]
        public int CooldownGroupId; 
        public float AttackRange;
        [Range(0, 1)] public float Weight = 0.3f; // 가중도 
        public GameObject ProjectilePrefab;
    }
}
