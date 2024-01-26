using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ConnorCardController : MonoBehaviour
{
    public TextMeshPro label;
    public TextMeshPro energyCost;
    //public Canvas myUI;

    public ConnorCard card;

    // Start is called before the first frame update
    void Start()
    {
        //myUI.worldCamera = Camera.main;
        label.text = card.cardName;
        energyCost.text = "Energy Cost: " + card.energyCost;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
