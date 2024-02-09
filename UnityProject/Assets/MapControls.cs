using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class MapControls : NetworkBehaviour
{
    ArbitraryMapGeneratiion mapGen;
    public TextMeshProUGUI text;
    private void Start()
    {
        mapGen = GameObject.Find("ArbitraryMap").GetComponent<ArbitraryMapGeneratiion>();
        if (isServer)
        {
            CMDChangePickingPlayer();
        }
    }
    private void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            //need to have some method for checking who's turn it is to pick a map.


           /* if (isServer != RunManager.instance.secondPlayerPick && AmbidexterousManager.Instance.PlayerList.Count > 1)
            {
                foreach(Vector3Int pos in mapGen.fogOfWar.cellBounds.allPositionsWithin)
                {
                    if(mapGen.fogOfWar.GetTile(pos) == mapGen.fogBorder)
                    {
                        if (Vector3.Distance(mapGen.fogOfWar.CellToWorld(pos), Camera.main.ScreenToWorldPoint(Input.mousePosition)) < 0.5f)
                        {
                            CMDExplorePosition(Input.mousePosition);
                            break;
                        }
                        Debug.DrawLine(pos, Camera.main.ScreenToWorldPoint(Input.mousePosition));
                    }

                }

            }*/
/*            else if(AmbidexterousManager.Instance.PlayerList.Count ==1)
            {*/
                CMDExplorePosition(Input.mousePosition);
            //}
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

        /*if(AmbidexterousManager.Instance.PlayerList.Count > 1)
        {
            
            if (rm.pickingPlayer == rm.LocalPlayerID)
            {
                rm.pickingPlayer++;
            }
            else
            {
                rm.pickingPlayer--;
            }
            //messages
            if (rm.pickingPlayer == rm.LocalPlayerID)
            {
                    
            }
            else
            {
                if (isServer)
                    text.text = "Click on a room to explore";
                else
                    text.text = "Please wait for the other player to make a selection";
            }
        }
        else
        {
            text.text = "Click on a room to explore";
        }*/



    }
    [Command(requiresAuthority = false)]
    void CMDChangePickingPlayer()
    {
        ChangePickingPlayer();
    }
    [ClientRpc]
    void ChangePickingPlayer()
    {
        var rm = RunManager.instance;

        if (rm.pickingPlayer == 0)
        {
            rm.pickingPlayer = 1;
        }
        else
        {
            rm.pickingPlayer = 0;
        }

        text.text = "Player " + rm.pickingPlayer + " is picking";
    }
}
