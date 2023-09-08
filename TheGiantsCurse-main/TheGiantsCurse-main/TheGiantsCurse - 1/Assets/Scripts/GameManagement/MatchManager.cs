using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MatchManager : NetworkBehaviour
{
    public static MatchManager instance;

    public PlayerController localPlayer;

    public GiantController giant;

    public List<ulong> clientIDList;

    private int giantPlayerCode;
    public ulong giantClientID;

    public int playerCount;

    private void Awake()
    {
        if(instance!=null && instance != this)
        {
            Destroy(this);
        }
        else if (instance == null)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            clientIDList = ServerManager.instance.GetClientIdList();
            playerCount = ServerManager.instance.clientData.Count;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    

    public void SetLocalPlayer(PlayerController player)
    {
        if (player.enabled)
        {
            localPlayer = player;
        }
    }

    public void ClientDisconnected(ulong clientId)
    {
        if (IsServer)
        {
            if(clientId == giantClientID)
            {
                //all players win
            }

            else if (clientIDList.Contains(clientId))
            {
                clientIDList.Remove(clientId);

                if(clientIDList.Count == 0)
                {
                    //Giant win
                }
            }
        }
    }

    #region Test structure

    //This is the test structure for non linear RPC, where the method is executed by everyone but the caller
    public void Test(int callerCode)
    {
        Debug.Log("I called");
        TestServerRpc(callerCode);
    }

    [ServerRpc(RequireOwnership = false)]
    private void TestServerRpc(int callerCode)
    {
        TestClientRpc(callerCode);
    }

    [ClientRpc]
    private void TestClientRpc(int callerCode)
    {
        if (callerCode != localPlayer.playerCode)
            LocalTest();
    }

    public void LocalTest()
    {
        Debug.Log("I am not the caller");
    }

    #endregion

    #region HazardEvent

    public void Hazard(int callerCode, int hazard)
    {
        HazardServerRpc(callerCode, hazard);
    }

    [ServerRpc(RequireOwnership =false)]
    private void HazardServerRpc(int callerCode, int hazard)
    {
        HazardClientRpc(callerCode, hazard);
    }

    [ClientRpc]
    private void HazardClientRpc(int callerCode, int hazard)
    {
        if (callerCode != localPlayer.playerCode)
            LocalHazard(hazard);
    }

    private void LocalHazard(int hazard)
    {
        HazardEvent.instance.ExecuteHazardEvent(hazard);
    }

    #endregion

    #region Phase shift

    public void EndReached(int callerCode, ulong clientId)
    {
        Debug.Log("I reached the end!");
        EndReachedServerRpc(callerCode, clientId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void EndReachedServerRpc(int callerCode, ulong clientId)
    {
        giantClientID = clientId;
        giantPlayerCode = callerCode;
        clientIDList.Remove(clientId);
        EndReachedClientRpc(callerCode);
    }

    [ClientRpc]
    private void EndReachedClientRpc(int callerCode)
    {
        if (callerCode != localPlayer.playerCode)
            MoveToFinalTrack();
        else
            TurnToGiant();
    }

    private void MoveToFinalTrack()
    {
        Debug.Log("Someone else reached the end");
        localPlayer.MoveToFinalTrack();
    }

    private void TurnToGiant()
    {
        Debug.Log("Turning into Giant now...");
        localPlayer.gameObject.SetActive(false);
        giant.enabled = true;
    }

    #endregion

    #region Win or Lose

    //called by PlayerController on death, it signals to everyone else that he died
    //to prevent the game from breaking if the host disconnects, I could implement a spectator mode
    public void PlayerDeath(ulong clientId)
    {
        PlayerDeathServerRpc(clientId);
    }

    //If all players died, these two should execute the win for the giant
    //Otherwise it just subtracts the dead player from the players list
    [ServerRpc(RequireOwnership = false)]
    private void PlayerDeathServerRpc(ulong clientId)
    {
        if (clientIDList.Contains(clientId))
        {
            clientIDList.Remove(clientId);
        }

        if (clientIDList.Count == 0)
        {
            GiantWinClientRpc(giantPlayerCode);
        }
    }

    [ClientRpc]
    private void GiantWinClientRpc(int giantCallerCode)
    {
        if(localPlayer.playerCode == giantCallerCode)
        {
            //win
            Debug.Log("You have killed all other players. YOU WIN");
        }
        else
        {
            //lose
            Debug.Log("The Giant has killed all players. YOU LOSE");
        }
    }

    //called when a player escapes, it should return him that he won and the others that they lost
    public void PlayerEscaped(int winner)
    {
        PlayerEscapedServerRpc(winner);
    }

    [ServerRpc(RequireOwnership =false)]
    private void PlayerEscapedServerRpc(int winner)
    {
        PlayerEscapedClientRpc(winner);
    }

    [ClientRpc]
    private void PlayerEscapedClientRpc(int winner)
    {
        if(localPlayer.playerCode == winner)
        {
            //win
            Debug.Log("You escaped from the cave. YOU WIN");
        }
        else
        {
            //loose
            Debug.Log("Someone else escaped from the cave. YOU LOSE");
        }
    }

    #endregion
}
