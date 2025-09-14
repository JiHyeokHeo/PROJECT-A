using System.Threading;
using UnityEngine;

namespace A
{
    // ������ ����, ���� ���迡�� AI�� ���� �� ������ �����ϴ� ������ ����
    public class MonsterContext 
    {
        public MonsterBase Owner;
        public Rigidbody2D RigidBody2D;
        public SpineAnimationDriver AnimationDriver;
        //public EventHandler EventHandler; // �ǽð� ü�� ó���� ���⼭ �ұ�..........???????????????????
        public MonsterConfigSO Config;
        public Transform Target; // �÷��̾�
        public float idleTime;
        public CancellationTokenSource CancellationToken = new CancellationTokenSource(); // ���� ���

        public void ResetToken()
        {
            CancellationToken.Cancel();
            CancellationToken.Dispose();
            CancellationToken = new CancellationTokenSource();
        }

        public float SqrAttackRange => Config.AttackRange * Config.AttackRange;
        public float SqrChaseStopRange => Config.ChaseStopRange * Config.ChaseStopRange;
    }
}
