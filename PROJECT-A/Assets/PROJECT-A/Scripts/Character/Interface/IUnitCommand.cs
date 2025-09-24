using System.Collections;
using UnityEngine;

namespace Character
{
    public interface IUnitCommand
    {
        ICharacter Character { get; }
        bool IsBlocking { get; }
        bool IsFinished { get; }

        void Execute();
        void Cancel();
        bool TryMerge(IUnitCommand newer);
        
    }
}