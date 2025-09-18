using UnityEngine;

namespace A
{
    [CreateAssetMenu(menuName = "PROJECT.A/Monster/MonsterConfigSO", fileName = "MonsterConfigSO")]
    public class MonsterConfigSO : ScriptableObject
    {
        [Header("기본 스탯")]
        public float MaxHp;
        public float MoveSpeed;
        public float Damage;

        [Header("시야/사거리")]
        public float DetectRange;
        public float AttackRange;
        public float ChaseStopRange;
        public bool isBoss;

        // 몬스터 패턴
        public MonsterPatternSetSO[] PatternSO;
    }
}
