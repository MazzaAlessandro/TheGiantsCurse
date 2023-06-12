using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSpawner : MonoBehaviour
{
    [SerializeField] private GameObject fire;
    [SerializeField] private AudioClip fireSound;
    
    private float interval = 0.125f;

    private GameObject fireInstance;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FireSpawn());
    }

    private IEnumerator FireSpawn()
    {
        SoundManager.instance.PlayEffect(fireSound);
        fireInstance = Instantiate(fire, transform);
        fireInstance.transform.SetParent(null);
        Destroy(fireInstance, 1f);
        fireInstance = null;
        yield return new WaitForSeconds(interval);
        StartCoroutine(FireSpawn());
    }
}
