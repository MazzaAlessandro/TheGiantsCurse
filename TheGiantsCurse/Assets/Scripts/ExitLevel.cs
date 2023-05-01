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
            CompleteLevel();
        }
    }

    private void CompleteLevel()
    {
        Debug.Log("The Player has reached the end of the level");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); 
    }
}
