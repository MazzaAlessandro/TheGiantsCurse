using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasmController : PlayerController
{
    [SerializeField] private float runningSpeed = 10f;
    [SerializeField] private float runDelay = 5f;

    private float runCounter = 0f;

    private bool isRunning = false;
    
    public override void SpeedHandling()
    {
        if (!isRunning)
        {
            if (!movementInput.Equals(Vector3.zero))
            {
                runCounter += Time.deltaTime;
                if (runCounter >= runDelay)
                {
                    isRunning = true;
                    Debug.Log("Start Running!");
                }
                    
            }
            else
            {
                isRunning = false;
                runCounter = 0f;
            }
        }

        if (Input.GetMouseButton(1) && aimingEnabled)
        {
            speed = aimingSpeed;
            Aiming();
        }
        else
        {
            arrowCharge = chargeStart;
            if (isRunning)
                speed = runningSpeed;
            else
                speed = movementSpeed;
            if (movementEnabled)
                RotateLook();
        }
    }

    public override void Stun(float stunDuration)
    {
        isRunning = false;
        base.Stun(stunDuration + 1.5f);
    }

}
