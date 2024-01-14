using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ConnorCardController : MonoBehaviour
{
    public TextMeshProUGUI label;
    public Canvas myUI;
    // Start is called before the first frame update
    void Start()
    {
        myUI.worldCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
