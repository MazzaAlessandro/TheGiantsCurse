using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrillGadget : Gadget
{

    [SerializeField] private float cooldown = 5f;

    private float countdown;

    private bool isReady;
    // Start is called before the first frame update
    void Start()
    {
        countdown = 0f;
        isReady = true;
        Debug.Log("Is Ready: " + isReady);
    }

    // Update is called once per frame
    void Update()
    {
        if(!isReady)
        {
            countdown += Time.deltaTime;
            if (countdown >= cooldown)
            {
                isReady = true;
                Debug.Log("Gadget is ready!");
            }
        }
    }

    public override void GadgetAction()
    {
        Debug.Log("Is Ready: " + isReady);
        if (isReady)
        {
            Debug.Log("Activate Drill Gadget! Cooldown: " + cooldown);
            countdown = 0f;
            isReady = false;
        }
        else
            Debug.Log("Gadget on cooldown");
    }
}
