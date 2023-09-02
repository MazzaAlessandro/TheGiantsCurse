using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MatchManager : NetworkBehaviour
{
    public static MatchManager instance;

    public PlayerController localPlayer;

    public GiantController giant;

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

    public void EndReached(int callerCode)
    {
        Debug.Log("I reached the end!");
        EndReachedServerRpc(callerCode);
    }

    [ServerRpc(RequireOwnership = false)]
    private void EndReachedServerRpc(int callerCode)
    {
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
}
