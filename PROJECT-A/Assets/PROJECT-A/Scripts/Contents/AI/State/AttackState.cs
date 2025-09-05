using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace A
{
    public class AttackState : AIState
    {
        public AttackState(MonsterBase monster) : base(monster)
        {
            //this.monster = monster;
        }

        public override EAIStateId aiStateId => EAIStateId.Attack;
        Collider2D[] overlappedObjects = new Collider2D[4]; 

        public override void Enter()
        {
            if (monster == null)
                return;
            monster.monsterContext.ResetToken();
            var token = monster.monsterContext.CancellationToken.Token;
            // 이전에 하던 패턴 중지
            //monster.monsterContext.CancellationToken.Cancel();
            //monster.monsterContext.CancellationToken.Dispose(); // 멀티스레드에서 사용할때 관련된 메모리 이슈  가이드라인에서는 권장

            Run(token).Forget();
        }

        public override void Exit()
        {
            // 혹시 모르니 나갈때도 캔슬
            monster.monsterContext.CancellationToken?.Cancel();
            monster.monsterContext.CancellationToken?.Dispose();
        }   


        // 공격 상황에서도 Event처리를 한다? 
         
        public override void Tick(float dt)
        {
            Physics2D.OverlapCircleNonAlloc(monster.monsterContext.RigidBody2D.position, monster.monsterConfig.AttackRange, overlappedObjects);

            if (overlappedObjects.Length > 0) 
            {
                monster.monsterContext.Target = overlappedObjects[0].transform;
            }
        }

        // 공격 패턴 지속
        async UniTaskVoid Run(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                MonsterContext context = monster.monsterContext;
                if (context.Target == null)
                {
                    await UniTask.Yield(PlayerLoopTiming.Update, ct); // 한 프레임 대기
                    continue;
                }
                Vector2 monsterPos = context.RigidBody2D.position;
                Vector2 targetPos = context.Target.position;

                float sqrTargetRange= (targetPos - monsterPos).sqrMagnitude;
                if (context.SqrAttackRange >= sqrTargetRange)
                {
                    await monster.patternScheduler.ExecuteNext(ct);      // 준비된 패턴 1개 실행(+쿨타임 대기 포함)
                }

                if (context.SqrChaseStopRange <= sqrTargetRange &&
                    monster.monsterContext.Target != null)
                {
                    monster.Move(targetPos - monsterPos, monster.monsterConfig.MoveSpeed);
                }
                
                // 사거리/상태 조건 체크해서 추격 등으로 빠지는 로직 추가 가능
                await UniTask.Yield(PlayerLoopTiming.FixedUpdate, ct);
            }
        }
    }
}
