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

        if (movementInput.Equals(Vector3.zero))
        {
            isRunning = false;
            runCounter = 0f;
        }

        if (Input.GetMouseButton(1) && aimingEnabled)
        {
            animator.SetBool("isAiming", true);
            float dir = Vector3.Dot(transform.forward, movementInput);
            Debug.Log(dir);
            animator.SetFloat("dir", dir);
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

    public override IEnumerator StopMovement(float duration)
    {
        movementEnabled = false;
        aimingEnabled = false;
        isRunning = false;
        runCounter = 0f;
        yield return new WaitForSeconds(duration);
        movementEnabled = true;
        aimingEnabled = true;
    }

    public override void Stun(float stunDuration)
    {
        if (canTakeDamage)
        {
            isRunning = false;
            runCounter = 0f;
            base.Stun(stunDuration + 1f);
        }
    }

    public override void TakeDamage(float damage)
    {
        if (canTakeDamage)
        {
            isRunning = false;
            runCounter = 0f;
            base.TakeDamage(damage);
        }
    }

    public override void Shoot()
    {
        if (canTakeDamage)
        {
            isRunning = false;
            runCounter = 0f;
            base.Shoot();
        }
        else
            Debug.Log("You can't shoot arrows underground!");
    }

    public void Dig()
    {
        StartCoroutine(StartDig());
    }

    private IEnumerator StartDig()
    {
        movementEnabled = false;
        float elapsedTime = 0f;
        float time = 0.25f;
        canTakeDamage = false;

        Vector3 start = transform.position;
        Vector3 end = new Vector3(transform.position.x, -1f, transform.position.z);

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
        movementEnabled = true;
        speed = runningSpeed;

        float elapsedTime = 0f;
        float time = 5f;

        while (elapsedTime < time)
        {
            holeInstance.transform.position = new Vector3(transform.position.x, 0, transform.position.z);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        movementEnabled = false;
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
        movementEnabled = true;

        animator.SetBool("isFalling", false);
        Destroy(holeInstance);
    }

}
