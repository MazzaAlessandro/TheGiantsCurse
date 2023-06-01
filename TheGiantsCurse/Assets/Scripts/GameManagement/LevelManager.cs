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
    private int[] roomsSequence = { 0, 3, 2, 1 };

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
        
        if(currentRoom<=roomsSequence.Length)
            currentRoom += 1;
        SceneManager.LoadScene(roomsSequence[currentRoom]);
        
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

    //this one is called when a player finishes the dungeon crawl, it should signal the others that he finished and turn him into a giant
    public void LastRoomFinished(GameObject player)
    {
        StartCoroutine(EndReached(player));
    }

    private IEnumerator EndReached(GameObject player)
    {
        HazardEvent.instance.Earthquake();
        yield return new WaitForSeconds(transitionTime);

        circleTransition.CloseBlackScreen();
        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene("FinalTrack");
        Destroy(player);
        yield return new WaitForSeconds(2f);

        circleTransition.OpenBlackScreen();
    }

    //this is called by any player that did not get to the core, it cuts right to the Final Track
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
        currentRoom = roomsSequence.Length;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().EnterLevel();
        yield return new WaitForSeconds(2f);

        circleTransition.OpenBlackScreen();
    }

    public bool isLastRoom()
    {
        return currentRoom == roomsSequence.Length - 1;
    }
}
