using Cysharp.Threading.Tasks;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

namespace A
{
    enum WarningSign
    {
        Outer,
        Inner,
    }

    public class CopyBara_Rush : MonsterPattern
    {
        // 3�ʰ� ĳ�����ϸ� ���� �������� ������ ���� ǥ��.
        // ���� 50�� ���� ���� ǥ�� ��, ���� 70�� ������ ĳ���� �ð���
        // ���� �������� ����.������ �� ���� �� �������� �����ϸ� ���� �� �����ϴ� �÷��̾�� ����.

        // WarningSign;
        SpriteRenderer[] spriteRenderer;

        public override async UniTask Execute(CancellationToken ct)
        {
            float t = 0;
            CustomEvent.Trigger(context.Owner.gameObject, "Switch", ECopyBaraAttackPattern.RushReady);

            Vector2 start = context.RigidBody2D.position;
            Vector2 end = context.Target.position;
            Vector2 dir = end - start;

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            // �Ÿ��� �°� ������ ����
            context.Owner.warningSign[(int)WarningSign.Outer].transform.localRotation =  Quaternion.Euler(0, 0, angle);
            float distance = (start - end).magnitude;
            context.Owner.warningSign[(int)WarningSign.Outer].transform.localScale = new Vector3(distance, 3, 1);

            if (spriteRenderer[(int)WarningSign.Outer])
            {
                SetWarningSign(true);    // ���� ���� on off
            }

            // TODO : �¾��� �� Flinch �ִϸ��̼� ��� �� Visual Scripting Add

            Transform warnTr = context.Owner.warningSign[(int)WarningSign.Inner].transform;
            float localStartScaleX = warnTr.localScale.x;
            while (t < castingTime)
            {
                // Ȥ�ö� ĵ�� ��û�� ������ ĵ�� ���Ѷ�
                ct.ThrowIfCancellationRequested();
                // ���� ȭ�� �ߵ��� ����

                float dur = t / castingTime;
                float lerpScale = Mathf.Lerp(localStartScaleX, 1f, dur);
                warnTr.localScale = new Vector3(lerpScale, 1f, 1f);

                t += Time.deltaTime;
                //Debug.Log("����ȭ�� Logging Cancel ����!");
                await UniTask.Yield(ct);
            }

            SetWarningSign(false); // ���� ���� on off
            // CustomEvent -> ����
            CustomEvent.Trigger(context.Owner.gameObject, "Switch", ECopyBaraAttackPattern.Rush);
            context.RigidBody2D.velocity = Vector2.zero; // �ܿ� �ӵ� ����
         
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
        }

        public override void Init(MonsterContext context, MonsterPatternSetSO data)
        {
            spriteRenderer = context.Owner.warningSign[(int)WarningSign.Outer].GetComponentsInChildren<SpriteRenderer>();
            this.context = context;
            cooldown = data.CoolDown;
            weight = data.Weight;
            castingTime = 3.0f;
        }

        private void SetWarningSign(bool on)
        {
            context.Owner.warningSign[(int)WarningSign.Inner].transform.localScale = new Vector3(0.3f, 1f, 1f); // �ϵ��ڵ� �� �ٲ߽ô�
            context.Owner.warningSign[(int)WarningSign.Outer].SetActive(on);
        }
    }
}
