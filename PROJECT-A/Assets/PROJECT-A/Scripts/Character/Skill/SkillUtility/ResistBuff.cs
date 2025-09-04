using A;
using System.Collections;
using UnityEngine;


public class ResistBuff : MonoBehaviour, IDamageModifier
{

    public float physBonus;
    public float magicBonus;
    public float duration;
    float endAt;

    private void OnEnable()
    {
        endAt = Time.time + duration; 
    }

    private void Update()
    {
        if (Time.time > endAt)
        {
            Destroy(this);
        }
    }

    public float ModifyDamage(float incoming, DamageKind kind, ICharacter target, GameObject src)
    {
        if (kind == DamageKind.Physical) return Mathf.Max(0f, incoming - physBonus);
        if (kind == DamageKind.Magical) return Mathf.Max(0f, incoming - magicBonus);
        return incoming;
    }
}
