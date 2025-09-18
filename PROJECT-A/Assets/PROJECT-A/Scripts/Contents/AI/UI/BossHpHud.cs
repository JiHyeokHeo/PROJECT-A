using System;
using TST;
using Unity.VisualScripting;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

namespace A
{
    public class BossHpHud : UIBase
    {
        public Slider healthSlider;
        public Slider groggySlider;

        private void OnEnable()
        {
            GameManager.Instance.OnMonsterHpChanged += UpdateHp;
            
        }

        private void OnDisable()
        {
            GameManager.Instance.OnMonsterHpChanged -= UpdateHp;
        }

        void UpdateHp(MonsterBase monster, float current, float max)
        {
            // TODO : 만약에 몬스터 타입에 따라 나눈다면 여기서 처리해야할듯
            if (monster.monsterConfig.isBoss)
                healthSlider.value = current / max;
        }
    }
}
