using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicGame : MonoBehaviour{
    
    private void Awake(){
        GameObject[] musicObj = GameObject.FindGameObjectsWithTag("MusicGame");
        if(musicObj.Length > 1){
            Destroy(this.gameObject);
        }
    }

}
