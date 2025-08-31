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
    }
}
