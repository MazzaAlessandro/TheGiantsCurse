using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingRockTriggers : MonoBehaviour
{
    [SerializeField] private bool start;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (start)
                FinalTrackManagement.instance.PlayerEnterArea();
            else
                FinalTrackManagement.instance.PlayerExitArea();
        }
    }
}
