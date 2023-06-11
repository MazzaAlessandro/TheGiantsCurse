using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitLevel : MonoBehaviour
{
    [SerializeField] private bool isFinalRoom;

    private void Start()
    {
        if (LevelManager.instance.isLastRoom())
            isFinalRoom = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!other.gameObject.GetComponent<PlayerController>().CanExit())
            {
                other.gameObject.GetComponent<PlayerController>().TurnOffExit();
                other.gameObject.GetComponent<PlayerController>().EnterLevel();
                if (isFinalRoom)
                    EndReached(other.gameObject);
                else
                    CompleteLevel();
            }
            
        }
    }

    private void CompleteLevel()
    {
        Debug.Log("The Player has reached the end of the level");
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); 
        LevelManager.instance.NextLevel();
    }

    private void EndReached(GameObject player)
    {
        Debug.Log("The Player has reached the end of the sequence");
        LevelManager.instance.LastRoomFinished(player);
    }
}
