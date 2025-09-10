using Character;
using System.Collections;
using UnityEngine;


public class HPFloorBuff : MonoBehaviour, IDamageModifier
{
    public float duration;
    public float floor;
    float endAt;
    ICharacter ch;

    void OnEnable()
    {
        endAt = Time.time + duration;
        ch = GetComponent<ICharacter>();
    }
    void Update()
    {
        if (Time.time >= endAt)
        {
            Destroy(this);
        }
    }

    public float ModifyDamage(float incoming, DamageKind kind, ICharacter target, GameObject src)
    {
        if (ch?.Stats == null)
            return incoming;
        var stats = ch.Stats;
        float max = ch.Stats.MaxHP;
        float cur = ch.Stats.HP;
        float min = max * floor;
        float hpAfter = Mathf.Max(0f, cur - incoming);
        if (hpAfter < min)
            return Mathf.Max(0f, cur - min);
        return incoming;
    }
}
