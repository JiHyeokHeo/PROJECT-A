using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

namespace A
{
    public class CopyBara_Smash : MonsterPattern
    {
        public override async UniTask<bool> Execute(CancellationToken ct)
        {
            float t = 0;

            Vector2 start = context.RigidBody2D.position;
            Vector2 end = context.Target.position;
            Vector2 dir = end - start;

            if ((start - end).magnitude > attackRange)
                return false;

            // �Ÿ��� �°� ������ ����
            SetWarningSign(true);    // ���� ���� on off

            // ���� ���� ������ Scale Rotation ����
            context.Owner.warningSign["Smash"].SetData(context, patternSO);

            // TODO : �¾��� �� Flinch �ִϸ��̼� ��� �� Visual Scripting Add
            Transform warnTr = context.Owner.warningSign["Smash"].inner.transform;
            float localStartScaleX = warnTr.localScale.x;
            //CustomEvent.Trigger(context.Owner.gameObject, "Switch", ECopyBaraAttackPattern.Smash);
            bool isPlay = false;
            while (t < castingTime)
            {
                if (t >= 0.9f && isPlay == false)
                {
                    CustomEvent.Trigger(context.Owner.gameObject, "Switch", ECopyBaraAttackPattern.Smash);
                    isPlay = true;
                }

                if (warnTr == null)
                    break;
                // Ȥ�ö� ĵ�� ��û�� ������ ĵ�� ���Ѷ�
                ct.ThrowIfCancellationRequested();
                // ���� ȭ�� �ߵ��� ����

                float dur = t / castingTime;
                float lerpScale = Mathf.Lerp(localStartScaleX, 1f, dur);
                warnTr.localScale = new Vector3(lerpScale, lerpScale, lerpScale);

                t += Time.deltaTime;
                await UniTask.Yield(ct);
            }

            SetWarningSign(false); // ���� ���� on off
            // CustomEvent -> ����
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
            context.Owner.warningSign["Smash"].ResetData(on);
        }
    }
}
