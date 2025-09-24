using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character
{
    public class SelectionModel
    {
        readonly List<ISelectable> _list = new(32);

        public IReadOnlyList<ISelectable> Items => _list;

        public void Clear()
        {
            for (int i = 0; i < _list.Count; i++)
                _list[i]?.SetSelected(false);
            _list.Clear();
        }

        public void Add(ISelectable sel)
        {
            if (sel == null)
                return;
            if (_list.Contains(sel))
                return;
            _list.Add(sel);
            sel.SetSelected(true);
        }

        public void Replace(ISelectable sel)
        {
            Clear();
            Add(sel);
        }

        public void AddRangeNoAlloc(List<ISelectable> src)
        {
            for (int i = 0; i < src.Count; i++)
                Add(src[i]);
        }

        public void ToCharacters(List<ICharacter> outList)
        {
            outList.Clear();
            for (int i = 0; i < _list.Count; i++)
            {
                if (_list[i] is Component c && c.TryGetComponent<ICharacter>(out var ch))
                    outList.Add(ch);
            }
        }
  
    }
}