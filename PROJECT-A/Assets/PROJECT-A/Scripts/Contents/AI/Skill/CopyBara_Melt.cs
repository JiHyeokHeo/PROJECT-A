using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TST;
using Unity.VisualScripting;
using UnityEngine;

namespace A
{
    public class CopyBara_Melt : MonsterPattern
    {
        public override async UniTask<bool> Execute(CancellationToken ct)
        {
            // 만약 멜트 상태에서 15초 동안 추적 후 멜트 Done을 실행 도중에 파훼하면 Flinch 실행
            // 해제 조건 ; 빛 영역이 필드에 생성 빛 영역은 2개가 등장하며
            Vector2 start = context.RigidBody2D.position;
            Vector2 end = context.Target.position;
            Vector2 dir = end - start;

            CustomEvent.Trigger(context.Owner.gameObject, "Switch", ECopyBaraAttackPattern.Melt);
            float t = 0f;
            while (t < 15f)
            {
                ct.ThrowIfCancellationRequested();

                t += Time.deltaTime;
                context.Owner.Move(dir, context.Config.MoveSpeed);
                await UniTask.Yield(ct); // 업데이트
            }

            // 만약 요청을 받았다면?
            if (ct.IsCancellationRequested)
                CustomEvent.Trigger(context.Owner.gameObject, "Switch", ECopyBaraAttackPattern.MeltFlinch);

            return true;
        }

        public override bool CheckExecuteCondition()
        {
            if (context.Owner.CurrentHP / context.Config.MaxHp < 0.5f)
                return true;

            return false;
        }
    }

   
}
