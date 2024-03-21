using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalePart : MonoBehaviour
{


/*    // Start is called before the first frame update
    void Start()
    {
        Invoke("LateStart", Time.deltaTime);
    }*/
    void Update()
    {
        if(GetComponent<RectTransform>().sizeDelta != transform.parent.GetComponent<RectTransform>().sizeDelta)
            GetComponent<RectTransform>().sizeDelta = transform.parent.GetComponent<RectTransform>().sizeDelta;
    }
}
