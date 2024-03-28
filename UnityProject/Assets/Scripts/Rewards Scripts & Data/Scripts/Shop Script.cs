using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using CardActions;
using DeckData;
using Mirror;

public class ShopScript : NetworkBehaviour
{
    public List<CardRewards> availablePacks = new List<CardRewards>();
    public List<CardRewards> generatedPacks = new List<CardRewards>();
    [SerializeField] private List<GameObject> packSpawnLocations;

    public TextMeshProUGUI text;
    private int picking;

    void Start()
    {
        picking = RunManager.instance.pickingPlayer;
        text.text = "Player " + (RunManager.instance.pickingPlayer + 1) + " is picking";

        
        foreach (GameObject spawn in packSpawnLocations)
        {
            int pack = Random.Range(0, availablePacks.Count);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Command(requiresAuthority = false)]
    public void CMDPackSpawning()
    {
        for (int i = 0; i < packSpawnLocations.Count; i++)
        {
            int pack = Random.Range(0, availablePacks.Count);
            PackSpawning(pack, i);
        }
    }

    [ClientRpc]
    public void PackSpawning(int pack, int i)
    {
        var spawn = availablePacks[pack];
        
    }

    [Command(requiresAuthority = false)]
    public void CMDChangePickingPlayer()
    {
        ChangePickingPlayer();
    }

    [ClientRpc]
    void ChangePickingPlayer()
    {
        if (RunManager.instance.pickingPlayer == 0)
        {
            RunManager.instance.pickingPlayer = 1;
        }
        else
        {
            RunManager.instance.pickingPlayer = 0;
        }

        text.text = "Player " + (RunManager.instance.pickingPlayer + 1) + " is picking";
    }

    [Command(requiresAuthority = false)]
    public void CMDLeave()
    {
        Leave();
    }

    [ClientRpc]
    private void Leave()
    {
        RunManager.instance.pickingPlayer = picking;
    }
}
