using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardEvent : MonoBehaviour
{
    public static HazardEvent instance;

    [SerializeField] private float hazardDuration = 10f;

    private float windForce = 5f;

    private Light globalLight;
    private CameraShake globalCamera;
    private GameObject[] affectedByWind;

    [SerializeField] private AudioClip windSound;

    private Vector3 windDirection;
    
    // Start is called before the first frame update
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        globalLight = GameObject.FindWithTag("GlobalLight").GetComponent<Light>();
        //globalCamera = GameObject.FindWithTag("MainCamera").GetComponent<CameraShake>();
    }

    private void Update()
    {
        if (windDirection != Vector3.zero)
        {
            affectedByWind = GameObject.FindGameObjectsWithTag("Arrow");
            foreach (GameObject arrow in affectedByWind) {
                //Debug.Log(windDirection);
                arrow.GetComponent<Rigidbody>().AddForce(windDirection * 0.1f, ForceMode.Impulse);
            }
        }
    }

    public void PickRandomEvent(int caller)
    {
        Debug.Log("event starts");
        int hazard = Random.Range(0,3);
        //MatchManager.instance.HazardCalled(hazard);
        /*switch (hazard)
        {
            case 0:
                Darkness();
                break;
            case 1:
                Earthquake();
                break;
            case 2:
                Wind();
                break;
        }*/
        MatchManager.instance.Hazard(caller, hazard);
    }

    public void ExecuteHazardEvent(int hazard)
    {
        switch (hazard)
        {
            case 0:
                Darkness();
                break;
            case 1:
                Earthquake();
                break;
            case 2:
                Wind();
                break;
            default:
                Darkness();
                break;
        }
    }

    public void Darkness()
    {
        globalLight = GameObject.FindWithTag("GlobalLight").GetComponent<Light>();
        Debug.Log("darkness starts");
        if (globalLight.intensity > 0.5f)
            StartCoroutine(Helper.Darkness(globalLight, 1, hazardDuration));
    }

    public void Earthquake()
    {
        globalCamera = GameObject.FindWithTag("MainCamera").GetComponent<CameraShake>();
        Debug.Log("shake starts");
        globalCamera.Shake(hazardDuration);
    }

    public void Wind()
    {
        StartCoroutine(WindCoroutine());
    }

    private IEnumerator WindCoroutine()
    {
        windDirection = new Vector3(Random.Range(-1, 2), 0, Random.Range(-1, 2));
        if (windDirection.Equals(Vector3.zero))
            windDirection = Vector3.forward;
        Debug.Log("Start wind with direction: " + windDirection);
        yield return new WaitForSeconds(hazardDuration);
        windDirection = Vector3.zero;
        Debug.Log("Now wind direction: " + windDirection);
    }

    public void SetCamera(GameObject camera)
    {
        globalCamera = camera.GetComponent<CameraShake>();
    }
}
