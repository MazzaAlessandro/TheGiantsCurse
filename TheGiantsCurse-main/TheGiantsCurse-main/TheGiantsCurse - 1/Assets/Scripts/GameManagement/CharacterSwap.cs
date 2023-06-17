using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
