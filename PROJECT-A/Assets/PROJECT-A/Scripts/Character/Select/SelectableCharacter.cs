using Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace A.Tory
{
    public class SelectableCharacter : MonoBehaviour, ISelectable
    {
        
        public GameObject selectionIndicator;
        public bool IsSelected { get; private set; }
        public void SetSelected(bool on)
        {
            IsSelected = on;
            if (selectionIndicator)
                selectionIndicator.SetActive(on);
        }
    }
}
