namespace Character
{
    public enum EffectKind
    {
        Damage,
        Heal,
        Buff,
        Debuff,
        Shield,
        CC,
        Threat
    }

    [System.Serializable]
    public class EffectDefinition
    {
        public EffectKind Kind;
        public float Value; // 데미지/치유/보호막양
        public float Duration; // 버프/디버프/CC 지속
        public float Period; // 도트/호트 간격
        public TagMask Tags; // 물리/마법/속성 등
        public BuffDefinition Buff; // 버프일 경우 상세(스택/가중치/태그)
        public CcDefinition Cc; // CC일 경우 상세(스턴/슬로우 등)
    }
}