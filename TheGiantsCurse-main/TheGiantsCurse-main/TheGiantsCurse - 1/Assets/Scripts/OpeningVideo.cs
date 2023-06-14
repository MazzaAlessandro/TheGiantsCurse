using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpeningVideo : MonoBehaviour
{
    [SerializeField]
    private float delayBeforeLoarding = 38f;

<<<<<<< HEAD
=======
    [SerializeField] int followingScene;
    [SerializeField] bool goingToMenu;

>>>>>>> eb6ba56ae2b41e64456d49018cf548fb001206d1
    private float timeEsapsed;

    // Update is called once per frame
    void Update(){
        timeEsapsed += Time.deltaTime;

        if(timeEsapsed > delayBeforeLoarding){
            SceneManager.LoadScene(3);
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
