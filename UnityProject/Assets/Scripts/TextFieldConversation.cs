using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
[CreateAssetMenu(fileName = "New Text Conversation", menuName = "VisualNovel/TextFieldConversation", order = 3)]
public class TextFieldConversation : ScriptableObject
{
    public List<Character> characters = new List<Character>();
    [Header("Conversation Step Documentation")]
    [Header("An exclaimation point followed by a number is a character ID." +
        "\nUsing !1 will be the first character in the characters list" +
        "\nwhile using a !0 will use the player character." +
        "\n\n<b>General Don'ts</b> " +
        "\nDouble Spaces anywhere will brick the command compiler.\nSo just dont put any and all will be fine" +
        "\nPlease put all actions a character does under one number.\nThe compiler will not recognize if you use the same number multiple times")]
    [TextArea(20, 20)]
    public string commandScript;

}
