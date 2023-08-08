using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableObject : MonoBehaviour
{
    [SerializeField] private GameObject healthPickUpPrefab, cooldownPickUpPrefab;
    [SerializeField] private int health = 20;

    [SerializeField] private AudioClip destroySound;

    private Vector3 originalPos;

    private GameObject healthPickUp, cooldownPickUp;
    // Start is called before the first frame update
    void Start()
    {
        originalPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("Took damage: " + damage + ". Remaining health: " + health);
        StartCoroutine(Shake());
        if(health <= 0)
        {
            ObstacleDestruction();
        }
    }

    private IEnumerator Shake()
    {
        for (float i = 0; i < 0.2f; i += Time.deltaTime)
        {
            transform.position = originalPos + Random.insideUnitSphere * 0.1f;
            yield return null;
        }

        transform.position = originalPos;
    }

    public void ObstacleDestruction()
    {
        SoundManager.instance.PlayEffect(destroySound);
        float dropChance = Random.Range(0, 10);
        Debug.Log("You got: " + dropChance);
        if (dropChance == 0)
        {
            Debug.Log("Spawn health!");
            healthPickUp = Instantiate(healthPickUpPrefab, transform);
            healthPickUp.transform.localPosition = Vector3.zero;
            healthPickUp.transform.SetParent(null);
            healthPickUp.transform.localScale = Vector3.one;
        }
        if (dropChance == 1)
        {
            Debug.Log("Spawn cooldown!");
            cooldownPickUp = Instantiate(cooldownPickUpPrefab, transform);
            cooldownPickUp.transform.localPosition = Vector3.zero;
            cooldownPickUp.transform.SetParent(null);
            cooldownPickUp.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
        }
        Destroy(this.gameObject);
    }
}
