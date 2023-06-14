using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpeningVideo : MonoBehaviour
{
    [SerializeField]
    private float delayBeforeLoarding = 38f;

    [SerializeField] int followingScene;
    [SerializeField] bool goingToMenu;

    private float timeEsapsed;

    // Update is called once per frame
    void Update(){
        timeEsapsed += Time.deltaTime;

        if(timeEsapsed > delayBeforeLoarding){
            SceneManager.LoadScene(followingScene);
        }
    }

    private void OnDestroy()
    {
        if (goingToMenu)
        {
            Debug.Log("onDestroy was called");
            if (LevelManager.instance != null)
            {
                Destroy(LevelManager.instance);
            }
            if (HazardEvent.instance != null)
            {
                Destroy(HazardEvent.instance);
            }
        }
    }
}
