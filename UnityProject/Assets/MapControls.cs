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

    public TextMeshProUGUI staminaText;

    public Camera renderTargetCam;
    public float yfactor;
    public Vector3 correction;

    private void Start()
    {
        mapGen = GameObject.Find("ArbitraryMap").GetComponent<ArbitraryMapGeneratiion>();
        if(isServer)
            CMDChangePickingPlayer();
        staminaText.text = "Stamina: " + RunManager.instance.Stamina;
        ForceUpdatePickingPlayerUI();
    }
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            if (RunManager.instance.pickingPlayer != RunManager.instance.LocalPlayerID && AmbidexterousManager.Instance.PlayerList.Count > 1)
            {
                Debug.Log("Local player isnt the picking player, so they can't pick where to go on the map");
                return;

            }


            //RunManager.instance.Stamina--;
            staminaText.text = "Stamina: " + RunManager.instance.Stamina;
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

                        offset.z = 0;
                        
                        offset.x /= scroll.transform.lossyScale.x;
                        //
                        offset.y /= scroll.transform.lossyScale.y;

                        //not 100% sure why it was happening, but the y offset would always be off when cast the the render target camera, so this should fix it
                        if(offset.y > 0.5f)
                            offset.y += yfactor * Mathf.Abs(offset.y - 0.5f);
                        else if(offset.y <= 0.5f)
                            offset.y -= yfactor * Mathf.Abs(offset.y - 0.5f);

                        //offset.y *= yfactor;
                        //Debug.Log(offset);
                        var cast = renderTargetCam.ViewportToWorldPoint(offset);
                        cast.z = 0;

                        var Min = new Vector3(renderTargetCam.transform.position.x - renderTargetCam.orthographicSize, renderTargetCam.transform.position.y - renderTargetCam.orthographicSize);
                        var Max = new Vector3(renderTargetCam.transform.position.x + renderTargetCam.orthographicSize, renderTargetCam.transform.position.y + renderTargetCam.orthographicSize);
                        Debug.Log(offset + ", min = " + Min +", max = " + Max);
                        cast.x = Mathf.Lerp(Min.x, Max.x, offset.x);
                        cast.y = Mathf.Lerp(Min.y, Max.y, offset.y);

                        //cast.x = currentX.x;

                        Debug.Log("castx " + cast.x + " is " + (((cast.x - Min.x)/(Max.x - Min.x)) * 100) + "% of " + Max.x);
                        Debug.Log("casty " + cast.y + " is " + (((cast.y - Min.y) / (Max.y - Min.y)) * 100) + "% of " + Max.y);

                        Debug.DrawLine(cast + Vector3.up, cast - Vector3.up, Color.white, 5);
                        Debug.DrawLine(cast + Vector3.right, cast - Vector3.right, Color.white, 5);

                        //Debug.DrawLine(new Vector3(cast.x, renderTargetCam.transform.position.y + renderTargetCam.orthographicSize), new Vector3(cast.x, renderTargetCam.transform.position.y - renderTargetCam.orthographicSize), Color.cyan, 15);
                        //Debug.DrawLine(new Vector3(renderTargetCam.transform.position.x + renderTargetCam.orthographicSize, cast.y), new Vector3(renderTargetCam.transform.position.x - renderTargetCam.orthographicSize, cast.y), Color.cyan, 15);
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
        var rm = RunManager.instance;

        if (rm.pickingPlayer == 0)
        {
            rm.pickingPlayer = 1;
            Debug.Log("player 2 is now picking");
        }
        else
        {
            rm.pickingPlayer = 0;
            Debug.Log("player 1 is now picking");
        }
        ChangePickingPlayerUI();
    }
    [Command(requiresAuthority = false)]
    void ForceUpdatePickingPlayerUI()
    {
        ChangePickingPlayerUI();
    }

    [ClientRpc]
    void ChangePickingPlayerUI()
    {
       var rm = RunManager.instance;

        /*        if (rm.pickingPlayer == 0)
                {
                    rm.pickingPlayer = 1;
                }
                else
                {
                    rm.pickingPlayer = 0;
                }
        */
        Debug.Log("client rpc to update picking player text");
        text.text = "Player " + rm.pickingPlayer + " is picking";
    }

}
