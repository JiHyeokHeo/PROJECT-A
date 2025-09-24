using System.Collections.Generic;

namespace Character
{
    public interface ISkillSet
    {
        IEnumerable<ISkill> Skills { get; }
    }
}