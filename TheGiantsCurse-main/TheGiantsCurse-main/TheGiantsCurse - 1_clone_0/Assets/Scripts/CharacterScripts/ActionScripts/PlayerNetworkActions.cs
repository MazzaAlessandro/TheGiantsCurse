using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetworkActions : NetworkBehaviour
{
    [SerializeField] protected Transform arrowSpawnPoint;
    [SerializeField] protected ArrowBehaviour arrowPrefab;
    private ArrowBehaviour currentArrow;

    #region Test structure

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

    #endregion

    #region Shoot Arrow

    public void ShootArrow(float speed, bool roped, bool fire)
    {
        ShootArrowServerRpc(speed, roped, fire);
    }

    [ServerRpc]
    private void ShootArrowServerRpc(float speed, bool roped, bool fire)
    {
        ShootArrowClientRpc(speed, roped, fire);
    }

    [ClientRpc]
    private void ShootArrowClientRpc(float speed, bool roped, bool fire)
    {
        ExecuteShootArrow(speed, roped, fire);
    }

    private void ExecuteShootArrow(float speed, bool roped, bool fire)
    {
        currentArrow = Instantiate(arrowPrefab, arrowSpawnPoint);
        currentArrow.transform.localPosition = Vector3.zero;

        if (roped)
            currentArrow.MakeRoped();
        if (fire)
            currentArrow.MakeFireArrow();

        currentArrow.Shoot(transform.forward * speed);
        currentArrow.SetOwner(this.gameObject);

        currentArrow = null;
    }

    #endregion
}
