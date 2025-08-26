using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace A
{
    public interface ISelectable 
    {
        public bool IsSelected { get; }
        public Transform SelectionPivot { get; }
        public Vector2 CurrentPos { get; }
    }
}
