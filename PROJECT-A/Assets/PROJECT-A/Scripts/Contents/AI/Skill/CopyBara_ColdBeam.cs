using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace A
{
    public class CopyBara_ColdBeam : MonsterPattern
    {
        GameObject projectilePrefab;

        public override async UniTask<bool> Execute(CancellationToken ct)
        {

            if (projectilePrefab == null || context.Target == null)
                return false;

            CustomEvent.Trigger(context.Owner.gameObject, "Switch", ECopyBaraAttackPattern.MeleeAttack);
            // 1. 발사 위치
            Vector2 spawnPos = context.Owner.transform.position;

            // 2. 타겟 방향 (정규화)
            Vector2 dir = ((Vector2)context.Target.position - spawnPos).normalized;

            // 3. 프리팹 생성
            GameObject go = GameObject.Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

            GameObject.Destroy(go, 3.0f);
            // 4. Rigidbody2D로 속도 부여
            Rigidbody2D rb = go.GetComponent<Rigidbody2D>();

            // 방향에서 각도 계산
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            // 발사체를 해당 각도로 회전
            go.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            if (rb != null)
            {
                float speed = 2f; // 패턴별 속도 (SO에 넣어도 됨)
                rb.velocity = dir * speed;
            }
            else
            {
                // Rigidbody 없으면 transform.Translate로 이동 관리해야 함
                go.transform.right = dir; // 총알 방향 맞추기
            }
            await UniTask.Delay(1900);
            CustomEvent.Trigger(context.Owner.gameObject, "Switch", ECopyBaraAttackPattern.RushDone);
            await UniTask.Delay(1900);
            CustomEvent.Trigger(context.Owner.gameObject, "Switch", ECopyBaraAttackPattern.Run);
            return true;
        }

        public override void Init(MonsterContext context, MonsterPatternSetSO data)
        {
            base.Init(context, data);
            projectilePrefab = data.ProjectilePrefab;
            castingTime = 1.9f;
        }
    }
}