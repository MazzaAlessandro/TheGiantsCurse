using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    private int currentRoom = 0;
    private int[] roomsSequence = {0, 1};

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }

    public void RoomsSequenceSet(int[] sequence)
    {
        roomsSequence = sequence;
    }

    public void NextLevel()
    {
        currentRoom += 1;
        try
        {
            SceneManager.LoadScene(roomsSequence[currentRoom]);
        }
        catch (IndexOutOfRangeException e)
        {
            SceneManager.LoadScene("FinalTrack");
        }
    }
}
