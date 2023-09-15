using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FirstMenu : MonoBehaviour{

    [SerializeField] private string startScene = "StartMenu";
    [SerializeField] private string optionsScene = "OptionMenu";

    private void Awake()
    {
        if (ServerManager.instance != null)
            Destroy(ServerManager.instance.gameObject);

        if (HazardEvent.instance != null)
            Destroy(HazardEvent.instance.gameObject);

        if (ClientManager.instance != null)
            Destroy(ClientManager.instance.gameObject);

    }
    private void Start()
    {
        SoundManager.instance.PlayMusic(SoundManager.Phases.MENU);
    }
    public void PlayGame(){
        SceneManager.LoadScene(startScene);
    }

    public void OptionGame(){
        SceneManager.LoadScene(optionsScene);
    }

    public void QuitGame(){
        Application.Quit();
    }

    public void ReturnMainMenu(){
        SceneManager.LoadScene(0);
    }

    public void StartLevel(){
        SceneManager.LoadScene(13);
    }

}
