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
        public override async UniTask<bool> Execute(CancellationToken ct)
        {
            float t = 0;
            CustomEvent.Trigger(context.Owner.gameObject, "Switch", ECopyBaraAttackPattern.RushReady);

            Vector2 start = context.RigidBody2D.position;
            Vector2 end = context.Target.position;
            Vector2 dir = end - start;

            if ((start - end).magnitude > attackRange)
                return false;

            // �Ÿ��� �°� ������ ����
            SetWarningSign(true);    // ���� ���� on off

            // ���� ���� ������ Scale Rotation ����
            context.Owner.warningSign["Rush"].SetData(context, patternSO);

            // TODO : �¾��� �� Flinch �ִϸ��̼� ��� �� Visual Scripting Add
            Transform warnTr = context.Owner.warningSign["Rush"].inner.transform;
            float localStartScaleX = warnTr.localScale.x;
            while (t < castingTime)
            {
                if (warnTr == null)
                    break;
                // Ȥ�ö� ĵ�� ��û�� ������ ĵ�� ���Ѷ�
                ct.ThrowIfCancellationRequested();
                // ���� ȭ�� �ߵ��� ����

                float dur = t / castingTime;
                float lerpScale = Mathf.Lerp(localStartScaleX, 1f, dur);
                warnTr.localScale = new Vector3(lerpScale, 1f, 1f);

                t += Time.deltaTime;
                await UniTask.Yield(ct); // ������Ʈ ���
            }

            SetWarningSign(false); // ���� ���� on off
            // CustomEvent -> ����
            CustomEvent.Trigger(context.Owner.gameObject, "Switch", ECopyBaraAttackPattern.Rush);
         
            while ((context.RigidBody2D.position - end).sqrMagnitude > 1.0f)
            {
                ct.ThrowIfCancellationRequested();

                context.Owner.Move(dir, context.Config.MoveSpeed);
                await UniTask.WaitForFixedUpdate(ct);
            }
            CustomEvent.Trigger(context.Owner.gameObject, "Switch", ECopyBaraAttackPattern.RushDone);

            // �ĵ����� ����
            await UniTask.Delay(2000);
            CustomEvent.Trigger(context.Owner.gameObject, "Switch", ECopyBaraAttackPattern.Run);
            return true;
        }

        public override void Init(MonsterContext context, MonsterPatternSetSO data)
        {
            base.Init(context, data);
            castingTime = 3.0f;
        }

        private void SetWarningSign(bool on)
        {
            context.Owner.warningSign["Rush"].ResetData(on);
        }
    }
}
