using UnityEngine;

namespace Character
{
    public class AttackMoveCommand : IUnitCommand
    {
        public ICharacter Character { get; }
        public bool IsBlocking => false;
        public bool IsFinished { get; private set; }

        private Vector2 _dest;
        private readonly CharacterCombat _combat;

        public AttackMoveCommand(ICharacter character, Vector2 dest)
        {
            Character = character;
            _dest = dest;
            _combat = character?.GetCapability<CharacterCombat>();
        }

        public void Execute()
        {
            _combat?.CancelAll();
            _combat?.IssueAttackMove(_dest);
            IsFinished = true;
        }

        public void Cancel() => _combat?.CancelAll();
        public bool TryMerge(IUnitCommand newer)
        {
            if (newer is AttackMoveCommand am && am.Character == Character)
            {
                _dest = am._dest;
                return true;
            }
            return false;
        }
    }
}