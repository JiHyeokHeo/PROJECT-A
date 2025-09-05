using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace A
{
    public class AttackState : AIState
    {
        public AttackState(MonsterBase monster) : base(monster)
        {
            //this.monster = monster;
        }

        public override EAIStateId aiStateId => EAIStateId.Attack;
        Collider2D[] overlappedObjects = new Collider2D[4]; 

        public override void Enter()
        {
            if (monster == null)
                return;
            monster.monsterContext.ResetToken();
            var token = monster.monsterContext.CancellationToken.Token;
            // ������ �ϴ� ���� ����
            //monster.monsterContext.CancellationToken.Cancel();
            //monster.monsterContext.CancellationToken.Dispose(); // ��Ƽ�����忡�� ����Ҷ� ���õ� �޸� �̽�  ���̵���ο����� ����

            Run(token).Forget();
        }

        public override void Exit()
        {
            // Ȥ�� �𸣴� �������� ĵ��
            monster.monsterContext.CancellationToken?.Cancel();
            monster.monsterContext.CancellationToken?.Dispose();
        }   


        // ���� ��Ȳ������ Eventó���� �Ѵ�? 
         
        public override void Tick(float dt)
        {
            Physics2D.OverlapCircleNonAlloc(monster.monsterContext.RigidBody2D.position, monster.monsterConfig.AttackRange, overlappedObjects);

            if (overlappedObjects.Length > 0) 
            {
                monster.monsterContext.Target = overlappedObjects[0].transform;
            }
        }

        // ���� ���� ����
        async UniTaskVoid Run(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                MonsterContext context = monster.monsterContext;
                if (context.Target == null)
                {
                    await UniTask.Yield(PlayerLoopTiming.Update, ct); // �� ������ ���
                    continue;
                }
                Vector2 monsterPos = context.RigidBody2D.position;
                Vector2 targetPos = context.Target.position;

                float sqrTargetRange= (targetPos - monsterPos).sqrMagnitude;
                if (context.SqrAttackRange >= sqrTargetRange)
                {
                    await monster.patternScheduler.ExecuteNext(ct);      // �غ�� ���� 1�� ����(+��Ÿ�� ��� ����)
                }

                if (context.SqrChaseStopRange <= sqrTargetRange &&
                    monster.monsterContext.Target != null)
                {
                    monster.Move(targetPos - monsterPos, monster.monsterConfig.MoveSpeed);
                }
                
                // ��Ÿ�/���� ���� üũ�ؼ� �߰� ������ ������ ���� �߰� ����
                await UniTask.Yield(PlayerLoopTiming.FixedUpdate, ct);
            }
        }
    }
}
