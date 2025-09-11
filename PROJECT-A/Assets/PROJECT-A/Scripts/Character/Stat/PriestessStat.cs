using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character
{
    public class PriestessStat : MonoBehaviour, IStats
    {
        [SerializeField]
        public float Speed => 5f;

        [SerializeField]
        public float HP => 100f;

        [SerializeField]
        public float Atk => 10f;

        [SerializeField]
        public float AtkSpeed => 1.0f;

        [SerializeField]
        public float MaxHP => 100f;
    }
}
