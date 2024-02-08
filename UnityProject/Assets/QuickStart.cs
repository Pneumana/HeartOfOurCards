using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickStart : MonoBehaviour
{
    private void Awake()
    {
        if (NetworkServer.active)
            gameObject.SetActive(false);
        else
        {
            GetComponent<NetworkManager>().StartHost();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
