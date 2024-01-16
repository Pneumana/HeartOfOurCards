using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ConversationStep
{
    public List<Command> _commands;
    public ConversationStep(List<Command> commands)
    {
        _commands = commands;
    }
}
