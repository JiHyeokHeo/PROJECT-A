using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
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

        public override void Enter()
        {
            if (monster == null)
                return;
            var token = monster.monsterContext.CancellationToken.Token;
            // 이전에 하던 패턴 중지
            //monster.monsterContext.CancellationToken.Cancel();
            //monster.monsterContext.CancellationToken.Dispose(); // 멀티스레드에서 사용할때 관련된 메모리 이슈  가이드라인에서는 권장

            Run(token).Forget();
        }

        public override void Exit()
        {
            // 혹시 모르니 나갈때도 캔슬
            monster.monsterContext.CancellationToken.Cancel();
        }   


        // 공격 상황에서도 Event처리를 한다? 
         
        public override void Tick(float dt)
        {
            // 몬스터의 공격 기능을 AttackState에서 관리 // 세부 구현은 override를 통한 다양성 확보
            //monster.Attack();
            // Casting Rush
            // Casting Smash
            // Melt
            // Melt_Idle

        }

        async UniTaskVoid Run(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                await monster.patternScheduler.ExecuteNext(ct);      // 준비된 패턴 1개 실행(+쿨타임 대기 포함)
                // 사거리/상태 조건 체크해서 추격 등으로 빠지는 로직 추가 가능
            }
        }

    }
}
