using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace A
{
    // 블랙보드 개념, 게임 세계에서 AI의 상태 및 정보를 저장하는 데이터 구조
    public class MonsterContext 
    {
        public Transform Owner;
        public Rigidbody2D RigidBody2D;
        public SpineAnimationDriver AnimationDriver;
        //public EventHandler EventHandler; // 실시간 체력 처리를 여기서 할까..........???????????????????
        public MonsterConfigSO MonsterConfig;
        public Transform Target; // 플레이어

        public CancellationTokenSource CancellationToken = new CancellationTokenSource(); // 패턴 취소

        public float SqrAttackRange => MonsterConfig.AttackRange * MonsterConfig.AttackRange;
        public float SqrChaseStopRange => MonsterConfig.ChaseStopRange * MonsterConfig.ChaseStopRange;
    }
}
