using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchAction : MonoBehaviour
{
    [SerializeField] private float slidingDoorSpeed = 1f;

    private float dist;

    private bool isActive;

    private GameObject door;

    private Vector3 destination;

    // Start is called before the first frame update
    void Awake()
    {
        isActive = false;
        door = this.gameObject.transform.GetChild(0).gameObject;
        destination = this.gameObject.transform.GetChild(1).gameObject.transform.position;
    }

    private void FixedUpdate()
    {
        if (isActive && door != null)
        {
            dist = Vector3.Distance(door.transform.position, destination);
            if (dist > 0.1f)
            {
                door.transform.position = Vector3.Lerp(door.transform.position, destination, slidingDoorSpeed * Time.deltaTime);
            }
            else
                Destroy(door.gameObject);
        }
    }

    public void SetActive()
    {
        if (!isActive)
            isActive = true;
    }
}
