using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character
{
    public interface ISelectable
    {
        bool IsSelected { get; }
        void SetSelected(bool on);
    }
}
