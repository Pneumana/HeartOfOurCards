using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DSVictoryText : MonoBehaviour
{
    public float time;
    public float speed;
    float waitTime;

    public Image darkSoulsText;
    public Image textBackground;

    private void Start()
    {
        StartCoroutine(Animate());
    }
    IEnumerator Animate()
    {
        Debug.Log("starting animation");
        do
        {
            time = Mathf.MoveTowards(time, 1, speed * Time.deltaTime);
            darkSoulsText.material.SetFloat("_blurScale", time);
            yield return new WaitForSeconds(0);
        } while (time<1);

        yield return null;
    }
    [ContextMenu("Reset Fade")]
    public void ReStart()
    {
        time = 0;
        StartCoroutine(Animate());
    }
    private void Update()
    {
        if(time==1)
            waitTime = Mathf.MoveTowards(waitTime, 1, Time.deltaTime);
        else
        {
            waitTime = 0;
        }
        if (waitTime >= 1)
        {
            waitTime = 0;
            ReStart();
        }
    }
}
