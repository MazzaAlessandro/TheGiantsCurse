using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetworkActions : NetworkBehaviour
{
    //Just testing if it works
    public void Test(int num)
    {
        Debug.Log($"Sent by: {num}");
        TestServerRpc(num);
    }

    [ServerRpc]
    private void TestServerRpc(int num)
    {
        Debug.Log($"Sent on server by: {num}");
        TestClientRpc(num);
        
    }

    [ClientRpc]
    private void TestClientRpc(int num)
    {
        Debug.Log($"Sent on client by: {num}");
        TestMethod(num);
        
    }

    private void TestMethod(int num)
    {
        Debug.Log($"Executed message sent by: {num}");
    }
}
