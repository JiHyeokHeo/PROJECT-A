using System;
using Unity.VisualScripting;
using UnityEngine;

namespace A
{
    public abstract class MonsterWarningSign : MonoBehaviour
    {
        public Transform inner;
        public Transform outer;
        public Vector2 offSet;

        // Start is called before the first frame update
        void Start()
        {
            transform.localPosition = offSet;
            ResetData();
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void ResetData(bool isActive = false)
        {
            if (outer == null || inner == null)
                return;

            outer.localScale = Vector3.one;
            inner.localScale = new Vector3(0.3f, 1f, 1f);
            gameObject.SetActive(isActive);
        }

        public abstract void SetData(MonsterContext context);
    }
}
