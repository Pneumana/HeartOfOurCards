using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "VisualNovel/Character", order = 2)]
public class Character : ScriptableObject
{
    [Header("Display name of the Character")]
    public string Name;
    [Header("Character Display name Color")]
    public Color Color;
}
