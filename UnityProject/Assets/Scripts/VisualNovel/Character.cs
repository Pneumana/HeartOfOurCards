using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "VisualNovel/Character", order = 2)]
public class Character : ScriptableObject
{
    [Header("Display name of the Character")]
    public string Name;
    [Header("Possessive Pronoun of the Character")]
    public string pss;
    [Header("Second Person Pronoun of the Character")]
    public string pnd;
    [Header("Character Display name Color")]
    public Color Color;
}
