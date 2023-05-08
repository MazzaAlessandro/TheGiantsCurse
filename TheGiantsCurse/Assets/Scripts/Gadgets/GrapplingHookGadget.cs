using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHookGadget : Gadget
{
    [SerializeField] private float hookSpeed = 10f;
    [SerializeField] private float cooldown = 3f;

    [SerializeField] private HookBehaviour Hook;

    private HookBehaviour hookInstance;

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
            GetComponentInParent<MoonriseController>().SetMovement(false);
            Debug.Log("Activate Grappling Hook Gadget! Cooldown: " + cooldown);
            hookInstance = Instantiate(Hook, transform);
            hookInstance.transform.localPosition = Vector3.forward;
            hookInstance.Shoot(transform.forward * hookSpeed);
            hookInstance = null;
            countdown = 0f;
            isReady = false;
        }
        else
            Debug.Log("Gadget on cooldown");
    }
}
