using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class SettingMenu : MonoBehaviour{

    public AudioMixer mainMixer;

    public void SetVolume(float volume){
        mainMixer.SetFloat("volume", volume);
    }

    public void SetFullscreen(bool isFullscreen){
        Screen.fullScreen = isFullscreen;
    }

    public void Back(){
        SceneManager.LoadScene(0);
    }

}
