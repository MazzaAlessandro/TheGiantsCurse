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

    private List<int> roomsSet = new List<int> { 4, 5, 6, 7, 8, 9, 10};
    private List<int> roomsSequence = new List<int> { 3 };

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
        SceneManager.LoadScene("MatchManager", LoadSceneMode.Additive);
    }

    private void Start()
    {
        RoomsSetup();
        SoundManager.instance.PlayMusic(SoundManager.Phases.PUZZLE);
    }

    private void RoomsSetup()
    {
        int cursor;

        for (int i = 0; i < 4; i++)
        {
            cursor = UnityEngine.Random.Range(0, roomsSet.Count);
            roomsSequence.Add(roomsSet[cursor]);
            roomsSet.RemoveAt(cursor);
        }

        foreach(int x in roomsSequence)
        {
            Debug.Log(x);
        }
    }

    public void RoomsSequenceSet(List<int> sequence)
    {
        roomsSequence = sequence;
    }

    public void FirstLevel()
    {
        StartCoroutine(LoadFirstLevel());
    }

    private IEnumerator LoadFirstLevel()
    {
        SceneManager.LoadScene(roomsSequence[0]);

        yield return new WaitForSeconds(transitionTime);

        circleTransition.OpenBlackScreen();
    }

    public void NextLevel()
    {
        StartCoroutine(LoadLevel());
    }

    private IEnumerator LoadLevel()
    {
        circleTransition.CloseBlackScreen();

        yield return new WaitForSeconds(transitionTime);
        
        if(currentRoom<=roomsSequence.Count)
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
        yield return new WaitForSeconds(2f);

        SoundManager.instance.PlayMusic(SoundManager.Phases.RACING);
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
        currentRoom = roomsSequence.Count;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().EnterLevel();
        yield return new WaitForSeconds(2f);

        circleTransition.OpenBlackScreen();
    }

    public bool isLastRoom()
    {
        return currentRoom == roomsSequence.Count - 1;
    }

    public void Death()
    {
        StartCoroutine(DeathCoroutine());
    }

    private IEnumerator DeathCoroutine()
    {
        circleTransition.CloseBlackScreen();
        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(14);
    }

    public void GiantDefeat()
    {
        StartCoroutine(GiantDefeatCoroutine());
    }

    private IEnumerator GiantDefeatCoroutine()
    {
        circleTransition.CloseBlackScreen();
        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(15);
    }

    public void Victory()
    {
        StartCoroutine(VictoryCoroutine());
    }

    private IEnumerator VictoryCoroutine()
    {
        circleTransition.CloseBlackScreen();
        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(16);
    }

    public void GiantVictory()
    {
        StartCoroutine(GiantVictoryCoroutine());
    }

    private IEnumerator GiantVictoryCoroutine()
    {
        circleTransition.CloseBlackScreen();
        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(17);
    }

    public void Test()
    {
        Debug.Log("You called NMM, this is the client side");
    }
}
