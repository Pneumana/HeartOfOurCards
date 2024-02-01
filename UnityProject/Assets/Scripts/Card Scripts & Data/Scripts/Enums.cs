namespace Enums
{
    public enum ActionTargetType
    {
        Enemy,
        Ally,
        AllEnemies,
        AllAllies,
        RandomEnemy,
        RandomAlly
    }

    public enum SpecialKeywords
    {
        Block,
        Strength,
        Dexterity,
        Frozen,
        Burn,
        Stun,
        Vulnerable
    }

    public enum CardActionType
    {
        Attack,
        Heal,
        Regen,
        AllyBlock,
        EnemyBlock,
        Draw,
        EarnMana,
        LifeSteal,
        Stun,
        IncreaseStrength,
        Exhaust,
        Burn,
        Frost,
        Bleed,
        SpawnCards,
        TempOverkill,
        Vulnerable
    }

    public enum CharacterType
    {
        Ally,
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
        Bleed
    }

    public enum CardType
    {
        Physical,
        Magical,
        Neutral
    }
}
