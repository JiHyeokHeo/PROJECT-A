using System.Collections.Generic;
using UnityEngine;

namespace Character
{
    [DefaultExecutionOrder(-10)]
    public sealed class UnitCommandQueue : MonoBehaviour, ICapability, ITickable
    {
        private ICharacter _character;
        private readonly Queue<IUnitCommand> _queue = new();
        private IUnitCommand _current;

        private void Awake()
        {
            _character = GetComponentInParent<ICharacter>(true);
        }

        public void Clear()
        {
            _current?.Cancel();
            _current = null;
            while (_queue.Count > 0)
                _queue.Dequeue().Cancel();
        }
        
        public void Enqueue(IUnitCommand cmd, bool clearExisting = false)
        {
            if (cmd == null)
                return;
            if (clearExisting)
                Clear();

            if (_current != null && _current.TryMerge(cmd))
                return;
            if (_queue.Count > 0 && _queue.Peek().TryMerge(cmd))
                return;
            _queue.Enqueue(cmd);
        }

        public void Tick(float dt)
        {
            if (_current == null)
            {
                if (_queue.Count == 0)
                    return;
                _current = _queue.Dequeue();
                _current.Execute();
            }

            if (_current.IsFinished || !_current.IsBlocking)
            {
                _current = null;
            }
        }
    }
}