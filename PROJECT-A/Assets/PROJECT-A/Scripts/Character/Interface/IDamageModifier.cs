using UnityEngine;
using Character;
// 만약 지혁님이 만든 인터페이스 즉 보스가 ICharacter가 아니라면 수정
public interface IDamageModifier
{
    float ModifyDamage(float incoming, DamageKind kind, ICharacter target, GameObject src);
}

public interface IOutgoingDamageModifier
{
    float ModifyOutgoing(float baseAmount, ICharacter attacker, ICharacter target, ref DamageKind kind);
}