using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Jobs;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;

public class VNCharacterController : MonoBehaviour
{

    public Character character;
    //define emotion sprites in the character data

    [SerializeField] Image Body;
    [SerializeField] Image EyeMask;
    [SerializeField] Image Iris;
    [SerializeField] Image EyeLines;

    [SerializeField] Image Mouth;

    float blinkTime;
    public Vector3 lookTarget;
    public Vector3 moveTarget;
    public float scaleTarget;
    public float speed;

    bool speaking;

    bool eyeLoadErr;


    public enum EmotionState
    {
        Neutral = default,
        Happy,
        Sad,
        Mad
    }
    //idk what we need for this I imagine we dont want to have too many of them
    public enum Pose
    {
        Default,
        Proud
    }
    public EmotionState currentEmotion = EmotionState.Neutral;
    Vector2 canvas;
    

    // Start is called before the first frame update
    void Start()
    {
        canvas = GameObject.Find("Canvas").GetComponent<CanvasScaler>().referenceResolution;
        changePose();
        var LerpX = canvas.x * moveTarget.x;
        var LerpY = canvas.y * moveTarget.y;
        transform.localPosition = new Vector2(LerpX, LerpY);
    }

    // Update is called once per frame
    void Update()
    {
        var localLook = lookTarget * transform.lossyScale.x;

        transform.localScale = new Vector3(Mathf.MoveTowards(transform.localScale.x, scaleTarget, Time.deltaTime * 6), 1, 1);

        var LerpX = canvas.x * moveTarget.x;
        var LerpY = canvas.y * moveTarget.y;
        transform.localPosition = Vector2.MoveTowards(transform.localPosition, new Vector2(LerpX, LerpY), Time.deltaTime * speed);
        //GetComponent<RectTransform>().anchoredPosition = 

        Iris.transform.localPosition =Vector3.MoveTowards(Iris.transform.localPosition, localLook, Time.deltaTime * 55);
        if (eyeLoadErr != true)
        {
            blinkTime += Time.deltaTime;
            if (blinkTime > 2.5f)
            {
                EyeLines.sprite = character.SquintLines;
                EyeMask.sprite = character.SquintMask;
                //EyeMask.GetComponent<SpriteMask>().sprite = character.SquintMask;
            }
            if (blinkTime > 2.625f)
            {
                EyeLines.sprite = character.ClosedLines;
                EyeMask.enabled = false;
                Iris.enabled = false;
            }
            if (blinkTime > 2.75f)
            {
                EyeMask.enabled = true;
                Iris.enabled = true;

                EyeLines.sprite = character.SquintLines;

                EyeMask.sprite = character.SquintMask;
                //EyeMask.GetComponent<SpriteMask>().sprite = character.SquintMask;
            }
            if (blinkTime > 2.87f)
            {
                blinkTime = 0;
                ChangeEmote(currentEmotion.ToString());
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("start eye roll");
            StartCoroutine(RollEyes());
        }
    }
    IEnumerator RollEyes()
    {
        Debug.Log("start eye roll");
        float step = 0;
        do
        {
            step += Time.deltaTime * 5;
            //float angle = Mathf.Atan2(relative.x, relative.y);
            lookTarget = new Vector3(Mathf.Sin(step) * 10, Mathf.Cos(step) * 10);
            yield return new WaitForSeconds(0);
        }
        while (step < Mathf.PI * 2);
        lookTarget = Vector3.zero;
        yield return null;
    }
    public void WarpToEnd()
    {
        var LerpX = canvas.x * moveTarget.x;
        var LerpY = canvas.y * moveTarget.y;
        transform.localPosition = new Vector2(LerpX, LerpY);
    }

    public void ChangeEmote(string input)
    {
        EmotionState emotion;
        if (!Enum.TryParse<EmotionState>(input, true, out emotion))
        {
            emotion = EmotionState.Neutral;
            Debug.Log("unable to load emotion " + input);
        }

        //try to find the emotion in the character.sprites.Name

        foreach (Character.CharacterSprite cs in character.Sprites)
        {
            if(cs.Name == input)
            {
                EyeLines.sprite = character.OpenLines;
                EyeMask.sprite = character.OpenMask;

                Body.sprite = cs.sprites;
                break;
            }
        }

        
    }
    public void changePose()
    {
        if (character.Iris == null)
        {
            Iris.enabled = false;
            eyeLoadErr = true;
        }
        if(character.ClosedLines == null || character.OpenLines == null || character.SquintLines == null)
        {
            EyeLines.enabled = false;
            eyeLoadErr = true;
        }
        if (character.SquintMask == null || character.OpenMask == null)
        {
            EyeMask.enabled = false;
            eyeLoadErr = true;
        }
        if (character.Default == null)
        {
            Body.enabled = false;
            eyeLoadErr = true;
        }
        Mouth.enabled = false;
        Body.sprite = character.Default;
        Body.transform.localPosition = character.DefaultPoseOffset;
        //Debug.Log(Body.sprite.rect.width + " / " + Body.sprite.rect.height + " : " + Body.sprite.rect);
        Body.transform.localScale = new Vector3((Body.sprite.rect.width/ Body.sprite.rect.height) * 0.5f, 0.5f);
    }
}
