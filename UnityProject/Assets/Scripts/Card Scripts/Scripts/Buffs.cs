using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Buffs
{
    public type Type;
    public enum type {block, vulnerable, poison }
    public Sprite buffIcon;
    [Range(0, 999)]
    public int buffValue;

}
