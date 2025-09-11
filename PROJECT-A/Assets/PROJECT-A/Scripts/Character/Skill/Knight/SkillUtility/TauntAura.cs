using System.Collections;
using UnityEngine;

public class TauntAura : MonoBehaviour
{
    public float radius = 4f;
    public float duration = 10f;
    public LayerMask enemyMask;
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
            return;
        }

        //var hits = Physics2D.OverlapCircleAll(transform.position, radius, enemyMask);
        //for (var h in hits)
        //{
        //    var ai = h.GetComponent < 지혁님 보스 공격 스크립트 > ();
        //    // 10초간 기사를 우선 공격
        //}
    }
}
