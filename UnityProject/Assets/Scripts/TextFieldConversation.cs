using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
[CreateAssetMenu(fileName = "New Text Conversation", menuName = "VisualNovel/TextFieldConversation", order = 3)]
public class TextFieldConversation : ScriptableObject
{
    public List<Character> characters = new List<Character>();
    [Header("Conversation Step Documentation")]
    [TextArea(20, 20)]
    public string commandScript;

}
