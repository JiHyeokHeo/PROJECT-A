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
            // 1. �߻� ��ġ
            Vector2 spawnPos = context.Owner.transform.position;

            // 2. Ÿ�� ���� (����ȭ)
            Vector2 dir = ((Vector2)context.Target.position - spawnPos).normalized;

            // 3. ������ ����
            GameObject go = GameObject.Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

            GameObject.Destroy(go, 3.0f);
            // 4. Rigidbody2D�� �ӵ� �ο�
            Rigidbody2D rb = go.GetComponent<Rigidbody2D>();

            // ���⿡�� ���� ���
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            // �߻�ü�� �ش� ������ ȸ��
            go.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            if (rb != null)
            {
                float speed = 2f; // ���Ϻ� �ӵ� (SO�� �־ ��)
                rb.velocity = dir * speed;
            }
            else
            {
                // Rigidbody ������ transform.Translate�� �̵� �����ؾ� ��
                go.transform.right = dir; // �Ѿ� ���� ���߱�
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