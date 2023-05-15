using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeSpawner : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject ropePickup;

    private GameObject ropeInstance;

    public void Interact()
    {
        Debug.Log("You got me!");
        ropeInstance = Instantiate(ropePickup, transform);
        ropeInstance.transform.localPosition = Vector3.zero;
    }
}
