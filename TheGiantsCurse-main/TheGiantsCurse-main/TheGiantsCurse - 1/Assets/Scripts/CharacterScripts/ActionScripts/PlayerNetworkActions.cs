using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetworkActions : NetworkBehaviour
{
    [SerializeField] private float interactRange = 1f;

    [SerializeField] protected Transform arrowSpawnPoint;
    [SerializeField] protected ArrowBehaviour arrowPrefab;
    [SerializeField] private Transform pickupTransform, throwTransform;

    [SerializeField] private float throwSpeed = 20f;

    public Rigidbody pickup;
    private ArrowBehaviour currentArrow;

    public bool holdingItem;

    private void FixedUpdate()
    {
        if (holdingItem)
            pickup.transform.position = pickupTransform.position;
    }

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

    #region interact and pickup

    public void Interact()
    {
        InteractServerRpc();
    }

    [ServerRpc]
    private void InteractServerRpc()
    {
        InteractClientRpc();
    }

    [ClientRpc]
    private void InteractClientRpc()
    {
        LocalInteract();
    }

    private void LocalInteract()
    {
        Ray r = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(r, out RaycastHit hitInfo, interactRange))
        {
            if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj))
            {
                Debug.Log("interact");
                interactObj.Interact();
            }

            if (hitInfo.collider.gameObject.CompareTag("Explosive") || hitInfo.collider.gameObject.CompareTag("Pickup"))
            {
                Pickup(hitInfo.rigidbody);
            }
        }
    }

    //Make the pickup follow the player
    void Pickup(Rigidbody obj)
    {
        pickup = obj;
        pickup.transform.SetParent(null);
        pickup.isKinematic = false;
        pickup.useGravity = false;
        holdingItem = true;
        Debug.Log("Interact with pickup object");
    }

    #endregion

    #region Drop

    public void Drop()
    {
        DropServerRpc();
    }

    [ServerRpc]
    private void DropServerRpc()
    {
        DropClientRpc();
    }

    [ClientRpc]
    private void DropClientRpc()
    {
        LocalDrop();
    }

    void LocalDrop()
    {
        pickup.transform.position = pickupTransform.position;
        pickup.transform.SetParent(null);
        pickup.isKinematic = false;
        pickup.useGravity = true;
        pickup = null;
        holdingItem = false;
    }

    #endregion

    #region Throw

    public void Throw()
    {
        ThrowServerRpc();
    }

    [ServerRpc]
    private void ThrowServerRpc()
    {
        ThrowClientRpc();
    }

    [ClientRpc]
    private void ThrowClientRpc()
    {
        LocalThrow();
    }

    void LocalThrow()
    {
        pickup.transform.position = throwTransform.position;
        pickup.transform.SetParent(null);
        pickup.isKinematic = false;
        pickup.useGravity = false;
        if (pickup.CompareTag("Explosive"))
        {
            pickup.GetComponent<Explosive>().MakeTrigger();
        }
        pickup.AddForce(transform.forward * throwSpeed, ForceMode.Impulse);
        pickup = null;
        holdingItem = false;
    }
    #endregion
}
