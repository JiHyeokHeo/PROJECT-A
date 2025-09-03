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

        // 3�ʰ� ĳ�����ϸ� ���� �������� ������ ���� ǥ��.
        // ���� 50�� ���� ���� ǥ�� ��, ���� 70�� ������ ĳ���� �ð���
        // ���� �������� ����.������ �� ���� �� �������� �����ϸ� ���� �� �����ϴ� �÷��̾�� ����.

        public override async UniTask Execute(CancellationToken ct)
        {
            float t = 0;
            while (t < castingTime)
            {
                // Ȥ�ö� ĵ�� ��û�� ������ ĵ�� ���Ѷ�
                ct.ThrowIfCancellationRequested();
                // ���� ȭ�� �ߵ��� ����

                CustomEvent.Trigger(MonsterContext.Owner.gameObject, "Switch", ECopyBaraAttackPattern.Rush);

                t += Time.deltaTime;
                Debug.Log("����ȭ�� Logging Cancel ����!");
                await UniTask.Yield(ct);
            }

            // CustomEvent -> ����
            float dashTime = 1.0f;                                        
            Vector2 start = MonsterContext.RigidBody2D.position;
            Vector2 end = MonsterContext.Target.position;

            CustomEvent.Trigger(MonsterContext.Owner.gameObject, "Switch", ECopyBaraAttackPattern.Rush);
            MonsterContext.RigidBody2D.velocity = Vector2.zero; // �ܿ� �ӵ� ����
            float e = 0f;
            while (e < dashTime)
            {
                ct.ThrowIfCancellationRequested();
                e += Time.fixedDeltaTime;
                MonsterContext.RigidBody2D.MovePosition(Vector2.Lerp(start, end, e / dashTime));
                await UniTask.WaitForFixedUpdate(ct); // ���� ���� Ÿ�ֿ̹� ���� �̵�
            }
        }

        public override void OnSetup(MonsterContext monsterContext)
        {
            MonsterContext = monsterContext;
        }
    }
}
