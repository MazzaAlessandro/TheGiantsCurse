using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchBehaviour : MonoBehaviour
{
    [SerializeField] private bool isLit;
    [SerializeField] private GameObject fire;
    [SerializeField] private AudioClip fireSound;
    // Start is called before the first frame update
    void Awake()
    {
        fire.SetActive(isLit);
    }

    public void LitTorch()
    {
        isLit = true;
        fire.SetActive(true);
        SoundManager.instance.PlayEffect(fireSound);
        Debug.Log("The torch is now lit");
    }

    public bool IsLit()
    {
        return isLit;
    }
}
