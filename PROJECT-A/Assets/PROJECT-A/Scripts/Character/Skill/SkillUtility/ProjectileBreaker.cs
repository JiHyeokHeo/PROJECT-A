using System.Collections;
using UnityEngine;

public class ProjectileBreaker : MonoBehaviour
{
    public float duration = 10f;
    float endAt;

    private void OnEnable()
    {
        endAt = Time.time + duration;
    }

    private void Update()
    {
        if (Time.time >= endAt)
            Destroy(this);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 투사체 인터페이스
        // 닿으면 소멸
    }
}
