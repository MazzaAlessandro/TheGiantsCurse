using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitLevel : MonoBehaviour
{
    [SerializeField] private bool isFinalRoom;

    [SerializeField] private GameObject nextRoomSpawnpoint;

    private void Awake()
    {
        /*try
        {
            if (LevelManager.instance.IsLastRoom())
                isFinalRoom = true;
        }
        catch (Exception e)
        {
            //Debug.Log("don't break");
        }*/
    }

    public void SetNextRoomSpawn(GameObject nextSpawn)
    {
        nextRoomSpawnpoint = nextSpawn;
    }

    public void MakeFinalRoom()
    {
        isFinalRoom = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!other.gameObject.GetComponent<PlayerController>().CanExit())
            {
                TransitionHandler.instance.CloseAndOpen(2f);
                other.gameObject.GetComponent<PlayerController>().TurnOffExit();
                other.gameObject.GetComponent<PlayerController>().EnterLevel();
                if (isFinalRoom)
                    EndReached(other.gameObject);
                else
                    CompleteLevel(other.gameObject);
            }
            
        }
    }

    private void CompleteLevel(GameObject player)
    {
        Debug.Log("New version of the exit");
        player.GetComponent<PlayerController>().SetSpawnpoint(nextRoomSpawnpoint);
        player.GetComponent<PlayerController>().EnterLevel();
    }

    /*private void CompleteLevel()
    {
        Debug.Log("The Player has reached the end of the level");
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); 
        LevelManager.instance.NextLevel();
    }*/


    //Here it should initiate a sequence that swaps the player with the Giant and moves all others to the final track
    //This is wrong rn, it takes this player to the final track
    private void EndReached(GameObject player)
    {
        Debug.Log("The Player has reached the end of the sequence");
        //wrong version
        //FinalTrackManagement.instance.AssignSpawn(player.GetComponent<PlayerController>(), player.GetComponent<PlayerController>().playerCode);
        //player.GetComponent<PlayerController>().EnterLevel();

        //v1.0
        //LevelManager.instance.LastRoomFinished(player);

        //v2.0
        FinalTrackManagement.instance.Swap(player);
    }
}
