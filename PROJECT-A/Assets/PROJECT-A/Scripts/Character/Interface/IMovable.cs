using UnityEngine;
using System;
namespace Character
{
    public enum MoveArriveReason
    {
        Reached,
        Cancelled,
        Interrupted
    }
    public interface IMovable : ICapability
    {
        bool CanMove { get; }
        void MoveTo(Vector2 worldPos);
        void Stop();
        event Action<Vector2, MoveArriveReason> onArrived;
    }
}
