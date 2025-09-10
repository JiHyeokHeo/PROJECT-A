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

        // WarningSign;
        SpriteRenderer spriteRenderer;

        public override async UniTask Execute(CancellationToken ct)
        {
            float t = 0;
            CustomEvent.Trigger(context.Owner.gameObject, "Switch", ECopyBaraAttackPattern.RushReady);

            Vector2 start = context.RigidBody2D.position;
            Vector2 end = context.Target.position;
            Vector2 dir = end - start;

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            // �Ÿ��� �°� ������ ����
            context.Owner.warningSign.transform.localRotation =  Quaternion.Euler(0, 0, angle);
            float distance = (start - end).magnitude;
            context.Owner.warningSign.transform.localScale = new Vector3(distance, 3, 1);

            Color c = spriteRenderer.color;
            if (spriteRenderer)
            {
                SetWarningSign(true);    // ���� ���� on off
                c.a = 0.3f;
                spriteRenderer.color = c;
            }

            while (t < castingTime)
            {
                // Ȥ�ö� ĵ�� ��û�� ������ ĵ�� ���Ѷ�
                ct.ThrowIfCancellationRequested();
                // ���� ȭ�� �ߵ��� ����

                if (spriteRenderer)
                {
                    float dur = t / castingTime;
                    c.a = Mathf.Lerp(0.2f, 0.6f, dur);
                    spriteRenderer.color = c;
                }

                t += Time.deltaTime;
                Debug.Log("����ȭ�� Logging Cancel ����!");
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
            spriteRenderer = context.Owner.warningSign.GetComponentInChildren<SpriteRenderer>();
            this.context = context;
            cooldown = data.CoolDown;
            weight = data.Weight;
            castingTime = 3.0f;
        }

        private void SetWarningSign(bool on)
        {
            context.Owner.warningSign.SetActive(on);
        }
    }
}
