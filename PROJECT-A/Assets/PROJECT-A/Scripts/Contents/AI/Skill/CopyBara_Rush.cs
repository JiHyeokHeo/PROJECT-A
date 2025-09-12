using Cysharp.Threading.Tasks;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

namespace A
{
    

    public class CopyBara_Rush : MonsterPattern
    {
        // 3초간 캐스팅하며 붉은 반투명한 선으로 범위 표시.
        // 투명도 50의 붉은 범위 표시 후, 투명도 70의 범위가 캐스팅 시간에
        // 걸쳐 차오르고 공격.범위가 다 차면 그 방향으로 돌진하며 돌진 중 접촉하는 플레이어에게 피해.
        public override async UniTask Execute(CancellationToken ct)
        {
            float t = 0;
            CustomEvent.Trigger(context.Owner.gameObject, "Switch", ECopyBaraAttackPattern.RushReady);

            Vector2 start = context.RigidBody2D.position;
            Vector2 end = context.Target.position;
            Vector2 dir = end - start;

            // 거리에 맞게 스케일 조절
            SetWarningSign(true);    // 워닝 사인 on off

            // 워닝 사인 데이터 Scale Rotation 변경
            context.Owner.warningSign["Rush"].SetData(context);

            // TODO : 맞았을 때 Flinch 애니메이션 재생 및 Visual Scripting Add
            Transform warnTr = context.Owner.warningSign["Rush"].inner.transform;
            float localStartScaleX = warnTr.localScale.x;
            while (t < castingTime)
            {
                if (warnTr == null)
                    break;
                // 혹시라도 캔슬 요청이 들어오면 캔슬 시켜라
                ct.ThrowIfCancellationRequested();
                // 붉은 화면 뜨도록 설정

                float dur = t / castingTime;
                float lerpScale = Mathf.Lerp(localStartScaleX, 1f, dur);
                warnTr.localScale = new Vector3(lerpScale, 1f, 1f);

                t += Time.deltaTime;
                await UniTask.Yield(ct);
            }

            SetWarningSign(false); // 워닝 사인 on off
            // CustomEvent -> 연동
            CustomEvent.Trigger(context.Owner.gameObject, "Switch", ECopyBaraAttackPattern.Rush);
            //context.RigidBody2D.velocity = Vector2.zero; // 잔여 속도 제거
         
            while ((context.RigidBody2D.position - end).sqrMagnitude > 1.0f)
            {
                ct.ThrowIfCancellationRequested();

                context.Owner.Move(dir, context.Config.MoveSpeed);
                await UniTask.WaitForFixedUpdate(ct);
            }
            CustomEvent.Trigger(context.Owner.gameObject, "Switch", ECopyBaraAttackPattern.RushDone);

            // 후딜레이 설정
            await UniTask.Delay(2000);

            CustomEvent.Trigger(context.Owner.gameObject, "Switch", ECopyBaraAttackPattern.Run);
        }

        public override void Init(MonsterContext context, MonsterPatternSetSO data)
        {
            this.context = context;
            cooldown = data.CoolDown;
            weight = data.Weight;
            castingTime = 3.0f;
        }

        private void SetWarningSign(bool on)
        {
            context.Owner.warningSign["Rush"].ResetData(on);
        }
    }
}
