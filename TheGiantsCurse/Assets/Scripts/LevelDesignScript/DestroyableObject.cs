using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableObject : MonoBehaviour
{
    [SerializeField] private GameObject healthPickUpPrefab;
    [SerializeField] private int health = 20;

    private GameObject healthPickUp;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if(health <= 0)
        {
            ObstacleDestruction();
        }
    }

    public void ObstacleDestruction()
    {
        healthPickUp = Instantiate(healthPickUpPrefab, transform);
        healthPickUp.transform.localPosition = Vector3.zero;
        healthPickUp.transform.SetParent(null);
        healthPickUp.transform.localScale = Vector3.one;
        Destroy(this.gameObject);
    }
}
