using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickStart : MonoBehaviour
{
    private void Awake()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        if (NetworkServer.active)
            gameObject.SetActive(false);
        else
        {
            GetComponent<NetworkManager>().StartHost();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
