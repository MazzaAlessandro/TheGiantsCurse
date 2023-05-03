using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickaxeGadget : Gadget
{
    [SerializeField] private float cooldown = 2f;

    private float countdown;

    private bool isReady;
    // Start is called before the first frame update
    void Start()
    {
        countdown = 0f;
        isReady = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isReady)
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
        if (isReady)
        {
            Debug.Log("Activate Pickaxe Gadget! Cooldown: " + cooldown);
            countdown = 0f;
            isReady = false;
        }
        else
            Debug.Log("Gadget on cooldown");
    }
}
