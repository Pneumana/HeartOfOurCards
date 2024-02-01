using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MapControls : NetworkBehaviour
{
    ArbitraryMapGeneratiion mapGen;
    private void Start()
    {
        mapGen = GameObject.Find("ArbitraryMap").GetComponent<ArbitraryMapGeneratiion>();
    }
    private void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            CMDExplorePosition(Input.mousePosition);
        }
    }
    [Command(requiresAuthority = false)]
    void CMDExplorePosition(Vector3 pos)
    {
        ClientExplorePosition(pos);
    }
    [ClientRpc]
    void ClientExplorePosition(Vector3 pos)
    {
        mapGen.ClickToExplore(pos);
    }
}
