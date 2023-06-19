using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedSwitchAction : MonoBehaviour, IInteractable
{

    [SerializeField] private float slidingDoorSpeed = 1f;
    [SerializeField] private float openTime = 5f;

    private float dist;
    private float countdown;

    private bool isActive;
    private bool isOpen;

    private GameObject door;

    [SerializeField] private Animator animator;

    private Vector3 origin;
    private Vector3 destination;

    [SerializeField] private AudioClip tickingSound;

    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
        isActive = false;
        isOpen = false;
        door = this.gameObject.transform.GetChild(0).gameObject;
        origin = this.gameObject.transform.GetChild(0).gameObject.transform.position;
        destination = this.gameObject.transform.GetChild(1).gameObject.transform.position;
    }

    private void FixedUpdate()
    {
        if (isActive && !isOpen)
        {
            dist = Vector3.Distance(door.transform.position, destination);
            if (dist > 0.1f)
            {
                door.transform.position = Vector3.Lerp(door.transform.position, destination, slidingDoorSpeed * Time.deltaTime);
            }
            else
            {
                isOpen = true;
                countdown = 0;
                Debug.Log("Door is open. Will close in: " + openTime);
            }
        }

        if(isActive && isOpen)
        {
            if (countdown < openTime)
            {
                countdown += Time.fixedDeltaTime;
            }
            else
            {
                dist = Vector3.Distance(door.transform.position, origin);
                if (dist > 0.1f)
                {
                    door.transform.position = Vector3.Lerp(door.transform.position, origin, slidingDoorSpeed * Time.deltaTime);
                }
                else
                {
                    isOpen = false;
                    isActive = false;
                    animator.SetTrigger("SwitchOff");
                    Debug.Log("Door is closed");
                }
            }
        }
    }

    public void SetActive()
    {
        if (!isActive)
            isActive = true;
    }
    
    public void Interact()
    {
        if (!isActive)
        {
            isActive = true;
            animator.SetTrigger("SwitchOn");
            SoundManager.instance.PlayMultipleTimes(tickingSound, tickingSound.length, 6);
        }
            
    }
}
