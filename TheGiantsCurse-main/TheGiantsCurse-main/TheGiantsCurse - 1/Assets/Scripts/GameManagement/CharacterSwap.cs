using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Despite the name, this DOES NOT handle the character swap. The code has been moved to FinalTrackManager to make it easier
public class CharacterSwap : MonoBehaviour
{
    public static CharacterSwap instance;

    public Transform giant;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void Swap(GameObject character)
    {
        character.SetActive(false);
        giant.GetComponent<GiantController>().enabled = true;
    }
}
