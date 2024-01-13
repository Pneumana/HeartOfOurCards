using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class DialougeDisplayer : MonoBehaviour
{
    public Character player;

    public TextMeshProUGUI speaker;
    public TextMeshProUGUI textBody;

    public int convoActionIndex;//gets the current ConversationStep in the current conversation

    public Conversation currentConvo;

    public TextFieldConversation textFieldConvo;

    public GameObject optionPrefab;

    public List<GameObject> sprites = new List<GameObject>();
    //target screen position normalized, speed
    public List<Vector2> positions = new List<Vector2>();

    public List<float> speeds = new List<float>();

    public bool displayingChoice;

    //maybe use for mouse wheel down do it doesnt go past stuff you haven't seen before.
    int farthestIntoConvo;
    //also use this to stop! animating sprites and positions
    //scrolling up acts as a history system
    //picked choices should be displayed with a different color
    //you cant change your choice after scrolling up
    List<Dictionary<int, List<string>>> convoActions = new List<Dictionary<int, List<string>>>();

    public struct CharData
    {
        //keeps track of the game objects, rect transform stuff, color, yadda yadd for each Character in the Conversation
        GameObject go;
        Vector2 targetPos;
        float speed;
        int sortOrder;//use CharacterScene.childcount - 1 as the clamp. 

        //sort int
    }
    //for multiplayer, this can be synced to whoever is in the conversation;
    /*
        {ActionNumber} //line starts with no whitespace == ActionStep
            [charID] //folliwing lines use this character id as their effector;
                [command]// line starts with two whitespace == COMMAND

    the command text should be read, then the sort of list system we have right now should be populated with the data from
    the command text, discovered using the regex patterns below

        to check for charID, use 
        ^\W\d
        to check command lines use
        @..*
        to check for action number look for
        ^\w.*
    */
    // Start is called before the first frame update
    void Start()
    {
        LoadConvo();
    }
    void SplitTextFieldConvo()
    {


        //check for 
        //MatchCollection actionLabels = Regex.Matches(textFieldConvo.commandScript, @"(?<=\w\s)(.|\w\s)*(?=;)");//find all actions
        MatchCollection actions = Regex.Matches(textFieldConvo.commandScript, @"(?<=.*\n)(:|.|\S\s)*(?<=;)");//find all actions

        if (actions.Count > 0)
        {
            
            Debug.Log("found " + actions.Count + " commands");
            for(int i =0; i<actions.Count; i++ )
            {
                Dictionary<int, List<string>> commandsPerCharacterID = new Dictionary<int, List<string>>();
                /*                if (actions[i].ToString() == "")
                                    continue;*/
                //convoActions.Add();
                //(?= !.*\s)(.|\w\s)*(?=:|;)
                MatchCollection ids = Regex.Matches(actions[i].ToString(), @"(?=^!.*\s)(.|\w\s|!\s)*(?=;|:)", RegexOptions.Multiline);
                Debug.Log("found " + ids.Count + " characterIDs in " + actions[i]);
                if (ids.Count > 0)
                {
                    //foreach characterID found in the current action
                    for (int x = 0; x < ids.Count; x++)
                    {
                        var isolateIDNumber = Regex.Match(ids[x].ToString(), @"(?<=^!)(\d)*");
                        Regex iHateSpaces = new Regex(@"\s");
                        var spaceless = iHateSpaces.Replace(isolateIDNumber.ToString(), "");
                        Debug.Log("discoveredCommand for id " + ids[x]);
                        //Find the commands in these lines?
                        //Regex commands = new Regex(@"^@");
                        MatchCollection commandsOnID = Regex.Matches(ids[x].ToString(), @"^@.*", RegexOptions.Multiline);
                        //split from any line starting with @
                        if (commandsOnID.Count > 0)
                        {
                            List<string> _command = new List<string>();
                            for (int y = 0; y < commandsOnID.Count; y++)
                            {
                                Debug.Log("isolated command for id " + spaceless + ":\n" + commandsOnID[y]);
                                _command.Add(commandsOnID[y].ToString());
                                
                            }
                            commandsPerCharacterID.Add(Int32.Parse(spaceless.ToString()), _command);
                        }
                        else
                        {
                            Debug.Log("no commands found");
                        }

                        //characterID, commandList
                        //x should actually be a number computed from the !# line that defines a characterID
                        

                        //Debug.Log(spaceless);
                        //Debug.Log("character id " + Int32.Parse(spaceless.ToString()) + " has recieved new commands");

                        //it's adding each action to any ids. this is not how it should be working!

                        //_command.Clear();
                    }
                    //

                }
                else
                {
                    Debug.LogWarning("No id was found for action " + actions[i]);
                    //act on the player?
            }
                convoActions.Add(commandsPerCharacterID);
                //action one 
            }
        }
        else
        {
            Debug.LogWarning("No matches found for any actions in conversation " + textFieldConvo.name + "! Did you forget to remove an empty line?");
        }
        //each action needs to ^\d.*
        //to find character ids
        //List<string> commands = new List<string>();
        //List<List<string>> charInstructions = new List<List<string>>();

        //Action->CharInstructions->COMMAND

        //convoActions.Add() adds an action
        //convoActions[0].Add() adds a character id
        //convoActions[0][0].Add("blargle") adds a command line

        //Dictionary with charID, command
        //Actions are a list of those^
        
    }
    void LoadConvo()
    {
        //split textFieldConvo
        SplitTextFieldConvo();
        Debug.LogWarning("screen size is " + Screen.width + ", " + Screen.height);
        //create player talk sprite
        var playerSprite = new GameObject();
        playerSprite.name = "PlayerTalkSprite";
        playerSprite.transform.SetParent(GameObject.Find("Canvas").transform);
        playerSprite.AddComponent<Image>();
        playerSprite.GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures/PlayerDefault");
        playerSprite.transform.SetParent(GameObject.Find("CharacterScene").transform);
        playerSprite.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0);
        playerSprite.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0);
        playerSprite.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0);
        playerSprite.GetComponent<RectTransform>().localScale = new Vector2(5, 5);
        sprites.Add(playerSprite);
        positions.Add(new Vector2(0, 0));

        speeds.Add(1000);
        foreach (Character cha in currentConvo.characters)
        {
            var charTalkSprite = new GameObject();
            charTalkSprite.name = cha.Name + "TalkSprite";
            charTalkSprite.transform.SetParent(GameObject.Find("Canvas").transform);
            charTalkSprite.AddComponent<Image>();
            charTalkSprite.GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures/" + cha.Name + "Default");
            charTalkSprite.transform.SetParent(GameObject.Find("CharacterScene").transform);
            charTalkSprite.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0);
            charTalkSprite.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0);
            charTalkSprite.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0);
            charTalkSprite.GetComponent<RectTransform>().localScale = new Vector2(5, 5);
            sprites.Add(charTalkSprite);
            positions.Add(new Vector2(0, 0));
            speeds.Add(1000);
        }
        DisplayConvoAction();
    }
    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < sprites.Count; i++)
        {
            //something about converting como
            var currentRectTransform = sprites[i].GetComponent<RectTransform>();

            var LerpX = Screen.width * positions[i].x;
            var LerpY = Screen.height * positions[i].y;

            //Debug.Log(Screen.width * positions[i].x + " by ")
            //currentRectTransform.anchoredPosition = Vector2.MoveTowards(currentRectTransform.anchoredPosition, new Vector2(LerpX, LerpY), Time.deltaTime * speeds[i]);
            //var runningCommand = currentConvo.actionSteps[convoActionIndex]._commands[i];

            var actorName = sprites[i].name;
            //Debug.Log(actorName + " has a position var of " + currentRectTransform.anchoredPosition + " with a target of " + positions[i]);
            currentRectTransform.anchoredPosition = Vector2.MoveTowards(currentRectTransform.anchoredPosition, new Vector2(LerpX, LerpY), Time.deltaTime * speeds[i]);
        }
    }

    void DisplayConvoAction()
    {
        /*if (currentConvo.actionSteps.Count - 1 < convoActionIndex)
            return;
        for(int i = 0; i< currentConvo.actionSteps[convoActionIndex]._commands.Count; i++)
        {
            var runningCommand = currentConvo.actionSteps[convoActionIndex]._commands[i];
            //var command = runningCommand._commandData;

            //CheckActionType(runningCommand._commandData, i);
            string actorName = "Null";
            Color actorColor = Color.white;
            if(runningCommand._characterID <= 0)
            {
                actorName = player.Name;
                actorColor = player.Color;
            }
            else
            {
                actorName = currentConvo.characters[runningCommand._characterID - 1].Name;
                actorColor = currentConvo.characters[runningCommand._characterID - 1].Color;
            }
            Debug.Log(actorName);
            speaker.text = "<color=#" + ColorUtility.ToHtmlStringRGB(actorColor) + ">" + actorName + "</color>";
        }*/
        if (convoActions.Count - 1 < convoActionIndex)
            return;

        //convoActions.Count is the number of actions in the conversation.

        /*        for (int i = 0; i < convoActions.Count; i++)
                {*/
        //if the 
        //convoActions[i] this is the current action

        //convoActions[i][charID] this is the commands for this one character
        Debug.Log("convoActionIndex is " + convoActionIndex);
        for(int charIndex = 0; charIndex < convoActions[convoActionIndex].Count; charIndex++)
        {
            int charID = convoActions[convoActionIndex].Keys.ElementAt(charIndex);
            Debug.LogWarning("Looping though characters on action. this is character id = " +charID);
            Debug.LogWarning(convoActions[convoActionIndex][charID][0]);
        }


        for(int charLoopIndex = 0; charLoopIndex < convoActions[convoActionIndex].Count; charLoopIndex++)
        {
            int charID = convoActions[convoActionIndex].Keys.ElementAt(charLoopIndex);
            Debug.Log(charID + " is the character ID for the current convoActionIndex");
            Debug.Log("there have been " + charLoopIndex + "/" + convoActions[convoActionIndex].Count + " characters processed this action before this one");
            if (charID <= 0)
            {
                    Debug.Log("character " + player.Name + " has the following commands @ step " + convoActionIndex + ":");
            }
            else
            {
                    Debug.Log("character " + textFieldConvo.characters[charID - 1].Name + " has the following commands @ step " + convoActionIndex + ":");
            }

            //Debug.Log("character " + textFieldConvo.characters[charID].Name + " has the following commands:");
            List<string> commands = convoActions[convoActionIndex][charID];
            for (int commandIndex = 0; commandIndex < commands.Count; commandIndex++)
            {
                    Debug.Log("running command for character "+ charID +":\n" + commands[commandIndex]);
                    CheckActionType(commands[commandIndex], charID);

            }
        }
    }
    void CheckActionType(string command, int actorCharID)
    {
        //SAY
        if (Regex.IsMatch(command, "^@SAY"))
        {
            //Debug.Log("command is SAY");
            var regex = new Regex("^@SAY");
            var commandRemoved = regex.Replace(command, "");
            textBody.text = commandRemoved.ToString();
            string actorName = "Null";
            Color actorColor = Color.white;

            if (actorCharID <= 0)
            {
                actorName = player.Name;
                actorColor = player.Color;
            }
            else
            {
                actorName = textFieldConvo.characters[actorCharID - 1].Name;
                actorColor = textFieldConvo.characters[actorCharID - 1].Color;
            }
            speaker.text = "<color=#" + ColorUtility.ToHtmlStringRGB(actorColor) + ">" + actorName + "</color>";
            Debug.Log("updated speaker to " + speaker.text);
            return;
        }
        //POSITION
        else if (Regex.IsMatch(command, "^@POS"))
        {
            Debug.Log("command is MOVE");
            Vector2 targetPos;
            //targetPos = new Vector2();
            Regex regex = new Regex("^@POS");
            Regex iHateSpaces = new Regex(@"\s");
            var commandSettings = regex.Replace(command, "");
            var spaceless = iHateSpaces.Replace(commandSettings, "");
            var split = Regex.Split(spaceless, ",");
            Debug.Log(commandSettings);
            Debug.Log(split[0]);
            Debug.Log(split[1]);
            Debug.Log(split[2]);
            var charID = actorCharID;
            ChangePosition(charID, new Vector2(float.Parse(split[0], System.Globalization.NumberStyles.AllowDecimalPoint), float.Parse(split[1], System.Globalization.NumberStyles.AllowDecimalPoint)), float.Parse(split[2], System.Globalization.NumberStyles.AllowDecimalPoint));
            targetPos = new Vector2(float.Parse(split[0], System.Globalization.NumberStyles.AllowDecimalPoint), float.Parse(split[1], System.Globalization.NumberStyles.AllowDecimalPoint));
            Debug.Log("@POS character id " + charID + " has new targetpos: " + targetPos + "\nrecieved from command " + command);
            return;
        }
        //ROTATION
        else if (Regex.IsMatch(command, "^@ROT"))
        {
            Debug.Log("command is Rotate");
            return;
        }
        //SCALE
        else if (Regex.IsMatch(command, "^@SCA"))
        {
            Debug.Log("command is Scale");
            return;
        }
        else if (Regex.IsMatch(command, "^@ORD"))
        {
            Debug.Log("command is Order");
            return;
        }
        //DISPLAY
        else if(Regex.IsMatch(command, "^@DIS"))
        {
            Debug.Log("command is Display");
            Regex regex = new Regex("^@DIS");
            var commandSettings = regex.Replace(command, "");
            Regex ihateSpaces = new Regex(" ");
            var trimSpaces = ihateSpaces.Replace(commandSettings, "");
            var charID = actorCharID;
            Debug.Log("trying to display " + trimSpaces);
            sprites[charID].GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures/" + trimSpaces);
            return;
        }
        //CHOICE
        else if (Regex.IsMatch(command, "^@CHO"))
        {
            Debug.Log("command is Choice");
            //OPTION
            //formatting should be something like @CHO @OP 
            //EACH OPTION NEEDS TO END WITH A SPACE!!!
            MatchCollection matches = Regex.Matches(command, @"(?<=@OP )(.*?)(?=&)(?<=&)(.*?)(?=%)");//no function support (?<=@OP )(.*?)(?=%)
            if (matches.Count > 0)
            {
                List<string> options = new List<string>();
                Debug.Log("found " + matches.Count + " options");
                foreach (Match match in matches)
                {
                    CaptureCollection captures = match.Captures;
                    Debug.Log(captures[0].Value);
                    options.Add(match.ToString());
                }
                //An INT should be passed from the dialouge that skips the convoActionIndex
                CreateChoiceOptions(options);
                displayingChoice = true;
            }

            return;
        }
        Debug.LogWarning("no command identified for character id " + actorCharID + " which is step " + convoActionIndex + " in conversation " + currentConvo.name);
    }
    //stat checks?
    public void CreateChoiceOptions(List<string> options)
    {
        for(int i = 0; i< options.Count; i++)
        {
            var split = Regex.Split(options.ElementAt(i), "&&");
            Debug.Log(split[0]);
            Debug.Log(split[1]);
            var button = Instantiate(optionPrefab);
            button.transform.Find("Label").GetComponent<TextMeshProUGUI>().text = split[0];
            button.transform.SetParent(GameObject.Find("MultipleChoice").transform);
            button.GetComponent<ForceConversationStep>().skipTo = Int32.Parse(split[1]);
        }
/*        foreach (string str in options.Keys)
        {
            var button = Instantiate(optionPrefab);
            button.transform.Find("Label").GetComponent<TextMeshProUGUI>().text = str;
            button.transform.SetParent(GameObject.Find("MultipleChoice").transform);
            button.GetComponent<ForceConversationStep>().skipTo = options.Values[];
        }*/
    }
    public void SkipTo(int index, bool triggeredByChoice)
    {
        if (triggeredByChoice)
        {
            displayingChoice = false;
            DestroyAllChoices();
        }

        if (index >= 0)
            convoActionIndex = index;
        else
            convoActionIndex++;
        //DestroyAllButtons
        DisplayConvoAction();
    }
    public void GobackOne()
    {
        if (convoActionIndex <= 0)
            return;
        displayingChoice = false;
        DestroyAllChoices();
        convoActionIndex--;
        //DestroyAllButtons
        DisplayConvoAction();
    }
    public void ChangePosition(int index, Vector2 pos, float speed = 5)
    {
        positions[index] = pos;
        speeds[index] = speed;
        //positions.Values.ElementAt(index) = pos;
    }
    public void DestroyAllChoices()
    {
        foreach(Transform child in GameObject.Find("MultipleChoice").transform.GetComponentInChildren<Transform>())
        {
            Destroy(child.gameObject);
        }
    }

    //for a conversation to end, all characters need to finish their movements. and the current action index thingy needs to be at the end.
}
