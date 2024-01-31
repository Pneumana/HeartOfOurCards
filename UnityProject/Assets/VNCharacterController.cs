using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    public enum EmotionState
    {
        Neutral,
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

    // Start is called before the first frame update
    void Start()
    {
        changePose();
    }

    // Update is called once per frame
    void Update()
    {
        /*        var dist = Vector3.Distance(Input.mousePosition, EyeMask.transform.position) / 75;
                dist = Mathf.Clamp(dist, -14.2f, 14.2f);
                //var angle = Input.mousePosition - EyeMask.transform.position;
                var relative = Iris.transform.InverseTransformPoint(Input.mousePosition);
                float angle = Mathf.Atan2(relative.x, relative.y);
                //lookTarget = new Vector3(Mathf.Sin(angle) * dist, Mathf.Cos(angle) * dist);*/
        var localLook = lookTarget * transform.lossyScale.x;

        transform.localScale = new Vector3(Mathf.MoveTowards(transform.localScale.x, scaleTarget, Time.deltaTime * 6), 1, 1);

        Iris.transform.localPosition =Vector3.MoveTowards(Iris.transform.localPosition, localLook, Time.deltaTime * 55);
        blinkTime += Time.deltaTime;
        if(blinkTime > 2.5f)
        {
            EyeLines.sprite = character.SquintLines;
            EyeMask.sprite = character.SquintMask;
            //EyeMask.GetComponent<SpriteMask>().sprite = character.SquintMask;
        }
        if(blinkTime > 2.625f)
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
            ChangeEmote(currentEmotion);
        }
    }


    public void ChangeEmote(EmotionState emotion)
    {
        switch (emotion)
        {
            case EmotionState.Neutral:
                //neutral face
                //neutral eyes
                EyeLines.sprite = character.OpenLines;
                EyeMask.sprite = character.OpenMask;
                //EyeMask.GetComponent<SpriteMask>().sprite = character.OpenMask;
                break;
            case EmotionState.Happy:
                //happy face

                //happy eyes
                break;
            case EmotionState.Sad:
                //frown

                //sad eyes
                break;
            case EmotionState.Mad:
                //frown

                //angry eyes


                break;
        }
    }
    public void changePose()
    {
        Body.transform.localPosition = character.DefaultPoseOffset;
    }
}
