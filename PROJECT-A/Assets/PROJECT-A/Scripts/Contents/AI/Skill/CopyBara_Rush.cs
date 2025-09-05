using Cysharp.Threading.Tasks;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

namespace A
{
    public class CopyBara_Rush : MonsterPattern
    {
        // 3�ʰ� ĳ�����ϸ� ���� �������� ������ ���� ǥ��.
        // ���� 50�� ���� ���� ǥ�� ��, ���� 70�� ������ ĳ���� �ð���
        // ���� �������� ����.������ �� ���� �� �������� �����ϸ� ���� �� �����ϴ� �÷��̾�� ����.

        public override async UniTask Execute(CancellationToken ct)
        {
            float t = 0;
            CustomEvent.Trigger(context.Owner.gameObject, "Switch", ECopyBaraAttackPattern.RushReady);
            while (t < castingTime)
            {
                // Ȥ�ö� ĵ�� ��û�� ������ ĵ�� ���Ѷ�
                ct.ThrowIfCancellationRequested();
                // ���� ȭ�� �ߵ��� ����

                t += Time.deltaTime;
                Debug.Log("����ȭ�� Logging Cancel ����!");
                await UniTask.Yield(ct);
            }

            // CustomEvent -> ����
            Vector2 start = context.RigidBody2D.position;
            Vector2 end = context.Target.position;
            Vector2 dir = end - start;

            CustomEvent.Trigger(context.Owner.gameObject, "Switch", ECopyBaraAttackPattern.Rush);
            context.RigidBody2D.velocity = Vector2.zero; // �ܿ� �ӵ� ����
         
            while ((context.RigidBody2D.position - end).sqrMagnitude > 1.0f)
            {
                ct.ThrowIfCancellationRequested();

                context.Owner.Move(dir, context.MonsterConfig.MoveSpeed);
                await UniTask.WaitForFixedUpdate(ct);
            }
            CustomEvent.Trigger(context.Owner.gameObject, "Switch", ECopyBaraAttackPattern.RushDone);
        }

        public override void Init(MonsterContext context, MonsterPatternSetSO data)
        {
            this.context = context;
            cooldown = data.CoolDown;
            weight = data.Weight;
            castingTime = 3.0f;
        }
    }
}
