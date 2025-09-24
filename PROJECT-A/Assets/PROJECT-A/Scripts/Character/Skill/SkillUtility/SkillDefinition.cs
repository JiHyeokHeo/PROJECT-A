using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Character
{
    public enum SkillKind
    {
        Passive,          // 항상 적용되는 패시브 효과
        Macro,            // 매크로/자동 시퀀스(연계 스킬, 조합 등)
        ActiveImmediate,  // 즉시 발동형 액티브 (즉발기, 공격/힐/버프 등)
        ActiveToggle      // 온/오프 전환형 (오라, 지속 버프 등)
    }
    
    public enum SkillTargetType
    {
        None,       // 대상 없음 (셀프 버프, 즉발기 등)
        Point,      // 특정 지점 지정
        Self,       // 자기 자신
        Ally,       // 아군 유닛
        Enemy,      // 적 유닛
        Area,       // 범위 내 무작위/다수
        Object      // 설치물, 구조물 등
    }
    public enum TargetingPolicy
    {
        Required,       // 반드시 대상 지정 필요 (예: 힐, 단일 공격기)
        Optional,       // 대상 없어도 가능 (예: 포인트 지정 가능 AoE)
        AutoPickInArea  // 자동으로 범위 내 적/아군 선택
    }
    public enum UseDistance
    {
        Self,       // 자기 자신
        Melee,      // 근접 (1~2m)
        Ranged,     // 원거리 (사거리 기반)
        Aura,       // 아군/적 범위에 지속 효과 (자신 중심 오라)
        Targeted,   // 특정 타겟 지정 사거리
        AoE         // 범위 지정 (포인트 기반)
    }
    public enum ShapeType
    {
        Point,      // 점 대상 (단일)
        Circle,     // 원형 범위
        Sector,     // 부채꼴
        Rectangle,  // 직사각형
        Line        // 직선 (광선, 투사체 경로 등)
    }
    [System.Flags]
    public enum TagMask
    {
        None    = 0,
        Physical = 1 << 0,
        Magical  = 1 << 1,
        Buff     = 1 << 2,
        Debuff   = 1 << 3,
        CC       = 1 << 4,   // 군중제어 (Stun)
        Weapon   = 1 << 5,
        Heal     = 1 << 6,
        Shield   = 1 << 7
    }
    
    
    [CreateAssetMenu(menuName = "Skill/Defintion")]
    public class SkillDefinition : ScriptableObject
    {
        public string id;
        public SkillKind kind;
        public SkillTargetType targetType;
        public TargetingPolicy targetingPolicy;
        public UseDistance useDistance;
        public ShapeType shapeType;
        public float range;
        public float radius, angle, length, width;
        public float cooldown;
        public float duration;
        public float tickInterval;
        public bool useProjectile;
        public ProjectileDefintion Projectile;
        public List<EffectDefinition> effects;
        public TagMask Tags;
        public bool WaitForAnimation;
    }
}