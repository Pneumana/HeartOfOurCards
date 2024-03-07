using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalePart : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("LateStart", 0f);
    }
    void LateStart()
    {
        GetComponent<RectTransform>().sizeDelta = transform.parent.GetComponent<RectTransform>().sizeDelta;
    }
}
