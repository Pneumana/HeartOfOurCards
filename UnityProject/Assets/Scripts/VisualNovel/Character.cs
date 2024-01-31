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
    [Header("Character Poses")]
    public Sprite Default;
    //upon changing to this pose, the character controller will set it's position to this value
    public Vector2 DefaultPoseOffset;
    [Header("Character Eyes")]
    public Sprite Iris;
    public Sprite OpenLines;
    public Sprite OpenMask;
    public Sprite SquintLines;
    public Sprite SquintMask;
    public Sprite ClosedLines;
    //no need for a mask for closed eyes, just disable the mask sprite and the iris
}
