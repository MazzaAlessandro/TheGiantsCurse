using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helper
{
    private static Matrix4x4 isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));

    public static Vector3 ToIso(this Vector3 input) => isoMatrix.MultiplyPoint3x4(input); 

    public static IEnumerator FadeLight(Light l, float start, float goal, float fadeTime)
    {
        float t = 0.0f;

        while (t < fadeTime)
        {
            t += Time.deltaTime;

            l.intensity = Mathf.Lerp(start, goal, t / fadeTime);
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    public static IEnumerator Darkness(Light l, float fadeTime, float duration)
    {
        float t = 0.0f;

        while (t < fadeTime)
        {
            t += Time.deltaTime;

            l.intensity = Mathf.Lerp(1, 0, t / fadeTime);
            yield return new WaitForSeconds(Time.deltaTime);
        }

        yield return new WaitForSeconds(duration);

        t = 0.0f;

        while (t < fadeTime)
        {
            t += Time.deltaTime;

            l.intensity = Mathf.Lerp(0, 1, t / fadeTime);
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
}
