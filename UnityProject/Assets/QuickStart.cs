using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickStart : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        if (NetworkServer.active)
        {
            Destroy(gameObject);
        }
        else
        {
            RunManager.instance.playerStatList.Add(new RunManager.PlayerStats(10, 10, 10, 10));
            GetComponent<NetworkManager>().StartHost();
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
