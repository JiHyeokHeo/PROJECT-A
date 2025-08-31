using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace A
{
    public interface ISelectable
    {
        bool IsSelected { get; }
        void SetSelected(bool on);
    }
}
