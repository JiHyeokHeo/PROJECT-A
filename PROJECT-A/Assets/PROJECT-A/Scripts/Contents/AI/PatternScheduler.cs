using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace A
{
    public class PatternScheduler
    {
        MonsterContext monsterContext;
        float total;

        public void SetUp(MonsterContext monsterContext)
        {
            this.monsterContext = monsterContext;
        }

        //// 체력에 비례해 가중도 체크
        //public void InitWeightByHp()
        //{
        //    if (monsterContext.MonsterConfig.Phases == null || monsterContext.MonsterConfig.Phases.Length == 0)
        //        return;

        //    float ratio = monsterContext.
        //}
    }
}
