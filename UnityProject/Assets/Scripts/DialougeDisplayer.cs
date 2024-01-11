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
        if (currentConvo.actionSteps.Count - 1 < convoActionIndex)
            return;
        for(int i = 0; i< currentConvo.actionSteps[convoActionIndex]._commands.Count; i++)
        {
            var runningCommand = currentConvo.actionSteps[convoActionIndex]._commands[i];
            //var command = runningCommand._commandData;

            CheckActionType(runningCommand._commandData, i);
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
        }
    }
    void CheckActionType(string command, int actionIndexInStep)
    {
        //SAY
        if (Regex.IsMatch(command, "^@SAY"))
        {
            Debug.Log("command is SAY");
            var regex = new Regex("^@SAY");
            var commandRemoved = regex.Replace(command, "");
            textBody.text = commandRemoved.ToString();
            return;
        }
        //POSITION
        else if (Regex.IsMatch(command, "^@POS"))
        {
            Debug.Log("command is MOVE");
            Vector2 targetPos;
            targetPos = new Vector2();
            Regex regex = new Regex("^@POS");
            var commandSettings = regex.Replace(command, "");
            var split = Regex.Split(commandSettings, ",");
            Debug.Log(split[0]);
            Debug.Log(split[1]);
            Debug.Log(split[2]);
            var charID = currentConvo.actionSteps[convoActionIndex]._commands[actionIndexInStep]._characterID;
            ChangePosition(charID, new Vector2(float.Parse(split[0]), float.Parse(split[1])), float.Parse(split[2]));

            Debug.Log(targetPos);
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
            var charID = currentConvo.actionSteps[convoActionIndex]._commands[actionIndexInStep]._characterID;
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
        Debug.LogWarning("no command identified @ " + actionIndexInStep + " which is step " + convoActionIndex + " in conversation " + currentConvo.name);
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
