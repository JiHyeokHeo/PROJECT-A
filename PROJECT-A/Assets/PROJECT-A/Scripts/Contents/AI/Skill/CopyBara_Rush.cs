using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

namespace A
{
    public class CopyBara_Rush : MonsterPattern
    {
        public override float Cooldown => 10.0f;
        public float castingTime = 3.0f;

        // 3초간 캐스팅하며 붉은 반투명한 선으로 범위 표시.
        // 투명도 50의 붉은 범위 표시 후, 투명도 70의 범위가 캐스팅 시간에
        // 걸쳐 차오르고 공격.범위가 다 차면 그 방향으로 돌진하며 돌진 중 접촉하는 플레이어에게 피해.

        public override async UniTask Execute(CancellationToken ct)
        {
            float t = 0;
            while (t < castingTime)
            {
                // 혹시라도 캔슬 요청이 들어오면 캔슬 시켜라
                ct.ThrowIfCancellationRequested();
                // 붉은 화면 뜨도록 설정

                CustomEvent.Trigger(MonsterContext.Owner.gameObject, "Switch", ECopyBaraAttackPattern.Rush);

                t += Time.deltaTime;
                Debug.Log("붉은화면 Logging Cancel 가능!");
                await UniTask.Yield(ct);
            }

            // CustomEvent -> 연동
            float dashTime = 1.0f;                                        
            Vector2 start = MonsterContext.RigidBody2D.position;
            Vector2 end = MonsterContext.Target.position;

            CustomEvent.Trigger(MonsterContext.Owner.gameObject, "Switch", ECopyBaraAttackPattern.Rush);
            MonsterContext.RigidBody2D.velocity = Vector2.zero; // 잔여 속도 제거
            float e = 0f;
            while (e < dashTime)
            {
                ct.ThrowIfCancellationRequested();
                e += Time.fixedDeltaTime;
                MonsterContext.RigidBody2D.MovePosition(Vector2.Lerp(start, end, e / dashTime));
                await UniTask.WaitForFixedUpdate(ct); // 물리 루프 타이밍에 맞춰 이동
            }
        }

        public override void OnSetup(MonsterContext monsterContext)
        {
            MonsterContext = monsterContext;
        }
    }
}
