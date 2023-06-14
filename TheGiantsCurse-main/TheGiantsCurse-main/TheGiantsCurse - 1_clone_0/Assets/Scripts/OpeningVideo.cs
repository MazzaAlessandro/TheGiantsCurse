using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpeningVideo : MonoBehaviour
{
    [SerializeField]
    private float delayBeforeLoarding = 38f;

    private float timeEsapsed;

    // Update is called once per frame
    void Update(){
        timeEsapsed += Time.deltaTime;

        if(timeEsapsed > delayBeforeLoarding){
            SceneManager.LoadScene(3);
        }
    }
}
