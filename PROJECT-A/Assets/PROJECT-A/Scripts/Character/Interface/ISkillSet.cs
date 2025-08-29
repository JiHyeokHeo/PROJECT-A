using A;
using System.Collections.Generic;

public interface ISkillSet
{
    IEnumerable<ISkill> Skills { get; }
    ISkill Get(SkillSlot slot);
    void Bind(SkillSlot slot, ISkill skill);
    bool TryCast(SkillSlot slot, ICharacter caster);
}