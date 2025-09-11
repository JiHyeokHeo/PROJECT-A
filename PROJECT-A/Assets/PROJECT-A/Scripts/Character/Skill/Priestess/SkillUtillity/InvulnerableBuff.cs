using Character;
using System.Collections;
using UnityEngine;

public class InvulnerableBuff : MonoBehaviour, IDamageModifier
{
    public float duration = 5f;
    float endAt;
    bool running;
    private void Update() 
    {
        if (running && Time.time >= endAt)
            Destroy(this); 
    }
    
    public void Refresh(float duration)
    {
        this.duration = duration;
        endAt = Time.time + duration;
        running = true;
    }

    public float ModifyDamage(float incoming, DamageKind kind, ICharacter target, GameObject src) => 0f;
}
