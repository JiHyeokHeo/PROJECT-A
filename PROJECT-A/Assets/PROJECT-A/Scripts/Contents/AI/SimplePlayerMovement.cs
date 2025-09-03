using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace A
{
    public class SimplePlayerMovement : MonoBehaviour
    {
        Transform transform;
        // Start is called before the first frame update
        void Start()
        {
            transform = GetComponent<Transform>();
        }

        // Update is called once per frame
        void Update()
        {
            Vector2 pos;
            pos.x =  30 * Mathf.Cos(180 * Time.time * Mathf.Deg2Rad);
            pos.y = transform.position.y;
            transform.position = pos;
        }
    }
}
