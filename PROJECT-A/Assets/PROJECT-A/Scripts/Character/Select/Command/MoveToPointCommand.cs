using System.Collections;
using UnityEngine;

namespace Character
{
    public class MoveToPointCommand : IUnitCommand
    {
        public ICharacter Character { get; private set; }
        public bool IsBlocking => true;
        public bool IsFinished { get; private set; }

        private readonly Vector2 _dest;
        private readonly float _arriveDist;
        private bool _subscribed;

        private readonly IMovable _move;

        public MoveToPointCommand(ICharacter unit, Vector2 dest, float arriveDist = 0.12f)
        {
            Character = unit;
            _dest = dest;
            _arriveDist = arriveDist;
            _move = unit.GetCapability<IMovable>();
        }

        public void Execute()
        {
            IsFinished = false;

            if (_move != null && !_subscribed)
            {
                _move.onArrived += OnArrived;
            }

            _move?.MoveTo(_dest);
            IsFinished = true;
        }
        void OnArrived(Vector2 dest, MoveArriveReason reason)
        {
            // 목적지 일치(혹은 충분히 가까움)만 인정
            if ((dest - _dest).sqrMagnitude > _arriveDist * _arriveDist)
                return;

            CleanupSub();
            IsFinished = true;
        }
        
        public void Cancel() => _move.Stop();
        public bool TryMerge (IUnitCommand newer) => false;
        void CleanupSub()
        {
            if (_subscribed && _move != null)
            {
                _move.onArrived -= OnArrived;
                _subscribed = false;
            }
        }

    }
}