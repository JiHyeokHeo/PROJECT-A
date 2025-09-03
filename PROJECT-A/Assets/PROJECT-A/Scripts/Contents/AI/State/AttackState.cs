using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
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

        public override void Enter()
        {
            if (monster == null)
                return;
            var token = monster.monsterContext.CancellationToken.Token;
            // ������ �ϴ� ���� ����
            //monster.monsterContext.CancellationToken.Cancel();
            //monster.monsterContext.CancellationToken.Dispose(); // ��Ƽ�����忡�� ����Ҷ� ���õ� �޸� �̽�  ���̵���ο����� ����

            Run(token).Forget();
        }

        public override void Exit()
        {
            // Ȥ�� �𸣴� �������� ĵ��
            monster.monsterContext.CancellationToken.Cancel();
        }   


        // ���� ��Ȳ������ Eventó���� �Ѵ�? 
         
        public override void Tick(float dt)
        {
            // ������ ���� ����� AttackState���� ���� // ���� ������ override�� ���� �پ缺 Ȯ��
            //monster.Attack();
            // Casting Rush
            // Casting Smash
            // Melt
            // Melt_Idle

        }

        async UniTaskVoid Run(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                await monster.patternScheduler.ExecuteNext(ct);      // �غ�� ���� 1�� ����(+��Ÿ�� ��� ����)
                // ��Ÿ�/���� ���� üũ�ؼ� �߰� ������ ������ ���� �߰� ����
            }
        }

    }
}
