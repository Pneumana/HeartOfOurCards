using DeckData;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Field Card Data", menuName = "Card Stuff/Field Card Base", order = 5)]
public class FieldCardData : ScriptableObject
{
    //in case a field card performs multiple actions with different proc requirements
    [SerializeField] public List<ProcActionSet> procActionSets = new List<ProcActionSet>();

    [SerializeField] public List<ProcType> procTypeSets = new List<ProcType>();

    [System.Serializable]
    public class ProcActionSet
    {
        public List<CardActionData> procActions;
    }


    //
    [Serializable]
    public enum ProcType
    {
        OnAlliedTurnStart,
        OnEnemyTurnStart,
        OnAllyCastSpell,
        OnEnemyCastSpell,
        OnAllyAttack,
        OnEnemyAttack,
        OnAllyKilled,
        OnEnemyKilled
    }

}
