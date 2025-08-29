using UnityEngine;
using Character;
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor.Validation;

public class Moveable : MonoBehaviour, IMovable
{
    [SerializeField] private float defaultMoveSpeed = 5f;
    
    // 최종 목적지와의 거리 차이
    [SerializeField] private float arriveDist = 0.1f;
    
    // 웨이 포인트에 대해 "충분히 가까움" 판정 반경 
    [SerializeField] private float waypointArriveDist = 0.12f;
    
    // 가속/감속 계수
    [SerializeField] private float accel = 15f;

    //분리 파라미터. separationRadius 안에 있는 아군(allyMask)에 대해 서로 밀어내는 힘을 살짝 주어 겹침/비빔을 줄임.
    [SerializeField] private float separationRadius = 0.6f;
    [SerializeField] private float separationStrength = 1.0f;
    [SerializeField] private LayerMask allyMask;

    // 물리 이동 주체
    private Rigidbody2D rb;
    
    private readonly Queue<Vector2> waypoints = new();
    
    //현재 이동중인지
    private bool hasPath;

    //최종 목표
    private Vector2 finalGoal;

    private IStats stats;
    public bool CanMove => true;

    
    public void MoveTo(Vector2 worldPos)
    {
        finalGoal = worldPos;
        waypoints.Clear();
        waypoints.Enqueue(worldPos);
        hasPath = true;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;      // 탑뷰라 중력 끄기
        rb.freezeRotation = true; // 충돌로 회전하지 않게 잠금

        stats = GetComponent<IStats>();
    }

    void FixedUpdate()
    {
        // 경로가 없으면 부드럽게 정지
        if (!hasPath)
        {
            rb.velocity = Vector2.MoveTowards(rb.velocity, Vector2.zero, accel * Time.fixedDeltaTime);
            return;
        }

        //웨이포인트 없거나 최종 도착 체크
        if (waypoints.Count == 0)
        {
            if (Vector2.Distance(rb.position, finalGoal) <= arriveDist)
            {
                hasPath = false;
                rb.velocity = Vector2.MoveTowards(rb.velocity, Vector2.zero, accel * Time.fixedDeltaTime);
                return;
            }
            waypoints.Enqueue(finalGoal);
        }

        //현재 타깃와 방향/거리
        var target = waypoints.Peek();
        Vector2 to = target - rb.position;
        float dist = to.magnitude;

        //웨이포인트 도착 판정
        if (dist <= waypointArriveDist)
        {
            waypoints.Dequeue();
            if (waypoints.Count > 0)
            {
                target = waypoints.Peek();
                to = target - rb.position;
            }
        }
        //현재 속도 목표 계산
        float moveSpeed = (stats != null ? stats.Speed : defaultMoveSpeed);

        Vector2 desired = (to.sqrMagnitude > 0.0001f) ? to.normalized * moveSpeed : Vector2.zero;

        // 분리 계산
        Vector2 sep = Vector2.zero;
        if (separationRadius > 0.01f)
        {
            var hits = Physics2D.OverlapCircleAll(rb.position, separationRadius, allyMask);
            foreach (var h in hits)
            {
                if (h.attachedRigidbody == rb)
                    continue;
                Vector2 away = (Vector2)rb.position - (Vector2)h.transform.position;
                float d = away.magnitude;
                if (d > 0.0001f)
                    sep += away.normalized * (1f / d);
            }
            sep *= separationStrength * moveSpeed * 0.5f;
        }

        //가감속으로 속도 보정
        Vector2 steer = (desired - rb.velocity) + sep;

        Vector2 newVel = rb.velocity + steer * Time.fixedDeltaTime * accel;
        newVel = Vector2.ClampMagnitude(newVel, moveSpeed);
        rb.velocity = newVel;
    }
}
