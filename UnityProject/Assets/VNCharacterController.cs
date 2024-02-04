using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    Vector2 canvas;
    

    // Start is called before the first frame update
    void Start()
    {
        canvas = GameObject.Find("Canvas").GetComponent<CanvasScaler>().referenceResolution;
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

        var LerpX = canvas.x * moveTarget.x;
        var LerpY = canvas.y * moveTarget.y;
        transform.localPosition = Vector2.MoveTowards(transform.localPosition, new Vector2(LerpX, LerpY), Time.deltaTime * speed);
        //GetComponent<RectTransform>().anchoredPosition = 

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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("start eye roll");
            StartCoroutine(RollEyes());
        }
    }
    void LateUpdate()
    {
        //check for sprites being null, if null fallback to the producer sprites
        //or hide them i guess lol
        if (Body.sprite == null)
            Body.enabled = false;
        else
            Body.enabled = true;

        if (EyeLines.sprite == null)
            EyeLines.enabled = false;
        else
            EyeLines.enabled = true;

        if (EyeMask.sprite == null)
            EyeMask.enabled = false;
        else
            EyeMask.enabled = true;

        if(Iris.sprite==null)
            Iris.enabled = false;
        else
            Iris.enabled = true;

        //if speaking, set mouth sprite to open
        //if not set mouth to closed by ChangeEmote(currentEmote)
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
        Body.sprite = character.Default;
        Body.transform.localPosition = character.DefaultPoseOffset;
        
    }
}