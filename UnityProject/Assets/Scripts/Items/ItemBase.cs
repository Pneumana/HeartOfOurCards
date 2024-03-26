using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Item", menuName = "Items/Create New Passive Item", order = 0)]
public class ItemBase : ScriptableObject
{
    [TextArea(20, 20)]
    public string description;
    [Header("Do events that happen to the other player \ncount toward this item activating?")]
    public bool procedByBothPlayers;
    [Header("Number of procs that need to happen for this item to activate")]
    public int procCountForActions;

    [SerializeField] public List<FieldCardData.ProcActionSet> procActionSets = new List<FieldCardData.ProcActionSet>();

    [SerializeField] public List<FieldCardData.ProcType> procTypeSets = new List<FieldCardData.ProcType>();

    public Sprite sprite;
}
