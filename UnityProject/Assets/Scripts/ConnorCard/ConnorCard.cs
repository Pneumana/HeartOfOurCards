using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "TSCardGame/Twin Stick Card", order = 1)]
public class ConnorCard :ScriptableObject
{
    public string cardName;
    public List<GameObject> attackPrefab = new List<GameObject>();
    public int energyCost;
    //card stats go here or something.

    //OnPlay Modifiers

    //OnTrigger Mofiers

    //Triggers
    //

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
