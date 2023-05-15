using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public float transitionTime = 1f;

    private int currentRoom = 0;
    private int[] roomsSequence = { 0, 1, 2 };

    [SerializeField] private CircleTransition circleTransition;

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
        StartCoroutine(LoadLevel());
    }

    private IEnumerator LoadLevel()
    {
        circleTransition.CloseBlackScreen();

        yield return new WaitForSeconds(transitionTime);

        currentRoom += 1;
        try
        {
            SceneManager.LoadScene(roomsSequence[currentRoom]);
        }
        catch (IndexOutOfRangeException e)
        {
            SceneManager.LoadScene("FinalTrack");
        }

        yield return new WaitForSeconds(transitionTime);

        circleTransition.OpenBlackScreen();
    }

    public void FallTransition()
    {
        StartCoroutine(Fall());
    }

    private IEnumerator Fall()
    {
        circleTransition.CloseBlackScreen();
        yield return new WaitForSeconds(transitionTime);
        circleTransition.OpenBlackScreen();
    }

    public void LoadFinalLevel()
    {
        StartCoroutine(FinalLevel());
    }

    private IEnumerator FinalLevel()
    {
        HazardEvent.instance.Earthquake();
        yield return new WaitForSeconds(transitionTime);

        circleTransition.CloseBlackScreen();
        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene("FinalTrack");
        yield return new WaitForSeconds(transitionTime);

        circleTransition.OpenBlackScreen();
    }
}
