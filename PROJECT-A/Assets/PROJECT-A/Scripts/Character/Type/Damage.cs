using UnityEngine;

public enum DamageKind
{
    Physical, //방어력
    Magical, // 마저
    True,// 고정뎀
    Projectile, // 투사체 판정
}

public struct Damage
{
    public float amount; // "원래" 데미지 숫자
    public DamageKind kind; // 물리/마법/고정/투사체
    public GameObject source; // 가해자 GO (어그로/가시뎀/생흡 등 출처 추적)
    public Vector2 hitPoint; // 맞은 지점
}