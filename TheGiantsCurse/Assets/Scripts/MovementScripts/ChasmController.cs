using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasmController : PlayerController
{
    [SerializeField] private float runningSpeed = 10f;
    [SerializeField] private float runDelay = 5f;

    [SerializeField] private GameObject hole;

    private GameObject holeInstance;

    private float runCounter = 0f;

    private bool isRunning = false;
    private bool canTakeDamage = true;
    
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
        if (canTakeDamage)
        {
            isRunning = false;
            base.Stun(stunDuration + 1f);
        }
    }

    public override void TakeDamage(float damage)
    {
        if (canTakeDamage)
        {
            base.TakeDamage(damage);
        }
    }

    public override void Shoot()
    {
        if (canTakeDamage)
            base.Shoot();
        else
            Debug.Log("You can't shoot arrows underground!");
    }

    public void Dig()
    {
        StartCoroutine(StartDig());
    }

    private IEnumerator StartDig()
    {
        float elapsedTime = 0f;
        float time = 0.25f;
        canTakeDamage = false;

        Vector3 start = transform.position;
        Vector3 end = new Vector3(transform.position.x, -0.51f, transform.position.z);

        while (elapsedTime < time)
        {
            transform.position = Vector3.Lerp(start, end, elapsedTime / time);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        holeInstance = Instantiate(hole, transform);
        holeInstance.transform.SetParent(null);
        holeInstance.transform.position = new Vector3(transform.position.x, 0, transform.position.z);

        StartCoroutine(Digging());
    }

    private IEnumerator Digging()
    {
        speed = runningSpeed;

        float elapsedTime = 0f;
        float time = 5f;

        while (elapsedTime < time)
        {
            holeInstance.transform.position = new Vector3(transform.position.x, 0, transform.position.z);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        speed = movementSpeed;

        StartCoroutine(EndDig());
    }

    public void ForcedEndDig()
    {
        StopCoroutine(Digging());
        StartCoroutine(EndDig());
    }

    private IEnumerator EndDig()
    {
        float elapsedTime = 0f;
        float time = 0.25f;

        Vector3 start = transform.position;
        Vector3 end = new Vector3(transform.position.x, 0.5f, transform.position.z);

        while (elapsedTime < time)
        {
            transform.position = Vector3.Lerp(start, end, elapsedTime / time);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        canTakeDamage = true;

        Destroy(holeInstance);
    }

}
