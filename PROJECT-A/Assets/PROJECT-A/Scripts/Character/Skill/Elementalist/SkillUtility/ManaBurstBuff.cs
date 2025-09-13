using System.Collections;
using UnityEngine;

public class ManaBurstBuff : MonoBehaviour
{
    [SerializeField]
    float timeout = 10f;
    float endAt;
    bool armed;

    public void Arm(float holdSeconds)
    {
        timeout = Mathf.Max(0f, holdSeconds <= 0f ? timeout : holdSeconds);
        endAt = Time.time + timeout;
        armed = true;
        enabled = true;
    }

    public bool ConsumeIfArmed()
    {
        if (!armed)
            return false;
        armed = false;
        Destroy(this);
        return true;
    }

    private void Update()
    {
        if (armed && timeout > 0f && Time.time >= endAt)
            Destroy(this);
    }

}
