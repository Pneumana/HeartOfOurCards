using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
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
    // Start is called before the first frame update
    void Start()
    {
        //create player talk sprite
        var playerSprite = Instantiate(new GameObject("PlayerTalkSprite"));
        playerSprite.transform.SetParent(GameObject.Find("Canvas").transform);
        playerSprite.AddComponent<Image>();
        playerSprite.GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures/PlayerDefault");
        sprites.Add(playerSprite);
        positions.Add(new Vector2(0, 0));
        speeds.Add(1);
        foreach (Character cha in currentConvo.characters)
        {
            var charTalkSprite = Instantiate(new GameObject(cha.Name + "TalkSprite"));
            charTalkSprite.transform.SetParent(GameObject.Find("Canvas").transform);
            charTalkSprite.AddComponent<Image>();
            charTalkSprite.GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures/" + cha.Name + "Default");
            sprites.Add(charTalkSprite);
            positions.Add(new Vector2(0, 0));
            speeds.Add(1);
        }
        DisplayConvoAction();
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 1; i < sprites.Count; i++)
        {
            //something about converting como
            var currentRectTransform = sprites[i].GetComponent<RectTransform>();
            currentRectTransform.anchoredPosition = Vector2.MoveTowards(currentRectTransform.anchoredPosition, positions[i], Time.deltaTime * speeds[i]);

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
        //DISPLAY
        else if(Regex.IsMatch(command, "^@DIS"))
        {
            Debug.Log("command is Display");
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
    public void SkipTo(int index)
    {
        if (index >= 0)
            convoActionIndex = index;
        else
            convoActionIndex++;
        //DestroyAllButtons
        DisplayConvoAction();
    }
    public void ChangePosition(int index, Vector2 pos, float speed = 5)
    {
        positions[index] = pos;
        speeds[index] = speed;
        //positions.Values.ElementAt(index) = pos;
    }
}
