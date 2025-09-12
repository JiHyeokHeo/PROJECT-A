using System;
using Unity.VisualScripting;
using UnityEngine;

namespace A
{
    public class MonsterWarningSign : MonoBehaviour
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

        public void SetData(MonsterContext context)
        {
            Vector2 start = context.RigidBody2D.position;
            Vector2 end = context.Target.position;
            Vector2 dir = end - start;

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            outer.transform.localRotation = Quaternion.Euler(0, 0, angle);
            float distance = (start - end).magnitude;
            outer.transform.localScale = new Vector3(distance, 1, 1);
        }

    }
}
