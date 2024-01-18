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
        Block,
        Draw,
        EarnMana,
        LifeSteal,
        Stun,
        IncreaseStrength,
        Exhaust,
        Burn
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
        Vulnerable
    }

}
