using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameOverManagement : MonoBehaviour
{
    public GameObject gameOverScreen;
    public TMP_Text gameOverCauseText;
    public TMP_Text waitText;

    public Button leaveButton;


    public static GameOverManagement instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else if (instance == null)
        {
            instance = this;
        }
    }

    public void Setup(int cause, bool canLeave)
    {
        gameOverScreen.SetActive(true);
        switch (cause)
        {
            case 0:
                gameOverCauseText.text = "YOU DIED";
                break;
            case 1:
                gameOverCauseText.text = "SOMEONE ESCAPED";
                break;
            case 2:
                gameOverCauseText.text = "YOU SURVIVED";
                break;
            case 3:
                gameOverCauseText.text = "PREYS HUNTED";
                break;
            default:
                gameOverCauseText.text = "YOU DIED";
                break;
        }

        if (canLeave)
        {
            leaveButton.gameObject.SetActive(true);
            waitText.gameObject.SetActive(false);
        }
        else
        {
            leaveButton.gameObject.SetActive(false);
            waitText.gameObject.SetActive(true);
        }
    }
}
