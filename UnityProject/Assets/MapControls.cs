using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.InputSystem;

public class MapControls : NetworkBehaviour
{
    ArbitraryMapGeneratiion mapGen;
    public TextMeshProUGUI text;
    public Camera renderTargetCam;

    public Vector3 correction;

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
        if (Input.GetKeyUp(KeyCode.Mouse0))
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
                //need to convert normalized position of where the scroll mesh was clicked to 
                var raycast = Camera.main.ScreenPointToRay(Input.mousePosition);
            Ray ray;
            RaycastHit hit;
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null)
                {

                    var scroll = hit.collider.gameObject;

                    if (scroll.name == "MapScrollQuad")
                    {
                        Debug.DrawLine(hit.point  + scroll.transform.up * 0.1f, hit.point - scroll.transform.up * 0.1f, Color.red, 10);
                        Debug.DrawLine(hit.point + scroll.transform.right * 0.1f, hit.point - scroll.transform.right * 0.1f, Color.red, 10);
                        //need some sort of scale for the max height 

                        //correction is 0.5x 0.5y when scale is one
                        var offsetScale = new Vector3(scroll.transform.lossyScale.x / 2, scroll.transform.lossyScale.y / 2);
                        var offset = ((hit.point + offsetScale) - (scroll.transform.position ));
                        offset.x /= scroll.transform.lossyScale.x;
                        offset.y /= scroll.transform.lossyScale.y;
                        offset.z = 0;

                        Debug.Log(offset.normalized + ", " + offset);
                        var cast = renderTargetCam.ViewportToWorldPoint(offset);
                        cast.z = 0;
                        CMDExplorePosition(cast);
                    }
                }
            }
                //CMDExplorePosition(Input.mousePosition);
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
