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
    public float Amount; // "원래" 데미지 숫자
    public DamageKind Kind; // 물리/마법/고정/투사체
    public GameObject Source; // 가해자 GO (어그로/가시뎀/생흡 등 출처 추적)
    public Vector2 HitPoint; // 맞은 지점
}