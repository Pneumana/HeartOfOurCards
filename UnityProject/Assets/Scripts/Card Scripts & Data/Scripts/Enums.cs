namespace Enums
{
    public enum ActionTargetType
    {
        Enemy,
        Ally,
        AllEnemies,
        AllAllies,
        RandomEnemy,
        RandomAlly,
        TwoRandomEnemies
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
        EnemyAttack,
        EnemyBlock,
        EnemyHeal,
        EnemyVulnerability,
        EnemyBleed
    }

    public enum CharacterType
    {
        Ally,
        P1,
        P2,
        Enemy
    }

    public enum EnemyActionType
    {
        Attack,
        Heal,
        Debuff,
        Block
    }

    public enum EnemyIntentionType
    {
        Attack,
        Defend,
        Heal,
        Debuff,
        Special
    }

    public enum CardPiles
    {
        CurrentDeck,
        DrawPile,
        DiscardPile,
        ExhaustPile
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
        Regen
    }

    public enum CardType
    {
        Physical,
        Magical,
        Neutral
    }
}
