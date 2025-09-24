namespace Character
{
    public class AttackTargetCommand : IUnitCommand
    {
        public ICharacter Character { get; }
        public bool IsBlocking => true;
        public bool IsFinished =>
            _target == null
            || _target.Health == null
            || _target.Health.IsDead;

        private ICharacter _target;
        private readonly CharacterCombat _combat;

        public AttackTargetCommand(ICharacter character, ICharacter target)
        {
            Character = character;
            _target = target;
            _combat = character.GetCapability<CharacterCombat>();
        }

        public void Execute()
        {
            _combat?.CancelAll();
            if (_target != null)
                _combat?.IssueAttackTarget(_target);
        }

        public void Cancel() => _combat?.CancelAll();
        public bool TryMerge(IUnitCommand newer)
        {
            if (newer is AttackTargetCommand atc && atc.Character == Character)
            {
                _target = atc._target;   // 최신 타겟으로 교체
                return true;             // 새 명령은 버리고 기존 명령만 업데이트
            }
            return false;
        }
    }
}