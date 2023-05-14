using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitLevel : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!other.gameObject.GetComponent<PlayerController>().CanExit())
            {
                other.gameObject.GetComponent<PlayerController>().TurnOffExit();
                other.gameObject.GetComponent<PlayerController>().EnterLevel();
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
}
