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
            // ���� ��Ʈ ���¿��� 15�� ���� ���� �� ��Ʈ Done�� ���� ���߿� �����ϸ� Flinch ����
            // ���� ���� ; �� ������ �ʵ忡 ���� �� ������ 2���� �����ϸ�
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
                await UniTask.Yield(ct); // ������Ʈ
            }

            // ���� ��û�� �޾Ҵٸ�?
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
