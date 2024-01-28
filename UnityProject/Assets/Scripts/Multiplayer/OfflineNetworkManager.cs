using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OfflineNetworkManager : NetworkManager
{
    public override void OnClientConnect()
    {
        base.OnClientConnect();
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        Debug.Log($"There are {numPlayers} on the server");
        for(int i = 0; i < NetworkServer.connections.Count; i++)
        {
            var player = NetworkServer.connections.ElementAt(i).Value;
            player.identity.GetComponent<GetPlayerID>().RenameOnAll(i, player.identity.netId);
        }
    }
}
