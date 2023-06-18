using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingMenu : MonoBehaviour{

    public Slider volumeSlider;

    private void Start()
    {
        SoundManager.instance.PlayMusic(SoundManager.Phases.MENU);
        volumeSlider.value = SoundManager.instance.GetCurrVolume();
    }

    public void SetVolume(float volume){
        SoundManager.instance.SetMasterVolume(volume);
    }

    public void SetFullscreen(bool isFullscreen){
        Screen.fullScreen = isFullscreen;
    }

    public void Back(){
        SceneManager.LoadScene(0);
    }

}
