using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkMatchManager : NetworkBehaviour
{
    public static NetworkMatchManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
        }
    }


    //==============This "Test" structure must be replicated for all set of funcions needed==============

    //The method that gets called by the client, calls the server
    public void Test()
    {
        Debug.Log("You are the one who pressed the button");
        TestServerRpc();
    }

    //The server recieves a call and for all the clients that did not send that call will call a direct message
    [ServerRpc(RequireOwnership = false)]
    void TestServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;

        if (NetworkManager.ConnectedClients.ContainsKey(clientId))
        {
            foreach(var client in NetworkManager.ConnectedClients)
            {
                if(client.Value != NetworkManager.ConnectedClients[clientId])
                {
                    TestServerSide(client.Key);
                }
            }
        }
    }

    //This handles calling the ClientRPC method for one specific client
    private void TestServerSide(ulong clientId)
    {
        if (!IsServer) 
            return;

        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientId }
            }
        };

        TestClientRpc(clientRpcParams);
    }

    //This is called by the server and executes only on the client, it can reference other scripts, expecially LevelManager and HazardEvent
    [ClientRpc]
    void TestClientRpc(ClientRpcParams clientRpcParams = default)
    {
        LevelManager.instance.Test();
    }

    //==============Handling Hazards==============

    //Called by who picks up the Hazard Items
    public void HazardCalled(int hazard)
    {
        Debug.Log("You picked up an Hazard Globe");
        HazardServerRpc(hazard);
    }

    //Alerts the server about the Hazard chosen and calls it for all other clients
    [ServerRpc(RequireOwnership = false)]
    void HazardServerRpc(int hazard, ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;

        if (NetworkManager.ConnectedClients.ContainsKey(clientId))
        {
            foreach (var client in NetworkManager.ConnectedClients)
            {
                if (client.Value != NetworkManager.ConnectedClients[clientId])
                {
                    HazardServerSide(client.Key, hazard);
                }
            }
        }
    }

    //This is needed to send the hazard to a specific client, calls the ClientRpc
    private void HazardServerSide(ulong clientId, int hazard)
    {
        if (!IsServer)
            return;

        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientId }
            }
        };

        HazardClientRpc(hazard, clientRpcParams);
    }

    //Alerts the HazardEvent script that it needs to start one of the hazard coroutines
    [ClientRpc]
    void HazardClientRpc(int hazard, ClientRpcParams clientRpcParams = default)
    {
        HazardEvent.instance.ExecuteHazardEvent(hazard);
    }

    //==============This is the part that handles when someone reaches the end==============

    //Called in LevelManager.LastRoomFinished(), so by the clients that gets to the end first
    public void EndReached()
    {
        Debug.Log("You picked up an Hazard Globe");
        EndReachedServerRpc();
    }

    //Alerts the server, that will then communicate with all other clients about the fact that someone got to the end
    [ServerRpc(RequireOwnership = false)]
    void EndReachedServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;

        if (NetworkManager.ConnectedClients.ContainsKey(clientId))
        {
            foreach (var client in NetworkManager.ConnectedClients)
            {
                if (client.Value != NetworkManager.ConnectedClients[clientId])
                {
                    EndReachedServerSide(client.Key);
                }
            }
        }
    }

    //Sends that message to a specific client
    private void EndReachedServerSide(ulong clientId)
    {
        if (!IsServer)
            return;

        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientId }
            }
        };

        EndReachedClientRpc(clientRpcParams);
    }

    //On the client side it will call LevelManager.LoadFinalLevel()
    [ClientRpc]
    void EndReachedClientRpc(ClientRpcParams clientRpcParams = default)
    {
        LevelManager.instance.LoadFinalLevel();
    }
}
