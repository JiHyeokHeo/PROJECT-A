using A;
using System;

public class PatternFactory
{
    // TODO : È¤½Ã ÄÁÅÙÃ÷°¡ ´Ã¾î³­´Ù¸é Dictionary Mapping °í·Á
    public static MonsterPattern Create(MonsterPatternSetSO definition)
    {
        MonsterPattern pattern = null;

        switch (definition.monsterID)
        {
            case (int)EMonsterID.CopyBara:
                CreatePattern(definition.PatternID, out pattern);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(definition.monsterID), definition.monsterID, null);
        }

        return pattern;
    }

    static MonsterPattern CreatePattern(EPatternID patternId, out MonsterPattern pattern)
    {
        switch (patternId)
        {
            case EPatternID.Rush:
            pattern = new CopyBara_Rush();
            break;
        default:
            throw new ArgumentOutOfRangeException(nameof(patternId), patternId, null);
        }

        return pattern;
    }
}