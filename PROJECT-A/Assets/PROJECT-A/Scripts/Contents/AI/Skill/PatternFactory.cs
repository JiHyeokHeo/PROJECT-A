using A;
using System;

public class PatternFactory
{
    public static MonsterPattern Create(MonsterPatternSetSO definition, MonsterContext context)
    {
        MonsterPattern pattern;

        switch (definition.PatternID)
        {
            case PatternID.Rush: 
                pattern = new CopyBara_Rush(); 
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(definition.PatternID), definition.PatternID, null);
        }

        pattern.Init(context, definition);
        return pattern;
    }
}