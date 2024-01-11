using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
[CreateAssetMenu(fileName = "New Conversation", menuName = "VisualNovel/Conversation", order = 1)]
public class Conversation : ScriptableObject
{
    public List<Character> characters = new List<Character>();
    [Header("Conversation Step Documentation")]
    [Space(8, order = 1)]
    [Header("@SAY followed by a string of text is used" +
        "\n for making a character say something." +
        "\n The character ID number uses the characters list above this.")]
    [Space(8, order = 2)]
    [Header("@DIS changes the character sprite to a new" +
        "\n one located within Resources/Textures/." +
        "\n This is the name of the image file")]
    [Space(8, order = 3)]
    [Header("@POS changes the position of a character" +
        "\n sprite to x,y coords")]
    [Space(8, order = 4)]
    [Header("@SCA changes the scale of a character" +
        "\n sprite to x,y dimensions")]
    [Space(8, order = 5)]
    [Header("@ROT changes the rotation of a character" +
        "\n sprite using euler angles")]
    public List<ConversationStep> actionSteps = new List<ConversationStep>();
    
    //an ACTIONSTEP is a list of commands that need to be processed

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void COMMAND()
    {

    }

    //Escape characters to load a name or something.
    //like uh CHAR: + an id loads the name of whatever character shares the index with the id.
    string SAY(string command, int speaker = 0)
    {
        Debug.Log(characters[speaker].Name);
        return characters[speaker].Name;
    }
}
