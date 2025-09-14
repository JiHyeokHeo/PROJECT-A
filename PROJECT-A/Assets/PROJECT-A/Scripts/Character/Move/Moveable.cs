using Character;
using System.Collections.Generic;
using UnityEngine;

public class Moveable : MonoBehaviour, IMovable
{
    [SerializeField] private float defaultMoveSpeed = 5f;
    
    // 최종 목적지와의 거리 차이
    [SerializeField] private float arriveDist = 0.1f;
    
    // 웨이 포인트에 대해 "충분히 가까움" 판정 반경 
    [SerializeField] private float waypointArriveDist = 0.12f;

    [SerializeField] private int maxAllyScan = 32;
    private Collider2D[] _allyBuffer;
    // 가속/감속 계수
    [SerializeField] private float accel = 15f;

    //분리 파라미터. separationRadius 안에 있는 아군(allyMask)에 대해 서로 밀어내는 힘을 살짝 주어 겹침/비빔을 줄임.
    [SerializeField] private float separationRadius = 0.1f;
    [SerializeField] private float separationStrength = 1.0f;
    [SerializeField] private LayerMask allyMask;

    [SerializeField] private bool usePathfinder = true;
    private IPathfinder pathfinder;

    [SerializeField] private SpineSideFlip2D spineSideFlip;

    // 물리 이동 주체
    private Rigidbody2D rb;
    private readonly Queue<Vector2> waypoints = new();
    //현재 이동중인지
    private bool hasPath;
    //최종 목표
    private Vector2 finalGoal;

    // 새 명령을 연달아 찍을 때 떨림 방지
    [SerializeField] 
    private float separationPauseOnNewOrder = 0.08f;
    private float _separationResumeTime;
    private Vector2 _lastIssuedTarget;


    private IStats stats;
    public bool CanMove => true;
    public void Stop()
    {
        hasPath = false;
        waypoints.Clear();
        rb.velocity = Vector2.zero;
        finalGoal = rb.position;
    }
    public void MoveTo(Vector2 worldPos)
    {
        // 같은 지점 연타 방지
        if ((_lastIssuedTarget - worldPos).sqrMagnitude < 0.04f) 
            return;
        _lastIssuedTarget = worldPos;

        finalGoal = worldPos;
        waypoints.Clear();

        rb.velocity = Vector2.zero;
        _separationResumeTime = Time.time + separationPauseOnNewOrder;

        if (usePathfinder && pathfinder != null)
        {
            var start = rb.position;
            var path = pathfinder.FindPath(start, worldPos);
            if (path != null && path.Count > 0)
            {
                const float minStep = 0.12f;
                Vector2 prev = start;
                foreach (var p in path)
                {
                    if ((p - prev).sqrMagnitude >= minStep *  minStep)
                    {
                        waypoints.Enqueue(p);
                        prev = p;
                    }
                }
                finalGoal = prev;
                hasPath = true;
                return;
            }
   
        }
        waypoints.Enqueue(worldPos);
        hasPath = true;
    }

    void Awake()
    {
        spineSideFlip = GetComponent<SpineSideFlip2D>();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;      // 탑뷰라 중력 끄기
        rb.freezeRotation = true; // 충돌로 회전하지 않게 잠금
        _allyBuffer = new Collider2D[maxAllyScan];
        stats = GetComponent<IStats>();
        if (usePathfinder)
            pathfinder = FindObjectOfType<NavPathfinder>();
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

        if (rb.velocity.sqrMagnitude > 0.0001f)
        {
            float dot = Vector2.Dot(rb.velocity.normalized, to.normalized);
            if (dot < -0.2f) rb.velocity *= 0.85f;
        }
        Vector2 desired = (to.sqrMagnitude > 0.0001f) ? to.normalized * moveSpeed : Vector2.zero;

        // 분리 계산
        Vector2 sep = Vector2.zero;
        if (Time.time >= _separationResumeTime && separationRadius > 0.01f)
        {
            int count = Physics2D.OverlapCircleNonAlloc(
                rb.position, separationRadius, _allyBuffer, allyMask);

            for (int i = 0; i < count; i++)
            {
                var h = _allyBuffer[i];
                if (!h || h.attachedRigidbody == rb) continue;

                Vector2 away = (Vector2)rb.position - (Vector2)h.transform.position;
                float d = Mathf.Max(away.magnitude, 0.2f);
                sep += away / d; // away.normalized * (1f / d)
            }

            sep *= separationStrength * moveSpeed * 0.5f;
            sep = Vector2.ClampMagnitude(sep, moveSpeed * 0.6f);
        }

        //가감속으로 속도 보정
        Vector2 steer = (desired - rb.velocity) + sep;
        Vector2 newVel = rb.velocity + steer * Time.fixedDeltaTime * accel;
        rb.velocity = Vector2.ClampMagnitude(newVel, moveSpeed);
        spineSideFlip.FaceByVelocity(rb.velocity);
    }
}
