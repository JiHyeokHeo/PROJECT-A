using A;
using System;

public class PatternFactory
{
    // TODO : È¤½Ã ÄÁÅÙÃ÷°¡ ´Ã¾î³­´Ù¸é Dictionary Mapping °í·Á
    public static MonsterPattern Create(MonsterPatternSetSO definition)
    {
        MonsterPattern pattern = null;

        if (definition.hasCoolDown)
        {
            switch (definition.monsterID)
            {
                case (int)EMonsterID.CopyBara:
                    CreatePatternWithCoolDown(definition.PatternID, out pattern);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(definition.monsterID), definition.monsterID, null);
            }
        }
        else
        {
            switch (definition.monsterID)
            {
                case (int)EMonsterID.CopyBara:
                    CreatePatternWithNoCoolDown(definition.PatternID, out pattern);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(definition.monsterID), definition.monsterID, null);
            }
        }



            return pattern;
    }

    static MonsterPattern CreatePatternWithCoolDown(EPatternID patternId, out MonsterPattern pattern)
    {
        switch (patternId)
        {
            case EPatternID.Rush:
                pattern = new CopyBara_Rush();
            break;
            case EPatternID.Smash:
                pattern = new CopyBara_Smash();
                break;
            case EPatternID.ColdBeam:
                pattern = new CopyBara_ColdBeam();
                break;
            default:
            throw new ArgumentOutOfRangeException(nameof(patternId), patternId, null);
        }

        return pattern;
    }

    static MonsterPattern CreatePatternWithNoCoolDown(EPatternID patternId, out MonsterPattern pattern)
    {
        switch (patternId)
        {
            case EPatternID.Melt:
                pattern = new CopyBara_Melt();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(patternId), patternId, null);
        }

        return pattern;
    }
}