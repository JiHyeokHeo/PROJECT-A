using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace A
{
    // ������ ����, ���� ���迡�� AI�� ���� �� ������ �����ϴ� ������ ����
    public class MonsterContext 
    {
        public Transform Owner;
        public Rigidbody2D RigidBody2D;
        public SpineAnimationDriver AnimationDriver;
        //public EventHandler EventHandler; // �ǽð� ü�� ó���� ���⼭ �ұ�..........???????????????????
        public MonsterConfigSO MonsterConfig;
        public Transform Target; // �÷��̾�

        public CancellationTokenSource CancellationToken = new CancellationTokenSource(); // ���� ���

        public float SqrAttackRange => MonsterConfig.AttackRange * MonsterConfig.AttackRange;
        public float SqrChaseStopRange => MonsterConfig.ChaseStopRange * MonsterConfig.ChaseStopRange;
    }
}
