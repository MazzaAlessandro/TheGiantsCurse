using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    
    [SerializeField] private float shakeMultiplier = 0.3f;

    private float shakeDuration = 0f;
    private float shakeAmount = 0f;
    private float totalDuration;

    private Vector3 originalPos;
    private Transform camTransform;

    // Start is called before the first frame update
    void Awake()
    {
        if (camTransform == null)
            camTransform = GetComponent(typeof(Transform)) as Transform;
    }

    private void OnEnable()
    {
        originalPos = camTransform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if(shakeDuration > 0)
        {
            camTransform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;
            if (shakeDuration > totalDuration / 2)
                shakeAmount += Time.deltaTime * shakeMultiplier;
            else
            {
                shakeAmount -= Time.deltaTime * shakeMultiplier;
                if (shakeAmount < 0)
                    shakeAmount = 0;
            }
            shakeDuration -= Time.deltaTime;
        }
        else
        {
            shakeDuration = 0f;
            camTransform.localPosition = originalPos;
        }
    }

    public void Shake(float duration)
    {
        totalDuration = duration;
        shakeDuration = duration;
    }

    public void SmallShake(float duration, float strenght)
    {
        StartCoroutine(SmallShakeCoroutine(duration, strenght));
    }

    private IEnumerator SmallShakeCoroutine(float duration, float strenght)
    {
        for(float i = 0; i < duration; i += Time.deltaTime)
        {
            camTransform.localPosition = originalPos + Random.insideUnitSphere * strenght;
            yield return null;
        }

        camTransform.localPosition = originalPos;
    }
}
