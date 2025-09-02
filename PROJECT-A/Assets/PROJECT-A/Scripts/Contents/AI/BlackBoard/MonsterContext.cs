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
        //public EventHandler EventHandler; // 몬스터 이벤트 처리 피격 등
        public MonsterConfig MonsterConfig;
        public Transform Target; // 플레이어

        public CancellationTokenSource CancellationTokenSource = new CancellationTokenSource(); // 패턴 취소

        public float SqrAttackRange => MonsterConfig.AttackRange * MonsterConfig.AttackRange;
        public float SqrChaseStopRange => MonsterConfig.ChaseStopRange * MonsterConfig.ChaseStopRange;
    }
}
