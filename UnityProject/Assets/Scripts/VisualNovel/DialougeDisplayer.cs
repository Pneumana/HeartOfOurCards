using DeckData;
using Mirror;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using static UnityEditor.Progress;
using static UnityEditor.ShaderData;
using static UnityEngine.InputSystem.Editor.InputActionCodeGenerator;
using static UnityEngine.InputSystem.HID.HID;
using static UnityEngine.Rendering.DebugUI;

public class DialougeDisplayer : MonoBehaviour
{
    public Character[] player = new Character[2];
    //public List<Character> playerList = new List<Character>();

    public TextMeshProUGUI speaker;
    public TextMeshProUGUI textBody;

    public int convoActionIndex;//gets the current ConversationStep in the current conversation

    public Conversation currentConvo;

    public TextFieldConversation textFieldConvo;

    public GameObject optionPrefab;
    public GameObject statCheckPrefab;
    public GameObject mPStatCheckPrefab;

    public List<GameObject> sprites = new List<GameObject>();
    //target screen position normalized, speed
    public List<Vector2> positions = new List<Vector2>();

    public List<float> speeds = new List<float>();

    public bool displayingChoice;

    public int PrimaryPlayer;

    public static DialougeDisplayer instance;

    //maybe use for mouse wheel down do it doesnt go past stuff you haven't seen before.
    int farthestIntoConvo;
    //also use this to stop! animating sprites and positions
    //scrolling up acts as a history system
    //picked choices should be displayed with a different color
    //you cant change your choice after scrolling up


    public GameObject characterControllerPrefab;

    List<Dictionary<int, List<string>>> convoActions = new List<Dictionary<int, List<string>>>();
    List<string> stepNames = new List<string>();

    public struct CharData
    {
        //keeps track of the game objects, rect transform stuff, color, yadda yadd for each Character in the Conversation
        GameObject go;
        Vector2 targetPos;
        float speed;
        int sortOrder;//use CharacterScene.childcount - 1 as the clamp. 

        //sort int
    }
    public struct StatCheckInfo
    {
        public string stat;
        public int requirement;
        public string label;
        public int pass;
        public int fail;
        public StatCheckInfo(string STAT, int REQUIREMENT, string LABEL, int PASS, int FAIL)
        {
            stat = STAT;
            requirement = REQUIREMENT;
            label = LABEL;
            pass = PASS;
            fail = FAIL;
        }
    }

    public struct MPStatCheckInfo
    {
        public string stat1;
        public string stat2;
        public int pass1;
        public int fail1;
        public int pass2;
        public int fail2;
        public string label1;
        public string label2;
        public MPStatCheckInfo(string STAT1, string LABEL1, int PASS1, int FAIL1, string STAT2, string LABEL2, int PASS2, int FAIL2)
        {
            stat1 = STAT1;
            stat2 = STAT2;
            label1 = LABEL1;
            label2 = LABEL2;
            pass1 = PASS1;
            pass2 = PASS2;
            fail1 = FAIL1;
            fail2 = FAIL2;
        }
    }

    [TextArea(20, 20)]
    public string commandsFromConvo;
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
        if(instance==null)
            instance = this;

        //check for override convo from RunManager
        if(RunManager.instance.ForceLoadConvoReference != null)
        {
            LoadNew(RunManager.instance.ForceLoadConvoReference);
            RunManager.instance.ForceLoadConvoReference = null;
            RunManager.instance.ForceLoadConvo = "";
        }
        else if (RunManager.instance.ForceLoadConvo != "")
        {
            LoadNew(Resources.Load<TextFieldConversation>("Conversations/" + RunManager.instance.ForceLoadConvo));
            RunManager.instance.ForceLoadConvo = "";        }
        PrimaryPlayer = RunManager.instance.pickingPlayer;

        LoadConvo();
    }
    void SplitTextFieldConvo()
    {


        //check for 
        //MatchCollection actionLabels = Regex.Matches(textFieldConvo.commandScript, @"(?<=\w\s)(.|\w\s)*(?=;)");//find all actions
        MatchCollection actions = Regex.Matches(textFieldConvo.commandScript, @"(?=..*\n)(:|.|\S\s)*(?<=;)");//find all actions
        commandsFromConvo = textFieldConvo.commandScript;
        if (actions.Count > 0)
        {
            
            Debug.Log("found " + actions.Count + " commands");
            for(int i =0; i<actions.Count; i++ )
            {
                var StepLabel = Regex.Match(actions[i].ToString(), @"^.*");
                if (StepLabel.Success)
                {
                    stepNames.Add(StepLabel.ToString());
                    Debug.Log("action step is " + i + " with name " + StepLabel);
                }
                Dictionary<int, List<string>> commandsPerCharacterID = new Dictionary<int, List<string>>();
                /*                if (actions[i].ToString() == "")
                                    continue;*/
                //convoActions.Add();
                //(?= !.*\s)(.|\w\s)*(?=:|;)
                MatchCollection ids = Regex.Matches(actions[i].ToString(), @"(?=^!.*\s)(.|\w\s|!\s|\?\s|\.\s|&&\s|%\s)*(?=;|:)", RegexOptions.Multiline);
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
    public void LoadConvo()
    {
        Character[] loadorder = player;
        //Invert the character order if it's player 2 picking the dialouge

        if (RunManager.instance.pickingPlayer == 1)
        {
            player[0] = loadorder[1];
            player[1] = loadorder[0];
        }
        //split textFieldConvo
        SplitTextFieldConvo();
        Debug.LogWarning("screen size is " + Screen.width + ", " + Screen.height);
        //create player talk sprite
        /*var playerSprite = Instantiate(characterControllerPrefab);
        playerSprite.name = "PlayerTalkSprite";
        //playerSprite.transform.SetParent(GameObject.Find("Canvas").transform);
        //playerSprite.AddComponent<Image>();
        //playerSprite.GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures/PlayerDefault");
        playerSprite.GetComponent<VNCharacterController>().character = player[0];
        playerSprite.transform.SetParent(GameObject.Find("CharacterScene").transform);
*//*        playerSprite.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0);
        playerSprite.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0);
        playerSprite.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0);
        playerSprite.GetComponent<RectTransform>().localScale = new Vector2(5, 5);*//*
        sprites.Add(playerSprite);
        //positions.Add(new Vector2(-0.5f, 0));

        speeds.Add(1000);*/

        foreach (Character cha in player)
        {
            var charTalkSprite = Instantiate(characterControllerPrefab);
            charTalkSprite.name = cha.Name + "TalkSprite";
            //charTalkSprite.transform.SetParent(GameObject.Find("Canvas").transform);
            //charTalkSprite.AddComponent<Image>();
            //charTalkSprite.GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures/" + cha.Name + "Default");
            charTalkSprite.transform.SetParent(GameObject.Find("CharacterScene").transform);
            charTalkSprite.GetComponent<VNCharacterController>().character = cha;
            /*charTalkSprite.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0);
            charTalkSprite.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0);
            charTalkSprite.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0);
            charTalkSprite.GetComponent<RectTransform>().localScale = new Vector2(5, 5);*/
            sprites.Add(charTalkSprite);
            //positions.Add(new Vector2(-0.5f, 0));
            speeds.Add(1000);
        }

        foreach (Character cha in textFieldConvo.characters)
        {
            var charTalkSprite = Instantiate(characterControllerPrefab);
            charTalkSprite.name = cha.Name + "TalkSprite";
            //charTalkSprite.transform.SetParent(GameObject.Find("Canvas").transform);
            //charTalkSprite.AddComponent<Image>();
            //charTalkSprite.GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures/" + cha.Name + "Default");
            charTalkSprite.transform.SetParent(GameObject.Find("CharacterScene").transform);
            charTalkSprite.GetComponent<VNCharacterController>().character = cha;
            /*charTalkSprite.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0);
            charTalkSprite.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0);
            charTalkSprite.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0);
            charTalkSprite.GetComponent<RectTransform>().localScale = new Vector2(5, 5);*/
            sprites.Add(charTalkSprite);
            //positions.Add(new Vector2(-0.5f, 0));
            speeds.Add(1000);
        }
        DisplayConvoAction();
    }
    // Update is called once per frame
    void Update()
    {
        /*for(int i = 0; i < sprites.Count; i++)
        {
            //something about converting como
            var currentRectTransform = sprites[i].GetComponent<RectTransform>();
            var canvas = GameObject.Find("Canvas").GetComponent<CanvasScaler>().referenceResolution;
            var LerpX = canvas.x * positions[i].x;
            var LerpY = canvas.y * positions[i].y;

            //Debug.Log(Screen.width * positions[i].x + " by ")
            //currentRectTransform.anchoredPosition = Vector2.MoveTowards(currentRectTransform.anchoredPosition, new Vector2(LerpX, LerpY), Time.deltaTime * speeds[i]);
            //var runningCommand = currentConvo.actionSteps[convoActionIndex]._commands[i];

            var actorName = sprites[i].name;
            //Debug.Log(actorName + " has a position var of " + currentRectTransform.anchoredPosition + " with a target of " + positions[i]);
            currentRectTransform.anchoredPosition = Vector2.MoveTowards(currentRectTransform.anchoredPosition, new Vector2(LerpX, LerpY), Time.deltaTime * speeds[i]);

        }*/
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
            if (charID <= 1)
            {
                    Debug.Log("character " + player[charID].Name + " has the following commands @ step " + convoActionIndex + ":");
            }
            else
            {
                    Debug.Log("character " + textFieldConvo.characters[charID - 2].Name + " has the following commands @ step " + convoActionIndex + ":");
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

            string actorName = "Null";
            Color actorColor = Color.white;

            MatchCollection secondPersonPronouns = Regex.Matches(commandRemoved, @"\spnd\d\s");
            MatchCollection possessivePronouns = Regex.Matches(commandRemoved, @"\spss\d\s");
            MatchCollection characterNames = Regex.Matches(commandRemoved, @"\sname\d\s");

            for(int i = 0; i<characterNames.Count; i++)
            {
                Regex currentname = new Regex(characterNames[i].ToString());
                Regex currentCharID = new Regex(@"\sname");
                var intSplit = currentCharID.Replace(characterNames[i].ToString(), "");
                Debug.Log(intSplit);
                int real = Int32.Parse(intSplit.ToString());
                Debug.Log("found character id " + intSplit[i] + " name mention");
                if (real<=1)
                {
                    commandRemoved = currentname.Replace(commandRemoved, " " + player[real].Name + " ");
                    Debug.Log(player[real].Name);
                }
                else
                {
                    commandRemoved = currentname.Replace(commandRemoved.ToString(), " " + textFieldConvo.characters[real-2].Name + " ");
                    Debug.Log(textFieldConvo.characters[real - 2].Name);
                }
                
            }
            for (int i = 0; i < possessivePronouns.Count; i++)
            {
                Regex currentpronoun = new Regex(possessivePronouns[i].ToString());
                Regex currentCharID = new Regex(@"\spss");
                var intSplit = currentCharID.Replace(possessivePronouns[i].ToString(), "");
                Debug.Log(intSplit);
                int real = Int32.Parse(intSplit.ToString());
                Debug.Log("found character id " + intSplit[i] + " name mention");
                if (real <= 1)
                {
                    commandRemoved = currentpronoun.Replace(commandRemoved, " " + player[real].pss + " ");
                    Debug.Log(player[real].pss);
                }
                else
                {
                    commandRemoved = currentpronoun.Replace(commandRemoved.ToString(), " " + textFieldConvo.characters[real - 2].pss + " ");
                    Debug.Log(textFieldConvo.characters[real - 2].pss);
                }

            }
            for (int i = 0; i < secondPersonPronouns.Count; i++)
            {
                Regex currentpronoun = new Regex(secondPersonPronouns[i].ToString());
                Regex currentCharID = new Regex(@"\spnd");
                var intSplit = currentCharID.Replace(secondPersonPronouns[i].ToString(), "");
                Debug.Log(intSplit);
                int real = Int32.Parse(intSplit.ToString());
                Debug.Log("found character id " + intSplit[i] + " name mention");
                if (real == 0)
                {
                    commandRemoved = currentpronoun.Replace(commandRemoved, " " + player[0].pnd + " ");
                    Debug.Log(player[0].pnd);
                }
                else
                {
                    commandRemoved = currentpronoun.Replace(commandRemoved.ToString(), " " + textFieldConvo.characters[real - 1].pnd + " ");
                    Debug.Log(textFieldConvo.characters[real - 1].pnd);
                }
            }
                textBody.text = commandRemoved.ToString();
            //replace pronoun second person pnd1 with he and pronoun possessive pss1 with the pronoun fetched from the character with id X's pronouns

            if (actorCharID <= 1)
            {
                actorName = player[actorCharID].Name;
                actorColor = player[actorCharID].Color;
            }
            else
            {
                actorName = textFieldConvo.characters[actorCharID - 2].Name;
                actorColor = textFieldConvo.characters[actorCharID - 2].Color;
            }
            //maybe add some ability to obscure who's talking, like before they give you their name?
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
            ChangePosition(charID, new Vector2(float.Parse(split[0], CultureInfo.InvariantCulture), float.Parse(split[1], CultureInfo.InvariantCulture)), float.Parse(split[2], CultureInfo.InvariantCulture));
            targetPos = new Vector2(float.Parse(split[0], CultureInfo.InvariantCulture), float.Parse(split[1], CultureInfo.InvariantCulture));
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
            //remove command
            //set player object to be Count of the scene object's children - the command value, clamped to 0 and the number of children
            Regex regex = new Regex("^@ORD");
            var commandSettings = regex.Replace(command, "");
            Regex ihateSpaces = new Regex(" ");
            var trimSpaces = ihateSpaces.Replace(commandSettings, "");
            int number = Int32.Parse(trimSpaces.ToString());
            number = Mathf.Clamp(number, 0, GameObject.Find("CharacterScene").transform.childCount);
            sprites[actorCharID].transform.SetSiblingIndex(number);
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
            //sprites[charID].GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures/" + trimSpaces);

            //this needs to change the sprites[charID].DisplayEmote?

            return;
        }
        //CHOICE
        else if (Regex.IsMatch(command, "^@CHO"))
        {
            //use actorCharID for playerID
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
                CreateChoiceOptions(options, PrimaryPlayer);
                displayingChoice = true;
            }

            return;
        }
        else if(Regex.IsMatch(command, "^@CHE"))
        {
            //check command, checks a player stat
        }
        else if(Regex.IsMatch(command, "^@DepSTAT")) 
        {
            //use actorCharID for playerID
            //this can be used as a jump destination for picking an option, it also redirects to different steps in a pass or fail
            //@STAT 
            //formatting is player ID, STAT, Number
            //@STAT @INT, Requirement, Label, Pass, Fail%
            Debug.Log("depstat command");
            Regex regex = new Regex("^@DepSTAT");
            Regex iHateSpaces = new Regex(@"\s");
            MatchCollection matches = Regex.Matches(command, @"(?<= @)(...)(.*?)(?=%)");
            //Debug.Log();
            if (matches.Count > 0)
            {
                List<StatCheckInfo> options = new List<StatCheckInfo>();
                Debug.Log("found " + matches.Count + " options");
                
                foreach (Match match in matches)
                {
                    var label = Regex.Match(match.ToString(), "(?<=\").*(?=\")");
                    var spaceless = iHateSpaces.Replace(match.ToString(), "");
                    var split = Regex.Split(spaceless.ToString(), @"\|");
                    //List<string> options = new List<StatCheckInfo>();
                    //options.Add();
                    int req = Int32.Parse(split[1]);
                    int[] passFail = new int[2];

                    for(int i = 0; i<passFail.Length; i++)
                    {
                        try
                        {
                            passFail[i] = Int32.Parse(split[3]);
                            Debug.Log("goto is formatted in int");
                        }
                        catch
                        {
                            Debug.Log("goto is formatted in string");
                            int index = stepNames.IndexOf(split[3 + i]);
                            if (index != -1)
                                passFail[i] = index;
                            else
                            {
                                Debug.LogWarning("There is no step in the current conversation named " + split[i]);
                            }
                        }
                    }
                    split[2] = label.ToString();
                    Debug.Log("addedn new option with " + split[0] + ", " + req + ", " + split[2] + ", " + passFail[0] + ", " + passFail[1]);
                    options.Add(new StatCheckInfo(split[0], req, split[2], passFail[0], passFail[1]));

                    /*CaptureCollection captures = match.Captures;
                    Debug.Log(captures[0].Value);
                    options.Add(match.ToString());*/

                }
                CreateStatOptions(options, PrimaryPlayer);
                displayingChoice = true;
            }

            var commandSettings = regex.Replace(command, "");

            //var split = Regex.Split(spaceless, ",");
            /*if (split.Contains(""))
            {
                Debug.LogError("Incorrect Check formatting! @INT|DMG|CON|NRG, Requirement, Label, Pass, Fail%");
                return;
            }
            switch (split[0].ToString())
            {
                case "INT":
                        
                    break;
                case "CON":
                    break;
                case "NRG":
                    break;
                case "DMG":
                    break;
                default:
                    Debug.LogError("No stat detected for stat command!");
                    return;
            }*/
            return;
        }
        else if(Regex.IsMatch(command, "^@HP"))
        {
            //formatted as HP playerID(1, 2, 0 for both) then the modifier to apply in INT
        }
        else if(Regex.IsMatch(command, @"^@END"))
        {
            //formatted as END Scene Name
            //Goes to the scene
            //Needs to wait for current animations to finish, then do a fade to black?

            var split = Regex.Split(command, @" ");
            Debug.Log(split[1].ToString());
            RunManager.instance.TryStartGame(split[1].ToString());
        }
        else if (Regex.IsMatch(command, @"^@INC"))
        {
            //formatted INC PlayerID(optional) Stat Increase

            var split = Regex.Split(command, @" ");
            //Debug.Log(split[1].ToString());

            var TargetPlayer = RunManager.instance.playerStatList[PrimaryPlayer];
            int num = 0;
            if (split.Length == 4)
            {
                Debug.Log("increase command used playerID");
                num = Int32.Parse(split[3]);
                //var _targetStat = TargetPlayer.GetType().GetField(split[2].ToString());
                //TargetPlayer.GetType().GetField(split[2].ToString()).SetValueDirect(__makeref(TargetPlayer), (int)TargetPlayer.GetType().GetField(split[2].ToString()).GetValue(TargetPlayer) + num);
                //Debug.Log(split[2] + " modified to " + (int)TargetPlayer.GetType().GetField(split[2].ToString()).GetValue(TargetPlayer) + " with a change of " + num);
                var usingSecondaryPlayer = Int32.Parse(split[1]);
                if (usingSecondaryPlayer == 0)
                {
                    actorCharID = PrimaryPlayer;
                }
                else
                {
                    if (PrimaryPlayer == 0)
                    {
                        actorCharID = 1;
                    }
                    else
                    {
                        actorCharID = 0;
                    }
                    var stor = player[0];

                    player[0] = player[1];
                    player[1] = stor;
                }

                RunManager.instance.ChangeStat(actorCharID, split[2], num);
            }
            else if (split.Length == 3)
            {
                Debug.Log("increasing a shared stat");
                num = Int32.Parse(split[2]);
                var rm = RunManager.instance;
                var _targetStat = rm.GetType().GetField(split[1].ToString());
                rm.GetType().GetField(split[1].ToString()).SetValueDirect(__makeref(rm), (int)rm.GetType().GetField(split[1].ToString()).GetValue(rm) + num);
                Debug.Log(split[1] + " modified to " + (int)rm.GetType().GetField(split[1].ToString()).GetValue(rm) + " with a change of " + num);
            }






            
            //RunManager.instance.TryStartGame(split[1].ToString());
        }
        else if(Regex.IsMatch(command, "^@LOAD"))
        {
            //formatted LOAD Conversation Name
            //loads a conversation from the Resources directory
            var split = Regex.Split(command, @" ");
            Debug.Log(split[1].ToString());

            //set current text field convo to loaded name
            //LoadNewConvo(currenttext field convo)
            //textFieldConvo = Resources.Load<TextFieldConversation>("/Conversations/" + split[1]);
            LoadNew(Resources.Load<TextFieldConversation>("Conversations/" + split[1].ToString()));
        }
        else if (Regex.IsMatch(command, "(?<=^@STAT)"))
        {
            //formatting is @STAT AutoPass(int) Stat1 "Label" Pass Fail Stat2 "Label" Pass Fail

            //leave pass fail for option 2 blank to use option 1's pass fail for both options

            //
            var RemoveCommand = new Regex(@"^@STAT");
            var removed = RemoveCommand.Replace(command, "");

            var split = Regex.Split(removed, @"\|");
            for(int i = 0; i < split.Length; i++)
            {
                var s = split[i];
                Debug.Log(s + ", " + split[i]);
            }
            Debug.Log("count = " + split.Length);
            var autoPass = Int32.Parse(split[0]);

            var stat1 = split[1].ToString();
            var stat2 = split[5].ToString();

            var stat1Label = split[2].ToString();
            var stat2Label = split[6].ToString();

            var stat1Pass = split[3].ToString();
            var stat1Fail = split[4].ToString();

            List<string> actionIDsToParse = new List<string>();

            string stat2Pass;
            string stat2Fail;

            actionIDsToParse.Add(stat1Pass);
            actionIDsToParse.Add(stat1Fail);
            if (split.Length == 9)
            {
                stat2Pass = split[7].ToString();
                stat2Fail = split[8].ToString();
                //causes index out of bounds if stat press
                actionIDsToParse.Add(stat2Pass);
                actionIDsToParse.Add(stat2Fail);
            }
            List<int> actionIDsParsed = new List<int>();
            for(int i = 0; i< actionIDsToParse.Count; i++)
            {
                try
                {
                    actionIDsParsed.Add(Int32.Parse(actionIDsToParse[i]));
                    Debug.Log("goto is formatted in int");
                }
                catch
                {
                    Debug.Log("goto is formatted in string");
                    int index = stepNames.IndexOf(actionIDsToParse[i]);
                    if (index != -1)
                        actionIDsParsed.Add(index);
                    else
                    {
                        actionIDsParsed.Add(-1);
                        Debug.LogWarning("There is no step in the current conversation named " + actionIDsToParse[i]);
                    }
                }
            }
            Debug.Log(actionIDsParsed.Count + " actions parced");


            MPStatCheckInfo info = new MPStatCheckInfo();
            if (split.Length < 9)
            {
                //use option 1 pass fail states
                info = new MPStatCheckInfo(stat1, stat1Label, actionIDsParsed[0], actionIDsParsed[1], stat2, stat2Label, actionIDsParsed[0], actionIDsParsed[1]);
            }
            else
            {
                info = new MPStatCheckInfo(stat1, stat1Label, actionIDsParsed[0], actionIDsParsed[1], stat2, stat2Label, actionIDsParsed[2], actionIDsParsed[3]);
            }
            displayingChoice = true;
            MonsterPromCreateStatOptions(info, PrimaryPlayer, autoPass);
            //var split = Regex.Split(spaceless, ",");
            /*if (split.Contains(""))
            {
                Debug.LogError("Incorrect Check formatting! @INT|DMG|CON|NRG, Requirement, Label, Pass, Fail%");
                return;
            }
            switch (split[0].ToString())
            {
                case "INT":
                        
                    break;
                case "CON":
                    break;
                case "NRG":
                    break;
                case "DMG":
                    break;
                default:
                    Debug.LogError("No stat detected for stat command!");
                    return;
            }*/
            return;
        }
        else if (Regex.IsMatch(command, "(?<=^@SWAPPLAYER)"))
        {
            if (PrimaryPlayer == 1)
                PrimaryPlayer = 0;
            else
                PrimaryPlayer = 1;
        }
        else if(Regex.IsMatch(command, "(?<=^@CARD)"))
        {
            var split = Regex.Split(command, @" ");
            var load = Resources.Load<CardData>("CardData/" + split[1].ToString());
            if(NetworkServer.connections.Count > 1)
            {
                AmbidexterousManager.Instance.PlayerList[PrimaryPlayer].decks[0].Add(load);
            }
            else
            {
                AmbidexterousManager.Instance.PlayerList[0].decks[PrimaryPlayer].Add(load);
            }
            
        }
        try
        {
            Debug.LogWarning("no command identified for character id " + actorCharID + " which is step " + convoActionIndex + " in conversation " + currentConvo.name);

        }
        catch { }
    }
    //stat checks?
    public void CreateChoiceOptions(List<string> options, int charID)
    {
        for(int i = 0; i< options.Count; i++)
        {
            var split = Regex.Split(options.ElementAt(i), "&&");
            Debug.Log(split[0]);
            Debug.Log(split[1]);
            var button = Instantiate(optionPrefab);
            button.transform.Find("Label").GetComponent<TextMeshProUGUI>().text = split[0];
            button.transform.SetParent(GameObject.Find("MultipleChoice").transform);


            //if parse fails, it's using a string. so use 
            //int index = myList.FindIndex(a => a.Prop == oProp);
            //on the step names list
            var skipToID = 0;
            try
            {
                skipToID = Int32.Parse(split[1]);
                Debug.Log("goto is formatted in int");
            }
            catch
            {
                Debug.Log("goto is formatted in string");
                int index = stepNames.IndexOf(split[1]);
                if(index != -1)
                    skipToID = index;
                else
                {
                    Debug.LogWarning("There is no step in the current conversation named " + split[1]);
                }
            }

            button.GetComponent<ForceConversationStep>().skipTo = skipToID;
            button.GetComponent<ForceConversationStep>().characterID = charID;
        }
/*        foreach (string str in options.Keys)
        {
            var button = Instantiate(optionPrefab);
            button.transform.Find("Label").GetComponent<TextMeshProUGUI>().text = str;
            button.transform.SetParent(GameObject.Find("MultipleChoice").transform);
            button.GetComponent<ForceConversationStep>().skipTo = options.Values[];
        }*/
    }
    public void CreateStatOptions(List<StatCheckInfo> options, int checkedCharacter)
    {
        for (int i = 0; i < options.Count; i++)
        {
            //options[i].stat;

            var button = Instantiate(statCheckPrefab);
            button.transform.Find("Label").GetComponent<TextMeshProUGUI>().text = options[i].label;
            button.transform.SetParent(GameObject.Find("MultipleChoice").transform);


            //if parse fails, it's using a string. so use 
            //int index = myList.FindIndex(a => a.Prop == oProp);
            //on the step names list

            button.GetComponent<VNStatCheck>().targetStat = options[i].stat;
            button.GetComponent<VNStatCheck>().passRequirement = options[i].requirement;
            button.GetComponent<VNStatCheck>().failID = options[i].fail;
            button.GetComponent<VNStatCheck>().passID = options[i].pass;
        }
        /*        foreach (string str in options.Keys)
                {
                    var button = Instantiate(optionPrefab);
                    button.transform.Find("Label").GetComponent<TextMeshProUGUI>().text = str;
                    button.transform.SetParent(GameObject.Find("MultipleChoice").transform);
                    button.GetComponent<ForceConversationStep>().skipTo = options.Values[];
                }*/
    }

    public void MonsterPromCreateStatOptions(MPStatCheckInfo info, int checkedCharacter, int autoPass)
    {
        var parent = GameObject.Find("MultipleChoice").transform;
        var op1 = Instantiate(mPStatCheckPrefab, parent);
        var op2 = Instantiate(mPStatCheckPrefab, parent);

        op1.transform.Find("Label").GetComponent<TextMeshProUGUI>().text = info.label1;
        var op1Brain = op1.GetComponent<MonsterPromStatCheck>();
        op1Brain.characterID = checkedCharacter;
        op1Brain.thisChoiceStat = info.stat1;
        op1Brain.otherChoiceStat = info.stat2;
        op1Brain.passID = info.pass1;
        op1Brain.failID = info.fail1;
        op1Brain.autoPass = autoPass;

        op2.transform.Find("Label").GetComponent<TextMeshProUGUI>().text = info.label2;
        var op2Brain = op2.GetComponent<MonsterPromStatCheck>();
        op2Brain.characterID = checkedCharacter;
        op2Brain.thisChoiceStat = info.stat2;
        op2Brain.otherChoiceStat = info.stat1;
        op2Brain.passID = info.pass2;
        op2Brain.failID = info.fail2;
        op2Brain.autoPass = autoPass;
    }

    public void SkipTo(int index, bool triggeredByChoice)
    {
        Debug.Log("skipping to " + index);
        if (triggeredByChoice)
        {
            displayingChoice = false;
            DestroyAllChoices();
        }

        switch (index)
        {
            case -1:
                //plus one
                break;
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
        sprites[index].GetComponent<VNCharacterController>().moveTarget = pos;
        //positions[index] = pos;
        //speeds[index] = speed;
        //positions.Values.ElementAt(index) = pos;
    }
    public void DestroyAllChoices()
    {
        foreach(Transform child in GameObject.Find("MultipleChoice").transform.GetComponentInChildren<Transform>())
        {
            Destroy(child.gameObject);
        }
    }

    public void LoadNew(TextFieldConversation convo)
    {
        //lalalala
        //need to reset all of the data and then LoadConvo
        while (sprites.Count > 0)
        {
            Destroy(sprites[0]);
            sprites.RemoveAt(0);
        }
        while(positions.Count > 0)
        {
            positions.RemoveAt(0);
        }
        while (speeds.Count > 0)
        {
            speeds.RemoveAt(0);
        }
        while (convoActions.Count > 0)
        {
            convoActions.RemoveAt(0);
        }
        textFieldConvo = convo;
        convoActionIndex=0;
        LoadConvo();
        DisplayConvoAction();
    }

    //for a conversation to end, all characters need to finish their movements. and the current action index thingy needs to be at the end.
}
