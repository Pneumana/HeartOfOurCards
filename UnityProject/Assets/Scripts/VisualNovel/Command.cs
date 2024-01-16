using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Command
{
    public enum CommandType
    {
        Say,//the ID will say the attached string
        Position,//the ID gameObject will move to the attached position
        Rotate,//the ID gameObject will rotate to match the euler angles given
        Scale,//
        Display//changes the character sprite to be a new one.
    }
    public string _commandData;
    public int _characterID;
    public Command(int characterID, string commandValue)
    {
        _characterID = characterID;
        _commandData = commandValue;
    }
}
