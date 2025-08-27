using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkillUser
{
    Transform transform { get; } // 유닛 위치
    IEnumerable<ISkill> Skills { get; } // 이 유닛이 가진 스킬 리스트
    bool IsAlive { get; } // 유닛 상태
}