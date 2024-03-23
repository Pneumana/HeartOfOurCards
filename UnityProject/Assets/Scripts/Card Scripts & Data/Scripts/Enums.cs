namespace Enums
{
    public enum ActionTargetType
    {
        Enemy,
        Ally,
        AllEnemies,
        AllAllies,
        HealthPool,
        RandomEnemy,
        RandomAlly,
        TwoRandomEnemies,
        Self
    }

    public enum SpecialKeywords
    {
        Block,
        Strength,
        Dexterity,
        Frozen,
        Burn,
        Stun,
        Vulnerable,
        Bleed,
        Regen
    }

    public enum CardActionType
    {
        Attack,
        Heal,
        Regen,
        AllyBlock,
        Draw,
        EarnEnergy,
        LifeSteal,
        Stun,
        IncreaseStrength,
        Exhaust,
        Burn,
        Frost,
        Bleed,
        SpawnCards,
        TempOverkill,
        Vulnerable,
        FireAttack,
        FrostAttack,
        AttackTwice,
        AttackScaleNothing,
        AttackFive,
        EnemyAttack,
        EnemyBlock,
        EnemyHeal,
        EnemyVulnerability,
        EnemyBleed,
        EnemyRegen,
        EnemyFreeze,
        EnemyBurn,
        ApplyStatus,
        CreateFieldCard,
        ChangeSharedStat,
        ChangeIndividualStat
    }

    public enum CharacterType
    {
        Ally,
        P1,
        P2,
        Enemy
    }

    public enum CombatStateType
    {
        PrepareCombat,
        AllyTurn,
        EnemyTurn,
        EndCombat
    }

    public enum StatusType
    {
        None = 0,
        Block,
        Strength,
        Dexterity,
        Frozen,
        Burn,
        Stun,
        Vulnerable,
        Bleed,
        Regen,
        TempStr,
        TempDex
    }

    public enum CardType
    {
        Physical,
        Magical,
        Neutral,
        Offensive,
        Defensive
    }
}
