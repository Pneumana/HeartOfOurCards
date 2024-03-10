using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Event Table", menuName = "VisualNovel/New Event Table", order = 4)]
public class ConversationTable : ScriptableObject
{
    public bool sequential;
    public List<TextFieldConversation> events = new List<TextFieldConversation>();
}
