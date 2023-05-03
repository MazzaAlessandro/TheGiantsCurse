using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchBehaviour : MonoBehaviour
{
    [SerializeField] private bool isLit;
    [SerializeField] private GameObject fire;
    // Start is called before the first frame update
    void Awake()
    {
        fire.SetActive(isLit);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LitTorch()
    {
        isLit = true;
        fire.SetActive(true);
        Debug.Log("The torch is now lit");
    }

    public bool IsLit()
    {
        return isLit;
    }
}
