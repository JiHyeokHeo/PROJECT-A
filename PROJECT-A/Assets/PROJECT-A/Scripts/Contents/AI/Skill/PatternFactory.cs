using A;
using System;
using Unity.VisualScripting;

public class PatternFactory
{
    public static MonsterPattern Create(MonsterPatternSetSO definition)
    {
        MonsterPattern pattern = null;

        pattern = CreatePatternWithCoolDown(definition.PatternID);

        return pattern;
    }

    static MonsterPattern CreatePatternWithCoolDown(string patternId, string namespaceFullpath = "A")
    {
        if (string.IsNullOrWhiteSpace(patternId))
            return null;

        Type type = Type.GetType($"{namespaceFullpath}.{patternId}");

        if (type != null)
        {
            object instance = Activator.CreateInstance(type);

            if (instance != null)
                return instance as MonsterPattern;
        }
  
        return null;
    }
}