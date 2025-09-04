using Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace A
{
    public class SimpleStats : MonoBehaviour, IStats
    {
        public float Speed => 5f;
        public float HP => 100f;
        public float Atk => 10f;
        public float AtkSpeed => 1.0f;
        public float MaxHP => 100f;
    }
}
