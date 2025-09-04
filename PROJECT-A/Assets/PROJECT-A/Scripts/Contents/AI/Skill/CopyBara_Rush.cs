using Cysharp.Threading.Tasks;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

namespace A
{
    public class CopyBara_Rush : MonsterPattern
    {
        public override float Cooldown => 10f;

        public float castingTime = 3.0f;

        // 3�ʰ� ĳ�����ϸ� ���� �������� ������ ���� ǥ��.
        // ���� 50�� ���� ���� ǥ�� ��, ���� 70�� ������ ĳ���� �ð���
        // ���� �������� ����.������ �� ���� �� �������� �����ϸ� ���� �� �����ϴ� �÷��̾�� ����.

        public override async UniTask Execute(CancellationToken ct)
        {
            float t = 0;
            CustomEvent.Trigger(MonsterContext.Owner.gameObject, "Switch", ECopyBaraAttackPattern.RushReady);
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
            Vector2 start = MonsterContext.RigidBody2D.position;
            Vector2 end = MonsterContext.Target.position;
            Vector2 dir = end - start;

            CustomEvent.Trigger(MonsterContext.Owner.gameObject, "Switch", ECopyBaraAttackPattern.Rush);
            MonsterContext.RigidBody2D.velocity = Vector2.zero; // �ܿ� �ӵ� ����
         
            while ((MonsterContext.RigidBody2D.position - end).sqrMagnitude > 1.0f)
            {
                ct.ThrowIfCancellationRequested();

                MonsterContext.Owner.Move(dir, 10.0f);
                await UniTask.WaitForFixedUpdate(ct);
            }
            CustomEvent.Trigger(MonsterContext.Owner.gameObject, "Switch", ECopyBaraAttackPattern.RushDone);
        }

        public override void OnSetup(MonsterContext monsterContext)
        {
            MonsterContext = monsterContext;
        }
    }
}
