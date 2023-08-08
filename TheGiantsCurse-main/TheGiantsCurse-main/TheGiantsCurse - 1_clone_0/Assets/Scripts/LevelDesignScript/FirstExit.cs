using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstExit : MonoBehaviour
{
    public List<GameObject> nextRoomSpawnpoints = new List<GameObject>();

    private GameObject nextRoom;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!other.gameObject.GetComponent<PlayerController>().CanExit())
            {
                TransitionHandler.instance.CloseAndOpen(2f);
                other.gameObject.GetComponent<PlayerController>().TurnOffExit();
                other.gameObject.GetComponent<PlayerController>().EnterLevel();

                EnterSequence(other.gameObject);
            }

        }
    }

    private void EnterSequence(GameObject player)
    {
        int route = player.GetComponent<PlayerController>().playerCode;
        nextRoom = nextRoomSpawnpoints[route];

        player.GetComponent<PlayerController>().SetSpawnpoint(nextRoom);
        player.GetComponent<PlayerController>().EnterLevel();
    }
}
