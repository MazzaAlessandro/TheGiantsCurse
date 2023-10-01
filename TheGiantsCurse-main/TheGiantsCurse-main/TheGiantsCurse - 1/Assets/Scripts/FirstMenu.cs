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

    /*private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SceneManager.LoadScene(4);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SceneManager.LoadScene(5);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SceneManager.LoadScene(7);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SceneManager.LoadScene(9);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SceneManager.LoadScene(10);
        }
    }*/

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
