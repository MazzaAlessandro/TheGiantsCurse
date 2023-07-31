using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

//This should be shared between all players, one single FinalTrackManagement in all the lobby
//Either that or this needs to basically be redone
public class FinalTrackManagement: NetworkBehaviour
{
    public static FinalTrackManagement instance;

    [SerializeField] private GameObject[] playersSpawnPoints;
    [SerializeField] public GameObject giantSpawnpoint;

    public Transform giant;

    public List<GameObject> players;
    private List<int> playerCodes = new List<int>();

    private ulong giantClientId;
    private bool alreadySent = false;

    //This struct is needed to send the list on NetCode since NetCode doesn't support Serializable items as parameters
    public struct ListToSend : INetworkSerializeByMemcpy
    {
        public List<int> pCodes;

        public ListToSend(List<int> list)
        {
            this.pCodes = list;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

        /*if (LevelManager.instance.playerToGiant != null)
        {
            //Swap(LevelManager.instance.playerToGiant);
        }

        players = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
        for(int i = 0; i < players.Count; i++)
        {
            players[i].GetComponent<PlayerController>().SetSpawnpoint(playersSpawnPoints[i]);
            playerCodes.Add(players[i].GetComponent<PlayerController>().GetServerCode());
            players[i].GetComponent<PlayerController>().EnterLevel();
        }

        if (IsServer)
        {
            ListToSend list = new ListToSend(playerCodes);
            SyncPlayerCodesClientRpc(list);
        }*/

        
    }

    //this is to assure that everyone has the same playerCodes list
    [ClientRpc]
    private void SyncPlayerCodesClientRpc(ListToSend codesList, ClientRpcParams clientRpcParams = default)
    {
        playerCodes = codesList.pCodes;
    }

    //This disable a specific character and enables the GiantController script for him
    //It's called before the assignment of the player list, so it should not cause any problem
    public void Swap(GameObject character)
    {
        character.SetActive(false);
        giant.GetComponent<GiantController>().enabled = true;
        giant.GetComponent<GiantController>().LocalCameraSetup();
        giant.GetComponent<NetworkObject>().Spawn();
    }

    //Assign a spawnpoint to one of the non-giant players
    public void AssignSpawn(PlayerController player, int spawn)
    {
        player.SetSpawnpoint(playersSpawnPoints[spawn]);
    }

    //==============Handling the Giant Victory==============

    //This is called by the GiantController owner, saves the clientId of that player so the Server can later get it immediately 
    public void AssignGiantCliendId()
    {
        Debug.Log("You are the Giant");
        AssignGiantClientIdServerRpc();
    }

    //assigns the giantClientId only on the server
    [ServerRpc(RequireOwnership = false)]
    private void AssignGiantClientIdServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;

        if (NetworkManager.ConnectedClients.ContainsKey(clientId))
        {
            if (!IsServer)
                return;

            giantClientId = clientId;
        }
    }

    //Called by this when the player list is empty
    public void GiantVictory()
    {
        Debug.Log("The last player died");
        GiantVictoryServerRpc();
    }

    //Gets the GiantClientId saved previously to send ONLY to the GiantController owner the message 
    [ServerRpc(RequireOwnership =false)]
    private void GiantVictoryServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;

        if (NetworkManager.ConnectedClients.ContainsKey(clientId))
        {
            GiantVictoryServerSide();
        }
    }

    //Executes the calls, only on server and only if it wasn't already called
    private void GiantVictoryServerSide()
    {
        if (!IsServer || alreadySent)
            return;

        alreadySent = true;

        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { giantClientId }
            }
        };

        GiantVictoryClientRpc(clientRpcParams);
    }

    //Activates the winning screen for the Giant
    [ClientRpc]
    private void GiantVictoryClientRpc(ClientRpcParams clientRpcParams = default)
    {
        LevelManager.instance.GiantVictory();
    }

    //==============Handling the death of a Player==============

    //When a player dies, it calls this, getting his hashcode (this allows us to only send an int, making communication faster)
    public void PlayerDied(GameObject player)
    {
        int i = players.IndexOf(player);
        PlayerDiedServerRpc(i);
    }

    //The server ensures that the client who sent it exists, then proceeds with the code
    [ServerRpc(RequireOwnership = false)]
    private void PlayerDiedServerRpc(int playerCode, ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;

        if (NetworkManager.ConnectedClients.ContainsKey(clientId))
        {
            PlayerDiedServerSide(playerCode);
        }
    }

    //Only executed on server, calls the ClientRpc method for all clients
    private void PlayerDiedServerSide(int playerCode)
    {
        if (!IsServer)
            return;

        PlayerDiedClientRpc(playerCode);
    }

    //removes the playerHash from the list. If no hashes are left, every player died, so it calls GiantVictory
    [ClientRpc]
    public void PlayerDiedClientRpc(int playerHash)
    {
        playerCodes.Remove(playerHash);

        if (playerCodes.Count == 0)
        {
            //Here I should insert the Giant's win. 
            Debug.Log("Everyone died, the giant should win");
            GiantVictory();
        }
    }

    //==============Handling the escape of a Player==============

    //This needs to be turned into a rpc behaviour, communicating to all other clients that a certain player escaped
    public void PlayerEscaped(GameObject escapedPlayer)
    {
        Debug.Log("Someone escaped");
        PlayerEscapedServerRpc();
    }

    [ServerRpc]
    void PlayerEscapedServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;

        if (NetworkManager.ConnectedClients.ContainsKey(clientId))
        {
            foreach (var client in NetworkManager.ConnectedClients)
            {
                if (client.Value != NetworkManager.ConnectedClients[clientId])
                {
                    PlayerEscapedServerSide(client.Key);
                }
            }
        }
    }

    void PlayerEscapedServerSide(ulong clientId)
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

        PlayerEscapedClientRpc(clientRpcParams);
    }

    [ClientRpc]
    void PlayerEscapedClientRpc(ClientRpcParams clientRpcParams = default)
    {
        LevelManager.instance.Death();
    }

    
}
