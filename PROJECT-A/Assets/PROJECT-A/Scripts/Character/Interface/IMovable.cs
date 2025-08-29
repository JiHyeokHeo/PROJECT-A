using UnityEngine;

namespace Character
{
    public interface IMovable
    {
        bool CanMove { get; }
        void MoveTo(Vector2 worldPos);
    }
}
