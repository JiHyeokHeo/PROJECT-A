using System.Collections;
using UnityEngine;
using Character;
using Unity.VisualScripting;


public class PeriodicHealBuff : MonoBehaviour
{
    public float duration = 10f;
    public float tickInterval = 1f;
    public float percentPerTick = 4f;
    public ICharacter caster;

    float endAt;
    IHealth heal;
    Coroutine co;

    public void StartOrRefresh(ICharacter caster, float duration, float tickInterval, float percentPerTick)
    {
        this.caster = caster;
        this.duration = duration;
        this.tickInterval = tickInterval;
        this.percentPerTick = percentPerTick;

        if (heal == null)
            heal = caster.Health;
        endAt = Time.time + duration;

        if (co != null)
            StopCoroutine(co);
        co = StartCoroutine(Co());
    }

    private IEnumerator Co()
    {
        var wait = new WaitForSeconds(tickInterval);
        while (Time.time < endAt)
        {
            if (heal != null)
                heal.HealPercent(percentPerTick);
            yield return wait;
        }
        co = null;
    }

    void OnDisable()
    {
        if (co != null) { StopCoroutine(co); co = null; }
    }
}
