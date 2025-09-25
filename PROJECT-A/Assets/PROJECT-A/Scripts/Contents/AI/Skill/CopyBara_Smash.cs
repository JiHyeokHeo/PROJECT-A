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

            // 거리에 맞게 스케일 조절
            SetWarningSign(true);    // 워닝 사인 on off

            // 워닝 사인 데이터 Scale Rotation 변경
            context.Owner.warningSign["Smash"].SetData(context, patternSO);

            // TODO : 맞았을 때 Flinch 애니메이션 재생 및 Visual Scripting Add
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
                // 혹시라도 캔슬 요청이 들어오면 캔슬 시켜라
                ct.ThrowIfCancellationRequested();
                // 붉은 화면 뜨도록 설정

                float dur = t / castingTime;
                float lerpScale = Mathf.Lerp(localStartScaleX, 1f, dur);
                warnTr.localScale = new Vector3(lerpScale, lerpScale, lerpScale);

                t += Time.deltaTime;
                await UniTask.Yield(ct);
            }

            SetWarningSign(false); // 워닝 사인 on off
            // CustomEvent -> 연동
            CustomEvent.Trigger(context.Owner.gameObject, "Switch", ECopyBaraAttackPattern.RushDone);
          
            // 후딜레이 설정
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
