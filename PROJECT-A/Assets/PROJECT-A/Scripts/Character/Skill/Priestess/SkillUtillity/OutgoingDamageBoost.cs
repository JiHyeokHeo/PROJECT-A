using Character;
using System.Collections;
using UnityEngine;

public class OutgoingDamageBoost : MonoBehaviour, IOutgoingDamageModifier
{

    public float duration = 5f;
    public float multiplier = 1.5f;

    float endAt;
    bool running;

    void Update() 
    {
        if (running && Time.time >= endAt)
        {
            Destroy(this);
        }
    }

    public void Refresh(float duration, float multiplier)
    {
        this.duration = duration;
        this.multiplier = multiplier;
        endAt = Time.time + duration;
        running = true;
    }

    public float ModifyOutgoing(float baseAmount, ICharacter attacker, ICharacter target, ref DamageKind kind) => baseAmount * multiplier;

}
